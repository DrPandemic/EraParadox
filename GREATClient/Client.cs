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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using GREATLib.Network;
using GREATLib.Entities.Champions;
using GREATLib.Entities.Spells;
using GREATLib.Entities;
using GREATLib.Entities.Structures;


namespace GREATClient
{
	public sealed class Client
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

		NetClient client;
		double SharedTime { get; set; }

		LinkedList<KeyValuePair<ServerCommand, NetBuffer>> CommandsToDo { get; set; }
		Dictionary<ServerCommand, ServerCommandEvent> EventsForCommand { get; set; }

		public Client()
		{
			NetPeerConfiguration config = new NetPeerConfiguration("GREAT");
			config.EnableMessageType(NetIncomingMessageType.DiscoveryResponse);
			config.EnableUPnP = true;

			#if DEBUG
			// LAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGGGGGGGG (MonoDevelop is saying bullshit, it works)
			config.SimulatedLoss = 0f;
			config.SimulatedMinimumLatency = 0.015f;
			config.SimulatedRandomLatency = 0f;
			#endif

			this.client = new NetClient(config);

			SharedTime = NetTime.Now;

			CommandsToDo = new LinkedList<KeyValuePair<ServerCommand, NetBuffer>>();
			EventsForCommand = GetEventsForCommand();
		}

		public void Start()
		{
			this.client.Start();
			client.UPnP.ForwardPort(client.Port, "GREAT Client");
			client.DiscoverLocalPeers(14242);
			// If the discover cluster-fucks on localhost, use that line instead
			//client.Connect("172.17.104.127", 14242);
		}

		public void Stop()
		{
			client.Disconnect("Client stopped.");
		}

