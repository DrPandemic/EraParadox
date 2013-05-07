//
//  StickManRangedAttack.cs
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
using GREATLib.Entities.Player.Champions;

namespace GREATLib.Entities.Player.Spells.AllSpells
{
	public class StickManRangedSpell : ISpell
    {
		Vec2 direction;
		IChampion owner;
		public override SpellTypes Type { get { return SpellTypes.StickMan_RangedAttack; } }
		double timeDashing = 0;
		static readonly TimeSpan TimeDashing = TimeSpan.FromSeconds(0.1);


		public StickManRangedSpell()
			: base(TimeSpan.FromSeconds(2))
        {
        }

		protected override void OnActivate(IChampion owner, GameMatch match, IEntity target, Vec2 mouseDelta)
		{
			this.owner = owner;
			//TODO: create a projectile, just jump the player for now
			Vec2 dir = Vec2.Normalize(mouseDelta);
			direction = dir;
			owner.Velocity.X += dir.X * 200f;
			owner.Velocity.Y += dir.Y * 500f;
			timeDashing = 0.0;
		}

		protected override void OnUpdate(double deltaSeconds)
		{
			base.OnUpdate(deltaSeconds);
			owner.Velocity.X += direction.X * 200f;
	
			timeDashing += deltaSeconds;
			if (timeDashing >= TimeDashing.TotalSeconds)
				Activated = false;
		}
    }
}
