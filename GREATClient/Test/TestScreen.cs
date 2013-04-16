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
using GREATLib.World.Tiles;
using GameContent;
using GREATLib.Entities.Player;

namespace GREATClient
{
    public class TestScreen : Screen
    {
		//TODO: remove. temporary physics tests
		PhysicsSystem physics = new PhysicsSystem();
		GameMatch match;

		public TestScreen(ContentManager content) : base(content)
        {
			match = new GameMatch();
        }
		protected override void OnLoadContent()
		{
			//TODO: DrawableGameMatch? I personnally like the idea (Jesse)
			AddChild(new DrawableTileMap(match.World.Map));

			//TODO: remove. simply testing the physics engine
			match.Players.Add(new Player() { 
				Champion = new StickmanChampion() {
					Position = new Vec2(200f, 50f)
				}
			});
			AddChild(new DrawableChampion(match.Players[0].Champion));
		}

		public override void Update(GameTime dt)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)) {
				Exit = true;
			}
			
			//TODO: remove. testing the physics engine
			KeyboardState ks = Keyboard.GetState();
			if (ks.IsKeyDown(Keys.Left)) physics.Move(match.Players[0].Champion, HorizontalDirection.Left);
			if (ks.IsKeyDown(Keys.Right)) physics.Move(match.Players[0].Champion, HorizontalDirection.Right);
			List<PhysicsEntity> entities = new List<PhysicsEntity>();
			entities.Add(match.Players[0].Champion);
			physics.Update((float)dt.ElapsedGameTime.TotalSeconds, entities);

			base.Update(dt);
		}
    }
}
