//
//  CollisionResolver.cs
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
using GREATLib.World;
using System.Diagnostics;
using System.Collections.Generic;
using GREATLib.World.Tiles;
using GREATLib.Entities;

namespace GREATLib.Physics
{
	/// <summary>
	/// The system in charge of resolving collisions between the game entities and the
	/// game world.
	/// </summary>
    public class CollisionResolver
    {
		GameWorld World { get; set; }

		public CollisionResolver(GameWorld world)
		{
			Debug.Assert(world != null);

			World = world;
		}

		/// <summary>
		/// Resolves the collisions between an entity and the world it is in.
		/// </summary>
		public void UndoCollisions(IEntity entity)
		{
			Debug.Assert(entity != null);

			var collisions = World.GetTouchedObjects(entity.CreateCollisionRectangle());

			HandleCollisionGroup(entity, collisions);
		}

		/// <summary>
		/// Handles the collision of an object with multiple entities.
		/// </summary>
		void HandleCollisionGroup(
			IEntity entity,
			List<KeyValuePair<Rect, CollisionType>> collisions)
		{
			Debug.Assert(entity != null);
			Debug.Assert(collisions != null);

			foreach (KeyValuePair<Rect, CollisionType> collision in collisions)
			{
				// We recreate an entity rectangle on every loop
				// because multiple collisions might affect the entity in the same frame.
				Rect entityRect = entity.CreateCollisionRectangle();
				Rect rect = collision.Key;
				CollisionType type = collision.Value;

				switch (type)
				{
					case CollisionType.Block:
						UndoCollision(entity, entityRect, rect);
						break;

						case CollisionType.Passable: break; // do nothing then

						default:
						throw new NotImplementedException("Collision type not implemented.");
				}
			}
		}

		/// <summary>
		/// Fixes a collision between an entity and a single object.
		/// </summary>
		static void UndoCollision(IEntity entity, Rect entityRect, Rect collided)
		{
			Debug.Assert(entity != null);
			Debug.Assert(entityRect != null);
			Debug.Assert(collided != null);

			Vec2 intersection = Utilities.GetIntersectionDepth(entityRect, collided);

			if (intersection != Vec2.Zero)
			{
				float abs_dept_x = Math.Abs(intersection.X);
				float abs_dept_y = Math.Abs(intersection.Y);

				// Resolve the collision on the axis where it will be less noticeable (the smallest collision amount)
				if (abs_dept_y < abs_dept_x) // collision on the y axis
				{
					entity.Position.Y += intersection.Y;

					if (intersection.Y < 0f) { // collided from the top side
						// We are therefore standing on something - we're on the ground
						entity.IsOnGround = true;
					}
					//TODO: only undo collision for platforms if we hit the ground

					// Only stop our movement if we're going straigth into the obstacle.
					if (Math.Sign(intersection.Y) == -Math.Sign(entity.Velocity.Y))
						entity.Velocity.Y = 0f; // stop our Y movement
				}
				else // collision on the x axis
				{
					entity.Position.X += intersection.X;
					entity.Velocity.X = 0f; // stop our X movement
				}
			}
		}
    }
}

