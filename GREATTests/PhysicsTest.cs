//
//  PhysicsTest.cs
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
using GREATLib.Entities;
using GREATLib;
using GREATLib.Physics;
using GREATLib.World;

namespace GREATTests
{
    [TestFixture()]
    public class PhysicsTest
    {
		const float PhysicsEpsilon = 0.0001f;

        [Test()]
        public void TestDirection()
        {
			GameWorld w = new GameWorld();
			PhysicsEngine p = new PhysicsEngine(w);

			IEntity e = new IEntity(0, new Vec2());
			Assert.AreEqual(e.Direction, HorizontalDirection.None, "default direction");

			p.Move(e, HorizontalDirection.Right);
			Assert.AreEqual(e.Direction, HorizontalDirection.Right, "right after movement");

			p.Move(e, HorizontalDirection.None);
			Assert.AreEqual(e.Direction, HorizontalDirection.Right, "still right after no movement");

			p.Move(e, HorizontalDirection.Right);
			Assert.AreEqual(e.Direction, HorizontalDirection.Right, "still right after double right movement");

			p.Move(e, HorizontalDirection.Left);
			Assert.AreEqual(e.Direction, HorizontalDirection.None, "none after cancelled movement");
        }
    }
}

