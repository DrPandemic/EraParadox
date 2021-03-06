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
using GREATLib.Entities;

namespace GREATServer.Network
{
    public class ChampionStats : ILiving
    {
		static readonly TimeSpan TIME_FOR_OUT_OF_COMBAT = TimeSpan.FromSeconds(10.0);

		Dictionary<SpellTypes, float> LastSpellUses { get; set; }
		public double RevivalTime { get; set; }
		public ulong? Killer { get; set; }
		public double TimeWhenLastEnemyHurtUs { get; set; }
		public uint Kills { get; set; }
		public uint Deaths { get; set; }
		public bool InCombat { get { return Killer.HasValue; } }

        public ChampionStats(float maxhp)
			: base(maxhp)
        {
			LastSpellUses = new Dictionary<SpellTypes, float>();
			RevivalTime = double.MaxValue;
			Killer = null;
			Kills = 0;
			Deaths = 0;
			TimeWhenLastEnemyHurtUs = 0.0;
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
				SpellsHelper.Info(spell).CastingTime.TotalSeconds >= Server.Instance.GetTime().TotalSeconds;
		}
		public bool ShouldRespawn()
		{
			return Server.Instance.GetTime().TotalSeconds >= RevivalTime;
		}
		public bool IsOnCooldown(SpellTypes spell)
		{
			TimeSpan lastUse = TimeOfLastSpellUse(spell);
			return lastUse == TimeSpan.Zero ? false : 
				lastUse.TotalSeconds + SpellsHelper.Info(spell).Cooldown.TotalSeconds > 
					Server.Instance.GetTime().TotalSeconds;
		}
		public bool ShouldGoOutOfCombat()
		{
			return InCombat &&
				TimeWhenLastEnemyHurtUs + TIME_FOR_OUT_OF_COMBAT.TotalSeconds <= Server.Instance.GetTime().TotalSeconds;
		}
		public void GoOutOfCombat()
		{
			Killer = null;
		}
		public void GoInCombat(ulong enemy)
		{
			Killer = (ulong?)enemy;
			TimeWhenLastEnemyHurtUs = Server.Instance.GetTime().TotalSeconds;
		}
    }
}

