//
//  ServerGame.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
//
//  Copyright (c) 2013 Jesse
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using Lidgren.Network;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using GREATLib;
using GREATLib.Network;
using GREATLib.Entities;
using GREATLib.Entities.Champions;
using GREATServer.Network;
using GREATLib.Entities.Spells;
using GREATLib.Physics;
using System.IO;
using GREATLib.World.Tiles;
using GREATLib.Entities.Structures;

namespace GREATServer
{
	/// <summary>
	/// Represents a game on the server.
	/// </summary>
    public sealed class ServerGame
    {
		static readonly TimeSpan STORE_HISTORY_INTERVAL = GameMatch.STATE_UPDATE_INTERVAL;
		static readonly TimeSpan HISTORY_MAX_TIME_KEPT = TimeSpan.FromSeconds(1.0);
		const float MAX_TOLERATED_OFF_DISTANCE = 10f;
		const double MAX_TIME_AHEAD = 0.01f;

		static readonly TimeSpan MIN_TIME_BETWEEN_ACTIONS = TimeSpan.FromMilliseconds(10.0);

		Random random = new Random();

		NetServer NetServer { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }
		List<LinearSpell> ActiveSpells { get; set; }

		GameMatch Match { get; set; }
		/// <summary>
		/// State history of the game, used to simulate a player's action when it happened
		/// on the client's machine.
		/// </summary>
		SnapshotHistory<MatchState> StateHistory { get; set; }

		/// <summary>
		/// Time since the last state update (position corrections) was sent.
		/// </summary>
		double TimeSinceLastCorrection { get; set; }
		double TimeSinceLastGameHistory { get; set; }

		/// <summary>
		/// A list of events that must be notified to the clients (e.g. a player died, shot a spell, etc.)
		/// </summary>
		List<KeyValuePair<ServerCommand, Action<NetBuffer>>> RemarkableEvents { get; set; }


        public ServerGame(NetServer server)
        {
			NetServer = server;
			Clients = new Dictionary<NetConnection, ServerClient>();
			ActiveSpells = new List<LinearSpell>();

			StateHistory = new SnapshotHistory<MatchState>(HISTORY_MAX_TIME_KEPT);
			Match = new GameMatch(MapLoader.MAIN_MAP_PATH);

			RemarkableEvents = new List<KeyValuePair<ServerCommand, Action<NetBuffer>>>();

			TimeSinceLastCorrection = 0.0;
			TimeSinceLastGameHistory = 0.0;
        }

		/// <summary>
		/// Sends a player command to a client.
		/// </summary>
		/// <param name="fillMessage">The function to call to fill the message with the command</param>
		void SendCommand(NetConnection connection, ServerCommand command, NetDeliveryMethod method,
		                 Action<NetOutgoingMessage> fillMessage)
		{
			NetOutgoingMessage msg = NetServer.CreateMessage();
			msg.Write((byte)command);

			fillMessage(msg);

			NetServer.SendMessage(msg, connection, method);
		}

		public void Update(double deltaTime)
		{
			// The server-side loop of the game

			// Handle actions. We check for recently received player actions
			// and apply them server-side.
			HandleActions();

			// Update logic. We update the actual game logic.
			UpdateLogic(deltaTime);

			// Send corrections. We regularly send the state changes of the entities to
			// other clients.
			SendStateChanges(deltaTime);

			// Store the current game state in our history to redo certain player actions.
			StoreGameState(deltaTime);
		}

		void StoreGameState(double dt)
		{
			if (TimeSinceLastGameHistory >= STORE_HISTORY_INTERVAL.TotalSeconds) {
				StateHistory.AddSnapshot(Match.CurrentState.Clone() as MatchState, Server.Instance.GetTime().TotalSeconds);
				TimeSinceLastGameHistory = 0.0;
			}
			TimeSinceLastGameHistory += dt;
		}

		/// <summary>
		/// Sends the state deltas to the clients.
		/// </summary>
		void SendStateChanges(double dt)
		{
			if (TimeSinceLastCorrection >= GameMatch.STATE_UPDATE_INTERVAL.TotalSeconds) {
				foreach (NetConnection connection in Clients.Keys) {
					SendCommand(
						connection,
						ServerCommand.StateUpdate,
						RemarkableEvents.Count == 0 ? NetDeliveryMethod.UnreliableSequenced : NetDeliveryMethod.ReliableUnordered,
						(msg) => FillStateUpdateMessage(msg, connection));
				}
				RemarkableEvents.Clear();
				TimeSinceLastCorrection = 0.0;
			}
			TimeSinceLastCorrection += dt;
		}

