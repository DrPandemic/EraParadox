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
using GREATLib.Entities;
using GREATLib.Entities.Player;
using GREATLib.Entities.Player.Champions.AllChampions;
using GREATLib.Entities.Physics;

namespace GREATServer
{
	/// <summary>
	/// Represents a game on the server.
	/// </summary>
    public class ServerGame
    {
		static readonly TimeSpan POSITION_UPDATE_RATE = GameMatch.STATE_UPDATE_INTERVAL;

		Random random = new Random();

		NetServer Server { get; set; }
		Dictionary<NetConnection, ServerClient> Clients { get; set; }

		GameMatch Match { get; set; }
		EntityIDGenerator IDGenerator { get; set; }



        public ServerGame(NetServer server)
        {
			IDGenerator = new EntityIDGenerator();
			Server = server;
			Clients = new Dictionary<NetConnection, ServerClient>();

			Match = new GameMatch();

			Timer updatePositions = new Timer(POSITION_UPDATE_RATE.TotalMilliseconds);
			updatePositions.Elapsed += UpdatePositions;
			updatePositions.Start();

			Timer update = new Timer(PhysicsSystem.UPDATE_RATE.TotalMilliseconds);
			update.Elapsed += Update;
			update.Start();
        }

		void Update(object sender, EventArgs e)
		{
			Match.Update(PhysicsSystem.UPDATE_RATE.TotalMilliseconds);
		}

		public void AddClient(NetConnection connection)
		{
			// Create the player associated to that client
			Player player = new Player();
			Match.AddPlayer(player, new StickmanChampion());
			player.Champion.Position = new Vec2((float)random.NextDouble() * 800f + 50f, (float)random.NextDouble() * 100f + 100f);

			// Tell the others that he joined
			SendToClients(GenerateNewPlayerMessage(player, false), NetDeliveryMethod.ReliableUnordered);

			// Create the client to remember him
			ServerClient client = new ServerClient(connection, player);
			Clients.Add(client.Connection, client);

			// Tell the new client about the old clients
			foreach (ServerClient c in Clients.Values)
				Server.SendMessage(GenerateNewPlayerMessage(c.Player, c.Player.Id == client.Player.Id),
				                   client.Connection,
				                   NetDeliveryMethod.ReliableUnordered);
		}

		NetOutgoingMessage GenerateNewPlayerMessage(Player player, bool ourId)
		{
			NetOutgoingMessage msg = Server.CreateMessage();
			msg.Write((byte)ServerCommand.NewPlayer);
			msg.Write((int)player.Id);
			msg.Write((bool)ourId);
			msg.Write((float)player.Champion.Position.X);
			msg.Write((float)player.Champion.Position.Y);
			return msg;
		}

		public void OnDataReceived(NetIncomingMessage msg)
		{
			Debug.Assert(Clients.ContainsKey(msg.SenderConnection));
			ServerClient client = Clients[msg.SenderConnection];

			byte code = msg.ReadByte();
			ClientCommand command = (ClientCommand)code;

			switch (command) {
				case ClientCommand.MoveLeft:
					Match.MovePlayer(client.Player.Id, HorizontalDirection.Left);
					break;
				case ClientCommand.StopMoveLeft:
					Match.StopPlayer(client.Player.Id, HorizontalDirection.Left);
					break;
				case ClientCommand.StopMoveRight:
					Match.StopPlayer(client.Player.Id, HorizontalDirection.Right);
					break;
				case ClientCommand.MoveRight:
					Match.MovePlayer(client.Player.Id, HorizontalDirection.Right);
					break;
				case ClientCommand.Jump:
					Match.JumpPlayer(client.Player.Id);
					break;

				default:
					Console.WriteLine("Unknown client command: " + command);
					break;
			}
		}

		void UpdatePositions(object sender, EventArgs e)
		{
			if (Clients.Count > 0) {
				SendToClients(GeneratePositionUpdateMessage(), NetDeliveryMethod.Unreliable);
			}
		}

		NetOutgoingMessage GeneratePositionUpdateMessage()
		{
			NetOutgoingMessage msg = Server.CreateMessage();
			msg.Write((byte)ServerCommand.PositionUpdate);
			msg.Write((byte)Clients.Count);
			foreach (ServerClient client in Clients.Values) {
				msg.Write((int)client.Player.Champion.Id);
				msg.Write((float)client.Player.Champion.Position.X);
				msg.Write((float)client.Player.Champion.Position.Y);
				msg.Write((byte)client.Player.Champion.CurrentAnimation);
				msg.Write((bool)client.Player.Champion.FacingLeft);
			}
			return msg;
		}

		void SendToClients(NetOutgoingMessage msg, NetDeliveryMethod method)
		{
			foreach (ServerClient client in Clients.Values) {
				NetOutgoingMessage tmp = Server.CreateMessage();
				tmp.Write(msg);
				Server.SendMessage(tmp, client.Connection, method);
			}
		}
    }
}

