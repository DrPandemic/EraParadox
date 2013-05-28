//
//  MovingEntity.cs
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
    public abstract class MovingEntity : IEntity
    {
		/// <summary>
		/// Gets or sets the velocity of the entity.
		/// </summary>
		/// <value>The velocity.</value>
		public Vec2 Velocity { get; set; }

		/// <summary>
		/// Gets the width of the collision box around the entity (for
		/// collisions, spells, etc.)
		/// </summary>
		/// <value>The width of the collision.</value>
		public float CollisionWidth { get; set; }
		protected abstract float DefaultCollisionWidth { get; }
		/// <summary>
		/// Gets the height of the collision box around the entity (for
		/// collisions, spells, etc.)
		/// </summary>
		/// <value>The height of the collision.</value>
		public float CollisionHeight { get; set; }
		protected abstract float DefaultCollisionHeight { get; }


        public MovingEntity()
        {
			Velocity = new Vec2();
			CollisionWidth = DefaultCollisionWidth;
			CollisionHeight = DefaultCollisionHeight;
        }

		/// <summary>
		/// *CREATES* a rectangle englobing the physics entity.
		/// Only use this when you need a full rectangle object, because
		/// it will create it everytime.
		/// </summary>
		/// <value>The rectangle.</value>
		public virtual Rect GetRectangle() 
		{
			// Since the position is the feet position, we take that into account
			return new Rect(
				Position.X - CollisionWidth / 2f, 
				Position.Y - CollisionHeight / 2f,
				CollisionWidth, CollisionHeight);
		}
    }
}

