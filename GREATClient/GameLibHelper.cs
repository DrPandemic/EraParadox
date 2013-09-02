//
//  GameLibHelper.cs
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
using Microsoft.Xna.Framework;
using GREATLib;

namespace GREATClient
{
	/// <summary>
	/// Helper functions to work with the game lib.
	/// </summary>
	public static class GameLibHelper
    {
		/// <summary>
		/// Converts a game library's vector to a Microsoft XNA's vector.
		/// </summary>
		/// <returns>The vector2.</returns>
		/// <param name="vec">The initial vector from the game lib.</param>
		public static Vector2 ToVector2(this Vec2 vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static Rectangle ToRectangle(this Rect rect)
		{
			return new Rectangle(
				(int)rect.X, 
				(int)rect.Y, 
				(int)rect.Width, 
				(int)rect.Height);
		}
    }
}

