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
    public static class Utilities
    {
		public static Random Random = new Random();

		/// <summary>
		/// Returns a random float number inside the range [min, max[.
		/// </summary>
		public static float RandomFloat(this Random random, float min, float max)
		{
			return (float)random.NextDouble() * (max - min) + min;
		}
		public static T RandomEnumValue<T>(this Random random)
		{
			var vals = Enum.GetValues(typeof(T));
			return (T)vals.GetValue(random.Next(vals.Length));
		}

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
		/// Makes a pair out of the two given values, deducing the type (less to type and independant of types).
		/// </summary>
		public static KeyValuePair<T, V> MakePair<T,V>(T key, V value)
		{
			return new KeyValuePair<T,V>(key, value);
		}

		public static bool InRange(Vec2 p, Vec2 q, float dist)
		{
			return Vec2.DistanceSquared(p, q) <= dist * dist;
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

		/// <summary>
		/// Gets the intersection depth between two rectangles, telling how the first one
		/// is inside the other one and in which direction we must push to fix the collision.
		/// </summary>
		/// <returns>The intersection depth.</returns>
		public static Vec2 GetIntersectionDepth(Rect a, Rect b)
		{
			// Calculate half sizes.
			float halfW_a = a.Width / 2f;
			float halfH_a = a.Height / 2f;
			float halfW_b = b.Width / 2f;
			float halfH_b = b.Height / 2f;

			// Calculate centers.
			Vec2 center_a = new Vec2(a.Left + halfW_a, a.Top + halfH_a);
			Vec2 center_b = new Vec2(b.Left + halfW_b, b.Top + halfH_b);

			// Calculate current and minimum non-intersecting distances between centers.
			float distance_x = center_a.X - center_b.X;
			float distance_y = center_a.Y - center_b.Y;
			float min_distance_x = halfW_a + halfW_b;
			float min_distance_y = halfH_a + halfH_b;

			// If we're not intersecting at all, return (0,0).
			if (Math.Abs(distance_x) >= min_distance_x || Math.Abs(distance_y) >= min_distance_y)
				return Vec2.Zero;

			// Calculate and return intersection depths.
			float depth_x = distance_x > 0 ? min_distance_x - distance_x : -min_distance_x - distance_x;
			float depth_y = distance_y > 0 ? min_distance_y - distance_y : -min_distance_y - distance_y;
			return new Vec2(depth_x, depth_y);
		}
    }
}

