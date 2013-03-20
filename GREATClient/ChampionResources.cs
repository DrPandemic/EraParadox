//
//  ChampionResources.cs
//
//  Author:
//       Jesse <${AuthorEmail}>
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
using Microsoft.Xna.Framework.Graphics;
using Champions;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GREATLib;
using System.Diagnostics;

namespace GREATClient
{
	/// <summary>
	/// Manages the champion resources.
	/// </summary>
    public class ChampionResources
    {
		/// <summary>
		/// The data of a loaded champion.
		/// </summary>
		private struct ChampionInfo
		{
			/// <summary>
			/// The data of the champion itself.
			/// </summary>
			public Champion Data { get; set; }
			
			/// <summary>
			/// The texture of the standing animation.
			/// </summary>
			public Texture2D Standing { get; set; }
			
			/// <summary>
			/// The texture of the running animation.
			/// </summary>
			public Texture2D Running { get; set; }
		}


		/// <summary>
		/// Gets or sets the champions that have been loaded.
		/// </summary>
		/// <value>The loaded champions.</value>
		Dictionary<ChampionTypes, ChampionInfo> LoadedChampions { get; set; }

		/// <summary>
		/// Gets or sets the content manager to load the assets.
		/// </summary>
		/// <value>The content.</value>
		ContentManager content { get; set; }

        public ChampionResources(ContentManager content)
        {
			LoadedChampions = new Dictionary<ChampionTypes, ChampionInfo>();
			this.content = content;
        }

		/// <summary>
		/// Loads the champion.
		/// </summary>
		/// <param name="type">The champion type.</param>
		private void LoadChampion(ChampionTypes type)
		{
			LoadedChampions.Add(type, LoadChampionInfo(type));
		}

		/// <summary>
		/// Loads the champion information.
		/// </summary>
		/// <returns>The champion info.</returns>
		/// <param name="type">Type.</param>
		private ChampionInfo LoadChampionInfo(ChampionTypes type)
		{
			const string STAND_SUFFIX = "_stand";
			const string RUN_SUFFIX = "_run";

			ChampionInfo champ = new ChampionInfo();
			champ.Data = ChampionFromType.GetChampion(type);
			champ.Standing = content.Load<Texture2D>(champ.Data.ContentName + STAND_SUFFIX);
			champ.Running = content.Load<Texture2D>(champ.Data.ContentName + RUN_SUFFIX);
			return champ;
		}

		/// <summary>
		/// Gets the champion from the type and loads it if it wasn't loaded yet.
		/// </summary>
		/// <returns>The champion.</returns>
		/// <param name="type">Type.</param>
		private ChampionInfo GetChampion(ChampionTypes type)
		{
			if (!LoadedChampions.ContainsKey(type))
				LoadChampion(type);
			return LoadedChampions[type];
		}

		/// <summary>
		/// Gets the champion animation information based on the
		/// type of the champion and the specified animation.
		/// </summary>
		/// <returns>The champion animation.</returns>
		/// <param name="type">Type.</param>
		/// <param name="animation">Animation.</param>
		private KeyValuePair<AnimationInfo, Texture2D> GetChampionAnimData(ChampionTypes type, PlayerAnimation animation)
		{
			AnimationInfo anim = null;
			Texture2D img = null;
			switch (animation) {
				case PlayerAnimation.Standing: 
					anim = GetChampion(type).Data.StandingAnim;
					img = GetChampion(type).Standing;
					break;
				case PlayerAnimation.Running: 
					anim = GetChampion(type).Data.RunningAnim;
					img = GetChampion(type).Running;
					break;
				default: throw new NotImplementedException("Animation not implemented while getting it in the resources.");
			}

			Debug.Assert(anim != null && img != null);

			return new KeyValuePair<AnimationInfo, Texture2D>(anim, img);
		}

		/// <summary>
		/// Gets the width of a single frame of an animation.
		/// </summary>
		/// <returns>The frame width.</returns>
		/// <param name="type">Champion type.</param>
		/// <param name="anim">Animation.</param>
		public int GetFrameWidth(ChampionTypes type, PlayerAnimation anim)
		{
			return GetTexture(type, anim).Width /
				GetChampionAnimData(type, anim).Key.FrameCount;
		}

		/// <summary>
		/// Gets the height of a single frame of an animation.
		/// </summary>
		/// <returns>The frame height.</returns>
		/// <param name="type">Champion type.</param>
		/// <param name="anim">Animation.</param>
		public int GetFrameHeight(ChampionTypes type, PlayerAnimation anim)
		{
			return GetTexture(type,anim).Height;
		}

		/// <summary>
		/// Gets the texture of the current animation of the champion.
		/// </summary>
		/// <returns>The texture.</returns>
		/// <param name="type">Champion type.</param>
		/// <param name="anim">Animation.</param>
		public Texture2D GetTexture(ChampionTypes type, PlayerAnimation anim)
		{
			return GetChampionAnimData(type, anim).Value;
		}

		/// <summary>
		/// Gets the frame count of an animation for a champion.
		/// </summary>
		/// <returns>The frame count.</returns>
		/// <param name="type">Type.</param>
		/// <param name="anim">Animation.</param>
		public int GetFrameCount(ChampionTypes type, PlayerAnimation anim)
		{
			return GetChampionAnimData(type, anim).Key.FrameCount;
		}

		/// <summary>
		/// Gets the frame rate of an animation for a champion.
		/// </summary>
		/// <returns>The frame rate.</returns>
		/// <param name="type">Type.</param>
		/// <param name="anim">Animation.</param>
		public int GetFrameRate(ChampionTypes type, PlayerAnimation anim)
		{
			return GetChampionAnimData(type, anim).Key.FrameRate;
		}

		/// <summary>
		/// Gets the source rectangle of the current animation of a champion.
		/// </summary>
		/// <returns>The source rect.</returns>
		/// <param name="type">Type.</param>
		/// <param name="anim">Animation.</param>
		/// <param name="frameId">The id of the current frame.</param>
		public Rectangle GetSourceRect(ChampionTypes type, PlayerAnimation anim, int frameId)
		{
			Debug.Assert(frameId >= 0 && frameId < GetFrameCount(type, anim));

			return new Rectangle(frameId * GetFrameWidth(type, anim),
			                     0,
			                     GetFrameWidth(type, anim), 
			                     GetFrameHeight(type, anim));
		}
    }
}

