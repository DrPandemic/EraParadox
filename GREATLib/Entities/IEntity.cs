//
//  IEntity.cs
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

namespace GREATLib.Entities
{
	/// <summary>
	/// Represents an entity, with a position in the world.
	/// </summary>
    public class IEntity : ISynchronizable
    {
		/// <summary>
		/// Gets or sets the position of the feet of the entity in the game world.
		/// </summary>
		/// <value>The position of the feet of the entity.</value>
		public Vec2 Position { get; set; }

        public IEntity()
        {
			Position = new Vec2();
        }
    }
}

