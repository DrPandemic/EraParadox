//
//  Champion.cs
//
//  Author:
//       Jesse <${AuthorEmail}>
//
//  Copyright (c) 2013 Jesse
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

namespace Champions
{
	/// <summary>
	/// Represents the data of a champion
	/// </summary>
    public abstract class Champion
    {
		/// <summary>
		/// Gets the name of the champion.
		/// </summary>
		/// <value>The name.</value>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the name of the content name of the champion (the file prefix
		/// to load it).
		/// </summary>
		/// <value>The name of the content.</value>
		public virtual string ContentName { get { return Name; } }

		/// <summary>
		/// Gets the information about the standing animation.
		/// </summary>
		/// <value>The standing animation.</value>
		public abstract AnimationInfo StandingAnim { get; }

		/// <summary>
		/// Gets the information about the running animation.
		/// </summary>
		/// <value>The running animation.</value>
		public abstract AnimationInfo RunningAnim { get; }

		/// <summary>
		/// Gets the width of the collision rectangle of the player.
		/// </summary>
		/// <value>The width of the collision rectangle.</value>
		public abstract int CollisionWidth { get; }

		/// <summary>
		/// Gets the height of the collision rectangle of the player.
		/// </summary>
		/// <value>The height of the collision rectangle.</value>
		public abstract int CollisionHeight { get; }
    }
}

