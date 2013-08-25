//
//  DrImage.cs
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace GREATClient.BaseClass
{
	/// <summary>
	/// Dr. image.
	/// </summary>
    public class DrawableImage : Drawable
    {
		/// <summary>
		/// Gets the name of the image file.
		/// </summary>
		/// <value>The name of the file.</value>
		public string FileName { get; private set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.DrawableImage"/> is fliped in x.
		/// </summary>
		/// <value><c>true</c> if flip x; otherwise, <c>false</c>.</value>
		bool flipX;
		public bool FlipX 
		{ 
			get { return flipX; } 
			set 
			{
				flipX = value;
				Effects = (value ? SpriteEffects.FlipHorizontally : SpriteEffects.None );
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		protected virtual int Width { get { return Texture.Width; } }

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		protected virtual int Height { get { return Texture.Height; } } 

        public DrawableImage(string file) : base()
        {
			FileName = file;
        }

		/// <summary>
		/// Loads the image of the object.
		/// </summary>
		/// <param name="content">Content.</param>
		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			if (content != null && gd != null) {
				Texture = content.Load<Texture2D>(FileName);
			}
		}

		/// <summary>
		/// Draw the object.
		/// </summary>
		/// <param name="batch">The spritebatch used to draw the object.</param>
		protected override void OnDraw(SpriteBatch batch)
		{

			batch.Begin();

			batch.Draw(Texture,GetAbsolutePosition(),SourceRectangle,Tint * Alpha,(float)Orientation,
			           RelativeOrigin * new Vector2(Width, Height),Scale,Effects,0);

			batch.End();

		}
    }
}

