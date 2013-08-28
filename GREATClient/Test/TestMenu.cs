//
//  TestMenu.cs
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
using GREATClient.BaseClass.Menu;
using Microsoft.Xna.Framework;
using GREATClient.BaseClass.Input;

namespace GREATClient.Test
{
	public enum MenuState {
		MainOppenned,
		AllClosed,
		VideoOppenned,
		AudioOppend,
		ExitOppend
	}
    public class TestMenu : Container
    {

		MenuState state { get; set; }
		Menu mainMenu { get; set; }
		Menu videoMenu { get; set; }
		Container exitPage { get; set; }

        public TestMenu()
        {
			Visible = false;
			state = MenuState.AllClosed;
			MenuItem main1 = new MenuItem(new DrawableLabel() { Text = "Video settigns" }) {
				StateSelected = new DrawableLabel() {Text = "Video settings", Tint = Color.Chocolate}
			};
			MenuItem main2 = new MenuItem(new DrawableLabel() { Text = "Audio settigns" }) {
				StateSelected = new DrawableLabel() {Text = "Audio settings", Tint = Color.Chocolate}
			};
			MenuItem main3 = new MenuItem(new DrawableLabel() { Text = "Return to Game" }) {
				StateSelected = new DrawableLabel() {Text = "Return to Game", Tint = Color.Chocolate},
				ClickAction = () => OpenOrCloseMainMenu(null,null)
			};
			MenuItem main4 = new MenuItem(new DrawableLabel() { Text = "Exit Game" }) {
				StateSelected = new DrawableLabel() {Text = "Exit Game", Tint = Color.Chocolate}
			};

			mainMenu = new Menu(main1, main2, main3, main4);
			mainMenu.AlignItemsVertically(30f);
			mainMenu.AllowKeyboard = true;
		}

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			AddChild(mainMenu);
			
			inputManager.RegisterEvent(InputActions.Escape, new EventHandler(OpenOrCloseMainMenu));
		}

		public void OpenOrCloseMainMenu(object sender, EventArgs e)
		{
			if (state == MenuState.AllClosed) {
				state = MenuState.MainOppenned;
				Visible = true;
				mainMenu.Clickable = true;
			} else if (state == MenuState.MainOppenned) {
				state = MenuState.AllClosed;
				Visible = false;
				mainMenu.UnselectItem();
				mainMenu.Clickable = false;
			}
		}
    }
}

