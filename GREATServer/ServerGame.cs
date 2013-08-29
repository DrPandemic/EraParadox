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

namespace GREATServer
{
	/// <summary>
	/// Represents a game on the server.
	/// </summary>
    public class ServerGame
    {
		const bool CORRECTIONS_ENABLED = false;

		static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromMilliseconds(15.0);
		static readonly TimeSpan CORRECTION_INTERVAL = TimeSpan.FromMilliseconds(UPDATE_INTERVAL.TotalMilliseconds * 2.0);

		static readonly TimeSpan HISTORY_MAX_TIME_KEPT = TimeSpan.FromSeconds(1.0);
		static readonly float MAX_TOLERATED_OFF_DISTANCE = 50f;

		static readonly TimeSpan MIN_TIME_BETWEEN_ACTIONS = TimeSpan.FromMilliseconds(10.0);

		Random random = new Random();

		NetServer NetServer { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }

		GameMatch Match { get; set; }
		/// <summary>
		/// State history of the game, used to simulate a player's action when it happened
		/// on the client's machine.
		/// </summary>
		SnapshotHistory<MatchState> StateHistory { get; set; }
		/// <summary>
		/// Player action history for every player, used to simulate a player's action when it happened on
		/// the client's machine.
		/// </summary>
		Dictionary<uint, SnapshotHistory<PlayerAction>> PlayerActionHistory { get; set; }

		/// <summary>
		/// Time since the last state update (position corrections) was sent.
		/// </summary>
		double TimeSinceLastCorrection { get; set; }


