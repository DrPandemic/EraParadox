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
using GREATLib.Entities.Champions;

namespace GREATClient.Network
{
	/// <summary>
	/// Represents the champion's data of the main player, the one playing the game on
	/// this instance of the program.
	/// </summary>
    public sealed class MainClientChampion : ClientChampion
    {
		class CorrectionInfo
		{
			public double Time { get; private set; }
			public Vec2 Position { get; private set; }
			public Vec2 Velocity { get; private set; }
			public ulong LastAcknowledgedActionId { get; private set; }

			public CorrectionInfo(double time, Vec2 pos, Vec2 vel, ulong lastAckId)
			{
				Time = time;
				Position = pos;
				Velocity = vel;
				LastAcknowledgedActionId = lastAckId;
			}
		}

		const float POSITION_DISTANCE_TO_SNAP = 100f;
		const float SMOOTH_FACTOR = 0.85f;
        static readonly TimeSpan CORRECTIONS_TIME_KEPT = TimeSpan.FromSeconds(1.0);

		/// <summary>
		/// Gets or sets the server position.
		/// This is the last position that we received from the server.
		/// </summary>
		/// <remarks>This shouldn't be changed within the class, only set when the server
		/// gives us a new position.</remarks>
		public Vec2 ServerPosition { get; private set; }
		Vec2 ServerVelocity { get; set; }

		GameMatch Match { get; set; }

		Queue<PlayerAction> PackagedActions { get; set; }
		List<PlayerAction> RecentActions { get; set; }
		List<CorrectionInfo> Corrections { get; set; }
		bool Corrected { get; set; }

		ulong PreviousLastAck { get; set; }

		IEntity NoCorrections { get; set; }
		public Vec2 NoCorrPos { get { return NoCorrections.Position; } }
		public Vec2 ServerCorrectionUsed { get; private set; }

		public MainClientChampion(ChampionSpawnInfo spawnInfo, GameMatch match)
			: base(spawnInfo)
        {
			Match = match;

			ServerPosition = Position;
			ServerCorrectionUsed = Position;

			PackagedActions = new Queue<PlayerAction>();
			RecentActions = new List<PlayerAction>();
			Corrections = new List<CorrectionInfo>();
			Corrected = false;
			PreviousLastAck = IDGenerator.NO_ID;


			NoCorrections = (IEntity)this.Clone();
        }

		/// <summary>
		/// Update the champion, applying client-side prediction and correction.
		/// </summary>
		public override void Update(GameTime deltaTime)
		{
			// client correction
			if (Corrected) {
				ApplyCorrection(Client.Instance.GetTime().TotalSeconds - deltaTime.ElapsedGameTime.TotalSeconds);
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
			} else { // If we must interpolate our position (we're not too far)
				DrawnPosition = Vec2.Lerp(DrawnPosition, Position, SMOOTH_FACTOR);
			}
		}

		/// <summary>
		/// Take the new state update from the server (i.e. its correction) and store it, so
		/// that we can apply it on our next frame.
		/// </summary>
		public override void AuthoritativeChangePosition(StateUpdateData data, double time)
		{
			base.AuthoritativeChangePosition(data, time);
			Animation = data.Animation;
			FacingLeft = data.FacingLeft;

			ServerPosition = data.Position;
			Corrections.Add(new CorrectionInfo(time, data.Position, data.Velocity, PreviousLastAck));
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
			// get a correction from the server to work with
			CorrectionInfo corr = GetCorrectionBeforeUnackAction();
			if (corr == null) { // no correction to use.
				return;
			}

			ServerCorrectionUsed = corr.Position;

			// go back to our last acknowledged state (if any)
			double time = corr.Time;

			// apply the server's correction
			Position = corr.Position;
			Velocity = corr.Velocity;

			// resimulate up until the given state update
			for (int i = 0; i < RecentActions.Count; ++i) {
				if (RecentActions[i].ID > corr.LastAcknowledgedActionId) {
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
		CorrectionInfo GetCorrectionBeforeUnackAction()
		{
			if (Corrections.Count == 0) { // no corrections? no need to apply a server correction then.
				return null;
			}

			if (RecentActions.Count == 0) // no ations to redo? we just take our most recent correction and apply it
				return Corrections[Corrections.Count - 1];

			CorrectionInfo corr = null;

			int i;
			for (i = Corrections.Count - 1; i >= 0 && corr == null; --i) {
				// first unacknowledged action of that correction
				int firstAction = RecentActions.FindIndex(a => a.ID > Corrections[i].LastAcknowledgedActionId);
				if (firstAction < 0 || // if we have a correction with *no* unacknowledged action, we want to use it! (most recent correction)
				    RecentActions[firstAction].Time > Corrections[i].Time) { // we don't have unacknowledged actions before our correction, so this is our first before our first unack-ed action
					corr = Corrections[i];
				}
			}

			if (corr == null)
				corr = Corrections[Corrections.Count - 1];

			return corr;
		}

		public override void SetLastAcknowledgedActionID(ulong id)
		{
			Debug.Assert(id >= PreviousLastAck);

			if (id > PreviousLastAck) { // a new id
				base.SetLastAcknowledgedActionID(id);
				PreviousLastAck = id;
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

				case PlayerActionType.Spell1:
				case PlayerActionType.Spell2:
				case PlayerActionType.Spell3:
				case PlayerActionType.Spell4:
					// no client-side prediction for spells
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

