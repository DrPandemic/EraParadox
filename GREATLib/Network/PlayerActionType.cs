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

namespace GREATLib.Network
{
	/// <summary>
	/// Represents a player action that a client may want to do in-game.
	/// </summary>
    public enum PlayerActionType
    {
		// No actions
		   Idle = 0

		// Movement
		,  MoveRight
		,  MoveLeft
		,  Jump

		// Abilities
		,  Spell1
		,  Spell2
		,  Spell3
		,  Spell4
	}

	public static class ActionTypeHelper
	{
		public static bool IsSpell(PlayerActionType a)
		{
			return PlayerActionType.Spell1 <= a && a <= PlayerActionType.Spell4;
		}
	}
}

