//
//  PhysicsEngine.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
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
using System.Diagnostics;
using GREATLib.World;
using GREATLib.Entities;

namespace GREATLib.Physics
{
	/// <summary>
	/// Physics engine to move game objects and resolve collisions.
	/// </summary>
    public class PhysicsEngine
    {
		/// <summary>
		/// The amount of time between every server physics update.
		/// </summary>
		public static readonly TimeSpan UPDATE_RATE = TimeSpan.FromMilliseconds(15.0);
		static readonly Vec2 GRAVITY = new Vec2(0f, 28f);
		/// <summary>
		/// Amount of passes to make a movement.
		/// Basically this represents in how many portions we split our movement on one frame
		/// to prevent too big movements from skipping potential collisions.
		/// </summary>
		const int PHYSICS_PASSES = 3;

		double TimeSinceLastUpdate { get; set; }

		CollisionResolver Collisions { get; set; }

		public PhysicsEngine(GameWorld world)
		{
			Debug.Assert(UPDATE_RATE.TotalSeconds > 0.0);
			Debug.Assert(GRAVITY.Y > 0f, "Gravity has to go towards the ground.");
			Debug.Assert(PHYSICS_PASSES > 0);
			Debug.Assert(world != null);

			TimeSinceLastUpdate = 0.0;

			Collisions = new CollisionResolver(world);
		}

		/// <summary>
		/// Runs a physics update if we have reached our physics update rate.
		/// Call this every frame or so with the delta seconds, and it will only update
		/// the entity when the time reaches the update rate.
		/// </summary>
		public void Update(double deltaSeconds, IEntity entity, ref int xMovement)
		{
			Debug.Assert(entity != null);
			Debug.Assert(deltaSeconds > 0);
			Debug.Assert(TimeSinceLastUpdate >= 0.0);

			TimeSinceLastUpdate += deltaSeconds;

			while (TimeSinceLastUpdate >= UPDATE_RATE.TotalSeconds) {
				ApplyUpdate(UPDATE_RATE.TotalSeconds, entity, ref xMovement);

				TimeSinceLastUpdate -= deltaSeconds;
			}

			Debug.Assert(TimeSinceLastUpdate < UPDATE_RATE.TotalSeconds);
		}

		/// <summary>
		/// Makes the specified entity move in a certain direction for the current frame.
		/// </summary>
		public void Move(IEntity entity, HorizontalDirection direction, ref int xMovement)
		{
			Debug.Assert(entity != null);
			Debug.Assert(Utilities.MakeList(HorizontalDirection.Left, HorizontalDirection.None, HorizontalDirection.Right).Contains(direction));

			if (direction != HorizontalDirection.None) { // we want to move
				Debug.Assert(Utilities.MakeList(HorizontalDirection.Left, HorizontalDirection.Right).Contains(direction));

				int current = (int)entity.Direction;
				int desired = (int)direction;

				xMovement += (int)direction;

				if (current == -desired) { // if we're cancelling our movement
					entity.Direction = HorizontalDirection.None;
				} else { // we're moving
					entity.Direction = direction;
				}
			}
		}

		public void Jump(IEntity entity)
		{
			Debug.Assert(entity != null
			             && entity.Velocity != null);

			// We may only jump when we're on the ground
			if (entity.IsOnGround) {
				entity.Velocity.Y -= entity.JumpForce;
				// assume that we'll lift off the ground to avoid multi-jump.
				entity.IsOnGround = false;
			}
		}

		/// <summary>
		/// Actually applies the update (without checking whether we really should update or not with the update rate).
		/// </summary>
		void ApplyUpdate(double deltaSeconds, IEntity entity, ref int xMovement)
		{
			Debug.Assert(deltaSeconds > 0.0);
			Debug.Assert(entity != null 
			             && entity.Position != null
			             && entity.Velocity != null);

			ApplyDesiredMovement(deltaSeconds, entity, xMovement);

			entity.IsOnGround = false; // reset the flag indicating if we're on the ground

			// Multiple physics passes to reduce the chance of "going through" obstacles when we're too fast.
			for (int pass = 0; pass < PHYSICS_PASSES; ++pass) {
				entity.Position += (entity.Velocity * deltaSeconds) / PHYSICS_PASSES;

				Collisions.UndoCollisions(entity);
			}

			// Make the movement fade out over time
			entity.Velocity.X *= entity.HorizontalAcceleration;

			// reset the movement
			xMovement = 0;
			entity.Direction = HorizontalDirection.None;
		}

		/// <summary>
		/// Applies the desired movement for the entity.
		/// Basically this function modifies the entity's VELOCITY, NOT the position.
		/// </summary>
		void ApplyDesiredMovement(double deltaSeconds, IEntity entity, int xMovement)
		{
			Debug.Assert(deltaSeconds > 0);
			Debug.Assert(entity != null 
			             && entity.Velocity != null);

			// The minimum speed at which we're supposed to go.
			float moveXVel = xMovement * entity.MoveSpeed;

			//if (!entity.IsOnGround) moveXVel *= entity.AirAcceleration;

			// Only move if we're not already moving too fast
			entity.Velocity.X += moveXVel;

			// Apply gravity
			entity.Velocity += GRAVITY;
		}
    }
}

