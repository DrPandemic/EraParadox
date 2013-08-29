//
//  DrawableChampion.cs
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
using GREATClient;
using Microsoft.Xna.Framework;
using GREATLib;
using System.Collections.Generic;
using System.Diagnostics;
using GREATClient.Network;
using GREATLib.Entities;
using GREATLib.Network;

namespace GREATClient
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : IDraw
    {
		/// <summary>
		/// Client-side version of the main champion, managing the network interaction and local
		/// simulation of the client's champion.
		/// </summary>
		MainClientChampion Champion { get; set; }
		public IEntity Entity { get { return Champion; } }

		/// <summary>
		/// TODO: class for debug info
		/// </summary>
		DrawableRectangle ChampionDrawnRect { get; set; }



        public DrawableChampion(ChampionSpawnInfo spawnInfo, ChampionsInfo championsInfo, GameMatch match) //TODO: new MainClientDrawableChampion
        {
			Champion = new MainClientChampion(spawnInfo, match);
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			ChampionDrawnRect = new DrawableRectangle(new Rectangle(0, 0, 15, 30), Color.White);
			Parent.AddChild(ChampionDrawnRect);
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			Champion.Update(dt);

			ChampionDrawnRect.Position = GameLibHelper.ToVector2(Champion.DrawnPosition);
		}

		protected override void OnDraw(Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
		{
			// The champion's animations take care of the drawing.
		}

		/// <summary>
		/// Packages a client-side action to be sent to the server. This also simulates the action locally
		/// for client-side prediction.
		/// </summary>
		public void PackageAction(PlayerActionType action)
		{
			PlayerAction toPackage = new PlayerAction(
            	IDGenerator.GenerateID(),
				action,
            	(float)Client.Instance.GetTime().TotalSeconds,
				Champion.Position);

			Champion.PackageAction(toPackage);
		}

		/// <summary>
		/// Sets the last acknowledged action by the server.
		/// </summary>
		public void SetLastAcknowledgedAction(uint actionID)
		{
			Champion.LastAcknowledgedActionID = actionID;
		}

		public Queue<PlayerAction> GetActionPackage()
		{
			return Champion.GetActionPackage();
		}
    }
}
