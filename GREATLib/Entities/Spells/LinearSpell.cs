//
//  LinearSpell.cs
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

namespace GREATLib.Entities.Spells
{
    public class LinearSpell : IEntity
    {
		public ICharacter Owner { get; private set; }
		public SpellTypes Type { get; set; }
		public float Range { get; set; }
		public TimeSpan Cooldown { get; set; }
		public TimeSpan CastingTime { get; set; }
		public float Damage { get; set; }

		Vec2 StartPosition { get; set; }
		public bool IsSolid { get; private set; }

		public LinearSpell(ulong id, ICharacter owner, Vec2 position, Vec2 target, SpellTypes type)
			: base(id, position,
			       900f, 5f, 5f) //TODO: depend on spell type
        {
			//TODO: depend on spell type
			Range = 350f;
			Cooldown = TimeSpan.FromSeconds(5.0);
			CastingTime = TimeSpan.FromSeconds(0.25);
			Damage = 10f;
			IsSolid = true;

			Type = type;
			Velocity = Vec2.Normalize(target - position) * MoveSpeed;
			StartPosition = (Vec2)position.Clone();
			Owner = owner;
        }

		public override void Clone(IEntity e)
		{
			LinearSpell s = (LinearSpell)e;
			base.Clone(s);
			Type = s.Type;
			Range = s.Range;
			Cooldown = s.Cooldown;
			CastingTime = s.CastingTime;
			Damage = s.Damage;
		}
		public override object Clone()
		{
			LinearSpell s = new LinearSpell(ID, Owner, Position, Position + Velocity, Type);
			s.Clone(this);
			return s;
		}

		public bool ReachedMaxRange()
		{
			return Vec2.DistanceSquared(Position, StartPosition) >= Range * Range;
		}
    }
}

