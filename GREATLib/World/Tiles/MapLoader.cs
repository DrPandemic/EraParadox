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
using Newtonsoft.Json;

namespace GREATLib.World.Tiles
{
    public class MapLoader
    {
		class MapObject
		{
			public int height;
			public int width;
			public int x;
			public int y;
			public string name;
		}
		class MapLayer
		{
			public List<int> data;
			public string name;
			public List<MapObject> objects;
		}
		class TileSetObj
		{
			public string image;
		}
		class Map
		{
			public int height;
			public List<MapLayer> layers;
			public int width;
			public List<TileSetObj> tilesets;
		}

		public const string MAIN_MAP_PATH = "Maps/map.json";
		public List<List<Tile>> TileRows { get; private set; }
		public string TileSet { get; private set; }

        public MapLoader(string mapPath)
        {
			string content = File.ReadAllText(mapPath);
			var map = JsonConvert.DeserializeObject<Map>(content);

			TileRows = ExtractTileRows(map);
			TileSet = ExtractTileSet(map);
        }

		const string TILES_LAYER = "Tiles";
		static List<List<Tile>> ExtractTileRows(Map map)
		{
			if (!map.layers.Exists(l => l.name == TILES_LAYER))
				throw new Exception("No Tiles layer in map. Redownload the map of the game.");

			List<List<Tile>> rows = new List<List<Tile>>();
			var layer = map.layers.Find(l => l.name == TILES_LAYER);
			int w = map.width;
			int h = map.height;

			if (layer.data.Count != w * h)
				throw new Exception("Map has the wrong size. Redownload the map of the game.");

			for (int y = 0; y < h; ++y) {
				var row = new List<Tile>();
				for (int x = 0; x < w; ++x) {
					row.Add(new Tile(layer.data[y * w + x], layer.data[y * w + x] != 0 ? CollisionType.Block : CollisionType.Passable ));
				}
				rows.Add(row);
			}

			return rows;
		}

		static string ExtractTileSet(Map map)
		{
			if (map.tilesets.Count != 1)
				throw new Exception("Map doesn't have only 1 tileset. Redownload the map of the game.");

			return map.tilesets[0].image;
		}
    }
}

