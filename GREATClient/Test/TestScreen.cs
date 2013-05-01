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
using GREATLib.Entities;

namespace GREATClient
{
    public class TestScreen : Screen
    {
		//TODO: remove. temporary local tests
		GameMatch match;
		int OurId { get; set; }

		public TestScreen(ContentManager content) : base(content)
        {
			OurId = EntityIDGenerator.NO_ID;
			match = new GameMatch();
        }
		protected override void OnLoadContent()
		{
			//TODO: DrawableGameMatch? I personnally like the idea (Jesse)
			AddChild(new DrawableTileMap(match.World.Map));

			//TODO: remove. simply testing the physics engine
			OurId = match.AddPlayer(new Player(), new StickmanChampion() {
				Position = new Vec2(200f, 100f)
			});
			AddChild(new DrawableChampion(match.GetPlayer(OurId).Champion));
			DrawableTriangle tr =  new DrawableTriangle(true);
			tr.Ascendant = false;
			tr.Tint = Color.Blue;
			tr.Scale = new Vector2(1f,2f);
			//AddChild(tr);
			DrawableCircleContour ci = new DrawableCircleContour(32);
			ci.Position = new Vector2(100,100);
			AddChild(ci);
			AddChild(new DrawableCircle());
		}

		public override void Update(GameTime dt)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)) {
				Exit = true;
			}
			
			//TODO: remove. testing the physics engine
			KeyboardState ks = Keyboard.GetState();
			if (ks.IsKeyDown(Keys.A)) { match.MovePlayer(OurId, HorizontalDirection.Left); }
			if (ks.IsKeyDown(Keys.D)) { match.MovePlayer(OurId, HorizontalDirection.Right); }
			if (ks.IsKeyDown(Keys.W)) { 
				match.JumpPlayer(OurId); }
			if (ks.IsKeyDown(Keys.R)) match.GetPlayer(OurId).Champion.Position.Y = 0f;

			match.Update((float)dt.ElapsedGameTime.TotalSeconds);

			base.Update(dt);
		}
    }
}

