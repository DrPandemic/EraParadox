//
//  SpellMenu.cs
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
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GREATClient.Display
{
    public class SpellMenu : Menu
    {
		SpellMenuItem Spell1 { get; set; }
		SpellMenuItem Spell2 { get; set; }
		SpellMenuItem Spell3 { get; set; }
		SpellMenuItem Spell4 { get; set; }

        public SpellMenu()
        {
			Spell1 = new SpellMenuItem();
			Spell2 = new SpellMenuItem();
			Spell3 = new SpellMenuItem();
			Spell4 = new SpellMenuItem();

			AddItem(Spell1);
			AddItem(Spell2);
			AddItem(Spell3);
			AddItem(Spell4);

        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{			
			AlignItemsHorizontally(60);

			// Set all spell infos.


		}

		protected override void OnUpdate(GameTime dt)
		{
			Debug.Assert(inputManager != null);
			if (Clickable && Visible) {
				bool noHit = true;
				for (int i = 0; i < ItemList.Count; ++i) {
					if (ItemList[i].IsBehind(inputManager.MousePosition)) {
						SelectedItem = i;
						SelecteGivenItem(null,null);
						noHit = false;
					}
					if (noHit) {
						SelectedItem = -1;
						SelecteGivenItem(null,null);
					}
				}
			}
		}
    }
}

