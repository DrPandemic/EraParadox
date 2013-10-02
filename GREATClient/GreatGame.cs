//
//  Game1.cs
//
//  Author:
//       William Turner <willtur.will@gmail.com>
//
//  Copyright (c) 2013 
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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GREATLib;
using GREATClient.Screens;
using GREATClient.BaseClass;
using GREATClient.Test;
using GREATClient.BaseClass.Input;
using GREATClient.BaseClass.ScreenInformation;
using System.Runtime.InteropServices;


namespace GREATClient
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GreatGame : Game
	{
		public static bool IsLinux
		{
			get
			{
				int p = (int)Environment.OSVersion.Platform;
				return (p == 4) || (p == 6) || (p == 128);
			}
		}

		/// The screen name
		const string SCREEN_NAME = "GREAT";
		/// The screen dimensions
		const int SCREEN_W = 800;
		const int SCREEN_H = 600;

		Client client;

		GraphicsDeviceManager graphics;
		GREATClient.BaseClass.Screen gameplay;

		bool ScreenInitialized { get; set; }
		bool ScreenResized { get; set; }

		/// <summary>
		/// Gets or sets the fail count.
		/// Because i need to count the number of time the window setup fails... (sad)
		/// </summary>
		/// <value>The fail count.</value>
		short FailCount { get; set; }

		public GreatGame()
		{
			Console.WriteLine("Game created.");
			client = Client.Instance;
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			Window.Title = SCREEN_NAME;
			IsMouseVisible = false;
			Window.AllowUserResizing = false;
			graphics.ApplyChanges();
			ScreenInitialized = false;
			ScreenResized = false;
			FailCount = 0;
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (!ScreenInitialized) {
				Window.ClientSizeChanged += OnScreenSizeChanged;

				ScreenInitialized = true;

				Console.WriteLine("Starting client...");
				//gameplay = new GameplayScreen(Content, this, client); // when testing: new TestScreen(Content);
				gameplay = new TestScreen(Content, this);
				client.Start();

				Console.WriteLine("Loading game content...");

				gameplay.LoadContent(GraphicsDevice);

				SetupScreen();
			}
#if LINUX
			else if (ScreenResized) {
				// Safety net, I really hate MonoGame (on Linux).
				if (!(Window.ClientBounds.Width == graphics.PreferredBackBufferWidth && Window.ClientBounds.Height == graphics.PreferredBackBufferHeight)) {
					if (FailCount > 10000) {
						Console.WriteLine("I'm really really sorry, but the screen was not able to be initialized.");
						Exit();
					}
					FailCount++;
					Console.WriteLine("Error for the window resize, trying to save the world from burning down.");
					SetupScreen();
				} else {
					FailCount=0;
				}
			}
#endif
			

			client.Update(gameTime.ElapsedGameTime.TotalSeconds);

			gameplay.Update(gameTime);

			if(gameplay.Exit)
				Exit();

			base.Update(gameTime);
		}

		void OnScreenSizeChanged(object sender, EventArgs args)
		{
#if LINUX
			Window.FixBorder();
#endif
			Mouse.SetPosition(InputManager.DefaultMouseX, InputManager.DefaultMouseY);
			gameplay.WindowIsReady(true);
			ScreenResized = true;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			gameplay.Draw();

			base.Draw(gameTime);
		}

		/// <summary>
		/// Raises the exiting event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="args">Arguments.</param>
		protected override void OnExiting(object sender, EventArgs args)
		{
			client.Stop();
		}

		void SetupScreen() {
			ScreenInfo screenInfo = ScreenInfo.GetInfo();

			if (screenInfo.WindowHeight < 480) {
				screenInfo.WindowHeight = 480;
			}
			if (screenInfo.WindowWidth < 800) {
				screenInfo.WindowWidth = 800;
			}
			
			screenInfo.ScreenHeight = GraphicsDevice.DisplayMode.Height;
			screenInfo.ScreenWidth = GraphicsDevice.DisplayMode.Width;

			screenInfo.SaveInfo();

			graphics.IsFullScreen = screenInfo.Fullscreen;

			if (screenInfo.AutoResolution || graphics.IsFullScreen) {
				graphics.PreferredBackBufferWidth = screenInfo.ScreenWidth;
				graphics.PreferredBackBufferHeight = screenInfo.ScreenHeight;
				screenInfo.WindowWidth = screenInfo.ScreenWidth;
				screenInfo.WindowHeight = screenInfo.ScreenHeight;
				screenInfo.SaveInfo();
			} else {
				graphics.PreferredBackBufferWidth = screenInfo.WindowWidth;
				graphics.PreferredBackBufferHeight = screenInfo.WindowHeight;
			}

			#if DEBUG
			graphics.PreferredBackBufferWidth = 900;
			graphics.PreferredBackBufferHeight = 700;

			graphics.IsFullScreen = false;
			#endif

			graphics.ApplyChanges();
		}
	}
}

