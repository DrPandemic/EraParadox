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


namespace GREATClient
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class GreatGame : Game
	{
		/// The screen name
		const string SCREEN_NAME = "GREAT";
		/// The screen dimensions
		const int SCREEN_W = 800;
		const int SCREEN_H = 600;

		Client client;

		GraphicsDeviceManager graphics;
		Screen gameplay;

		bool ScreenInitialized { get; set; }

		public GreatGame()
		{
			Console.WriteLine("Game created.");
			client = Client.Instance;
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.Title = SCREEN_NAME;
			Window.AllowUserResizing = false;
			graphics.ApplyChanges();
			ScreenInitialized = false;
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (!ScreenInitialized) {
				SetupScreen();
				ScreenInitialized = true;

				Console.WriteLine("Starting client...");
				//gameplay = new GameplayScreen(Content, this, client); // when testing: new TestScreen(Content);
				gameplay = new TestScreen(Content,this);
				client.Start();

				Console.WriteLine("Loading game content...");

				gameplay.LoadContent(GraphicsDevice);
			}

			client.Update(gameTime.ElapsedGameTime.TotalSeconds);

			gameplay.Update(gameTime);

			if(gameplay.Exit)
				Exit();

			base.Update(gameTime);
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


			if (screenInfo.AutoResolution) {
				graphics.PreferredBackBufferWidth = screenInfo.ScreenWidth;
				graphics.PreferredBackBufferHeight = screenInfo.ScreenHeight;
			} else {
				graphics.PreferredBackBufferWidth = screenInfo.WindowWidth;
				graphics.PreferredBackBufferHeight = screenInfo.WindowHeight;
			}

			graphics.IsFullScreen = screenInfo.Fullscreen;
			Window.AllowUserResizing = false;

			#if DEBUG
			graphics.PreferredBackBufferWidth = screenInfo.WindowWidth;
			graphics.PreferredBackBufferHeight = screenInfo.WindowHeight;

			graphics.IsFullScreen = false;
			#endif

			graphics.ApplyChanges();

			Viewport view = graphics.GraphicsDevice.Viewport;
			view.Bounds = new Rectangle(0,
			                            0,
			                            graphics.PreferredBackBufferWidth,
			                            graphics.PreferredBackBufferHeight);
			graphics.GraphicsDevice.Viewport = view;
		}
	}
}

