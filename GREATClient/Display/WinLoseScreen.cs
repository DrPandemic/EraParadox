//
//  WinLoseScreen.cs
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
	public class WinLoseScreen : Container
    {
		const string LOST_MESSAGE = "You lost the game!";
		const string WON_MESSAGE = "Congratulations, you won!";
		const double CHANGING_DURATION = 2.0;
		const float MAX_ALPHA = 0.5f;

		DrawableLabel Message { get; set; }

		DrawableRectangle Overlay { get; set; }

        public WinLoseScreen() {
			Visible = false;
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			Overlay = new DrawableRectangle(new GREATLib.Rect(0, 0, screenService.GameWindowSize.X, screenService.GameWindowSize.Y), Color.White);
			Overlay.Alpha = 0f;
			AddChild(Overlay);

			AddChild(Message = new DrawableLabel(),2);
			Message.Scale = new Vector2(2f);
			Message.RelativeOrigin = new Vector2(0.5f);
			Message.SetPositionInScreenPercent(50, 50);

			base.OnLoad(content, gd);
		}

		public void Display(bool won) {
			Overlay.Alpha = 0f;

			Message.Text = won ? WON_MESSAGE: LOST_MESSAGE;
			Overlay.Tint = won ? Color.Azure: Color.Crimson;
			Visible = true;
		}
		public void Hide() {
			Visible = false;
		}

		protected override void OnUpdate(GameTime dt)
		{
			if(Visible) {
				if(Overlay.Alpha < MAX_ALPHA) {
					Overlay.Alpha += MAX_ALPHA * dt.ElapsedGameTime.Ticks / TimeSpan.FromSeconds(CHANGING_DURATION).Ticks;
				}
			}
			base.OnUpdate(dt);
		}
    }
}

