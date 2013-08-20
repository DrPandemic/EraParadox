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

namespace GREATLib.Entities
{
	/// <summary>
	/// Represents a shared entity between the server and the client, e.g. a champion.
	/// </summary>
    public class IEntity : ICloneable
    {
		/// <summary>
		/// The unique ID of the entity.
		/// </summary>
		/// <value>The I.</value>
		public uint ID { get; private set; }

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
		/// Gets or sets a value indicating whether this instance is on ground or not.
		/// Mainly used to handle jumping and apply certain accelerations.
		/// </summary>
		public bool IsOnGround { get; set; }

		/// <summary>
		/// Gets or sets the jump force of the entity.
		/// </summary>
		public short JumpForce { get; set; }
		/// <summary>
		/// Gets or sets the horizontal acceleration of the entity, which is how much of our horizontal velocity
		/// we maintain per second.
		/// </summary>
		/// <example>0.9 would keep 90% of the entity's X velocity every frame.</example>
		public float HorizontalAcceleration { get; set; }

		/// <summary>
		/// Gets or sets the width of the collision rectangle of the entity.
		/// </summary>
		public float CollisionWidth { get; set; }
		/// <summary>
		/// Gets or sets the height of the collision rectangle of the entity.
		/// </summary>
		public float CollisionHeight { get; set; }

		/// <summary>
		/// Gets the direction of the entity during the current frame.
		/// </summary>
		public HorizontalDirection Direction { get; set; }

		/// <summary>
		/// Gets or sets the simulated position of the entity.
		/// On the server, this is the real position of our entities.
		/// On the client, if this is the main champion, this is our
		/// currently simulated position (our local client-side predicted
		/// version).
		/// </summary>
		public Vec2 Position { get; set; }

        public IEntity(uint id, Vec2 startingPosition)
        {
			//TODO: depend on who the champion is
			MoveSpeed = 100f;
			CollisionWidth = 15f;
			CollisionHeight = 30f;
			JumpForce = 750;
			HorizontalAcceleration = 9e-9f;

			ID = id;
			Position = startingPosition;

			Velocity = new Vec2();
			Direction = HorizontalDirection.None;
			IsOnGround = true;
        }

		/// <summary>
		/// Creates the rectangle that represents the collision rectangle of the entity.
		/// </summary>
		public Rect CreateCollisionRectangle()
		{
			return new Rect(Position.X, Position.Y, CollisionWidth, CollisionHeight);
		}

		public object Clone()
		{
			IEntity clone = (IEntity)this.MemberwiseClone();
			clone.Position = (Vec2)Position.Clone();
			clone.Velocity = (Vec2)Velocity.Clone();
			return clone;
		}
    }
}

