//
//  MatchState.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using GREATLib.Physics;

namespace GREATLib.Network
{
	/// <summary>
	/// The current state of a game match.
	/// </summary>
    public class MatchState : ICloneable
    {
		PhysicsEngine Physics { get; set; }


		Dictionary<uint, IEntity> Entities { get; set; }

		public MatchState(PhysicsEngine physics)
		{
			Entities = new Dictionary<uint, IEntity>();
			Physics = physics;
		}

		/// <summary>
		/// Adds a new entity to the game match.
		/// </summary>
		public void AddEntity(IEntity entity)
		{
			Debug.Assert(entity != null);
			Debug.Assert(!Entities.ContainsKey(entity.ID));

			if (!Entities.ContainsKey(entity.ID)) {
				Entities.Add(entity.ID, entity);
			}
		}

		/// <summary>
		/// Returns whether the game contains an entity with the given ID or not.
		/// </summary>
		public bool ContainsEntity(uint id)
		{
			return Entities.ContainsKey(id);
		}

		/// <summary>
		/// Gets the entity with the specified ID.
		/// </summary>
		public IEntity GetEntity(uint id)
		{
			Debug.Assert(ContainsEntity(id));

			return Entities[id];
		}

		/// <summary>
		/// Applies one physics update to an entity (with the given ID) if it should (the physics
		/// engine only updates at a certain rate).
		/// </summary>
		public void ApplyPhysicsUpdate(uint id, double deltaSeconds)
		{
			Debug.Assert(deltaSeconds > 0.0);
			Debug.Assert(Entities.ContainsKey(id));

			if (Entities.ContainsKey(id)) {
				Physics.Update(deltaSeconds, Entities[id]);
			}
		}

		/// <summary>
		/// Moves the specified entity (given its ID) in the specified direction.
		/// </summary>
		public void Move(uint id, HorizontalDirection direction)
		{
			Debug.Assert(Entities.ContainsKey(id));

			if (Entities.ContainsKey(id)) {
				Physics.Move(Entities[id], direction);
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

		/// <summary>
		/// Clones the match state.
		/// </summary>
		public object Clone()
		{
			MatchState state = new MatchState(Physics);
			foreach (IEntity entity in Entities.Values) {
				state.Entities.Add(entity.ID, (IEntity)entity.Clone());
			}
			return state;
		}
    }
}

