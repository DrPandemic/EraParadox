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

		/// <summary>
		/// Gets or sets the rectangles of the tiles.
		/// </summary>
		/// <value>The tile rectangles.</value>
		private Dictionary<Tile, Rect> TileRectangles { get; set; }

        public TileMap()
        {
			TileRows = GetDummyData();
			InitMap();
        }

		/// <summary>
		/// Returns a dummy testing tilemap.
		/// </summary>
		/// <returns>The dummy data.</returns>
		private List<List<Tile>> GetDummyData()
		{
			// Temporary code to get a quick-and-dirty map.
			List<int> ids = Utilities.MakeList(0, 1);
			List<CollisionType> collisions = Utilities.MakeList(
				CollisionType.Passable, CollisionType.Block);

			List<List<int>> tiles = Utilities.MakeList(
				Utilities.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1),
				Utilities.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

			return tiles.ConvertAll(
				row => row.ConvertAll(
					id => new Tile(id, collisions[ids.IndexOf(id)])));
		}

		private void InitMap()
		{
			TileRectangles = new Dictionary<Tile, Rect>();
			for (int y = 0; y < GetHeightTiles(); ++y)
			{
				for (int x = 0; x < GetWidthTiles(); ++x)
				{
					TileRectangles.Add(TileRows[y][x],
						new Rect(x * Tile.WIDTH,
					         y * Tile.HEIGHT,
					         Tile.WIDTH,
					         Tile.HEIGHT));
				}
			}
		}

		/// <summary>
		/// Gets the touched tiles by a passed rectangle.
		/// </summary>
		public List<KeyValuePair<Rect, CollisionType>> GetTouchedTiles(Rect rectangle)
		{
			Debug.Assert(rectangle != null);
			Debug.Assert(TileRectangles != null, "Map not initialized.");

			// Get the start/end indices of the tiles that our rectangle touches
			int centerX = (int)((rectangle.Left + rectangle.Width / 2f) / Tile.WIDTH);
			int centerY = (int)((rectangle.Top + rectangle.Height / 2f) / Tile.HEIGHT);

			List<KeyValuePair<Rect, CollisionType>> touched = new List<KeyValuePair<Rect, CollisionType>>(8); // reserve the space we'll use

			// if we just get the touched tiles with 2 nested loops, we'll get a list in this order:
			// [1][2][3]
			// [4][5][6]
			// [7][8][9]
			// but then we'll fix the diagonal tiles before the horizontal/vertical ones, and we don't want that
			// (it produces weird collision resolution otherwise).
			// We want this order: (see http://www.raywenderlich.com/15230/how-to-make-a-platform-game-like-super-mario-brothers-part-1/ss-tile-order-given-and-desired)
			// [6][3][7]
			// [4][1][5]
			// [8][2][9]

			AddTouchedTile(centerX, centerY, touched);         // center tile (may not be necessary for big objects, but it is for smaller ones)
			AddTouchedTile(centerX, centerY + 1, touched);     // bottom tile
			AddTouchedTile(centerX, centerY - 1, touched);     // top tile
			AddTouchedTile(centerX - 1, centerY, touched);     // left tile
			AddTouchedTile(centerX + 1, centerY, touched);     // right tile
			AddTouchedTile(centerX - 1, centerY - 1, touched); // top left tile
			AddTouchedTile(centerX + 1, centerY - 1, touched); // top right tile
			AddTouchedTile(centerX - 1, centerY + 1, touched); // bottom left tile
			AddTouchedTile(centerX + 1, centerY + 1, touched); // bottom right tile

			return touched;
		}

		void AddTouchedTile(int x, int y, ICollection<KeyValuePair<Rect, CollisionType>> touchedTiles)
		{
			if (AreValidTileIndices(x, y)) {
				CollisionType collision = TileRows[y][x].Collision;
				if (collision != CollisionType.Passable) { // we have a collision
					Debug.Assert(TileRectangles.ContainsKey(TileRows[y][x]), "Tile rectangle not created.");
					Rect tileRect = TileRectangles[TileRows[y][x]];
					touchedTiles.Add(new KeyValuePair<Rect, CollisionType>(tileRect, collision));
				}
			}
		}

		/// <summary>
		/// Checks whether the given tile indices are valid.
		/// </summary>
		bool AreValidTileIndices(int x, int y)
		{
			return x >= 0 && x < GetWidthTiles() &&
				   y >= 0 && y < GetHeightTiles();
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

		/// <summary>
		/// Checks if it is a valid x index.
		/// </summary>
		public bool IsValidXIndex(int x)
		{
			return x >= 0 && x < GetWidthTiles();
		}
		/// <summary>
		/// Checks if it is a valid y index.
		/// </summary>
		public bool IsValidYIndex(int y)
		{
			return y >= 0 && y < GetHeightTiles();
		}

		/// <summary>
		/// Gets the x index of the tile (as a rectangle).
		/// </summary>
		public int GetTileXIndex(Rect rect)
		{
			return (int)(rect.Left / Tile.WIDTH);
		}
		/// <summary>
		/// Gets the y index of the tile (as a rectangle).
		/// </summary>
		public int GetTileYIndex(Rect rect)
		{
			return (int)(rect.Top / Tile.HEIGHT);
		}

		/// <summary>
		/// Gets the collision of the given tile.
		/// IF the tile is out of the tilemap, it returns a blocking collision.
		/// </summary>
		public CollisionType GetCollision(int tileX, int tileY)
		{
			return IsValidXIndex(tileX) && IsValidYIndex(tileY) ?
				TileRows[tileY][tileX].Collision : CollisionType.Block;
		}
    }
}

