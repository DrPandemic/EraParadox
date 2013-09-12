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
using GREATClient.BaseClass.BaseAction;

namespace GREATClient.Display
{
	public enum MenuState {
		MainOpened,
		AllClosed,
		VideoOpened,
		AudioOpened,
		ExitOpened
	}
    public class ESCMenu : Container
    {
		public Vector2 MenuSize { get; set; }

		MenuState State { get; set; }
		// Menus
		Menu MainMenu { get; set; }
		Menu VideoMenu { get; set; }
		Menu AudioMenu { get; set; }
		Menu ExitMenu { get; set; }

		//Main
		DrawableImage MainBackground { get; set; }
		Container MainBackgroundLayer { get; set; }
		// Gear
		DrawableImage GearMain { get; set; }
		DrawableImage GearMiniMain { get; set; }

		//Audio
		DrawableImage AudioBackground { get; set; }
		Container AudioBackgroundLayer { get; set; }
		// Gear
		DrawableImage GearAudio { get; set; }

		// Video
		DrawableImage VideoBackground { get; set; }
		Container VideoBackgroundLayer { get; set; }
		// Gear
		DrawableImage GearVideo { get; set; }
		DrawableImage GearVideo2 { get; set; }

		// Exit
		DrawableImage ExitBackground { get; set; }
		Container ExitLayer { get; set; }
		// Gear 
		DrawableImage GearExit { get; set; }
		DrawableImage GearExit2 { get; set; }
		DrawableImage GearExitMini { get; set; }

		Container PlacementLayer { get; set; }

