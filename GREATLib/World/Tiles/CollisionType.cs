//
//  TileCollision.cs
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
	/// The different collision types.
	/// </summary>
    public enum CollisionType
    {
		/// <summary>
		/// A passable object, with no collision event on touch.
		/// </summary>
		Passable,

		/// <summary>
		/// An impassable object, such as a wall, that can't be passed.
		/// </summary>
		Block,

		/// <summary>
		/// A platform object. Entities only collide with it from its top side,
		/// allowing them to go through it through the bottom.
		/// </summary>
		Platform,

		/// <summary>
		/// An ascending 45 degrees ramp, from the left-to-right perspective.
		/// E.g. :
		///     /
		///   /
		/// /
		/// </summary>
		RampAscend,

		/// <summary>
		/// A descending 45 degrees ramp, from the left-to-right perspective.
		/// E.g. :
		/// \
		///  \
		///   \
		/// </summary>
		RampDescend
    }
}

