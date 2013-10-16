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

namespace GREATServer.Network
{
    public class ChampionStats
    {
		public float Health { get; private set; }
		public float MaxHealth { get; private set; }
		public bool Alive { get { return Health > 0f; } }
		public bool HealthChanged { get; private set; }

        public ChampionStats(float maxhp)
        {
			MaxHealth = maxhp;
			Health = MaxHealth;
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
    }
}

