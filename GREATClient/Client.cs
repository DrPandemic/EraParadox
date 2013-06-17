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
using System.IO;
using GREATLib;
using System.Collections.Generic;
using GREATLib.Entities.Physics;

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
		public EventHandler<PositionUpdateEventArgs> OnPositionUpdate;

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
			client.Disconnect("FUCK OFF!");
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
			return TimeSpan.FromSeconds((double)client.ServerConnection.AverageRoundtripTime);
		}

		void OnDataReceived(NetIncomingMessage msg)
		{
			byte code = msg.ReadByte();
			ServerCommand command = (ServerCommand)code;

			switch (command) {
				case ServerCommand.NewPlayer:
					OnNewPlayer(this, new NewPlayerEventArgs(msg));
					break;

				case ServerCommand.PositionUpdate:
					OnPositionUpdate(this, new PositionUpdateEventArgs(msg));
					break;

				default:
					throw new NotImplementedException();
			}
		}

		public void SendCommand(ClientCommand command)
		{
			NetOutgoingMessage msg = client.CreateMessage();
			msg.Write((byte)command);

			client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
		}
	}


	public class NewPlayerEventArgs : EventArgs
	{
		public int ID { get; private set; }
		public bool IsOurID { get; private set; }
		public Vec2 Position { get; private set; }
		public NewPlayerEventArgs(NetIncomingMessage msg)
		{
			ID = msg.ReadInt32();
			IsOurID = msg.ReadBoolean();
			Position = new Vec2(msg.ReadFloat(), msg.ReadFloat());
		}
	}
	public struct PositionUpdateData
	{
		public int ID;
		public Vec2 Position;
		public Animation CurrentAnimation;
		public bool FacingLeft;
	}
	public class PositionUpdateEventArgs : EventArgs
	{
		public List<PositionUpdateData> Data { get; private set; }
		public PositionUpdateEventArgs(NetIncomingMessage msg)
		{
			byte count = (byte)msg.ReadByte();
			Data = new List<PositionUpdateData>(count);
			for (int i = 0; i < count; ++i) {
				Data.Add(new PositionUpdateData() { 
					ID = msg.ReadInt32(),
					Position = new Vec2(msg.ReadFloat(), msg.ReadFloat()),
					CurrentAnimation = (Animation)msg.ReadByte(),
					FacingLeft = msg.ReadBoolean()
				});
			}
		}
	}
}

