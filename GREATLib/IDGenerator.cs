//
//  IDGenerator.cs
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

namespace GREATLib
{
	/// <summary>
	/// Unique identifier generator. New IDs are guarenteed to be higher than older ones, except if the
	/// value EVER exceeds the maximum amount of an unsigned int, then it wraps to 0.
	/// </summary>
    public static class IDGenerator
    {
		const ulong START_ID = 1u;
		public const ulong NO_ID = 0u;

		/// <summary>
		/// Gets or sets the current ID.
		/// </summary>
		static ulong CurrentID { get; set; }

		static IDGenerator()
		{
			CurrentID = START_ID;
		}

		/// <summary>
		/// Generates a unique ID incrementally bigger as time goes on.
		/// </summary>
		public static ulong GenerateID()
		{
			try {
				CurrentID = checked (CurrentID+1); // check for overflow ( http://msdn.microsoft.com/en-us/library/74b4xzyw(v=vs.71).aspx )
			} catch (OverflowException) {
				CurrentID = START_ID;
			}
			return CurrentID;
		}
    }
}

