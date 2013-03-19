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
using Microsoft.Xna.Framework.Storage;
using GREATLib;
using System.Collections.Generic;


namespace GREATClient
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		const int SCREEN_W = 800;
		const int SCREEN_H = 600;

		const float INTERPOLATION_LERP_FACTOR = 0.1f;

		Client client;

		Texture2D player;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Dictionary<int, Vec2> PlayerCurrentPositions = null;

		public Game1()
		{
			Console.WriteLine("Game created.");
			client = new Client();
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			graphics.IsFullScreen = false;
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
			client.Start();

			graphics.PreferredBackBufferWidth = SCREEN_W;
			graphics.PreferredBackBufferHeight = SCREEN_H;

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			Console.WriteLine("Loading game content...");
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			player = Content.Load<Texture2D>("stand");
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit();
			}

			CheckInput();


			InterpolatePositions();


			client.Update();
			base.Update(gameTime);
		}

		/// <summary>
		/// Checks for player input.
		/// </summary>
		void CheckInput()
		{
			KeyboardState ks = Keyboard.GetState();

			// Player wants to move left
			if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A)) {
				client.QueueCommand(ClientMessage.MoveLeft);
			}
			// Player wants to move right
			if (ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D)) {
				client.QueueCommand(ClientMessage.MoveRight);
			}
		}

		/// <summary>
		/// Interpolates the positions of the players to give a more fluid experience to the user.
		/// </summary>
		void InterpolatePositions()
		{
			if (client.Players != null) { // we *do* have players to show
				// Never received positions so far, just pick the client's
				if (PlayerCurrentPositions == null) {
					PlayerCurrentPositions = new Dictionary<int, Vec2>();
					foreach (int playerId in client.Players.Keys) {
						PlayerCurrentPositions.Add(playerId, client.Players[playerId].Position);
					}
				}

				foreach (int playerId in client.Players.Keys) {
					// first time we see this player
					if (!PlayerCurrentPositions.ContainsKey(playerId)) {
						PlayerCurrentPositions.Add(playerId, client.Players[playerId].Position);
					}
					// we interpolate to the player's position
					else { 
						PlayerCurrentPositions[playerId] = Vec2.Lerp(PlayerCurrentPositions[playerId], client.Players[playerId].Position, INTERPOLATION_LERP_FACTOR);
					}
				}
			}
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);


			spriteBatch.Begin();

			if (client.Players != null) {
				foreach (Player p in client.Players.Values) {
					// Take the interpolated position if we have one, the real one if we don't.
					Vec2 pos = PlayerCurrentPositions != null && PlayerCurrentPositions.ContainsKey(p.Id) ?
						PlayerCurrentPositions[p.Id] : p.Position;

					spriteBatch.Draw(player, pos.ToVector2(), Color.White);
				}
			}

			spriteBatch.End();

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

