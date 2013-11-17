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
using GREATClient.BaseClass.Input;
using GameContent;
using GREATClient.BaseClass;

namespace GREATClient.Display
{
    public class SpellMenu : Menu
    {
		SpellMenuItem Spell1Item { get; set; }
		SpellMenuItem Spell2Item { get; set; }
		SpellMenuItem Spell3Item { get; set; }
		SpellMenuItem Spell4Item { get; set; }

		public SpellMenu(CurrentChampionState state)
        {
			Spell1Item = new SpellMenuItem(state.Spell1, new DrawableImage("UIObjects/spell1Icon"));
			Spell2Item = new SpellMenuItem(state.Spell2, new DrawableImage("UIObjects/spell2Icon"));
			Spell3Item = new SpellMenuItem(state.Spell3, new DrawableImage("UIObjects/spell3Icon"));
			Spell4Item = new SpellMenuItem(state.Spell4, new DrawableImage("UIObjects/spell4Icon"));

			AddItem(Spell1Item);
			AddItem(Spell2Item);
			AddItem(Spell3Item);
			AddItem(Spell4Item);
        }

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{			
			AlignItemsHorizontally(60);

			// Set all spell infos.
			inputManager.RegisterEvent(InputActions.Spell1, new EventHandler(Spell1Event));
			inputManager.RegisterEvent(InputActions.Spell2, new EventHandler(Spell2Event));
			inputManager.RegisterEvent(InputActions.Spell3, new EventHandler(Spell3Event));
			inputManager.RegisterEvent(InputActions.Spell4, new EventHandler(Spell4Event));
		}
		  
		/// <summary>
		/// When spell 1 is casted.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Spell1Event(object sender, EventArgs e) {
			if (e != null && e is InputEventArgs) {
				InputEventArgs args = (InputEventArgs)e;
				if (!args.Handled) {
					Spell1Item.Cast();
					args.Handled = true;
				}
			}
		}

		/// <summary>
		/// When spell 2 is casted.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Spell2Event(object sender, EventArgs e) {
			if (e != null && e is InputEventArgs) {
				InputEventArgs args = (InputEventArgs)e;
				if (!args.Handled) {
					Spell2Item.Cast();
					args.Handled = true;
				}
			}
		}

		/// <summary>
		/// When spell 3 is casted.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Spell3Event(object sender, EventArgs e) {
			if (e != null && e is InputEventArgs) {
				InputEventArgs args = (InputEventArgs)e;
				if (!args.Handled) {
					Spell3Item.Cast();
					args.Handled = true;
				}
			}
		}

		/// <summary>
		/// When spell 4 is casted.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		void Spell4Event(object sender, EventArgs e) {
			if (e != null && e is InputEventArgs) {
				InputEventArgs args = (InputEventArgs)e;
				if (!args.Handled) {
					Spell4Item.Cast();
					args.Handled = true;
				}
			}
		}
	}
}

