//
//  CameraService.cs
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

namespace GameContent
{
    public class CameraService
    {
		const float CAM_LERP_FACTOR = 0.1f;
		const float BOTTOM_MAX_LIMIT_EXTEND = 0.3f; // When we're at the bottom, we put the camera a bit below the map to see the gameplay even with the UI
		/// <summary>
		/// Top-left position of the screen within the world.
		/// </summary>
		/// <value>The world position.</value>
		public Vec2 WorldPosition { get; private set; }

        public CameraService()
        {
			WorldPosition = new Vec2();
        }

		public void CenterCameraTowards(Vec2 position, float screenWidth, float screenHeight, float worldWidth, float worldHeight)
		{
			var before = WorldPosition;
			WorldPosition = new Vec2(position.X - screenWidth / 2f, position.Y - screenHeight / 2f);
			WorldPosition.X = Math.Min(worldWidth - screenWidth, Math.Max(0f, WorldPosition.X));
			WorldPosition.Y = Math.Min(worldHeight - screenHeight * (1f - BOTTOM_MAX_LIMIT_EXTEND), Math.Max(0f, WorldPosition.Y));

			WorldPosition = Vec2.Lerp(before, WorldPosition, CAM_LERP_FACTOR);
		}

		public Vec2 ToWorld(Vec2 screenPos)
		{
			return screenPos + WorldPosition;
		}
		public Vec2 ToScreen(Vec2 worldPos)
		{
			return worldPos - WorldPosition;
		}
    }
}

