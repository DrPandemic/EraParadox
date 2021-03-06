//
//  GameScore.cs
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

namespace GREATClient.GameContent
{
    public class GameScore
    {
		public int PlayerKills { get; set; }
		public int PlayerDeaths { get; set; }
		public int TeamKills { get; set; }
		public int TeamDeaths { get; set; }

        public GameScore()
        {
			PlayerKills = 0;
			PlayerDeaths = 0;
			TeamKills = 0;
			TeamDeaths = 0;
        }
    }
}

