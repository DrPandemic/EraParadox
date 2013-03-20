//
//  CharacterTypes.cs
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

//NOTE:
// Are you adding a new champion?
// Here is what you have to do:
// - Add it to the enum here.
// - Create a file representing it in the "Champions" folder deriving from
// the "Champion" class.
// - Add it to the class below (to make a link between the type and the data).

namespace Champions
{
	/// <summary>
	/// All the possible champions.
	/// </summary>
    public enum ChampionTypes
    {
		Stickman = 0
    }

	/// <summary>
	/// Manages the link between the champion type and the actual data.
	/// </summary>
	public static class ChampionFromType
	{
		/// <summary>
		/// Gets the champion data based on its type.
		/// </summary>
		/// <returns>The champion.</returns>
		/// <param name="type">Type.</param>
		public static Champion GetChampion(ChampionTypes type)
		{
			switch (type) {
				case ChampionTypes.Stickman:
					return new StickmanChampion();

				default: throw new NotImplementedException(
						"Champion type \"" + type.ToString() + "\" not implemented while getting it from its type.");
			}
		}
	}
}

