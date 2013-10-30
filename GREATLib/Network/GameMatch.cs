//
//  GameMatch.cs
//
//  Author:
//       Jesse <>
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
using GREATLib.World;
using System.Collections.Generic;
using System.Diagnostics;
using GREATLib.Physics;
using GREATLib.Entities;
using GREATLib.Entities.Structures;

namespace GREATLib.Network
{
	/// <summary>
	/// An individual game match, 
	/// </summary>
    public class GameMatch
    {
		/// <summary>
		/// The time taken between each state update sent by the server.
		/// This is in the game's lib because it must be the same value for both the client
		/// and the server.
		/// </summary>
		public static readonly TimeSpan STATE_UPDATE_INTERVAL = TimeSpan.FromMilliseconds(50.0);

		public GameWorld World { get; private set; }
		public MatchState CurrentState { get; set; }

		public TeamStructures LeftStructures { get; private set; }
		public TeamStructures RightStructures { get; private set; }
		public List<IStructure> Structures { get; private set; }

		PhysicsEngine Physics { get; set; }


        public GameMatch(string mapPath)
        {
			World = new GameWorld(mapPath);
			Physics = new PhysicsEngine(World);
			CurrentState = new MatchState(Physics);

			LeftStructures = new TeamStructures(Teams.Left,
				World.Map.Meta.LeftMeta.BaseTileIds);
			RightStructures = new TeamStructures(Teams.Right,
				World.Map.Meta.RightMeta.BaseTileIds);

			Structures = new List<IStructure>();
			LeftStructures.Structures.ForEach(Structures.Add);
			RightStructures.Structures.ForEach(Structures.Add);
        }

		public void Update(double deltaSeconds)
		{
		}
    }
}

