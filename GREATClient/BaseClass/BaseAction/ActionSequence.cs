//
//  ActionSequence.cs
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GREATClient.BaseClass.BaseAction
{
    public class ActionSequence : ActionOverTime
    {
		public static int INFINITE_SEQUENCE = -100;

		/// <summary>
		/// Gets or sets the repeat count.
		/// </summary>
		/// <value>The repeat count.</value>
		protected int RepeatCount { get; set; }

		/// <summary>
		/// Gets or sets the initial repeat count.
		/// Is used to be able to reset the instance.
		/// </summary>
		/// <value>The initial repeat count.</value>
		protected int InitialRepeatCount { get; set; }

		/// <summary>
		/// Gets or sets the action list.
		/// </summary>
		/// <value>The action list.</value>
		List<ActionOverTime> ActionList { get; set; }

		/// <summary>
		/// Keep a reference to the current action.
		/// </summary>
		/// <value>The current action.</value>
		int CurrentAction { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Action.ActionSequence"/> class.
		/// Won't repeat the actions.
		/// </summary>
		/// <param name="actions">Actions.</param>
		public ActionSequence(params ActionOverTime[] actions) : base(new TimeSpan(0))
        {
			Updatable = false;
			CurrentAction = 0;
			RepeatCount = 1;
			InitialRepeatCount = 1;
			SetActionList(actions);
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Action.ActionSequence"/> class.
		/// Will be repeated the given number of time.
		/// If a negative number is passed, the actions will be repeated forever.
		/// </summary>
		/// <param name="repeat">Repeat count.</param>
		/// <param name="actions">Actions.</param>
		public ActionSequence(int repeat, params ActionOverTime[] actions) : base(new TimeSpan(0))
		{
			Updatable = false;
			CurrentAction = 0;
			RepeatCount = repeat < 0 ? INFINITE_SEQUENCE : repeat;
			InitialRepeatCount = RepeatCount;
			SetActionList(actions);
		}

		/// <summary>
		/// Sets the list of actions.
		/// </summary>
		/// <param name="actions">Actions.</param>
		void SetActionList(params ActionOverTime[] actions)
		{
			ActionList = actions.OfType<ActionOverTime>().ToList();
			ActionList.ForEach(item => item.DoneActionForSequence = new Action<ActionOverTime>(AnActionIsDone));
		}

		public override void Ready()
		{
			RunAction();
		}

		/// <summary>
		/// Runs the next action.
		/// </summary>
		protected void RunAction()
		{
			Debug.Assert(Target != null);
			if (RepeatCount > 0) {
				Target.PerformAction(ActionList[CurrentAction]);
			} else if (RepeatCount <= INFINITE_SEQUENCE) {
				// Infinite loop.
				RepeatCount = INFINITE_SEQUENCE;
				Target.PerformAction(ActionList[CurrentAction]);
			} else {
				Done(null);
			}
		}

		/// <summary>
		/// Called each time an action is completed.
		/// </summary>
		/// <param name="action">Action.</param>
		public void AnActionIsDone(ActionOverTime action)
		{
			// Reset the action when it is over.
			ActionList[CurrentAction].Reset();

			if (CurrentAction >= ActionList.Count - 1) {
				CurrentAction = 0;
				RepeatCount--;
			} else {
				CurrentAction++;
			}
			RunAction();
		}

		public override void Reset()
		{
			base.Reset();
			Updatable = false;
			CurrentAction = 0;
			RepeatCount = InitialRepeatCount;
			ActionList.ForEach((ActionOverTime item) => item.Reset());
		}
	}
}

