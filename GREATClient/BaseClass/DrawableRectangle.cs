//
//  DrSquare.cs
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
using Microsoft.Xna.Framework;
using GREATLib;

namespace GREATClient
{
	/// <summary>
	/// Dr. Rectangle
	/// </summary>
    public class DrawableRectangle : Drawable
    {
		/// <summary>
		/// Gets or sets the size of the index element.
		/// </summary>
		/// <value>The size of the index element.</value>
		public Vector2 Size { get; set; }

		public DrawableRectangle(Rect rect, Color tint) 
			: this(new Rectangle(
				(int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height),
			       tint)
		{
		}
		public DrawableRectangle(Rectangle rect, Color tint) 
			: this(new Vector2(rect.Width, rect.Height),
			       new Vector2(rect.X, rect.Y), tint)
		{
		}

		public DrawableRectangle() : this(new Vector2(1,1),new Vector2(0,0),Color.White)
		{

		}
		public DrawableRectangle(Vector2 size, Vector2 position, Color tint) : base()
        {
			Size = size;
			Position = position;
			Tint = tint;
		
        }
		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			Texture = new Texture2D(gd,1,1);
			Texture.SetData(new Color[] { Color.White });
		}
		protected override void OnDraw(SpriteBatch batch)
		{
			batch.Begin();
			batch.Draw(Texture,new Rectangle((int)(GetAbsolutePosition().X-OriginRelative.X*Size.X),
			                                 (int)(GetAbsolutePosition().Y-OriginRelative.Y*Size.Y),
			                                 (int)Size.X,(int)Size.Y),
			           						Tint);		
			batch.End();
		}
    }
}

