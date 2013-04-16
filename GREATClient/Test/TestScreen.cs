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
using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using GREATLib.Entities.Physics;
using GREATLib.Entities.Player.Champions;
using GREATLib;
using Microsoft.Xna.Framework.Graphics;

namespace GREATClient
{
    public class TestScreen : Screen
    {
		DrawableRectangle sq1;

		//TODO: remove. temporary physics tests
		PhysicsSystem physics = new PhysicsSystem();
		StickmanChampion champion = new StickmanChampion();

		public TestScreen(ContentManager content) : base(content)
        {
        }
		protected override void OnLoadContent()
		{
			//TODO: remove. simply testing the physics engine
			champion.Position = new Vec2(200f, 300f);

			DrawableImage testbigimg = new DrawableImage("testo") { 
				OriginRelative = Vector2.Zero 
			};
			DrawableImage teststickman = new DrawableImage("Stickman_run") {
				OriginRelative = Vector2.Zero,
				Position = new Vector2(100f, 0f)
			};
			sq1 = new DrawableRectangle() {
				OriginRelative = new Vector2(0.5f, 1f),
				Tint = Color.Red,
				Size = new Vector2(50f, 50f)
			};
			DrawableRectangle testrect = new DrawableRectangle(new Vector2(50f, 50f),
			                                                   new Vector2(200f, 0f),
			                                                   Color.Blue) {
				OriginRelative = Vector2.Zero
			};


			Container main = new Container(Content);
			AddChild(main);

			main.AddChild(testbigimg);
			Container mid = new Container(Content) { Position = new Vector2(150f, 100f) };
			main.AddChild(mid);

			mid.AddChild(teststickman);
			Container low = new Container(Content) { Position = new Vector2(100f, 50f) };
			mid.AddChild(low);

			low.AddChild(testrect);

			AddChild(sq1);
		}

		public override void Update(GameTime dt)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)) {
				Exit = true;
			}
			
			//TODO: remove. testing the physics engine
			KeyboardState ks = Keyboard.GetState();
			if (ks.IsKeyDown(Keys.Left)) physics.Move(champion, HorizontalDirection.Left);
			if (ks.IsKeyDown(Keys.Right)) physics.Move(champion, HorizontalDirection.Right);
			List<PhysicsEntity> entities = new List<PhysicsEntity>();
			entities.Add(champion);
			physics.Update((float)dt.ElapsedGameTime.TotalSeconds, entities);

			sq1.Position = new Vector2(champion.Position.X, champion.Position.Y);

		}
    }
}

