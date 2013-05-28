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
using GREATLib.World;
using System.Diagnostics;

namespace GREATLib.Entities.Physics
{
	/// <summary>
	/// The game's physics system, in charge of 
	/// </summary>
    public class PhysicsSystem
    {
		private static readonly Vec2 GRAVITY = new Vec2(0f, 20f);
		/// <summary>
		/// The amount of passes to make a movement.
		/// For example, when set to 3, it will move the entity
		/// 3 times by 1/3 of its velocity and check for collisions.
		/// It is mainly used to make sure that no entity goes
		/// through obstacles when going too fast.
		/// </summary>
		private const int PHYSICS_PASSES = 3;

		private CollisionHandler Collisions { get; set; }

        public PhysicsSystem()
        {
			Collisions = new CollisionHandler();
        }

		/// <summary>
		/// Applies the world physics to the specified entities.
		/// </summary>
		/// <param name="deltaSeconds">The delta seconds since the last frame.</param>
		/// <param name="entities">The game entities.</param>
		public void Update(double deltaSeconds, GameWorld world, 
		                   IEnumerable<PhysicsEntity> entities)
		{
			Debug.Assert(deltaSeconds > 0f, "Delta seconds too small for physics update.");

			foreach (PhysicsEntity entity in entities)
			{
				UpdateEntity(deltaSeconds, world, entity);
			}
		}

		/// <summary>
		/// Updates an individual entity, given the elapsed time in milliseconds
		/// since the last frame.
		/// </summary>
		/// <param name="deltaSeconds">Delta seconds since the last frame.</param>
		/// <param name="entity">The entity.</param>
		private void UpdateEntity(double deltaSeconds, 
		                          GameWorld world,
		                          PhysicsEntity entity)
		{
			ApplyMovement(entity);

			entity.IsOnGround = false; // reset the flag indicating if we're on the ground

			// Really apply the desired movement to our position
			for (int pass = 0; pass < PHYSICS_PASSES; ++pass)
			{
				entity.Position += (entity.Velocity * (float)deltaSeconds) / PHYSICS_PASSES;
				Collisions.HandleCollisions(entity, world);
			}

			if (entity.Direction != HorizontalDirection.None) {
				entity.CurrentAnimation = Animation.Move;
				entity.FacingLeft = entity.Direction == HorizontalDirection.Left;
			}
			else
				entity.CurrentAnimation = Animation.Idle;

			entity.Direction = HorizontalDirection.None; // Reset our moving value
			// Make the movement fade out over time
			entity.Velocity.X *= entity.HorizontalAcceleration;
		}

		static void ApplyMovement(PhysicsEntity entity)
		{
			// The minimum speed at which we're supposed to go.
			float moveXVel = (int)entity.Direction * entity.MoveSpeed;
			//TODO: implement air drag here
			// Only move if we're not already moving too fast
			if ((moveXVel < 0 && moveXVel < entity.Velocity.X) || (moveXVel > 0 && moveXVel > entity.Velocity.X))
				entity.Velocity.X = moveXVel;

			// Apply gravity
			entity.Velocity += GRAVITY;
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
		/// Tries to make the specified entity jump.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public void Jump(PhysicsEntity entity)
		{
			if (entity.IsOnGround)
			{
				entity.Velocity.Y =  -entity.JumpForce;
				entity.IsOnGround = false; // assume that we'll lift off the ground to avoid multi-jump.
			}
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

