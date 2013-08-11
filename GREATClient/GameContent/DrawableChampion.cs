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
using GREATLib.World;

namespace GREATClient
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : IDraw
    {
		/// <summary>
		/// Gets or sets the function for action.
		/// </summary>
		Dictionary<PlayerActionType, Action> FunctionForAction { get; set; }

		/// <summary>
		/// Client-side version of the main champion, managing the network interaction and local
		/// simulation of the client's champion.
		/// </summary>
		MainClientChampion Champion { get; set; }

		/// <summary>
		/// TODO: class for debug info
		/// </summary>
		DrawableRectangle ChampionDrawnRect { get; set; }



        public DrawableChampion(ChampionsInfo championsInfo, GameWorld world) //TODO: new MainClientDrawableChampion
        {
			FillFunctionsForActions();
			Champion = new MainClientChampion(world);
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
		/// Fills the functions associated to certain player actions.
		/// </summary>
		void FillFunctionsForActions()
		{
			Debug.Assert(FunctionForAction == null);

			FunctionForAction = new Dictionary<PlayerActionType, Action>();

			FunctionForAction.Add(PlayerActionType.Jump, () => {
				Champion.Jump(); 
			});
			FunctionForAction.Add(PlayerActionType.MoveLeft, () => {
				Champion.MoveLeft();
			});
			FunctionForAction.Add(PlayerActionType.MoveRight, () => {
				Champion.MoveRight();
			});
		}

		/// <summary>
		/// Packages a client-side action to be sent to the server. This also simulates the action locally
		/// for client-side prediction.
		/// </summary>
		public void PackageAction(PlayerActionType action)
		{
			Debug.Assert(FunctionForAction.ContainsKey(action));

			PlayerAction toPackage = new PlayerAction() { 
				Type = action,
				Time = Client.Instance.GetTime().TotalSeconds,
				ID   = IDGenerator.GenerateID()
			};

			FunctionForAction[action]();

			Champion.PackageAction(toPackage);
		}

		public List<PlayerAction> GetActionPackage()
		{
			return Champion.GetActionPackage();
		}
    }
}
