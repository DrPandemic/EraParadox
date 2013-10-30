//
//  ServerCommand.cs
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
	/// A command from the server.
	/// </summary>
	/// <remarks>The comments are written as if the server directly asked something to a client.</remarks>
    public enum ServerCommand
    {
		/// <summary>
		/// You just joined the game. Here is the data about your freshly created champion along with
		/// the data of all the players already in the game.
		/// </summary>
		JoinedGame = 0

		/// <summary>
		/// Here is an update of the state of all the entities that changed. Force your data to
		/// fit this.
		/// </summary>
		, StateUpdate

		/// <summary>
		/// A new player joined your game. Here is the data about his freshly created champion.
		/// </summary>
		, NewRemotePlayer

		/// <summary>
		/// A player has cast a spell.
		/// </summary>
		, SpellCast

		/// <summary>
		/// This spell should disappear.
		/// </summary>
		, SpellDisappear

		/// <summary>
		/// The stats of a player changed.
		/// </summary>
		, StatsChanged

		/// <summary>
		/// A player died.
		/// </summary>
		, ChampionDied

		/// <summary>
		/// The stats of a structure have changed.
		/// </summary>
		, StructureStatsChanged

		/// <summary>
		/// A structure has been destroyed.
		/// </summary>
		, StructureDestroyed

		/// <summary>
		/// The game has ended.
		/// </summary>
		, EndOfGame
    }
}

