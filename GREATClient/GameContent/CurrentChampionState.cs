//
//  CurrentChampionState.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
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
using GREATLib.Entities.Spells;

namespace GameContent
{
	public class SpellCastInfo
	{
		public TimeSpan Cooldown { get; set; }
		private TimeSpan timeLeft;
		public TimeSpan TimeLeft {
			get { return timeLeft; }
			set {
				timeLeft = value;
				if (timeLeft.TotalSeconds < 0.0)
					timeLeft = TimeSpan.Zero;
			}
		}
		public SpellCastInfo(TimeSpan cooldown)
		{
			Cooldown = cooldown;
			TimeLeft = cooldown;
		}
	}
    public class CurrentChampionState
    {
		/// <summary>
		/// Gets or sets the max life.
		/// </summary>
		/// <value>The max life.</value>
		public float MaxLife { get; set; }

		/// <summary>
		/// Gets or sets the current life.
		/// </summary>
		/// <value>The current life.</value>
		public float CurrentLife { get; set; }

		/// <summary>
		/// Gets or sets the max resource.
		/// The resource is the mana/energie/etc.
		/// </summary>
		/// <value>The max resource.</value>
		public float MaxResource { get; set; }

		/// <summary>
		/// Gets or sets the current resource.
		/// The resource is the mana/energie/etc.
		/// </summary>
		/// <value>The current resource.</value>
		public float CurrentResource { get; set; }

		public SpellCastInfo Spell1 { get; set; }
		public SpellCastInfo Spell2 { get; set; }
		public SpellCastInfo Spell3 { get; set; }
		public SpellCastInfo Spell4 { get; set; }


		public CurrentChampionState(float maxLife, float maxResource)
        {
			MaxLife = maxLife;
			CurrentLife = maxLife;
			MaxResource = maxResource;
			CurrentResource = maxResource;

			Spell1 = Spell2 = Spell3 = Spell4 = new SpellCastInfo(TimeSpan.Zero);
        }

		public void SetSpellCooldown(SpellTypes spell, TimeSpan cooldown)
		{
			var castInfo = SpellCastInfo(spell);
			castInfo.Cooldown = cooldown;
			castInfo.TimeLeft = cooldown;
		}

		private SpellCastInfo SpellCastInfo(SpellTypes spell)
		{
			int spellNum = SpellsHelper.SpellNumber(spell);
			switch (spellNum) {
				case SpellsHelper.SPELL_1: return Spell1;
				case SpellsHelper.SPELL_2: return Spell2;
				case SpellsHelper.SPELL_3: return Spell3;
				case SpellsHelper.SPELL_4: return Spell4;
				default:
					throw new NotImplementedException("Spell number not supported.");
			}
		}

		public void Update(TimeSpan dt)
		{
			Spell1.TimeLeft -= dt;
			Spell2.TimeLeft -= dt;
			Spell3.TimeLeft -= dt;
			Spell4.TimeLeft -= dt;

			Console.WriteLine(Spell1.TimeLeft.TotalSeconds);
		}
    }
}

