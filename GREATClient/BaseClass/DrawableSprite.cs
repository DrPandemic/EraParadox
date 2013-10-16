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



namespace GREATClient.BaseClass
{
    public class DrawableSprite : DrawableImage
    {
		public const int INFINITE = -1;


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
		protected int Line 
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
		protected float FrameRate 
		{ 
			get { return frameRate; }
			set
			{
				Debug.Assert(value >= 0 , "The line must be zero or greater");
				frameRate = value;
				CalculateTimeByFrame();
			}
		}

		/// <summary>
		/// Gets or sets the frame count.
		/// </summary>
		/// <value>The frame count.</value>
		int frameCount;
		protected int FrameCount 
		{ 
			get { return frameCount; } 
			set
			{
				Debug.Assert(value > 0, "The frame count must be more than 0");
				frameCount = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is playing.
		/// </summary>
		/// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
		protected bool IsPlaying { get; set; }

		/// <summary>
		/// Gets or sets the repetition count.
		/// Every negative number will represent infinite
		/// </summary>
		/// <value>The repetition count.</value>
		protected int RepetitionCount { get; set; }

		/// <summary>
		/// Gets or sets the time to now when to change of anim
		/// </summary>
		/// <value>The until next animation.</value>
		protected TimeSpan UntilNextAnim { get; set; }

		/// <summary>
		/// Gets or sets the time by frame.
		/// </summary>
		/// <value>The time by frame.</value>
		protected TimeSpan TimeByFrame { get; set; }

		/// <summary>
		/// Gets or sets the current frame.
		/// </summary>
		/// <value>The current frame.</value>
		int currentFrame;
		protected int CurrentFrame  
		{ 
			get { return currentFrame; }
			set
			{
				Debug.Assert(value >= 0, "The current frame must be 0 or more");
				currentFrame = value;
				GenerateSourceRectangle();
			}
		}

		protected override int Width { get { return FrameWidth; } }
		protected override int Height { get { return FrameHeight; } }


		public DrawableSprite(string assetName, int frameWidth, int frameHeight, int line, float frameRate, int frameCount, int repetitionCount = INFINITE, bool autoStart = true)
			: base(assetName)
        {
			FrameWidth = frameWidth;
			FrameHeight = frameHeight;
			Line = line;
			FrameRate = frameRate;
			FrameCount = frameCount;
			RepetitionCount = repetitionCount;

			IsPlaying = autoStart;

			UntilNextAnim = TimeSpan.FromSeconds(0);
			CurrentFrame = 0;
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

		/// <summary>
		/// Calculates the time by frame with the frame rate.
		/// </summary>
		private void CalculateTimeByFrame()
		{
			Debug.Assert(FrameRate > 0, "The frame rate must be positive.");
			TimeByFrame = TimeSpan.FromMilliseconds(1000.0/FrameRate);
		}

		/// <summary>
		/// Generates the source rectangle with the current frame, line, width and height
		/// </summary>
		private void GenerateSourceRectangle()
		{
			SourceRectangle = new Rectangle(CurrentFrame * FrameWidth, Line * FrameHeight, FrameWidth, FrameHeight);
		}

		protected override void OnUpdate(GameTime dt)
		{
			if(IsPlaying && FrameRate != 0 && (RepetitionCount != 0))
			{
				UntilNextAnim = UntilNextAnim.Add(dt.ElapsedGameTime);

				if (UntilNextAnim > TimeByFrame) {
					UntilNextAnim = UntilNextAnim.Subtract(TimeByFrame);
					CurrentFrame++;

					if (CurrentFrame >= FrameCount) {
						CurrentFrame = 0;

						if (RepetitionCount > 0)
							RepetitionCount--;

						if (RepetitionCount == 0) { // if we are done animating, stay on the last frame
							CurrentFrame = FrameCount - 1;
							Console.WriteLine("THE END!" + RepetitionCount);
						}
					}
				}
			}
		}
    }
}

