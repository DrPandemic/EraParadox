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
using GREATLib.Entities.Physics;
using GREATLib.Entities.Player.Champions;
using System.Collections.Generic;
using GREATLib;
using GREATClient.Screens;
using GREATClient.BaseClass;


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
		const int SCREEN_W = 1366;
		const int SCREEN_H = 768;

		Client client;

		GraphicsDeviceManager graphics;
		Screen gameplay;

		public GreatGame()
		{
			Console.WriteLine("Game created.");
			client = Client.Instance;
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
			Window.Title = SCREEN_NAME;

			Window.AllowUserResizing = true;
			graphics.PreferredBackBufferWidth = SCREEN_W;
			graphics.PreferredBackBufferHeight = SCREEN_H;
			graphics.ApplyChanges();
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			Console.WriteLine("Starting client...");
			//gameplay = new GameplayScreen(Content, this, client); // when testing: new TestScreen(Content);
			gameplay = new TestScreen(Content,this);
			client.Start();

			this.Services.AddService(typeof(InputManager), new InputManager());

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			Console.WriteLine("Loading game content...");

			gameplay.LoadContent(GraphicsDevice);
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			client.Update();

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
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

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
	}
}

