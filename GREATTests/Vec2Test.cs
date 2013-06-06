//
//  Test.cs
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
    public class Vec2Test
    {
		void TestCtor(Vec2 v, float x, float y, string ctor)
		{
			Assert.AreEqual(v.X, x, float.Epsilon, "X " + ctor);
			Assert.AreEqual(v.Y, y, float.Epsilon, "Y " + ctor);
		}
        [Test()]
        public void TestConstructors()
        {
			TestCtor(new Vec2(), 0f, 0f, "default ctor");
			TestCtor(Vec2.Zero, 0f, 0f, "Vec2.Zero");
			TestCtor(new Vec2(5f, 2f), 5f, 2f, "(5,2) ctor");
			TestCtor(new Vec2(-5f, 15f), -5f, 15f, "(-5,15) ctor");
			TestCtor(new Vec2(0f, -15f), 0f, -15f, "(0,-15) ctor");
			TestCtor(Vec2.One, 1f, 1f, "Vec2.One");
			TestCtor(Vec2.UnitX, 1f, 0f, "Vec2.UnitX");
			TestCtor(Vec2.UnitY, 0f, 1f, "Vec2.UnitY");
        }

		void TestLength(Vec2 v, float length, string msg, float eps = float.Epsilon)
		{
			Assert.AreEqual(v.GetLength(), length, eps, msg);
			Assert.AreEqual(v.GetLengthSquared(), length * length, eps, msg + " squared");
		}

		[Test()]
		public void TestLengths()
		{
			TestLength(new Vec2(1f, 0f), 1f, "|(1,0)| = 1");
			TestLength(new Vec2(0f, 1f), 1f, "|(0,1)| = 1");
			TestLength(new Vec2(3f, 4f), 5f, "|(3,4)| = 5");
			TestLength(new Vec2(4f, 3f), 5f, "|(4,3)| = 5");
			TestLength(new Vec2(1f, 2f), (float)Math.Sqrt(5.0), "|(1,2)| = sqrt(5)", 0.0001f);
		}
    }
}

