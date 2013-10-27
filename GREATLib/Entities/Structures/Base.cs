//
//  Base.cs
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

namespace GREATLib.Entities.Structures
{
    public class Base : IStructure
    {
		private const float HEALTH = 250f;
		private const float WIDTH = 100f;
		private const float HEIGHT = 100f;

		private static readonly TimeSpan RegenTick = TimeSpan.FromSeconds(15.0);
		private const float HEALTH_REGEN = 1f;

		private LifeRegenerator Regen { get; set; }

        public Base(Vec2 feetPos)
			: base(HEALTH,
			       new Rect(
					feetPos.X - WIDTH / 2f,
					feetPos.Y - HEIGHT,
			        WIDTH,
			        HEIGHT))
        {
			Regen = new LifeRegenerator(this,
			                            null,
			                            RegenTick, HEALTH_REGEN);
        }

		public override void Update(TimeSpan dt)
		{
			Regen.Update(dt);
		}
    }
}

