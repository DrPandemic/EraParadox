//
//  Client.cs
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
using System.Collections.Generic;

namespace GREATClient
{
	public class Client
	{
		static volatile Client instance;
		static object syncInstance = new object();
		public static Client Instance
		{
			get {
				if (instance == null) {
					lock (syncInstance) {
						if (instance == null) instance = new Client();
					}
				}
				return instance;
			}
		}

		/// <summary>
		/// The id of the client's player.
		/// FIXME: We already have client.UniqueIdentifier
		/// </summary>
		public long OurId
		{
			get {
				return client.UniqueIdentifier;
			}
		}
		/// <summary>
		/// Gets or sets the players of the game, given by the id (key).
		/// </summary>
		/// <value>The players.</value>
		public Dictionary<long, Player> Players { get; set; }

		/// <summary>
		/// The set of commands that the player wants to do.
		/// </summary>
		List<KeyValuePair<int, ClientMessage>> DesiredCommands = new List<KeyValuePair<int, ClientMessage>>();
		/// <summary>
		/// The current command identifier. This id represents at what position the command was desired 
		/// (lower = older commands, higher = newer commands).
		/// TODO: Lidgren should already be handling this
		/// </summary>
		int CurrentCommandId = 0;

		NetClient client;

		Client()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
			config.EnableUPnP = true;

			#if DEBUG
			// LAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGG (MonoDevelop is saying bullshit, it works)
			config.SimulatedLoss = 0.01f;
			config.SimulatedMinimumLatency = 0.05f;
			config.SimulatedRandomLatency = 0.05f;
			#endif

			this.client = new NetClient(config);
		}

		public void Start()
		{
			client.Start();
			client.UPnP.ForwardPort(client.Port, "GREAT Client");
			client.DiscoverLocalPeers(14242);
			// If the discover cluster-fucks on localhost, use that line instead of the one above
			//client.Connect("127.0.0.1", 14242);
		}

		public void Stop()
		{
			client.Disconnect("FUCK OFF!");
		}

		public void Update()
		{
			NetIncomingMessage msg;
			while ((msg = client.ReadMessage()) != null) {
				switch (msg.MessageType) {
					case NetIncomingMessageType.DiscoveryResponse:
						Console.WriteLine("Discover response from {0}", msg.SenderEndPoint);
						client.Connect(msg.SenderEndPoint);
						break;
					case NetIncomingMessageType.StatusChanged:
						NetConnectionStatus status = (NetConnectionStatus)msg.ReadByte();
						if (status == NetConnectionStatus.Connected) {
							Console.WriteLine("Connected to " + NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + "!");
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						ReadData(msg);
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				client.Recycle(msg);
			}
		}

		/// <summary>
		/// Reads data from a server message.
		/// </summary>
		/// <param name="msg">Message.</param>
		private void ReadData(NetIncomingMessage msg)
		{
			try
			{
				int serverMsgCode = msg.ReadInt32();

				ServerMessage type = (ServerMessage)serverMsgCode;
		
				switch (type)
				{
					//case ServerMessage.GivePlayerId:
					//	OurId = msg.ReadInt32(); // the client's player id. This should probably be done differently and thus is temporary
					//	break;

					case ServerMessage.PositionSync:
						SyncPlayers(msg);
						break;

					case ServerMessage.AcknowledgeCommand:
						AcknowledgedCommand(msg);
						break;

					default:
						throw new NotImplementedException("Server message type \"" + type + "\" not implemented.");
				}
			}
			catch(Exception e)
			{
				Console.WriteLine("Data not properly formatted: " + msg.ToString() + ", error=" + e.Message);
			}
		}

		/// <summary>
		/// The server acknowledged one of our old commands, so we can do some client-side
		/// prediction.
		/// </summary>
		/// <param name="msg">Message.</param>
		private void AcknowledgedCommand(NetIncomingMessage msg)
		{
			int commandId = msg.ReadInt32();

			// Remove all the commands that have been acknowledged.
			// Since the command we received is the latest acknowledged, 
			// remove all the commands that happenned before (i.e. lower Id
			// since higher Id means a command that happenned later).
			DesiredCommands.RemoveAll(pair => pair.Key <= commandId);
		}

		/// <summary>
		/// Take the last acknowledged command and performs the other
		/// commands locally to predict what will most likely happen.
		/// </summary>
		private void ClientSidePrediction()
		{
			if (Players != null && OurId != Player.InvalidId) { // players are loaded and we know who we are
				Physics.UpdateAnimation(Players[OurId]);

				// Reperform the commands since the last acknowledge to predict
				foreach (KeyValuePair<int, ClientMessage> pair in DesiredCommands) {
					switch (pair.Value) {
						case ClientMessage.MoveLeft:
							Physics.Move(Players[OurId], Direction.Left);
							break;

						case ClientMessage.MoveRight:
							Physics.Move(Players[OurId], Direction.Right);
							break;

						default:
							throw new NotImplementedException("Client message \"" + pair.Value.ToString() + "\" not implemented while doing client-side prediction.");
					}
				}
			}
		}

		/// <summary>
		/// Queues a command to later me executed.
		/// </summary>
		/// <param name="command">Command.</param>
		public void QueueCommand(ClientMessage command)
		{
			//TODO: send commands in packs rather than everytime a new command happens.
			NetOutgoingMessage msg = client.CreateMessage();
			int commandCode = (int)command;
			msg.Write(commandCode);
			// TODO: This feels redundant, I don't like it (Will)
			msg.Write(CurrentCommandId);
			client.SendMessage(msg, NetDeliveryMethod.ReliableUnordered);

			// Keep the command as a command that we want to do until it is acknowledged.
			DesiredCommands.Add(new KeyValuePair<int, ClientMessage>(CurrentCommandId, command));
			++CurrentCommandId; // move to the next Id
		}

		/// <summary>
		/// Synchronizes the players in the game.
		/// </summary>
		/// <param name="msg">Message.</param>
		private void SyncPlayers(NetIncomingMessage msg)
		{
			if (Players == null)
				Players = new Dictionary<long, Player>();

			while (msg.Position != msg.LengthBits) {
				Vec2 pos = new Vec2();
				Player p = new Player();
				msg.ReadAllProperties(p);
				msg.ReadAllProperties(pos);
				p.Position = pos;

				if (!Players.ContainsKey(p.Id)) {
					Players.Add(p.Id, p);
				}
				else {
					Players[p.Id] = p;
				}
			}

			ClientSidePrediction();
		}
	}
}

