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

namespace GREATLib.Entities.Structures
{
    public class TeamStructures
    {
		public List<IStructure> Structures { get; private set; }
		public Base Base { get; private set; }
		public Tower BaseTower { get; private set; }
		public Tower TopTower { get; private set; }
		public Tower BottomTower { get; private set; }

        public TeamStructures(Teams team, Vec2 baseTileIds, Vec2 baseTowerTileIds,
		                      Vec2 topTowerTileIds, Vec2 bottomTowerTileIds)
        {
			Structures = Utilities.MakeList<IStructure>(
				Base = new Base(team, GetFeetPosForStructure(baseTileIds)),
				BaseTower = new Tower(StructureTypes.BaseTower, team, GetFeetPosForStructure(baseTowerTileIds)),
				TopTower = new Tower(StructureTypes.TopTower, team, GetFeetPosForStructure(topTowerTileIds)),
				BottomTower = new Tower(StructureTypes.BottomTower, team, GetFeetPosForStructure(bottomTowerTileIds)));
        }

		private static Vec2 GetFeetPosForStructure(Vec2 tileIds)
		{
			return tileIds * new Vec2(Tile.WIDTH, Tile.HEIGHT) + new Vec2(Tile.WIDTH / 2f, Tile.HEIGHT);
		}
    }
}

