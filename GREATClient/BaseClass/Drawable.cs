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

namespace GREATClient.BaseClass
{
    public abstract class Drawable : IDraw
    {

		/// <summary>
		/// Gets or sets a value indicating whether this instance is loaded.
		/// </summary>
		/// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
		protected bool IsLoaded { get; set; }

		/// <summary>
		/// Gets the texture.
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture { get; protected set; }

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
		/// Gets or sets the relative relative.
		/// The range is from 0 to 1
		/// Instead of pixels
		/// </summary>
		/// <value>The origin relative.</value>
		public Vector2 RelativeOrigin { get; set; }

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

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.Drawable"/> class.
		/// </summary>
		protected Drawable()
		{
			Tint = Color.White;
			Texture = null;
			Position = new Vector2(0,0);
			Alpha = 1f;
			Orientation = 0f;
			SourceRectangle = null;
			Scale = new Vector2(1,1);
			RelativeOrigin = new Vector2(0,0);
			Effects = SpriteEffects.None;
			IsLoaded = false;
		}

		/// <summary>
		/// Unloads the image of the object.
		/// </summary>
		protected override void OnUnload()
		{
			base.OnUnload();
		}

		/// <summary>
		/// Draw the object.
		/// </summary>
		/// <param name="batch">The spritebatch used to draw the object.</param>
		public override void Draw(SpriteBatch batch)
		{
			//If the texture wasn't loaded, because of the order that the layer was added
			if (Texture == null && !IsLoaded) {
				Load(Parent, Parent.GetGraphics());
			}

			base.Draw(batch);
		}

		public override bool IsBehind(Vector2 position)
		{
			return false;
		}
    }
}

