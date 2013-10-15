//
//  ClientLinearSpell.cs
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

namespace GREATClient.Network
{
    public class ClientLinearSpell
    {
		public bool Active { get; set; }
		public ulong ID { get; private set; }
		float Time { get; set; }
		public Vec2 Velocity { get; private set; }
		public Vec2 Position { get; private set; }
		float Range { get; set; }
		float Width { get; set; }
		Vec2 StartingPosition { get; set; }

		public ClientLinearSpell(ulong id, Vec2 pos, float time, Vec2 velocity, float range, float width)
        {
			ID = id;
			StartingPosition = pos;
			Position = StartingPosition;
			Velocity = velocity;
			Time = Math.Max(0f, (float)Client.Instance.GetTime().TotalSeconds - time);
			Active = true;
			Range = range;
			Width = width;
        }

		public void Update(double dt)
		{
			Time += (float)dt;
			Position = StartingPosition + Velocity * Time;
			if (Vec2.DistanceSquared(Position, StartingPosition) > Range * Range) {
				Active = false;
			}
		}
    }
}

