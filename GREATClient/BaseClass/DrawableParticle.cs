//
//  DrawableParticle.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GREATClient
{
	public class DrawableParticle : DrawableImage
    {
		/// <summary>
		/// Gets or sets the life time.
		/// </summary>
		/// <value>The life time.</value>
		protected TimeSpan LifeTime { get; set; }

		/// <summary>
		/// Gets or sets the max velocity.
		/// </summary>
		/// <value>The max velocity.</value>
		protected Vector2 MaxVelocity { get; set; }

		/// <summary>
		/// Gets or sets the velocity.
		/// The velocity is in pixel/second
		/// </summary>
		/// <value>The velocity.</value>
		protected Vector2 Velocity { get; set; }

		/// <summary>
		/// Gets or sets the outside force.
		/// </summary>
		/// <value>The force.</value>
		protected Vector2 Force { get; set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="GREATClient.DrawableParticle"/> is alive.
		/// </summary>
		/// <value><c>true</c> if alive; otherwise, <c>false</c>.</value>
		public bool Alive { get; private set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.DrawableParticle"/> class.
		/// The randomizer are calculated like that : 
		/// LifeTime = lifeTime * (lifeTimeRandomizer - 2 * (random * lifeTimeRandomizer)).
		/// The force will be multiplied with the velo.
		/// But the velo and the force are calculated for one second.
		/// </summary>
		/// <param name="lifeTime">Life time.</param>
		/// <param name="initialVelocity">Initial velocity.</param>
		/// <param name="force">Force.</param>
		/// <param name="forceRandomizer">Force randomizer.</param>
		/// <param name="velocityRandomizer">Velocity randomizer.</param>
		/// <param name="lifeTimeRandomizer">Life time randomizer.</param>
		/// <param name="file">File.</param>
        public DrawableParticle(TimeSpan lifeTime, Vector2 initialVelocity, Vector2 force, 
		                        float lifeTimeRandomizer = 0, float velocityRandomizer = 0, float forceRandomizer = 0 ,
		                        string file = "particle") 
			: base(file)
        {
			Scale = new Vector2(0.1f, 0.1f);

			LifeTime =  TimeSpan.FromTicks((long)(lifeTime.Ticks * GetRandomForPrecision(lifeTimeRandomizer)));

			MaxVelocity = new Vector2(initialVelocity.X * GetRandomForPrecision(velocityRandomizer),
			                          initialVelocity.Y * GetRandomForPrecision(velocityRandomizer));
			Velocity = MaxVelocity;

			Force = new Vector2(force.X * GetRandomForPrecision(forceRandomizer),
			                    force.Y * GetRandomForPrecision(forceRandomizer));

			Alive = true;

			OriginRelative = new Vector2(0.5f, 0.5f);
        }

		protected override void OnUpdate(GameTime dt)
		{
			//Calculate life time
			LifeTime -= dt.ElapsedGameTime;

			if (LifeTime <= TimeSpan.Zero) {
				Alive = false;
			} else {
				//Calculate the effect of the force on the velocity
				//The point here, is that the force is calculate for one second
				Velocity += (Force * (float)dt.ElapsedGameTime.TotalMilliseconds) / 1000;

				//Calculate the effect on the position
				Position += (Velocity * (float)dt.ElapsedGameTime.TotalMilliseconds) / 1000;
			}
		}



		/// <summary>
		/// Generate a random number between the -precision and +precision
		/// </summary>
		/// <returns>The random for precision.</returns>
		/// <param name="precision">Precision.</param>
		float GetRandomForPrecision(float precision)
		{
			return 1.0f + Utilities.Random.RandomFloat(-precision, precision);
		}
		public override void Draw(SpriteBatch batch)
		{
			if (Alive)
				base.Draw(batch);
		}

    }
}

