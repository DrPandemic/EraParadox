//
//  ILiving.cs
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

namespace GREATLib.Entities
{
    public class ILiving
    {
		public float Health { get; private set; }
		public float MaxHealth { get; private set; }
		public bool Alive { get { return Health > 0f; } }
		private bool HealthChanged { get; set; }

        public ILiving(float maxhp)
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
		/// <summary>
		/// Gets whether the health recently changed or not, clearing the flag
		/// at the same time (which implies that this function must NOT be called multiple
		/// times within the same frame, or it will be useless).
		/// </summary>
		/// <returns><c>true</c>, if health changed was gotten, <c>false</c> otherwise.</returns>
		public bool GetHealthChangedAndClearFlag()
		{
			bool changed = HealthChanged;
			ClearHealthChangedFlag();
			return changed;
		}
		private void ClearHealthChangedFlag()
		{
			HealthChanged = false;
		}
    }
}

