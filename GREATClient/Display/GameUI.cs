//
//  GameUI.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GREATClient.Display
{
    public class GameUI : Container
    {
		PingCounter UIPingCounter { get; set; }
		FPSCounter UIFPSCounter { get; set; }

		DrawableImage Map { get; set; }
		DrawableImage SpellBackground { get; set; }
		DrawableImage Life { get; set; }
		DrawableImage Mana { get; set; }

		DrawableImage MoneyBackground { get; set; }
		//DrawableImage ObjectBackground { get; set; }
		//DrawableImage StatBackground { get; set; }

        public GameUI()
        {
			UIFPSCounter = new FPSCounter();
			AddChild(UIFPSCounter,2);
			UIPingCounter = new PingCounter(GetPing);
			AddChild(UIPingCounter,2);

			Map = new DrawableImage("UIObjects/map");
			AddChild(Map);
			SpellBackground = new DrawableImage("UIObjects/spellBackground");
			AddChild(SpellBackground);
			Life = new DrawableImage("UIObjects/life");
			AddChild(Life);
			Mana = new DrawableImage("UIObjects/mana");
			AddChild(Mana);

			MoneyBackground = new DrawableImage("UIObjects/boxBackground");
			AddChild(MoneyBackground);
			/*ObjectBackground = new DrawableImage("UIObjects/boxBackground");
			AddChild(ObjectBackground);
			StatBackground = new DrawableImage("UIObjects/boxBackground");
			AddChild(StatBackground);*/
        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			Map.SetPositionRelativeToScreen(ScreenBound.BottomRight, 
			                                new Vector2(- Map.Texture.Width - 10, - Map.Texture.Height - 10));
			Mana.SetPositionRelativeToObject(Map, new Vector2(-Mana.Texture.Width - 6, 
			                                                   Map.Texture.Height - Mana.Texture.Height), false);
			Life.SetPositionRelativeToObject(Mana, new Vector2(-Life.Texture.Width - 6, 
			                                                   Mana.Texture.Height - Life.Texture.Height), false);
			SpellBackground.SetPositionRelativeToObject(Life, new Vector2(-SpellBackground.Texture.Width - 10, 
	                                                              			Life.Texture.Height - SpellBackground.Texture.Height), 
			                                            false);

			MoneyBackground.SetPositionRelativeToScreen(ScreenBound.BottomLeft, 
			                                            new Vector2(10, - MoneyBackground.Texture.Height - 10));
			UIFPSCounter.SetPositionRelativeToObject(MoneyBackground, new Vector2(20, 10));
			UIPingCounter.SetPositionRelativeToObject(UIFPSCounter, new Vector2(200, 0));
		}

		protected double GetPing()
		{
			return 32d;
		}
    }
}

