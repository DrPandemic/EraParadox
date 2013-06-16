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
		const float BigEpsilon = 0.0001f;

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

		[Test()]
		public void TestEquals()
		{
			Assert.AreEqual(Vec2.UnitX, new Vec2(1f, 0f), "(1,0) == (1,0)");
			Assert.AreEqual(Vec2.UnitY, new Vec2(0f, 1f), "(0,1) == (0,1)");
			Assert.AreNotEqual(Vec2.UnitX, Vec2.UnitY, "(1,0) != (0,1)");
			Assert.AreNotEqual(Vec2.UnitX, new Vec2(5f, 0f), "(1,0) != (5,0)");
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
			TestLength(new Vec2(1f, 2f), (float)Math.Sqrt(5.0), "|(1,2)| = sqrt(5)", BigEpsilon);
		}

		public void TestDistance(Vec2 a, Vec2 b, float dist, string msg, float eps = float.Epsilon)
		{
			Assert.AreEqual(Vec2.Distance(a, b), dist, eps, msg);
			Assert.AreEqual(Vec2.DistanceSquared(a, b), dist * dist, eps, msg + " squared");
		}

		[Test()]
		public void TestDistances()
		{
			TestDistance(Vec2.Zero, Vec2.Zero, 0f, "d((0,0), (0,0)) = 0");
			TestDistance(Vec2.Zero, Vec2.UnitX, 1f, "d((0,0), (1,0)) = 1");
			TestDistance(Vec2.UnitY, Vec2.Zero, 1f, "d((0,1), (0,0)) = 1");
			TestDistance(new Vec2(1f, 5f), new Vec2(3f, 3f), (float)Math.Sqrt(8.0), "d((1,5), (3,3)) = sqrt(8)", BigEpsilon);
		}

		[Test()]
		public void TestNormalize()
		{
			Assert.AreEqual(Vec2.Normalize(Vec2.UnitX), Vec2.UnitX, "normalize(1,0) = (1,0)");
			Assert.AreEqual(Vec2.Normalize(new Vec2(50f, 0f)), Vec2.UnitX, "normalize(50,0) = (1,0)");
			Assert.AreEqual(Vec2.Normalize(Vec2.UnitY), Vec2.UnitY, "normalize(0,1) = (0,1)");
			Assert.AreEqual(Vec2.Normalize(new Vec2(0f, 50f)), Vec2.UnitY, "normalize(0,50) = (0,1)");

			Assert.AreEqual(Vec2.Normalize(Vec2.Zero), Vec2.Zero, "normalize(0,0) = (0,0)");

			Assert.AreEqual(Vec2.Normalize(new Vec2(3f, 4f)), new Vec2(3f/5f, 4f / 5f), "normalize(3,4) = (3/5, 4/5)");
		}

		[Test()]
		public void TestNegativity()
		{
			Assert.AreEqual(-Vec2.One, new Vec2(-1f, -1f), "-(1,1) = (-1,-1)");
			Assert.AreEqual(-Vec2.UnitX, new Vec2(-1f, 0f), "-(1,0) = (-1, 0)");
			Assert.AreEqual(-Vec2.UnitY, new Vec2(0f, -1f), "-(0,1) = (0,-1)");
			Assert.AreEqual(-new Vec2(-1f, -1f), Vec2.One, "-(-1,-1) = (1,1)");
		}

		[Test()]
		public void TestAddition()
		{
			Assert.AreEqual(Vec2.UnitX + Vec2.UnitY, Vec2.One, "(1,0)+(0,1) = (1,1)");
			Assert.AreEqual(new Vec2(5f, 2f) + new Vec2(4f, 3f), new Vec2(9f, 5f), "(5,2)+(4,3) = (9,5)");
			Assert.AreEqual(new Vec2(-4f, 7f) + new Vec2(3f, -40f), new Vec2(-1f, -33f), "(-4,7)+(3,-40) = (-1,-33)");
			Assert.AreEqual(new Vec2(0f, 30f) + Vec2.Zero, new Vec2(0, 30f), "(0,30)+(0,0) = (0,30)");
			Assert.AreEqual(Vec2.Zero + Vec2.Zero, Vec2.Zero, "(0,0)+(0,0) = (0,0)");
		}

		[Test()]
		public void TestSubtraction()
		{
			Assert.AreEqual(Vec2.UnitX - Vec2.UnitX, Vec2.Zero, "(1,0)-(1,0) = (0,0)");
			Assert.AreEqual(Vec2.UnitY - Vec2.UnitY, Vec2.Zero, "(0,1)-(0,1) = (0,0)");
			Assert.AreEqual(-Vec2.One - Vec2.One, new Vec2(-2f, -2f), "-(1,1)-(1,1) = (-2,-2)");
			Assert.AreEqual(new Vec2(30f, 12f) - new Vec2(15f, -3f), new Vec2(15f, 15f), "(30,12)-(15,-3) = (15,15)");
			Assert.AreEqual(Vec2.Zero - Vec2.Zero, Vec2.Zero, "(0,0)-(0,0) = (0,0)");
		}

		[Test()]
		public void TestMultiplication()
		{
			Assert.AreEqual(Vec2.UnitX * 1f, Vec2.UnitX, "(1,0)*1 = (1,0)");
			Assert.AreEqual(1f * Vec2.UnitY, Vec2.UnitY, "1*(0,1) = (0,1)");
			Assert.AreEqual(new Vec2(5f, 2f) * 5f, new Vec2(25f, 10f), "(5,2)*5 = (25,10)");
			Assert.AreEqual(10f * new Vec2(15f, 30f), new Vec2(150f, 300f), "10*(15,30) = (150,300)");
			Assert.AreEqual(Vec2.Zero * 100000f, Vec2.Zero, "(0,0)*100000 = (0,0)");
			Assert.AreEqual(16 * Vec2.Zero, Vec2.Zero, "16*(0,0) = (0,0)");
		}

		[Test()]
		public void TestDivision()
		{
			Assert.AreEqual(Vec2.UnitX / 3f, new Vec2(1/3f, 0f), "(1,0)/3 = (1/3,0)");
			Assert.AreEqual(Vec2.One / 5f, new Vec2(1/5f, 1 / 5f), "(1,1)/5 = (1/5,1/5)");
			Assert.AreEqual(Vec2.Zero / 300000f, Vec2.Zero, "(0,0)/300000 = (0,0)");
		}

		[Test()]
		public void TestLerp()
		{
			Assert.AreEqual(Vec2.Lerp(Vec2.Zero, Vec2.UnitX, 0.5f), Vec2.UnitX * 0.5f, "lerp((0,0), (1,0), 0.5) = (0.5,0)");
			Assert.AreEqual(Vec2.Lerp(new Vec2(100f, 100f), new Vec2(500f, 500f), 0.25f), new Vec2(200f, 200f), "lerp((100,100), (500,500), 0.25) = (200,200)");
			Assert.AreEqual(Vec2.Lerp(Vec2.Zero, Vec2.Zero, 0.9f), Vec2.Zero, "lerp((0,0), (0,0), 0.9) = (0,0)");
			Assert.AreEqual(Vec2.Lerp(new Vec2(500f, 500f), new Vec2(100f, 100f), 0.25f), new Vec2(400f, 400f), "lerp((500,500), (100,100), 0.25) = (400,400)");
		}
    }
}

