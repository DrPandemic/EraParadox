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
using System.Diagnostics;

namespace GREATClient.Test
{
	public enum MenuState {
		MainOpened,
		AllClosed,
		VideoOpened,
		AudioOpened,
		ExitOpened
	}
    public class TestMenu : Container
    {

		MenuState State { get; set; }
		Menu MainMenu { get; set; }
		Menu VideoMenu { get; set; }
		Menu AudioMenu { get; set; }
		Menu ExitMenu { get; set; }
		DrawableRectangle MainRectangle { get; set; }
		DrawableRectangle AudioRectangle { get; set; }
		DrawableRectangle VideoRectangle { get; set; }
		Container ExitLayer { get; set; }

        public TestMenu()
        {
			State = MenuState.AllClosed;
			// Main
			MenuItem main1 = new MenuItem(new DrawableLabel() { Text = "Video settings" }) {
				StateSelected = new DrawableLabel() {Text = "Video settings", Tint = Color.Chocolate},
				ClickAction = () => OpenVideo()
			};
			MenuItem main2 = new MenuItem(new DrawableLabel() { Text = "Audio settings" }) {
				StateSelected = new DrawableLabel() {Text = "Audio settings", Tint = Color.Chocolate},
				ClickAction = () => OpenAudio()
			};
			MenuItem main3 = new MenuItem(new DrawableLabel() { Text = "Return to Game" }) {
				StateSelected = new DrawableLabel() {Text = "Return to Game", Tint = Color.Chocolate},
				ClickAction = () => OpenOrCloseMainMenu(null,null)
			};
			MenuItem main4 = new MenuItem(new DrawableLabel() { Text = "Exit Game" }) {
				StateSelected = new DrawableLabel() {Text = "Exit Game", Tint = Color.Chocolate},
				ClickAction = () => OpenExit()
			};

			MainMenu = new Menu(main1, main2, main3, main4);
			MainMenu.AlignItemsVertically(30f);
			MainMenu.AllowKeyboard = true;

			// Audio
			MenuItem audio1 = new MenuItem(new DrawableLabel() { Text = "Less boomboom" }) {
				StateSelected = new DrawableLabel() {Text = "Less boomboom", Tint = Color.Chocolate}
			};
			MenuItem audio2 = new MenuItem(new DrawableLabel() { Text = "More boomboom" }) {
				StateSelected = new DrawableLabel() {Text = "More boomboom", Tint = Color.Chocolate}
			};
			MenuItem audio3 = new MenuItem(new DrawableLabel() { Text = "Return to Main Menu" }) {
				StateSelected = new DrawableLabel() {Text = "Return to Main Menu", Tint = Color.Chocolate},
				ClickAction = () => OpenOrCloseMainMenu(null,null)
			};
			AudioMenu = new Menu(audio1,audio2,audio3);
			AudioMenu.AlignItemsVertically(30f);
			AudioMenu.AllowKeyboard = true;

			// Video
			MenuItem video1 = new MenuItem(new DrawableLabel() { Text = "Big Texture" }) {
				StateSelected = new DrawableLabel() {Text = "Big Texture", Tint = Color.Chocolate}
			};
			MenuItem video2 = new MenuItem(new DrawableLabel() { Text = "Small Texture" }) {
				StateSelected = new DrawableLabel() {Text = "Small Texture", Tint = Color.Chocolate}
			};
			MenuItem video3 = new MenuItem(new DrawableLabel() { Text = "Return to Main Menu" }) {
				StateSelected = new DrawableLabel() {Text = "Return to Main Menu", Tint = Color.Chocolate},
				ClickAction = () => OpenOrCloseMainMenu(null,null)
			};

			VideoMenu = new Menu(video1,video2,video3);
			VideoMenu.AlignItemsVertically(30f);
			VideoMenu.AllowKeyboard = true;

			// Exit
			MenuItem exit1 = new MenuItem(new DrawableLabel() { Text = "No" }) {
				StateSelected = new DrawableLabel() {Text = "Nice", Tint = Color.Chocolate},
				ClickAction = () => OpenOrCloseMainMenu(null,null)
			};
			MenuItem exit2 = new MenuItem(new DrawableLabel() { Text = "Yes" }) {
				StateSelected = new DrawableLabel() {Text = "Why", Tint = Color.Chocolate},
				ClickAction = () => {
					Screen s = GetScreen();
					if (s != null) {
						s.Exit = true;
					} else {
						Debug.Fail("It should has a screen");
					}
				}
			};

			ExitMenu = new Menu(exit1, exit2);
			ExitMenu.AlignItemsHorizontally(80);
			ExitMenu.AllowKeyboard = true;
			ExitMenu.Position = new Vector2(130,50);

			MainRectangle = new DrawableRectangle(new Vector2(150, 120), new Vector2(0, 0), Color.DarkGray);
			AudioRectangle = new DrawableRectangle(new Vector2(200, 85), new Vector2(0, 0), Color.DarkGray);
			VideoRectangle = new DrawableRectangle(new Vector2(200, 85), new Vector2(0, 0), Color.DarkGray);
			MainRectangle.Visible = false;
			AudioRectangle.Visible = false;
			VideoRectangle.Visible = false;

			ExitLayer = new Container();
			ExitLayer.AddChild(new DrawableRectangle(new Vector2(250, 80), new Vector2(0, 0), Color.DarkGray));
			ExitLayer.AddChild(new DrawableLabel(){Text = "Do you really want to quit?"});
			ExitLayer.AddChild(ExitMenu);
			ExitLayer.Visible = false;
		}

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			AddChild(MainMenu);
			AddChild(AudioMenu);
			AddChild(VideoMenu);
			AddChild(MainRectangle,0);
			AddChild(AudioRectangle,0);
			AddChild(VideoRectangle,0);
			AddChild(ExitLayer,0);

