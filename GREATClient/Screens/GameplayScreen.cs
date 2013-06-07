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
			DrawableChampion champion = new DrawableChampion(new StickmanChampion() { Position = e.Position, Id = e.ID }, ChampionsInfo);

			Champions.Add(e.ID, champion);
			AddChild(champion);

			if (e.IsOurID)
				OurChampion = champion;
		}
		void OnPositionUpdate(object sender, PositionUpdateEventArgs e)
		{
			foreach (PositionUpdateData data in e.Data) {
				if (Champions.ContainsKey(data.ID)) {
					Champions[data.ID].Champion.Position = data.Position;
				}
			}
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			//TODO: input manager
			const Keys LEFT = Keys.A;
			const Keys RIGHT = Keys.D;
			const Keys JUMP = Keys.W;
			KeyboardState keyboard = Keyboard.GetState();
			MouseState mouse = Mouse.GetState();

			if (keyboard.IsKeyDown(LEFT))
				Client.SendCommand(ClientCommand.MoveLeft);
			if (keyboard.IsKeyDown(RIGHT))
				Client.SendCommand(ClientCommand.MoveRight);
			if (oldKeyboard.IsKeyUp(JUMP) && keyboard.IsKeyDown(JUMP))
				Client.SendCommand(ClientCommand.Jump);

			oldKeyboard = keyboard;
			oldMouse = mouse;
			//ENDTODO

			base.OnUpdate(dt);
		}
    }
}

