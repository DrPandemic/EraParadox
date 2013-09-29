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
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace GREATClient.BaseClass.Input
{
    public class InputManager
    {
		/// <summary>
		/// The default mouse x.
		/// </summary>
		public static int DefaultMouseX = 400;

		/// <summary>
		/// The default mouse y.
		/// </summary>
		public static int DefaultMouseY = 240;

		/// <summary>
		/// Gets or sets a value indicating whether the window is ready.
		/// </summary>
		/// <value><c>true</c> if this instance is window ready; otherwise, <c>false</c>.</value>
		public bool IsWindowReady { get; set; }

		/// <summary>
		/// Gets or sets the cursor.
		/// </summary>
		/// <value>The cursor.</value>
		public DrawableImage Cursor { get; set; }

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

		/// <summary>
		/// Gets the mouse position.
		/// </summary>
		/// <value>The mouse position.</value>
		public Vector2 MousePosition 
		{ 
			get {
				return new Vector2(MouseX,MouseY);
			}
		}

		/// <summary>
		/// Gets the mouse x.
		/// </summary>
		/// <value>The mouse x.</value>
		public int MouseX 
		{
			get {
				return OldMouse.X;
			}
		}

		/// <summary>
		/// Gets the mouse y.
		/// </summary>
		/// <value>The mouse y.</value>
		public int MouseY
		{
			get {
				return OldMouse.Y;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the screen is active AKA the window is focused.
		/// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
		public bool IsActive { get; set; }

        public InputManager()
        {
			IsActive = false;
			IsWindowReady = false;
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
							InputEvents[action.Key](this, new InputEventArgs() { MousePosition = MousePosition });
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

			if (IsWindowReady && IsActive) {
				Cursor.Position = MousePosition;
			}
		}

		public void Update()
		{
			if (Updatable) {
				OnUpdate();
			}
		}

		/// <summary>
		/// Checks the state of the mouse to throw the good events.
		/// </summary>
		/// <returns><c>true</c>, if mouse state was checked, <c>false</c> otherwise.</returns>
		/// <param name="keyboardState">Keyboard state.</param>
		/// <param name="mouseState">Mouse state.</param>
		/// <param name="inputState">Input state.</param>
		bool CheckMouseState(KeyboardState keyboardState, MouseState mouseState, InputState inputState) {
			Debug.Assert(!inputState.IsKeyboard);
			Debug.Assert(inputState.MouseKey != MouseKeys.None);

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

		/// <summary>
		/// Checks precise mouse key state.
		/// </summary>
		/// <returns><c>true</c>, if A mouse key was checked, <c>false</c> otherwise.</returns>
		/// <param name="buttonState">Button state.</param>
		/// <param name="oldButtonState">Old button state.</param>
		/// <param name="keyState">Key state.</param>
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
			if (!InputEvents.ContainsKey(action)) {
				InputEvents.Add(action,null);
			}
			InputEvents[action] += callback;
		}

		/// <summary>
		/// Remove all events for a given action.
		/// </summary>
		/// <param name="action">Action.</param>
		public void RemoveAction(InputActions action)
		{
			InputEvents.Remove(action);
		}

		/// <summary>
		/// Removes the action.
		/// </summary>
		/// <param name="e">E.</param>
		public void RemoveAction(InputActions a, EventHandler e)
		{
			if (InputEvents.ContainsKey(a)) {
				InputEvents[a] -= e;
			}
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
		/// If there is multiple dead keys, drop the action.
		/// </summary>
		/// <returns><c>true</c>, if dead key is down, <c>false</c> otherwise.</returns>
		/// <param name="keyboardState">Keyboard state.</param>
		/// <param name="deadKey">Dead key.</param>
		bool CheckDeadKey(KeyboardState keyboardState, DeadKeys deadKey)
		{
			bool altDown = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
			bool shiftDown = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
			bool controlDown = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.LeftControl);

			if (deadKey == DeadKeys.None) {
				return true;
			}
			// Makes sure there is only one dead key pressed.
			if (altDown ? (!shiftDown && !controlDown) : (shiftDown ^ controlDown)) {
				switch (deadKey) {
					case DeadKeys.Alt:
						if (altDown) {
							return true;
						}
						break;
						case DeadKeys.Shift:
						if (shiftDown) {
							return true;
						}
						break;
						case DeadKeys.Control:
						if (controlDown) {
							return true;
						}
						break;
						case DeadKeys.None:
						return true;
				}
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

		/// <summary>
		/// Gets or sets the mouse position.
		/// Will be zero for keyboard events.
		/// </summary>
		/// <value>The mouse position.</value>
		public Vector2 MousePosition { get; set; }

		public InputEventArgs() 
		{
			Handled = false;
			MousePosition = Vector2.Zero;
		}
	}

	public class NotImplementedActionException : Exception
	{
		public NotImplementedActionException() : base("Couldn't read the XML(" + Inputs.INPUT_FILE + ").") {}
	}

	public class ActionDeserializationException : NotImplementedActionException
	{
		public ActionDeserializationException() : base() {}
	}
}

