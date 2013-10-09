//
//  DrawableSpell.cs
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
using GREATClient.Network;
using Microsoft.Xna.Framework;
using GREATLib;

namespace GREATClient.GameContent
{
    public class DrawableSpell : Container
    {
		IDraw Display { get; set; } //TODO: change for image/animation class

		ClientLinearSpell Spell { get; set; }

        public DrawableSpell(ClientLinearSpell spell)
        {
			Spell = spell;
        }

		public override void Load(Container container, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.Load(container, gd);
			AddChild(Display = new DrawableRectangle(new Rect(Spell.Position.X, Spell.Position.Y, 5f, 5f), Color.Cyan) { RelativeOrigin = new Vector2(.5f)});
		}

		public override void Update(Microsoft.Xna.Framework.GameTime dt)
		{
			base.Update(dt);
			Spell.Update(dt.ElapsedGameTime.TotalSeconds);
			Display.Position = GameLibHelper.ToVector2(Spell.Position);
		}
    }
}

