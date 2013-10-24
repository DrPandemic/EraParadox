//
//  Drawable_ManMega_HintOfASpark.cs
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
using GREATClient.GameContent;
using GREATClient.Network;
using GREATClient.BaseClass;
using Microsoft.Xna.Framework;

namespace GREATClient.GameContent.Spells
{
    public class Drawable_ManMega_HintOfASpark : DrawableSpell
    {
        public Drawable_ManMega_HintOfASpark(ClientLinearSpell spell)
			: base(spell,
			       new DrawableImage("manmega_hintofaspark"))
        {
			Tint = Color.Teal;
        }

		public override void Load(Container container, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.Load(container, gd);
			AddParticlesTrail(100, TimeSpan.FromSeconds(1.5), Color.White, (p) => {
				p.ParticleVelocityRandomizer = 0.4f;
			});
		}
    }
}

