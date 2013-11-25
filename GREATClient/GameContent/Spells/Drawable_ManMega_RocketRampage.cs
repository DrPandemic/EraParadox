//
//  Drawable_ManMega_RocketRampage.cs
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
using GREATClient.Network;
using GREATClient.BaseClass;
using Microsoft.Xna.Framework;

namespace GREATClient.GameContent.Spells
{
    public class Drawable_ManMega_RocketRampage : DrawableSpell
    {
        public Drawable_ManMega_RocketRampage(ClientLinearSpell spell)
			: base(spell,
			       new DrawableImage("Champions/ManMega/manmega_rocketrampage"))
        {
			Tint = Color.Red;
        }

		public override void Load(Container container, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.Load(container, gd);
			AddParticlesTrail(100, TimeSpan.FromSeconds(1.0), Tint);
		}
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			Bullet.Orientation = (float)Math.Atan2((double)Spell.Velocity.Y,(double)Spell.Velocity.X);
		}
    }
}

