//
//  IChampion.cs
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
using GREATLib.Entities.Physics;

namespace GREATLib.Entities.Player.Champions
{
	/// <summary>
	/// Represents a game champion.
	/// </summary>
    public abstract class IChampion : PhysicsEntity
    {
		/// <summary>
		/// Gets or sets the name of the champion
		/// </summary>
		/// <value>The name.</value>
		public abstract string Name { get; }

        public IChampion()
        {
        }
    }
}

