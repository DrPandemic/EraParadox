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
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATLib.Entities.Spells
{
	// Note: It is VERY important that the spell # here is the same as the one in the ChampionTypesHelper.GetSpellFromAction
	// function.
	// PLEASE MAKE SURE TO MODIFY THEM AT BOTH PLACES.
    public enum SpellTypes
    {
		// ManMega
		ManMega_RocketRampage = SpellsHelper.SPELL_1,
		ManMega_HintOfASpark = SpellsHelper.SPELL_3
    }

	public enum SpellKind
	{
		OffensiveSkillshot,
		DefensiveSkillshot
	}
	public class SpellInfo
	{
		public TimeSpan Cooldown { get; private set; }
		public TimeSpan CastingTime { get; private set; }
		public float Range { get; private set; }
		public float Speed { get; private set; }
		public float Width { get; private set; }
		public SpellKind Kind { get; private set; }
		public float Value { get; private set; }

		public SpellInfo(TimeSpan cooldown, TimeSpan cast, float range, 
		                 float speed, float width, SpellKind kind, float value)
		{
			Cooldown = cooldown;
			CastingTime = cast;
			Range = range;
			Speed = speed;
			Width = width;
			Kind = kind;
			Value = value;
		}
	}

	public static class SpellsHelper
	{
		public const int SPELL_1 = 1;
		public const int SPELL_2 = 2;
		public const int SPELL_3 = 3;
		public const int SPELL_4 = 4;
		const int NB_SPELLS = 4;
		const int MIN_SPELL_NUM = 1;
		const int MAX_SPELL_NUM = MIN_SPELL_NUM + NB_SPELLS - 1;

		static Dictionary<SpellTypes, SpellInfo> Spells = FillSpellsInfo();

		private static Dictionary<SpellTypes, SpellInfo> FillSpellsInfo()
		{
			var d = new Dictionary<SpellTypes, SpellInfo>();

			// ManMega
			d.Add(SpellTypes.ManMega_RocketRampage, new SpellInfo(
				TimeSpan.FromSeconds(1), 
				TimeSpan.FromSeconds(0.2),
				350f,
				900f,
				5f,
				SpellKind.OffensiveSkillshot,
				10f
				));
			d.Add(SpellTypes.ManMega_HintOfASpark, new SpellInfo(
				TimeSpan.FromSeconds(5),
				TimeSpan.FromSeconds(0.15),
				300f,
				800f,
				3f,
				SpellKind.DefensiveSkillshot,
				15f
				));

			return d;
		}

		public static SpellInfo Info(SpellTypes s)
		{
			if (Spells.ContainsKey(s))
				return Spells[s];
			else {
				ILogger.Log("Spell not implemented: " + s + ".");
				return Spells[SpellTypes.ManMega_RocketRampage]; // just a default value that is not relevant
			}
		}

		public static int SpellNumber(SpellTypes s)
		{
			int num = (int)s;
			Debug.Assert(num >= MIN_SPELL_NUM && num <= MAX_SPELL_NUM);
			return num;
		}
	}
}

