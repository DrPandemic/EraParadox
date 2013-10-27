//
//  DrawableBase.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
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
using GREATClient.BaseClass;
using GREATLib;
using GREATLib.Entities.Structures;
using GREATLib.Entities;
using Microsoft.Xna.Framework;

namespace GREATClient.GameContent
{
    public class DrawableBase : Container
    {
		const string LEFT_IMAGE = "lbase";
		const string RIGHT_IMAGE = "rbase";

		private Base Base { get; set; }
		private Teams Team { get; set; }

        public DrawableBase(Teams team, Base theBase)
        {
			Base = theBase;
			Team = team;

			Position = new Vector2(theBase.Rectangle.X + theBase.Rectangle.Width / 2f,
			                       theBase.Rectangle.Bottom);
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			AddChild(new DrawableImage(Team == Teams.Left ? LEFT_IMAGE : RIGHT_IMAGE) {
				RelativeOrigin = new Vector2(0.5f, 1.0f)
			});
		}
    }
}