        public ServerGame(NetServer server)
        {
			NetServer = server;
			Clients = new Dictionary<NetConnection, ServerClient>();

			StateHistory = new SnapshotHistory<MatchState>(HISTORY_MAX_TIME_KEPT);
			PlayerActionHistory = new Dictionary<uint, SnapshotHistory<PlayerAction>>();
			Match = new GameMatch();

			TimeSinceLastCorrection = 0.0;

			Timer updateTimer = new Timer(UPDATE_INTERVAL.TotalMilliseconds);
			updateTimer.Elapsed += Update;
			updateTimer.Start();
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

		void Update(object sender, EventArgs e)
		{
			// The server-side loop of the game

			// Store the current game state in our history to redo certain player actions.
			StateHistory.AddSnapshot(Match.CurrentState.Clone() as MatchState, GREATServer.Server.Instance.GetTime().TotalSeconds);

			// Handle actions. We check for recently received player actions
			// and apply them server-side.
			HandleActions();

			// Update logic. We update the actual game logic.
			UpdateLogic();

			// Send corrections. We regularly send the state changes of the entities to
			// other clients.
			if (CORRECTIONS_ENABLED) {
				SendStateChanges();
			}
		}

		/// <summary>
		/// Sends the state deltas to the clients.
		/// </summary>
		void SendStateChanges()
		{
			if (TimeSinceLastCorrection >= CORRECTION_INTERVAL.TotalSeconds) {
				foreach (NetConnection connection in Clients.Keys) {
					SendCommand(
						connection,
						ServerCommand.StateUpdate,
						NetDeliveryMethod.UnreliableSequenced,
						(msg) => FillStateUpdateMessage(msg, connection));
				}
				TimeSinceLastCorrection = 0.0;
			}
			TimeSinceLastCorrection += UPDATE_INTERVAL.TotalSeconds;
		}

		/// <summary>
		/// Fills a message with state update information.
		/// </summary>
		void FillStateUpdateMessage(NetBuffer msg, NetConnection playerConnection)
		{
			Debug.Assert(Clients.ContainsKey(playerConnection));

			msg.Write((uint)Clients[playerConnection].LastAcknowledgedActionID);
			foreach (NetConnection connection in Clients.Keys) {
				ServerClient client = Clients[connection];
				msg.Write((uint)client.Champion.ID);
				msg.Write((float)client.Champion.Position.X);
				msg.Write((float)client.Champion.Position.Y);
				msg.Write((float)client.Champion.Velocity.X);
				msg.Write((float)client.Champion.Velocity.Y);
				msg.Write((bool)client.Champion.IsOnGround);
			}
		}

		/// <summary>
		/// Checks for new player actions and applies them to the current game logic.
		/// </summary>
		void HandleActions()
		{
			foreach (ServerClient client in Clients.Values) {
				Dictionary<PlayerActionType, double> lastTimeOfAction = new Dictionary<PlayerActionType, double>();

				foreach (PlayerAction action in client.ActionsPackage) {
					HandleAction(client.Champion, action);
					client.LastAcknowledgedActionID = Math.Max(client.LastAcknowledgedActionID, action.ID);
				}

				client.ActionsPackage.Clear();
			}
		}

		void HandleAction(IEntity champion, PlayerAction action)
		{
			float now = (float)Server.Instance.GetTime().TotalSeconds;
			float time = action.Time;

			// Make sure we're not using weird times
			time = ValidateActionTime(action, now);

			// Store the action in our history
			Debug.Assert(PlayerActionHistory.ContainsKey(champion.ID));
			SnapshotHistory<PlayerAction> actionHistory = PlayerActionHistory[champion.ID];
			actionHistory.AddSnapshot(action, time);

			// Go to the given action time and resimulate all the other actions by the player until now
			if (!actionHistory.IsEmpty() && !StateHistory.IsEmpty()) {
				KeyValuePair<double, PlayerAction>? actionState = actionHistory.GetClosestSnapshot(time);
				Debug.Assert(actionState.HasValue);
				do {
					// Go to the closest game snapshot to the action that we're simulating
					KeyValuePair<double, MatchState> state = StateHistory.GetClosestSnapshot(time);

					IEntity player = state.Value.GetEntity(champion.ID);

					// Simulate from our closest snapshot to be up-to-date (if we have something to simulate)
					float deltaT = Math.Abs(time - (float)state.Key);
					if (deltaT > 0f) {
						state.Value.ApplyPhysicsUpdate(player.ID, deltaT);
					}

					// Make sure we're not using hacked positions
					Vec2 position = ValidateActionPosition(player, action);

					// Actually execute the action on our currently simulated state
					DoAction(player, action);

					// Resimulate all the states below our next action so that they are affected
					// by the player's action.
					var nextActionState = actionHistory.GetNext(actionState.Value);
					float limitTime = nextActionState.HasValue ? (float)nextActionState.Value.Key : now; // we don't want to simulate at or after that time

					var nextState = StateHistory.GetNext(state);
					while (nextState.HasValue && nextState.Value.Key < limitTime) { // while we have not reached our simulation target time (next action or now)
						// get how much time we have to simulate for next state
						float timeUntilNextState = (float)nextState.Value.Key - time;
						Debug.Assert(timeUntilNextState >= 0f);

						// simulate the current state
						state.Value.ApplyPhysicsUpdate(champion.ID, deltaT);

						// apply the simulation modifications to the next state
						nextState.Value.Value.GetEntity(champion.ID).Clone(state.Value.GetEntity(champion.ID));

						// switch to the next state
						state = nextState.Value;
					}

				} while ((actionState = actionHistory.GetNext(actionState.Value)).HasValue); // while there are actions to simulate
			}
		}

		static float ValidateActionTime(PlayerAction action, float currentTime)
		{
			float time = action.Time;

			// action time is too old? might be a hacker/extreme lag. Log it, keep it but clamp it
			float oldestAcceptedTime = (float)(currentTime - HISTORY_MAX_TIME_KEPT.TotalSeconds);
			if (action.Time < oldestAcceptedTime) {
				time = oldestAcceptedTime;
				ILogger.Log("Action " + action.ID + " seems a bit late. Accepting it, but might be a hacker/extreme lag.", LogPriority.Warning);
			}

			// action time seems too recent? might be a hacker/time error. Log it, keep it but clamp it
			if (action.Time > currentTime) {
				time = currentTime;
				ILogger.Log("Action " + action.ID + " seems a bit too new. Accepting it, but might be a hacker/time error.", LogPriority.Warning);
			}

			return time;
		}

		static Vec2 ValidateActionPosition(IEntity player, PlayerAction action)
		{
			Vec2 position = action.Position;
			// If the position provided by the client seems legit, we take it. Otherwise, we ignore it
			// and log it (might be a hacker).
			if (Vec2.DistanceSquared(player.Position, action.Position) >= MAX_TOLERATED_OFF_DISTANCE * MAX_TOLERATED_OFF_DISTANCE) {
				position = player.Position;
				ILogger.Log("Action " + action.ID + "'s position seems a bit odd. Using the stored server position instead (hacker?). Client will need server correction.", LogPriority.Warning);
			}

			return position;
		}

		void DoAction(IEntity champion, PlayerAction action)
		{
			switch (action.Type) {
				case PlayerActionType.MoveLeft:
					Match.CurrentState.Move(champion.ID, HorizontalDirection.Left);
					break;
					case PlayerActionType.MoveRight:
					Match.CurrentState.Move(champion.ID, HorizontalDirection.Right);
					break;
					case PlayerActionType.Jump:
					Match.CurrentState.Jump(champion.ID);
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
		void UpdateLogic()
		{
			foreach (ServerClient client in Clients.Values) {
				Match.CurrentState.ApplyPhysicsUpdate(client.Champion.ID, UPDATE_INTERVAL.TotalSeconds);

				//TODO: remove, used for testing purposes
				ILogger.Log(client.Champion.Position.ToString());
			}
		}

		/// <summary>
		/// Adds the client to the current game.
		/// </summary>
		public void AddClient(NetConnection connection)
		{
			ILogger.Log("New player added to the game.", LogPriority.High);

			IEntity champion = CreateRandomChampion();

			ServerClient client = new ServerClient(connection, champion);
			Clients.Add(connection, client);

			Debug.Assert(!PlayerActionHistory.ContainsKey(champion.ID));
			PlayerActionHistory.Add(champion.ID, new SnapshotHistory<PlayerAction>(HISTORY_MAX_TIME_KEPT));

			Match.CurrentState.AddEntity(champion);

			// Send to the client that asked to join
			SendCommand(connection,
			       ServerCommand.NewPlayer,
			       NetDeliveryMethod.ReliableOrdered,
			       (msg) => FillNewPlayerMessage(msg, champion, true));
			//TODO: send currently existing players to new player

			//TODO: send to the other players as well here
		}

		/// <summary>
		/// Creates a message indicating that a player has joined the game and that
		/// the client should create a new drawable champion associated to it.
		/// </summary>
		/// <param name="isOwner">Whether this is the new player or not.</param>
		static void FillNewPlayerMessage(NetOutgoingMessage msg, IEntity champion, bool isOwner)
		{
			msg.Write((uint)champion.ID);
			msg.Write((float)champion.Position.X);
			msg.Write((float)champion.Position.Y);
			msg.Write((bool)isOwner);
		}

		/// <summary>
		/// Creates a random champion at a random starting position (mainly used
		/// for testing purposes).
		/// </summary>
		static IEntity CreateRandomChampion()
		{
			return new IEntity(IDGenerator.GenerateID(), 
			                   new Vec2(Utilities.RandomFloat(Utilities.Random, 100f, 400f), 0f));
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
					uint id = message.ReadUInt32();
					float time = message.ReadFloat();
					PlayerActionType type = (PlayerActionType)message.ReadByte();
					Vec2 position = new Vec2(message.ReadFloat(), message.ReadFloat());

					ILogger.Log(String.Format("Action package: id={0}, time={1}, type={2}", id,time,type));
					PlayerAction action = new PlayerAction(id, type, time, position);

					Clients[message.SenderConnection].ActionsPackage.Add(action);
				}
			} catch (Exception e) {
				ILogger.Log("Action package badly formatted: " + e.ToString(), LogPriority.Error);
			}
		}
    }
}

