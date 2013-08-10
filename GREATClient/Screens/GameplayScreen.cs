//
//  GameplayScreen.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
//
//  Copyright (c) 2013 Jesse
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using GREATLib;
using GREATLib.World.Tiles;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Network;
using GREATLib.World;

namespace GREATClient.Screens
{
    public class GameplayScreen : Screen
    {
		Client Client { get; set; }

		//TODO: place these in a class to manage input
		KeyboardState oldKeyboard;
		MouseState oldMouse;
		//ENDTODO

		GameWorld World { get; set; }
		DrawableTileMap Map { get; set; }

		ChampionsInfo ChampionsInfo { get; set; }
		Dictionary<int, DrawableChampion> Champions { get; set; }
		DrawableChampion OurChampion { get; set; }

		GameTime GameTime { get; set; }

        public GameplayScreen(ContentManager content, Game game, Client client)
			: base(content, game)

        {
			Client = client;
			ChampionsInfo = new ChampionsInfo();

			//TODO: input manager
			oldKeyboard = Keyboard.GetState();
			oldMouse = Mouse.GetState();
			//ENDTODO

			GameTime = null;
        }

		protected override void OnLoadContent()
		{
			base.OnLoadContent();

			World = new GameWorld();

			Map = new DrawableTileMap(World.Map);
			AddChild(Map);

			Champions = new Dictionary<int, DrawableChampion>();
			AddChild(OurChampion = new DrawableChampion(ChampionsInfo, World));

			AddChild(new FPSCounter());
			AddChild(new PingCounter(() => {return Client.Instance.GetPing().TotalMilliseconds;}));
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			GameTime = dt;

			// The client-side loop of the game

			// 1. Handle input. We package input to send to the server and simulate them locally 
			//    for client-side prediction.
			HandleInput();

			// 2. Check for server changes. We apply changes of states received from the server,
			//    applying movement correction when needed.
			ApplyServerModifications();

			// 3. Update local physics. We run the physics loop that is ran on the server to keep
			//    our local simulation running.
			base.OnUpdate(dt); // this is done by the player's drawablechampion

			// 4. Send input. We send packaged client actions to the server at regular intervals.
			SendInput();
		}

		/// <summary>
		/// Package local input as actions to eventually send to the server.
		/// At the same time, we simulate the input locally for client-side prediction.
		/// </summary>
		void HandleInput()
		{
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();

			List<PlayerActionType> Actions = new List<PlayerActionType>();

			// This will be replaced by the inputmanager
			const Keys LEFT = Keys.A;
			const Keys RIGHT = Keys.D;
			const Keys JUMP = Keys.W;
			if (keyboard.IsKeyDown(LEFT)) {
				Actions.Add(PlayerActionType.MoveLeft);
			} 

			if (keyboard.IsKeyDown(RIGHT)) {
				Actions.Add(PlayerActionType.MoveRight);
			} 

			if (keyboard.IsKeyDown(JUMP) && oldKeyboard.IsKeyUp(JUMP)) {
				Actions.Add(PlayerActionType.Jump);
			}

			Actions.ForEach(OurChampion.PackageAction);

			oldKeyboard = keyboard;
			oldMouse = mouse;
		}

		/// <summary>
		/// Check for recent server update to apply to our local perception of the game (movement
		/// correction, etc.)
		/// </summary>
		void ApplyServerModifications()
		{
			//TODO
		}

		/// <summary>
		/// Checks whether we should send our packaged player actions to the server yet (we do
		/// so at regular intervals).
		/// </summary>
		void SendInput()
		{
			//TODO
		}
    }
}

