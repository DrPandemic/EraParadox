//
//  SnapshotHistoryTest.cs
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

namespace GREATTests
{
    [TestFixture()]
    public class SnapshotHistoryTest
    {
		[Test()]
		public void TestEmpty()
		{
			SnapshotHistory<int> h = new SnapshotHistory<int>(TimeSpan.FromMilliseconds(1000.0));
			Assert.True(h.IsEmpty());
			h.AddSnapshot(0, 0.0);
			Assert.False(h.IsEmpty());
		}

		[Test()]
		public void TestAddSnapshot()
		{
			SnapshotHistory<double> h = new SnapshotHistory<double>(TimeSpan.FromSeconds(1000.0));
			h.AddSnapshot(0.0, 0.0);
			h.AddSnapshot(1.0, 1.0);
			h.AddSnapshot(2.0, 2.0);
			h.AddSnapshot(1.5, 1.5);
			h.AddSnapshot(3.0, 3.0);
			h.AddSnapshot(0.5, 0.5);
			Assert.AreEqual(2.0, h.GetClosestSnapshot(2.1).Key, "select snapshot from added");
		}

		[Test()]
		public void TestDuplicates()
		{
			SnapshotHistory<double> h = new SnapshotHistory<double>(TimeSpan.FromSeconds(1000.0));
			h.AddSnapshot(0.0, 0.0);
			h.AddSnapshot(1.0, 1.0);
			h.AddSnapshot(2.0, 2.0);

			Assert.AreEqual(1.0, h.GetNext(h.GetSnapshotBefore(0.0)).Value.Value, "next to first is second with 1.0");

			h.AddSnapshot(5.0, 1.0);

			Assert.AreEqual(5.0, h.GetNext(h.GetSnapshotBefore(0.0)).Value.Value, "next to first is second with 5.0 (after we added a duplicate)");

			Assert.AreEqual(2.0, h.GetNext(h.GetNext(h.GetSnapshotBefore(0.0)).Value).Value.Value, "last is still 2.0 (didn't keep the 1.0,1.0)");
		}

		[Test()]
		public void TestBefore()
		{
			SnapshotHistory<int> h = new SnapshotHistory<int>(TimeSpan.FromSeconds(1000.0));

			h.AddSnapshot(1, 1.0);

			Assert.AreEqual(1, h.GetSnapshotBefore(2.0).Value, "first element with one element(1.0) and t=2");
			Assert.AreEqual(1, h.GetSnapshotBefore(1.0).Value, "first element with one element(1.0) and t=1");
			Assert.AreEqual(1, h.GetSnapshotBefore(0.0).Value, "gives first element when given time before the first snapshot");

			h.AddSnapshot(2, 2.0);

			Assert.AreEqual(2, h.GetSnapshotBefore(5.0).Value, "second element before with 2 elements(1.0,2.0) and t=5");
			Assert.AreEqual(2, h.GetSnapshotBefore(2.0).Value, "second element before with 2 elements(1.0,2.0) and t=2");
			Assert.AreEqual(1, h.GetSnapshotBefore(1.0).Value, "first element before with 2 elements(1.0,2.0) and t=1");
			Assert.AreEqual(1, h.GetSnapshotBefore(0.0).Value, "gives first element when given time before the first snapshot (2 elements)");

			h.AddSnapshot(5, 5.0);

			Assert.AreEqual(5, h.GetSnapshotBefore(7.0).Value, "third element before with 3 elements(1.0,2.0,5.0) and t=7");
			Assert.AreEqual(5, h.GetSnapshotBefore(5.0).Value, "third element before with 3 elements(1.0,2.0,5.0) and t=5");
			Assert.AreEqual(2, h.GetSnapshotBefore(2.0).Value, "second element before with 3 elements(1.0,2.0,5.0) and t=2");
			Assert.AreEqual(1, h.GetSnapshotBefore(1.5).Value, "first element before with 3 elements(1.0,2.0,5.0) and t=1");
			Assert.AreEqual(1, h.GetSnapshotBefore(1.0).Value, "first element before with 3 elements(1.0,2.0,5.0) and t=1.5");
			Assert.AreEqual(1, h.GetSnapshotBefore(0.0).Value, "gives first element when given time before the first snapshot (3 elements)");
		}

