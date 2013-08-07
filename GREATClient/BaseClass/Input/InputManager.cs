//
//  InputManager.cs
//
//  Author:
//       The Parasithe <bipbip500@hotmail.com>
//
//  Copyright (c) 2013 The Parasithe
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
using Microsoft.Xna.Framework.Input;
using GREATClient.BaseClass.Input;
using System.Collections.Generic;

namespace GREATClient.BaseClass.Input
{
    public class InputManager
    {
		/// <summary>
		/// Gets or sets the inputs.
		/// </summary>
		/// <value>The inputs.</value>
		Inputs Inputs { get; set; }

		/// <summary>
		/// The keyboard state.
		/// </summary>
		KeyboardState OldKeyboard { get; set; }

		/// <summary>
		/// The dictionary holding the events handler for each action.
		/// </summary>
		Dictionary<InputActions,EventHandler> InputEvents { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Input.InputManager"/> is updatable.
		/// </summary>
		/// <value><c>true</c> if updatable; otherwise, <c>false</c>.</value>
		public bool Updatable { get; set; }

        public InputManager()
        {
			Inputs = new Inputs();
			InputEvents = new Dictionary<InputActions,EventHandler>();
			Keys k = Inputs.GetKey(InputActions.Spell1);
			InputInfo info = Inputs.GetInfo(InputActions.Spell1);
			Updatable = true;
        }

		protected void OnUpdate()
		{
			foreach (KeyValuePair<InputActions,KeyValuePair<Keys,KeyState>> action in Inputs.Info) {
				if (InputEvents.ContainsKey(action.Key)) {
					switch (action.Value.Value) {
						case KeyState.Up:
							if (Keyboard.GetState().IsKeyUp(action.Value.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							break;
						case KeyState.Down:
							if (Keyboard.GetState().IsKeyDown(action.Value.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							break;
						case KeyState.Pressed:
							if (Keyboard.GetState().IsKeyDown(action.Value.Key) && OldKeyboard.IsKeyUp(action.Value.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							break;
						case KeyState.Released:
							if (Keyboard.GetState().IsKeyUp(action.Value.Key) && OldKeyboard.IsKeyDown(action.Value.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							break;
					}
				}
			}
			OldKeyboard = Keyboard.GetState();
		}

		public void Update()
		{
			if (Updatable) {
				OnUpdate();
			}
		}

		/// <summary>
		/// Registers the event for a given action.
		/// </summary>
		/// <param name="action">Action.</param>
		/// <param name="callback">Callback.</param>
		public void RegisterEvent(InputActions action, EventHandler callback)
		{
			InputEvents.Add(action,null);
			InputEvents[action] += callback;
		}
    }

	public class InputEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Input.InputEventArgs"/> is handled.
		/// </summary>
		/// <value><c>true</c> if handled; otherwise, <c>false</c>.</value>
		public bool Handled { get; set; }

		public InputEventArgs() 
		{
			Handled = false;
		}
	}

	public class NotImplementedActionException : Exception
	{
		public NotImplementedActionException() : base("You fucked the XML(" + Inputs.INPUT_FILE + "). Don't do it again!") {}
	}

	public class ActionDeserializationException : NotImplementedActionException
	{
		public ActionDeserializationException() : base() {}
	}
}

