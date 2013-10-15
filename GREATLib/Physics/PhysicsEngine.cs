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
		/// The amount of time between every physics update. The users of the class
		/// may call the update with any delta, but internally we always update with
		/// the same timestep (and do accomodations to fit the given delta).
		/// </summary>
		static readonly TimeSpan FIXED_TIMESTEP = TimeSpan.FromMilliseconds(15.0);
		static readonly double DELTA_S = FIXED_TIMESTEP.TotalSeconds;
		/// <summary>
		/// The gravity force per second applied.
		/// </summary>
		static readonly Vec2 GRAVITY = new Vec2(0f, 2000f);//1866.67f);
		/// <summary>
		/// Amount of passes to make a movement.
		/// Basically this represents in how many portions we split our movement on one frame
		/// to prevent too big movements from skipping potential collisions.
		/// </summary>
		public const int PHYSICS_PASSES = 3;

		double TimeSinceLastUpdate { get; set; }

		CollisionResolver Collisions { get; set; }

		static PhysicsEngine()
		{
			Debug.Assert(FIXED_TIMESTEP.TotalSeconds > 0.0);
			Debug.Assert(GRAVITY.Y > 0f, "Gravity has to go towards the ground.");
			Debug.Assert(PHYSICS_PASSES > 0);
		}

		public PhysicsEngine(GameWorld world)
		{
			Debug.Assert(world != null);

			TimeSinceLastUpdate = 0.0;

			Collisions = new CollisionResolver(world);
		}

		/// <summary>
		/// Runs a physics update on the given entity with a given delta seconds.
		/// Internally, this function calls the physics update with a fixed timestep.
		/// Therefore, if the delta seconds is not a 
		/// </summary>
		public void Update(double deltaSeconds, IEntity entity)
		{
			Debug.Assert(deltaSeconds > 0.0);
			Debug.Assert(entity != null 
			             && entity.Position != null
			             && entity.Velocity != null);

			while (deltaSeconds >= FIXED_TIMESTEP.TotalSeconds) {
				ApplyUpdate(entity);
				deltaSeconds -= FIXED_TIMESTEP.TotalSeconds;
			}

			if (deltaSeconds > 0.0) { // we have leftover time to simulate
				float progress = (float)(deltaSeconds / FIXED_TIMESTEP.TotalSeconds);

				InterpolateUpdate(entity, progress);
			}
		}

		/// <summary>
		/// Interpolates between the current entity state and its future state (with one
		/// more physics update).
		/// </summary>
		void InterpolateUpdate(IEntity entity, float progress)
		{
			Debug.Assert(entity != null
						 && entity.Position != null
						 && entity.Velocity != null);
			Debug.Assert(progress + double.Epsilon >= 0.0 && progress <= 1.0 + double.Epsilon);

			// Keep some values that will be interpolated
			Vec2 initialPos = entity.Position;
			Vec2 initialVel = entity.Velocity;

			// Simulate the next step
			ApplyUpdate(entity);

			// interpolate some elements
			entity.Position = Vec2.Lerp(initialPos, entity.Position, progress);
			entity.Velocity = Vec2.Lerp(initialVel, entity.Velocity, progress);
		}

		void ApplyUpdate(IEntity e)
		{
			if (e is ICharacter)
				UpdateCharacter((ICharacter)e);
			else
				UpdateEntity(e);
		}

		void UpdateEntity(IEntity entity)
		{
			Debug.Assert(entity != null
			             && entity.Position != null
			             && entity.Velocity != null);

			ApplyMovement(entity, null);
		}

		/// <summary>
		/// Actually applies a physics update to an entity with a fixed timestep.
		/// </summary>
		void UpdateCharacter(ICharacter character)
		{
			Debug.Assert(character != null
			             && character.Position != null
			             && character.Velocity != null);

			// Apply gravity
			character.Velocity += GRAVITY * DELTA_S;
			 
			RestrictSpeed(character);

			ApplyMovement(character, () => Collisions.UndoCollisions(character));

			// Make the movement fade out over time
			character.Velocity.X *= (float)Math.Pow(character.HorizontalAcceleration, DELTA_S);
		}

		void ApplyMovement(IEntity e, Action onPhysicsPass)
		{
			// Multiple physics passes to reduce the chance of "going through" obstacles when we're too fast.
			Vec2 passMovement = (e.Velocity * DELTA_S) / PHYSICS_PASSES;
			for (int pass = 0; pass < PHYSICS_PASSES; ++pass) {
				e.Position += passMovement;
				if (onPhysicsPass != null) onPhysicsPass();
			}
		}

		void RestrictSpeed(IEntity entity)
		{
			// Restrict movement speed
			if (entity.Velocity.GetLengthSquared() > IEntity.MAX_SPEED * IEntity.MAX_SPEED)
				entity.Velocity = Vec2.Normalize(entity.Velocity) * IEntity.MAX_SPEED;
		}

		/// <summary>
		/// Makes the specified entity move in a certain direction for the current frame.
		/// </summary>
		public void Move(IEntity entity, HorizontalDirection direction)
		{
			Debug.Assert(entity != null
			             && entity.Velocity != null);
			Debug.Assert(Utilities.MakeList(HorizontalDirection.Left, HorizontalDirection.None, HorizontalDirection.Right)
			             	.Contains(direction));

			if (direction != HorizontalDirection.None) { // we want to move
				Debug.Assert(Utilities.MakeList(HorizontalDirection.Left, HorizontalDirection.Right).Contains(direction));

				// Find in what direction we should apply the force
				float moveForce = (int)direction * entity.MoveSpeed;

				// Apply the movement
				entity.Velocity.X += moveForce;
			}
		}

		public void Jump(ICharacter character)
		{
			Debug.Assert(character != null
			             && character.Velocity != null);

			// We may only jump when we're on the ground
			if (IsOnGround(character)) {
				character.Velocity.Y = -character.JumpForce;
			}
		}

		/// <summary>
		/// Resets the movement of the entity.
		/// </summary>
		public void ResetMovement(IEntity entity)
		{
			entity.Velocity = Vec2.Zero;
		}

		public bool IsOnGround(IEntity entity)
		{
			return Collisions.HasCollisionBelow(entity);
		}

		public bool CollidesWithWorld(IEntity e)
		{
			return Collisions.HasCollisions(e);
		}
    }
}

