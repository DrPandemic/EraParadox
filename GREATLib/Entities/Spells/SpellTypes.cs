//
//  SpellTypes.cs
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

namespace GREATLib.Entities.Spells
{
    public enum SpellTypes
    {
		StickManSpell1
    }

	public static class SpellsHelper
	{
		public static TimeSpan Cooldown(SpellTypes s)
		{
			switch (s)
			{
				case SpellTypes.StickManSpell1:
					return TimeSpan.FromSeconds(1);

				default:
					ILogger.Log("No cooldown implemented for spell " + s + ".");
					return new TimeSpan();
			}
		}

		public static TimeSpan CastingTime(SpellTypes s)
		{
			switch (s) {
				case SpellTypes.StickManSpell1:
					return TimeSpan.FromSeconds(0.25);

				default:
					ILogger.Log("No casting time implemented for spell " + s + ".");
					return new TimeSpan();
			}
		}
	}
}

