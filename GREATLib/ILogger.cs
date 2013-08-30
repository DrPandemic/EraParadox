//
//  ILogger.cs
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
using System.Diagnostics;
using System.IO;

namespace GREATLib
{
	/// <summary>
	/// Priority of the message to log.
	/// </summary>
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
	/// Manages the program's logs.
	/// </summary>
    public abstract class ILogger
    {
		/// <summary>
		/// The default logger to use.
		/// </summary>
		public static ILogger Default = new CustomLogger(false); // true: log to file, false: log to console

		protected const LogPriority DEFAULT_MESSAGE_PRIORITY = LogPriority.VeryLow;
		protected const LogPriority DEFAULT_MIN_PRIORITY = LogPriority.VeryLow;

		/// <summary>
		/// Gets or sets the minimum priority to show of the logger.
		/// </summary>
		/// <value>The minimum priority.</value>
		public LogPriority MinPriority { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GREATClient.ILogger"/> class.
		/// </summary>
		/// <param name="priority">Minimum priority to show.</param>
		public ILogger(LogPriority priority = DEFAULT_MIN_PRIORITY)
		{
			MinPriority = priority;
		}

		/// <summary>
		/// Log the specified message (if the priority is high enough) on the output of the logger.
		/// </summary>
		public abstract void LogMessage(string message, LogPriority priority = DEFAULT_MESSAGE_PRIORITY);
		/// <summary>
		/// Log the specified message (if the priority is high enough) on the default logger.
		/// </summary>
		public static void Log(string message, LogPriority priority = DEFAULT_MESSAGE_PRIORITY)
		{
			Debug.Assert(message != null);
			Default.LogMessage(message, priority);
		}
    }
}