			inputManager.RegisterEvent(InputActions.Escape, new EventHandler(OpenOrCloseMainMenu));

			MainMenu.Active(false);
			AudioMenu.Active(false);
			VideoMenu.Active(false);
			ExitMenu.Active(false);
		}

		public void OpenOrCloseMainMenu(object sender, EventArgs e)
		{
			if (State == MenuState.AllClosed) {
				State = MenuState.MainOpened;
				MainRectangle.Visible = true;
				MainMenu.Active(true);
			} else if (State == MenuState.MainOpened) {
				State = MenuState.AllClosed;
				MainRectangle.Visible = false;
				MainMenu.UnselectItem();
				MainMenu.Active(false);
			} else if (State == MenuState.AudioOpened) {
				State = MenuState.MainOpened;
				AudioRectangle.Visible = false;
				MainRectangle.Visible = true;
				AudioMenu.Active(false);
				MainMenu.Active(true);
			} else if (State == MenuState.VideoOpened) {
				State = MenuState.MainOpened;
				VideoRectangle.Visible = false;
				MainRectangle.Visible = true;
				VideoMenu.Active(false);
				MainMenu.Active(true);
			} else if (State == MenuState.ExitOpened) {
				State = MenuState.MainOpened;
				ExitLayer.Visible = false;
				MainRectangle.Visible = true;
				ExitMenu.Active(false);
				MainMenu.Active(true);
			}
		}

		void OpenAudio()
		{
			MainMenu.Active(false);
			AudioMenu.Active(true);
			State = MenuState.AudioOpened;
			AudioRectangle.Visible = true;
			MainRectangle.Visible = false;
		}

		void OpenVideo()
		{
			MainMenu.Active(false);
			VideoMenu.Active(true);
			State = MenuState.VideoOpened;
			VideoRectangle.Visible = true;
			MainRectangle.Visible = false;
		}

		void OpenExit()
		{
			MainMenu.Active(false);
			ExitMenu.Active(true);
			State = MenuState.ExitOpened;
			MainRectangle.Visible = false;
			ExitLayer.Visible = true;
		}
    }
}

