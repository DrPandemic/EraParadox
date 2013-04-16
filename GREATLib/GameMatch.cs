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
using GREATLib.Entities.Player;
using System.Collections.Generic;

namespace GREATLib
{
	/// <summary>
	/// An individual game match, 
	/// </summary>
    public class GameMatch
    {
		/// <summary>
		/// Gets the world behind the game.
		/// </summary>
		/// <value>The world.</value>
		public GameWorld World { get; private set; }

		/// <summary>
		/// Gets the players of the match.
		/// </summary>
		/// <value>The players.</value>
		public List<Player> Players { get; private set; }

        public GameMatch()
        {
			World = new GameWorld();
			Players = new List<Player>();
        }
    }
}

