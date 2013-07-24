//
//  DrawableChampionSprite.cs
//
//  Author:
//       parasithe <>
//
//  Copyright (c) 2013 parasithe
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
using GREATLib.Entities.Player.Champions;

namespace GREATClient.GameContent
{
    public class DrawableChampionSprite : DrawableSprite
    {
		/// <summary>
		/// Gets or sets the champion.
		/// </summary>
		/// <value>The champion.</value>
		IChampion champion { get; set; }

		/// <summary>
		/// Gets or sets the infos.
		/// </summary>
		/// <value>The infos.</value>
		ChampionInfo information { get; set; }

		public DrawableChampionSprite(IChampion champ, ChampionsInfo championsInfo)
			: base (championsInfo.GetInfo(champ.Type).AssetName,
			        championsInfo.GetInfo(champ.Type).FrameWidth,
			        championsInfo.GetInfo(champ.Type).FrameHeight,
			        championsInfo.GetInfo(champ.Type).GetAnimation(AnimationInfo.IDLE).Line,
			        championsInfo.GetInfo(champ.Type).GetAnimation(AnimationInfo.IDLE).FrameRate,
			        championsInfo.GetInfo(champ.Type).GetAnimation(AnimationInfo.IDLE).FrameCount)
        {
			champion = champ;
			information = championsInfo.GetInfo(champion.Type);
        }

		/// <summary>
		/// Plaies the animation.
		/// If the nimation doesn't exist, this call is ignored;
		/// </summary>
		/// <param name="name">Name.</param>
		public void PlayAnimation(string name)
		{
			AnimationInfo anim = information.GetAnimation(name);
			if (anim!=null) {
				Line = anim.Line;
				FrameCount = anim.FrameCount;
				FrameRate = anim.FrameRate;
				CurrentFrame = 0;
			}
		}
    }
}

