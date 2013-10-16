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
using GREATLib.Network;
using System.Collections.Generic;
using GREATServer.Network;

namespace GREATServer
{
	/// <summary>
	/// Represents a client on the server.
	/// </summary>
    public class ServerClient
    {
		public NetConnection Connection { get; private set; }
		public ServerChampion Champion { get; private set; }
		public ChampionAnimData AnimData { get; private set; }
		public ChampionStats ChampStats { get; private set; }
		public List<PlayerAction> ActionsPackage { get; private set; }
		public ulong LastAcknowledgedActionID { get; set; }

		public ServerClient(NetConnection conn, ServerChampion champion)
        {
			Connection = conn;
			Champion = champion;
			ActionsPackage = new List<PlayerAction>();
			LastAcknowledgedActionID = IDGenerator.NO_ID;
			AnimData = new ChampionAnimData();
			ChampStats = new ChampionStats(100f); //TODO: depend on champion
        }
    }
}

