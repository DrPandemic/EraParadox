//
//  Projectile.cs
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
using GREATLib.Entities.Physics;

namespace GREATLib.Entities.Player.Spells
{
    public abstract class Projectile : MovingEntity
    {
		public abstract SpellTypes OwnerSpell { get; }
		protected abstract float Speed { get; }
		protected abstract float Radius { get; }
		protected abstract float Range { get; }

		protected override float DefaultCollisionWidth { get { return Radius; } }
		protected override float DefaultCollisionHeight { get { return Radius; } }

		Vec2 StartPosition { get; set; }

        public Projectile(Vec2 position, Vec2 direction)
        {
			StartPosition = position;
			Position = position;
			Velocity = Vec2.Normalize(direction) * Speed;
        }

		public void Update(double deltaSeconds, GameMatch match)
		{
			Position += Velocity * (float)deltaSeconds;

			if (Vec2.DistanceSquared(Position, StartPosition) >= Range * Range) {
				RemoveMe = true; // We're done
			}

			//TODO: call OnAllyHit and OnEnemyHit on collision with LivingEntities
		}

		public float GetDistanceLeft()
		{
			return Range - Vec2.Distance(Position, StartPosition);
		}
    }
}

