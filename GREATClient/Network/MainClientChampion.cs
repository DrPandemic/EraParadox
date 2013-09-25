//
//  MainChampion.cs
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
using GREATLib;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using GREATLib.Entities;
using GREATLib.Network;
using System.Diagnostics;
using System;
using GREATClient.BaseClass;

namespace GREATClient.Network
{
	/// <summary>
	/// Represents the champion's data of the main player, the one playing the game on
	/// this instance of the program.
	/// </summary>
    public sealed class MainClientChampion : ClientChampion
    {
		static readonly TimeSpan HISTORY_KEPT_TIME = TimeSpan.FromSeconds(1.0);
		/// <summary>
		/// The distance required between the simulated position and the drawn position
		/// that makes us snap directly to it (instead of interpolating to it, we just 
		/// directly set it).
		/// </summary>
		const float POSITION_DISTANCE_TO_SNAP = 50f;
		/// <summary>
		/// The distance required between the simulated position and the drawn position
		/// that makes us lerp towards it. If we're very close to the server position,
		/// we do not want to move or it feels choppy.
		/// </summary>
		const float MIN_POSITION_DISTANCE_TO_LERP = 0f;

		/// <summary>
		/// Gets or sets the server position.
		/// This is the last position that we received from the server.
		/// </summary>
		/// <remarks>This shouldn't be changed within the class, only set when the server
		/// gives us a new position.</remarks>
		public Vec2 ServerPosition { get; private set; }
		/// <summary>
		/// TODO: TOREMOVE		
		/// </summary>
		/// <value>The position before correction.</value>
		public Vec2 PositionBeforeCorrection { get; private set; }

		Vec2 PositionBeforeLerp { get; set; }
		double TimeSinceLastServerUpdate { get; set; }
		uint LastAcknowledgedActionID { get; set; }

		GameMatch Match { get; set; }

		Queue<PlayerAction> PackagedActions { get; set; }
		SnapshotHistory<PlayerAction> ActionsHistory { get; set; }
		SnapshotHistory<IEntity> History { get; set; }

		public MainClientChampion(ChampionSpawnInfo spawnInfo, GameMatch match)
			: base(spawnInfo)
        {
			Match = match;

			ServerPosition = Position;

			PackagedActions = new Queue<PlayerAction>();
			History = new SnapshotHistory<IEntity>(HISTORY_KEPT_TIME);
			ActionsHistory = new SnapshotHistory<PlayerAction>(HISTORY_KEPT_TIME);

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;

			PositionBeforeCorrection = Position;

			LastAcknowledgedActionID = IDGenerator.NO_ID;
        }

		/// <summary>
		/// Update the champion, applying client-side prediction.
		/// </summary>
		public override void Update(GameTime deltaTime)
		{
			// client-side prediction
			Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds);

			LerpTowardsServerPosition(deltaTime.ElapsedGameTime.TotalSeconds);

			History.AddSnapshot((IEntity)this.Clone(), Client.Instance.GetTime().TotalSeconds);
		}

		void LerpTowardsServerPosition(double deltaSeconds)
		{
			float distanceSq = Vec2.DistanceSquared(Position, DrawnPosition);

			if (distanceSq >= POSITION_DISTANCE_TO_SNAP * POSITION_DISTANCE_TO_SNAP) { // if we must snap directly to the simulated position
				ILogger.Log(String.Format("Snapping position({0}) to simulated({1}). -> distance squared:{2}", DrawnPosition, Position, distanceSq), LogPriority.High);
				DrawnPosition = Position;
				TimeSinceLastServerUpdate = 0.0;
				PositionBeforeLerp = DrawnPosition;
			} else { // If we must interpolate our position (we're not too far)
				Debug.Assert(TimeSinceLastServerUpdate >= 0.0);

				TimeSinceLastServerUpdate += deltaSeconds;
				double progress = TimeSinceLastServerUpdate / GameMatch.STATE_UPDATE_INTERVAL.TotalSeconds;
				progress = Math.Min(1.0, progress);

				DrawnPosition = Vec2.Lerp(PositionBeforeLerp, Position, (float)progress);

			}
		}

		/// <summary>
		/// Take the new position given by the server and resimulate our unacknowledged actions
		/// from there.
		/// </summary>
		public override void AuthoritativeChangePosition(Vec2 position, double time)
		{
			ServerPosition = position;

			if (!History.IsEmpty()) {
				ResimulateAfterCorrection(time);
			} else {
				Position = (Vec2)ServerPosition.Clone();
			}

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;
		}

