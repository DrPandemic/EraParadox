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
using GREATLib;
using System.Diagnostics;

namespace GREATClient.Network.Physics
{
	/// <summary>
	/// Physics engine to offer move game objects and resolve collisions.
	/// If you need physics in the game, you are looking at the right place.
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

        public PhysicsEngine()
        {
			Debug.Assert(UPDATE_RATE.TotalSeconds > 0.0);
			Debug.Assert(GRAVITY.Y > 0f, "Gravity has to go towards the ground.");
			Debug.Assert(PHYSICS_PASSES > 0);

			TimeSinceLastUpdate = 0.0;
        }

		/// <summary>
		/// Runs a physics update if we have reached our physics update rate.
		/// Call this every frame or so with the delta seconds, and it will only update
		/// the entity when the time reaches the update rate.
		/// </summary>
		public void Update(double deltaSeconds, MainClientChampion entity, ref int xMovement)
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
		public void Move(MainClientChampion entity, HorizontalDirection direction, ref int xMovement)
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

		/// <summary>
		/// Actually applies the update (without checking whether we really should update or not with the update rate).
		/// </summary>
		void ApplyUpdate(double deltaSeconds, MainClientChampion entity, ref int xMovement)
		{
			Debug.Assert(deltaSeconds > 0.0);
			Debug.Assert(entity != null 
			             && entity.SimulatedPosition != null
			             && entity.Velocity != null);

			ApplyDesiredMovement(deltaSeconds, entity, xMovement);

			// Multiple physics passes to reduce the chance of "going through" obstacles when we're too fast.
			for (int pass = 0; pass < PHYSICS_PASSES; ++pass) {
				entity.SimulatedPosition += (entity.Velocity * deltaSeconds) / PHYSICS_PASSES;
				//TODO: collisions
				ILogger.Log(entity.SimulatedPosition.ToString());
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
		void ApplyDesiredMovement(double deltaSeconds, MainClientChampion entity, int xMovement)
		{
			// The minimum speed at which we're supposed to go.
			float moveXVel = xMovement * entity.MoveSpeed;

			//if (!entity.IsOnGround) moveXVel *= entity.AirAcceleration;

			// Only move if we're not already moving too fast
			if ((moveXVel < 0 && moveXVel < entity.Velocity.X) || (moveXVel > 0 && moveXVel > entity.Velocity.X))
				entity.Velocity.X = moveXVel;

			// Apply gravity
			//entity.Velocity += GRAVITY;
		}
    }
}

