//
//  Character.cs
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
using GREATLib.Entities.Champions;

namespace GREATLib.Entities
{
    public class ICharacter : IEntity
    {
		/// <summary>
		/// Gets or sets the jump force of the entity.
		/// </summary>
		public short JumpForce { get; set; }
		/// <summary>
		/// Gets or sets the horizontal acceleration of the entity, which is how much of our horizontal velocity
		/// we maintain per second.
		/// </summary>
		/// <example>0.9 would keep 90% of the entity's X velocity every frame.</example>
		public float HorizontalAcceleration { get; set; }

		public float Health { get; private set; }
		public float MaxHealth { get; private set; }
		public bool Alive { get { return Health > 0f; } }
		public ChampionAnimation Animation { get; set; }
		public bool FacingLeft { get; set; }
		public Teams Team { get; private set; }

        public ICharacter(ulong id, Vec2 position, Teams team, float maxhp, float hp)
			: base(id, position, 
			       90f, 15f, 30f)//TODO: stats by champion
        {
			JumpForce = 750;
			HorizontalAcceleration = 9e-9f;

			MaxHealth = maxhp;
			Health = hp;
			Animation = ChampionAnimation.idle;
			FacingLeft = false;
			Team = team;
        }

		public void Heal(float amount)
		{
			Health = Math.Min(MaxHealth, Health + amount);
		}
		public void Hurt(float amount)
		{
			Health = Math.Max(0f, Health - amount);
		}

		public override void Clone(IEntity e)
		{
			ICharacter c = (ICharacter)e;
			base.Clone(c);
			JumpForce = c.JumpForce;
			HorizontalAcceleration = c.HorizontalAcceleration;
			Animation = c.Animation;
			FacingLeft = c.FacingLeft;
			MaxHealth = c.MaxHealth;
			Health = c.Health;
		}
		public override object Clone()
		{
			ICharacter c = new ICharacter(ID, Position, Team, MaxHealth, Health);
			c.Clone(this);
			return c;
		}

		const float HANDS_MARGIN = 0.2f;
        public Vec2 GetHandsPosition()
        {
            return new Vec2 (Position.X + CollisionWidth / 2f,
                            Position.Y + CollisionHeight / 2f) + // center
                (FacingLeft ? -1 : 1) * new Vec2 (CollisionWidth / 2f, 0f) + // on the side that we're looking
				(FacingLeft ? -1 : 1) * new Vec2(CollisionWidth, 0f) * HANDS_MARGIN; // plus a bit of over (for our hands)
        }

		public Vec2 GetFeetPosition()
		{
			return new Vec2(Position.X + CollisionWidth / 2f,
			                Position.Y + CollisionHeight);
		}
    }
}

