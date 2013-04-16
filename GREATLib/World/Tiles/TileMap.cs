//
//  TileMap.cs
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
using System.Collections.Generic;

namespace GREATLib.World.Tiles
{
	/// <summary>
	/// Represents a 2D tile map, to hold the collision data and such of a 
	/// platformer map.
	/// </summary>
    public class TileMap
    {
		/// <summary>
		/// Gets or sets the list of rows (which is a list of tiles).
		/// </summary>
		/// <value>The tile rows.</value>
		public List<List<Tile>> TileRows { get; private set; }

        public TileMap()
        {
			TileRows = GetDummyData();
        }

		/// <summary>
		/// Returns a dummy testing tilemap.
		/// </summary>
		/// <returns>The dummy data.</returns>
		private List<List<Tile>> GetDummyData()
		{
			List<int> ids = GeneralHelper.MakeList(0, 1);
			List<TileCollision> collisions = GeneralHelper.MakeList(
				TileCollision.Passable, TileCollision.Block);

			List<List<int>> tiles = GeneralHelper.MakeList(
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 0, 1, 0, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 0, 0, 1, 1, 1, 0, 0, 0, 1),
				GeneralHelper.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

			return tiles.ConvertAll(
				row => row.ConvertAll(
					id => new Tile(id, collisions[ids.IndexOf(id)])));
		}
    }
}

