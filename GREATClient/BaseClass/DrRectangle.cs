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

namespace GREATClient
{
    public class DrRectangle : Drawable
    {
		/// <summary>
		/// Gets or sets the size of the index element.
		/// </summary>
		/// <value>The size of the index element.</value>
		public Vector2 Size { get; set; }

		public DrRectangle() : this(new Vector2(1,1),new Vector2(0,0),Color.White)
		{

		}
		public DrRectangle(Vector2 size, Vector2 position, Color tint) : base()
        {
			Size = size;
			Position = position;
			Tint = tint;
        }
		protected override void OnLoad(ContentManager content, GraphicsDevice gd)
		{
			Texture = new Texture2D(gd,1,1);
			Texture.SetData(new Color[] { Tint });
		}
		public override void Draw(SpriteBatch batch)
		{
			base.Draw(batch);
			batch.Begin();
			batch.Draw(Texture,new Rectangle((int)GetAbsolutePosition().X,(int)GetAbsolutePosition().Y,(int)Size.X,(int)Size.Y),Tint);			
			batch.End();
		}
    }
}

