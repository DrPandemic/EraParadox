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

namespace GREATClient
{
    public class Client
    {
		NetClient client;

        public Client()
        {
			NetPeerConfiguration config = new NetPeerConfiguration("GREATClient");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);

			this.client = new NetClient(config);
        }

		public void Start()
		{
			client.DiscoverLocalPeers(14242);
		}

		public void Stop()
		{
			client.Disconnect("FUCK OFF!");
		}

		public void ExecuteFrame()
		{
			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.DiscoveryResponse:
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
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				client.Recycle(msg);
			}
		}
    }
}

