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
		int NumberOfParticules { get; set; }

		/// <summary>
		/// The length of the animation.
		/// </summary>
		TimeSpan? AnimationLength { get; set; }

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
			NumberOfParticules = numberOfParticle;
			AnimationLength = animationLength;

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
				DrawableParticle particle = new DrawableParticle(lifeTime, new Vector2(200, 0), new Vector2(10, 30), 0.2f, 0.1f, 1.5f);
				Particules.Add(particle);
				AddChild(particle);
			}
		}

		//TODO : faire aparraitre graduellement les particules et les faire revivre
		protected override void OnUpdate(GameTime dt)
		{
			int numberToRevive = 0;

			if (AnimationLength == null) {
				//TODO 
				numberToRevive = 1;
			} else {
				numberToRevive = NumberOfParticules / (int)AnimationLength.Value.TotalMilliseconds;
				if(numberToRevive==0)
					++numberToRevive;
			}


			//Reveive the good amount of particules
			for(int i = Particules.Count - 1 ; i > 0 && numberToRevive > 0 ; --i )
			{
				if (!Particules[i].Alive) {
					Particules[i].Reset();
					--numberToRevive;
				}
			}

			base.OnUpdate(dt);

		}
    }
}

