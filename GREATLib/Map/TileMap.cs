//
//  TileMap.cs
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
using GREATLib;
using System.Diagnostics;

namespace Map
{
	public class Tile
	{
		public const int WIDTH = 80;
		public const int HEIGHT = WIDTH;

		/// <summary>
		/// Gets or sets the tile's identifier.
		/// </summary>
		/// <value>The tile's identifier.</value>
		public TileId Id { get; set; }
		/// <summary>
		/// Gets or sets the x index of the tile.
		/// </summary>
		/// <value>The x index.</value>
		public int X { get; set; }
		/// <summary>
		/// Gets or sets the y index of the tile.
		/// </summary>
		/// <value>The y index.</value>
		public int Y { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Map.Tile"/> struct.
		/// </summary>
		/// <param name="id">The tile identifier.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Tile(TileId id, int x, int y)
		{
			Id = id;
			X = x;
			Y = y;
		}
	}

	/// <summary>
	/// Represents a tile map.
	/// </summary>
	public class TileMap
	{
		/// <summary>
		/// Tiles of the map, represented by Ids.
		/// </summary>
		/// <returns>The map.</returns>
		private List<List<Tile>> Tiles { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATLib.TileMap"/> class.
		/// </summary>
		public TileMap()
		{
			//TODO: load from file or better structured than that, simply for test purposes now.
			Tiles = LoadDummyMap();
		}

		/// <summary>
		/// Loads the dummy map for test purposes.
		/// </summary>
		/// <returns>The dummy map.</returns>
		private List<List<Tile>> LoadDummyMap()
		{
			List<List<int>> data = new List<List<int>>();
			data.Add(GeneralHelper.MakeList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			data.Add(GeneralHelper.MakeList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			//data.Add(GeneralHelper.MakeList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			data.Add(GeneralHelper.MakeList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			data.Add(GeneralHelper.MakeList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			data.Add(GeneralHelper.MakeList(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));
			data.Add(GeneralHelper.MakeList(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1));
			data.Add(GeneralHelper.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));
			data.Add(GeneralHelper.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));
			data.Add(GeneralHelper.MakeList(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1));

			List<List<Tile>> tiles = new List<List<Tile>>();
			for (int y = 0; y < data.Count; ++y) {
				List<Tile> row = new List<Tile>();
				for (int x = 0; x < data[y].Count; ++x)
					row.Add(new Tile((TileId)data[y][x], x, y));
				tiles.Add(row);
			}
			return tiles;
		}

		/// <summary>
		/// Gets the touched tiles under a specified rectangle.
		/// </summary>
		/// <returns>The touched tiles.</returns>
		/// <param name="left">Left component of the rectangle.</param>
		/// <param name="top">Top component of the rectangle.</param>
		/// <param name="width">Width of the rectangle.</param>
		/// <param name="height">Height of the rectangle.</param>
		public List<Tile> GetTouchedTiles(int left, int top, int width, int height)
		{
			int right = left + width;
			int bottom = top + height;

			int startX = GeneralHelper.Clamp(left / Tile.WIDTH,
			                                 0, GetWidthTiles() - 1);
			int startY = GeneralHelper.Clamp(top / Tile.HEIGHT,
			                                 0, GetHeightTiles() - 1);
			int endX = GeneralHelper.Clamp((int)Math.Ceiling((double)(right / Tile.WIDTH)),
			                               0, GetWidthTiles() - 1);
			int endY = GeneralHelper.Clamp((int)Math.Ceiling((double)(bottom / Tile.HEIGHT)),
			                               0, GetHeightTiles() - 1);

			List<Tile> touched = new List<Tile>();
			for (int y = startY; y <= endY; ++y) {
				for (int x = startX; x <= endX; ++x) {
					touched.Add(GetTile(x, y));
				}
			}
			return touched;
		}

		/// <summary>
		/// Gets the width in tiles.
		/// </summary>
		/// <returns>The width in tiles.</returns>
		public int GetWidthTiles()
		{
			return Tiles.Count > 0 ? Tiles[0].Count : 0;
		}

		/// <summary>
		/// Gets the height in tiles.
		/// </summary>
		/// <returns>The height in tiles.</returns>
		public int GetHeightTiles()
		{
			return Tiles.Count;
		}

		/// <summary>
		/// Gets the tile at the specified indices.
		/// </summary>
		/// <returns>The tile.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Tile GetTile(int x, int y)
		{
			Debug.Assert(Tiles.Count > 0 &&
				y >= 0 && y < GetHeightTiles() &&
				x >= 0 && x < GetWidthTiles());
			return Tiles[y][x];
		}
	}
}

