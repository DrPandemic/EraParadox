//
//  Main.cs
//
//  Author:
//       William Turner <willtur.will@gmail.com>
//
//  Copyright (c) 2013 
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
using System.Threading;

namespace GREATServer
{
	class MainClass
	{
		static readonly TimeSpan SLEEP_TIME = TimeSpan.FromMilliseconds(50.0);

		static Server server;

		public static void Main(string[] args)
		{
			int port = 14242;
			if (args.Length > 0) {
				int.TryParse(args[0], out port);
			}

			Server.Port = port;

			server = Server.Instance;
			server.Start();

			while ((!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Q) &&
			       !Server.Exit) {
				server.Update(SLEEP_TIME.TotalSeconds);
				Thread.Sleep(SLEEP_TIME);
			}

			server.Stop();
		}
	}
}
