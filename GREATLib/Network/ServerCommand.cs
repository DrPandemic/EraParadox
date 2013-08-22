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
		/// Here is a new player to add to your current game.
		/// </summary>
		NewPlayer = 10

		/// <summary>
		/// Here is a position update of all the entities that changed.
		/// </summary>
		, PositionUpdate
    }
}

