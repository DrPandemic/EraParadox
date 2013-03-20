//
//  TileId.cs
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

namespace Map
{
	/// <summary>
	/// A tile identifier from a tile map.
	/// </summary>
    public enum TileId
    {
		Empty = 0,
		Block = 1
    }

	/// <summary>
	/// Data about the tiles.
	/// </summary>
	public static class TileData
	{
		/// <summary>
		/// Determines if the specified tile is solid.
		/// </summary>
		/// <returns><c>true</c> if the tile is solid; otherwise, <c>false</c>.</returns>
		/// <param name="id">The tile identifier.</param>
		public static bool IsSolid(TileId id)
		{
			switch (id) {
				case TileId.Empty: 
					return false;

				case TileId.Block: 
					return true;

				default: throw new NotImplementedException("Tile id not implemented in IsSolid.");
			}
		}
	}
}

