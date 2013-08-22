//
//  ServerClient.cs
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
using GREATLib;
using GREATLib.Entities;

namespace GREATServer
{
	/// <summary>
	/// Represents a client on the server.
	/// </summary>
    public class ServerClient
    {
		public NetConnection Connection { get; set; }
		public IEntity Champion { get; set; }

        public ServerClient(NetConnection conn, IEntity champion)
        {
			Connection = conn;
			Champion = champion;
        }
    }
}

