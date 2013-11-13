//
//  DrawableTowerLifeBar.cs
//
//  Author:
//       HPSETUP3 <${AuthorEmail}>
//
//  Copyright (c) 2013 HPSETUP3
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
    public class DrawableTowerLifeBar : DrawableChampionLifeBar
    {
		override protected float CONTOUR_WIDTH { get { return 1f;}}
		override protected float MAX_WIDTH { get { return 75f;}}
		override protected float NORMAL_HEIGHT { get { return 5f;}}

        public DrawableTowerLifeBar(bool isAlly)
			: base(isAlly)
        {
        }
    }
}

