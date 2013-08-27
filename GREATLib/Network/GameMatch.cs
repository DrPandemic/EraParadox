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

		PhysicsEngine Physics { get; set; }


        public GameMatch()
        {
			World = new GameWorld();
			Physics = new PhysicsEngine(World);
			CurrentState = new MatchState(Physics);
        }

		public void Update(double deltaSeconds)
		{
		}
    }
}

