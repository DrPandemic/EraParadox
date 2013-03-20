//
//  Physics.cs
//
//  Author:
//       Jesse <${AuthorEmail}>
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
using Map;
using Champions;
using System.Collections.Generic;

namespace GREATLib
{
	/// <summary>
	/// The game physics.
	/// </summary>
    public static class Physics
    {
		/// <summary>
		/// TODO: Speed depending on the player's class and actual movement speed.
		/// </summary>
		private const float PLAYER_SPEED = 5f;

		private static readonly Vec2 GRAVITY = new Vec2(0f, 9f);

		/// <summary>
		/// Move the specified player in the specified direction.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="direction">Direction.</param>
		/// <param name="map">The map to undo the collisions.</param>
		public static void Move(Player player, Direction direction, TileMap map)
		{
			player.Animation = (int)PlayerAnimation.Running;
			player.FacingLeft = direction == Direction.Left;
			player.Position += (float)direction * Vec2.UnitX * PLAYER_SPEED;
			UndoCollisions(player, map);
		}

		/// <summary>
		/// Applies the physics to the players.
		/// </summary>
		/// <param name="players">Players.</param>
		/// <param name="map">Map.</param>
		public static void ApplyPhysics(IEnumerable<Player> players, TileMap map)
		{
			foreach (Player player in players) {
				player.Position += GRAVITY;
				UndoCollisions(player, map);
			}
		}

		/// <summary>
		/// Fixes the collisions between the player and the map.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="map">Map.</param>
		private static void UndoCollisions(Player player, TileMap map)
		{
			Champion champ = ChampionFromType.GetChampion((ChampionTypes)player.Champion);

			List<Tile> touched = map.GetTouchedTiles(
				(int)GetPlayerLeft(player, champ),
				(int)GetPlayerTop(player, champ),
				champ.CollisionWidth,
				champ.CollisionHeight);

			foreach (Tile tile in touched) {
				if (TileData.IsSolid(tile.Id)) {
					float left = player.Position.X - champ.CollisionWidth / 2f; // get the left side (we're at the center (the feet))
					float top = (int)player.Position.Y - champ.CollisionHeight; // get the top side (we're at the bottom (the feet))

					UndoCollision(player, GetPlayerLeft(player, champ), GetPlayerTop(player, champ), champ.CollisionWidth, champ.CollisionHeight,
				    	          tile.X * Tile.WIDTH, tile.Y * Tile.HEIGHT, Tile.WIDTH, Tile.HEIGHT);
				}
			}
		}

		/// <summary>
		/// Gets the player left side position.
		/// </summary>
		/// <returns>The player left.</returns>
		/// <param name="player">Player.</param>
		/// <param name="champ">Champ.</param>
		private static float GetPlayerLeft(Player player, Champion champ)
		{
			return player.Position.X - champ.CollisionWidth / 2f;
		}

		/// <summary>
		/// Gets the player top side position.
		/// </summary>
		/// <returns>The player top.</returns>
		/// <param name="player">Player.</param>
		/// <param name="champ">Champ.</param>
		private static float GetPlayerTop(Player player, Champion champ)
		{
			return player.Position.Y - champ.CollisionHeight;
		}

		/// <summary>
		/// Fixes a single collision between A (player) and B (other rectangle).
		/// </summary>
		private static void UndoCollision(Player player, float leftA, float topA, int widthA, int heightA,
		                                  float leftB, float topB, int widthB, int heightB)
		{
			Vec2 intersection = GetIntersectionDepth(leftA, topA, widthA, heightA,
			                                         leftB, topB, widthB, heightB);

			if (intersection != Vec2.Zero) { // there was a collision
				float abs_depth_x = Math.Abs(intersection.X);
				float abs_depth_y = Math.Abs(intersection.Y);

				// Resolve the collision on the axis where it will be the less noticeable (the smallest collision amount)
				if (abs_depth_y < abs_depth_x) {
					player.Position += Vec2.UnitY * intersection.Y;
				}
				else {
					player.Position += Vec2.UnitX * intersection.X;
				}
			}
		}

		/// <summary>
		/// Gets the intersection depth between A and B.
		/// </summary>
		/// <returns>The intersection depth.</returns>
		private static Vec2 GetIntersectionDepth(float leftA, float topA, int widthA, int heightA,
		                                         float leftB, float topB, int widthB, int heightB)
		{
			// Calculate half sizes.
			float halfW_a = widthA / 2f;
			float halfH_a = heightA / 2f;
			float halfW_b = widthB / 2f;
			float halfH_b = heightB / 2f;

			// Calculate centers.
			Vec2 center_a = new Vec2(leftA + halfW_a, topA + halfH_a);
			Vec2 center_b = new Vec2(leftB + halfW_b, topB + halfH_b);

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

		/// <summary>
		/// Update the specified player's animation.
		/// Note: This must be called before the move calls, because it resets
		/// an animation state that will be changed right after if it should.
		/// </summary>
		/// <param name="player">Player.</param>
		public static void UpdateAnimation(Player player)
		{
			// Assume that we stopped running (we'll receive an other message otherwise).
			if (player.Animation == (int)PlayerAnimation.Running) 
				player.Animation = (int)PlayerAnimation.Standing;
		}
    }
}

