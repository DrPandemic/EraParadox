//
//  DrawableTriangle.cs
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

namespace GREATClient
{
    public class DrawableTriangle : DrawableImage
    {
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.DrawableTriangle"/> is ascendant.
		/// </summary>
		/// <value><c>true</c> if ascendant; otherwise, <c>false</c>.</value>
		public bool Ascendant { 
			get { return Effects == SpriteEffects.FlipHorizontally; }
			set {  Effects = value ? SpriteEffects.FlipHorizontally :SpriteEffects.None; }
		}

		public DrawableTriangle(bool ascendant) : base("triangle")
        {
			Ascendant=ascendant;
        }
    }
}

