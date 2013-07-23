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
using System.Timers;

namespace GREATClient
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
		KeyboardState NewKeyboard { get; set; }

        public InputManager()
        {
			Inputs = new Inputs();

        }

		public void OnUpdate()
		{
			NewKeyboard = Keyboard.GetState();
		}
    }
}

