//
//  Container.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace GREATClient
{
    public class Container : IDraw
    {
		/// <summary>
		/// Gets or sets the content manager.
		/// </summary>
		/// <value>The content.</value>
		public ContentManager Content { get; private set; }
		/// <summary>
		/// Gets or sets the children of the container.
		/// </summary>
		/// <value>The children.</value>
		protected List<IDraw> Children { get; set; }

		/// <summary>
		/// A list of the children that want to be removed. This is used because it is impossible
		/// to remove an object from a collection while iterating it (e.g. remove an object in its Update method).
		/// Instead, we remove the items
		/// when the frame ends.
		/// </summary>
		private List<IDraw> toRemove = new List<IDraw>();

		/// <summary>
		/// Gets the graphics.
		/// Only the screen hold the reference to the object
		/// </summary>
		/// <returns>The graphics.</returns>
		public virtual GraphicsDevice GetGraphics()
		{
			if(Parent != null)
				return Parent.GetGraphics();
			return null;
		}

		public Container(ContentManager content) 
        {
			Content = content;
			Children = new List<IDraw>();
			toRemove = new List<IDraw>();
        }

		/// <summary>
		/// Adds the child with a specified z-index.
		/// </summary>
		/// <param name="child">Child.</param>
		/// <param name="z">The z coordinate.</param>
		public void AddChild(IDraw child, int z)
		{
			if(child.Parent == null)
			{
				child.Load(this, GetGraphics());
				child.Z=z;
				int index = Children.FindIndex(c => child.Z > c.Z);
				if(index==-1)
				
					index = Children.FindLastIndex(c => child.Z == c.Z);
				
				++index;

				Children.Insert(Children.Count == 0 ? 0 :index ,child);
			}
			else
				throw new Exception("The IDraw is already in a container");
		}

		/// <summary>
		/// Adds the child with a default z-index of 0.
		/// </summary>
		/// <param name="child">Child.</param>
		public void AddChild(IDraw child)
		{
			AddChild(child, 0);
		}

		/// <summary>
		/// Removes the specified child.
		/// </summary>
		/// <param name="child">Child.</param>
		public void RemoveChild(IDraw child)
		{
			toRemove.Add(child);
			child.UnLoad();
		}

		/// <summary>
		/// Update the container and its children.
		/// </summary>
		/// <param name="dt">Delta time (time since the last frame).</param>
		protected override void OnUpdate(GameTime dt)
		{
			Children.ForEach(child => child.Update(dt));

			// We remove the objects that we should
			toRemove.ForEach(child => Children.Remove(child));
			toRemove.Clear();
		}

		/// <summary>
		/// Draw the container and its children.
		/// </summary>
		/// <param name="batch">The spritebatch, used in the drawing process.</param>
		protected override void OnDraw(SpriteBatch batch)
		{
			Children.ForEach(child => child.Draw(batch));
		}
    }
}

