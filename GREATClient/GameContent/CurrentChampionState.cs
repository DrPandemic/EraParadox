//
//  CurrentChampionState.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
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

namespace GameContent
{
    public class CurrentChampionState
    {
		/// <summary>
		/// Gets or sets the max life.
		/// </summary>
		/// <value>The max life.</value>
		public int MaxLife { get; set; }

		/// <summary>
		/// Gets or sets the current life.
		/// </summary>
		/// <value>The current life.</value>
		public int CurrentLife { get; set; }

		/// <summary>
		/// Gets or sets the max resource.
		/// The resource is the mana/energie/etc.
		/// </summary>
		/// <value>The max resource.</value>
		public int MaxResource { get; set; }

		/// <summary>
		/// Gets or sets the current resource.
		/// The resource is the mana/energie/etc.
		/// </summary>
		/// <value>The current resource.</value>
		public int CurrentResource { get; set; }

        public CurrentChampionState(int maxLife, int maxResource)
        {
			MaxLife = maxLife;
			CurrentLife = maxLife;
			MaxResource = maxResource;
			CurrentResource = maxResource;
        }
    }
}

