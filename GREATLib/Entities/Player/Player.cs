//
//  Player.cs
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
using GREATLib.Entities.Player.Champions;

namespace GREATLib.Entities.Player
{
	/// <summary>
	/// Represents a player in the game.
	/// </summary>
    public class Player : ISynchronizable
    {
		/// <summary>
		/// Gets or sets the champion of the player.
		/// </summary>
		/// <value>The champion.</value>
		public IChampion Champion { get; set; }

        public Player()
        {
			//TODO: how should we pass the champion? it could be an enum (+ a factory to
			//create the actual IChampion object) or the object itself, maybe?
			Champion = null;
        }
    }
}

