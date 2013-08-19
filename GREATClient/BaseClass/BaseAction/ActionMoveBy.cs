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
	public class ActionMoveBy : ActionMoveTo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Action.ActionMoveBy"/> class.
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="moveBy">Move by.</param>
		public ActionMoveBy(TimeSpan duration, Vector2 moveBy) : base(duration,moveBy)
		{ }

		public override void Ready()
		{
			Debug.Assert(Target != null);

			MouvementByMillisecond = new Vector2(Destination.X / (float)Duration.TotalMilliseconds, 
			                                     Destination.Y / (float)Duration.TotalMilliseconds); 
		}
	}
}

