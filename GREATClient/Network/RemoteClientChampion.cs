//
//  RemoteClientChampion.cs
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
using GREATLib.Entities;
using GREATLib.Network;
using GREATLib;
using System.Collections.Generic;
using System.Diagnostics;
using GREATLib.Entities.Champions;

namespace GREATClient.Network
{
    public class RemoteClientChampion : ClientChampion
    {
		/// <summary>
		/// We stay about n cycles behind in the past in case we lose packets.
		/// </summary>
		static readonly TimeSpan LERP_TIME_IN_THE_PAST = TimeSpan.FromSeconds(GameMatch.STATE_UPDATE_INTERVAL.TotalSeconds * 2.0);
		/// <summary>
		/// We keep a bit more than our time used to lerp, just in case.
		/// </summary>
		static readonly TimeSpan HISTORY_TIME_KEPT = TimeSpan.FromSeconds(LERP_TIME_IN_THE_PAST.TotalSeconds * 10);
		static readonly float SMOOTH_LERP_FACTOR = 0.8f;
		/// <summary>
		/// The distance required between the simulated position and the drawn position
		/// that makes us snap directly to it (instead of interpolating to it, we just 
		/// directly set it).
		/// </summary>
		const float POSITION_DISTANCE_TO_SNAP = 50f;

		SnapshotHistory<StateUpdateData> StateHistory { get; set; }

		public Vec2 ServerPosition { get; set; }

        public RemoteClientChampion(ChampionSpawnInfo spawnInfo)
			: base(spawnInfo)
        {
			StateHistory = new SnapshotHistory<StateUpdateData>(HISTORY_TIME_KEPT);
        }

		public override void AuthoritativeChangePosition(StateUpdateData data, double time)
		{
			ServerPosition = data.Position;
			StateHistory.AddSnapshot(
				data,
				GetCurrentTimeSeconds());
		}

		public override void Update(Microsoft.Xna.Framework.GameTime dt)
		{
			base.Update(dt);

			InterpolateInThePast();
			SmoothTowardsInterpolatedPosition();
		}

		void SmoothTowardsInterpolatedPosition()
		{
			float distanceSq = Vec2.DistanceSquared(DrawnPosition, Position);
			if (distanceSq < POSITION_DISTANCE_TO_SNAP * POSITION_DISTANCE_TO_SNAP) {
				DrawnPosition = Vec2.Lerp(DrawnPosition, Position, SMOOTH_LERP_FACTOR);
			} else {
				ILogger.Log(String.Format("Snapping remote client position({0}) to simulated({1}). -> distance squared:{2}", DrawnPosition, Position, distanceSq), LogPriority.High);
				DrawnPosition = Position;
			}
		}

		void InterpolateInThePast()
		{
			double targetTime = GetCurrentTimeSeconds() - LERP_TIME_IN_THE_PAST.TotalSeconds;

			if (!StateHistory.IsEmpty()) {
				KeyValuePair<double, StateUpdateData> before = StateHistory.GetSnapshotBefore(targetTime);
				KeyValuePair<double, StateUpdateData>? after = StateHistory.GetNext(before);

				if (after.HasValue) {
					Debug.Assert(targetTime <= after.Value.Key);
					Debug.Assert(before.Key <= after.Value.Key);

					if (targetTime < before.Key) { // if we have a too small history
						ILogger.Log(String.Format("Remote history too small. Snapping time {0} to {1}", targetTime, before.Key),
						            LogPriority.High);
						targetTime = before.Key;
					}

					double progress = (targetTime - before.Key) / (after.Value.Key - before.Key);
					Debug.Assert(0.0 - double.Epsilon <= progress && progress <= 1.0 + double.Epsilon);

					Position = Vec2.Lerp(before.Value.Position, after.Value.Value.Position, (float)progress);
					// Take animation of closest state
					var closestState = Math.Abs(targetTime - before.Key) > Math.Abs(targetTime - after.Value.Key) ? before.Value : after.Value.Value;
					Animation = closestState.Animation;
					FacingLeft = closestState.FacingLeft;
				} else {
					Position = before.Value.Position;
					//TODO: extrapolation here
				}
			}
		}

		double GetCurrentTimeSeconds()
		{
			return Client.Instance.GetTime().TotalSeconds;
		}
    }
}

