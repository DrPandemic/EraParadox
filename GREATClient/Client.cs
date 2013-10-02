//
//  Client.cs
//
//  Author:
//       William Turner <willtur.will@gmail.com>
//
//  Copyright (c) 2013 
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
using GREATLib;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using GREATLib.Network;
namespace GREATClient
{
	public sealed class Client
	{
		static volatile Client instance;
		static object syncInstance = new object();
		public static Client Instance
		{
			get {
				if (instance == null) {
					lock (syncInstance) {
						if (instance == null) instance = new Client();
					}
				}
				return instance;
			}
		}

		NetClient client;
		double SharedTime { get; set; }

		LinkedList<KeyValuePair<ServerCommand, NetBuffer>> CommandsToDo { get; set; }
		Dictionary<ServerCommand, ServerCommandEvent> EventsForCommand { get; set; }

		public Client()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
			config.EnableUPnP = true;

			#if DEBUG
			// LAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGG (MonoDevelop is saying bullshit, it works)
			config.SimulatedLoss = 0f;
			config.SimulatedMinimumLatency = 0.015f;
			config.SimulatedRandomLatency = 0f;
			#endif

			this.client = new NetClient(config);

			SharedTime = NetTime.Now;

			CommandsToDo = new LinkedList<KeyValuePair<ServerCommand, NetBuffer>>();
			EventsForCommand = GetEventsForCommand();
		}

		public void Start()
		{
			this.client.Start();
			client.UPnP.ForwardPort(client.Port, "GREAT Client");
			client.DiscoverLocalPeers(14242);
			// If the discover cluster-fucks on localhost, use that line instead
			//client.Connect("127.0.0.1", 14242);
		}

		public void Stop()
		{
			client.Disconnect("Client stopped.");
		}

		public void Update(double dt)
		{
			SharedTime += dt;

			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.DiscoveryResponse:
						Console.WriteLine("Discover response from {0}", msg.SenderEndPoint);
						client.Connect(msg.SenderEndPoint);
						break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if (status == NetConnectionStatus.Connected) {
							Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						OnDataReceived(msg);
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				client.Recycle(msg);
			}
		}

		public TimeSpan GetPing()
		{
            TimeSpan ping = client.ServerConnection != null && client.ServerConnection.AverageRoundtripTime >= 0f ?
				TimeSpan.FromSeconds((double)client.ServerConnection.AverageRoundtripTime) : 
				TimeSpan.Zero;

            Debug.Assert(0.0 <= ping.TotalSeconds);

            return ping;
		}

		public TimeSpan GetTime()
		{
			return TimeSpan.FromSeconds(SharedTime);
		}

		void OnDataReceived(NetBuffer msg)
		{
			byte code = msg.ReadByte();
			ServerCommand command = (ServerCommand)code;

			CommandsToDo.AddLast(Utilities.MakePair<ServerCommand, NetBuffer>(command, msg));

			ExecuteCommands();
		}

		void ExecuteCommands()
		{
			List<KeyValuePair<ServerCommand, NetBuffer>> toRemove = new List<KeyValuePair<ServerCommand, NetBuffer>>();
			foreach (var command in CommandsToDo) {
				if (EventsForCommand.ContainsKey(command.Key)) { // if we have implemented the action to do when we receive this command type
					if (EventsForCommand[command.Key].Execute(command.Value)) {
						toRemove.Add(command); // if we have executed the command properly, we can remove it
					}
				} else {
					throw new NotImplementedException();
				}
			}

			toRemove.ForEach(com => CommandsToDo.Remove(com));
		}

		Dictionary<ServerCommand, ServerCommandEvent> GetEventsForCommand()
		{
			Dictionary<ServerCommand, ServerCommandEvent> e = new Dictionary<ServerCommand, ServerCommandEvent>();

			// we reveive our information after joining a game
			e.Add(ServerCommand.JoinedGame,
			      new ServerCommandEvent(
					(msg) => new JoinedGameEventArgs(msg),
                    (data) => SetSharedTime(((JoinedGameEventArgs)data).Time)));

			// a new player has joined our game
			e.Add(ServerCommand.NewRemotePlayer,
			      new ServerCommandEvent(
					(msg) => new NewRemotePlayerEventArgs(msg)));

			// a state update from the server
			e.Add(ServerCommand.StateUpdate,
			      new ServerCommandEvent(
					(msg) => new StateUpdateEventArgs(msg),
                    (data) => SetSharedTime(((StateUpdateEventArgs)data).Time)));

			return e;
		}

