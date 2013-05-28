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

namespace GREATClient
{
    public class TestScreen : Screen
    {
		//TODO: remove. temporary local tests
		GameMatch match;
		int OurId { get; set; }
		Player Owner;


		ChampionsInfo ChampionsInfo { get; set; }

		KeyboardState oldks;
		MouseState oldms;

		public TestScreen(ContentManager content) : base(content)
        {
			oldms = new MouseState();
			ChampionsInfo = new ChampionsInfo();
			OurId = EntityIDGenerator.NO_ID;
			match = new GameMatch();
        }
		protected override void OnLoadContent()
		{
			//TODO: DrawableGameMatch? I personnally like the idea (Jesse)
			AddChild(new DrawableTileMap(match.World.Map));

			//TODO: eventually remove. simply testing the physics engine
			OurId = match.AddPlayer(new Player(), new StickmanChampion() {
				Position = new Vec2(200f, 100f)
			});
			Owner = match.GetPlayer(OurId);

			AddChild(new DrawableChampion(Owner.Champion, ChampionsInfo));
			DrawableTriangle tr =  new DrawableTriangle(true);
			tr.Ascendant = false;
			tr.Tint = Color.Blue;
			tr.Scale = new Vector2(1f,2f);

			oldks = Keyboard.GetState();
			oldms = Mouse.GetState();
		}

		protected override void OnUpdate(GameTime dt)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape)) {
				Exit = true;
			}
			
			//TODO: remove. testing the physics engine
			KeyboardState ks = Keyboard.GetState();
			MouseState ms = Mouse.GetState();
			if (ks.IsKeyDown(Keys.A)) { match.MovePlayer(OurId, HorizontalDirection.Left); }
			if (ks.IsKeyDown(Keys.D)) { match.MovePlayer(OurId, HorizontalDirection.Right); }

			if (oldks.IsKeyUp(Keys.W) && ks.IsKeyDown(Keys.W)) { match.JumpPlayer(OurId); }
			if (ks.IsKeyDown(Keys.R)) match.GetPlayer(OurId).Champion.Position.Y = 0f;

			if (oldms.RightButton == ButtonState.Released && ms.RightButton == ButtonState.Pressed) 
				Owner.Champion.RangedSpell.Activate(Owner.Champion, match, null, 
				                                    new Vec2(ms.X - Owner.Champion.Position.X, 
				         								ms.Y - (Owner.Champion.Position.Y - Owner.Champion.CollisionHeight / 2f)));
			oldms = ms;

			match.Update((float)dt.ElapsedGameTime.TotalSeconds);

			base.OnUpdate(dt);

			oldks = ks;
			oldms = ms;
		}
    }
}

