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
using System.Diagnostics;
using GREATClient.BaseClass;

namespace GREATClient.GameContent
{
    public class DrawableChampionSprite : DrawableSprite
    {
		/// <summary>
		/// Gets or sets the information about a specific champion.
		/// </summary>
		/// <value>The infos.</value>
		ChampionInfo Information { get; set; }

		/// <summary>
		/// Gets or sets the current animation.
		/// </summary>
		/// <value>The current animation.</value>
		public ChampionAnimation CurrentAnimation { get; set; }

		public DrawableChampionSprite(ChampionTypes type, ChampionsInfo championsInfo)
			: base (championsInfo.GetInfo(type).AssetName,
			        championsInfo.GetInfo(type).FrameWidth,
			        championsInfo.GetInfo(type).FrameHeight,
			        championsInfo.GetInfo(type).GetAnimation(ChampionAnimation.idle).Line,
			        championsInfo.GetInfo(type).GetAnimation(ChampionAnimation.idle).FrameRate,
			        championsInfo.GetInfo(type).GetAnimation(ChampionAnimation.idle).FrameCount)
        {
			Information = championsInfo.GetInfo(type);
			CurrentAnimation = ChampionAnimation.idle;
        }

		/// <summary>
		/// Plays the animation.
		/// If the animation doesn't exist, this call is ignored;
		/// </summary>
		/// <param name="name">Name.</param>
		public void PlayAnimation(ChampionAnimation name)
		{
			AnimationInfo anim = Information.GetAnimation(name);

			Debug.Assert(anim != null, "The animation does not exist.");

			if (anim!=null) {
				CurrentAnimation = name;
				Line = anim.Line;
				FrameCount = anim.FrameCount;
				FrameRate = anim.FrameRate;
				CurrentFrame = 0;
			}
		}
    }
}

