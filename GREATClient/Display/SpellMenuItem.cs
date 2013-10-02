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

namespace GREATClient.Display
{
    public class SpellMenuItem : MenuItem
    {
		public SpellMenuItem() : base(new DrawableImage("UIObjects/spellBox"),
		                              new DrawableImage("UIObjects/spellBox"),
		                              new DrawableImage("UIObjects/spellBox"))
        {
			Clickable = false;
			StateClicking.Position = new Vector2(2, 2);
			AddChild(new DrawableImage("UIObjects/spellBoxDropShadow"){Position = new Vector2(2,2)},0);

			ClickAction = () => Cast();
        }

		public void SetSpell()
		{

		}

		public void Cast()
		{

		}
    }
}

