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
	public class TeamMetaInfo
	{
		// meta info (in tile IDs)
		public Vec2 SpawnTileIds { get; private set; }
		public Vec2 BaseTileIds { get; private set; }
		public Vec2 BaseTowerTileIds { get; private set; }
		public Vec2 BottomTowerTileIds { get; private set; }
		public Vec2 TopTowerTileIds { get; private set; }

		public TeamMetaInfo(Vec2 spawnTile, Vec2 baseTile, Vec2 baseTowerTile, Vec2 botTowerTile, Vec2 topTowerTile)
		{
			SpawnTileIds = spawnTile;
			BaseTileIds = baseTile;
			BaseTowerTileIds = baseTowerTile;
			BottomTowerTileIds = botTowerTile;
			TopTowerTileIds = topTowerTile;
		}
	}
	public class MapMetaInfo
	{
		// Left meta info (in tile IDs)
		public TeamMetaInfo LeftMeta { get; private set; }

		// Right meta info (in tile IDs)
		public TeamMetaInfo RightMeta { get; private set; }
		public MapMetaInfo(TeamMetaInfo left, TeamMetaInfo right)
		{
			LeftMeta = left;
			RightMeta = right;
		}
	}
	public class MapLoadException : Exception
	{
		public MapLoadException(string errorType)
			: base(errorType + ". Redownload the map of the game to fix.")
		{
		}
	}
    public class MapLoader
    {
		class MapObject
		{
			public int height = 0;
			public int width = 0;
			public int x = 0;
			public int y = 0;
			public string name = "";
		}
		class MapLayer
		{
			public List<int> data = new List<int>();
			public string name = "";
			public List<MapObject> objects = new List<MapObject>();
		}
		class TileSetObj
		{
			public string image = "";
		}
		class Map
		{
			public int height = 0;
			public List<MapLayer> layers = new List<MapLayer>();
			public int width = 0;
			public List<TileSetObj> tilesets = new List<TileSetObj>();
		}

		public const string MAP_FOLDER = "Maps";
		public static readonly string MAIN_MAP_PATH = Path.Combine(MAP_FOLDER, "map.json");
		public List<List<Tile>> TileRows { get; private set; }
		public string TileSet { get; private set; }
		public MapMetaInfo Meta { get; private set; }

        public MapLoader(string mapPath)
        {
			string content = File.ReadAllText(mapPath);
			var map = JsonConvert.DeserializeObject<Map>(content);

			TileRows = ExtractTileRows(map);
			TileSet = ExtractTileSet(map);

			Meta = new MapMetaInfo(
				new TeamMetaInfo(
					ExtractMetaTileIds(map, "l_spawn"),
					ExtractMetaTileIds(map, "l_base"),
					ExtractMetaTileIds(map, "l_base_tower"),
					ExtractMetaTileIds(map, "l_bot_tower"),
					ExtractMetaTileIds(map, "l_top_tower")),
				new TeamMetaInfo(
					ExtractMetaTileIds(map, "r_spawn"),
					ExtractMetaTileIds(map, "r_base"),
					ExtractMetaTileIds(map, "r_base_tower"),
					ExtractMetaTileIds(map, "r_bot_tower"),
					ExtractMetaTileIds(map, "r_top_tower")));
        }

		const string TILES_LAYER = "Tiles";
		static List<List<Tile>> ExtractTileRows(Map map)
		{
			if (!map.layers.Exists(l => l.name == TILES_LAYER))
				throw new MapLoadException("No Tiles layer in map");

			List<List<Tile>> rows = new List<List<Tile>>();
			var layer = map.layers.Find(l => l.name == TILES_LAYER);
			int w = map.width;
			int h = map.height;

			if (layer.data.Count != w * h)
				throw new MapLoadException("Map has the wrong size");

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
				throw new MapLoadException("Map doesn't have only 1 tileset");

			return map.tilesets[0].image;
		}

		const string META_LAYER = "Meta";
		static Vec2 ExtractMetaTileIds(Map map, string metaName)
		{
			if (!map.layers.Exists(l => l.name == META_LAYER))
				throw new MapLoadException("No Meta layer in map");

			var layer = map.layers.Find(l => l.name == META_LAYER);

			if (!layer.objects.Exists(o => o.name == metaName))
				throw new MapLoadException("Missing meta info " + metaName);

			var obj = layer.objects.Find(o => o.name == metaName);

			Vec2 center = new Vec2(obj.x + obj.width / 2f, obj.y + obj.height / 2f);
			return new Vec2((int)(center.X / Tile.WIDTH), (int)(center.Y / Tile.HEIGHT));
		}
    }
}