		void ResimulateAfterCorrection(double time)
		{
			var snapshot = History.GetSnapshotBefore(time);
			Clone(snapshot.Value); // go back to our previous state

			// simulate to our current time from our previous snapshot
			// --S--A--T------S------S----Now
			//   |----->
			// we simulate from our previous S (state) to current T (time)
			Simulate(snapshot, time);

			// take the server's position
			PositionBeforeCorrection = (Vec2)Position.Clone();
			//ILogger.Log(String.Format("{0} -> {1} <- {2}", Position, ServerPosition, snapshot.Value.Position));
			ILogger.Log(String.Format("{0} , s:{1}, c:{2}    ....     sp:{3}    cp:{4}", (ServerPosition - PositionBeforeCorrection),
			                          LastAcknowledgedActionID,
			                          ActionsHistory.IsEmpty() ? IDGenerator.NO_ID : ActionsHistory.GetLast().Value.ID,
			            			  ServerPosition,
			            			  PositionBeforeCorrection));
			Position = (Vec2)ServerPosition.Clone();
			snapshot = History.AddSnapshot((IEntity)this.Clone(), time);

			// resimulate up until now
			double now = Math.Max(Client.Instance.GetTime().TotalSeconds,
			                      !ActionsHistory.IsEmpty() ? ActionsHistory.GetLast().Key : 0.0); // due to time inaccuracies, some actions may be "in the future", but we want to replay them
			Simulate(snapshot, now);
		}

		public override void SetLastAcknowledgedActionID(uint id)
		{
			base.SetLastAcknowledgedActionID(id);
			LastAcknowledgedActionID = id;
		}

		/// <summary>
		/// possibilities:
		/// 1. Our next step is an other state: we just simulate up until there
		/// --S--T------S-----S----Now
		/// |------>
		/// 
		/// 2. There are actions before our next state: we redo them
		/// --S--T-A-A--S-----S----Now
		/// |-#-#-->
		/// 
		/// 3. We are at our last state: we simulate until now
		/// --S---------S-----S-T--Now
		/// |-->
		/// 
		/// Therefore, we want to simulate until our next target time (which can be the next state or "now"),
		/// redoing any actions along the way.
		/// </summary>
		void Simulate(KeyValuePair<double, IEntity> snapshot, double end)
		{
			double deltaT;
			double time = snapshot.Key;

			while (time < end) {
				var next = History.GetNext(snapshot);
				double target = next.HasValue ? next.Value.Key : end;

				// Redo our actions along the way to our next state
				RedoActions(ref time, target);

				// Simulate to our next state
				deltaT = target - time;
				if (deltaT > 0.0) {
					Match.CurrentState.ApplyPhysicsUpdate(ID, deltaT);
				}

				// Store our resimulated state in our next state and move to the next one
				if (next.HasValue) {
					next.Value.Value.Clone(this);
					snapshot = next.Value;
				}
				time = target;
			}
		}

		void SkipOldActions(ref KeyValuePair<double, PlayerAction>? action, double time)
		{
			// We get our current action to represent our first action right after our current state:
			// --A----S---A---A--- ...
			//            | we want this action, the first right after our state
			while (action.HasValue &&
			       action.Value.Key < time) {
				action = ActionsHistory.GetNext(action.Value);
			}
		}

		void RedoActions(ref double time, double target)
		{
			KeyValuePair<double, PlayerAction>? action = !ActionsHistory.IsEmpty() ? ActionsHistory.GetSnapshotBefore(time) : new KeyValuePair<double,PlayerAction>?();

			SkipOldActions(ref action, time);

			// If we really have actions to redo, redo them until the next state
			while (action.HasValue &&
			       action.Value.Key > time &&
			       action.Value.Key < target &&
			       action.Value.Value.ID <= LastAcknowledgedActionID) {
				// simulate the time before our action:
				// --S---A--------...
				//   |--->
				// or
				// --S---A-----A--...
				//       |----->
				// depending on what is before us
				var deltaT = action.Value.Key - time;
				if (deltaT > 0.0) {
					Match.CurrentState.ApplyPhysicsUpdate(ID, deltaT);
				}

				// redo the action
				ExecuteAction(action.Value.Value.Type);

				// move to our next action
				time = action.Value.Key;
				action = ActionsHistory.GetNext(action.Value);
			}
		}

		public void PackageAction(PlayerAction action)
		{
			PackagedActions.Enqueue(action);

			ActionsHistory.AddSnapshot(action, action.Time);

			ExecuteAction(action.Type);
		}

		void ExecuteAction(PlayerActionType type)
		{
			switch (type) {
				case PlayerActionType.MoveLeft: 
					Match.CurrentState.Move(ID, HorizontalDirection.Left);
					break;

					case PlayerActionType.MoveRight:
					Match.CurrentState.Move(ID, HorizontalDirection.Right);
					break;

					case PlayerActionType.Jump:
					Match.CurrentState.Jump(ID);
					break;

					default:
					Debug.Fail(String.Format("Invalid action type \"{0}\"", type));
					break;
			}
		}

		public Queue<PlayerAction> GetActionPackage()
		{
			return PackagedActions;
		}
    }
}

