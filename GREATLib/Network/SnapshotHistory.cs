//
//  SnapshotHistory.cs
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
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GREATLib.Network
{
	/// <summary>
	/// Keeps an history of snapshots (keyframes, states, ...) for a certain amount of time.
	/// </summary>
    public class SnapshotHistory<TState>
    {
		TimeSpan MaxHistoryTime { get; set; }
		Queue<KeyValuePair<double, TState>> States { get; set; }

		public SnapshotHistory(TimeSpan maxHistoryTime)
		{
			MaxHistoryTime = maxHistoryTime;
			States = new Queue<KeyValuePair<double, TState>>();
		}

		/// <summary>
		/// Adds the snapshot to the history and cleans outdated snapshots.
		/// </summary>
		public void AddSnapshot(TState snapshot, double currentSeconds)
		{
			Debug.Assert(States != null);
			Debug.Assert(currentSeconds >= 0.0);
			Debug.Assert(IsEmpty() || States.Peek().Key <= currentSeconds); // at least the first one is older (can't check the others)

			CleanOutdated(currentSeconds);

			States.Enqueue(Utilities.MakePair(currentSeconds, snapshot));
		}

		/// <summary>
		/// Gets the closest snapshot to the given time. If the given time is older than our
		/// oldest snaphost, we just give our last one.
		/// </summary>
		public KeyValuePair<double, TState> GetClosestSnapshot(double time)
		{
			Debug.Assert(time >= 0.0);

			KeyValuePair<double, TState> state = States.Peek();

			foreach (KeyValuePair<double, TState> pair in States) {
				if (Math.Abs(pair.Key - time) < Math.Abs(state.Key - time)) { // take the closest snapshot
					state = pair;
				}
			}

			return state;
		}

		/// <summary>
		/// Determines whether the history is empty.
		/// </summary>
		public bool IsEmpty()
		{
			return States.Count == 0;
		}

		/// <summary>
		/// Removes the outdated snapshots.
		/// </summary>
		void CleanOutdated(double currentSeconds)
		{
			Debug.Assert(States != null);
			Debug.Assert(currentSeconds >= 0.0);

			if (!IsEmpty()) {
				double minTime = Math.Max(currentSeconds - MaxHistoryTime.TotalSeconds, 0.0); // keep a positive minimum time

				while (!IsEmpty() && States.Peek().Key < minTime) { // remove old states
					States.Dequeue();
				}
			}
		}
    }
}

