//
//  StructureTypes.cs
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

namespace GREATLib.Entities.Structures
{
    public enum StructureTypes
    {
		Base = 0,
		BaseTower,
		BottomTower,
		TopTower
    }

	public static class StructureHelper
	{
		public static bool IsTower(StructureTypes type)
		{
			return Utilities.MakeList(StructureTypes.BaseTower,
			                          StructureTypes.BottomTower,
			                          StructureTypes.TopTower).Contains(type);
		}
	}
}

