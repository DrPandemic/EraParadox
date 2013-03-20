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

		/// <summary>
		/// Move the specified player in the specified direction.
		/// </summary>
		/// <param name="player">Player.</param>
		/// <param name="direction">Direction.</param>
		public static void Move(Player player, Direction direction)
		{
			player.Animation = (int)PlayerAnimation.Running;
			player.FacingLeft = direction == Direction.Left;
			player.Position += (float)direction * Vec2.UnitX * PLAYER_SPEED;
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

