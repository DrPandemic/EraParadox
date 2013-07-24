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
		static readonly TimeSpan POSITION_UPDATE_RATE = GameMatch.STATE_UPDATE_INTERVAL;

		Random random = new Random();

		NetServer Server { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }

		GameMatch Match { get; set; }



        public ServerGame(NetServer server)
        {
			Server = server;
			Clients = new Dictionary<NetConnection, ServerClient>();
        }

		void Update(object sender, EventArgs e)
		{
		}
    }
}

