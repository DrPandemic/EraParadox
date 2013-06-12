//
//  DrawableChampion.cs
//
//  Author:
//       Jesse <>
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
using GREATClient;
using GREATLib.Entities.Player.Champions;
using Microsoft.Xna.Framework;
using GREATLib.Entities.Physics;

namespace GREATClient
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : IDraw
    {
		const float INTERPOLATION_LERP_FACTOR = 0.4f;

		DrawableImage Idle { get; set; }
		DrawableSprite Run { get; set; }
		public IChampion Champion { get; set; }

		DrawableRectangle RealPositionDebugRect { get; set; }

		Vector2 position;

        public DrawableChampion(IChampion champion, ChampionsInfo championsInfo)
        {
			Champion = champion;
			position = champion.Position.ToVector2();

			Idle = new DrawableImage(championsInfo.GetInfo(champion.Type).AssetName + "_stand");
			Run = new DrawableSprite(championsInfo.GetInfo(champion.Type).AssetName + "_run",
			                         34, 33, 0, 20, 6,DrawableSprite.INFINITE);
			RealPositionDebugRect = new DrawableRectangle(
				new Rectangle((int)Champion.Position.X, (int)Champion.Position.Y, (int)Champion.CollisionWidth, (int)Champion.CollisionHeight), 
				Color.Green);
			RealPositionDebugRect.Alpha = 0.5f;

			Idle.OriginRelative = Run.OriginRelative = RealPositionDebugRect.OriginRelative = 
				new Vector2(0.5f, 1f); // position at the feet

			Run.Visible = false;
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			Parent.AddChild(Idle);
			Parent.AddChild(Run);
			Parent.AddChild(RealPositionDebugRect);
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			Idle.Visible = (Champion.CurrentAnimation == Animation.Idle);
			Run.Visible = !Idle.Visible;

			if (Run.Visible) {
				if (Champion.IsOnGround)
					Run.Play();
				else
					Run.Stop();
			}

			position = Vector2.Lerp(position, Champion.Position.ToVector2(), INTERPOLATION_LERP_FACTOR);

			Run.Position = 
				Idle.Position = 
				RealPositionDebugRect.Position = 
				position;
			Run.FlipX = Idle.FlipX = Champion.FacingLeft;
		}
		protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			// Run and Idle take care of that.
		}
    }
}
