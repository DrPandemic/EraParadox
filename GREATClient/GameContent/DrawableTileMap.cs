//
//  DrawableTileMap.cs
//
//  Author:
//       Jesse <>
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
using GREATLib.World.Tiles;
using GREATClient;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using GREATClient.BaseClass;
using System.Diagnostics;
using GREATClient.BaseClass.ScreenInformation;
using System.IO;

namespace GREATClient.GameContent
{
	/// <summary>
	/// Draws a tilemap.
	/// </summary>
    public class DrawableTileMap : IDraw
    {
		/// <summary>
		/// Gets or sets the drawn map.
		/// </summary>
		/// <value>The map.</value>
		public TileMap Map { get; set; }

		Texture2D TileSet { get ;set; }
		string TileSetName { get; set; }
		int TileSetTilesWidth { get; set; }

		ScreenService Screen { get; set; }

		public DrawableTileMap(TileMap map, string tileset)
        {
			Map = map;
			TileSetName = tileset;
        }
		public override float GetEffectiveAlpha()
		{
			throw new NotImplementedException();
		}
		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			base.OnLoad(content, gd);
			TileSet = content.Load<Texture2D>(Path.Combine(MapLoader.MAP_FOLDER, TileSetName));
			TileSetTilesWidth = TileSet.Width / Tile.WIDTH;
			Screen = (ScreenService)GetScreen().Services.GetService(typeof(ScreenService));
		}

		protected override void OnDraw(SpriteBatch batch)
		{
			Vector2 position = GetAbsolutePosition();

            int startX = MathHelper.Clamp((int)(-position.X / Tile.WIDTH), 0, Map.GetWidthTiles() - 1);
			int endX = MathHelper.Clamp((int)((-position.X + Screen.GameWindowSize.X) / Tile.WIDTH) + 1, 0, Map.GetWidthTiles() - 1);
			int startY = MathHelper.Clamp((int)(-position.Y / Tile.HEIGHT), 0, Map.GetHeightTiles() - 1);
			int endY = MathHelper.Clamp((int)((-position.Y + Screen.GameWindowSize.Y) / Tile.HEIGHT) + 1, 0, Map.GetHeightTiles() - 1);

			batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
			for (int y = startY; y < endY; ++y)
				for (int x = startX; x <= endX; ++x)
					if (Map.TileRows[y][x].Collision != CollisionType.Passable)
						batch.Draw(TileSet, new Rectangle(
							(int)(position.X + x * Tile.WIDTH),
							(int)(position.Y + y * Tile.HEIGHT),
							Tile.WIDTH, Tile.HEIGHT),
						    	   GetSourceRectangle(Map.TileRows[y][x].Id),
						           Color.White);
			batch.End();
		}
        Rectangle GetSourceRectangle(int tileId)
		{
            int onImageId = tileId - 1;
            Debug.Assert(onImageId >= 0);
            return new Rectangle((onImageId % TileSetTilesWidth) * Tile.WIDTH,
                                 (onImageId / TileSetTilesWidth) * Tile.HEIGHT,
			                     Tile.WIDTH,
			                     Tile.HEIGHT);
		}

		public override bool IsBehind(Vector2 position)
		{
			return false;
			/*
			return Map.IsValidXIndex((int)((position.X - GetAbsolutePosition().X) / Tile.WIDTH)) &&
				   Map.IsValidYIndex((int)((position.Y - GetAbsolutePosition().Y) / Tile.HEIGHT));*/
		}
    }
}

