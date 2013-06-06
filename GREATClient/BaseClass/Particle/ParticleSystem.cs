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

namespace GREATClient
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
		int NumberOfParticules 
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

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.ParticleSystem"/> class.
		/// While the animation length is equal to null, the animation is endless
		/// </summary>
		/// <param name="content">Content.</param>
		/// <param name="numberOfParticle">Number of particle.</param>
		/// <param name="animationLength">Animation length.</param>
		/// <param name="particleLifeTime">Particle life time.</param>
        public ParticleSystem(ContentManager content,
		                      int numberOfParticle, 
		                      TimeSpan? animationLength = null,
		                      TimeSpan? particleLifeTime = null) 
							  : base(content)
        {
			Console.WriteLine(NumberOfParticules);
			NumberOfParticules = numberOfParticle;
			MaxAnimationLength = animationLength;

			Particules = new List<DrawableParticle>();

			TimeSpan lifeTime = (particleLifeTime == null ? new TimeSpan(0, 0, 1) : particleLifeTime.Value);

			CreateParticles(numberOfParticle,lifeTime);
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
				DrawableParticle particle = new DrawableParticle(lifeTime, new Vector2(200, 0), new Vector2(10, 40), 0.2f, 0.1f, 1.5f);
				Particules.Add(particle);
				AddChild(particle);
			}
		}

		/// <summary>
		/// Calculates the time until next spawn.
		/// </summary>
		private void CalculateTimeUntilNextSpawn()
		{
		
			if (AnimationLength != null && NumberOfParticules != 0) {
				MaxTimeUntilNextSpawn = TimeSpan.FromTicks(MaxAnimationLength.Value.Ticks / NumberOfParticules);
				TimeUntilNextSpawn = MaxTimeUntilNextSpawn;
			} 
			else if (AnimationLength == null && NumberOfParticules != 0) {
				MaxTimeUntilNextSpawn = TimeSpan.FromTicks(new TimeSpan(0, 0, 1).Ticks / NumberOfParticules);
				TimeUntilNextSpawn = MaxTimeUntilNextSpawn;
			}
		
		}

		//TODO : faire aparraitre graduellement les particules et les faire revivre
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

