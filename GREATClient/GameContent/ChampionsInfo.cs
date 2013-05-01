//
//  ChampionsInfo.cs
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
using System.Collections.Generic;
using GREATLib.Entities.Player.Champions;
using System.Diagnostics;

namespace GREATClient
{
	/// <summary>
	/// The information of an individual champion.
	/// </summary>
	public class ChampionInfo
	{
		public string Name { get; set; }
		public string AssetName { get; set; }
		public string Description { get; set; }

		public ChampionInfo(string name, string assetName, string description)
		{
			Name = name;
			AssetName = assetName;
			Description = description;
		}
	}

	/// <summary>
	/// The class holding various information about all the champions
	/// in the game. Whenever a new champion is added, this list must be updated.
	/// </summary>
    public class ChampionsInfo
    {
		private Dictionary<ChampionTypes, ChampionInfo> Info { get; set; }

        public ChampionsInfo()
        {
			FillInfo();
        }

		public ChampionInfo GetInfo(ChampionTypes champion)
		{
			Debug.Assert(Info.ContainsKey(champion));
			return Info[champion];
		}

		private void FillInfo()
		{
			Info = new Dictionary<ChampionTypes, ChampionInfo>();

			//TODO: read from a champions.xml file! Users will be able to rename
			// everything as they like and it will be much easier to manage (and update).
			Info.Add(ChampionTypes.StickMan,
			         new ChampionInfo("Stick Man", "Stickman",
			                 "The drawn man that escaped his blank sheet and wants to take revenge upon his creators. Long has he awaited for a mere taste of liberty to finally try and defeat his oppressors. He will use any tools at hand, pencils or erasers, to defeat his enemies. Do not underestimate the powers of a drawing. Fear the Stick Man."));
		}
    }
}

