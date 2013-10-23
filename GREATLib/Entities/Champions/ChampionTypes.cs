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
		ManMega
	}

	public static class ChampionTypesHelper
	{
		public static SpellTypes GetSpellFromAction(ChampionTypes type, PlayerActionType action)
		{
			switch (type) {
				case ChampionTypes.ManMega:
					switch (action) {
						case PlayerActionType.Spell3: return SpellTypes.ManMega_RocketRampage;
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

