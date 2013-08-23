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
using System.Collections.Generic;
using System.Diagnostics;
using GREATClient.Network;
using GREATLib.Network;

namespace GREATClient
{
	public class Client
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

		public EventHandler<NewPlayerEventArgs> OnNewPlayer;
		public EventHandler<StateUpdateEventArgs> OnStateUpdate;

		public Client()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
			config.EnableUPnP = true;

			#if DEBUG
			// LAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGG (MonoDevelop is saying bullshit, it works)
			config.SimulatedLoss = 0.01f;
			config.SimulatedMinimumLatency = 0.05f;
			config.SimulatedRandomLatency = 0.05f;
			#endif

			this.client = new NetClient(config);
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

		public void Update()
		{
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
			return client.ServerConnection != null ?
				TimeSpan.FromSeconds((double)client.ServerConnection.AverageRoundtripTime) : 
				TimeSpan.Zero;
		}

		public TimeSpan GetTime()
		{
			return TimeSpan.FromSeconds(NetTime.Now);
		}

		void OnDataReceived(NetIncomingMessage msg)
		{
			byte code = msg.ReadByte();
			ServerCommand command = (ServerCommand)code;

			switch (command) {
				case ServerCommand.NewPlayer:
					ILogger.Log("New player command.", LogPriority.High);
					if (OnNewPlayer != null) {
						OnNewPlayer(this, new NewPlayerEventArgs(msg));
					}
					break;

				case ServerCommand.StateUpdate:
					ILogger.Log("State update.", LogPriority.VeryLow);
					if (OnStateUpdate != null) {
						OnStateUpdate(this, new StateUpdateEventArgs(msg));
					}
					break;

				default:
					throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Sends the action package of the player, indicating the inputs that the player
		/// has done in the past few milliseconds.
		/// </summary>
		public void SendPlayerActionPackage(List<PlayerAction> actions)
		{
			NetOutgoingMessage msg = client.CreateMessage();
			msg.Write((byte)ClientCommand.ActionPackage);

			foreach (PlayerAction action in actions) {
				msg.Write((uint)action.ID);
				msg.Write((float)action.Time);
				msg.Write((byte)action.Type);
			}

			client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
		}
	}


	public class NewPlayerEventArgs : EventArgs
	{
		public uint ID { get; private set; }
		public bool IsOurID { get; private set; }
		public Vec2 Position { get; private set; }
		public NewPlayerEventArgs(NetIncomingMessage msg)
		{
			ID = msg.ReadUInt32();
			Position = new Vec2(msg.ReadFloat(), msg.ReadFloat());
			IsOurID = msg.ReadBoolean();
		}
	}
	public struct StateUpdateData
	{
		public uint ID { get; private set; }
		public Vec2 Position { get; private set; }
		public bool IsOnGround { get; private set; }

		public StateUpdateData(uint id, Vec2 pos, bool isOnGround)
			: this()
		{
			ID = id;
			Position = pos;
			IsOnGround = isOnGround;
		}
	}
	public class StateUpdateEventArgs : EventArgs
	{
		public List<StateUpdateData> StateUpdate { get; private set; }

		public StateUpdateEventArgs(NetIncomingMessage msg)
		{
			StateUpdate = new List<StateUpdateData>();
			while (msg.Position < msg.LengthBits) {
				uint id = msg.ReadUInt32();
				Vec2 pos = new Vec2(msg.ReadFloat(), msg.ReadFloat());
				bool onGround = msg.ReadBoolean();

				StateUpdate.Add(new StateUpdateData(id, pos, onGround));
				ILogger.Log(
					String.Format("state update data: id={0}, pos={1}, onground={2}", id, pos, onGround));
			}
		}
	}
}

