//
//  ActionFadeBy.cs
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
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace GREATClient.BaseClass.BaseAction
{
    public class ActionFadeBy : ActionOverTimeDrawable
    {
		/// <summary>
		/// Gets or sets the alpha.
		/// </summary>
		/// <value>The alpha.</value>
		protected float Alpha { get; set; }

		/// <summary>
		/// Gets or sets the initiallpha.
		/// Is used to be able to reset the instance.
		/// </summary>
		/// <value>The initiallpha.</value>
		protected float InitialAlpha { get; set; }

		/// <summary>
		/// Gets or sets the alpha change be millisecond.
		/// </summary>
		/// <value>The alpha change be millisecond.</value>
		protected double AlphaChangeBeMillisecond { get; set; }

        public ActionFadeBy(TimeSpan duration, float alpha) : base(duration)
        {
			Alpha = alpha;
			InitialAlpha = alpha;
        }

		public override void Ready()
		{
			Debug.Assert(Target != null);

			AlphaChangeBeMillisecond = Alpha / Duration.TotalMilliseconds;
		}

		public override void Reset()
		{
			Alpha = InitialAlpha;
			base.Reset();
		}

		protected override void OnUpdate(GameTime dt)
		{
			Debug.Assert(Target != null);

			(Target).Alpha += (float)(AlphaChangeBeMillisecond * dt.ElapsedGameTime.TotalMilliseconds);
		}
    }
}

