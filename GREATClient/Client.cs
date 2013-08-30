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
		double SharedTime { get; set; }

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

			SharedTime = NetTime.Now;
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
			return client.ServerConnection != null ?
				TimeSpan.FromSeconds((double)client.ServerConnection.AverageRoundtripTime) : 
				TimeSpan.Zero;
		}

		public TimeSpan GetTime()
		{
			return TimeSpan.FromSeconds(SharedTime);
		}

		void OnDataReceived(NetIncomingMessage msg)
		{
			byte code = msg.ReadByte();
			ServerCommand command = (ServerCommand)code;

			switch (command) {
				case ServerCommand.NewPlayer:
					if (OnNewPlayer != null) {
						NewPlayerEventArgs e = new NewPlayerEventArgs(msg);
						OnNewPlayer(this, e);
						SetSharedTime(e.Time);
					}
					break;

				case ServerCommand.StateUpdate:
					if (OnStateUpdate != null) {
						StateUpdateEventArgs e = new StateUpdateEventArgs(msg);
						OnStateUpdate(this, e);
						SetSharedTime(e.Time);
					}
					break;

				default:
					throw new NotImplementedException();
			}
		}

		void SetSharedTime(double time)
		{
			SharedTime = time + GetPing().TotalSeconds / 2.0;
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


	public class NewPlayerEventArgs : EventArgs
	{
		public double Time { get; private set; }
		public uint ID { get; private set; }
		public bool IsOurID { get; private set; }
		public Vec2 Position { get; private set; }
		public NewPlayerEventArgs(NetIncomingMessage msg)
		{
			Time = msg.ReadDouble();
			ID = msg.ReadUInt32();
			Position = new Vec2(msg.ReadFloat(), msg.ReadFloat());
			IsOurID = msg.ReadBoolean();
		}
	}
	public struct StateUpdateData
	{
		public uint ID { get; private set; }
		public Vec2 Position { get; private set; }

		public StateUpdateData(uint id, Vec2 pos)
			: this()
		{
			ID = id;
			Position = pos;
		}
	}
	public class StateUpdateEventArgs : EventArgs
	{
		public double Time { get; private set; }
		public uint LastAcknowledgedActionID { get; private set; }
		public List<StateUpdateData> EntitiesUpdatedState { get; private set; }

		public StateUpdateEventArgs(NetIncomingMessage msg)
		{
			EntitiesUpdatedState = new List<StateUpdateData>();

			Time = msg.ReadDouble();
			LastAcknowledgedActionID = msg.ReadUInt32();
			while (msg.Position < msg.LengthBits) {
				uint id = msg.ReadUInt32();
				Vec2 pos = new Vec2(msg.ReadFloat(), msg.ReadFloat());

				EntitiesUpdatedState.Add(new StateUpdateData(id, pos));
			}
		}
	}
}

