//
//  IEntity.cs
//
//  Author:
//       Jesse <jesse.emond@hotmail.com>
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
using GREATLib;
using GREATLib.Entities.Champions;

namespace GREATLib.Entities
{
	/// <summary>
	/// Represents a shared entity between the server and the client, e.g. a champion.
	/// </summary>
    public class IEntity : ICloneable
    {
		public const float MAX_SPEED = 1500f;

		/// <summary>
		/// The unique ID of the entity.
		/// </summary>
		/// <value>The I.</value>
		public ulong ID { get; private set; }

		/// <summary>
		/// Gets or sets the velocity of the entity.
		/// </summary>
		public Vec2 Velocity { get; set; }
		/// <summary>
		/// Gets or sets the movement speed of the entity.
		/// </summary>
		/// <value>The move speed.</value>
		public float MoveSpeed { get; set; }

		/// <summary>
		/// Gets or sets the width of the collision rectangle of the entity.
		/// </summary>
		public float CollisionWidth { get; set; }
		/// <summary>
		/// Gets or sets the height of the collision rectangle of the entity.
		/// </summary>
		public float CollisionHeight { get; set; }

		/// <summary>
		/// Gets or sets the simulated position of the entity.
		/// On the server, this is the real position of our entities.
		/// On the client, if this is the main champion, this is our
		/// currently simulated position (our local client-side predicted
		/// version).
		/// </summary>
		public Vec2 Position { get; set; }

        public IEntity(ulong id, Vec2 startingPosition,
		               float moveSpeed, float width, float height)
        {
			ID = id;
			Position = startingPosition;
			MoveSpeed = moveSpeed;
			CollisionWidth = width;
			CollisionHeight = height;

			Velocity = new Vec2();
        }

		/// <summary>
		/// Clone the specified entity to copy its values.
		/// </summary>
		public virtual void Clone(IEntity e)
		{
			CollisionWidth = e.CollisionWidth;
            CollisionHeight = e.CollisionHeight;
            ID = e.ID;
            MoveSpeed = e.MoveSpeed;
            Position = e.Position.Clone() as Vec2;
            Velocity = e.Velocity.Clone() as Vec2;
		}

		/// <summary>
		/// Creates the rectangle that represents the collision rectangle of the entity.
		/// </summary>
		public Rect CreateCollisionRectangle()
		{
			return new Rect(Position.X, Position.Y, CollisionWidth, CollisionHeight);
		}

		public virtual object Clone()
		{
			IEntity clone = new IEntity(ID, Position, MoveSpeed, CollisionWidth, CollisionHeight);
			clone.Clone(this);
			return clone;
		}
    }
}

