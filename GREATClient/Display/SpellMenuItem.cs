//
//  SpellMenuItem.cs
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
using GREATClient.BaseClass.Menu;
using GREATClient.BaseClass;
using Microsoft.Xna.Framework;
using GameContent;

namespace GREATClient.Display
{
    public class SpellMenuItem : MenuItem
    {
		static float NORMAL_WIDTH = 45f;

		private SpellCastInfo Info { get; set; }

		private DrawableRectangle CoolDown { get; set; }

		DrawableImage Icon { get; set; }

		public SpellMenuItem(SpellCastInfo info, DrawableImage icon) : base(new DrawableImage("UIObjects/spellBox"),
		                              new DrawableImage("UIObjects/spellBox"),
		                              new DrawableImage("UIObjects/spellBox"))
        {
			Clickable = false;
			//StateClicking.Position = new Vector2(2, 2);
			AddChild(new DrawableImage("UIObjects/spellBoxDropShadow"){Position = new Vector2(2,2)},0);

			//ClickAction = () => Cast();

			Info = info;

			CoolDown = new DrawableRectangle(new Rectangle((int)NORMAL_WIDTH/2, (int)NORMAL_WIDTH, (int)NORMAL_WIDTH, (int)NORMAL_WIDTH), Color.Aqua);
			CoolDown.RelativeOrigin = new Vector2(0.5f,1f);
			CoolDown.Alpha = 0.3f;
			AddChild(CoolDown,3);

			Icon = icon;
			AddChild(Icon,2);
		}

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			// Yes magic numbers.
			Icon.Position = new Vector2(13,18 - Icon.Texture.Height/3);


			base.OnLoad(content, gd);
		}

		protected override void OnUpdate(GameTime dt)
		{
			if(Info.TimeLeft.Ticks <= 0) {
				CoolDown.Size = new Vector2(1f);
			} else {
				CoolDown.Size = new Vector2(NORMAL_WIDTH,NORMAL_WIDTH*Info.TimeLeft.Ticks/Info.Cooldown.Ticks);
			}
			base.OnUpdate(dt);
		}

		public void Cast() {}
    }
}

