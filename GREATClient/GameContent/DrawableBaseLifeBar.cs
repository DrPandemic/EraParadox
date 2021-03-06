//
//  DrawableBaseLifeBar.cs
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
	public class DrawableBaseLifeBar : DrawableChampionLifeBar
    {

		override protected float CONTOUR_WIDTH { get { return 1f;}}
		override protected float MAX_WIDTH { get { return 100f;}}
		override protected float NORMAL_HEIGHT { get { return 10f;}}

		public DrawableBaseLifeBar(bool isAlly) : base(isAlly)
        {
        }
    }
}

