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
	public class StickManRangedSpellProjectile : Projectile
	{
		public override SpellTypes OwnerSpell { get { return SpellTypes.StickMan_RangedAttack; } }
		protected override float Speed { get { return 700f; } }
		protected override float Radius { get { return 10f; } }
		protected override float Range { get { return 300f; } }

		public StickManRangedSpellProjectile(Vec2 pos, Vec2 dir)
			: base(pos, dir)
		{
		}
	}
	public class StickManRangedSpell : ISpell
    {
		public override SpellTypes Type { get { return SpellTypes.StickMan_RangedAttack; } }


		public StickManRangedSpell()
			: base(TimeSpan.FromSeconds(2))
        {
        }

		protected override void OnActivate(IChampion owner, GameMatch match, IEntity target, Vec2 mouseDelta)
		{
			match.AddProjectile(new StickManRangedSpellProjectile(owner.GetSpellSpawnPos(), mouseDelta));
		}
    }
}
