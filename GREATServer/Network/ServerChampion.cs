//
//  ServerClient.cs
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
using GREATLib.Entities;
using GREATLib;
using GREATLib.Entities.Champions;

namespace GREATServer.Network
{
    public class ServerChampion : IEntity
    {
		public bool CastSpell1 { get; set; }
		public bool CastSpell2 { get; set; }
		public bool CastSpell3 { get; set; }
		public bool CastSpell4 { get; set; }
		public int Movement { get; set; }

		public ServerChampion(uint id, Vec2 pos)
			: base(id, pos)
        {
			ResetAnimInfo();
        }

		public void ResetAnimInfo()
		{
			CastSpell1 = CastSpell2 = CastSpell3 = CastSpell4 = false;
			Movement = 0;
		}

		public ChampionAnimation GetAnim(float health, bool onGround)
		{
			// Dead champion
			if (health <= 0f) return ChampionAnimation.die;

			// Casting spell
			if (CastSpell1) return ChampionAnimation.spell1;
			if (CastSpell2) return ChampionAnimation.spell2;
			if (CastSpell3) return ChampionAnimation.spell3;
			if (CastSpell4) return ChampionAnimation.spell4;

			// Falling/jumping
			if (!onGround) return ChampionAnimation.jump;

			// Moving left/right
			if (Movement != 0) return ChampionAnimation.run;

			// Default to idle
			return ChampionAnimation.idle;
		}

		public override object Clone()
		{
			ServerChampion s = new ServerChampion(ID, Position);
			s.Clone(this);
			return s;
		}
		public override void Clone(IEntity champ)
		{
			ServerChampion c = (ServerChampion)champ;

			base.Clone(c);
			CastSpell1 = c.CastSpell1;
			CastSpell2 = c.CastSpell2;
			CastSpell3 = c.CastSpell3;
			CastSpell4 = c.CastSpell4;
			Movement = c.Movement;
		}
    }
}

