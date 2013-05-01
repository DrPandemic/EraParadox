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
using GREATLib.Entities.Player.Champions;
using Microsoft.Xna.Framework;

namespace GREATClient
{
	/// <summary>
	/// Represents a champion in the game.
	/// </summary>
    public class DrawableChampion : DrawableImage
    {
		public IChampion Champion { get; set; }

        public DrawableChampion(IChampion champion, ChampionsInfo championsInfo)
			: base(championsInfo.GetInfo(champion.Type).AssetName + "_stand") //TODO: handle many animations instead
        {
			Champion = champion;
			OriginRelative = new Vector2(0.5f, 1f); // position at the feet
        }
		public override void Update(Microsoft.Xna.Framework.GameTime dt)
		{
			base.Update(dt);
			Position = Champion.Position.ToVector2();
		}
    }
}