        public ESCMenu()
        {
			State = MenuState.AllClosed;
			PlacementLayer = new Container();			
			MainBackgroundLayer = new Container();
			VideoBackgroundLayer = new Container();
			AudioBackgroundLayer = new Container();

			// Main
			MenuItem main1 = new MenuItem(new DrawableLabel() {Text = "Video settings", RelativeOrigin = new Vector2(0.5f,0.0f) }, 
				new DrawableLabel() {Text = "Video settings", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "Video settings", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine}) 
			{ ClickAction = () => OpenVideo() };

			MenuItem main2 = new MenuItem(new DrawableLabel() {Text = "Audio settings" , RelativeOrigin = new Vector2(0.5f,0.0f)},
				new DrawableLabel() {Text = "Audio settings", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "Audio settings", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine}) 
			{ ClickAction = () => OpenAudio() };

			MenuItem main3 = new MenuItem(new DrawableLabel() {Text = "Return to Game", RelativeOrigin = new Vector2(0.5f,0.0f)},
				new DrawableLabel() {Text = "Return to Game", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "Return to Game", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine}) 
			{ ClickAction = () => OpenOrCloseMainMenu(null,null) };

			MenuItem main4 = new MenuItem(new DrawableLabel() {Text = "Exit Game" , RelativeOrigin = new Vector2(0.5f,0.0f)},
				new DrawableLabel() {Text = "Exit Game", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "Exit Game", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine}) 
			{ ClickAction = () => OpenExit() };

			MainMenu = new Menu(main1, main2, main3, main4);
			MainMenu.AlignItemsVertically(30f);
			MainMenu.AllowKeyboard = true;

			// Audio
			MenuItem audio1 = new MenuItem(new DrawableLabel() {Text = "Less boomboom", RelativeOrigin = new Vector2(0.5f,0.0f)},
				new DrawableLabel() {Text = "Less boomboom", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "Less boomboom", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine});

			MenuItem audio2 = new MenuItem(new DrawableLabel() {Text = "More boomboom", RelativeOrigin = new Vector2(0.5f,0.0f) },
				new DrawableLabel() {Text = "More boomboom", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "More boomboom", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine});

			MenuItem audio3 = new MenuItem(new DrawableLabel() { Text = "Return to Main Menu", RelativeOrigin = new Vector2(0.5f,0.0f) },
				new DrawableLabel() {Text = "Return to Main Menu", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate},
				new DrawableLabel() {Text = "Return to Main Menu", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine}) 
			{ ClickAction = () => OpenOrCloseMainMenu(null,null) };

			AudioMenu = new Menu(audio1,audio2,audio3);
			AudioMenu.AlignItemsVertically(30f);
			AudioMenu.AllowKeyboard = true;

			// Video
			MenuItem video1 = new MenuItem(new DrawableLabel() { Text = "Big Texture", RelativeOrigin = new Vector2(0.5f,0.0f) },
				new DrawableLabel() { Text = "Big Texture", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate },
				new DrawableLabel() { Text = "Big Texture", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine })
			{ ClickAction = () => screenService.GameWindowSize = screenService.ScreenSize };

			MenuItem video2 = new MenuItem(new DrawableLabel() { Text = "Small Texture", RelativeOrigin = new Vector2(0.5f,0.0f) },
				new DrawableLabel() { Text = "Small Texture", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate },
				new DrawableLabel() { Text = "Small Texture", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine })
			{ ClickAction = () => screenService.GameWindowSize = new Vector2(900,700) };

			MenuItem video25 = new MenuItem(new DrawableLabel() { Text = "Fullscreen", RelativeOrigin = new Vector2(0.5f,0.0f) },
				new DrawableLabel() { Text = "Fullscreen", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate },
				new DrawableLabel() { Text = "Fullscreen", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine })
			{ ClickAction = () => screenService.SwitchFullscreen() };

			MenuItem video3 = new MenuItem(new DrawableLabel() { Text = "Return to Main Menu", RelativeOrigin = new Vector2(0.5f,0.0f) },
				new DrawableLabel() { Text = "Return to Main Menu", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Chocolate },
				new DrawableLabel() { Text = "Return to Main Menu", RelativeOrigin = new Vector2(0.5f,0.0f), Tint = Color.Aquamarine }) 
			{ ClickAction = () => OpenOrCloseMainMenu(null,null) };

			VideoMenu = new Menu(video1,video2,video25,video3);
			VideoMenu.AlignItemsVertically(30f);
			VideoMenu.AllowKeyboard = true;

			// Exit
			MenuItem exit1 = new MenuItem(new DrawableLabel() { Text = "No" },
			                              new DrawableLabel() { Text = "Nice", Tint = Color.Chocolate },
			                              new DrawableLabel() { Text = "Nice", Tint = Color.Aquamarine }) 
			{ ClickAction = () => OpenOrCloseMainMenu(null,null) };

			MenuItem exit2 = new MenuItem(new DrawableLabel() { Text = "Yes" },
			                              new DrawableLabel() { Text = "Why", Tint = Color.Chocolate },
			                              new DrawableLabel() { Text = "Why", Tint = Color.Aquamarine }) {	
				ClickAction = () => {
					Screen s = GetScreen();
					if (s != null) {
						s.Exit = true;
					} else {
						Debug.Fail("It should has a screen");
					}
				}
			};


			//Exit
			ExitMenu = new Menu(exit1, exit2);
			ExitMenu.AlignItemsHorizontally(80);
			ExitMenu.AllowKeyboard = true;
			ExitMenu.Position = new Vector2(160,95);
			ExitLayer = new Container();
			ExitBackground = new DrawableImage("UIObjects/menuExit");
			ExitLayer.Visible = false;
			// Exit gear
			GearExit = new DrawableImage("UIObjects/menuGear");
			GearExit.Scale = new Vector2(0.9f, 0.9f);
			GearExit.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearExit.Position = new Vector2(25,130);
			GearExit.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));
			GearExit2 = new DrawableImage("UIObjects/menuGear");
			GearExit2.Scale = new Vector2(0.8f, 0.8f);
			GearExit2.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearExit2.Position = new Vector2(200,130);
			GearExit2.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));
			GearExitMini = new DrawableImage("UIObjects/menuGear");
			GearExitMini.Scale = new Vector2(0.6f, 0.6f);
			GearExitMini.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearExitMini.Position = new Vector2(115,130);
			GearExitMini.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),-2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),-18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));

			//Main
			MainBackground = new DrawableImage("UIObjects/menu");
			MainBackgroundLayer.Visible = false;
			MainMenu.Position = new Vector2(100,25);
			// Main gear
			GearMain = new DrawableImage("UIObjects/menuGear");
			GearMain.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearMain.Position = new Vector2(25,35);
			GearMain.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));
			GearMiniMain = new DrawableImage("UIObjects/menuGear");
			GearMiniMain.Scale = new Vector2(0.5f, 0.5f);
			GearMiniMain.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearMiniMain.Position = new Vector2(13,115);
			GearMiniMain.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),-2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),-18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));

			//Audio
			AudioBackground = new DrawableImage("UIObjects/menuAudio");
			AudioBackgroundLayer.Visible = false;
			AudioMenu.Position = new Vector2(130,27);
			// Audio gear
			GearAudio = new DrawableImage("UIObjects/menuGear");
			GearAudio.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearAudio.Position = new Vector2(130,35);
			GearAudio.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));

			//Video
			VideoBackground = new DrawableImage("UIObjects/menuVideo");
			VideoBackgroundLayer.Visible = false;
			VideoMenu.Position = new Vector2(150,25);
			//Gear
			GearVideo = new DrawableImage("UIObjects/menuGear");
			GearVideo.Scale = new Vector2(0.6f, 0.6f);
			GearVideo.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearVideo.Position = new Vector2(290,130);
			GearVideo.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));
			// Gear2
			GearVideo2 = new DrawableImage("UIObjects/menuGear");
			GearVideo2.Scale = new Vector2(0.6f, 0.6f);
			GearVideo2.RelativeOrigin = new Vector2(0.5f,0.5f);
			GearVideo2.Position = new Vector2(250,170);
			GearVideo2.PerformAction(new ActionSequence(-1,new ActionRotateBy(new TimeSpan(0,0,0,0,350),-2,false), new ActionRotateBy(new TimeSpan(0,0,0,0,200),-18,false), new ActionDelay(new TimeSpan(0,0,0,0,450))));
		}

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			AddChild(PlacementLayer);

			PlacementLayer.AddChild(MainMenu,2);
			PlacementLayer.AddChild(AudioMenu,2);
			PlacementLayer.AddChild(VideoMenu,2);
			PlacementLayer.AddChild(MainBackgroundLayer,1);
			PlacementLayer.AddChild(AudioBackgroundLayer,1);
			PlacementLayer.AddChild(VideoBackgroundLayer,1);
			PlacementLayer.AddChild(ExitLayer,1);

			VideoBackgroundLayer.AddChild(VideoBackground,1);
			VideoBackgroundLayer.AddChild(new DrawableLabel(){Text = "*Changes will take effect after the next restart.", 
															  Scale = new Vector2(0.6f,0.6f),
															  Position = new Vector2(20,150)});

			VideoBackgroundLayer.AddChild(GearVideo,0);
			VideoBackgroundLayer.AddChild(GearVideo2,0);

			AudioBackgroundLayer.AddChild(AudioBackground, 1);
			AudioBackgroundLayer.AddChild(GearAudio, 0);

			MainBackgroundLayer.AddChild(GearMain,0);
			MainBackgroundLayer.AddChild(GearMiniMain,0);
			MainBackgroundLayer.AddChild(MainBackground,1);

			ExitLayer.AddChild(ExitBackground,1);
			ExitLayer.AddChild(new DrawableLabel(){Text = "Do you really want to quit?", Position = new Vector2(30,25)});
			ExitLayer.AddChild(ExitMenu,2);
			ExitLayer.AddChild(GearExit,0);
			ExitLayer.AddChild(GearExit2,0);
			ExitLayer.AddChild(GearExitMini,0);

			inputManager.RegisterEvent(InputActions.Escape, new EventHandler(OpenOrCloseMainMenu));

			MainMenu.Active(false);
			AudioMenu.Active(false);
			VideoMenu.Active(false);
			ExitMenu.Active(false);

			SetMenuSize(new Vector2(MainBackground.Texture.Width,MainBackground.Texture.Height));
		}

		/// <summary>
		/// Replaces the placement layer.
		/// </summary>
		void SetMenuSize(Vector2 size)
		{			
			MenuSize = size;
			PlacementLayer.Position = new Vector2(-MenuSize.X / 2, -MenuSize.Y /2);
		}

		public void OpenOrCloseMainMenu(object sender, EventArgs e)
		{
			if (State == MenuState.AllClosed) {
				State = MenuState.MainOpened;
				MainBackgroundLayer.Visible = true;
				MainMenu.Active(true);
				SetMenuSize(new Vector2(MainBackground.Texture.Width,MainBackground.Texture.Height));
			} else if (State == MenuState.MainOpened) {
				State = MenuState.AllClosed;
				MainBackgroundLayer.Visible = false;
				MainMenu.UnselectItems();
				MainMenu.Active(false);
			} else if (State == MenuState.AudioOpened) {
				State = MenuState.MainOpened;
				AudioBackgroundLayer.Visible = false;
				MainBackgroundLayer.Visible = true;
				AudioMenu.Active(false);
				MainMenu.Active(true);
				SetMenuSize(new Vector2(MainBackground.Texture.Width,MainBackground.Texture.Height));
			} else if (State == MenuState.VideoOpened) {
				State = MenuState.MainOpened;
				VideoBackgroundLayer.Visible = false;
				MainBackgroundLayer.Visible = true;
				VideoMenu.Active(false);
				MainMenu.Active(true);
				SetMenuSize(new Vector2(MainBackground.Texture.Width,MainBackground.Texture.Height));
			} else if (State == MenuState.ExitOpened) {
				State = MenuState.MainOpened;
				ExitLayer.Visible = false;
				MainBackgroundLayer.Visible = true;
				ExitMenu.Active(false);
				MainMenu.Active(true);
				SetMenuSize(new Vector2(MainBackground.Texture.Width,MainBackground.Texture.Height));
			}
		}

		void OpenAudio()
		{
			MainMenu.Active(false);
			AudioMenu.Active(true);
			State = MenuState.AudioOpened;
			AudioBackgroundLayer.Visible = true;
			MainBackgroundLayer.Visible = false;
			SetMenuSize(new Vector2(AudioBackground.Texture.Width,AudioBackground.Texture.Height));
		}

		void OpenVideo()
		{
			MainMenu.Active(false);
			VideoMenu.Active(true);
			State = MenuState.VideoOpened;
			VideoBackgroundLayer.Visible = true;
			MainBackgroundLayer.Visible = false;
			SetMenuSize(new Vector2(VideoBackground.Texture.Width,VideoBackground.Texture.Height));
		}

		void OpenExit()
		{
			MainMenu.Active(false);
			ExitMenu.Active(true);
			State = MenuState.ExitOpened;
			MainBackgroundLayer.Visible = false;
			ExitLayer.Visible = true;
			SetMenuSize(new Vector2(ExitBackground.Texture.Width,ExitBackground.Texture.Height));
		}
    }
}