		/// <summary>
		/// Fills a message with state update information.
		/// </summary>
		void FillStateUpdateMessage(NetBuffer msg, NetConnection playerConnection)
		{
			Debug.Assert(Clients.ContainsKey(playerConnection));

			ulong lac = Clients[playerConnection].LastAcknowledgedActionID;
			double time = Server.Instance.GetTime().TotalSeconds;

			float vx = Clients[playerConnection].Champion.Velocity.X;
			float vy = Clients[playerConnection].Champion.Velocity.Y;

			byte nbClients = (byte)Clients.Count;

			msg.Write(lac);
			msg.Write(time);
			msg.Write(vx);
			msg.Write(vy);
			msg.Write(nbClients);
			foreach (NetConnection connection in Clients.Keys) {
				ServerClient client = Clients[connection];

				ulong id = client.Champion.ID;
				float x = client.Champion.Position.X;
				float y = client.Champion.Position.Y;
				byte anim = (byte)client.Champion.Animation;
				bool facingLeft = client.Champion.FacingLeft;

				msg.Write(id);
				msg.Write(x);
				msg.Write(y);
				msg.Write(anim);
				msg.Write(facingLeft);
			}
			foreach (var pair in RemarkableEvents) {
				byte command = (byte)pair.Key;
				msg.Write(command);
				pair.Value(msg);
			}
		}

		/// <summary>
		/// Checks for new player actions and applies them to the current game logic.
		/// </summary>
		void HandleActions()
		{
			foreach (ServerClient client in Clients.Values) {
				if (client.ActionsPackage.Count > 0) {
					foreach (PlayerAction action in client.ActionsPackage) {
						client.AnimData.Reset();

						if (IsMovementAction(action.Type)) {
							if (client.ChampStats.Alive)
								HandleMovementAction(client.Champion.ID, action);
						} else {
							HandleAction(client, action);
						}
						client.LastAcknowledgedActionID = Math.Max(client.LastAcknowledgedActionID, action.ID);

						UpdateAnimationDataFromAction(client.AnimData, action.Type);
					}

					client.ActionsPackage.Clear();
				}
			}
		}

		static bool IsMovementAction(PlayerActionType action)
		{
			switch (action) {
				case PlayerActionType.MoveLeft:
				case PlayerActionType.MoveRight:
				case PlayerActionType.Jump:
					return true;

				default:
					return false;
			}
		}

		static void UpdateAnimationDataFromAction(ChampionAnimData anim, PlayerActionType action)
		{
			switch (action) {
				case PlayerActionType.Idle:
					anim.Idle = true;
					break;

				case PlayerActionType.MoveLeft:
					--anim.Movement;
					break;

				case PlayerActionType.MoveRight:
					++anim.Movement;
					break;

				default: break;
			}
		}

		void HandleAction(ServerClient client, PlayerAction action)
		{
			if (ActionTypeHelper.IsSpell(action.Type)) {
				var spell = ChampionTypesHelper.GetSpellFromAction(client.Champion.Type, action.Type);
				if (client.ChampStats.Alive &&
				    !client.ChampStats.IsOnCooldown(spell)) { // we're not dead and the spell is not on cooldown

					CastSpell(client.Champion, action);
					client.ChampStats.UsedSpell(spell);
				}
			} else if (action.Type != PlayerActionType.Idle) {
				ILogger.Log("Unknown player action type: " + action.Type);
			}
		}

		void CastSpell(ICharacter champ, PlayerAction action)
		{
			Debug.Assert(action.Target != null);

			champ.FacingLeft = action.Target.X < champ.Position.X + champ.CollisionWidth / 2f;

			LinearSpell spell = new LinearSpell(
				IDGenerator.GenerateID(),
				champ,
				champ.GetHandsPosition(),
				action.Target ?? Vec2.Zero,
				ChampionTypesHelper.GetSpellFromAction(champ.Type, action.Type));

			Match.CurrentState.AddEntity(spell);
			ActiveSpells.Add(spell);

			float castTime = (float)Server.Instance.GetTime().TotalSeconds;
			LinearSpell copy = (LinearSpell)spell.Clone();

			AddRemarkableEvent(ServerCommand.SpellCast,
				(NetBuffer msg) => {
					ulong id = copy.ID;
					byte type = (byte)copy.Type;
					float time = castTime;
					float px = copy.Position.X;
					float py = copy.Position.Y;
					float vx = copy.Velocity.X;
					float vy = copy.Velocity.Y;
					float cd = (float)copy.Info.Cooldown.TotalSeconds;
					float range = copy.Info.Range;
					float width = copy.CollisionWidth;

					msg.Write(id);
					msg.Write(type);
					msg.Write(time);
					msg.Write(px);
					msg.Write(py);
					msg.Write(vx);
					msg.Write(vy);
					msg.Write(cd);
					msg.Write(range);
					msg.Write(width);
			});
		}

