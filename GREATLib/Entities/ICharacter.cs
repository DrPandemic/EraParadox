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

		public ChampionAnimation Animation { get; set; }
		public bool FacingLeft { get; set; }

        public ICharacter(uint id, Vec2 position)
			: base(id, position, 
			       90f, 15f, 30f)//TODO: stats by champion
        {
			JumpForce = 750;
			HorizontalAcceleration = 9e-9f;

			Animation = ChampionAnimation.idle;
			FacingLeft = false;
        }

		public override void Clone(IEntity e)
		{
			ICharacter c = (ICharacter)e;
			base.Clone(c);
			JumpForce = c.JumpForce;
			HorizontalAcceleration = c.HorizontalAcceleration;
			Animation = c.Animation;
			FacingLeft = c.FacingLeft;
		}
		public override object Clone()
		{
			ICharacter c = new ICharacter(ID, Position);
			c.Clone(this);
			return c;
		}
    }
}

