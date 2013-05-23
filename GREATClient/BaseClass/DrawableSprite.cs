//
//  DrawableSprite.cs
//
//  Author:
//       The Parasithe <bipbip500@hotmail.com>
//
//  Copyright (c) 2013 The Parasithe
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Microsoft.Xna.Framework;



namespace GREATClient
{
    public class DrawableSprite : DrawableImage
    {
		const int INFINITE = -1;


		/// <summary>
		/// Gets or sets the width of a frame.
		/// </summary>
		/// <value>The width of the frame.</value>
		int frameWidth;
		int FrameWidth 
		{ 
			get { return frameWidth; }
			set
			{
				Debug.Assert(value > 0, "The frame width must be positive.");
				frameWidth = value;
			}
		}

		/// <summary>
		/// Gets or sets the height of a frame.
		/// </summary>
		/// <value>The height of the frame.</value>
		int frameHeight;
		int FrameHeight 
		{ 
			get { return frameHeight; }
			set
			{
				Debug.Assert(value > 0, "The frame height must be positive.");
				frameHeight = value;
			}
		}

		/// <summary>
		/// Gets or sets the line where the animation is on the spritesheet
		/// </summary>
		/// <value>The line.</value>
		int line;
		int Line 
		{ 
			get { return line; }
			set 
			{
				Debug.Assert(value >= 0, "The line must be zero or greater");
				line = value;
			}
		
		}

		/// <summary>
		/// Gets or sets the frame rate.
		/// </summary>
		/// <value>The frame rate.</value>
		float frameRate;
		float FrameRate 
		{ 
			get { return frameRate; }
			set
			{
				Debug.Assert(value >= 0 , "The line must be zero or greater");
				frameRate = value;
			}
		}

		/// <summary>
		/// Gets or sets the frame count.
		/// </summary>
		/// <value>The frame count.</value>
		int frameCount;
		int FrameCount 
		{ 
			get { return frameCount; } 
			set
			{
				Debug.Assert(value > 0, "The frame count");
				frameCount = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is playing.
		/// </summary>
		/// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
		bool IsPlaying { get; set; }

		/// <summary>
		/// Gets or sets the repetition count.
		/// Every negative number will represent infinite
		/// </summary>
		/// <value>The repetition count.</value>
		int RepetitionCount { get; set; }

		/// <summary>
		/// Gets or sets the until next animation.
		/// </summary>
		/// <value>The until next animation.</value>
		TimeSpan UntilNextAnim { get; set; }

		/// <summary>
		/// Gets or sets the time by frame.
		/// </summary>
		/// <value>The time by frame.</value>
		TimeSpan TimeByFrame { get; set; }

		/// <summary>
		/// Gets or sets the current frame.
		/// </summary>
		/// <value>The current frame.</value>
		int CurrentFrame  { get; set; }


		public DrawableSprite(string assetName, int frameWidth, int frameHeight, int line, float frameRate, int frameCount, int repetitionCount = 1, bool autoStart = true)
			: base(assetName)
        {
			FrameWidth = frameWidth;
			FrameHeight = frameHeight;
			Line = line;
			FrameRate = frameRate;
			FrameCount = frameCount;
			RepetitionCount = repetitionCount;

			IsPlaying = false;
        }
	
		/// <summary>
		/// Start the animation
		/// </summary>
		public void Play()
		{
			IsPlaying = true;

		}

		/// <summary>
		/// Stop this animation
		/// </summary>
		public void Stop()
		{
			IsPlaying = false;
		}


		private void CalculateNextTick()
		{
			//TODO calculer le nobre de temps que dure une frame
		}

		protected override void OnUpdate(GameTime dt)
		{
			if(IsPlaying)
			{
				//TODO Regarder quand on doit changer de frame
			}
		}

		protected override void OnDraw(SpriteBatch batch)
		{
		}
    }
}