		void SetSharedTime(double time)
		{
			Debug.Assert(time >= 0.0);

			// Take the given server time + our approximation of how long it took to get the message.
			SharedTime = time + GetPing().TotalSeconds / 2.0;

			Debug.Assert(0.0 <= SharedTime && time <= SharedTime);
		}

		/// <summary>
		/// Registers the command handler to react to certain server commands.
		/// </summary>
		public void RegisterCommandHandler(ServerCommand command, EventHandler<CommandEventArgs> handler)
		{
			if (EventsForCommand.ContainsKey(command)) {
				EventsForCommand[command].Handler += handler;
			} else {
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Sends the action package of the player, indicating the inputs that the player
		/// has done in the past few milliseconds.
		/// </summary>
		public void SendPlayerActionPackage(IEnumerable<PlayerAction> actions)
		{
			NetOutgoingMessage msg = client.CreateMessage();
			msg.Write((byte)ClientCommand.ActionPackage);

			foreach (PlayerAction action in actions) {
				uint id = action.ID;
				float time = action.Time;
				byte type = (byte)action.Type;
				float x = action.Position.X;
				float y = action.Position.Y;

				msg.Write(id);
				msg.Write(time);
				msg.Write(type);
				msg.Write(x);
				msg.Write(y);
			}

			client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
		}
	}


	public class CommandEventArgs : EventArgs 
	{
		public CommandEventArgs(NetBuffer msg)
		{
		}
	}
	public class JoinedGameEventArgs : CommandEventArgs
	{
		public double Time { get; private set; }
		public PlayerData OurData { get; private set; }
		public List<PlayerData> RemotePlayers { get; private set; }

		public JoinedGameEventArgs(NetBuffer msg) : base(msg)
		{
            RemotePlayers = new List<PlayerData>();
			OurData = new PlayerData(msg); // the first is our data
			while (msg.Position < msg.LengthBits) {
				RemotePlayers.Add(new PlayerData(msg));
			}
		}
	}
	public class NewRemotePlayerEventArgs : CommandEventArgs
	{
		public PlayerData Data { get; private set; }
		public NewRemotePlayerEventArgs(NetBuffer msg) : base(msg)
		{
			Data = new PlayerData(msg);
		}
	}
	public struct PlayerData
	{
		public uint ID { get; private set; }
		public Vec2 Position { get; private set; }

		public PlayerData(NetBuffer msg) : this()
		{
			ID = msg.ReadUInt32();
			Position = new Vec2(msg.ReadFloat(), msg.ReadFloat());
		}
	}
	public struct StateUpdateData
	{
		public uint ID { get; private set; }
		public Vec2 Position { get; private set; }
		public Vec2 Velocity { get; private set; }

		public StateUpdateData(uint id, Vec2 pos, Vec2 vel)
			: this()
		{
			ID = id;
			Position = pos;
			Velocity = vel;
		}
	}
	public class StateUpdateEventArgs : CommandEventArgs
	{
		public uint LastAcknowledgedActionID { get; private set; }
		public double Time { get; private set; }
		public List<StateUpdateData> EntitiesUpdatedState { get; private set; }

		public StateUpdateEventArgs(NetBuffer msg) : base(msg)
		{
			EntitiesUpdatedState = new List<StateUpdateData>();

			LastAcknowledgedActionID = msg.ReadUInt32();
			Time = msg.ReadDouble();
			Vec2 velocity = new Vec2(msg.ReadFloat(), msg.ReadFloat());
			while (msg.Position < msg.LengthBits) {
				uint id = msg.ReadUInt32();
				Vec2 pos = new Vec2(msg.ReadFloat(), msg.ReadFloat());

				EntitiesUpdatedState.Add(new StateUpdateData(id, pos, velocity));
			}
		}
	}
}

