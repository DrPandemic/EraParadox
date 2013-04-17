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

namespace GameContent
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

		private Texture2D pixel;

		public DrawableTileMap(TileMap map)
        {
			Map = map;
        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			base.OnLoad(content, gd);
			pixel = new Texture2D(gd, 1, 1);
			pixel.SetData(new Color[] { Color.White });

		}

		public override void Draw(SpriteBatch batch)
		{
			Vector2 position = GetAbsolutePosition();
			batch.Begin();
			for (int y = 0; y < Map.GetHeightTiles(); ++y)
				for (int x = 0; x < Map.GetWidthTiles(); ++x)
					if (Map.TileRows[y][x].Collision != CollisionType.Passable)
						batch.Draw(pixel, new Rectangle(
							(int)(position.X + x * Tile.WIDTH),
							(int)(position.Y + y * Tile.HEIGHT),
							Tile.WIDTH, Tile.HEIGHT), Color.Red);
			batch.End();
		}
    }
}

