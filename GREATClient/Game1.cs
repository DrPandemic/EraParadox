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
using Champions;
using Map;


namespace GREATClient
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Game
	{
		/// The screen dimensions
		const int SCREEN_W = 800;
		const int SCREEN_H = 600;

		Client client;

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

<<<<<<< Upstream, based on origin/master

		Dictionary<long, int> playerCurrentFrame = new Dictionary<long, int>();
		Dictionary<long, Vec2> playerCurrentPositions = null;

=======
		Dictionary<int, int> PlayerCurrentFrame = new Dictionary<int, int>();
		Dictionary<int, Vec2> PlayerCurrentPositions = null;

		ChampionResources resources;
<<<<<<< Upstream, based on origin/master
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
=======


		//TODO: remove, used for testing purposes (debug-drawing the map).
		Texture2D pixel;

>>>>>>> e9aeff0 Temporary tilemap (to test physics) with collisions and basic gravity.

		public Game1()
		{
			Console.WriteLine("Game created.");
			client = Client.Instance;
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

<<<<<<< Upstream, based on origin/master
			//TODO: put in character resources class and initialize here
			player = Content.Load<Texture2D>("stand");
			playerRun = Content.Load<Texture2D>("run");
=======
			resources = new ChampionResources(Content);
<<<<<<< Upstream, based on origin/master
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
=======


			//TODO: remove, simply used for the debug-drawn map
			pixel = new Texture2D(GraphicsDevice, 1, 1);
			pixel.SetData(GeneralHelper.MakeList(Color.White).ToArray());
>>>>>>> e9aeff0 Temporary tilemap (to test physics) with collisions and basic gravity.
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)) {
				Exit();
			}

			CheckInput();

			InterpolatePositions();

			client.Update();

			AnimatePlayers();
			base.Update(gameTime);
		}

		/// <summary>
		/// Animates the players.
		/// </summary>
		void AnimatePlayers()
		{
			if (client.Players != null) {
				foreach (Player p in client.Players.Values) {
<<<<<<< Upstream, based on origin/master
					if (!playerCurrentFrame.ContainsKey(p.Id)) // first time we meet the player
						playerCurrentFrame.Add(p.Id, 0); // start at frame 0
=======
					ChampionTypes champ = (ChampionTypes)p.Champion;
					PlayerAnimation anim = (PlayerAnimation)p.Animation;

					if (!PlayerCurrentFrame.ContainsKey(p.Id)) // first time we meet the player
						PlayerCurrentFrame.Add(p.Id, 0); // start at frame 0
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
					else {
						++playerCurrentFrame[p.Id]; // we add one frame to our count

<<<<<<< Upstream, based on origin/master
						playerCurrentFrame[p.Id] = 
							(playerCurrentFrame[p.Id]) % // take our total frame count
								(GetFrameCount((PlayerAnimation)p.Animation) * GetFrameRate((PlayerAnimation)p.Animation));
=======
						PlayerCurrentFrame[p.Id] = 
							(PlayerCurrentFrame[p.Id]) % // take our total frame count
								(resources.GetFrameCount(champ, anim) * resources.GetFrameRate(champ, anim));
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
						// and see if we should go back to frame #1.
					}
				}
			}
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
			//TODO: use the client's ping to interpolate until the next sync instead
			//of an arbitrary value.
			const float INTERPOLATION_LERP_FACTOR = 0.2f;

			if (client.Players != null) { // we *do* have players to show
				// Never received positions so far, just pick the client's
				if (playerCurrentPositions == null) {
					playerCurrentPositions = new Dictionary<long, Vec2>();
					foreach (long playerId in client.Players.Keys) {
						playerCurrentPositions.Add(playerId, client.Players[playerId].Position);
					}
				}

				foreach (long playerId in client.Players.Keys) {
					// first time we see this player
					if (!playerCurrentPositions.ContainsKey(playerId)) {
						playerCurrentPositions.Add(playerId, client.Players[playerId].Position);
					}
					// we interpolate to the player's position
					else { 
						playerCurrentPositions[playerId] = Vec2.Lerp(playerCurrentPositions[playerId], client.Players[playerId].Position, INTERPOLATION_LERP_FACTOR);
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

<<<<<<< Upstream, based on origin/master
			if (client.Players != null) {
				foreach (Player p in client.Players.Values) {
					// Take the interpolated position if we have one, the real one if we don't.
<<<<<<< Upstream, based on origin/master
					Vec2 pos = playerCurrentPositions != null && playerCurrentPositions.ContainsKey(p.Id) ?
						playerCurrentPositions[p.Id] : p.Position;
=======
					Vec2 pos = PlayerCurrentPositions != null && PlayerCurrentPositions.ContainsKey(p.Id) ?
						PlayerCurrentPositions[p.Id] : p.Position;
					ChampionTypes champ = (ChampionTypes)p.Champion;
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
					PlayerAnimation anim = (PlayerAnimation)p.Animation;
=======
			DrawMap();
>>>>>>> e9aeff0 Temporary tilemap (to test physics) with collisions and basic gravity.

			//TODO: remove, debugging purposes
			if (client.Players != null && client.OurId != Player.InvalidId) {
				Player p = client.Players[client.OurId];
				Champion champ = ChampionFromType.GetChampion(
					(ChampionTypes)p.Champion);

				spriteBatch.Draw(pixel, new Rectangle(
					(int)client.Players[client.OurId].Position.X - champ.CollisionWidth/2,
					(int)client.Players[client.OurId].Position.Y - champ.CollisionHeight,
					champ.CollisionWidth, champ.CollisionHeight),
				                 Color.Green * 0.5f);
			}

			DrawPlayers();

			spriteBatch.End();

			base.Draw(gameTime);
		}

<<<<<<< Upstream, based on origin/master
<<<<<<< Upstream, based on origin/master
		private Texture2D GetTexture(PlayerAnimation anim)
		{
			//TODO: put in a character resources object
			switch (anim) {
				case PlayerAnimation.Standing:
					return player;
				case PlayerAnimation.Running:
					return playerRun;
				default: throw new NotImplementedException("Animation not implemented while getting texture.");
			}
		}

=======
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
=======
		/// <summary>
		/// Draws the map of the game.
		/// </summary>
		void DrawMap()
		{
			//TODO: temporary, use the real tile ids, etc.
			//TODO: refactor into its own class, only here for fast testing purposes
			for (int row = 0; row < client.Map.GetHeightTiles(); ++row) {
				for (int column = 0; column < client.Map.GetWidthTiles(); ++column) {
					if (client.Map.GetTile(column, row).Id == Map.TileId.Block)
						spriteBatch.Draw(pixel, new Rectangle(column * Tile.WIDTH, row * Tile.HEIGHT, Tile.WIDTH, Tile.HEIGHT), Color.Red);
				}
			}
		}

		/// <summary>
		/// Draws the players in the game.
		/// </summary>
		void DrawPlayers()
		{
			if (client.Players != null) {
				foreach (Player p in client.Players.Values) {
					DrawPlayer(p);
				}
			}
		}

		/// <summary>
		/// Draws an individual player.
		/// </summary>
		/// <param name="p">The player.</param>
		void DrawPlayer(Player p)
		{
			// Take the interpolated position if we have one, the real one if we don't.
			Vec2 pos = PlayerCurrentPositions != null && PlayerCurrentPositions.ContainsKey(p.Id) ? PlayerCurrentPositions[p.Id] : p.Position;
			ChampionTypes champ = (ChampionTypes)p.Champion;
			PlayerAnimation anim = (PlayerAnimation)p.Animation;
			spriteBatch.Draw(resources.GetTexture(champ, anim), pos.ToVector2(), GetSourceRectangle(p), Color.White, 0f, new Vector2(resources.GetFrameWidth(champ, anim) / 2f, resources.GetFrameHeight(champ, anim)), // place origin at the feet
			Vector2.One, p.FacingLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		}

		/// <summary>
		/// Gets the source rectangle of a player.
		/// </summary>
		/// <returns>The source rectangle.</returns>
		/// <param name="p">The player.</param>
>>>>>>> e9aeff0 Temporary tilemap (to test physics) with collisions and basic gravity.
		private Rectangle GetSourceRectangle(Player p)
		{
			if (!playerCurrentFrame.ContainsKey(p.Id))
				playerCurrentFrame.Add(p.Id, 0);

			ChampionTypes champ = (ChampionTypes)p.Champion;
			PlayerAnimation anim = (PlayerAnimation)p.Animation;

<<<<<<< Upstream, based on origin/master
			//TODO: put in a character resources object
			return new Rectangle(playerCurrentFrame[p.Id] / GetFrameRate(anim) * GetFrameWidth(anim),
			                     0,
			                     GetFrameWidth(anim), 
			                     GetFrameHeight(anim));

		}

		private int GetFrameWidth(PlayerAnimation anim)
		{
			//TODO: put in a character resources object
			return GetTexture(anim).Width / GetFrameCount(anim);
		}

		private int GetFrameHeight(PlayerAnimation anim)
		{
			//TODO: put in a character resources object
			return GetTexture(anim).Height;
		}

		private int GetFrameCount(PlayerAnimation anim)
		{
			//TODO: put in a character resources object
			switch (anim) {
				case PlayerAnimation.Standing:
					return 1;
				case PlayerAnimation.Running:
					return 6;
				default:
					throw new NotImplementedException("Animation not implemented while getting frame count.");
			}
		}

		private int GetFrameRate(PlayerAnimation anim)
		{
			//TODO: put in a character resources object
			switch (anim) {
				case PlayerAnimation.Standing:
					return 1;
				case PlayerAnimation.Running:
					return 3;
				default:
					throw new NotImplementedException("Animation not implemented while getting frame framerate.");
			}
=======
			return resources.GetSourceRect(champ, anim, 
			                               PlayerCurrentFrame[p.Id] / resources.GetFrameRate(champ, anim));
>>>>>>> 8b491dd More structured character resources system along with the concept of "champions".
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

