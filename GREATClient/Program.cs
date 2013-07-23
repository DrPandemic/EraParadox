using System;
using System.Collections.Generic;
using System.Linq;

namespace GREATClient
{
	static class Program
	{
		static GreatGame game;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Console.WriteLine("Creating game...");
			game = new GreatGame();
			game.Run();
		}
	}
}
