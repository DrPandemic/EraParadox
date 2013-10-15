//
//  DeathScreen.cs
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
using GREATClient.BaseClass;
using Microsoft.Xna.Framework;

namespace GREATClient.GameContent
{
    public class DeathScreen : Container
    {
		/// <summary>
		/// It is the red visible around the screen.
		/// </summary>
		/// <value>The red rectangle.</value>
		DrawableRectangle RedRectangle { get; set; }

		/// <summary>
		/// The color of the border is increasing.
		/// AKA the red is becoming more red.
		/// </summary>
		bool ColorIncreasing;

		TimeSpan DeathDuration { get; set; }

		/// <summary>
		/// Represents the death timer on the death screen;
		/// </summary>
		/// <value>The death timer.</value>
		DrawableLabel DeathTimer { get; set; }

        public DeathScreen()
        {
			ColorIncreasing = true;
			DeathDuration = TimeSpan.FromSeconds(0);
			Visible = false;
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			RedRectangle = new DrawableRectangle(new GREATLib.Rect(0, 0, screenService.GameWindowSize.X, screenService.GameWindowSize.Y), Color.Red);
			RedRectangle.Alpha = 0f;
			AddChild(RedRectangle);

			DrawableLabel deathMessage = new DrawableLabel();
			deathMessage.Text = "You died!";
			deathMessage.Scale = new Vector2(2f);
			deathMessage.RelativeOrigin = new Vector2(0.5f);
			deathMessage.SetPositionInScreenPercent(50, 40);
			AddChild(deathMessage,2);

			DeathTimer = new DrawableLabel();
			DeathTimer.Text = DeathDuration.Seconds.ToString();
			DeathTimer.Scale = new Vector2(2f);
			DeathTimer.RelativeOrigin = new Vector2(0.5f);
			DeathTimer.SetPositionInScreenPercent(50, 50);
			AddChild(DeathTimer,2);
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			if(RedRectangle.Alpha <= 0f) {
				ColorIncreasing = true;
			} else if (RedRectangle.Alpha >=0.6f) {
				ColorIncreasing = false;
			}

			if (ColorIncreasing) {
				RedRectangle.Alpha += (1.0f / 120);
			} else {
				RedRectangle.Alpha -= (1.0f / 120);
			}

			DeathDuration -= dt.ElapsedGameTime;
			DeathDuration = DeathDuration.Ticks >= 0 ? DeathDuration: TimeSpan.FromSeconds(0);
			DeathTimer.Text = DeathDuration.Seconds.ToString();
		}

		public void DisplayScreen(TimeSpan time) {
			DeathDuration = time;
			Visible = true;
		}
    }
}

