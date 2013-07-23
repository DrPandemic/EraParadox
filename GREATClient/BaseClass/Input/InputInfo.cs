//
//  InputInfo.cs
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
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;

namespace GREATClient
{
	[Serializable()]
	public class InputInfo
	{
		public const string SPELL_1 = "spell1";
		public const string SPELL_2 = "spell2";
		public const string SPELL_3 = "spell3";
		public const string SPELL_4 = "spell4";


		[System.Xml.Serialization.XmlAttribute("name")]
		public string Name { get; set; }

		[System.Xml.Serialization.XmlAttribute("key")]
		public Keys Key { get; set; }
	}
	[Serializable]
	[System.Xml.Serialization.XmlRoot("inputCollection")]
	public class InputInfos
    {
		[System.Xml.Serialization.XmlArray("inputs")]
		[System.Xml.Serialization.XmlArrayItem("input", typeof(InputInfo))]
		public InputInfo[] Inputs { get; set; }

    }

	public class Inputs
	{
		/// <summary>
		/// All the key infos.
		/// </summary>
		/// <value>The info.</value>
		public Dictionary<Keys, InputInfo> Info { get; set; }

		public Inputs()
        {
			FillInfo();
        }

		public InputInfo GetInfo(Keys key)
		{
			Debug.Assert(Info.ContainsKey(key));
			return Info[key];
		}
		private void FillInfo()
		{
			const string INPUTS_PATH = "Content/inputs.xml";
			Info = new Dictionary<Keys, InputInfo>();

			InputInfos inputs = null;

			XmlSerializer serializer = new XmlSerializer(typeof(InputInfos));

			StreamReader reader = new StreamReader(INPUTS_PATH);
			inputs = (InputInfos)serializer.Deserialize(reader);
			reader.Close();

			foreach (InputInfo info in inputs.Inputs)
			{
				Debug.Assert(!Info.ContainsKey(info.Key),"There was 2 times the same key in the inputs.xml");
				Info.Add(info.Key, info);
			}
		}
	}
}

