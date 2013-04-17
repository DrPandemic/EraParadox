//
//  CollisionHandler.cs
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
using GREATLib.World;
using System.Collections.Generic;
using GREATLib.World.Tiles;

namespace GREATLib.Entities.Physics
{
	/// <summary>
	/// The system in charge of handling collisions happening between the game entities
	/// and the game world.
	/// </summary>
    public class CollisionHandler
    {
		/// <summary>
		/// Resolves the collisions between an entity and the game world.
		/// </summary>
		/// <param name="entity">Entity.</param>
		/// <param name="world">World.</param>
		public void HandleCollisions(PhysicsEntity entity, GameWorld world)
		{
			List<KeyValuePair<Rect, CollisionType>> collisions = 
				world.GetTouchedObjects(entity.GetRectangle());

			foreach (KeyValuePair<Rect, CollisionType> collision in collisions)
			{
				// Note: we recreate an entity rectangle on every loop
				// because multiple collisions might affect the entity in the same frame.
				Rect entityRect = entity.GetRectangle();
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

		private void UndoCollision(PhysicsEntity entity, Rect entityRect,
		                           Rect collided)
		{
			Vec2 intersection = GetIntersectionDepth(entityRect, collided);

			if (intersection != Vec2.Zero)
			{
				float abs_dept_x = Math.Abs(intersection.X);
				float abs_dept_y = Math.Abs(intersection.Y);

				// Resolve the collision on the axis where it will be less noticeable (the smallest collision amount)
				if (abs_dept_y < abs_dept_x) // collision on the y axis
				{
					entity.Position.Y += intersection.Y;

					//TODO: check to hit the ground here
					//TODO: only undo collision for platforms if we hit the ground

					entity.Velocity.Y = 0f; // stop our Y movement
				}
				else // collision on the x axis
				{
					entity.Position.X += intersection.X;
					entity.Velocity.X = 0f; // stop our X movement
				}
			}
		}

		private Vec2 GetIntersectionDepth(Rect A, Rect B)
		{
			// Calculate half sizes.
			float halfW_a = A.Width / 2f;
			float halfH_a = A.Height / 2f;
			float halfW_b = B.Width / 2f;
			float halfH_b = B.Height / 2f;
			
			// Calculate centers.
			Vec2 center_a = new Vec2(A.Left + halfW_a, A.Top + halfH_a);
			Vec2 center_b = new Vec2(B.Left + halfW_b, B.Top + halfH_b);
			
			// Calculate current and minimum non-intersecting distances between centers.
			float distance_x = center_a.X - center_b.X;
			float distance_y = center_a.Y - center_b.Y;
			float min_distance_x = halfW_a + halfW_b;
			float min_distance_y = halfH_a + halfH_b;
			
			// If we're not intersecting at all, return (0,0).
			if (Math.Abs(distance_x) >= min_distance_x || Math.Abs(distance_y) >= min_distance_y)
				return Vec2.Zero;
			
			// Calculate and return intersection depths.
			float depth_x = distance_x > 0 ? min_distance_x - distance_x : -min_distance_x - distance_x;
			float depth_y = distance_y > 0 ? min_distance_y - distance_y : -min_distance_y - distance_y;
			return new Vec2(depth_x, depth_y);
		}
    }
}