		void AddRemarkableEvent(ServerCommand command, Action<NetBuffer> action)
		{
			RemarkableEvents.Add(Utilities.MakePair(command, action));
		}

		void HandleMovementAction(ulong id, PlayerAction action)
		{
			double now = Server.Instance.GetTime().TotalSeconds;
			double time = action.Time;

			// Make sure we're not using weird times
			time = ValidateActionTime(action, now);

			// Go to the given action time if we have a state history
			if (!StateHistory.IsEmpty()) {
				// Go to the game snapshot before the action that we're simulating
				KeyValuePair<double, MatchState> stateBefore = StateHistory.GetSnapshotBefore(time);
				KeyValuePair<double, MatchState> state = new KeyValuePair<double, MatchState>(
					stateBefore.Key,
					stateBefore.Value.Clone() as MatchState);

				// Simulate from our previous snapshot to our current action to be up-to-date
				if (state.Value.ContainsEntity(id)) {
					var player = (ICharacter)state.Value.GetEntity(id);
					float deltaT = (float)(time - state.Key);
					if (deltaT > 0f) { // if we have something to simulate...
						state.Value.ApplyPhysicsUpdate(id, deltaT);
					}

					// Make sure we're not using hacked positions
					player.Position = ValidateActionPosition(player, action);

					// Actually execute the action on our currently simulated state
					DoAction(state.Value, player, action);

					// Store our intermediate state at the action time.
					state = StateHistory.AddSnapshot(state.Value, time);
				}



				// Resimulate all the states up to now so that they are affected
				// by the player's action.
				var nextState = StateHistory.GetNext(state);
				while (nextState.HasValue) {
					// get how much time we have to simulate for next state
					float timeUntilNextState = (float)(nextState.Value.Key - time);
					Debug.Assert(timeUntilNextState >= 0f);

					// simulate the next state
					if (nextState.Value.Value.ContainsEntity(id)) {
						nextState.Value.Value.GetEntity(id).Clone(state.Value.GetEntity(id));
						if (timeUntilNextState > 0f) {
							nextState.Value.Value.ApplyPhysicsUpdate(id, timeUntilNextState);
						}
					}

					// switch to the next state
					state = nextState.Value;
                    time = state.Key;
                    nextState = StateHistory.GetNext(state);
				}

				// Modify our current game state to apply our simulation modifications.
				var last = StateHistory.GetLast();
				if (Match.CurrentState.ContainsEntity(id) && last.Value.ContainsEntity(id)) {
					Match.CurrentState.GetEntity(id).Clone(last.Value.GetEntity(id));
				}
			}
		}

		static double ValidateActionTime(PlayerAction action, double currentTime)
		{
			double time = action.Time;

			// action time is too old? might be a hacker/extreme lag. Log it, keep it but clamp it
			double oldestAcceptedTime = currentTime - HISTORY_MAX_TIME_KEPT.TotalSeconds;
			if (action.Time < oldestAcceptedTime) {
				time = oldestAcceptedTime;
				ILogger.Log(String.Format("Action {0} seems a bit late. Accepting it, but might be a hacker/extreme lag. Given time: {1}, server time: {2}",
				                          action.ID, action.Time, currentTime), 
				            LogPriority.Warning);
			}

			// action time seems too recent? might be a hacker/time error. Log it, keep it but clamp it
			if (action.Time > currentTime + MAX_TIME_AHEAD) {
				time = currentTime;
				ILogger.Log(String.Format("Action {0} seems a bit too new. Accepting it, but might be a hacker/time error. Given time: {1}, server time: {2}",
				                          action.ID, action.Time, currentTime),
				            LogPriority.Warning);
			}

			return time;
		}

