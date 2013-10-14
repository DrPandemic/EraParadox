//
//  ParticleSystem.cs
//
//  Author:
//       parasithe <>
//
//  Copyright (c) 2013 parasithe
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GREATClient.BaseClass.Particle
{
    public class ParticleSystem : Container
    {

		/// <summary>
		/// The particules.
		/// </summary>
		protected List<DrawableParticle> Particules { get; set; }

		/// <summary>
		/// The number of particules.
		/// </summary>
		int numberOfPaticules;
		int NumberOfParticles 
		{ 
			get { return numberOfPaticules; }
			set {
				numberOfPaticules = value;
				CalculateTimeUntilNextSpawn();
			}
		}

		/// <summary>
		/// The length of the animation.
		/// </summary>
		TimeSpan? animationLength;
		TimeSpan? AnimationLength 
		{ 
			get { return animationLength; }
			set {
				animationLength = value;
				CalculateTimeUntilNextSpawn();
			}
		}
	
		/// <summary>
		/// Gets or sets the length of the max animation.
		/// </summary>
		/// <value>The length of the max animation.</value>
		TimeSpan? maxAnimationLength;
		TimeSpan? MaxAnimationLength 
		{
			get { return maxAnimationLength; }
			set 
			{
				maxAnimationLength = value;
				AnimationLength = value;
			}
		}

		/// <summary>
		/// Gets or sets the time until next spawn.
		/// </summary>
		/// <value>The time until next spawn.</value>
		TimeSpan TimeUntilNextSpawn { get; set; }

		/// <summary>
		/// Gets or sets the max time until next spawn.
		/// </summary>
		/// <value>The max time until next spawn.</value>
		TimeSpan MaxTimeUntilNextSpawn { get; set; }


		public Vector2 ParticleInitialVelocity { get; set; } 
		public Vector2 ParticleForce { get; set; }
		public float ParticleLifeTimeRandomizer { get; set; }
		public float ParticleVelocityRandomizer { get; set; }
		public float ParticleForceRandomizer { get; set; }
		public float ParticleAlphaPercent { get; set; }
		public string ParticleFile { get; set; }
		TimeSpan LifeTime;
		public Color Tint;

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.ParticleSystem"/> class.
		/// While the animation length is equal to null, the animation is endless
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="numberOfParticle">Number of particle.</param>
		/// <param name="animationLength">Animation length.</param>
		/// <param name="particleLifeTime">Particle life time.</param>
        public ParticleSystem(int numberOfParticle, 
		                      TimeSpan? animationLength = null,
		                      TimeSpan? particleLifeTime = null) 
							  : base()
        {
			// Particles settings
			ParticleFile = "particle";
			ParticleInitialVelocity = new Vector2(30, -100);
			ParticleForce = new Vector2(2, 10);
			ParticleLifeTimeRandomizer = 0.3f;
			ParticleVelocityRandomizer = 0.2f;
			ParticleForceRandomizer = 1.8f;
			ParticleAlphaPercent = 1f;
			Tint = Color.White;

			NumberOfParticles = numberOfParticle;
			MaxAnimationLength = animationLength;

			Particules = new List<DrawableParticle>();

			LifeTime = (particleLifeTime == null ? new TimeSpan(0, 0, 5) : particleLifeTime.Value);
        }

		/// <summary>
		/// Creates the particles.
		/// This method is made to be overwriten
		/// By customising this function, the particle 
		/// behavior can be customized
		/// </summary>
		protected virtual void CreateParticles(int number, TimeSpan lifeTime)
		{
			for (int i = 0; i < number; ++i) {
				DrawableParticle particle = new DrawableParticle(lifeTime, ParticleInitialVelocity, ParticleForce, 
				                                                 ParticleLifeTimeRandomizer, ParticleVelocityRandomizer, ParticleForceRandomizer, ParticleAlphaPercent, ParticleFile);
				particle.Tint = Tint;

				Particules.Add(particle);
				AddChild(particle);
			}
		}

		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			CreateParticles(NumberOfParticles,LifeTime);
		}

		/// <summary>
		/// Calculates the time until next spawn.
		/// </summary>
		private void CalculateTimeUntilNextSpawn()
		{
		
			if (AnimationLength != null && NumberOfParticles != 0) {
				MaxTimeUntilNextSpawn = TimeSpan.FromTicks(MaxAnimationLength.Value.Ticks / NumberOfParticles);
				TimeUntilNextSpawn = MaxTimeUntilNextSpawn;
			} 
			//TODO repenser ici
			else if (AnimationLength == null && NumberOfParticles != 0) {
				MaxTimeUntilNextSpawn = TimeSpan.FromTicks(new TimeSpan(0, 0, 1).Ticks / NumberOfParticles);
				TimeUntilNextSpawn = MaxTimeUntilNextSpawn;
			}
		
		}

		protected override void OnUpdate(GameTime dt)
		{
			if (AnimationLength == null || AnimationLength.Value.TotalMilliseconds > 0) {
				//Calculate the animation length
				AnimationLength -= dt.ElapsedGameTime;

				//Calculate the number of particules to spawn
				int numberToRevive = 0;

				TimeUntilNextSpawn -= dt.ElapsedGameTime;

				while (TimeUntilNextSpawn.TotalMilliseconds <= 0) {
					++numberToRevive;
					TimeUntilNextSpawn += MaxTimeUntilNextSpawn;
				}


				//Revive the good amount of particules
				for (int i = Particules.Count - 1; i >= 0 && numberToRevive > 0; --i) {
					if (!Particules[i].Alive) {
						Particules[i].Reset();
						--numberToRevive;
					}
				}
			}
			base.OnUpdate(dt);

		}
    }
}