		public void Update(double dt)
		{
			SharedTime += dt;

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
							Console.WriteLine(NetUtility.ToHexString(msg.SenderConnection.RemoteUniqueIdentifier) + " connected!");
						}
						break;
					case NetIncomingMessageType.VerboseDebugMessage:
					case NetIncomingMessageType.DebugMessage:
					case NetIncomingMessageType.WarningMessage:
					case NetIncomingMessageType.ErrorMessage:
						Console.WriteLine(msg.ReadString());
						break;
					case NetIncomingMessageType.Data:
						OnDataReceived(msg);
						break;
					default:
						Console.WriteLine("Unhandled type: " + msg.MessageType);
						break;
				}
				client.Recycle(msg);
			}
		}

		public TimeSpan GetPing()
		{
            TimeSpan ping = client.ServerConnection != null && client.ServerConnection.AverageRoundtripTime >= 0f ?
				TimeSpan.FromSeconds((double)client.ServerConnection.AverageRoundtripTime) : 
				TimeSpan.Zero;

            Debug.Assert(0.0 <= ping.TotalSeconds);

            return ping;
		}

		public TimeSpan GetTime()
		{
			return TimeSpan.FromSeconds(SharedTime);
		}

		void OnDataReceived(NetBuffer msg)
		{
			byte code = msg.ReadByte();
			ServerCommand command = (ServerCommand)code;

			CommandsToDo.AddLast(Utilities.MakePair<ServerCommand, NetBuffer>(command, msg));

			ExecuteCommands();
		}

		void ExecuteCommands()
		{
			List<KeyValuePair<ServerCommand, NetBuffer>> toRemove = new List<KeyValuePair<ServerCommand, NetBuffer>>();
			foreach (var command in CommandsToDo) {
				if (EventsForCommand.ContainsKey(command.Key)) { // if we have implemented the action to do when we receive this command type
					if (EventsForCommand[command.Key].Execute(command.Value)) {
						toRemove.Add(command); // if we have executed the command properly, we can remove it
					}
				} else {
					throw new NotImplementedException();
				}
			}

			toRemove.ForEach(com => CommandsToDo.Remove(com));
		}

		Dictionary<ServerCommand, ServerCommandEvent> GetEventsForCommand()
		{
			Dictionary<ServerCommand, ServerCommandEvent> e = new Dictionary<ServerCommand, ServerCommandEvent>();

			// we reveive our information after joining a game
			e.Add(ServerCommand.JoinedGame,
			      new ServerCommandEvent(
					(msg) => new JoinedGameEventArgs(msg),
                    (data) => SetSharedTime(((JoinedGameEventArgs)data).Time)));

			// a new player has joined our game
			e.Add(ServerCommand.NewRemotePlayer,
			      new ServerCommandEvent(
					(msg) => new NewRemotePlayerEventArgs(msg)));

			// a state update from the server
			e.Add(ServerCommand.StateUpdate,
			      new ServerCommandEvent(
					(msg) => new StateUpdateEventArgs(msg),
                    (data) => SetSharedTime(((StateUpdateEventArgs)data).Time)));

			return e;
		}

		void SetSharedTime(double time)
		{
			Debug.Assert(time >= 0.0);

			// Take the given server time + our approximation of how long it took to get the message.
			SharedTime = time + GetPing().TotalSeconds / 2.0;

			Debug.Assert(0.0 <= SharedTime && time <= SharedTime);
		}

		/// <summary>
		/// Registers the command handler to react to certain server commands.
		/// </summary>
		public void RegisterCommandHandler(ServerCommand command, EventHandler<CommandEventArgs> handler)
		{
			if (EventsForCommand.ContainsKey(command)) {
				EventsForCommand[command].Handler += handler;
			} else {
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Sends the action package of the player, indicating the inputs that the player
		/// has done in the past few milliseconds.
		/// </summary>
		public void SendPlayerActionPackage(IEnumerable<PlayerAction> actions)
		{
			NetOutgoingMessage msg = client.CreateMessage();
			msg.Write((byte)ClientCommand.ActionPackage);

			foreach (PlayerAction action in actions) {
				ulong id = action.ID;
				float time = action.Time;
				byte type = (byte)action.Type;
				float x = action.Position.X;
				float y = action.Position.Y;

				msg.Write(id);
				msg.Write(time);
				msg.Write(type);
				msg.Write(x);
				msg.Write(y);

				if (ActionTypeHelper.IsSpell(action.Type)) {
					Debug.Assert(action.Target != null, "Trying to us target on non-spell action.");
					float tx = action.Target.X;
					float ty = action.Target.Y;
					msg.Write(tx);
					msg.Write(ty);
				}
			}

			client.SendMessage(msg, NetDeliveryMethod.ReliableOrdered);
		}
	}


	public class CommandEventArgs : EventArgs 
	{
		public CommandEventArgs(NetBuffer msg)
		{
		}
	}
	public class JoinedGameEventArgs : CommandEventArgs
	{
		public double Time { get; private set; }
		public PlayerData OurData { get; private set; }
		public List<PlayerData> RemotePlayers { get; private set; }

		public JoinedGameEventArgs(NetBuffer msg) : base(msg)
		{
            RemotePlayers = new List<PlayerData>();
			OurData = new PlayerData(msg); // the first is our data
			while (msg.Position < msg.LengthBits) {
				RemotePlayers.Add(new PlayerData(msg));
			}
		}
	}
	public class NewRemotePlayerEventArgs : CommandEventArgs
	{
		public PlayerData Data { get; private set; }
		public NewRemotePlayerEventArgs(NetBuffer msg) : base(msg)
		{
			Data = new PlayerData(msg);
		}
	}
	public struct PlayerData
	{
		public ulong ID { get; private set; }
		public Vec2 Position { get; private set; }
		public ChampionTypes Type { get; private set; }
		public Teams Team { get; private set; }
		public float MaxHealth { get; private set; }
		public float Health { get; private set; }

		public PlayerData(NetBuffer msg) : this()
		{
			ID = msg.ReadUInt64();
			Position = new Vec2(msg.ReadFloat(), msg.ReadFloat());
			Type = (ChampionTypes)msg.ReadByte();
			Team = msg.ReadBoolean() ? Teams.Left : Teams.Right;
			MaxHealth = msg.ReadFloat();
			Health = msg.ReadFloat();
		}
	}
	public struct StateUpdateData
	{
		public ulong ID { get; private set; }
		public Vec2 Position { get; private set; }
		public Vec2 Velocity { get; private set; }
		public ChampionAnimation Animation { get; private set; }
		public bool FacingLeft { get; private set; }

		public StateUpdateData(ulong id, Vec2 pos, Vec2 vel, ChampionAnimation anim, bool facingLeft)
			: this()
		{
			ID = id;
			Position = pos;
			Velocity = vel;
			Animation = anim;
			FacingLeft = facingLeft;
		}
	}
	public class StateUpdateEventArgs : CommandEventArgs
	{
		public ulong LastAcknowledgedActionID { get; private set; }
		public double Time { get; private set; }
		public List<StateUpdateData> EntitiesUpdatedState { get; private set; }
		public List<RemarkableEventData> RemarkableEvents { get; private set; }

		public StateUpdateEventArgs(NetBuffer msg) : base(msg)
		{
			EntitiesUpdatedState = new List<StateUpdateData>();
			RemarkableEvents = new List<RemarkableEventData>();

			LastAcknowledgedActionID = msg.ReadUInt64();
			Time = msg.ReadDouble();
			Vec2 velocity = new Vec2(msg.ReadFloat(), msg.ReadFloat());
			byte nbClients = msg.ReadByte();
			for (byte i = 0; i < nbClients; ++i) {
				ulong id = msg.ReadUInt64();
				Vec2 pos = new Vec2(msg.ReadFloat(), msg.ReadFloat());
				ChampionAnimation anim = (ChampionAnimation)msg.ReadByte();
				bool facingLeft = msg.ReadBoolean();

				EntitiesUpdatedState.Add(new StateUpdateData(id, pos, velocity, anim, facingLeft));
			}
			while (msg.Position != msg.LengthBits) {
				ServerCommand cmd = (ServerCommand)msg.ReadByte();
				RemarkableEventData data = null;
				switch (cmd) {
					case ServerCommand.SpellCast:
						data = new SpellCastEventData(
							msg.ReadUInt64(),
							msg.ReadUInt64(),
							(SpellTypes)msg.ReadByte(),
							msg.ReadFloat(),
							new Vec2(msg.ReadFloat(), msg.ReadFloat()),
							new Vec2(msg.ReadFloat(), msg.ReadFloat()),
							TimeSpan.FromSeconds(msg.ReadFloat()),
							msg.ReadFloat(),
							msg.ReadFloat());
						break;

					case ServerCommand.SpellDisappear:
						data = new SpellDisappearEventData(msg.ReadUInt64());
						break;

					case ServerCommand.StatsChanged:
						data = new StatsChangedEventData(msg.ReadUInt64(), msg.ReadFloat());
						break;

					case ServerCommand.ChampionDied:
						data = new ChampionDiedEventData(msg.ReadUInt64(),
						                                 msg.ReadUInt64(),
						                                 msg.ReadUInt32(), msg.ReadUInt32(), msg.ReadUInt32(), msg.ReadUInt32(),
						                                 TimeSpan.FromSeconds(msg.ReadUInt16()));
						break;

					case ServerCommand.StructureStatsChanged:
						data = new StructureStatsChangedEventData(msg.ReadBoolean() ? Teams.Left : Teams.Right,
						                                          (StructureTypes)msg.ReadByte(),
						                                          msg.ReadFloat());
						break;

					case ServerCommand.StructureDestroyed:
						data = new StructureDestroyedEventData(msg.ReadBoolean() ? Teams.Left : Teams.Right,
						                                       (StructureTypes)msg.ReadByte());
						break;

					case ServerCommand.EndOfGame:
						data = new EndOfGameEventData(msg.ReadBoolean() ? Teams.Left : Teams.Right);
						break;

					default:
						Debug.Fail("Unknown server command when updating (unknown remarkable event)");
						break;
				}
				if (data != null) {
					RemarkableEvents.Add(data);
				}
			}
		}
	}
	public class RemarkableEventData
	{
		public ServerCommand Command { get; private set; }
		public RemarkableEventData(ServerCommand cmd)
		{
			Command = cmd;
		}
	}
	public class SpellCastEventData : RemarkableEventData
	{
		public ulong ID { get; private set; }
		public ulong OwnerID { get; private set; }
		public SpellTypes Type { get; private set; }
		public float Time { get; private set; }
		public Vec2 Position { get; private set; }
		public Vec2 Velocity { get; private set; }
		public TimeSpan Cooldown { get; private set; }
		public float Range { get; private set; }
		public float Width { get; private set; }

		public SpellCastEventData(ulong id, ulong owner, SpellTypes type, float time, Vec2 pos, 
		                          Vec2 vel, TimeSpan cooldown, float range, float width)
			: base(ServerCommand.SpellCast)
		{
			ID = id;
			OwnerID = owner;
			Type = type;
			Time = time;
			Position = pos;
			Velocity = vel;
			Cooldown = cooldown;
			Range = range;
			Width = width;
		}
	}
	public class SpellDisappearEventData : RemarkableEventData
	{
		public ulong ID { get; private set; }
		public SpellDisappearEventData(ulong id)
			: base(ServerCommand.SpellDisappear)
		{
			ID = id;
		}
	}
	public class StatsChangedEventData : RemarkableEventData
	{
		public ulong ChampID { get; private set; }
		public float Health { get; private set; }
		public StatsChangedEventData(ulong id, float hp)
			: base(ServerCommand.StatsChanged)
		{
			ChampID = id;
			Health = hp;
		}
	}
	public class ChampionDiedEventData : RemarkableEventData
	{
		public ulong ChampID { get; private set; }
		public ulong? Killer { get; private set; }
		public uint Kills { get; private set; }
		public uint Deaths { get; private set; }
		public uint LeftKills { get; private set; }
		public uint RightKills { get; private set; }
		public TimeSpan RespawnTime { get; private set; }
		public ChampionDiedEventData(ulong id, ulong killer,
		                             uint kills, uint deaths, uint leftKills, uint rightKills,
		                             TimeSpan respawn)
			: base(ServerCommand.ChampionDied)
		{
			ChampID = id;
			Killer = killer != IDGenerator.NO_ID ? (ulong?)killer : null;

			Kills = kills;
			Deaths = deaths;
			LeftKills = leftKills;
			RightKills = rightKills;

			RespawnTime = respawn;
		}
	}
	public class StructureStatsChangedEventData : RemarkableEventData
	{
		public Teams Team { get; private set; }
		public StructureTypes Type { get; private set; }
		public float Health { get; private set; }
		public StructureStatsChangedEventData(Teams team, StructureTypes type, float hp)
			: base(ServerCommand.StructureStatsChanged)
		{
			Team = team;
			Type = type;
			Health = hp;
		}
	}
	public class StructureDestroyedEventData : RemarkableEventData
	{
		public Teams Team { get; private set; }
		public StructureTypes Type { get; private set; }
		public StructureDestroyedEventData(Teams team, StructureTypes type)
			: base(ServerCommand.StructureDestroyed)
		{
			Team = team;
			Type = type;
		}
	}
	public class EndOfGameEventData : RemarkableEventData
	{
		public Teams Winner { get; private set; }
		public EndOfGameEventData(Teams winner)
			: base(ServerCommand.EndOfGame)
		{
			Winner = winner;
		}
	}
}

