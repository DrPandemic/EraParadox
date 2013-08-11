//
//  InputActions.cs
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

namespace GREATClient.BaseClass.Input
{
    public enum InputActions {
		None,
		Spell1,
		Spell2,
		Spell3,
		Spell4,
		Jump,
		GoLeft,
		GoRight
	}

	public enum KeyState {
		Up,
		Down,
		Pressed,
		Released
	}

	public enum DeadKeys {
		None,
		Control,
		Alt,
		Shift
	}
}

