//
//  InputInfo.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using GREATClient.BaseClass.Input;
using System.Collections;

namespace GREATClient.BaseClass.Input
{
	[Serializable]
	public class KeyboardInputInfo
	{
		[System.Xml.Serialization.XmlAttribute("action")]
		public InputActions Action { get; set; }

		[System.Xml.Serialization.XmlAttribute("key")]
		public Keys Key { get; set; }

		[System.Xml.Serialization.XmlAttribute("state")]
		public KeyState State { get; set; }

		[System.Xml.Serialization.XmlAttribute("deadKey")]
		public DeadKeys DeadKey { get; set; }
	}

	[Serializable]
	public class MouseInputInfo
	{
		[System.Xml.Serialization.XmlAttribute("action")]
		public InputActions Action { get; set; }

		[System.Xml.Serialization.XmlAttribute("key")]
		public MouseKeys Key { get; set; }

		// For thw WheelUp/Down the state will be ignore.
		[System.Xml.Serialization.XmlAttribute("state")]
		public KeyState State { get; set; }

		[System.Xml.Serialization.XmlAttribute("deadKey")]
		public DeadKeys DeadKey { get; set; }
	}

	[Serializable]
	[System.Xml.Serialization.XmlRoot("inputCollection")]
	public class InputInfos
    {
		[System.Xml.Serialization.XmlArray("keyboardInputs")]
		[System.Xml.Serialization.XmlArrayItem("input", typeof(KeyboardInputInfo))]
		public KeyboardInputInfo[] KeyboadInputs { get; set; }

		[System.Xml.Serialization.XmlArray("MouseInputs")]
		[System.Xml.Serialization.XmlArrayItem("input", typeof(MouseInputInfo))]
		public MouseInputInfo[] MouseInputs { get; set; }
	}

	/// <summary>
	/// Contains every informations about an input from the keyboard.
	/// </summary>
	public class InputState 
	{
		public bool IsKeyboard { get; private set; }

		Keys keyboardKey;
		public Keys KeyboardKey 
		{ 
			get {
				return keyboardKey;
			}
			set {
				IsKeyboard = true;
				mouseKey = MouseKeys.None;
				keyboardKey = value;
			}
		}
		MouseKeys mouseKey;
		public MouseKeys MouseKey 
		{
			get {
				return mouseKey;
			}
			set {
				IsKeyboard = false;
				keyboardKey = Keys.None;
				mouseKey = value;
			}
		}

		KeyState state;
		public KeyState State { get; set; }
		public DeadKeys DeadKey { get; set; }
	}

	public class Inputs
	{
		/// <summary>
		/// The XML file containing the inputs.
		/// </summary>
		public const string INPUT_FILE = "inputs.xml";

		/// <summary>
		/// All the key infos.
		/// </summary>
		/// <value>The info.</value>
		public Dictionary<InputActions, InputState> Info { get; private set; }

		public Inputs()
        {
			FillInfo();
        }

		/// <summary>
		/// Gets the key for a given action.
		/// </summary>
		/// <returns>The key.</returns>
		/// <param name="action">Action.</param>
		public Keys GetKey(InputActions action)
		{
			if (!Info.ContainsKey(action)) {
				throw new Exception("The action is not present in the XML");
			}
			return Info[action].KeyboardKey;
		}

		/// <summary>
		/// Gets the info for a given action.
		/// </summary>
		/// <returns>The info.</returns>
		/// <param name="action">Action.</param>
		public KeyboardInputInfo GetInfo(InputActions action)
		{
			if (!Info.ContainsKey(action)) {
				throw new Exception("The action is not present in the XML");
			}
			return new KeyboardInputInfo() { Action = action, Key = Info[action].KeyboardKey, State = Info[action].State, DeadKey =  Info[action].DeadKey };
		}

		/// <summary>
		/// Gets the action for a given key and key state.
		/// </summary>
		/// <returns>The action.</returns>
		/// <param name="pair">The key, the key state and the dead key state</param>
		/*public InputActions GetAction(InputState pair){
			InputActions action = InputActions.None;
			return action;
		}*/

		/// <summary>
		/// Fills the info object from the xml.
		/// </summary>
		private void FillInfo()
		{
			const string INPUTS_PATH = "Content/" + INPUT_FILE;
			Info = new Dictionary<InputActions, InputState>();
			InputInfos inputs = null;

			XmlSerializer serializer = new XmlSerializer(typeof(InputInfos));

			StreamReader reader = new StreamReader(INPUTS_PATH);
			inputs = (InputInfos)serializer.Deserialize(reader);
			reader.Close();
		
			foreach (KeyboardInputInfo info in inputs.KeyboadInputs)
			{
				if (Info.ContainsKey(info.Action)) {
					throw new ActionDeserializationException();
				} else {					
					Info.Add(info.Action, new InputState() {KeyboardKey = info.Key, State = info.State, DeadKey = info.DeadKey});				
				}
			}
			foreach (MouseInputInfo info in inputs.MouseInputs)
			{
				if (Info.ContainsKey(info.Action)) {
					throw new ActionDeserializationException();
				} else {
					Info.Add(info.Action, new InputState() {MouseKey = info.Key, State = info.State, DeadKey = info.DeadKey});				
				}
			}
		}
	}
}

