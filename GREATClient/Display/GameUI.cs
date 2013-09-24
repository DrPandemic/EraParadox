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
using GameContent;

namespace GREATClient.Display
{
    public class GameUI : Container
    {
		// Counters
		PingCounter UIPingCounter { get; set; }
		FPSCounter UIFPSCounter { get; set; }

		DrawableImage Map { get; set; }
		DrawableImage SpellBackground { get; set; }
		// Bars
		DrawableImage Life { get; set; }
		DrawableImage Resource { get; set; }

		DrawableImage MoneyBackground { get; set; }
		//DrawableImage ObjectBackground { get; set; }
		//DrawableImage StatBackground { get; set; }

		SpellMenu UISpellMenu { get; set; }

		/// <summary>
		/// Gets or sets the state of the champion.
		/// Can be updated live.
		/// </summary>
		/// <value>The state of the champion.</value>
		CurrentChampionState ChampionState { get; set; }

        public GameUI(CurrentChampionState ccs)
        {
			ChampionState = ccs;

			UIFPSCounter = new FPSCounter();
			AddChild(UIFPSCounter,2);
			UIPingCounter = new PingCounter(GetPing);
			AddChild(UIPingCounter,2);

			Map = new DrawableImage("UIObjects/map");
			AddChild(Map);
			SpellBackground = new DrawableImage("UIObjects/spellBackground");
			AddChild(SpellBackground);
			Life = new DrawableImage("UIObjects/life");
			Life.RelativeOrigin = new Vector2(0f,1f);
			AddChild(Life);
			Resource = new DrawableImage("UIObjects/mana");
			Resource.RelativeOrigin = new Vector2(0f,1f);
			AddChild(Resource);

			MoneyBackground = new DrawableImage("UIObjects/boxBackground");
			AddChild(MoneyBackground);
			/*ObjectBackground = new DrawableImage("UIObjects/boxBackground");
			AddChild(ObjectBackground);
			StatBackground = new DrawableImage("UIObjects/boxBackground");
			AddChild(StatBackground);*/

			UISpellMenu = new SpellMenu();
			AddChild(UISpellMenu,3);
        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			Map.SetPositionRelativeToScreen(ScreenBound.BottomRight, 
			                                new Vector2(- Map.Texture.Width - 10, - Map.Texture.Height - 10));
			Resource.SetPositionRelativeToObject(Map, new Vector2(-Resource.Texture.Width - 6, 
			                                                   Map.Texture.Height), false);
			Life.SetPositionRelativeToObject(Resource, new Vector2(-Life.Texture.Width - 6, 
			                                                   0), false);
			SpellBackground.SetPositionRelativeToObject(Life, new Vector2(-SpellBackground.Texture.Width - 10, 
	                                                              			- SpellBackground.Texture.Height), 
			                                            false);
			UISpellMenu.SetPositionRelativeToObject(SpellBackground, new Vector2(18,15));


			MoneyBackground.SetPositionRelativeToScreen(ScreenBound.BottomLeft, 
			                                            new Vector2(10, - MoneyBackground.Texture.Height - 10));
			UIFPSCounter.SetPositionRelativeToObject(MoneyBackground, new Vector2(20, 10));
			UIPingCounter.SetPositionRelativeToObject(UIFPSCounter, new Vector2(200, 0));

			SetLifeAndResource();
		}

		protected override void OnUpdate(GameTime dt)
		{
			SetLifeAndResource();
		}

		/// <summary>
		/// Sets the life and resource.
		/// Will update the UI bars.
		/// </summary>
		void SetLifeAndResource()
		{
			Vector2 v = Life.Scale; 
			v.Y = (float)ChampionState.CurrentLife / ChampionState.MaxLife;
			v.Y = v.Y >= 0 ? v.Y : 0;
			Life.Scale = v;;
			v = Resource.Scale;
			v.Y = (float)ChampionState.CurrentResource / ChampionState.MaxResource;
			v.Y = v.Y >= 0 ? v.Y : 0;
			Resource.Scale = v;
		}

		protected double GetPing()
		{
			return 32d;
		}
    }
}
