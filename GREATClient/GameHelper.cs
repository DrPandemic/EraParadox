//
//  GameHelper.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
//
//  Copyright (c) 2013 Jesse
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

namespace GREATClient
{
    public static class GameHelper
    {
		/// <summary>
		/// Utility function to easily get game services.
		/// Usage:
		/// InputManager input = game.GetService<InputManager>();
		/// </summary>
		/// <returns>The service.</returns>
		/// <param name="game">Game.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetService<T>(this Game game)
		{
			return (T)game.Services.GetService(typeof(T));
		}
    }
}

