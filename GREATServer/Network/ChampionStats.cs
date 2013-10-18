//
//  ChampionStats.cs
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
using GREATLib.Entities.Spells;
using GREATLib.Entities.Champions;
using GREATLib.Network;

namespace GREATServer.Network
{
    public class ChampionStats
    {
		public float Health { get; private set; }
		public float MaxHealth { get; private set; }
		public bool Alive { get { return Health > 0f; } }
		public bool HealthChanged { get; private set; }
		Dictionary<SpellTypes, float> LastSpellUses { get; set; }
		public double RevivalTime { get; set; }

        public ChampionStats(float maxhp)
        {
			MaxHealth = maxhp;
			Health = MaxHealth;
			LastSpellUses = new Dictionary<SpellTypes, float>();
			RevivalTime = double.MaxValue;
			ClearHealthChangedFlag();
        }

		public void Heal(float amount)
		{
			SetHealth(Health + amount);
		}
		public void Hurt(float amount)
		{
			SetHealth(Health - amount);
		}
		public void SetHealth(float amount)
		{
			if (amount != Health) {
				HealthChanged = true;
			}
			Health = Math.Min(MaxHealth, Math.Max(0f, amount));
		}
		public void ClearHealthChangedFlag()
		{
			HealthChanged = false;
		}
		public void UsedSpell(SpellTypes spell)
		{
			float time = (float)Server.Instance.GetTime().TotalSeconds;
			if (LastSpellUses.ContainsKey(spell))
				LastSpellUses[spell] = time;
			else
				LastSpellUses.Add(spell, time);
		}
		TimeSpan TimeOfLastSpellUse(SpellTypes spell)
		{
			return TimeSpan.FromSeconds(LastSpellUses.ContainsKey(spell) ? LastSpellUses[spell] : 0f);
		}
		public bool IsCastingSpell(ChampionTypes champ, PlayerActionType action)
		{
			SpellTypes spell = ChampionTypesHelper.GetSpellFromAction(champ, action);
			return TimeOfLastSpellUse(spell).TotalSeconds +
				SpellsHelper.CastingTime(spell).TotalSeconds >= Server.Instance.GetTime().TotalSeconds;
		}
		public bool ShouldRespawn()
		{
			return Server.Instance.GetTime().TotalSeconds >= RevivalTime;
		}
		public bool IsOnCooldown(SpellTypes spell)
		{
			return TimeOfLastSpellUse(spell).TotalSeconds +
				SpellsHelper.Cooldown(spell).TotalSeconds > Server.Instance.GetTime().TotalSeconds;
		}
    }
}

