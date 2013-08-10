//
//  Tile.cs
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

namespace GREATLib.World.Tiles
{
	/// <summary>
	/// Individual tile from a 2D tilemap.
	/// </summary>
    public class Tile
    {
		/// <summary>
		/// The width of an individual tile.
		/// </summary>
		public const int WIDTH = 32;
		/// <summary>
		/// The height of an individual tile.
		/// </summary>
		public const int HEIGHT = WIDTH;

		/// <summary>
		/// Gets or sets the identifier representing the tile.
		/// </summary>
		/// <value>The identifier.</value>
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the collision type of the tile.
		/// </summary>
		/// <value>The collision.</value>
		public CollisionType Collision { get; set; }

        public Tile(int id, CollisionType collision)
        {
			Id = id;
			Collision = collision;
        }
    }
}

