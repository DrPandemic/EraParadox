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

namespace GREATServer
{
	/// <summary>
	/// Represents a game on the server.
	/// </summary>
    public class ServerGame
    {
		static readonly TimeSpan POSITION_UPDATE_RATE = TimeSpan.FromMilliseconds(1000.0 / 30.0);

		NetServer Server { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }



        public ServerGame(NetServer server)
        {
			Server = server;
			Clients = new Dictionary<NetConnection, ServerClient>();

			Timer updatePositions = new Timer(POSITION_UPDATE_RATE.TotalMilliseconds);
			updatePositions.Elapsed += UpdatePositions;
			updatePositions.Start();
        }

		public void AddClient(ServerClient client)
		{
			Clients.Add(client.Connection, client);
		}

		public void OnDataReceived(NetIncomingMessage msg)
		{
			Debug.Assert(Clients.ContainsKey(msg.SenderConnection));
			ServerClient client = Clients[msg.SenderConnection];

			byte code = msg.ReadByte();
			ClientCommand command = (ClientCommand)code;

			Console.WriteLine("Received command: " + command);
		}

		void UpdatePositions(object sender, EventArgs e)
		{
			/*if (Clients.Count > 0)
				Console.WriteLine(Clients[0].Position);
			else
				Console.WriteLine("No clients. :(");*/
		}

		void SendToClients(NetOutgoingMessage msg, NetDeliveryMethod method)
		{
			foreach (ServerClient client in Clients.Values)
				Server.SendMessage(msg, client.Connection, method);
		}
    }
}

