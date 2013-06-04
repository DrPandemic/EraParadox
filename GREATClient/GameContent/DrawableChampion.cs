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
		DrawableImage Idle { get; set; }
		DrawableSprite Run { get; set; }
		public IChampion Champion { get; set; }

        public DrawableChampion(IChampion champion, ChampionsInfo championsInfo)
        {
			Champion = champion;

			Idle = new DrawableImage(championsInfo.GetInfo(champion.Type).AssetName + "_stand");
			Run = new DrawableSprite(championsInfo.GetInfo(champion.Type).AssetName + "_run",
			                         34, 33, 0, 20, 6,DrawableSprite.INFINITE);
			Idle.OriginRelative = Run.OriginRelative =
				new Vector2(0.5f, 1f); // position at the feet

			Run.Visible = false;
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			Parent.AddChild(Idle);
			Parent.AddChild(Run);
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

			Run.Position = Idle.Position = Champion.Position.ToVector2();
			Run.FlipX = Idle.FlipX = Champion.FacingLeft;
		}
		protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			// Run and Idle take care of that.
		}
    }
}
