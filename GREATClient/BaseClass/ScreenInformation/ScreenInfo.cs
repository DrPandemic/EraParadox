//
//  ScreenInfo.cs
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
using System.Xml.Serialization;
using System.IO;

namespace GREATClient.BaseClass.ScreenInformation
{
	[Serializable]
	[System.Xml.Serialization.XmlRoot("screenInfo")]
    public class ScreenInfo
    {
		/// <summary>
		/// The XML file containing the screen information.
		/// </summary>
		public const string SCREEN_INFORMATION = "screen.xml";

		[System.Xml.Serialization.XmlAttribute("windowWidth")]
		public int WindowWidth { get; set; }

		[System.Xml.Serialization.XmlAttribute("windowHeight")]
		public int WindowHeight { get; set; }

		[System.Xml.Serialization.XmlAttribute("screenWidth")]
		public int ScreenWidth { get; set; }

		[System.Xml.Serialization.XmlAttribute("screenHeight")]
		public int ScreenHeight { get; set; }

		[System.Xml.Serialization.XmlAttribute("autoResolution")]
		public bool AutoResolution { get; set; }

		[System.Xml.Serialization.XmlAttribute("fullscreen")]
		public bool Fullscreen { get; set; }

		[System.Xml.Serialization.XmlAttribute("limitResolution")]
		public bool LimitResolution { get; set; }

		/// <summary>
		/// Fills the info object from the xml.
		/// </summary>
		static public ScreenInfo GetInfo()
		{
			const string SCREEN_INFO_PATH = "Content/" + SCREEN_INFORMATION;
			ScreenInfo screenInfo = null;

			XmlSerializer serializer = new XmlSerializer(typeof(ScreenInfo));

			StreamReader reader = new StreamReader(SCREEN_INFO_PATH);
			screenInfo = (ScreenInfo)serializer.Deserialize(reader);
			reader.Close();
			return screenInfo;
		}

		public void SaveInfo()
		{			
			const string SCREEN_INFO_PATH = "Content/" + SCREEN_INFORMATION;

			XmlSerializer serializer = new XmlSerializer(typeof(ScreenInfo));
			TextWriter textWriter = new StreamWriter(SCREEN_INFO_PATH);
			serializer.Serialize(textWriter, this);
			textWriter.Close();
		}
    }
}

