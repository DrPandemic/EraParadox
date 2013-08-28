//
//  TestScreen.cs
//
//  Author:
//       The Parasithe <bipbip500@hotmail.com>
//
//  Copyright (c) 2013 The Parasithe
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
using GREATLib;
using GREATLib.Entities;
using GREATLib.Entities.Physics;
using GREATLib.Entities.Player;
using GREATLib.Entities.Player.Champions.AllChampions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GREATLib.Entities.Player.Spells;
using System;
using GREATClient.BaseClass;
using GREATClient.GameContent;
using GREATClient.Display;
using GREATClient.BaseClass.Input;
using GREATClient.BaseClass.BaseAction;
using GREATClient.BaseClass.Menu;

namespace GREATClient.Test
{
    public class TestScreen : Screen
    {
		ChampionsInfo ChampionsInfo { get; set; }

		KeyboardState oldks;
		MouseState oldms;

		ActionSequence AS;

		DrawableChampionSprite champSprite;

		public TestScreen(ContentManager content, Game game) : base(content, game)
        {
			oldms = new MouseState();
			ChampionsInfo = new ChampionsInfo();
        }
		protected override void OnLoadContent()
		{
			champSprite = new DrawableChampionSprite(new StickmanChampion() { Position = new Vec2(200f, 100f) }, ChampionsInfo) 
			{ Position = new Vector2(200f, 100f) };

			AddChild(champSprite);

			AS = new ActionSequence(ActionSequence.INFINITE_SEQUENCE,
			                        new ActionMoveBy(new TimeSpan(0,0,1), new Vector2(100, 100)), 
			                        new ActionMoveBy(new TimeSpan(0,0,1), new Vector2(-100, -100)));

			champSprite.PerformAction(AS);

			DrawableTriangle tr =  new DrawableTriangle(true);
			tr.Ascendant = false;
			tr.Tint = Color.Blue;
			tr.Scale = new Vector2(1f,2f);

			oldks = Keyboard.GetState();
			oldms = Mouse.GetState();

			Container cc = new Container();
			cc.AddChild(new FPSCounter());
			AddChild(cc);

			//Test particle
			/*ParticleSystem sys = new ParticleSystem(Content, 1000, null);
			sys.Position = new Vector2(100, 100);
			AddChild(sys);*/

			AddChild(new PingCounter(yo));

			//inputManager.RegisterEvent(InputActions.Spell1, new EventHandler(Jump));
			//inputManager.RegisterEvent(InputActions.Jump, new EventHandler(Jump2));

			MenuItem i1 = new MenuItem(new DrawableLabel() {Text = "EP"}) {
				StateSelected = new DrawableLabel() {Text = "OP"},
				ClickAction = () => System.Console.WriteLine("a")
			};

			MenuItem i2 = new MenuItem(new DrawableCircleContour(8)) {
				StateSelected = new DrawableCircleContour(5)
			};

			MenuItem i3 = new MenuItem(new DrawableCircleContour(8)) {
				StateSelected = new DrawableCircleContour(5)
			};

			Menu me = new Menu(i1, i2, i3);
			me.AllowKeyboard = true;
			me.Position = new Vector2(100f, 100f);
			me.AlignItemsHorizontally(40f);

			AddChild(me);
		}

		/// <summary>
		/// Boobies reference
		/// </summary>
		protected double yo()
		{
			return 32d;
		}

		private void Jump(object sender, EventArgs e)
		{
			if (!((InputEventArgs)e).Handled) {
				champSprite.StopAllActions();
				champSprite.PerformAction(new ActionMoveBy(new TimeSpan(0,0,2), (arg1, arg2) => {
					float a = -0.005f;
					float b = arg1.Y;
					int xAddition = 200;
					float x = (xAddition * arg2) + arg1.X; 
					float y = (float)(a * Math.Pow(xAddition * arg2 ,2d) + b);
					System.Console.WriteLine(arg2);
					return new Vector2(x, y);
				}));
				((InputEventArgs)e).Handled = true;
			}
		}

		private void Jump2(object sender, EventArgs e)
		{
			if (!((InputEventArgs)e).Handled) {
				champSprite.PerformAction(new ActionFadeBy(new TimeSpan(0,0,3),-0.5f));
				((InputEventArgs)e).Handled = true;
			}
		}

		protected override void OnUpdate(GameTime dt)
		{
			//TODO: remove. testing the physics engine
			KeyboardState ks = Keyboard.GetState();
			MouseState ms = Mouse.GetState();

			if (ks.IsKeyDown(Keys.Escape))
				Exit = true;

			if (ks.IsKeyDown(Keys.E)) { champSprite.PlayAnimation(AnimationInfo.JUMP);}
			if (ks.IsKeyDown(Keys.Q)) { champSprite.PlayAnimation(AnimationInfo.RUN);}

			oldks = ks;
			oldms = ms;
			if (inputManager.IsActionFired(InputActions.GoRight))
			{
				int a = 1;
			}

			base.OnUpdate(dt);
		}
    }
}

