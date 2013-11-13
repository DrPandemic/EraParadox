//
//  ChampionTypes.cs
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
using System;
using GREATLib.Entities.Spells;
using GREATLib.Network;

namespace GREATLib.Entities.Champions
{
	public enum ChampionAnimation
	{
		run,
		idle,
		die,
		jump,
		spell1,
		spell2,
		spell3,
		spell4
	}

	public enum ChampionTypes
	{
		ManMega,
		Zoro
	}

	public static class ChampionTypesHelper
	{
		// Note: It is VERY important that the spell # here is the same as the one in the SpellTypes enum (see SpellTypesHelper).
		// PLEASE MAKE SURE TO MODIFY THEM AT BOTH PLACES.
		public static SpellTypes GetSpellFromAction(ChampionTypes type, PlayerActionType action)
		{
			switch (type) {
				case ChampionTypes.ManMega:
					switch (action) {
						case PlayerActionType.Spell1: return SpellTypes.ManMega_RocketRampage;
						case PlayerActionType.Spell2: return SpellTypes.ManMega_Slash;
						case PlayerActionType.Spell3: return SpellTypes.ManMega_HintOfASpark;
						case PlayerActionType.Spell4: return SpellTypes.ManMega_Shotgun;
					}
					break;

				case ChampionTypes.Zoro:
					switch (action) {
						case PlayerActionType.Spell1: return SpellTypes.Zoro_Tooth;
						case PlayerActionType.Spell2: return SpellTypes.Zoro_Slash;
						case PlayerActionType.Spell3: return SpellTypes.Zoro_Double;
						case PlayerActionType.Spell4: return SpellTypes.Zoro_Wall;
					}
					break;

					default:
					ILogger.Log("Champion type not implemented " + type + ".");
					break;
			}

			return SpellTypes.ManMega_RocketRampage; // Unknown spell: use one by default
		}
	}
}

