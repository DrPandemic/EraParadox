//
//  ClientCommandEvent.cs
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
using Lidgren.Network;

namespace GREATClient
{
	/// <summary>
	/// Class containing all the things to do when a new server event comes in.
	/// </summary>
	public sealed class ServerCommandEvent
    {
		Func<NetBuffer, CommandEventArgs> CreateEventArgs { get; set; }
		public EventHandler<CommandEventArgs> Handler { get; set; }
		Action<CommandEventArgs> OnExecute { get; set; }

		public ServerCommandEvent(Func<NetBuffer, CommandEventArgs> createEventArgs,
		                          Action<CommandEventArgs> onExecute = null)
		{
			Handler = null;
			OnExecute = onExecute;
			CreateEventArgs = createEventArgs;
		}

		/// <summary>
		/// Execute the specified command.
		/// </summary>
		/// <returns>Whether the command was executed or not (if the handler was set).</returns>
		public bool Execute(NetBuffer message)
		{
			if (Handler != null) {
				CommandEventArgs e = CreateEventArgs(message);
				Handler(null, e);
				if (OnExecute != null) {
					OnExecute(e);
				}
			}

			return Handler != null;
		}
    }
}

