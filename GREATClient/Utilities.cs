//
//  Utilities.cs
//
//  Author:
//       parasithe <>
//
//  Copyright (c) 2013 parasithe
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
using Microsoft.Xna.Framework;

namespace GREATClient
{
    public static class Utilities
    {
		public static Random Random = new Random();

		public static float RandomFloat(this Random random, float min, float max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}

		/// <summary>
		/// Helper to quickly create Lists for DEBUGGING PURPOSES.
		/// Usage:
		/// List<string> list = Utilities.MakeList("hello", "this", "is", "a", "list");
		/// </summary>
		/// <returns>The list.</returns>
		/// <param name="elements">Elements.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> MakeList<T>(params T[] elements)
		{
			List<T> list = new List<T>();
			foreach (T e in elements) {
				list.Add(e);
			}
			return list;
		}

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

