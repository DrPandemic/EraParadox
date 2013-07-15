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

namespace GREATClient
{
	public enum LogPriority
	{
		VeryLow = 0,
		Low,
		Normal,
		High,
		VeryHigh,
		Warning,
		Error,
		Critical
	}

	/// <summary>
	/// Class used to log important game events.
	/// </summary>
    public class Logger
    {
		public static Logger Default = new Logger();

		const LogPriority DEFAULT_MESSAGE_PRIORITY = LogPriority.Normal;
		const LogPriority DEFAULT_MIN_PRIORITY = LogPriority.VeryLow;

		public TextWriter Output { get; set; }
		public LogPriority MinPriority { get; set; }

		public Logger(LogPriority priority = DEFAULT_MIN_PRIORITY)
		: this(Console.Out, priority) { }

		public Logger(TextWriter output, LogPriority priority = DEFAULT_MIN_PRIORITY)
		{
			MinPriority = priority;
			Output = output;
		}

		/// <summary>
		/// Log the specified message (if the priority is high enough) on the output of the logger.
		/// </summary>
		public void LogMessage(string message, LogPriority priority = DEFAULT_MESSAGE_PRIORITY)
		{
			if (priority >= MinPriority) { // the priority is high enough
				Output.WriteLine(DateTime.Now + ": " + message);
			}
		}
		/// <summary>
		/// Log the specified message (if the priority is high enough) on the default logger.
		/// </summary>
		public static void Log(string message, LogPriority priority = DEFAULT_MESSAGE_PRIORITY)
		{
			Default.LogMessage(message, priority);
		}
    }
}

