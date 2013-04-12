//
//  PhysicsSystem.cs
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

namespace GREATLib.Entities.Physics
{
	/// <summary>
	/// The game's physics system, in charge of 
	/// </summary>
    public class PhysicsSystem
    {
        public PhysicsSystem()
        {
        }

		/// <summary>
		/// Applies the world physics to the specified entities.
		/// </summary>
		/// <param name="deltaSeconds">The delta seconds since the last frame.</param>
		/// <param name="entities">The game entities.</param>
		public void Update(float deltaSeconds, List<PhysicsEntity> entities)
		{
			entities.ForEach(e => Update(deltaSeconds, e));
		}

		/// <summary>
		/// Updates an individual entity, given the elapsed time in milliseconds
		/// since the last frame.
		/// </summary>
		/// <param name="deltaSeconds">Delta seconds since the last frame.</param>
		/// <param name="entity">The entity.</param>
		private void Update(float deltaSeconds, PhysicsEntity entity)
		{
			// The minimum speed at which we're supposed to go.
			float moveXVel = (int)entity.Direction * entity.MoveSpeed;
			//TODO: implement air drag here

			// Only move if we're not already moving too fast
			if ((moveXVel < 0 && moveXVel < entity.Velocity.X) ||
			    (moveXVel > 0 && moveXVel > entity.Velocity.X))
				entity.Velocity.X = moveXVel;

			//TODO: apply gravity here

			// Apply the movement
			entity.Position += entity.Velocity * deltaSeconds;

			//TODO: undo collisions with the world geometry here

			entity.Direction = HorizontalDirection.None; // Reset our moving value

			// Make the movement fade out over time
			entity.Velocity.X *= entity.HorizontalAcceleration;
		}

		/// <summary>
		/// Move the specified entity in a direction with a given speed.
		/// </summary>
		/// <param name="entity">The entity to move.</param>
		/// <param name="direction">The direction in which we want to move.</param>
		public void Move(PhysicsEntity entity, HorizontalDirection direction)
		{
			entity.Direction = (entity.Direction == HorizontalDirection.None ? // our first movement
				direction : // set to our new one
				HorizontalDirection.None); // cancel the other movement (i.e. left+right = no movement)
		}

		/// <summary>
		/// Applies a force to an entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="force">The force to apply.</param>
		public void ApplyForce(PhysicsEntity entity, Vec2 force)
		{
			entity.Velocity += force;
		}
    }
}

