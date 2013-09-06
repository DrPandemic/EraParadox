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
using GREATLib.Entities;
using GREATLib.Network;
using GREATClient.BaseClass;
using GREATClient.GameContent;
using GREATClient.Display;
using GREATClient.Network;

namespace GREATClient.Screens
{
    public sealed class GameplayScreen : Screen
    {
		const bool CORRECTIONS_ENABLED = false;

		static readonly TimeSpan SEND_INPUTS_TO_SERVER_INTERVAL = TimeSpan.FromMilliseconds(30.0);

		Client Client { get; set; }

		//TODO: place these in a class to manage input
		KeyboardState oldKeyboard;
		MouseState oldMouse;
		//ENDTODO

		GameMatch Match { get; set; }
		DrawableTileMap Map { get; set; }

		ChampionsInfo ChampionsInfo { get; set; }
		MainDrawableChampion OurChampion { get; set; }

		GameTime GameTime { get; set; }

		double TimeSinceLastInputSent { get; set; }

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
			TimeSinceLastInputSent = 0.0;

			Match = new GameMatch();
        }

		protected override void OnLoadContent()
		{
			base.OnLoadContent();

			Map = new DrawableTileMap(Match.World.Map);
			AddChild(Map);

			Client.RegisterCommandHandler(ServerCommand.JoinedGame, OnJoinedGame);
			Client.RegisterCommandHandler(ServerCommand.NewRemotePlayer, OnNewRemotePlayer);
			Client.RegisterCommandHandler(ServerCommand.StateUpdate, OnStateUpdate);

			AddChild(new FPSCounter());
			AddChild(new PingCounter(() => {return Client.Instance.GetPing().TotalMilliseconds;}));
		}

		void OnStateUpdate(object sender, CommandEventArgs args)
		{
			StateUpdateEventArgs e = args as StateUpdateEventArgs;
			Debug.Assert(e != null);

			if (OurChampion != null) {
				OurChampion.SetLastAcknowledgedAction(e.LastAcknowledgedActionID);


				foreach (StateUpdateData state in e.EntitiesUpdatedState) {
					if (Match.CurrentState.ContainsEntity(state.ID)) {
						IEntity entity = Match.CurrentState.GetEntity(state.ID);
						entity.AuthoritativeChangePosition(state.Position, e.Time);
					}
				}
			}
		}

		void OnJoinedGame(object sender, CommandEventArgs args)
		{
			JoinedGameEventArgs e = args as JoinedGameEventArgs;
			Debug.Assert(e != null);
			Debug.Assert(ChampionsInfo != null && Match != null);

			AddChampionToGame(e.OurData, true);

			foreach (PlayerData remote in e.RemotePlayers) {
				AddChampionToGame(remote, false);
			}
		}

		void OnNewRemotePlayer(object sender, CommandEventArgs args)
		{
			NewRemotePlayerEventArgs e = args as NewRemotePlayerEventArgs;
			Debug.Assert(e != null);
			Debug.Assert(ChampionsInfo != null && Match != null);

			AddChampionToGame(e.Data, false);
		}
		void AddChampionToGame(PlayerData data, bool ourChampion)
		{
			ChampionSpawnInfo spawn = new ChampionSpawnInfo(data.ID, data.Position);

			IDraw idraw;
			IEntity ientity;

			if (ourChampion) {
				OurChampion = new MainDrawableChampion(spawn, Match);
				idraw = OurChampion;
				ientity = OurChampion.Entity;
			} else {
				var remote = new RemoteDrawableChampion(spawn);
				idraw = remote;
				ientity = remote.Entity;
			}

			AddChild(idraw);

			Match.CurrentState.AddEntity(ientity);

			ILogger.Log(
				String.Format("New champion: id={0}, pos={1}, isOurChamp={2}", spawn.ID, spawn.SpawningPosition, ourChampion),
				LogPriority.High);
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			GameTime = dt;

			// The client-side loop of the game

			// 1. Handle input. We package input to send to the server and simulate them locally 
			//    for client-side prediction.
			HandleInput();

			// 2. Send input. We send packaged client actions to the server at regular intervals.
			SendInput();

			// 3. Check for server changes. We apply changes of states received from the server,
			//    applying movement correction when needed.
			ApplyServerModifications();

			// 4. Update local physics. We run the physics loop that is ran on the server to keep
			//    our local simulation running.
			base.OnUpdate(dt); // this is done by the player's drawablechampion
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

			if (OurChampion != null) {
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
			}

			oldKeyboard = keyboard;
			oldMouse = mouse;
		}

		/// <summary>
		/// Check for recent server update to apply to our local perception of the game (movement
		/// correction, etc.)
		/// </summary>
		void ApplyServerModifications()
		{
			//TODO: move it here
		}

		/// <summary>
		/// Checks whether we should send our packaged player actions to the server yet (we do
		/// so at regular intervals).
		/// </summary>
		void SendInput()
		{
			if (OurChampion != null) {
				TimeSinceLastInputSent += GameTime.ElapsedGameTime.TotalSeconds;

				if (TimeSinceLastInputSent >= SEND_INPUTS_TO_SERVER_INTERVAL.TotalSeconds) {
					var package = OurChampion.GetActionPackage();

					if (package.Count > 0) {
						// Send packaged input
						Client.SendPlayerActionPackage(package);
						package.Clear();

						TimeSinceLastInputSent = 0f;
					}
				}
			}
		}
    }
}

