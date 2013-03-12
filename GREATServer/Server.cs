//
//  Server.cs
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

namespace GREATServer
{
	public class Server
	{
		NetServer server;

		public Server()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			config.Port = 14242;
			config.EnableUPnP = true;

			this.server = new NetServer(config);
		}

		public void Start()
		{
			server.Start();
			server.UPnP.ForwardPort(server.Port, "GREAT Server");
		}

		public void Stop()
		{
			server.Shutdown("I'M DYING D:");
		}

		public void Update()
		{
			NetIncomingMessage msg;
			while ((msg = server.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.DiscoveryRequest:
						Console.WriteLine("Discover request from {0}", msg.SenderEndPoint);
						server.SendDiscoveryResponse(null, msg.SenderEndPoint);
						break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if (status == NetConnectionStatus.Connected) {
							Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
							NetOutgoingMessage sup = server.CreateMessage("Sup?");
							server.SendMessage(sup, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered);
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						Console.WriteLine(msg.ReadString());
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				server.Recycle(msg);
			}
		}
	}
}

