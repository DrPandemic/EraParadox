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
		static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromMilliseconds(15.0);

		Random random = new Random();

		NetServer Server { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }

		GameMatch Match { get; set; }



        public ServerGame(NetServer server)
        {
			Server = server;
			Clients = new Dictionary<NetConnection, ServerClient>();

			Timer updateTimer = new Timer(UPDATE_INTERVAL.TotalMilliseconds);
			updateTimer.Elapsed += Update;
			updateTimer.Start();
        }

		void Update(object sender, EventArgs e)
		{

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
					Debug.Assert(false, "Invalid client command.");
					ILogger.Log("Invalid client command received: " + command, LogPriority.Warning);
					break;
			}
		}

		void OnActionPackage(NetIncomingMessage message)
		{
			try {
				byte size = message.ReadByte();
				for (int i = 0; i < size; ++i) {
					uint id = message.ReadUInt32();
					float time = message.ReadFloat();
					PlayerActionType type = (PlayerActionType)message.ReadByte();

					ILogger.Log(String.Format("Action package: size={3}, id={0}, time={1}, type={2}", id,time,type,size));
				}
			} catch (Exception e) {
				ILogger.Log("Action package badly formatted: " + e.ToString(), LogPriority.Error);
			}
		}
    }
}

