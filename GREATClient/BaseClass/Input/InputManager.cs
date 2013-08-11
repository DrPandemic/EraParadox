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
		/// Gets or sets a list of actions fired in this tick.
		/// </summary>
		/// <value>The actions fired.</value>
		List<InputActions> ActionsFired { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.Input.InputManager"/> is updatable.
		/// </summary>
		/// <value><c>true</c> if updatable; otherwise, <c>false</c>.</value>
		public bool Updatable { get; set; }

        public InputManager()
        {
			Inputs = new Inputs();
			InputEvents = new Dictionary<InputActions,EventHandler>();
			ActionsFired = new List<InputActions>();
			Updatable = true;
        }

		protected void OnUpdate()
		{
			ActionsFired.Clear();
			foreach (KeyValuePair<InputActions,InputState> action in Inputs.Info) {
				switch (action.Value.State) {
					// The type of key state.
					case KeyState.Up:
						// Makes sure it is in the good state.
						if (Keyboard.GetState().IsKeyUp(action.Value.Key) && CheckDeadKey(Keyboard.GetState(),action.Value.DeadKey)) {
							if (InputEvents.ContainsKey(action.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							// Adds it to the list to be able to query all event fired in this frame.
							ActionsFired.Add(action.Key);
						}
						break;
					case KeyState.Down:
						if (Keyboard.GetState().IsKeyDown(action.Value.Key) && CheckDeadKey(Keyboard.GetState(),action.Value.DeadKey)) {
							if (InputEvents.ContainsKey(action.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							ActionsFired.Add(action.Key);
						}
						break;
					case KeyState.Pressed:
						if (Keyboard.GetState().IsKeyDown(action.Value.Key) && OldKeyboard.IsKeyUp(action.Value.Key) && 
						    CheckDeadKey(Keyboard.GetState(),action.Value.DeadKey)) 
						{
							if (InputEvents.ContainsKey(action.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							ActionsFired.Add(action.Key);
						}
						break;
					case KeyState.Released:
						if (Keyboard.GetState().IsKeyUp(action.Value.Key) && OldKeyboard.IsKeyDown(action.Value.Key) && 
						    CheckDeadKey(Keyboard.GetState(),action.Value.DeadKey)) 
						{
							if (InputEvents.ContainsKey(action.Key)) {
								InputEvents[action.Key](this, new InputEventArgs());
							}
							ActionsFired.Add(action.Key);
						}
						break;
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

		/// <summary>
		/// Determines if an action was fired on during since last drop call.
		/// </summary>
		/// <returns><c>true</c>, if the event happend, <c>false</c> otherwise.</returns>
		/// <param name="action">Action.</param>
		public bool IsActionFired(InputActions action)
		{
			return ActionsFired.Contains(action);
		}

		/// <summary>
		/// Checks if the dead key is down.
		/// </summary>
		/// <returns><c>true</c>, if dead key is down, <c>false</c> otherwise.</returns>
		/// <param name="keyboardState">Keyboard state.</param>
		/// <param name="deadKey">Dead key.</param>
		bool CheckDeadKey(KeyboardState keyboardState, DeadKeys deadKey)
		{
			switch (deadKey) {
				case DeadKeys.Alt:
					if (keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt)) {
						return true;
					}
				break;
				case DeadKeys.Shift:
					if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)) {
						return true;
					}
				break;
				case DeadKeys.Control:
					if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.LeftControl)) {
						return true;
					}
				break;
				case DeadKeys.None:
					return true;
				break;
			}
			return false;
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

