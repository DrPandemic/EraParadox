//
//  Parallax.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
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
using Microsoft.Xna.Framework;
using System.Linq;
using GREATClient.BaseClass.ScreenInformation;

namespace GREATClient.BaseClass
{
    public class Parallax : Container
    {
		const float PARALLAX_ALPHA = 0.5f;

		Container Land;
		int LandLength;
		Container Fog;
		int FogLength;
		Container Cloud;
		int CloudLength;

		Vector2 WindowSize;

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Parallax"/> class.
		/// Be careful : If you put an image the same size as the world, the image won't move.
		/// </summary>
		/// <param name="worldSize">World size.</param>
		/// <param name="actions">Actions.</param>
		public Parallax()
        {
			Land = new Container();
			LandLength = 0;
			Fog = new Container();
			FogLength = 0;
			Cloud = new Container();
			CloudLength = 0;
			AddChild(Land,2);
			AddChild(Cloud,1);
			AddChild(Fog,3);
			//actions.OfType<Drawable>().ToList().ForEach((Drawable item) => AddChild(item));
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			float landOffset = 100;
			Land.AddChild(new DrawableImage("background/land1") {Position = new Vector2(0,landOffset), Alpha = PARALLAX_ALPHA});
			Land.AddChild(new DrawableImage("background/land2") {Position = new Vector2(1024,landOffset), Alpha = PARALLAX_ALPHA});
			Land.AddChild(new DrawableImage("background/land3") {Position = new Vector2(1024*2,landOffset), Alpha = PARALLAX_ALPHA});
			Land.AddChild(new DrawableImage("background/land4") {Position = new Vector2(1024*3,landOffset), Alpha = PARALLAX_ALPHA});
			Land.AddChild(new DrawableImage("background/land5") {Position = new Vector2(1024*4,landOffset), Alpha = PARALLAX_ALPHA});
			Land.AddChild(new DrawableImage("background/land6") {Position = new Vector2(1024*5,landOffset), Alpha = PARALLAX_ALPHA});
			LandLength = 1024 * 5 + 372;

			float fogOffset = 250;
			float xFogOffset = -40;
			Fog.AddChild(new DrawableImage("background/fog1") {Position = new Vector2(0 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			Fog.AddChild(new DrawableImage("background/fog2") {Position = new Vector2(1024 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			Fog.AddChild(new DrawableImage("background/fog3") {Position = new Vector2(1024*2 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			Fog.AddChild(new DrawableImage("background/fog4") {Position = new Vector2(1024*3 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			Fog.AddChild(new DrawableImage("background/fog5") {Position = new Vector2(1024*4 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			Fog.AddChild(new DrawableImage("background/fog6") {Position = new Vector2(1024*5 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			Fog.AddChild(new DrawableImage("background/fog7") {Position = new Vector2(1024*6 + xFogOffset,fogOffset), Alpha = PARALLAX_ALPHA});
			FogLength = 1024 * 7 + (int)xFogOffset - 250;

			float cloudOffset = -200;
			float xCloudOffset = -40;
			Cloud.AddChild(new DrawableImage("background/cloud1") {Position = new Vector2(xCloudOffset,cloudOffset), Alpha = PARALLAX_ALPHA});
			Cloud.AddChild(new DrawableImage("background/cloud2") {Position = new Vector2(1024 + xCloudOffset,cloudOffset), Alpha = PARALLAX_ALPHA});
			Cloud.AddChild(new DrawableImage("background/cloud3") {Position = new Vector2(1024*2 + xCloudOffset,cloudOffset), Alpha = PARALLAX_ALPHA});
			Cloud.AddChild(new DrawableImage("background/cloud4") {Position = new Vector2(1024*3 + xCloudOffset,cloudOffset), Alpha = PARALLAX_ALPHA});
			Cloud.AddChild(new DrawableImage("background/cloud5") {Position = new Vector2(1024*4 + xCloudOffset,cloudOffset), Alpha = PARALLAX_ALPHA});
			CloudLength = 1024 * 4 + 408 + (int)xCloudOffset;

			WindowSize = screenService.GameWindowSize;
		}

		/// <summary>
		/// Sets the position ratio of the camera in the world.
		/// Moves all the layers in the parallax.
		/// The value have to be between 0 and 1.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void SetCurrentRatio(float x, float y)
		{
			x = x < 0 ? 0 : (x > 100 ? 100 : x);
			y = y < 0 ? 0 : (y > 100 ? 100 : y);
			float land = -LandLength * x / 100;
			float fog = -FogLength * x / 100;
			float cloud = -CloudLength * x / 100;
			if (!(land < -LandLength+WindowSize.X || fog < -FogLength+WindowSize.X || cloud < -CloudLength+WindowSize.X)) {
				Land.Position = new Vector2(Math.Max(-LandLength * x/100,-LandLength+WindowSize.X), 0);
				Fog.Position = new Vector2(Math.Max(-FogLength * x/100,-FogLength+WindowSize.X), 0);
				Cloud.Position = new Vector2(Math.Max(-CloudLength * x/100,-CloudLength+WindowSize.X), 0);
			}
		}
    }
}

