//
//  ScreenService.cs
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

namespace GREATClient.BaseClass
{
    public class ScreenService
    {
		/// <summary>
		/// Gets or sets the graphics device manager.
		/// Is use to change the screen resolution / fullscreen.
		/// </summary>
		/// <value>The m_ graphics device manager.</value>
		GraphicsDeviceManager m_GraphicsDeviceManager { get; set; }

		/// <summary>
		/// Gets the size of the physical screen not the game window.
		/// </summary>
		/// <value>The size of the screen.</value>
		public Vector2 ScreenSize
		{
			get {
				return new Vector2(m_GraphicsDeviceManager.GraphicsDevice.DisplayMode.Width,
				                   m_GraphicsDeviceManager.GraphicsDevice.DisplayMode.Height);
			}
		}

		/// <summary>
		/// Gets or sets the size of the game window.
		/// </summary>
		/// <value>The size of the game window.</value>
		public Vector2 GameWindowSize
		{
			get {
				return new Vector2(m_GraphicsDeviceManager.GraphicsDevice.Viewport.Width,
				                   m_GraphicsDeviceManager.GraphicsDevice.Viewport.Height);
			}
			set {
				NextWindowSize = value;
				WindowSizeChanged = true;
			}
		}

		/// <summary>
		/// Gets or sets the next size of the window.
		/// Is used to update the PreferredBackBuffer in the update.
		/// </summary>
		/// <value>The size of the next window.</value>
		public Vector2 NextWindowSize { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="GREATClient.BaseClass.ScreenService"/> window size changed.
		/// </summary>
		/// <value><c>true</c> if window size changed; otherwise, <c>false</c>.</value>
		bool WindowSizeChanged { get; set; }

		public ScreenService(GraphicsDeviceManager gdm)
        {
			m_GraphicsDeviceManager = gdm;
			WindowSizeChanged = false;
			NextWindowSize = new Vector2(500,500);
        }

		public void SetFullscreen(bool fullscreen) {
			m_GraphicsDeviceManager.IsFullScreen = fullscreen;
		}
    
		public void Update()
		{
			if (WindowSizeChanged) {
				m_GraphicsDeviceManager.PreferredBackBufferHeight = (int)NextWindowSize.Y;
				m_GraphicsDeviceManager.PreferredBackBufferWidth = (int)NextWindowSize.X;
				m_GraphicsDeviceManager.ApplyChanges();

				WindowSizeChanged = false;
			}
		}
	}
}