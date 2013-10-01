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
			public double Time { get; set; }
			public Vec2 Position { get; set; }
			public Vec2 Velocity { get; set; }

			public AcknowledgeInfo(double time, Vec2 pos, Vec2 vel)
			{
				Time = time;
				Position = pos;
				Velocity = vel;
			}
		}

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
		Vec2 ServerVelocity { get; set; }

		Vec2 PositionBeforeLerp { get; set; }
		double TimeSinceLastServerUpdate { get; set; }
		uint LastAcknowledgedActionID { get; set; }
		AcknowledgeInfo ServerAcknowledge { get; set; }

		GameMatch Match { get; set; }

		Queue<PlayerAction> PackagedActions { get; set; }
		List<PlayerAction> UnacknowledgedActions { get; set; }

		Dictionary<uint, double> l = new Dictionary<uint, double>();
		IEntity LastAcknowledgedState { get; set; }
		double LastAcknowledgedStateTime { get; set; }

		IEntity NoCorrections { get; set; }
		public Vec2 NoCorrPos { get { return NoCorrections.Position; } }

		public MainClientChampion(ChampionSpawnInfo spawnInfo, GameMatch match)
			: base(spawnInfo)
        {
			Match = match;

			ServerPosition = Position;

			PackagedActions = new Queue<PlayerAction>();
			UnacknowledgedActions = new List<PlayerAction>();

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;

			LastAcknowledgedActionID = IDGenerator.NO_ID;
			LastAcknowledgedState = null;

			NoCorrections = (IEntity)this.Clone();
        }

		/// <summary>
		/// Update the champion, applying client-side prediction.
		/// </summary>
		public override void Update(GameTime deltaTime)
		{
			// client correction
			if (ServerAcknowledge != null) { // we must resimulate from the given acknowledged action time
				ApplyCorrection(ServerAcknowledge);
				ServerAcknowledge = null;
			}

			// client-side prediction
			IEntity temp = (IEntity)this.Clone();
			this.Clone(NoCorrections);
			Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds);
			NoCorrections = (IEntity)Match.CurrentState.GetEntity(ID).Clone();
			this.Clone(temp);
			Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds);

			LerpTowardsServerPosition(deltaTime.ElapsedGameTime.TotalSeconds);
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
		public override void AuthoritativeChangePosition(Vec2 position, Vec2 velocity, double time)
		{
			ServerPosition = position;
			ServerAcknowledge = new AcknowledgeInfo(time, (Vec2)position.Clone(), (Vec2)velocity.Clone());

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;
		}

		void ApplyCorrection(AcknowledgeInfo ack)
		{
			Vec2 original = (Vec2)Position.Clone();

			// remove the actions that the server has done
			RemoveAcknowledgedActions();

			// go back to our last acknowledged state (if any)
			double time = UnacknowledgedActions.Count > 0 ? UnacknowledgedActions[0].Time : 0.0;
			if (LastAcknowledgedState != null) {
				this.Clone(LastAcknowledgedState);
				Console.WriteLine("From last ack: " + Position);
				time = LastAcknowledgedStateTime;
			} else {
				Console.WriteLine("NOPE.");
			}

			// apply the server's correction
			Position = ack.Position;
			Velocity = ack.Velocity;

			// resimulate up until the given state update
			for (int i = 0; i < UnacknowledgedActions.Count; ++i) {
				var deltaT = UnacknowledgedActions[i].Time - time;

				if (deltaT > 0) {
					Match.CurrentState.ApplyPhysicsUpdate(ID, deltaT);
				}

				ExecuteAction(UnacknowledgedActions[i].Type);

				time = UnacknowledgedActions[i].Time;
			}
		}

		void RemoveAcknowledgedActions()
		{
			UnacknowledgedActions.RemoveAll(a => a.ID <= LastAcknowledgedActionID);
		}

		public override void SetLastAcknowledgedActionID(uint id)
		{
			if (id > LastAcknowledgedActionID) { // a new id
				base.SetLastAcknowledgedActionID(id);
				LastAcknowledgedActionID = id;

				LastAcknowledgedState = (IEntity)this.Clone();
				LastAcknowledgedStateTime = Client.Instance.GetTime().TotalSeconds;
			}
		}

		public void PackageAction(PlayerAction action)
		{
			PackagedActions.Enqueue(action);

			UnacknowledgedActions.Add(action);
			l.Add(action.ID, action.Time);

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

