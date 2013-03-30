//
//  PhysicsEntity.cs
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

namespace GREATLib.Entities.Physics
{
	/// <summary>
	/// Represents an entity that is affected by physics.
	/// </summary>
    public abstract class PhysicsEntity : IEntity
    {
		public const float DEFAULT_HORIZONTAL_ACCELERATION = 0.9f;

		/// <summary>
		/// Gets or sets the velocity of the entity.
		/// </summary>
		/// <value>The velocity.</value>
		public Vec2 Velocity { get; set; }

		/// <summary>
		/// Gets or sets the horizontal acceleration of the entity.
		/// </summary>
		/// <value>The horizontal acceleration.</value>
		public float HorizontalAcceleration { get; set; }

        public PhysicsEntity()
        {
			Velocity = new Vec2();
			HorizontalAcceleration = DEFAULT_HORIZONTAL_ACCELERATION;
        }
    }
}

