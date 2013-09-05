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

		DrawableRectangle ChampionDrawnRect { get; set; }

		public DrawableChampion(ChampionT champion)
        {
			Champion = champion;
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			ChampionDrawnRect = new DrawableRectangle(new Rectangle(0, 0, 15, 30), Color.White);
			Parent.AddChild(ChampionDrawnRect);
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			Champion.Update(dt);

			ChampionDrawnRect.Position = GameLibHelper.ToVector2(Champion.DrawnPosition);
		}

		protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			// The champion's animations take care of the drawing.
		}

		public override bool IsBehind(Vector2 position)
		{
			//TODO: use the rectangle of the current animation?
			return GameLibHelper.ToRectangle(Champion.CreateCollisionRectangle()).Contains(
				(int)position.X,
				(int)position.Y);
		}
    }
}
