//
//  Logger.cs
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
using System.IO;

namespace GREATLib
{
	/// <summary>
	/// Class used to log important game events.
	/// </summary>
    public class CustomLogger : ILogger
    {
		/// <summary>
		/// Gets or sets the output method of the logger.
		/// </summary>
		/// <value>The output.</value>
		public TextWriter Output { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.CustomLogger"/> class.
		/// </summary>
		/// <param name="priority">Minimum priority to show.</param>
		public CustomLogger(LogPriority priority = DEFAULT_MIN_PRIORITY)
		: this(Console.Out, priority) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.CustomLogger"/> class.
		/// </summary>
		/// <param name="output">Output method of the logger.</param>
		/// <param name="priority">Minimum priority of the messages to show.</param>
		public CustomLogger(TextWriter output, LogPriority priority = DEFAULT_MIN_PRIORITY)
		{
			MinPriority = priority;
			Output = output;
		}

		/// <summary>
		/// Log the specified message (if the priority is high enough) on the output of the logger.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="priority">Priority.</param>
		public override void LogMessage(string message, LogPriority priority = DEFAULT_MESSAGE_PRIORITY)
		{
			if (priority >= MinPriority) {
				Output.WriteLine(DateTime.Now.ToString() + ": " + message);
			}
		}
    }
}

