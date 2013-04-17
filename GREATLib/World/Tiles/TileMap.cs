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
using System.Diagnostics;

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
			// Temporary code to get a quick-and-dirty map.
			List<int> ids = GeneralHelper.MakeList(0, 1);
			List<CollisionType> collisions = GeneralHelper.MakeList(
				CollisionType.Passable, CollisionType.Block);

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

		/// <summary>
		/// Gets the touched tiles by a passed rectangle.
		/// </summary>
		/// <returns>The touched tiles.</returns>
		/// <param name="left">Left side of the rectangle.</param>
		/// <param name="top">Top side of the rectangle.</param>
		/// <param name="width">Width of the rectangle.</param>
		/// <param name="height">Height of the rectangle.</param>
		public List<KeyValuePair<Rect, CollisionType>> GetTouchedTiles(Rect rectangle)
		{
			// Get the start/end indices of the tiles that our rectangle touches
			int startX = GeneralHelper.Clamp((int)rectangle.Left / Tile.WIDTH,
			                                 0, GetWidthTiles() - 1);
			int startY = GeneralHelper.Clamp((int)rectangle.Top / Tile.HEIGHT,
			                                 0, GetHeightTiles() - 1);
			int endX = GeneralHelper.Clamp((int)Math.Ceiling((double)rectangle.Right / Tile.WIDTH) - 1,
			                               0, GetWidthTiles() - 1);
			int endY = GeneralHelper.Clamp((int)Math.Ceiling((double)rectangle.Bottom / Tile.HEIGHT) - 1,
			                               0, GetHeightTiles() - 1);

			List<KeyValuePair<Rect, CollisionType>> touched = new List<KeyValuePair<Rect, CollisionType>>();

			for (int y = startY; y <= endY; ++y)
			{
				for (int x = startX; x <= endX; ++x)
				{
					CollisionType collision = TileRows[y][x].Collision;
					if (collision != CollisionType.Passable) // we have a collision
					{
						Rect tileRect = new Rect(x * Tile.WIDTH, y * Tile.HEIGHT,
						                         Tile.WIDTH, Tile.HEIGHT);
						touched.Add(new KeyValuePair<Rect, CollisionType>(
							tileRect, collision));
					}
				}
			}

			return touched;
		}

		/// <summary>
		/// Gets the width of the tilemap, in tiles.
		/// </summary>
		/// <returns>The width in tiles.</returns>
		public int GetWidthTiles()
		{
			return TileRows.Count > 0 ? TileRows[0].Count : 0;
		}
		/// <summary>
		/// Gets the height of the tilemap, in tiles.
		/// </summary>
		/// <returns>The height in tiles.</returns>
		public int GetHeightTiles()
		{
			return TileRows.Count;
		}
    }
}

