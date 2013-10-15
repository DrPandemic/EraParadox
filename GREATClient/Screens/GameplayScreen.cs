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
using GREATClient.BaseClass.Input;
using GREATLib.Entities.Spells;

namespace GREATClient.Screens
{
    public sealed class GameplayScreen : Screen
    {
		static readonly List<KeyValuePair<InputActions, PlayerActionType>> InputTypeForAction = Utilities.MakeList(
			Utilities.MakePair(InputActions.GoLeft, PlayerActionType.MoveLeft),
			Utilities.MakePair(InputActions.GoRight, PlayerActionType.MoveRight),
			Utilities.MakePair(InputActions.Jump, PlayerActionType.Jump),
			Utilities.MakePair(InputActions.Spell1, PlayerActionType.Spell1),
			Utilities.MakePair(InputActions.Spell2, PlayerActionType.Spell2),
			Utilities.MakePair(InputActions.Spell3, PlayerActionType.Spell3),
			Utilities.MakePair(InputActions.Spell4, PlayerActionType.Spell4));


		const bool CORRECTIONS_ENABLED = false;

		static readonly TimeSpan SEND_INPUTS_TO_SERVER_INTERVAL = TimeSpan.FromMilliseconds(30.0);

		Client Client { get; set; }

		GameMatch Match { get; set; }
		DrawableTileMap Map { get; set; }

		ChampionsInfo ChampionsInfo { get; set; }
		MainDrawableChampion OurChampion { get; set; }

		GameTime GameTime { get; set; }

		double TimeSinceLastInputSent { get; set; }

		double TimeOfLastStateUpdate { get; set; }
		List<StateUpdateData> LastStateUpdateData { get; set; }
		List<RemarkableEventData> RemarkableEvents { get; set; }
		Dictionary<ulong, DrawableSpell> Spells { get; set; }

        public GameplayScreen(ContentManager content, Game game, Client client)
			: base(content, game)
        {
			Client = client;
			ChampionsInfo = new ChampionsInfo();

			GameTime = null;
			TimeSinceLastInputSent = 0.0;

			Match = new GameMatch();
			LastStateUpdateData = new List<StateUpdateData>();
			RemarkableEvents = new List<RemarkableEventData>();
			Spells = new Dictionary<ulong, DrawableSpell>();
			TimeOfLastStateUpdate = 0.0;
        }

		protected override void OnLoadContent()
		{
			base.OnLoadContent();

			ESCMenu menu = new ESCMenu();
			AddChild(menu, 5);
			menu.SetPositionInScreenPercent(50, 50);

			Map = new DrawableTileMap(Match.World.Map);
			AddChild(Map);

			Client.RegisterCommandHandler(ServerCommand.JoinedGame, OnJoinedGame);
			Client.RegisterCommandHandler(ServerCommand.NewRemotePlayer, OnNewRemotePlayer);
			Client.RegisterCommandHandler(ServerCommand.StateUpdate, OnStateUpdate);

			FPSCounter fps = new FPSCounter();
			PingCounter ping = new PingCounter(() => {
				return Client.Instance.GetPing().TotalMilliseconds;});

			AddChild(fps);
			AddChild(ping);

			fps.SetPositionRelativeToScreen(ScreenBound.BottomLeft, new Vector2(10,-30));
			ping.SetPositionRelativeToObject(fps, new Vector2(100,0), false);
		}

