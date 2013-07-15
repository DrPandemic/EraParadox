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
using GREATLib.Entities.Player.Champions.AllChampions;
using System.Diagnostics;
using GREATLib.Entities.Player.Champions;
using Microsoft.Xna.Framework;

namespace GREATClient.Screens
{
    public class GameplayScreen : Screen
    {
		Client Client { get; set; }

		//TODO: place these in a class to manage input
		KeyboardState oldKeyboard;
		MouseState oldMouse;
		//ENDTODO

		DrawableTileMap Map { get; set; }

		ChampionsInfo ChampionsInfo { get; set; }
		Dictionary<int, DrawableChampion> Champions { get; set; }
		DrawableChampion OurChampion { get; set; }

		GameTime GameTime { get; set; }

        public GameplayScreen(ContentManager content, Client client)
			: base(content)
        {
			Client = client;
			Client.OnNewPlayer += OnNewPlayer;
			Client.OnPositionUpdate += OnPositionUpdate;

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

			Map = new DrawableTileMap(new TileMap());
			AddChild(Map);

			Champions = new Dictionary<int, DrawableChampion>();
			OurChampion = null;
		}

		void OnNewPlayer(object sender, NewPlayerEventArgs e)
		{
			Debug.Assert(e != null);

			IChampion champion = new StickmanChampion();
			e.UpdateChampion(champion);
			DrawableChampion drawableChampion = new DrawableChampion(champion, ChampionsInfo);

			Champions.Add(e.ID, drawableChampion);
			AddChild(drawableChampion);

			if (e.IsOurID) {
				Debug.Assert(OurChampion == null); // We must not override our champion, indicates a problem.
				OurChampion = drawableChampion;
			}
		}
		void OnPositionUpdate(object sender, PositionUpdateEventArgs e)
		{
			Debug.Assert(e != null);
			Debug.Assert(e.Data != null);

			foreach (PositionUpdateData data in e.Data) {
				if (Champions.ContainsKey(data.ID)) {
					data.UpdateChampion(Client.GetTime().TotalSeconds, Champions[data.ID]);
				}
			}
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			GameTime = dt;

			//TODO: input manager
			const Keys LEFT = Keys.A;
			const Keys RIGHT = Keys.D;
			const Keys JUMP = Keys.W;
			const Keys DEBUG = Keys.T;
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();

			if (oldKeyboard.IsKeyUp(LEFT) && keyboard.IsKeyDown(LEFT)) {
				Client.SendCommand(ClientCommand.MoveLeft);
			} else if (oldKeyboard.IsKeyDown(LEFT) && keyboard.IsKeyUp(LEFT)) {
				Client.SendCommand(ClientCommand.StopMoveLeft);
			}

			if (oldKeyboard.IsKeyUp(RIGHT) && keyboard.IsKeyDown(RIGHT)) {
				Client.SendCommand(ClientCommand.MoveRight);
			} else if (oldKeyboard.IsKeyDown(RIGHT) && keyboard.IsKeyUp(RIGHT)) {
					Client.SendCommand(ClientCommand.StopMoveRight);
				}

			if (oldKeyboard.IsKeyUp(JUMP) && keyboard.IsKeyDown(JUMP)) {
				Client.SendCommand(ClientCommand.Jump);
			}


			//DEBUG INPUT
			if (oldKeyboard.IsKeyUp(DEBUG) && keyboard.IsKeyDown(DEBUG)) {
				DrawableChampion.SHOW_DEBUG_RECT = !DrawableChampion.SHOW_DEBUG_RECT;
			}


			oldKeyboard = keyboard;
			oldMouse = mouse;
			//ENDTODO

			base.OnUpdate(dt);
		}
    }
}

