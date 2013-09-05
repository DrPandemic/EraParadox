///
//  Server.cs
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
using Lidgren.Network;
using GREATLib;

namespace GREATServer
{
	public class Server
	{
		static volatile Server instance;
		static object syncInstance = new object();
		public static Server Instance
		{
			get {
				if (instance == null) {
					lock (syncInstance) {
						if (instance == null) instance = new Server();
					}
				}
				return instance;
			}
		}

		NetServer server;


		// The running game. TODO: replace by a list of current games.
		double Time { get; set; }
		ServerGame Game { get; set; }
		double LastUpdateTime { get; set; }

		Server()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
			config.Port = 14242;
			config.EnableUPnP = true;

#if DEBUG
			// LAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGG (MonoDevelop is saying bullshit, it works)
			config.SimulatedLoss = 0.01f;
			config.SimulatedMinimumLatency = 0.1f;//05f;
			config.SimulatedRandomLatency = 0.01f;
#endif

			this.server = new NetServer(config);
		}

		public void Start()
		{
			server.Start();
			server.UPnP.ForwardPort(server.Port, "GREAT Server");

			Time = 0.0;
			//TODO: Temporary game initialization.
			Game = new ServerGame(server);
			LastUpdateTime = GetTime().TotalSeconds;
		}

		public void Stop()
		{
			server.Shutdown("I'M DYING D:");
		}

		public TimeSpan GetTime()
		{
			return TimeSpan.FromSeconds(Time);
		}

		public void Update(double dt)
		{
			NetIncomingMessage msg;
			while ((msg = server.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.DiscoveryRequest:
						Console.WriteLine("Discover request from {0}", msg.SenderEndPoint);
						server.SendDiscoveryResponse(null, msg.SenderEndPoint);
						break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if (status == NetConnectionStatus.Connected) {
							Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
							OnConnection(msg.SenderConnection);
						}
						else if (status == NetConnectionStatus.Disconnecting) {
							Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " disconnected!");
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						// TODO: update all the games instead of just one.
						Game.OnDataReceived(msg);
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				server.Recycle(msg);
			}

			double time = GetTime().TotalSeconds;
			Game.Update(time - LastUpdateTime);

			LastUpdateTime = time;

			Time += dt;
		}

		/// <summary>
		/// Called when a client connects to the server.
		/// </summary>
		void OnConnection(NetConnection connection)
		{
			//TODO: check which game to join instead of just the first
			Game.AddClient(connection);
		}
	}
}

