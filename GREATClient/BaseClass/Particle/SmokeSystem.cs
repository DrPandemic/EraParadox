//
//  SmokeSystem.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
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
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GREATClient.BaseClass.Particle
{
    public class SmokeSystem : ParticleSystem
    {
		public SmokeSystem()
		{
			// Particles settings
			ParticleFile = "MapObjects/smoke";
			ParticleInitialVelocity = new Vector2(-10, -100);
			ParticleForce = new Vector2(-5, 2);
			ParticleLifeTimeRandomizer = 0.3f;
			ParticleVelocityRandomizer = 0.5f;
			ParticleForceRandomizer = 1.8f;
			ParticleAlphaPercent = 0.25f;
			Tint = Color.White;

			NumberOfParticles = 70;
			MaxAnimationLength = null;

			Particles = new List<DrawableParticle>();

			LifeTime = new TimeSpan(0, 0, 5);

			ParticleScale = 0.5f;
		}
    }
}

