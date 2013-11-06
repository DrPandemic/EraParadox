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
using GREATLib.Entities.Spells;

namespace GREATClient.GameContent
{
    public class DrawableSpell : Container
    {
		protected Color Tint { get; set; }

		Drawable Bullet { get; set; }

		public ClientLinearSpell Spell { get; private set; }

		protected bool RemoveWhenDeleted { get; set; }
		protected bool ApplyUpdates { get; set; }

        public DrawableSpell(ClientLinearSpell spell, Drawable bullet)
        {
			Spell = spell;
			Bullet = bullet;
			Tint = Color.White;

			RemoveWhenDeleted = true;
			ApplyUpdates = true;
        }

		protected void AddParticlesTrail(int particles, TimeSpan particleLifeTime, Color tint, Action<ParticleSystem> modifySystem = null)
		{
			var p = new ParticleSystem(particles, null, particleLifeTime);
			p.ParticleInitialVelocity = GameLibHelper.ToVector2(-Spell.Velocity);
			p.Tint = tint;
			if (modifySystem != null)
				modifySystem(p);
			AddChild(p);
		}

		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			AddChild(Bullet);
			Bullet.RelativeOrigin = new Vector2(0.5f, 0.5f);
			Bullet.Orientation = (float)Math.Atan2((double)Spell.Velocity.Y,(double)Spell.Velocity.X);
		}

		protected override void OnUpdate(GameTime dt)
		{
			if (ApplyUpdates) {
				Spell.Update(dt.ElapsedGameTime.TotalSeconds);
			}
			Position = GameLibHelper.ToVector2(Spell.Position);
			base.OnUpdate(dt);

			if (RemoveWhenDeleted && !Spell.Active) {
				Parent.RemoveChild(this);
			}
		}
    }
}

