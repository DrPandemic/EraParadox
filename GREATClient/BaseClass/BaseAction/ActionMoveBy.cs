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
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.BaseAction.ActionMoveBy"/> is in function mode.
		/// </summary>
		/// <value><c>true</c> if function mode; otherwise, <c>false</c>.</value>
		bool FunctionMode 
		{ 
			get {
				return Movement != null;
			}
		}

		/// <summary>
		/// Gets or sets the movement function.
		/// Vector2 : First position.
		/// Float : Movement completion ratio (InitialDuration - Duration) / InitialDuration) of this tick.
		/// Vector2 : Position after the function modification.
		/// </summary>
		/// <value>The movement.</value>
		Func<Vector2,float,Vector2> Movement { get; set; }

		/// <summary>
		/// Gets or sets the first position.
		/// Only used with the funciton mode.
		/// </summary>
		/// <value>The first position.</value>
		Vector2 FirstPosition { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Action.ActionMoveBy"/> class.
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="moveBy">Move by.</param>
		public ActionMoveBy(TimeSpan duration, Vector2 moveBy) : base(duration,moveBy)
		{ }

		public ActionMoveBy(TimeSpan duration, Func<Vector2,float,Vector2> movement) : base(duration,Vector2.Zero)
		{
			Movement = movement;
		}

		public override void Ready()
		{
			Debug.Assert(Target != null);
			if (!FunctionMode) {
				MouvementByMillisecond = new Vector2(Destination.X / (float)Duration.TotalMilliseconds, 
				                                     Destination.Y / (float)Duration.TotalMilliseconds); 
			} else {
				FirstPosition = Target.Position;
			}
		}

		protected override void OnUpdate(GameTime dt)
		{
			if (!FunctionMode) {				
				base.OnUpdate(dt);
			} else {
				Debug.Assert(Target != null);
				float ratio = (InitialDuration.Ticks - Duration.Ticks) / (float)InitialDuration.Ticks;
				if (ratio > 1) {
					ratio = 1;
				}
				Target.Position = Movement(FirstPosition, ratio);
			}
		}
	}
}

