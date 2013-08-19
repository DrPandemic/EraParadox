//
//  Action.cs
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

namespace GREATClient.BaseClass.BaseAction
{
    public abstract class ActionOverTime
    {
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BaseClass.Action.ActionOverTime"/> is started.
		/// </summary>
		/// <value><c>true</c> if started; otherwise, <c>false</c>.</value>
		public bool Started { get; protected set; }

		/// <summary>
		/// Gets or sets the speed of the action.
		/// </summary>
		/// <value>The speed.</value>
		public float Speed { get; set; }

		/// <summary>
		/// Gets or sets the duration of the action.
		/// </summary>
		/// <value>The duration.</value>
		public TimeSpan Duration { get; protected set; }

		/// <summary>
		/// Gets or sets the initial duration.
		/// Is used to be able to reset the instance.
		/// </summary>
		/// <value>The initial duration.</value>
		protected TimeSpan InitialDuration { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this action is completed.
		/// </summary>
		/// <value><c>true</c> if this action is done; otherwise, <c>false</c>.</value>
		public bool IsDone { get; protected set; }

		/// <summary>
		/// Gets or sets the action fired when the action is done.
		/// </summary>
		/// <value>The done action.</value>
		public Action<Object> DoneAction { get; set; }

		/// <summary>
		/// Gets or sets the done action for sequence.
		/// Used by <see cref="GREATClient.BaseClass.Action.ActionSequence"/>.
		/// </summary>
		/// <value>The done action for sequence.</value>
		public Action<ActionOverTime> DoneActionForSequence { get; set; }

		/// <summary>
		/// Gets or sets the target.
		/// The target will be the object on which the action will act.
		/// The target can be mondify, but it shouldn't.
		/// </summary>
		/// <value>The target.</value>
		public IDraw Target { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Action.ActionOverTime"/> is updatable.
		/// </summary>
		/// <value><c>true</c> if updatable; otherwise, <c>false</c>.</value>
		protected bool Updatable { get; set; }

		public ActionOverTime(TimeSpan duration)
        {
			Started = false;
			IsDone = false;
			Speed = 1f;
			Updatable = true;
			Duration = duration;
			InitialDuration = duration;
        }

		/// <summary>
		/// Start this action.
		/// Can be overwritten, but it's not recommanded.
		/// </summary>
		public virtual void Start() 
		{
			Started = true;
			OnStart();
		}

		/// <summary>
		/// Called by Start().
		/// </summary>
		protected virtual void OnStart() { }

		/// <summary>
		/// Stop this action.
		/// Can be overwritten, but it's not recommanded.
		/// </summary>
		public virtual void Stop()
		{
			Started = false;
			OnStop();
		}

		/// <summary>
		/// Called by Stop().
		/// </summary>
		protected virtual void OnStop() { }

		/// <summary>
		/// Update
		/// Dt is disference of time since last call
		/// </summary>
		/// <param name="dt">Dt.</param>
		public void Update(GameTime dt) {
			if (Updatable && Started) {
				OnUpdate(dt);
				Duration -= dt.ElapsedGameTime;
				if (Duration.Ticks <= 0) {
					Done(null);
				}
			}
		}

		/// <summary>
		/// Called after Update if Updatable
		/// </summary>
		/// <param name="dt">Dt.</param>
		protected virtual void OnUpdate(GameTime dt)
		{ }

		/// <summary>
		/// Call this method when the action is done.
		/// Will call the DoneAction.
		/// </summary>
		/// <param name="args">The argument pass to the DoneAction.</param>
		protected virtual void Done(Object args) 
		{
			IsDone = true;
			Stop();
			if (DoneAction != null) {
				DoneAction(args);
			}
			if (Target != null) {
				Target.ActionDone(this, args);
			}
			if (DoneActionForSequence != null) {
				DoneActionForSequence(this);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public virtual void Reset() {
			Started = false;
			IsDone = false;
			Updatable = true;
			Target = null;
			Duration = InitialDuration;
		}

		/// <summary>
		/// Called be <see cref="GREATClient.BaseClass.IDraw"/> when it set the target.
		/// </summary>
		public virtual void Ready()
		{ }
    }
}

