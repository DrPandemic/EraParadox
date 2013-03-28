//
//  Player.cs
//
//  Author:
//       Jesse <${AuthorEmail}>
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
using System.Collections.Generic;

namespace GREATLib
{
	/// <summary>
	/// A player in the game.
	/// </summary>
    public class Player
	{
		/// <summary>
		/// Gets or sets the postion of the player in the world.
		/// </summary>
		/// <value>The postion.</value>
		public Vec2 Position { get; set; }

        public Player()
        {
			Position = new Vec2(0f, 0f);
        }
    }
}

