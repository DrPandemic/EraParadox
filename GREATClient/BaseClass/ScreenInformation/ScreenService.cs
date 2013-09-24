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
using Microsoft.Xna.Framework.Input;

namespace GREATClient.BaseClass.ScreenInformation
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
		/// Gets or sets the screen info.
		/// This object contains the XML with the information.
		/// </summary>
		/// <value>The screen info.</value>
		ScreenInfo m_ScreenInfo;
		ScreenInfo screenInfo 
		{ 
			get {
				return m_ScreenInfo;
			}
			set {
				m_ScreenInfo = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is full screen.
		/// </summary>
		/// <value><c>true</c> if this instance is full screen; otherwise, <c>false</c>.</value>
		public bool IsFullScreen 
		{
			get {
				return screenInfo.Fullscreen;
			}
			set {
				screenInfo.Fullscreen = value;
				screenInfo.SaveInfo();
			}
		}

		/// <summary>
		/// Gets the size of the physical screen not the game window.
		/// </summary>
		/// <value>The size of the screen.</value>
		public Vector2 ScreenSize
		{
			get {
				return new Vector2(screenInfo.ScreenWidth,
				                   screenInfo.ScreenHeight);
			}
		}

		/// <summary>
		/// Gets or sets the size of the game window.
		/// </summary>
		/// <value>The size of the game window.</value>
		public Vector2 GameWindowSize
		{
			get {
				return new Vector2(screenInfo.WindowWidth,
				                   screenInfo.WindowHeight);
			}
			set {
				screenInfo.WindowWidth = (int)value.X;
				screenInfo.WindowHeight = (int)value.Y;
				screenInfo.SaveInfo();
			}
		}

		public ScreenService(GraphicsDeviceManager gdm)
        {
			m_GraphicsDeviceManager = gdm;
			screenInfo = ScreenInfo.GetInfo();
        }

		/// <summary>
		/// Switchs the display from windowed to fullscreen or the other way around.
		/// </summary>
		public void SwitchFullscreen()
		{
			IsFullScreen = !IsFullScreen;
		}

		public void Update()
		{
			//Mouse.SetPosition(10,10);
		}
	}
}