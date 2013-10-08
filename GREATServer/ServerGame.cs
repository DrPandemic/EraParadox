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
			Match = new GameMatch();
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
			RemarkableEvents.Clear();
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
							HandleMovementAction(client.Champion.ID, action);
						} else {
							HandleAction(client.Champion, action);
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

		void HandleAction(ICharacter champion, PlayerAction action)
		{
			if (IsSpell(action.Type)) {
				CastSpell(champion, action);
			} else if (action.Type != PlayerActionType.Idle) {
				ILogger.Log("Unkown player action type: " + action.Type);
			}
		}

		void CastSpell(ICharacter champ, PlayerAction action)
		{
			LinearSpell spell = new LinearSpell(
				IDGenerator.GenerateID(),
				champ.GetHandsPosition(),
				Vec2.Zero, //TODO: use given position
				SpellTypes.StickManSpell1); //TODO: depend on spell used

			Match.CurrentState.AddEntity(spell);
			ActiveSpells.Add(spell);

			Console.WriteLine(spell.Position);
			float castTime = (float)Server.Instance.GetTime().TotalSeconds;
			LinearSpell copy = (LinearSpell)spell.Clone();

			RemarkableEvents.Add(Utilities.MakePair<ServerCommand, Action<NetBuffer>>(
													ServerCommand.SpellCast,
			                                        (NetBuffer msg) => {
				byte type = (byte)copy.Type;
				float time = castTime;
				float px = copy.Position.X;
				float py = copy.Position.Y;
				float vx = copy.Velocity.X;
				float vy = copy.Velocity.Y;
				float cd = (float)copy.Cooldown.TotalSeconds;
				Console.WriteLine(copy.Position);

				msg.Write(type);
				msg.Write(time);
				msg.Write(px);
				msg.Write(py);
				msg.Write(vx);
				msg.Write(vy);
				msg.Write(cd);
			}));
		}

		static bool IsSpell(PlayerActionType action)
		{
			return PlayerActionType.Spell1 <= action && action <= PlayerActionType.Spell4;
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
				//ILogger.Log("Action " + action.ID + "'s position seems a bit odd. Using the stored server position instead (hacker?). Client will need server correction.", LogPriority.Warning);
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

				client.Champion.Animation = client.Champion.GetAnim(false, //TODO: replace with actual HP
				                                                    Match.CurrentState.IsOnGround(client.Champion.ID),
				                                                    false,
				                                                    false,
				                                                    false,
				                                                    false,
				                                                    client.AnimData.Movement,
				                                                    client.AnimData.Idle,
				                                                    client.Champion.Animation);
			}
		}
		void UpdateSpells(double dt)
		{
			ActiveSpells.ForEach(s =>
			{
				Match.CurrentState.ApplyPhysicsUpdate(s.ID, (float)dt);
			});
		}

		/// <summary>
		/// Adds the client to the current game.
		/// </summary>
		public void AddClient(NetConnection connection)
		{
			ILogger.Log("New player added to the game.", LogPriority.High);

			ServerChampion champion = CreateRandomChampion();

			// Send to the client that asked to join, along with the info of the current players
			List<IEntity> remoteChampions = new List<IEntity>();
			foreach (ServerClient remote in Clients.Values) {
				remoteChampions.Add(remote.Champion);
			}
			SendCommand(connection,
			            ServerCommand.JoinedGame,
			            NetDeliveryMethod.ReliableUnordered,
			       		(msg) => FillJoinedGameMessage(msg, champion, remoteChampions));

			// Send the new player event to other players
			foreach (NetConnection clientConn in Clients.Keys) {
				if (clientConn != connection) {
					SendCommand(clientConn,
					           ServerCommand.NewRemotePlayer,
					           NetDeliveryMethod.ReliableUnordered,
					           (msg) => FillNewRemotePlayerMessage(msg, champion));
				}
			}

			// Apply the changes to our game state
	        ServerClient client = new ServerClient(connection, champion);
	        Clients.Add(connection, client);
	        Match.CurrentState.AddEntity(champion);
		}

		/// <summary>
		/// Fills a message for the players already in a game to indicate that a new player joined the game.
		/// </summary>
		static void FillNewRemotePlayerMessage(NetBuffer msg, IEntity champion)
		{
			FillChampionInfo(msg, champion);
		}

		/// <summary>
		/// Creates a message indicating that he has joined the game and that
		/// the client should create a new drawable champion associated to it.
		/// </summary>
		static void FillJoinedGameMessage(NetBuffer msg, IEntity champion, List<IEntity> remoteChampions)
		{
			double time = Server.Instance.GetTime().TotalSeconds;
			remoteChampions.Insert(0, champion); // add our champion to the beginning

			// and send all the champions together
			foreach (IEntity champ in remoteChampions) {
				FillChampionInfo(msg, champ);
			}
		}

		static void FillChampionInfo(NetBuffer msg, IEntity champion)
		{
			ulong id = champion.ID;
			float x = champion.Position.X;
			float y = champion.Position.Y;

			msg.Write(id);
			msg.Write(x);
			msg.Write(y);
		}

		/// <summary>
		/// Creates a random champion at a random starting position (mainly used
		/// for testing purposes).
		/// </summary>
		static ServerChampion CreateRandomChampion()
		{
			return new ServerChampion(IDGenerator.GenerateID(), 
			                   new Vec2(Utilities.RandomFloat(Utilities.Random, 100f, 400f), 150f));
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

					PlayerAction action = new PlayerAction(id, type, time, position);

					Clients[message.SenderConnection].ActionsPackage.Add(action);
				}
			} catch (Exception e) {
				ILogger.Log("Action package badly formatted: " + e.ToString(), LogPriority.Error);
			}
		}
    }
}