		static Vec2 ValidateActionPosition(IEntity player, PlayerAction action)
		{
			Vec2 position = action.Position;
			// If the position provided by the client seems legit, we take it. Otherwise, we ignore it
			// and log it (might be a hacker).
			if (Vec2.DistanceSquared(player.Position, position) >= MAX_TOLERATED_OFF_DISTANCE * MAX_TOLERATED_OFF_DISTANCE) {
				position = player.Position;
			}

			return position;
		}

		static void DoAction(MatchState match, ICharacter champion, PlayerAction action)
		{
			switch (action.Type) {
				case PlayerActionType.MoveLeft:
					match.Move(champion.ID, HorizontalDirection.Left);
					champion.FacingLeft = true;
					break;
				case PlayerActionType.MoveRight:
					match.Move(champion.ID, HorizontalDirection.Right);
					champion.FacingLeft = false;
					break;

				case PlayerActionType.Jump:
					match.Jump(champion.ID);
					break;

					// Ignore the actions that are not related to movement
				case PlayerActionType.Idle:
				case PlayerActionType.Spell1:
				case PlayerActionType.Spell2:
				case PlayerActionType.Spell3:
				case PlayerActionType.Spell4:
					break;

				default:
					Debug.Fail("Invalid player action.");
					ILogger.Log("Invalid player action passed in a package: " + action.Type.ToString(), LogPriority.Warning);
					break;
			}
		}

		/// <summary>
		/// Update the game physics and check for important events that must be reported
		/// to other clients.
		/// </summary>
		void UpdateLogic(double dt)
		{
			UpdateSpells(dt);
			UpdateChampions(dt);
			UpdateStructures(dt);
		}
		void UpdateStructures(double dt)
		{
			Match.Structures.ForEach(s => {
				s.Update(TimeSpan.FromSeconds(dt));

				//TODO: check for health changes and sync with players, check for game end
			});
		}
		void UpdateChampions(double dt)
		{
			// Use the time elapsed since our last snapshot as a delta time
			double time = StateHistory.IsEmpty() ? dt : 
				Server.Instance.GetTime().TotalSeconds - StateHistory.GetLast().Key;

			foreach (ServerClient client in Clients.Values) {
				if (time > 0.0) {
					Match.CurrentState.ApplyPhysicsUpdate(client.Champion.ID, (float)time);
				}

				UpdateChampionHealth(client);

				client.Champion.Animation = client.Champion.GetAnim(!client.ChampStats.Alive, //TODO: replace with actual HP
				                                                    Match.CurrentState.IsOnGround(client.Champion.ID),
				                                                    client.ChampStats.IsCastingSpell(client.Champion.Type, PlayerActionType.Spell1),
				                                                    client.ChampStats.IsCastingSpell(client.Champion.Type, PlayerActionType.Spell2),
				                                                    client.ChampStats.IsCastingSpell(client.Champion.Type, PlayerActionType.Spell3),
				                                                    client.ChampStats.IsCastingSpell(client.Champion.Type, PlayerActionType.Spell4),
				                                                    client.AnimData.Movement,
				                                                    client.AnimData.Idle,
				                                                    client.Champion.Animation);
			}
		}
		void UpdateChampionHealth(ServerClient client)
		{
			// check to respawn after being dead for long enough
			if (!client.ChampStats.Alive &&
			    client.ChampStats.ShouldRespawn()) {
				Respawn(client);
			}

			if (client.ChampStats.GetHealthChangedAndClearFlag()) {
				AddRemarkableEvent(ServerCommand.StatsChanged,
				                   (msg) => {
					msg.Write(client.Champion.ID);
					msg.Write(client.ChampStats.Health);
				});

				if (!client.ChampStats.Alive) { // the player died!
					client.ChampStats.RevivalTime = Server.Instance.GetTime().TotalSeconds + GetRespawnTime().TotalSeconds;
					AddRemarkableEvent(ServerCommand.ChampionDied,
					                   (msg) => {
						msg.Write(client.Champion.ID);
						msg.Write((ushort)GetRespawnTime().TotalSeconds);
					});
				}
			}
		}
		void Respawn(ServerClient client)
		{
			client.ChampStats.SetHealth(client.ChampStats.MaxHealth); // heal back to max health
			client.Champion.Position = GetSpawnPosition(client.Champion.Team);
			client.ChampStats.RevivalTime = double.MaxValue;
			client.Champion.FacingLeft = client.Champion.Team == Teams.Right; // face the opposite team
		}
		static TimeSpan GetRespawnTime()
		{
			return TimeSpan.FromSeconds(15.0); //TODO: depend on game time?
		}
		void UpdateSpells(double dt)
		{
			var toRemove = new List<LinearSpell>();
			ActiveSpells.ForEach(s =>
			{
				Vec2 before = (Vec2)s.Position.Clone();
				Match.CurrentState.ApplyPhysicsUpdate(s.ID, (float)dt);
				Vec2 pass = (s.Position - before) / PhysicsEngine.PHYSICS_PASSES;

				s.Position = before;
				for (int i = 0; i < PhysicsEngine.PHYSICS_PASSES; ++i) {

					// Check for entities collisions
					var rect = s.CreateCollisionRectangle();
					var enemyTeam = TeamsHelper.Opposite(s.Owner.Team);

					// Check to hit players
					bool remove = CheckForSpellPlayerCollisions(s, rect, enemyTeam);

					// Check to hit structures
					if (!remove) {
						remove = CheckForSpellStructuresCollisions(s, rect, 
                                   enemyTeam == Teams.Left ? Match.LeftStructures : Match.RightStructures);
					}

					// Check to remove spells
					if (!remove && // don't check if we know it already has to be removed
					   Match.CurrentState.SpellShouldDisappear(s)) {
						remove = true;
					}

					// The spell has to be removed
					if (remove) {
						toRemove.Add(s);
						break;
					}
					s.Position += pass;
				}
			});

			toRemove.ForEach(s =>
			{
				ActiveSpells.Remove(s);
				RemarkableEvents.Add(Utilities.MakePair<ServerCommand, Action<NetBuffer>>(
										ServerCommand.SpellDisappear,
										(msg) => 
				{
					ulong id = s.ID;
					msg.Write(id);
				}));
			});
		}

