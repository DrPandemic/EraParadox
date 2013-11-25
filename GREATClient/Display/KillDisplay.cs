//
//  KillDisplay.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
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
using GREATClient.GameContent;
using GREATClient.BaseClass;
using System.Collections.Generic;
using GREATClient.BaseClass.BaseAction;
using Microsoft.Xna.Framework;
using GREATLib.Entities.Champions;

namespace GREATClient.Display
{
    public class KillDisplay : Container
    {
		private class Kill : Container {
			public enum RemovingState {
				Alive,
				Disappearing,
				Remove
			}
			static int DurationShown = 3;
			static float MovementDuration = 0.5f;
			static float Height = 80f;
			public static float Width = 250f;

			public bool MoveFinished { get; private set; }

			int MoveCounter { get; set; }

			public TimeSpan Timer { get; private set; }

			public RemovingState Remove { get; private set; }

			public Kill(ChampionTypes killer, ChampionTypes killed, bool FirstIsAlly, ChampionsInfo championsInfo) {
				AddChild(new DrawableImage("UIObjects/killed") {Position = new Vector2(60,0)});

				AddChild(new DrawableImage("UIObjects/deathCircle") {Tint = FirstIsAlly ? Color.Green : Color.Red});
				AddChild(new DrawableImage("UIObjects/innerDeathCircle") {Position = new Vector2(5)});
				AddChild(new DrawableImage("UIObjects/deathCircle") {Position = new Vector2(Width - 80,0),
					Tint = FirstIsAlly ? Color.Red : Color.Green});
				AddChild(new DrawableImage("UIObjects/innerDeathCircle") {Position = new Vector2(Width - 75,5)});

				AddChild(new DrawableImage(championsInfo.GetInfo(killed).Portait) {Position = new Vector2(Width - 75,5)});
				AddChild(new DrawableImage(championsInfo.GetInfo(killer).Portait) {Position = new Vector2(5)});



				MoveFinished = true;
				MoveCounter = 0;
				Timer = new TimeSpan(0,0,DurationShown);
				Remove = RemovingState.Alive;
			}

			void StartAction() {
				if (MoveFinished) {
					MoveFinished = false;

					PerformAction(new ActionMoveBy(new TimeSpan(0,0,0,0,(int)(MovementDuration * 1000)), new Vector2(0f,Height)){DoneAction = thing => {
							MoveCounter --;
							MoveFinished = true;
						}});
				}
			}

			public void MoveDown() {
				MoveCounter++;
			}
			// Is called when appear on screen
			public void Active() {
				MoveFinished = false;
				PerformAction(new ActionFadeTo(new TimeSpan(0,0,0,0,(int)(MovementDuration * 1000)),0.8f));
				PerformAction(new ActionMoveTo(new TimeSpan(0,0,0,0,(int)(MovementDuration * 1000)), new Vector2(0,0)) { DoneAction = thing => {
						MoveFinished = true;
					}});
			}

			protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
			{
				if(Remove == RemovingState.Alive) {
					Timer -= dt.ElapsedGameTime;

					if(MoveCounter > 0 && MoveFinished) {
						StartAction();
					}
					
					if (Timer <= TimeSpan.Zero) {
						Remove = RemovingState.Disappearing;
						PerformAction(new ActionFadeTo(new TimeSpan(0,0,0,0,(int)(MovementDuration * 1000)),0f) {DoneAction = thing => {
								Remove = RemovingState.Remove;
							}});
					}
				}

				base.OnUpdate(dt);
			}
		}

		// Represents the number of pixel between the left border of a kill and the end of the screen, same for the top.
		static float ScreenOffset = 20f;

		Container Panel { get; set; }

		List<Kill> Kills { get; set; }
		// Kills are stored here when there is too much kill and the display can't follow
		List<Kill> KillsToBeAdded { get; set; }
		List<Kill> KillsToRemove { get; set; }

		ChampionsInfo ChampionsInfo { get; set; }

		public KillDisplay(ChampionsInfo championsInfo) {
			AddChild(Panel = new Container());
			Kills = new List<Kill>();
			KillsToBeAdded = new List<Kill>();
			KillsToRemove = new List<Kill>();
			ChampionsInfo = championsInfo;
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			Panel.SetPositionRelativeToScreen(ScreenBound.TopRight, new Vector2(-Kill.Width - ScreenOffset,ScreenOffset));
			base.OnLoad(content, gd);
		}

		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			ActiveAKill();
			foreach (Kill aKill in Kills) {
				if(aKill.Remove == Kill.RemovingState.Remove) {
					RemoveChild(aKill);
				}
			}
			KillsToRemove.RemoveAll(kill => kill.Remove == Kill.RemovingState.Remove);
			base.OnUpdate(dt);
		}

		public void Display(ChampionTypes? killer, ChampionTypes killed, bool FirstIsAlly) {
			//TODO: handle tower kills (killer == null)
			Kill kill = new Kill(killer ?? ChampionTypes.ManMega /*TODO: remove this temporary check*/,killed,FirstIsAlly, ChampionsInfo);
			kill.Position = new Vector2(Kill.Width + ScreenOffset, 0);
			kill.Alpha = 0;
			KillsToBeAdded.Add(kill);
		}

		void ActiveAKill() {
			if(KillsToBeAdded.Count != 0) {
				bool test = true;
				foreach(Kill aKill in Kills) {
					if(!aKill.MoveFinished) {
						test = false;
						break;
					}
				}
				if(test) {
					//Move down everything
					foreach(Kill aKill in Kills) {
						aKill.MoveDown();
					}
					Kill kill = KillsToBeAdded[0];
					Panel.AddChild(kill);
					Kills.Add(kill);
					KillsToBeAdded.Remove(kill);
					kill.Active();
				}
			}
		}
    }
}

