//
//  FPSCounter.cs
//
//  Author:
//       The Parasithe <bipbip500@hotmail.com>
//
//  Copyright (c) 2013 The Parasithe
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GREATClient.BaseClass;

namespace GREATClient.Display
{
    public class FPSCounter : DrawableLabel
    {
		/// <summary>
		/// ThE NUMBER OF UPDATE BY SECOND
		/// </summary>
		const int UPDATE_BY_SECOND = 2;

		/// <summary>
		/// Gets or sets the since start.
		/// </summary>
		/// <value>The since start.</value>
		TimeSpan SinceStart { get; set; }

		/// <summary>
		/// Gets or sets the last draw.
		/// </summary>
		/// <value>The last draw.</value>
		TimeSpan LastDraw { get; set; }

		/// <summary>
		/// Gets or sets the last time the fps was updated
		/// </summary>
		/// <value>The last display.</value>
		TimeSpan LastUpdate { get; set; }


		public FPSCounter() : base(UIConstants.UI_FONT)
        {
			SinceStart = new TimeSpan();
			LastDraw = new TimeSpan();
			LastUpdate = new TimeSpan();
        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd) {
			if (gd != null)
				Position = new Vector2(10, gd.Viewport.TitleSafeArea.Height * 19 / 20);
			base.OnLoad(content, gd);
		}

		protected override void OnUpdate(GameTime dt)
		{
			SinceStart = SinceStart.Add(dt.ElapsedGameTime);
			LastUpdate = LastUpdate.Add(dt.ElapsedGameTime);
		}

		protected override void OnDraw(SpriteBatch batch)
		{
			if( LastUpdate.TotalMilliseconds > 1000 / UPDATE_BY_SECOND)
			{
				Text = (1000 / (SinceStart.TotalMilliseconds - LastDraw.TotalMilliseconds)).ToString();
				Text = Text.Substring(0, 5) + " fps";
				LastUpdate = new TimeSpan();
			}
			
			LastDraw = SinceStart;


			batch.Begin();
			Vector2 FontOrigin = Font.MeasureString( Text ) / 2;
			// Draw the string
			batch.DrawString( Font, Text, GetAbsolutePosition(), Tint * Alpha, 
			                 Orientation, OriginRelative * Font.MeasureString(Text), Scale, Effects, 0 );
			batch.End();
		}
    }
}