		[Test()]
		public void TestClosest()
		{
			SnapshotHistory<int> h = new SnapshotHistory<int>(TimeSpan.FromSeconds(1000.0));

			h.AddSnapshot(5, 15.0);

			Assert.AreEqual(5, h.GetClosestSnapshot(15.0).Value, "1 elem and t=same");
			Assert.AreEqual(5, h.GetClosestSnapshot(1000.0).Value, "1 elem and t=bigger"); 
			Assert.AreEqual(15.0, h.GetClosestSnapshot(0.0).Key, "1 elem and t=smaller, check for time");

			h.AddSnapshot(7, 26.0);

			Assert.AreEqual(5, h.GetClosestSnapshot(15.0).Value, "elem 1 and t=same as first and 2elems");
			Assert.AreEqual(5, h.GetClosestSnapshot(20.0).Value, "elem 1 and t=closer to first and 2elems");
			Assert.AreEqual(7, h.GetClosestSnapshot(26.0).Value, "elem 2 and t=same as second and 2elems");
			Assert.AreEqual(7, h.GetClosestSnapshot(22.0).Value, "elem 2 and t=closer to second and 2elems");
			Assert.AreEqual(7, h.GetClosestSnapshot(40.0).Value, "elem 2 and t=closer to second (higher) and 2elems");

			h.AddSnapshot(9, 50.0);

			Assert.AreEqual(5, h.GetClosestSnapshot(15.0).Value, "elem 1 and t=same as first and 3elems");
			Assert.AreEqual(5, h.GetClosestSnapshot(20.0).Value, "elem 1 and t=closer to first and 3elems");
			Assert.AreEqual(7, h.GetClosestSnapshot(26.0).Value, "elem 2 and t=same as second and 3elems");
			Assert.AreEqual(7, h.GetClosestSnapshot(22.0).Value, "elem 2 and t=closer to second and 3elems");
			Assert.AreEqual(9, h.GetClosestSnapshot(50.0).Value, "elem 2 and t=same as third and 3elems");
			Assert.AreEqual(9, h.GetClosestSnapshot(40.0).Value, "elem 2 and t=closer to third and 3elems");
		}

        [Test()]
        public void TestCleanup()
        {
			SnapshotHistory<int> h = new SnapshotHistory<int>(TimeSpan.FromMilliseconds(1000.0));

			h.AddSnapshot(1, 0.0);

			Assert.AreEqual(1, h.GetClosestSnapshot(0.0).Value, "1 elem t=0");

			h.AddSnapshot(2, 1.1);

			Assert.AreEqual(2, h.GetClosestSnapshot(0.0).Value, "elem 1 cleaned t=1001");
        }

		[Test()]
		public void TestGetNext()
		{
			SnapshotHistory<int> h = new SnapshotHistory<int>(TimeSpan.FromSeconds(1000.0));

			h.AddSnapshot(1, 0.0);

			h.AddSnapshot(2, 1.0);

			h.AddSnapshot(3, 2.0);

			var s = h.GetClosestSnapshot(0.0);
			Assert.AreEqual(1, s.Value, "get first with closest");
			s = h.GetNext(s).Value;
			Assert.AreEqual(2, s.Value, "next to first is second");
			s = h.GetNext(s).Value;
			Assert.AreEqual(3, s.Value, "next to second is third");
			Assert.False(h.GetNext(s).HasValue, "next to last is null");

			Assert.AreEqual(3, h.GetNext(h.GetClosestSnapshot(1.0)).Value.Value, "next to closest to second is third");
		}
    }
}

