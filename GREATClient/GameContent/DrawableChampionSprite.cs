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

namespace GREATClient
{
    public class DrawableChampionSprite : DrawableSprite
    {
		/// <summary>
		/// Gets or sets the information about a specific champion.
		/// </summary>
		/// <value>The infos.</value>
		ChampionInfo Information { get; set; }

		public DrawableChampionSprite(ChampionTypes type, ChampionsInfo championsInfo)
			: base (championsInfo.GetInfo(type).AssetName,
			        championsInfo.GetInfo(type).FrameWidth,
			        championsInfo.GetInfo(type).FrameHeight,
			        championsInfo.GetInfo(type).GetAnimation(AnimationInfo.IDLE).Line,
			        championsInfo.GetInfo(type).GetAnimation(AnimationInfo.IDLE).FrameRate,
			        championsInfo.GetInfo(type).GetAnimation(AnimationInfo.IDLE).FrameCount)
        {
			Information = championsInfo.GetInfo(type);
        }

		/// <summary>
		/// Plays the animation.
		/// If the animation doesn't exist, this call is ignored;
		/// </summary>
		/// <param name="name">Name.</param>
		public void PlayAnimation(string name)
		{
			Debug.Assert(!String.IsNullOrWhiteSpace(name), "No animation specified");

			AnimationInfo anim = Information.GetAnimation(name);

			Debug.Assert(anim != null, "The animation does not exist.");

			if (anim!=null) {
				Line = anim.Line;
				FrameCount = anim.FrameCount;
				FrameRate = anim.FrameRate;
				CurrentFrame = 0;
			}
		}
    }
}

