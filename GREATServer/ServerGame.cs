//
//  ServerGame.cs
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
using Lidgren.Network;
using System.Collections.Generic;
using System.Timers;
using System.Diagnostics;
using GREATLib;
using GREATLib.Network;
using GREATLib.Entities;

namespace GREATServer
{
	/// <summary>
	/// Represents a game on the server.
	/// </summary>
    public class ServerGame
    {
		static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromMilliseconds(45.0);

		Random random = new Random();

		NetServer Server { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }

		GameMatch Match { get; set; }


        public ServerGame(NetServer server)
        {
			Server = server;
			Clients = new Dictionary<NetConnection, ServerClient>();
			Match = new GameMatch();

			Timer updateTimer = new Timer(UPDATE_INTERVAL.TotalMilliseconds);
			updateTimer.Elapsed += Update;
			updateTimer.Start();
        }

		/// <summary>
		/// Sends a player command to a client.
		/// </summary>
		/// <param name="fillMessage">The function to call to fill the message with the command</param>
		void SendCommand(NetConnection connection, ServerCommand command, NetDeliveryMethod method,
		                 Action<NetOutgoingMessage> fillMessage)
		{
			NetOutgoingMessage msg = Server.CreateMessage();
			msg.Write((byte)command);

			fillMessage(msg);

			Server.SendMessage(msg, connection, method);
		}

		void Update(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Adds the client to the current game.
		/// </summary>
		public void AddClient(NetConnection connection)
		{
			ILogger.Log("New player added to the game.", LogPriority.High);

			IEntity champion = CreateRandomChampion();

			ServerClient client = new ServerClient(connection, champion);
			Clients.Add(connection, client);

			// Send to the client that asked to join
			SendCommand(connection,
			       ServerCommand.NewPlayer,
			       NetDeliveryMethod.ReliableOrdered,
			       (msg) => FillNewPlayerMessage(msg, champion, true));

			//TODO: send to the other players as well here
		}

		/// <summary>
		/// Creates a message indicating that a player has joined the game and that
		/// the client should create a new drawable champion associated to it.
		/// </summary>
		/// <param name="isOwner">Whether this is the new player or not.</param>
		static void FillNewPlayerMessage(NetOutgoingMessage msg, IEntity champion, bool isOwner)
		{
			msg.Write((uint)champion.ID);
			msg.Write((float)champion.Position.X);
			msg.Write((float)champion.Position.Y);
			msg.Write((bool)isOwner);
		}

		/// <summary>
		/// Creates a random champion at a random starting position (mainly used
		/// for testing purposes).
		/// </summary>
		static IEntity CreateRandomChampion()
		{
			return new IEntity(IDGenerator.GenerateID(), 
			                   new Vec2(Utilities.RandomFloat(Utilities.Random, 100f, 400f), 0f));
		}

		/// <summary>
		/// When we received data from one of our players.
		/// </summary>
		public void OnDataReceived(NetIncomingMessage message)
		{
			ClientCommand command = (ClientCommand)message.ReadByte();

			switch (command) {
				case ClientCommand.ActionPackage:
					OnActionPackage(message);
					break;

				default:
					Debug.Assert(false, "Invalid client command.");
					ILogger.Log("Invalid client command received: " + command, LogPriority.Warning);
					break;
			}
		}

		void OnActionPackage(NetIncomingMessage message)
		{
			try {
				byte size = message.ReadByte();
				for (int i = 0; i < size; ++i) {
					uint id = message.ReadUInt32();
					float time = message.ReadFloat();
					PlayerActionType type = (PlayerActionType)message.ReadByte();

					ILogger.Log(String.Format("Action package: size={3}, id={0}, time={1}, type={2}", id,time,type,size));
				}
			} catch (Exception e) {
				ILogger.Log("Action package badly formatted: " + e.ToString(), LogPriority.Error);
			}
		}
    }
}

