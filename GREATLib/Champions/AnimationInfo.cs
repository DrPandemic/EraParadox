//
//  AnimationInfo.cs
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

namespace Champions
{
	/// <summary>
	/// Represents information about a certain animation.
	/// </summary>
    public class AnimationInfo
    {
		/// <summary>
		/// Gets the frame rate.
		/// </summary>
		/// <value>The frame rate.</value>
		public int FrameRate { get; private set; }

		/// <summary>
		/// Gets the frame count.
		/// </summary>
		/// <value>The frame count.</value>
		public int FrameCount { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Champions.AnimationInfo"/> class.
		/// </summary>
        public AnimationInfo(int frameRate, int frameCount)
        {
			FrameRate = frameRate;
			FrameCount = frameCount;
        }
    }
}

