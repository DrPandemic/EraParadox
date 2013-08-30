//
//  IEntityTest.cs
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

namespace GREATTests
{
    [TestFixture()]
    public class IEntityTest
    {
        [Test()]
        public void TestClone()
        {
			IEntity e = new IEntity(0, new Vec2(100f, 0f));
			IEntity clone = (IEntity)e.Clone();

			Assert.AreEqual(100f, e.Position.X, "original unchanged");
			Assert.AreEqual(100f, clone.Position.X, "clone has same values");

			e.Position.X = 2f;

			Assert.AreEqual(100f, clone.Position.X, "clone unchanged after modification or original");
        }
    }
}

