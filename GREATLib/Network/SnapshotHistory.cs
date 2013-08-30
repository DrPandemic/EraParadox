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
	/// The snapshot could not be found in the history.
	/// </summary>
	public class SnapshotNotInHistoryException : Exception { }

	/// <summary>
	/// No snapshot could be given since the history is empty.
	/// </summary>
	public class EmptyHistoryException : Exception { }

	/// <summary>
	/// Keeps an history of snapshots (keyframes, states, ...) for a certain amount of time.
	/// </summary>
    public class SnapshotHistory<TState>
    {
		TimeSpan MaxHistoryTime { get; set; }
		List<KeyValuePair<double, TState>> States { get; set; }

		public SnapshotHistory(TimeSpan maxHistoryTime)
		{
			MaxHistoryTime = maxHistoryTime;
			States = new List<KeyValuePair<double, TState>>();
		}

		/// <summary>
		/// Adds the snapshot to the history and cleans outdated snapshots.
		/// </summary>
		public void AddSnapshot(TState snapshot, double currentSeconds)
		{
			Debug.Assert(States != null);
			Debug.Assert(currentSeconds >= 0.0);

			CleanOutdated(currentSeconds);

			// Find where our state should go (ordered by time)
			int i = States.FindLastIndex(pair => pair.Key < currentSeconds) + 1;
			States.Insert(i, Utilities.MakePair(currentSeconds, snapshot));

			Debug.Assert(IsHistorySorted());
		}

		bool IsHistorySorted()
		{
			for (int i = 0; i < States.Count - 1; ++i) {
				if (States[i].Key - States[i+1].Key > float.Epsilon) {
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Gets the closest snapshot to the given time. If the given time is older than our
		/// oldest snaphost, we just give our last one.
		/// </summary>
		public KeyValuePair<double, TState> GetClosestSnapshot(double time)
		{
			Debug.Assert(time >= 0.0);

			KeyValuePair<double, TState> state = States[0];

			foreach (KeyValuePair<double, TState> pair in States) {
				if (Math.Abs(pair.Key - time) < Math.Abs(state.Key - time)) { // take the closest snapshot
					state = pair;
				}
			}

			return state;
		}

		/// <summary>
		/// Gets the last snapshot, if it exists, of the history.
		/// </summary>
		/// <returns>The last.</returns>
		public KeyValuePair<double, TState> GetLast()
		{
			if (States.Count == 0) {
				throw new EmptyHistoryException();
			}

			return States[States.Count - 1];
		}

		/// <summary>
		/// Gets the next snapshot after the one provided. If there are no other snapshots, returns null.
		/// </summary>
		public KeyValuePair<double, TState>? GetNext(KeyValuePair<double, TState> snapshot)
		{
			if (!States.Contains(snapshot)) {
				throw new SnapshotNotInHistoryException();
			}

			Debug.Assert(States.FindAll((s) => IsSameSnapshot(s, snapshot)).Count == 1); // only there once

			int index = States.FindIndex((s) => IsSameSnapshot(s, snapshot));

			Debug.Assert(index >= 0);

			++index; // go to the next element in history

			return index < States.Count ? States[index] : new KeyValuePair<double, TState>?();
		}

		bool IsSameSnapshot(KeyValuePair<double, TState> s1,
		                    KeyValuePair<double, TState> s2)
		{
			return s1.Key == s2.Key &&
				s1.Value.Equals(s2.Value);
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

				while (!IsEmpty() && States[0].Key < minTime) { // remove old states
					States.RemoveAt(0);
				}
			}
		}
    }
}

