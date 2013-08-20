//
//  RectTests.cs
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
using NUnit.Framework;
using System;
using GREATLib;

namespace GREATTests
{
    [TestFixture()]
    public class RectTest
    {
		void TestCtor(Rect r, float x, float y, float width, float height, string message)
		{
			Assert.AreEqual(x, r.X, message + " x");
			Assert.AreEqual(y, r.Y, message + " y");
			Assert.AreEqual(width, r.Width, message + " width");
			Assert.AreEqual(height, r.Height, message + " height");
		}

        [Test()]
        public void TestConstructors()
        {
			TestCtor(new Rect(), 0f, 0f, 0f, 0f, "default ctor");
			TestCtor(new Rect(5f, 10f, 15f, 20f), 5f, 10f, 15f, 20f, "ctor with data");
        }

		[Test()]
		public void TestEquality()
		{
			Assert.AreEqual(new Rect(), new Rect(0f, 0f, 0f, 0f), "empty rect equality");
			Assert.AreNotEqual(new Rect(), new Rect(1f, 1f, 1f, 1f), "empty rect inequality with rect with data");
		}

		void TestRectSides(Rect r, float left, float right, float top, float bottom, string message)
		{
			Assert.AreEqual(left, r.Left, message + " left");
			Assert.AreEqual(right, r.Right, message + " right");
			Assert.AreEqual(top, r.Top, message + " top");
			Assert.AreEqual(bottom, r.Bottom, message + " bottom");
		}
		[Test()]
		public void TestSides()
		{
			TestRectSides(new Rect(), 0f, 0f, 0f, 0f, "all empty sides rect");
			TestRectSides(new Rect(5f, 10f, 20f, 50f), 5f, 25f, 10f, 60f, "rect with data #1");
			TestRectSides(new Rect(0f, 15f, 1f, 1f), 0f, 1f, 15f, 16f, "rect with data #2");
		}
    }
}

