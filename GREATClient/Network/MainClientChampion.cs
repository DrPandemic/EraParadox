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
		class AcknowledgeInfo
		{
			public double Time { get; private set; }
			public Vec2 Position { get; private set; }
			public Vec2 Velocity { get; private set; }
			public uint LastAcknowledgedActionId { get; private set; }

			public AcknowledgeInfo(double time, Vec2 pos, Vec2 vel, uint lastAckId)
			{
				Time = time;
				Position = pos;
				Velocity = vel;
				LastAcknowledgedActionId = lastAckId;
			}
		}

		/// <summary>
		/// The distance required between the simulated position and the drawn position
		/// that makes us snap directly to it (instead of interpolating to it, we just 
		/// directly set it).
		/// </summary>
		const float POSITION_DISTANCE_TO_SNAP = 100f;
		/// <summary>
		/// The distance required between the simulated position and the drawn position
		/// that makes us lerp towards it. If we're very close to the server position,
		/// we do not want to move or it feels choppy.
		/// </summary>
		const float MIN_POSITION_DISTANCE_TO_LERP = 0f;
        static readonly TimeSpan CORRECTIONS_TIME_KEPT = TimeSpan.FromSeconds(1.0);

		/// <summary>
		/// Gets or sets the server position.
		/// This is the last position that we received from the server.
		/// </summary>
		/// <remarks>This shouldn't be changed within the class, only set when the server
		/// gives us a new position.</remarks>
		public Vec2 ServerPosition { get; private set; }
		Vec2 ServerVelocity { get; set; }

		Vec2 PositionBeforeLerp { get; set; }
		double TimeSinceLastServerUpdate { get; set; }

		GameMatch Match { get; set; }

		Queue<PlayerAction> PackagedActions { get; set; }
		List<PlayerAction> RecentActions { get; set; }
		List<AcknowledgeInfo> Corrections { get; set; }
		bool Corrected { get; set; }

		IEntity LastAcknowledgedState { get; set; }
		double LastAcknowledgedStateTime { get; set; }
		uint PreviousLastAck { get; set; }

		IEntity NoCorrections { get; set; }
		public Vec2 NoCorrPos { get { return NoCorrections.Position; } }

		public MainClientChampion(ChampionSpawnInfo spawnInfo, GameMatch match)
			: base(spawnInfo)
        {
			Match = match;

			ServerPosition = Position;

			PackagedActions = new Queue<PlayerAction>();
			RecentActions = new List<PlayerAction>();
			Corrections = new List<AcknowledgeInfo>();
			Corrected = false;
			PreviousLastAck = IDGenerator.NO_ID;

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;

			LastAcknowledgedState = null;

			NoCorrections = (IEntity)this.Clone();
        }

		/// <summary>
		/// Update the champion, applying client-side prediction and correction.
		/// </summary>
		public override void Update(GameTime deltaTime)
		{
			// client correction
			if (Corrected) {
				ApplyCorrection(Client.Instance.GetTime().TotalSeconds);
				Corrected = false;
			}

			// client-side prediction
			IEntity temp = (IEntity)this.Clone();
			this.Clone(NoCorrections);
			Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds);
			NoCorrections = (IEntity)Match.CurrentState.GetEntity(ID).Clone();
			this.Clone(temp);
			Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds);

			LerpTowardsSimulatedPosition(deltaTime.ElapsedGameTime.TotalSeconds);
		}

		/// <summary>
		/// Lerps the drawn position towards our simulated position.
		/// </summary>
		void LerpTowardsSimulatedPosition(double deltaSeconds)
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
		/// Take the new state update from the server (i.e. its correction) and store it, so
		/// that we can apply it on our next frame.
		/// </summary>
		public override void AuthoritativeChangePosition(Vec2 position, Vec2 velocity, double time)
		{
			ServerPosition = position;
			Corrections.Add(new AcknowledgeInfo(time, (Vec2)position.Clone(), (Vec2)velocity.Clone(), PreviousLastAck));
			Corrected = true;

            // Remove corrections that are too old
            double limit = Client.Instance.GetTime().TotalSeconds - CORRECTIONS_TIME_KEPT.TotalSeconds;
			int i = 0;
			for (; i < Corrections.Count &&
                   Corrections[i].Time < limit; ++i) { }
            Corrections.RemoveRange(0, i);

			// Remove actions that are most certainly acknowledged
			if (Corrections.Count > 0) {
				for (i = 0; i < RecentActions.Count &&
				     RecentActions[i].ID <= Corrections[0].LastAcknowledgedActionId; ++i) { }
				RecentActions.RemoveRange(0, i);
			}

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;
		}

		/// <summary>
		/// Takes the acknowledge info from the server (the position and velocity correction along with
		/// the last acknowledged action) and incorporate it in our game.
		/// The way that this works is that we take the most recent usable correction (that is to say, the
		/// correction that happened before any of our not-yet-acknowledged actions), and resimulate the
		/// world along with the unacknowledged actions up until now. This should give results very similar
		/// to a game without correction without external interaction.
		/// </summary>
		void ApplyCorrection(double currentTime)
		{
			Vec2 original = (Vec2)Position.Clone();
			Vec2 ov = (Vec2)Velocity.Clone();

			// get a correction from the server to work with
			AcknowledgeInfo ack = GetCorrectionBeforeUnackAction();
			if (ack == null) { // no correction to use.
				Console.Write("COME ON . >:(");
				return;
			}


			// go back to our last acknowledged state (if any)
			double time = ack.Time;
			/*if (LastAcknowledgedState != null) {
				this.Clone(LastAcknowledgedState);
				time = LastAcknowledgedStateTime;
			}*/

			// apply the server's correction
			Position = ack.Position;
			Velocity = ack.Velocity;

			// resimulate up until the given state update
			for (int i = 0; i < RecentActions.Count; ++i) {
				if (RecentActions[i].ID > ack.LastAcknowledgedActionId) {
					Debug.Assert(RecentActions[i].Time >= ack.Time);

					var deltaT = RecentActions[i].Time - time;

					if (deltaT > 0) {
						Match.CurrentState.ApplyPhysicsUpdate(ID, deltaT);
					}

					ExecuteAction(RecentActions[i].Type);

					time = RecentActions[i].Time;
				}
			}

			var deltaTime = currentTime - time;
			if (deltaTime > 0) {
				Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime);
			}

            if (Vec2.Distance(Position, original) > 1)
            {
				Console.WriteLine(String.Format("s-o: {0}   a: {5}   s: {1}   o: {2}  lac: {3}   c(ua): {4}   s-a: {6}   o-a: {7}",
                                                Position - original,
                                                Position,
                                                original,
				                                ack.LastAcknowledgedActionId,
				                                RecentActions.FindAll(a => a.ID > ack.LastAcknowledgedActionId).Count,
				                                ack.Position,
				                                Position - ack.Position,
				                                original - ack.Position));
            }
		}

		/// <summary>
		/// Get the correction info received from the server that is *right before* our first
		/// unacknowledged action. This simplifies our calculations a lot, since we just apply
		/// this correction, reapply all the unacknowledged actions and our correction is done.
		/// If we would always simply take the latest correction, we'd have to deal with applying
		/// corrections while simulating unacknowledged actions right before our correction, effectively
		/// undoing our actions.
		/// </summary>
		/// <returns>null if there are no unacknowledged actions or not enough corrections (must then ignore the correction for the frame).</returns>
		AcknowledgeInfo GetCorrectionBeforeUnackAction()
		{
			if (Corrections.Count == 0) // no corrections? no need to apply a server correction then.
				return null;

			if (RecentActions.Count == 0) // no ations to redo? we just take our most recent correction and apply it
				return Corrections[Corrections.Count - 1];

			AcknowledgeInfo ack = null;

			for (int i = Corrections.Count - 1; i >= 0 && ack == null; --i) {
				// first unacknowledged action of that correction
				int firstAction = RecentActions.FindIndex(a => a.ID > Corrections[i].LastAcknowledgedActionId);
				if (firstAction < 0 || // if we have a correction with *no* unacknowledged action, we want to use it! (most recent correction)
				    RecentActions[firstAction].Time > Corrections[i].Time) { // we don't have unacknowledged actions before our correction, so this is our first before our first unack-ed action
					ack = Corrections[i];
				}
			}

			return ack;
		}

		public override void SetLastAcknowledgedActionID(uint id)
		{
			Debug.Assert(id >= PreviousLastAck);

			if (id > PreviousLastAck) { // a new id
				base.SetLastAcknowledgedActionID(id);
				PreviousLastAck = id;

				LastAcknowledgedState = (IEntity)this.Clone();
				LastAcknowledgedStateTime = Client.Instance.GetTime().TotalSeconds;
			}
		}

		public void PackageAction(PlayerAction action)
		{
			PackagedActions.Enqueue(action);

			RecentActions.Add(action);

			IEntity temp = (IEntity)this.Clone();
			this.Clone(NoCorrections);
			ExecuteAction(action.Type);
			NoCorrections = (IEntity)Match.CurrentState.GetEntity(ID).Clone();
			this.Clone(temp);
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

