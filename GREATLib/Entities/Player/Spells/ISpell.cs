//
//  ISpell.cs
//
//  Author:
//       Jesse <>
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
using GREATLib;
using GREATLib.Entities;
using GREATLib.Entities.Physics;
using GREATLib.Entities.Player.Champions;

namespace GREATLib.Entities.Player.Spells
{
	/// <summary>
	/// Represents a champion's spell.
	/// Note: this spell does not represent any moving entity, but only the spell itself.
	/// If it should create a projectile, it will do so when activated.
	/// </summary>
    public abstract class ISpell : ISynchronizable
    {
		public abstract SpellTypes Type { get; }

		/// <summary>
		/// Gets or sets the cooldown, which is the time it takes to cast this spell a second time.
		/// </summary>
		/// <value>The cooldown.</value>
		public TimeSpan Cooldown { get; private set; }

		/// <summary>
		/// Gets or sets the time left on cooldown.
		/// </summary>
		/// <value>The time spent on cooldown.</value>
		private TimeSpan TimeLeftOnCooldown { get; set; }

		/// <summary>
		/// Gets a value indicating whether this instance is on cooldown.
		/// </summary>
		/// <value><c>true</c> if this instance is on cooldown; otherwise, <c>false</c>.</value>
		public bool IsOnCooldown { get { return TimeLeftOnCooldown.TotalMilliseconds > 0.0; } }

		protected bool Activated { get; set; }

        public ISpell(TimeSpan cooldown)
        {
			Activated = false;
			Cooldown = cooldown;
			TimeLeftOnCooldown = new TimeSpan();
        }

		/// <summary>
		/// Activates the spell, when the player casts a spell.
		/// </summary>
		/// <param name="match">The match, used by the spell to create its effect.</param>
		/// <param name="target">The target (which can be null if it is nontargettable) to apply the spell to.</param>
		/// <param name="mouseDelta">The delta vector showing the difference from the champion to the mouse (the direction it points to).</param>
		/// <returns>Whether the spell was casted or not.</returns>
		public bool Activate(IChampion owner, GameMatch match, IEntity target, Vec2 mouseDelta)
		{
			bool casted = false;
			if (!IsOnCooldown)
			{
				casted = true;
				Activated = true;
				TimeLeftOnCooldown = Cooldown;
				OnActivate(owner, match, target, mouseDelta);
			}

			return casted;
		}

		public void Update(double deltaSeconds)
		{
			if (IsOnCooldown)
			{
				if (TimeLeftOnCooldown.TotalSeconds <= deltaSeconds) // cooldown is over
					TimeLeftOnCooldown = TimeSpan.Zero;
				else // make it go down
					TimeLeftOnCooldown = TimeLeftOnCooldown.Subtract(TimeSpan.FromSeconds(deltaSeconds));

				Console.WriteLine(TimeLeftOnCooldown.TotalMilliseconds);
			}

			if (Activated)
				OnUpdate(deltaSeconds);
		}

		protected virtual void OnUpdate(double deltaSeconds)
		{
		}

		protected abstract void OnActivate(IChampion owner, GameMatch match, IEntity target, Vec2 mouseDelta);
    }
}
