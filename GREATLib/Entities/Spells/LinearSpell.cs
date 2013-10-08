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
		public SpellTypes Type { get; set; }
		public float Range { get; set; }
		public TimeSpan Cooldown { get; set; }
		public TimeSpan CastingTime { get; set; }
		public float Damage { get; set; }

        public LinearSpell(ulong id, Vec2 position, Vec2 target, SpellTypes type)
			: base(id, position,
			       100f, 5f, 5f) //TODO: depend on spell type
        {
			//TODO: depend on spell type
			Range = 100f;
			Cooldown = TimeSpan.FromSeconds(5.0);
			CastingTime = TimeSpan.FromSeconds(0.25);
			Damage = 10f;

			Type = type;
			Velocity = Vec2.Normalize(target - position) * MoveSpeed;
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
			LinearSpell s = new LinearSpell(ID, Position, Position + Velocity, Type);
			s.Clone(this);
			return s;
		}
    }
}

