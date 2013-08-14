//
//  GameMatch.cs
//
//  Author:
//       Jesse <>
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
using GREATLib.World;
using System.Collections.Generic;
using System.Diagnostics;
using GREATLib.Physics;
using GREATLib.Entities;

namespace GREATLib.Network
{
	/// <summary>
	/// An individual game match, 
	/// </summary>
    public class GameMatch
    {
		/// <summary>
		/// The time taken between each state update sent by the server.
		/// This is in the game's lib because it must be the same value for both the client
		/// and the server.
		/// </summary>
		public static readonly TimeSpan STATE_UPDATE_INTERVAL = TimeSpan.FromMilliseconds(50.0);

		public GameWorld World { get; private set; }

		PhysicsEngine Physics { get; set; }
		Dictionary<uint, IEntity> Entities { get; set; }

        public GameMatch()
        {
			World = new GameWorld();
			Physics = new PhysicsEngine(World);

			Entities = new Dictionary<uint, IEntity>();
        }

		/// <summary>
		/// Adds a new entity to the game match.
		/// </summary>
		/// <param name="entity">Entity.</param>
		public void AddEntity(IEntity entity)
		{
			Debug.Assert(entity != null);
			Debug.Assert(!Entities.ContainsKey(entity.ID));

			if (!Entities.ContainsKey(entity.ID)) {
				Entities.Add(entity.ID, entity);
			}
		}

		/// <summary>
		/// Applies one physics update to an entity (with the given ID) if it should (the physics
		/// engine only updates at a certain rate).
		/// </summary>
		public void ApplyPhysicsUpdate(uint id, double deltaSeconds, ref int xMovement)
		{
			Debug.Assert(deltaSeconds > 0.0);
			Debug.Assert(Entities.ContainsKey(id));

			if (Entities.ContainsKey(id)) {
				Physics.Update(deltaSeconds, Entities[id], ref xMovement);
			}
		}

		/// <summary>
		/// Moves the specified entity (given its ID) in the specified direction.
		/// </summary>
		public void Move(uint id, HorizontalDirection direction, ref int xMovement)
		{
			Debug.Assert(Entities.ContainsKey(id));

			if (Entities.ContainsKey(id)) {
				Physics.Move(Entities[id], direction, ref xMovement);
			}
		}

		/// <summary>
		/// Makes the specified entity (given its ID) jump.
		/// </summary>
		public void Jump(uint id)
		{
			Debug.Assert(Entities.ContainsKey(id));

			if (Entities.ContainsKey(id)) {
				Physics.Jump(Entities[id]);
			}
		}
    }
}

