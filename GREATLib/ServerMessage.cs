//
//  ServerMessage.cs
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
	/// Represents a message sent by the server.
	/// </summary>
    public enum ServerMessage
    {
		/// <summary>
		/// Sends the positions of all the other players.
		/// </summary>
		PositionSync = 0x01,
		/// <summary>
		/// Gives the id of a client's player.
		/// FIXME: No need for that
		/// </summary>
		//GivePlayerId = 0x02,
		/// <summary>
		/// Acknowledges a client's command (used for client-side prediction).
		/// </summary>
		AcknowledgeCommand = 0x03
    }
}

