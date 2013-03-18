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

namespace GREATClient
{
	public class Client
	{
		public List<Player> Players { get; set; }

		NetClient client;

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
							NetOutgoingMessage sup = client.CreateMessage("Sup?");
							client.SendMessage(sup, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered);
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						ReadData(msg);
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				client.Recycle(msg);
			}
		}

		/// <summary>
		/// Reads data from a server message.
		/// </summary>
		/// <param name="msg">Message.</param>
		private void ReadData(NetIncomingMessage msg)
		{
			try
			{
				int serverMsgCode = msg.ReadInt32();

				ServerMessage type = (ServerMessage)serverMsgCode;
		
				switch (type)
				{
					case ServerMessage.PositionSync:
						SyncPositions(msg);
						break;

					default:
						throw new NotImplementedException("Server message type \"" + type + "\" not implemented.");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Data not properly formatted: " + msg.ToString() + ", error=" + e.Message);
			}
		}

		/// <summary>
		/// Sends a command to the server. This should be changed later on to
		/// send a list of commands rather than just one.
		/// </summary>
		/// <param name="command">Command.</param>
		public void SendCommand(ClientMessage command)
		{
			//TODO: queue commands and send them in packs rather than everytime a new command happens.
			NetOutgoingMessage msg = client.CreateMessage();
			int commandCode = (int)command;
			msg.Write(commandCode);
			client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);
		}

		/// <summary>
		/// Synchronizes the positions of the players.
		/// </summary>
		/// <param name="msg">Message.</param>
		private void SyncPositions(NetIncomingMessage msg)
		{
			Players = new List<Player>();
			while (msg.PositionInBytes != msg.LengthBytes) {
				Vec2 pos = new Vec2();
				msg.ReadAllProperties(pos);
				Players.Add(new Player() { Position = pos });
			}
		}
	}
}

