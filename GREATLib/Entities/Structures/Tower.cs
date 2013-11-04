
//
//  Tower.cs
//
//  Author:
//       HPSETUP3 <${AuthorEmail}>
//
//  Copyright (c) 2013 HPSETUP3
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
using System.Diagnostics;

namespace GREATLib.Entities.Structures
{
    public class Tower : IStructure
    {
		private const float HEALTH = 100f;
		private const float WIDTH = 100f;
		private const float HEIGHT = 200f;

        public Tower(StructureTypes type, Teams team, Vec2 feetPos)
			: base(HEALTH,
			       type,
			       team,
			       new Rect(
				   	feetPos.X - WIDTH / 2f,
					feetPos.Y - HEIGHT,
					WIDTH,
					HEIGHT))
        {
			Debug.Assert(StructureHelper.IsTower(type));
        }
    }
}

