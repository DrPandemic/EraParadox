//
//  GeneralHelper.cs
//
//  Author:
//       Jesse <${AuthorEmail}>
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
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATLib
{
	/// <summary>
	/// General helper functions.
	/// </summary>
    public static class GeneralHelper
    {
		/// <summary>
		/// Makes a list from the passed arguments, generally used for fast
		/// debugging.
		/// </summary>
		/// <returns>The list.</returns>
		/// <param name="elems">The elements.</param>
		public static List<T> MakeList<T>(params T[] elems)
		{
			List<T> l = new List<T>();
			foreach (T e in elems)
				l.Add(e);
			return l;
		}

		/// <summary>
		/// Clamp the specified value in the interval [min, max].
		/// It forces it to be between both.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="min">The minimum.</param>
		/// <param name="max">The maximum.</param>
		public static int Clamp(int value, int min, int max)
		{
			Debug.Assert(min <= max);
			return Math.Min(max, Math.Max(min, value));
		}
    }
}

