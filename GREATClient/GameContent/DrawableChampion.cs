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
using Microsoft.Xna.Framework;
using GREATLib;
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATClient
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : IDraw
    {
		DrawableImage Idle { get; set; }
		DrawableSprite Run { get; set; }

        public DrawableChampion(ChampionsInfo championsInfo)
        {
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			Parent.AddChild(Idle);
			Parent.AddChild(Run);
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
		}


		protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			// Run and Idle take care of that.
		}
    }
}
