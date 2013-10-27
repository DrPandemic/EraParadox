//
//  TeamStructures.cs
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
using GREATLib.Entities.Structures;
using GREATLib;
using System.Collections.Generic;
using GREATLib.World.Tiles;

namespace GREATServer
{
    public class TeamStructures
    {
		public List<IStructure> Structures { get; private set; }
		public Base Base { get; private set; }

        public TeamStructures(Vec2 baseTileIds)
        {
			Base = new Base(GetFeetPosForStructure(baseTileIds));

			Structures = Utilities.MakeList<IStructure>(Base); //TODO: fill this
        }

		private static Vec2 GetFeetPosForStructure(Vec2 tileIds)
		{
			return tileIds * new Vec2(Tile.WIDTH, Tile.HEIGHT) + new Vec2(Tile.WIDTH / 2f, Tile.HEIGHT);
		}
    }
}

