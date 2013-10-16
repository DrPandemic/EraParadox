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
using GREATClient.BaseClass.Particle;

namespace GREATClient.GameContent
{
    public class DrawableSpell : Container
    {
		IDraw Display { get; set; } //TODO: change for image/animation class
		ParticleSystem Particles;
		public Color Tint;

		DrawableImage Bullet { get; set; }

		public ClientLinearSpell Spell { get; private set; }

        public DrawableSpell(ClientLinearSpell spell)
        {
			Spell = spell;
			Tint = Color.Red;
        }

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);
			//AddChild(Display = new DrawableRectangle(new Rect(Spell.Position.X, Spell.Position.Y, 5f, 5f), Color.Cyan) { RelativeOrigin = new Vector2(.5f)});

			Particles = new ParticleSystem(100, null, new TimeSpan(0, 0, 1));
			Particles.ParticleInitialVelocity = new Vector2(Spell.Velocity.X * -1, Spell.Velocity.Y * -1);

			Particles.Tint = Tint;

			AddChild(Particles);

			AddChild(Bullet = new DrawableImage("bullet") {RelativeOrigin = new Vector2(0.5f,0.5f)});
			Bullet.Orientation = (float)Math.Atan2((double)Spell.Velocity.Y,(double)Spell.Velocity.X);
		}

		protected override void OnUpdate(GameTime dt)
		{
			Spell.Update(dt.ElapsedGameTime.TotalSeconds);
			//Display.Position = GameLibHelper.ToVector2(Spell.Position);
			Bullet.Position = GameLibHelper.ToVector2(Spell.Position);
			Particles.Position = GameLibHelper.ToVector2(Spell.Position);
			base.OnUpdate(dt);

			//TODO: fade out on particles (but put spell icon invisible) here ?
			if (!Spell.Active) {
				Parent.RemoveChild(this);
			}
		}
    }
}

