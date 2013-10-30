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

namespace GREATClient.Display
{
    public class DeathScreen : Container
    {
		const float MIN_RED_ALPHA = 0f;
		const float MAX_RED_ALPHA = 0.6f;
		const float PERIOD = 2f; // the time, in seconds, of a full cycle (min->max->min)

		// the cosine function parameters
		const float SIGN = -1f; // start from the minimum
		const float A = (MAX_RED_ALPHA - MIN_RED_ALPHA)/2f;
		const float B = PERIOD / MathHelper.TwoPi;
		const float H = 0f;
		const float K = (MAX_RED_ALPHA + MIN_RED_ALPHA)/2f;

		/// <summary>
		/// It is the red visible around the screen.
		/// </summary>
		/// <value>The red rectangle.</value>
		DrawableRectangle RedRectangle { get; set; }

		TimeSpan DeathDuration { get; set; }
		float timeDead;

		/// <summary>
		/// Represents the death timer on the death screen;
		/// </summary>
		/// <value>The death timer.</value>
		DrawableLabel DeathTimer { get; set; }

        public DeathScreen()
        {
			DeathDuration = TimeSpan.FromSeconds(0);
			Visible = false;
			timeDead = 0f;
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
			DeathDuration -= dt.ElapsedGameTime;
			timeDead += (float)dt.ElapsedGameTime.TotalSeconds;
			DeathDuration = DeathDuration.Ticks >= 0 ? DeathDuration: TimeSpan.FromSeconds(0);
			DeathTimer.Text = Math.Ceiling(DeathDuration.TotalSeconds).ToString();

			RedRectangle.Alpha = SIGN * A * (float)Math.Cos((timeDead - H) / B) + K;
		}

		public void DisplayScreen(TimeSpan time) {
			DeathDuration = time;
			Visible = true;
			timeDead = 0f;
		}
    }
}

