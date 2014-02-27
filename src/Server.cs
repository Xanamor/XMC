#region Header

//    XMC, a Minecraft SMP server.
//    Copyright (C) 2011 XMC. All rights reserved.
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion Header

//disable CS0219: variable X is assined but its value is never used
//#pragma warning disable 219
//disable CS0168: variable X is declared but never used
//#pragma warning disable 168
namespace XMC
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading;
	using Constants;

	//using Plugins;
	//TODO: Rework Plugins, untill then I have removed the plugin system. --Xinnx009@gmail.com
	using Utils;

	public class Server
	{
		#region Fields

		public Dictionary<string, Player> ActivePlayers;
		public string ListenAddr;
		public string Motd;
		public string Name;
		public List<Player> PlayerList;
		public int Port;
		public bool Running;
		public string ServerHash;
		public List<Window> WindowList;
		public Map World;
		public string WorldName;
		private static Thread titleMonitor = 
			new Thread (new ThreadStart (Monitor));
		//TODO: Output Statistics to a external source...MySql maybe?
		private TcpListener _Listener;

		#endregion Fields

		#region Constructors

		public Server ()
		{
			Running = false;

			ListenAddr = SSettings.ListenAddr;
			Port = SSettings.ListenPort;
			WorldName = SSettings.WorldName;
			Name = SSettings.ServerName;
			Motd = SSettings.MOTD;

			ServerHash = "-";

			World = null;
			PlayerList = new List<Player> ();
			ActivePlayers = new Dictionary<string,Player> ();
			if (ListenAddr == "0.0.0.0")
				_Listener = new TcpListener (new IPEndPoint (IPAddress.Any, Port));
			else
				_Listener = new TcpListener (new IPEndPoint (IPAddress.Parse (ListenAddr), Port));
		}

		#endregion Constructors

		#region Methods

		public void BlockChanged (int x, int y, int z, Block newBlock)
		{
			Chunk c = World.GetChunkAt (x, z);
			foreach (Player p in PlayerList) {
				if (p.VisibleChunks.Contains (c)) {
					p.BlockChanged (x, y, z, newBlock);
				}
			}
		}

		public void Despawn (Player player)
		{
			MessageAll (Color.Announce + player.Username + " has left");
		}

		public void MessageAll (string message)
		{
			foreach (Player p in PlayerList) {
				p.SendMessage (message);
			}
		}

		public void Run ()
		{
			World = new Map (WorldName);
			World.Time = 0;
			if (!File.Exists (WorldName + "/level.dat")) {
				XMC.Log ("Generating world " + WorldName);
				World.Generate ();
				World.ForceSave ();
			}

			if (!Directory.Exists (SSettings.PlayerSavePath)) {
				Directory.CreateDirectory (SSettings.PlayerSavePath);
			}
			_Listener.Start ();
			XMC.Log ("Listening on port " + Port);
			titleMonitor.Start ();
			Running = true;

			InventoryItem i = new InventoryItem (3);
			PickupEntity e = new PickupEntity (World.SpawnX, World.SpawnY, World.SpawnZ, i);

			Stopwatch clock = new Stopwatch ();
			clock.Start ();
			double lastUpdate = 0;
			double lastGc = 0;
			double lastSaveAll = 0;

			while (Running) {
				// Check for new connections
				while (_Listener.Pending ()) {
					AcceptConnection (_Listener.AcceptTcpClient ());
					//Running = false;
				}

				if (lastUpdate + 0.2 < clock.Elapsed.TotalSeconds) {
					World.Update ();
					lastUpdate = clock.Elapsed.TotalSeconds;
				}

				if (lastGc + 30 < clock.Elapsed.TotalSeconds) {
					World.ForceSave ();
					GC.Collect ();
					lastGc = clock.Elapsed.TotalSeconds;
				}

				if (lastSaveAll + 60 < clock.Elapsed.TotalSeconds) {
					SaveAll ();
					lastSaveAll = clock.Elapsed.TotalSeconds;
				}
				// Rest
				Thread.Sleep (100);
			}

			World.ForceSave ();
		}

		public void SaveAll ()
		{
			foreach (Player p in PlayerList) {
				XMC.Log ("Saving all players...");
				p.SavePlayer ();
			}
		}

		public void Spawn (Player player)
		{
			MessageAll (Color.Announce + player.Username + " has joined");
		}

		private static void Monitor ()
		{
			PerformanceCounter cpuCounter = new PerformanceCounter ();
			cpuCounter.CategoryName = "Processor";
			cpuCounter.CounterName = "% Processor Time";
			cpuCounter.InstanceName = "_Total";

			var startTime = DateTime.Now;
			while (XMC.Server.Running) {
				try {
					Thread.Sleep (2000);

					Console.Title =
                "XMC || CPU: " + (cpuCounter.NextValue () + "%") +
					" || Memory: " + (GC.GetTotalMemory (false) / 1024) + "KB" + "]";
				} catch (Exception ex) {
					Console.WriteLine (ex);
				}
			}
		}
		// ====================
		// Private helpers.
		private void AcceptConnection (TcpClient client)
		{
			Player p = new Player (client);
			PlayerList.Add (p);
		}

		#endregion Methods
	}
}