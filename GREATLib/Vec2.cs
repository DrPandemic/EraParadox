//
//  MyClass.cs
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

namespace GREATLib
{
	/// <summary>
	/// A two-dimensional vector that can either represent a position or a vector.
	/// The rough equivalent of Vector2 from the framework.
	/// </summary>
    public class Vec2
    {
		/// <summary>
		/// A (1,1) vector.
		/// </summary>
		public static readonly Vec2 One = new Vec2(1f, 1f);
		/// <summary>
		/// A (1,0) vector.
		/// </summary>
		public static readonly Vec2 UnitX = new Vec2(1f, 0f);
		/// <summary>
		/// A (0,1) vector.
		/// </summary>
		public static readonly Vec2 UnitY = new Vec2(0f, 1f);
		/// <summary>
		/// A (0,0) vector.
		/// </summary>
		public static readonly Vec2 Zero = new Vec2(0f, 0f);

		/// <summary>
		/// Gets or sets the x coordinate.
		/// </summary>
		/// <value>The x coordinate.</value>
		public float X { get; set; }
		/// <summary>
		/// Gets or sets the y coordinate.
		/// </summary>
		/// <value>The y coordinate.</value>
		public float Y { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATLib.Vec2"/> class
		/// with the values (0,0).
		/// </summary>
        public Vec2()
			: this(0f, 0f)
        {
        }
		/// <summary>
		/// Initializes a new instance of the <see cref="GREATLib.Vec2"/> class
		/// with the values (x,y).
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public Vec2(float x, float y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// Returns a string that represents the position.
		/// </summary>
		/// <returns>A string that represents the position.</returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString()
		{
			return string.Format("({0}, {1})", X, Y);
		}

		/// <summary>Unary + operator.</summary>
		/// <param name="vec">The vector.</param>
		public static Vec2 operator+(Vec2 vec)
		{
			return vec;
		}
		/// <summary>Binary + operator.</summary>
		/// <param name="lhs">Left-hand side parameter.</param>
		/// <param name="rhs">Right-hand side parameter.</param>
		public static Vec2 operator+(Vec2 lhs, Vec2 rhs)
		{
			return new Vec2(lhs.X + rhs.X, lhs.Y + rhs.Y);
		}
		/// <summary>Unary - operator.</summary>
		/// <param name="vec">The vector.</param>
		public static Vec2 operator-(Vec2 vec)
		{
			return new Vec2(-vec.X, -vec.Y);
		}
		/// <summary>Binary - operator.</summary>
		/// <param name="lhs">Left-hand side parameter.</param>
		/// <param name="rhs">Right-hand side parameter.</param>
		public static Vec2 operator-(Vec2 lhs, Vec2 rhs)
		{
			return lhs + (-rhs);
		}
		/// <summary>Scalar multiplication.</summary>
		/// <param name="lhs">Left-hand side parameter.</param>
		/// <param name="k">The scalar.</param>
		public static Vec2 operator*(Vec2 lhs, float k)
		{
			return new Vec2(k * lhs.X, k * lhs.Y);
		}
		//// <summary>Scalar multiplication.</summary>
		/// <param name="k">The scalar.</param
		/// <param name="rhs">Right-hand side parameter.</param>
		public static Vec2 operator*(float k, Vec2 rhs)
		{
			return new Vec2(k * rhs.X, k * rhs.Y);
		}
		/// <summary>Scalar division.</summary>
		/// <param name="lhs">Left-hand side parameter.</param>
		/// <param name="k">The scalar.</param>
		public static Vec2 operator/(Vec2 lhs, float k)
		{
			return lhs * (1f / k);
		}
		/// <summary>
		/// Linear interpolation from a vector to another using a specified factor.
		/// </summary>
		/// <param name="start">Start vector.</param>
		/// <param name="goal">Goal vector.</param>
		/// <param name="factor">Factor of the linear interpolation.</param>
		public static Vec2 Lerp(Vec2 start, Vec2 goal, float factor)
		{
			return new Vec2(start.X + factor * (goal.X - start.X),
			                start.Y + factor * (goal.Y - start.Y));
		}
    }
}

