//
//  ActionRotateTo.cs
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

namespace GREATClient.BaseClass.BaseAction
{
    public class ActionRotateTo : ActionRotateBy
    {
		public ActionRotateTo(TimeSpan duration, float rotation, bool isRadian) : base(duration,rotation,isRadian)
        {
        }

		public override void Ready()
		{
			Debug.Assert(Target != null);				

			RotationByMillisecond = (Rotation - ((Drawable)Target).Orientation) / Duration.TotalMilliseconds;
		}
    }
}

