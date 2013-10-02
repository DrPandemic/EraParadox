//
//  Parallax.cs
//
//  Author:
//       Jean-Samuel Aubry-Guzzi <bipbip500@gmail.com>
//
//  Copyright (c) 2013 Jean-Samuel Aubry-Guzzi
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
using Microsoft.Xna.Framework;
using System.Linq;

namespace GREATClient.BaseClass
{
    public class Parallax : Container
    {
		/// <summary>
		/// Gets or sets the size of the world.
		/// </summary>
		/// <value>The size of the world.</value>
		public Vector2 WorldSize { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.BaseClass.Parallax"/> class.
		/// Be careful : If you put an image the same size as the world, the image won't move.
		/// </summary>
		/// <param name="worldSize">World size.</param>
		/// <param name="actions">Actions.</param>
		public Parallax(Vector2 worldSize, params Drawable[] actions)
        {
			WorldSize = worldSize;
			actions.OfType<Drawable>().ToList().ForEach((Drawable item) => AddChild(item));
        }

		/// <summary>
		/// Sets the position ratio of the camera in the world.
		/// Moves all the layers in the parallax.
		/// The value have to be between 0 and 1.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public void SetCurrentRatio(float x, float y)
		{
			x = x < 0 ? 0 : (x > 100 ? 100 : x);
			y = y < 0 ? 0 : (y > 100 ? 100 : y);
			Children.ForEach((IDraw item) => {
				item.Position = new Vector2((WorldSize.X - ((Drawable)item).Texture.Width) * x/100,
				                            (WorldSize.Y - ((Drawable)item).Texture.Height) * y/100);
			});
		}
    }
}

