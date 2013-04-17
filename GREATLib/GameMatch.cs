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
using GREATLib.Entities.Player;
using System.Collections.Generic;
using GREATLib.Entities;
using GREATLib.Entities.Physics;
using GREATLib.Entities.Player.Champions;
using System.Diagnostics;

namespace GREATLib
{
	/// <summary>
	/// An individual game match, 
	/// </summary>
    public class GameMatch : ISynchronizable
    {
		public PhysicsSystem Physics { get; private set; }

		public GameWorld World { get; private set; }

		private EntityIDGenerator IDGenerator { get; set; }

		private Dictionary<int, PhysicsEntity> PhysicsEntities { get; set; }
		private Dictionary<int, Player> Players { get; set; }

        public GameMatch()
        {
			IDGenerator = new EntityIDGenerator();
			Physics = new PhysicsSystem();
			World = new GameWorld();
			Players = new Dictionary<int, Player>();
			PhysicsEntities = new Dictionary<int, PhysicsEntity>();
        }

		public void Update(float deltaSeconds)
		{
			Debug.Assert(deltaSeconds > 0f, "The delta seconds while updating the match is too small.");

			Physics.Update(deltaSeconds, World, PhysicsEntities.Values);
		}

		/// <summary>
		/// Adds the player to the match with his champion and returns its id.
		/// </summary>
		/// <returns>The player's id.</returns>
		/// <param name="player">Player.</param>
		/// <param name="champion">Champion.</param>
		public int AddPlayer(Player player, IChampion champion)
		{
			player.Id = IDGenerator.GenerateID();
			player.Champion = champion;
			Players.Add(player.Id, player);

			AddEntity(champion);

			return player.Id;
		}

		public void MovePlayer(int playerId, HorizontalDirection direction)
		{
			Debug.Assert(playerId != EntityIDGenerator.NO_ID, "Invalid ID for a player.");
			Debug.Assert(Players.ContainsKey(playerId), "No player with the given id.");

			Physics.Move(Players[playerId].Champion, direction);
		}
		/// <summary>
		/// Gets the player with the given ID.
		/// </summary>
		/// <returns>The player.</returns>
		/// <param name="id">The player's identifier.</param>
		public Player GetPlayer(int id)
		{
			Debug.Assert(id != EntityIDGenerator.NO_ID, "Invalid ID for a player.");
			Debug.Assert(Players.ContainsKey(id), "No player with the given id.");

			return Players[id];
		}
		/// <summary>
		/// Adds the entity to the match and returns its id.
		/// </summary>
		/// <returns>The entity's id.</returns>
		/// <param name="entity">Entity.</param>
		private int AddEntity(PhysicsEntity entity)
		{
			entity.Id = IDGenerator.GenerateID();
			PhysicsEntities.Add(entity.Id, entity);
			return entity.Id;
		}
    }
}

