//
//  DrawableChampionLifeBar.cs
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
    public class DrawableChampionLifeBar : Container
    {
		const float LERP_FACTOR = 0.1f;

		const float CONTOUR_WIDTH = 1f;
		const float MAX_WIDTH = 50.0f;
		const float NORMAL_HEIGHT = 3.0f;

		bool IsAlly { get; set; }

		float health;
		public float Health { 
			get { return health; }
			set {
 				health = Math.Min(MaxHealth, Math.Max(0f, value));
			}
		}
		float maxHealth;
		public float MaxHealth { 
			get { return maxHealth; }
			set {
 				maxHealth = Math.Max(0f, value);
				Health = Health; // auto assignement to fix health if needed
			}
		}

		float Ratio { get { return Health / MaxHealth; } }
		float currentRatio;

		DrawableRectangle LifeBar { get; set; }

        public DrawableChampionLifeBar(bool isAlly)
        {
			MaxHealth = 0f;
			Health = 0f;
			currentRatio = 1f;
			IsAlly = isAlly;
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			LifeBar = new DrawableRectangle(new Vector2(MAX_WIDTH,NORMAL_HEIGHT),new Vector2(-MAX_WIDTH / 2f, -NORMAL_HEIGHT / 2f), IsAlly ? Color.Green: Color.DarkRed);
			AddChild(new DrawableRectangle(new Vector2(MAX_WIDTH + CONTOUR_WIDTH * 2f, NORMAL_HEIGHT + CONTOUR_WIDTH * 2f), 
			                               new Vector2(-(MAX_WIDTH + CONTOUR_WIDTH * 2f)/2f, -(NORMAL_HEIGHT + CONTOUR_WIDTH * 2f)/2f), Color.White));
			AddChild(LifeBar);
		}

		protected override void OnUpdate(GameTime dt)
		{
			currentRatio = MathHelper.Lerp(currentRatio, Ratio, LERP_FACTOR);
			LifeBar.Size = new Vector2(MAX_WIDTH * currentRatio, NORMAL_HEIGHT);
		}
    }
}

