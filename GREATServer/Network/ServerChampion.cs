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
	public class ChampionAnimData
	{
		public int Movement { get; set; }
		public bool Idle { get; set; }

		public ChampionAnimData()
		{
			Reset();
		}

		public void Reset()
		{
			Movement = 0;
			Idle = true;
		}
	}

    public class ServerChampion : ICharacter
    {
		public ServerChampion(ulong id, Vec2 pos, Teams team)
			: base(id, pos, team)
        {
        }

		public ChampionAnimation GetAnim(bool dead, bool onGround,
		                                 bool castingSpell1,
		                                 bool castingSpell2,
		                                 bool castingSpell3,
		                                 bool castingSpell4,
		                                 int movement,
		                                 bool isIdle,
		                                 ChampionAnimation oldAnim)
		{
			// Dead champion
			if (dead) return ChampionAnimation.die;

			// Casting spell
			if (castingSpell1) return ChampionAnimation.spell1;
			if (castingSpell2) return ChampionAnimation.spell2;
			if (castingSpell3) return ChampionAnimation.spell3;
			if (castingSpell4) return ChampionAnimation.spell4;

			// Falling/jumping
			if (!onGround) return ChampionAnimation.jump;

			// Moving left/right
			if (movement != 0) return ChampionAnimation.run;

			// No recent actions
			if (isIdle) return ChampionAnimation.idle;

			// No updates since, keep our previous animation
			return oldAnim;
		}

		public override object Clone()
		{
			ServerChampion s = new ServerChampion(ID, Position, Team);
			s.Clone(this);
			return s;
		}
		public override void Clone(IEntity champ)
		{
			ServerChampion c = (ServerChampion)champ;
			base.Clone(c);
		}
    }
}

