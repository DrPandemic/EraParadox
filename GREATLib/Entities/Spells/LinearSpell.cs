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
		public Teams Team { get; private set; }
		public ICharacter Owner { get; private set; }
		public SpellTypes Type { get; set; }
		public SpellInfo Info { get; set; }

		Vec2 StartPosition { get; set; }
		public bool IsSolid { get; private set; }

		public LinearSpell(ulong id, Teams team, Vec2 position, Vec2 target, SpellTypes type, ICharacter owner)
			: base(id, position,
			       SpellsHelper.Info(type).Speed, 
			       SpellsHelper.Info(type).Width, SpellsHelper.Info(type).Width)
        {
			Info = SpellsHelper.Info(type);

			IsSolid = Info.Solid;

			Type = type;
			Velocity = Vec2.Normalize(target - position) * MoveSpeed;
			StartPosition = (Vec2)position.Clone();
			Team = team;
			Owner = owner;
        }

		public override void Clone(IEntity e)
		{
			LinearSpell s = (LinearSpell)e;
			base.Clone(s);
			Type = s.Type;
		}
		public override object Clone()
		{
			LinearSpell s = new LinearSpell(ID, Team, Position, Position + Velocity, Type, Owner);
			s.Clone(this);
			return s;
		}

		public bool ReachedMaxRange()
		{
			return Vec2.DistanceSquared(Position, StartPosition) >= Info.Range * Info.Range;
		}
    }
}

