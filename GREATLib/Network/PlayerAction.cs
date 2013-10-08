//
//  PlayerAction.cs
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

namespace GREATLib.Network
{
	/// <summary>
	/// Represents the action that a clients wants to make (e.g. go right, jump, etc.)
	/// </summary>
    public struct PlayerAction
    {
		/// <summary>
		/// Gets or sets the unique ID of the action.
		/// </summary>
		public ulong ID { get; private set; }

		/// <summary>
		/// The action that we're making (e.g. go right, jump, shoot, etc.) 
		/// </summary>
		public PlayerActionType Type { get; private set; }

		/// <summary>
		/// Gets or sets the time, in seconds, when the action was requested.
		/// </summary>
		public float Time { get; private set; }

		/// <summary>
		/// Gets the position of the player at the moment the action was executed.
		/// </summary>
		public Vec2 Position { get; private set; }
		public Vec2 Target { get; private set; }

		public PlayerAction(ulong id, PlayerActionType type, float time, Vec2 position, Vec2 target = null)
			: this()
		{
			ID = id;
			Type = type;
			Time = time;
			Position = position;
			Target = target;
		}
    }
}

