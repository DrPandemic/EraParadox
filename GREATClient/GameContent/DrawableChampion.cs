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
using GREATClient;
using Microsoft.Xna.Framework;
using GREATLib;
using System.Collections.Generic;
using GREATLib.Entities;
using GREATLib.Network;
using GREATClient.BaseClass;
using GREATClient.Network;
using Microsoft.Xna.Framework.Graphics;
using GREATLib.Entities.Champions;

namespace GREATClient.GameContent
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
	public abstract class DrawableChampion<ChampionT> : Container 
		where ChampionT : ClientChampion
    {
		public IEntity Entity { get { return Champion; } }
		protected ChampionT Champion { get; set; }

		/// <summary>
		/// Gets or sets the current animation.
		/// </summary>
		/// <value>The current animation.</value>
		ChampionAnimation CurrentAnimation 
		{ 
			get {
				return ChampionSprite.CurrentAnimation;
			}
			set {
				if (value != CurrentAnimation) {
					ChampionSprite.PlayAnimation(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the champion sprite.
		/// This is the object that will be displayed.
		/// </summary>
		/// <value>The champion sprite.</value>
		DrawableChampionSprite ChampionSprite { get; set; }
		DrawableRectangle ChampionRect { get ; set; }

        public DrawableChampion(ChampionT champion, ChampionsInfo championsInfo)
        {
			Champion = champion;
			ChampionSprite = new DrawableChampionSprite(ChampionAnimation.idle, ChampionTypes.StickMan, championsInfo);
			ChampionSprite.PlayAnimation(ChampionAnimation.run);
			ChampionSprite.RelativeOrigin = new Vector2(0.5f, 1f);
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			AddChild(ChampionSprite);
			AddChild(ChampionRect = new DrawableRectangle(Champion.CreateCollisionRectangle(), Color.White * 0.7f));
			ChampionRect.Visible = MainDrawableChampion.VIEW_DEBUG_RECTS;
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			// Update the champion animation
			CurrentAnimation = Champion.Animation;

			Champion.Update(dt);

			var rect = Champion.CreateCollisionRectangle();
			ChampionSprite.Position = GameLibHelper.ToVector2(
				Champion.DrawnPosition + new Vec2(rect.Width / 2f, rect.Height));
			ChampionRect.Position = GameLibHelper.ToVector2(Champion.DrawnPosition);
		}

		public override bool IsBehind(Vector2 position)
		{
			if (Parent != null) {
				//TODO: use the rectangle of the current animation?
				return GameLibHelper.ToRectangle(Champion.CreateCollisionRectangle()).Contains(
					(int)position.X,
					(int)position.Y);
			}
			return false;
		}
    }
}
