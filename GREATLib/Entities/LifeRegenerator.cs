//
//  LifeRegenerator.cs
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
    public class LifeRegenerator
    {
		private ILiving Entity { get; set; }

		/// <summary>
		/// Gets the duration of the heal.
		/// A null duration represents infinite heal.
		/// </summary>
		public TimeSpan? HealDuration { get; private set; }
		/// <summary>
		/// Time it takes for each heal to happen.
		/// </summary>
		public TimeSpan HealTick { get; private set; }
		/// <summary>
		/// How much we should heal for on every heal tick.
		/// </summary>
		public float HealValue { get; private set; }

		private TimeSpan TimeSinceLastHeal;
		private TimeSpan TimeSinceStart;

		public bool IsDone { get { return !HealDuration.HasValue || 
				TimeSinceStart.TotalSeconds > HealDuration.Value.TotalSeconds; } }
		private bool ShouldHeal { get { return TimeSinceLastHeal.TotalSeconds > HealTick.TotalSeconds; } }

        public LifeRegenerator(ILiving entity, TimeSpan? duration, TimeSpan tick,
		                       float value)
        {
			Entity = entity;

			HealDuration = duration;
			HealTick = tick;
			HealValue = value;

			TimeSinceLastHeal = TimeSinceStart = TimeSpan.Zero;
        }

		public void Update(TimeSpan dt)
		{
			TimeSinceStart += dt;
			TimeSinceLastHeal += dt;

			if (!IsDone && ShouldHeal) {
				Entity.Heal(HealValue);
			}
		}
    }
}

