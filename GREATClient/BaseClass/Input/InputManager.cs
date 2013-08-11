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
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
		/// Gets or sets the old mouse.
		/// </summary>
		/// <value>The old mouse.</value>
		MouseState OldMouse { get; set; }

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
			OldMouse = Mouse.GetState();
			OldKeyboard = Keyboard.GetState();
        }

		protected void OnUpdate()
		{
			ActionsFired.Clear();
			KeyboardState keyboardState = Keyboard.GetState();
			MouseState mouseState = Mouse.GetState();
			foreach (KeyValuePair<InputActions,InputState> action in Inputs.Info) {
				// Manage mouse events.
				if (!action.Value.IsKeyboard && action.Value.MouseKey != MouseKeys.None) {
					if (CheckMouseState(keyboardState,mouseState, action.Value)) {
						if (InputEvents.ContainsKey(action.Key)) {
							InputEvents[action.Key](this, new InputEventArgs());
						}
						ActionsFired.Add(action.Key);
					}
				} else if (action.Value.KeyboardKey != Keys.None) {
					switch (action.Value.State) {
						// The type of key state.
						case KeyState.Up:
							// Makes sure it is in the good state.
							if (keyboardState.IsKeyUp(action.Value.KeyboardKey) && CheckDeadKey(keyboardState, action.Value.DeadKey)) {
								if (InputEvents.ContainsKey(action.Key)) {
									InputEvents[action.Key](this, new InputEventArgs());
								}
								// Adds it to the list to be able to query all event fired in this frame.
								ActionsFired.Add(action.Key);
							}
						break;
						case KeyState.Down:
							if (keyboardState.IsKeyDown(action.Value.KeyboardKey) && CheckDeadKey(keyboardState,action.Value.DeadKey)) {
								if (InputEvents.ContainsKey(action.Key)) {
									InputEvents[action.Key](this, new InputEventArgs());
								}
								ActionsFired.Add(action.Key);
							}
						break;
						case KeyState.Pressed:
							if (keyboardState.IsKeyDown(action.Value.KeyboardKey) && OldKeyboard.IsKeyUp(action.Value.KeyboardKey) && 
							    CheckDeadKey(keyboardState,action.Value.DeadKey)) 
							{
								if (InputEvents.ContainsKey(action.Key)) {
									InputEvents[action.Key](this, new InputEventArgs());
								}
								ActionsFired.Add(action.Key);
							}
						break;
						case KeyState.Released:
							if (keyboardState.IsKeyUp(action.Value.KeyboardKey) && OldKeyboard.IsKeyDown(action.Value.KeyboardKey) && 
							    CheckDeadKey(keyboardState,action.Value.DeadKey)) 
							{
								if (InputEvents.ContainsKey(action.Key)) {
									InputEvents[action.Key](this, new InputEventArgs());
								}
								ActionsFired.Add(action.Key);
							}
						break;
					}				
				}
			}
			OldKeyboard = keyboardState;
			OldMouse = mouseState;
		}

		public void Update()
		{
			if (Updatable) {
				OnUpdate();
			}
		}

		bool CheckMouseState(KeyboardState keyboardState, MouseState mouseState, InputState inputState) {
			Assert.IsFalse(inputState.IsKeyboard);
			Assert.IsFalse(inputState.MouseKey == MouseKeys.None);

			if (inputState.MouseKey != MouseKeys.None) {
				// Start with dead key.
				if (CheckDeadKey(keyboardState,inputState.DeadKey)) {				
					// Manage wheel mouvement.
					if (inputState.MouseKey == MouseKeys.WheelDown) {
						if (mouseState.ScrollWheelValue < OldMouse.ScrollWheelValue) {
							return true;
						}
					} else if (inputState.MouseKey == MouseKeys.WheelDown) {
						if (mouseState.ScrollWheelValue > OldMouse.ScrollWheelValue) {
							return true;
						}
						// Now the other keys.
					} else {
						switch (inputState.MouseKey) {
							case MouseKeys.Left:
								if (CheckAMouseKey(mouseState.LeftButton, OldMouse.LeftButton, inputState.State)) {
									return true;
								}
							break;
							case MouseKeys.Right:
								if (CheckAMouseKey(mouseState.RightButton, OldMouse.RightButton, inputState.State)) {
									return true;
								}
							break;
							case MouseKeys.Wheel:
								if (CheckAMouseKey(mouseState.MiddleButton, OldMouse.MiddleButton, inputState.State)) {
									return true;
								}
							break;
						}
					}
				}
			}


			return false;
		}

		bool CheckAMouseKey(ButtonState buttonState, ButtonState oldButtonState, KeyState keyState ) {			

			switch (keyState) {
				case KeyState.Down:
					if (buttonState == ButtonState.Pressed) {
						return true;
					}
				break;
				case KeyState.Up:
					if (buttonState == ButtonState.Released) {
						return true;
					}
				break;
				case KeyState.Released:
					if (buttonState == ButtonState.Released && oldButtonState == ButtonState.Pressed) {
						return true;
					}
				break;
				case KeyState.Pressed:
					if (buttonState == ButtonState.Pressed && oldButtonState == ButtonState.Released) {
						return true;
					}
				break;
			}

			return false;
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
					if (!(keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt)) &&
					    !(keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift)) &&
					    !(keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.LeftControl))) {
						return true;
					}
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

