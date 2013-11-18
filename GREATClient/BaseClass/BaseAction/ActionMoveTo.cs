//
//  ActionMoveTo.cs
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
using System.Diagnostics;

namespace GREATClient.BaseClass.BaseAction
{
    public class ActionMoveTo : ActionOverTime
    {
		/// <summary>
		/// Gets or sets the destination.
		/// </summary>
		/// <value>The destination.</value>
		protected Vector2 Destination { get; set; }

		/// <summary>
		/// Gets or sets the mouvement that should be done every millisecond.
		/// </summary>
		/// <value>The mouvement by millisecond.</value>
		protected Vector2 MouvementByMillisecond { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Action.ActionMoveTo"/> class.
		/// </summary>
		/// <param name="duration">Duration of the animation.</param>
		/// <param name="destination">Destination of the <see cref="GREATClient.BaseClass.IDraw"/>.</param>
        public ActionMoveTo(TimeSpan duration, Vector2 destination) : base(duration)
        {
			Debug.Assert(duration.Ticks > 0);
			Destination = destination;
        }

		public override void Ready()
		{
			Debug.Assert(Target != null);
			float x = (Destination.X - Target.Position.X) / (float)Duration.TotalMilliseconds;
			float y = (Destination.Y - Target.Position.Y) / (float)Duration.TotalMilliseconds;

			MouvementByMillisecond = new Vector2(x,y);
		}

		protected override void OnUpdate(GameTime dt)
		{
			Debug.Assert(Target != null);

			Target.Position = Vector2.Add(Target.Position, 
			                              Vector2.Multiply(MouvementByMillisecond,
			                                               (float)dt.ElapsedGameTime.TotalMilliseconds));
		}

		public override void Reset()
		{
			base.Reset();
		}
    }
}

