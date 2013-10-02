//
//  PingCounter.cs
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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GREATClient.BaseClass;

namespace GREATClient.Display
{
    public class PingCounter : DrawableLabel
    {
		/// <summary>
		/// Gets or sets the function called to get the ping
		/// </summary>
		/// <value>The ping.</value>
		Func <double>FunctionUpdate { get; set; }

		public PingCounter(Func <double>functionUpdate) : base (UIConstants.UI_FONT)
        {
			FunctionUpdate = functionUpdate;
        }

		protected override void OnUpdate(GameTime dt)
		{
			Text = FunctionUpdate() + " ms";
		}
    }
}