		void OnStateUpdate(object sender, CommandEventArgs args)
		{
			StateUpdateEventArgs e = args as StateUpdateEventArgs;
			Debug.Assert(e != null);

			OurChampion.Champion.SetLastAcknowledgedActionID(e.LastAcknowledgedActionID);
			LastStateUpdateData = new List<StateUpdateData>(e.EntitiesUpdatedState.ToArray());
			RemarkableEvents.AddRange(e.RemarkableEvents);
			TimeOfLastStateUpdate = Client.GetTime().TotalSeconds;
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
			ChampionSpawnInfo spawn = new ChampionSpawnInfo(data.ID, data.Position, data.Team, data.MaxHealth, data.Health);

			IDraw idraw;
			IEntity ientity;

			if (ourChampion) {
				OurChampion = new MainDrawableChampion(spawn, Match, ChampionsInfo);
				idraw = OurChampion;
				ientity = OurChampion.Champion;
			} else {
				var remote = new RemoteDrawableChampion(spawn, spawn.Team == OurChampion.Champion.Team, ChampionsInfo);
				idraw = remote;
				ientity = remote.Champion;
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

			if (Keyboard.GetState().IsKeyDown(Keys.Escape) && Keyboard.GetState().IsKeyDown(Keys.LeftShift))
				Exit = true;
		}
		/// <summary>
		/// Package local input as actions to eventually send to the server.
		/// At the same time, we simulate the input locally for client-side prediction.
		/// </summary>
		void HandleInput()
		{
			if (OurChampion != null) {
				InputTypeForAction.ForEach(pair =>
				{
					if (inputManager.IsActionFired(pair.Key)) {
						OurChampion.PackageAction(pair.Value, ActionTypeHelper.IsSpell(pair.Value) ? GetTargetWorldPosition() : null);
					}
				});
			}
		}

		Vec2 GetTargetWorldPosition()
		{
			//TODO: use camera here !
			return new Vec2(inputManager.MouseX, inputManager.MouseY);
		}

		/// <summary>
		/// Check for recent server update to apply to our local perception of the game (movement
		/// correction, etc.)
		/// </summary>
		void ApplyServerModifications()
		{
			if (OurChampion != null && LastStateUpdateData.Count > 0) {
				foreach (StateUpdateData state in LastStateUpdateData) {
					if (Match.CurrentState.ContainsEntity(state.ID)) {
						IEntity entity = Match.CurrentState.GetEntity(state.ID);
						ClientChampion champ = (ClientChampion)entity;
						champ.AuthoritativeChangePosition(state, TimeOfLastStateUpdate);
					}
				}
				LastStateUpdateData.Clear();

				ApplyRemarkableEvents();
			}
		}

		void ApplyRemarkableEvents()
		{
			RemarkableEvents.ForEach(r =>
			{
				switch (r.Command) {
					case ServerCommand.SpellCast:
						CastSpell((SpellCastEventData)r);
						break;

					case ServerCommand.SpellDisappear:
						RemoveSpell((SpellDisappearEventData)r);
						break;

					case ServerCommand.StatsChanged:
						ChangeStats((StatsChangedEventData)r);
						break;

					default:
						Debug.Fail("Unknown server command (unknown remarkable event).");
						break;
				}
			});

			RemarkableEvents.Clear();
		}
		void CastSpell(SpellCastEventData e)
		{
			var s = new DrawableSpell(new ClientLinearSpell(e.ID, e.Position, e.Time, e.Velocity, e.Range, e.Width));
			Spells.Add(e.ID, s);
			AddChild(s);
		}
		void RemoveSpell(SpellDisappearEventData e)
		{
			if (Spells.ContainsKey(e.ID)) {
				Spells[e.ID].Spell.Active = false;
				Spells.Remove(e.ID);
			}
		}
		void ChangeStats(StatsChangedEventData e)
		{
			if (Match.CurrentState.ContainsEntity(e.ChampID)) {
				((ICharacter)Match.CurrentState.GetEntity(e.ChampID)).SetHealth(e.Health);
			}
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

					if (package.Count == 0) { // no actions? send a heartbeat to say that we're connected
						package.Enqueue(new PlayerAction(IDGenerator.GenerateID(),
						                                 PlayerActionType.Idle,
						                                 (float)Client.Instance.GetTime().TotalSeconds,
						                                 OurChampion.Champion.Position));
					}

					// Send packaged input
					Client.SendPlayerActionPackage(package);
					package.Clear();

					TimeSinceLastInputSent = 0f;
				}
			}
		}
    }
}

