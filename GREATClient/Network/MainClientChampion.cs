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
		/// <summary>
		/// The distance required between the simulated position and the drawn position
		/// that makes us snap directly to it (instead of interpolating to it, we just 
		/// directly set it).
		/// </summary>
		const float POSITION_DISTANCE_TO_SNAP = 50f;

		/// <summary>
		/// Gets or sets the last acknowledged action ID of the player.
		/// That is to say, the last action from us that the server executed.
		/// </summary>
		public uint LastAcknowledgedActionID { get; set; }

		/// <summary>
		/// Gets or sets the server position.
		/// This is the last position that we received from the server.
		/// </summary>
		/// <remarks>This shouldn't be changed within the class, only set when the server
		/// gives us a new position.</remarks>
		public Vec2 ServerPosition { get; private set; }

		Vec2 PositionBeforeLerp { get; set; }
		double TimeSinceLastServerUpdate { get; set; }

		GameMatch Match { get; set; }

		Queue<PlayerAction> PackagedActions { get; set; }
		Queue<PlayerAction> UnacknowledgedActions { get; set; }

		public MainClientChampion(ChampionSpawnInfo spawnInfo, GameMatch match)
			: base(spawnInfo)
        {
			LastAcknowledgedActionID = IDGenerator.NO_ID;
			Match = match;

			ServerPosition = Position;

			PackagedActions = new Queue<PlayerAction>();
			UnacknowledgedActions = new Queue<PlayerAction>();

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;
        }

		/// <summary>
		/// Update the champion, applying client-side prediction.
		/// </summary>
		public override void Update(GameTime deltaTime)
		{
			// client-side prediction
			Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds);

			LerpTowardsServerPosition(deltaTime.ElapsedGameTime.TotalSeconds);
		}

		void LerpTowardsServerPosition(double deltaSeconds)
		{
			float distanceSq = Vec2.DistanceSquared(Position, DrawnPosition);

			// If we must interpolate our position (we're not too far)
			if (distanceSq < POSITION_DISTANCE_TO_SNAP * POSITION_DISTANCE_TO_SNAP) {

				Debug.Assert(TimeSinceLastServerUpdate >= 0.0);

				TimeSinceLastServerUpdate += deltaSeconds;
				double progress = TimeSinceLastServerUpdate / GameMatch.STATE_UPDATE_INTERVAL.TotalSeconds;
				progress = Math.Min(1.0, progress);

				DrawnPosition = Vec2.Lerp(PositionBeforeLerp, Position, (float)progress);

			} else { // if we must snap directly to the simulated position
				ILogger.Log(String.Format("Snapping position({0}) to simulated({1}). -> distance squared:{2}", DrawnPosition, Position, distanceSq), LogPriority.High);
				DrawnPosition = Position;
				TimeSinceLastServerUpdate = 0.0;
				PositionBeforeLerp = DrawnPosition;
			}
		}

		/// <summary>
		/// Take the new position given by the server and resimulate our unacknowledged actions
		/// from there.
		/// </summary>
		public override void AuthoritativeChangePosition(Vec2 position, double time)
		{
			ServerPosition = position;

			// resimulate the unacknowledged actions
			/*Position = ServerPosition.Clone() as Vec2;

			// remove the acknowledged actions
			while (UnacknowledgedActions.Count > 0 &&
			       UnacknowledgedActions.Peek().ID <= LastAcknowledgedActionID) {
				UnacknowledgedActions.Dequeue();
			}

			// go through the unacknowledged actions and just replay them
			float lastActionTime = UnacknowledgedActions.Count > 0 ? UnacknowledgedActions.Peek().Time : 0f;
			foreach (PlayerAction action in UnacknowledgedActions) {
				ExecuteAction(action.Type);

				float deltaTime = action.Time - lastActionTime;
				Debug.Assert(deltaTime >= 0f);

				if (deltaTime > 0f) { // we don't want to do a physics update if we're on the same frame (dt==0)
					Match.CurrentState.ApplyPhysicsUpdate(ID, deltaTime);
				}
			}*/

			TimeSinceLastServerUpdate = 0.0;
			PositionBeforeLerp = Position;
		}

		public void PackageAction(PlayerAction action)
		{
			PackagedActions.Enqueue(action);
			UnacknowledgedActions.Enqueue(action);

			ExecuteAction(action.Type);
			ILogger.Log(String.Format("Doing action: id={0}, time={1}, type={2}, pos={3}", 
			                          action.ID,action.Time,action.Type,action.Position), 
			            LogPriority.Low);
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

