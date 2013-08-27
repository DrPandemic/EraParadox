//
//  MatchTests.cs
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
using GREATLib.Network;
using GREATLib.Physics;
using GREATLib.World;
using GREATLib.Entities;
using GREATLib;

namespace GREATTests
{
    [TestFixture()]
    public class MatchTest
    {
		void TestClonedEntityValues(IEntity original, IEntity clone, bool equal, string message)
		{
			string eql_msg = equal ? " same " : " different ";
			string intro = "cloned value" + eql_msg;
			Action<object, object, string> assert;
			if (equal) {
				assert = Assert.AreEqual;
			} else {
				assert = Assert.AreNotEqual;
			}

			Assert.AreEqual(original.ID, clone.ID, intro + "ID " + message); // must stay the same

			assert(original.Position.X, clone.Position.X, intro + "position " + message);
			assert(original.Velocity.Y, clone.Velocity.Y, intro + "velocity " + message);
			assert(original.CollisionHeight, clone.CollisionHeight, intro + "collision height " + message);
			assert(original.CollisionWidth, clone.CollisionWidth, intro + "collision width " + message);
			assert(original.Direction, clone.Direction, intro + "direction " + message);
			assert(original.HorizontalAcceleration, clone.HorizontalAcceleration, intro + "horizontal acceleration " + message);
			assert(original.IsOnGround, clone.IsOnGround, intro + "is on ground " + message);
			assert(original.JumpForce, clone.JumpForce, intro + "jump force " + message);
			assert(original.MoveSpeed, clone.MoveSpeed, intro + "move speed " + message);
		}
		void ChangeClonedEntity(IEntity e, Vec2 pos, Vec2 vel, float colW, float colH, HorizontalDirection dir,
		                  float horiAccel, bool onGround, short jumpF, float moveSpeed)
		{
			e.Position.X = pos.X;
			e.Position.Y = pos.Y;
			e.Velocity.X = vel.X;
			e.Velocity.Y = vel.Y;
			e.CollisionWidth = colW;
			e.CollisionHeight = colH;
			e.Direction = dir;
			e.HorizontalAcceleration = horiAccel;
			e.IsOnGround = onGround;
			e.JumpForce = jumpF;
			e.MoveSpeed = moveSpeed;
		}

        [Test()]
        public void TestStateClone()
        {
			MatchState state = new MatchState(new PhysicsEngine(new GameWorld()));

			IEntity e = new IEntity(IDGenerator.GenerateID(), new Vec2(100f, -99f));
			state.AddEntity(e);

			Assert.True(state.ContainsEntity(e.ID), "entity1 added to match");

			IEntity e2 = new IEntity(IDGenerator.GenerateID(), new Vec2(42f, 24f));
			state.AddEntity(e2);

			Assert.True(state.ContainsEntity(e2.ID), "entity2 added to match");


			MatchState clone = state.Clone() as MatchState;

			Assert.True(clone.ContainsEntity(e.ID), "clone contains entity1");
			Assert.True(clone.ContainsEntity(e2.ID), "clone contains entity2");

			IEntity clonedE = clone.GetEntity(e.ID);
			TestClonedEntityValues(e, clonedE, true, "after clone (e1)");

			ChangeClonedEntity(clonedE, new Vec2(11f, 111f), new Vec2(22f, 222f), 1000f, 2000f, HorizontalDirection.Right,
			                   512f, false, 255, 333f);
			TestClonedEntityValues(e, clonedE, false, "after clone modif (e1)");

			IEntity clonedE2 = clone.GetEntity(e2.ID);
			TestClonedEntityValues(e2, clonedE2, true, "after clone (e2)");

			ChangeClonedEntity(clonedE2, new Vec2(87f, 78f), new Vec2(52f, 25f), 76f, 88f, HorizontalDirection.Left,
			                   3636f, false, 1212, 2121f);
			TestClonedEntityValues(e2, clonedE2, false, "after clone modif (e2)");
        }
    }
}

