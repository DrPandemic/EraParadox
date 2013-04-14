//
//  Drawable.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GREATClient
{
    public abstract class Drawable : IDraw
    {

		/// <summary>
		/// Gets the texture.
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture { get; protected set; }

		/// <summary>
		/// Gets or sets the alpha.
		/// 0 to 1
		/// </summary>
		/// <value>The alpha.</value>
		public float Alpha { get; set; }

		/// <summary>
		/// Gets or sets the orientation.
		/// </summary>
		/// <value>The orientation.</value>
		public float Orientation { get; set; }

		/// <summary>
		/// Gets or sets the source rectangle.
		/// </summary>
		/// <value>The source rectangle.</value>
		public Rectangle? SourceRectangle { get; set; }

		/// <summary>
		/// Gets or sets the scale.
		/// 1 = normal size
		/// </summary>
		/// <value>The scale.</value>
		public Vector2 Scale { get; set; }

		/// <summary>
		/// Gets or sets the origin relative.
		/// The range is from 0 to 1
		/// Instead of pixels
		/// </summary>
		/// <value>The origin relative.</value>
		public Vector2 OriginRelative { get; set; }

		/// <summary>
		/// Gets or sets the tint.
		/// </summary>
		/// <value>The tint.</value>
		public Color Tint { get; set; }

		/// <summary>
		/// Gets or sets the effects.
		/// </summary>
		/// <value>The effects.</value>
		public SpriteEffects Effects { get; set; }
		protected Drawable()
		{
			Tint = Color.White;
			Texture = null;
			Position = new Vector2(0,0);
			Alpha = 1f;
			Orientation = 0f;
			SourceRectangle = null;
			Scale = new Vector2(1,1);
			OriginRelative = new Vector2(0.5f,0.5f);
			Effects = SpriteEffects.None;
		}

		/// <summary>
		/// Unloads the image of the object.
		/// </summary>
		protected override void OnUnload()
		{
			base.OnUnload();
			Texture.Dispose();
		}

		/// <summary>
		/// Draw the object.
		/// </summary>
		/// <param name="batch">The spritebatch used to draw the object.</param>
		public override void Draw(SpriteBatch batch)
		{
			//If the texture wasn't load, beacause of the order of layer add
			if(Texture == null)
				OnLoad(Parent.Content,Parent.GetGraphics());

		}


    }
}