		bool CheckForSpellStructuresCollisions(LinearSpell spell, Rect spellRect, TeamStructures structures)
		{
			foreach (IStructure structure in structures.Structures) {
				if (structure.Alive && // not a destroyed target
					spell.Info.Kind == SpellKind.OffensiveSkillshot && // offensive spell
					structure.Rectangle.Intersects(spellRect)) { // we hit it

					structure.Hurt(spell.Info.Value); // we hurt it

					Console.WriteLine(structure.Health);
					return true;
				}
			}

			return false;
		}
		bool CheckForSpellPlayerCollisions(LinearSpell spell, Rect spellRect, Teams enemyTeam)
		{
			foreach (ServerClient client in Clients.Values) { // check for collisions with players
				// With ennemies
				if (client.ChampStats.Alive && // not a dead target
				    spell.Info.Kind == SpellKind.OffensiveSkillshot && // offensive spell
				    client.Champion.Team == enemyTeam && // against an ennemy
				    client.Champion.CreateCollisionRectangle().Intersects(spellRect)) { // we hit him

					client.ChampStats.Hurt(spell.Info.Value); // we hurt him
					return true;
				}
				// With allies
				else if (client.ChampStats.Alive && // not a dead target
				         spell.Info.Kind == SpellKind.DefensiveSkillshot && // deffensive spell
				         client.Champion.Team == spell.Owner.Team &&  // on an ally
				         client.Champion.ID != spell.Owner.ID && // that is NOT us
				         client.Champion.CreateCollisionRectangle().Intersects(spellRect)) { // we hit him

					client.ChampStats.Heal(spell.Info.Value); // we heal him
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Adds the client to the current game.
		/// </summary>
		public void AddClient(NetConnection connection)
		{
			ILogger.Log("New player added to the game.", LogPriority.High);

			ServerChampion champion = CreateRandomChampion();
			ServerClient client = new ServerClient(connection, champion);

			// Send to the client that asked to join, along with the info of the current players
			List<ServerClient> remoteClients = new List<ServerClient>();
			foreach (ServerClient remote in Clients.Values) {
				remoteClients.Add(remote);
			}
			SendCommand(connection,
			            ServerCommand.JoinedGame,
			            NetDeliveryMethod.ReliableUnordered,
			            (msg) => FillJoinedGameMessage(msg, client, remoteClients));

			// Send the new player event to other players
			foreach (NetConnection clientConn in Clients.Keys) {
				if (clientConn != connection) {
					SendCommand(clientConn,
					           ServerCommand.NewRemotePlayer,
					           NetDeliveryMethod.ReliableUnordered,
					           (msg) => FillNewRemotePlayerMessage(msg, client));
				}
			}

			// Apply the changes to our game state
	        Clients.Add(connection, client);
	        Match.CurrentState.AddEntity(champion);
		}

		/// <summary>
		/// Fills a message for the players already in a game to indicate that a new player joined the game.
		/// </summary>
		static void FillNewRemotePlayerMessage(NetBuffer msg, ServerClient client)
		{
			FillChampionInfo(msg, client.Champion, client.ChampStats);
		}

		/// <summary>
		/// Creates a message indicating that he has joined the game and that
		/// the client should create a new drawable champion associated to it.
		/// </summary>
		static void FillJoinedGameMessage(NetBuffer msg, ServerClient champion, List<ServerClient> remoteChampions)
		{
			double time = Server.Instance.GetTime().TotalSeconds;
			remoteChampions.Insert(0, champion); // add our champion to the beginning

			// and send all the champions together
			foreach (ServerClient champ in remoteChampions) {
				FillChampionInfo(msg, champ.Champion, champ.ChampStats);
			}
		}

		static void FillChampionInfo(NetBuffer msg, ICharacter champion, ChampionStats stats)
		{
			ulong id = champion.ID;
			float x = champion.Position.X;
			float y = champion.Position.Y;
			byte type = (byte)champion.Type;
			bool team = champion.Team == Teams.Left;
			float maxhp = stats.MaxHealth;
			float hp = stats.Health;

			msg.Write(id);
			msg.Write(x);
			msg.Write(y);
			msg.Write(type);
			msg.Write(team);
			msg.Write(maxhp);
			msg.Write(hp);
		}

		/// <summary>
		/// Creates a random champion at a random starting position (mainly used
		/// for testing purposes).
		/// </summary>
		ServerChampion CreateRandomChampion()
		{
			var team = GetSmallestTeam();
			return new ServerChampion(IDGenerator.GenerateID(),
			                          GetSpawnPosition(team),
			                          RandomChampionType(),
			                          team);
		}
		Vec2 GetSpawnPosition(Teams team)
		{
			Vec2 tile = team == Teams.Left ? 
				Match.World.Map.Meta.LeftMeta.SpawnTileIds :
				Match.World.Map.Meta.RightMeta.SpawnTileIds;

			return tile * new Vec2(Tile.WIDTH, Tile.HEIGHT) + new Vec2(Tile.WIDTH / 2f, -Tile.HEIGHT);
		}
		ChampionTypes RandomChampionType()
		{
			return ChampionTypes.ManMega; // 100% random, obtained using a dice-roll.
		}

		Teams GetSmallestTeam()
		{
			Dictionary<Teams, int> teams = new Dictionary<Teams, int>();
			teams.Add(Teams.Left, 0);
			teams.Add(Teams.Right, 0);
			foreach (ServerClient client in Clients.Values) {
				teams[client.Champion.Team]++;
			}

			KeyValuePair<Teams, int> min = Utilities.MakePair(RandomTeam(), int.MaxValue);
			foreach (var pair in teams) {
				if (pair.Value < min.Value) {
					min = new KeyValuePair<Teams, int>(pair.Key, pair.Value);
				}
			}

			return min.Key;
		}

		Teams RandomTeam()
		{
			return random.Next(2) == 0 ? Teams.Left : Teams.Right;
		}

		/// <summary>
		/// When we received data from one of our players.
		/// </summary>
		public void OnDataReceived(NetIncomingMessage message)
		{
			ClientCommand command = (ClientCommand)message.ReadByte();

			switch (command) {
				case ClientCommand.ActionPackage:
					OnActionPackage(message);
					break;

				default:
					Debug.Fail("Invalid client command.");
					ILogger.Log("Invalid client command received: " + command, LogPriority.Warning);
					break;
			}
		}

		void OnActionPackage(NetIncomingMessage message)
		{
			Debug.Assert(Clients.ContainsKey(message.SenderConnection));

			try {
				while (message.Position < message.LengthBits) {
					ulong id = message.ReadUInt64();
					float time = message.ReadFloat();
					PlayerActionType type = (PlayerActionType)message.ReadByte();
					Vec2 position = new Vec2(message.ReadFloat(), message.ReadFloat());
					Vec2 target = ActionTypeHelper.IsSpell(type) ? new Vec2(message.ReadFloat(), message.ReadFloat()) : null;

					PlayerAction action = new PlayerAction(id, type, time, position, target);

					Clients[message.SenderConnection].ActionsPackage.Add(action);
				}
			} catch (Exception e) {
				ILogger.Log("Action package badly formatted: " + e.ToString(), LogPriority.Error);
			}
		}
    }
}

