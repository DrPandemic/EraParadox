//
//  ActionRotateBy.cs
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
    public class ActionRotateBy : ActionOverTimeDrawable
    {
		/// <summary>
		/// Gets or sets the rotation.
		/// </summary>
		/// <value>The rotation.</value>
		protected float Rotation { get; set; }

		/// <summary>
		/// Gets or sets the initial rotation.
		/// Is used to be able to reset the instance.
		/// </summary>
		/// <value>The initial rotation.</value>
		protected float InitialRotation { get; set; }

		/// <summary>
		/// Gets or sets the rotation by millisecond.
		/// </summary>
		/// <value>The rotation by millisecond.</value>
		protected double RotationByMillisecond { get; set; }

		public ActionRotateBy(TimeSpan duration, float rotation, bool isRadian) : base (duration)
        {
			if (!isRadian) {
				rotation = Utilities.GetRadian(rotation);
			}
			Rotation = rotation;
			InitialRotation = rotation;
        }

		protected override void OnUpdate(GameTime dt)
		{
			Debug.Assert(Target != null);

			((Drawable)Target).Orientation += RotationByMillisecond * dt.ElapsedGameTime.TotalMilliseconds;
		}

		public override void Ready()
		{
			Debug.Assert(Target != null);				

			RotationByMillisecond = Rotation / Duration.TotalMilliseconds;
		}

		public override void Reset()
		{
			Rotation = InitialRotation;
			base.Reset();
		}
    }
}

