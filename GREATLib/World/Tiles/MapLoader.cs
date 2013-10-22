//
//  MapLoader.cs
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
using System.IO;
using System.Collections.Generic;

namespace GREATLib.World.Tiles
{
    public class MapLoader
    {
		public const string MAIN_MAP_PATH = "Maps/map.json";
		public List<List<Tile>> TileRows { get; private set; }

        public MapLoader(string mapPath)
        {
			string content = File.ReadAllText(mapPath);
			Console.WriteLine(content);

			TileRows = new List<List<Tile>>();
        }
    }
}

