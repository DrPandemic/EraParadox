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
using GREATClient;
using Microsoft.Xna.Framework;
using GREATLib;
using System.Collections.Generic;
using GREATLib.Entities;
using GREATLib.Network;
using GREATClient.BaseClass;
using GREATClient.Network;

namespace GREATClient.GameContent
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : Container
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
		DrawableRectangle ChampionServerRect { get; set; } 

		/// <summary>
		/// Gets or sets the current animation.
		/// </summary>
		/// <value>The current animation.</value>
		ChampionAnimation CurrentAnimation 
		{ 
			get {
				return ChampionSprite.CurrentAnimation;
			}
			set {
				if (value != CurrentAnimation) {
					ChampionSprite.PlayAnimation(value);
				}
			}
		}

		/// <summary>
		/// Gets or sets the champion sprite.
		/// This is the object that will be displayed.
		/// </summary>
		/// <value>The champion sprite.</value>
		DrawableChampionSprite ChampionSprite { get; set; }

        public DrawableChampion(ChampionSpawnInfo spawnInfo, ChampionsInfo championsInfo, GameMatch match) //TODO: new MainClientDrawableChampion
        {
			Champion = new MainClientChampion(spawnInfo, match);
			ChampionSprite = new DrawableChampionSprite(ChampionTypes.StickMan, new ChampionsInfo());
			ChampionSprite.PlayAnimation(ChampionAnimation.run);
			ChampionSprite.RelativeOrigin = new Vector2(0.5f,0.5f);
        }
		protected override void OnLoad(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.GraphicsDevice gd)
		{
			base.OnLoad(content, gd);

			AddChild(ChampionServerRect = new DrawableRectangle(new Rectangle(0, 0, 15, 30), Color.Green));

			ChampionDrawnRect = new DrawableRectangle(new Rectangle(0, 0, 15, 30), Color.White);
			AddChild(ChampionDrawnRect);

			AddChild(ChampionSprite);
		}
		protected override void OnUpdate(Microsoft.Xna.Framework.GameTime dt)
		{
			// Update the champion animation


			//Champion.Update(dt);

			ChampionServerRect.Position = GameLibHelper.ToVector2(Champion.ServerPosition);
			ChampionDrawnRect.Position = GameLibHelper.ToVector2(Champion.DrawnPosition);
			ChampionSprite.Position = GameLibHelper.ToVector2(Champion.DrawnPosition);
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

		public override bool IsBehind(Vector2 position)
		{
			if (Parent != null) {
				//TODO: use the rectangle of the current animation?
				return GameLibHelper.ToRectangle(Champion.CreateCollisionRectangle()).Contains(
					(int)position.X,
					(int)position.Y);
			}
			return false;
		}
    }
}
