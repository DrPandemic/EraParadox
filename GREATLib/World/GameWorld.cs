//
//  GameWorld.cs
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
using GREATLib.World.Tiles;
using System.Collections.Generic;

namespace GREATLib.World
{
	/// <summary>
	/// The game world, holding the various elements that make up
	/// a match (without the entities).
	/// For example, the game world holds:
	/// - The map
	/// - The lane logic
	/// - The special objects
	/// - Objects that can be collected
	/// </summary>
    public class GameWorld
    {
		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public TileMap Map { get; private set; }

        public GameWorld()
        {
			Map = new TileMap();
        }

		/// <summary>
		/// Gets the touched objects in the world.
		/// </summary>
		/// <returns>The touched objects along with their associated collision type.</returns>
		/// <param name="collidable">Collidable.</param>
		public List<KeyValuePair<Rect, CollisionType>> GetTouchedObjects(Rect collidable)
		{
			return Map.GetTouchedTiles(collidable);
		}
    }
}

