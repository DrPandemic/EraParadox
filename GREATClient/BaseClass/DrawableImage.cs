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

namespace GREATClient
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
			if(content != null && gd != null)
				Texture = content.Load<Texture2D>(FileName);
		}

		/// <summary>
		/// Draw the object.
		/// </summary>
		/// <param name="batch">The spritebatch used to draw the object.</param>
		protected override void OnDraw(SpriteBatch batch)
		{

			batch.Begin();
			batch.Draw(Texture,GetAbsolutePosition(),SourceRectangle,Tint,Orientation,
			           OriginRelative * new Vector2(Texture.Width, Texture.Height),Scale,Effects,0);
			batch.End();

		}
    }
}

