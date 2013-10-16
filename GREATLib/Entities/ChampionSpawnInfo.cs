//
//  ChampionSpawnInfo.cs
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
using GREATLib.Entities.Champions;

namespace GREATLib.Entities
{
    public struct ChampionSpawnInfo
    {
		public ulong ID { get; private set; }
		public Vec2 SpawningPosition { get; private set; }
		public ChampionTypes Type { get; private set; }
		public Teams Team { get; private set; }
		public float MaxHealth { get; private set; }
		public float Health { get; private set; }

		public ChampionSpawnInfo(ulong id, Vec2 spawn, ChampionTypes type, Teams team, float maxhp, float hp) 
			: this() // to be able to have automatic properties (http://stackoverflow.com/a/420441/395386)
		{
			ID = id;
			SpawningPosition = spawn;
			Type = type;
			Team = team;
			MaxHealth = maxhp;
			Health = hp;
		}
    }
}

