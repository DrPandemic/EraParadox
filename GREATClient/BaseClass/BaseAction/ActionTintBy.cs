//
//  ActionTintBy.cs
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
	public class ActionTintBy : ActionOverTimeDrawable
    {
		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		protected Vector3 Color { get; set; }

		/// <summary>
		/// Gets or sets the initial color.
		/// With that, we can reset to the initial color.
		/// </summary>
		/// <value>The initial color.</value>
		protected Color InitialColor { get; set; }

		/// <summary>
		/// Gets or sets the color change be millisecond.
		/// </summary>
		/// <value>The color change be millisecond.</value>
		protected Vector3 ColorChangeBeMillisecond { get; set; }

		public ActionTintBy(TimeSpan duration, Vector3 color) : base(duration)
		{
			Color = color / 255f;
			InitialColor = new Color(color);
		}

		public override void Ready()
		{
			Debug.Assert(Target != null);
			ColorChangeBeMillisecond = Vector3.Divide(Color,(float)Duration.TotalMilliseconds);
		}

		public override void Reset()
		{
			Color = InitialColor.ToVector3();
			base.Reset();
		}

		protected override void OnUpdate(GameTime dt)
		{
			Debug.Assert(Target != null);
			Drawable Tar = (Drawable)Target;
			Tar.Tint = new Color((Tar.Tint.ToVector3()) + (ColorChangeBeMillisecond * (float)dt.ElapsedGameTime.TotalMilliseconds));
		}
    }
}

