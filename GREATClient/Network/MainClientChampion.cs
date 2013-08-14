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

namespace GREATClient.Network
{
	/// <summary>
	/// Represents the champion's data of the main player, the one playing the game on
	/// this instance of the program.
	/// </summary>
    public class MainClientChampion : IEntity, IUpdatable
    {
		/// <summary>
		/// Gets the drawn position of the champion.
		/// This is the position where we should currently draw the champion.
		/// </summary>
		/// <value>The drawn position.</value>
		public Vec2 DrawnPosition { get; private set; }

		GameMatch Match { get; set; }

		/// <summary>
		/// The amount of moving actions that we have to make.
		/// When we move right, we do +1.
		/// When we move left, we do -1.
		/// </summary>
		int xMovement;

		List<PlayerAction> PackagedActions { get; set; }

		public MainClientChampion(ChampionSpawnInfo spawnInfo, GameMatch match)
			: base(spawnInfo.ID, spawnInfo.SpawningPosition)
        {
			Match = match;

			DrawnPosition = Position;

			xMovement = 0;
			PackagedActions = new List<PlayerAction>();
        }

		/// <summary>
		/// Update the champion, applying client-side prediction.
		/// </summary>
		public void Update(GameTime deltaTime)
		{
			// client-side prediction
			Match.ApplyPhysicsUpdate(ID, deltaTime.ElapsedGameTime.TotalSeconds, ref xMovement);

			DrawnPosition = Position;

			//TODO: remove, used for testing purposes
			ILogger.Log(Position.ToString());
		}

		public void PackageAction(PlayerAction action)
		{
			PackagedActions.Add(action);
		}

		public void MoveLeft()
		{
			Match.Move(ID, HorizontalDirection.Left, ref xMovement);
		}
		public void MoveRight()
		{
			Match.Move(ID, HorizontalDirection.Right, ref xMovement);
		}
		public void Jump()
		{
			Match.Jump(ID);
		}

		public List<PlayerAction> GetActionPackage()
		{
			return PackagedActions;
		}
    }
}

