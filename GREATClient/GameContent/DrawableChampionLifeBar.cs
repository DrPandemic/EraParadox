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
		const float MaxWidth = 60.0f;
		const float NormalHeight = 6.0f;

		bool IsAlly { get; set; }

		float m_Life;
		/// <summary>
		/// Gets or sets the life.
		/// The life is on 100.
		/// </summary>
		/// <value>The life.</value>
		public float Life 
		{ 
			get {
				return m_Life;
			}
			set {
				m_Life = (value <= 100.0f ? (value >= 0.0f ? value : 0.0f) : 100.0f);
			}
		}

		DrawableRectangle LifeBar { get; set; }

        public DrawableChampionLifeBar(bool isAlly)
        {
			IsAlly = isAlly;
			Life = 100;

        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			LifeBar = new DrawableRectangle(new Vector2(MaxWidth,NormalHeight),new Vector2(-30, -3), IsAlly ? Color.Green: Color.DarkRed);
			AddChild(new DrawableRectangle(new Vector2(70,16),new Vector2(-35, -8), IsAlly ? Color.Green: Color.DarkRed));
			AddChild(new DrawableRectangle(new Vector2(64,10),new Vector2(-32, -5), Color.White));
			AddChild(LifeBar);
		}

		protected override void OnUpdate(GameTime dt)
		{
			LifeBar.Size = new Vector2(1.0f * MaxWidth * Life / 100, NormalHeight);
		}
    }
}

