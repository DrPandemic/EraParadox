//
//  ClientChampion.cs
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
using GREATLib;
using GREATLib.Entities;
using GREATClient.BaseClass;
using Microsoft.Xna.Framework;
using GREATLib.Entities.Champions;

namespace GREATClient.Network
{
	public abstract class ClientChampion : ICharacter, IUpdatable
    {
		/// <summary>
		/// Gets the drawn position of the champion.
		/// This is the position where we should currently draw the champion.
		/// </summary>
		/// <value>The drawn position.</value>
		public Vec2 DrawnPosition { get; protected set; }

        public ClientChampion(ChampionSpawnInfo spawnInfo)
			: base(spawnInfo.ID, spawnInfo.SpawningPosition)
        {
			DrawnPosition = Position;
        }

		public virtual void Update(GameTime dt)
		{
		}

		/// <summary>
		/// Called when an authority (i.e. the server) indicates a new position.
		/// Depending on who the champion is (the local player or a remote client),
		/// we'll act differently.
		/// </summary>
		public virtual void AuthoritativeChangePosition(StateUpdateData data, double time)
		{
			Position = data.Position;
		}

		/// <summary>
		/// Sets the ID of the last acknowledged action by the server.
		/// </summary>
		public virtual void SetLastAcknowledgedActionID(uint id) { }
    }
}

