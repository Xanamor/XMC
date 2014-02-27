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

namespace XMC
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.IO.Compression;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using zlib;

	public class Connection
	{
		#region Fields

		public string IPString;
		private byte[] _Buffer;
		private TcpClient _Client;
		private Player _Player;
		private string _QuitMsg;
		private bool _Running;
		private Thread _Thread;
		private Queue<byte[]> _TransmitQueue;
		private StringBuilder _StringBuilder;

		#endregion Fields

		#region Constructors

		public Connection (TcpClient client, Player player)
		{
			_Client = client;
			IPString = _Client.Client.RemoteEndPoint.ToString ();

			_Running = true;
			_TransmitQueue = new Queue<byte[]> ();
			_Buffer = new byte[0];
			_Player = player;

			_Thread = new Thread (ConnectionThread);
			_Thread.Name = "XMC-Player " + _Client.GetHashCode ();
			_Thread.Start ();
			_StringBuilder = new StringBuilder ();
		}

		#endregion Constructors

		#region Methods

		public void Disconnect (string message)
		{
			_QuitMsg = message;
			lock (_TransmitQueue) {
				_TransmitQueue.Clear ();
			}
			Transmit (Net.OutPacketType.Disconnect, message);
			lock (_TransmitQueue) {
				TransmitRaw (_TransmitQueue.Dequeue ());
			}
			_Client.GetStream ().Flush ();
			_Client.Close ();
		}

		public void SendChunk (Chunk chunk)
		{
         
		}

		public void Stop ()
		{
			_Running = false;
		}

		public void Transmit (Net.OutPacketType type, params object[] args)
		{
	    
		}

		private void ConnectionThread ()
		{
			Stopwatch clock = new Stopwatch ();
			clock.Start ();
			double lastKeepAlive = 0;

			while (_Running) {
				try {
					while (_TransmitQueue.Count > 0) {
						byte[] next;
						lock (_TransmitQueue) {
							next = _TransmitQueue.Dequeue ();
						}
						TransmitRaw (next);
					}

					if (!_Client.Connected) {
						_Client.Close ();
						_Running = false;
						break;
					}

					if (_Client.GetStream ().DataAvailable) {
						IncomingData ();
					}

					if (lastKeepAlive + 20 < clock.Elapsed.TotalSeconds) {
						Transmit (PacketType.KeepAlive);
						lastKeepAlive = clock.Elapsed.TotalSeconds;
					}

					Thread.Sleep (30);
				} catch (Exception e) {
					XMC.LogError (e);
					Disconnect ("Server error: " + e.Message);
					_Running = false;
				}
			}
			if (_Player.Spawned) {
				XMC.Log (_Player.Username + " has left (" + _QuitMsg + ")");
				_Player.Despawn ();
			} else {
				XMC.Log ("/" + IPString + " disconnected (" + _QuitMsg + ")");
			}
		}

		private void GetOffsetPos (ref int x, ref sbyte y, ref int z, int face)
		{
			switch (face) {
			case 0:
				--y;
				break;
			case 1:
				++y;
				break;
			case 2:
				--z;
				break;
			case 3:
				++z;
				break;
			case 4:
				--x;
				break;
			case 5:
				++x;
				break;
			default:
				break;
			}
		}

		private void IncomingData ()
		{
			NetworkStream stream = _Client.GetStream ();
			Builder<byte> buffer = new Builder<byte> ();
			buffer.Append (_Buffer);

			while (stream.DataAvailable) {
				buffer.Append ((byte)stream.ReadByte ());
			}

			_Buffer = buffer.ToArray ();
			buffer = null;

			while (_Buffer.Length > 0) {
				Pair<int, object[]> pair = CheckCompletePacket ();
				int length = pair.First;
				if (length > 0) {
					byte[] newBuffer = new byte[_Buffer.Length - length];
					Array.Copy (_Buffer, length, newBuffer, 0, _Buffer.Length - length);
					_Buffer = newBuffer;

					ProcessPacket (pair.Second);
				} else {
					break;
				}
			}
		}

		private void ProcessPacket (object[] packet)
		{
			Net.InPacketType type = (Net.InPacketType)(byte)packet [0];
			XMC.Log ("Recieved Packet " + type + " (" + (byte)type + ")");
			switch (type) {
			case (Net.InPacketType.KeepAlive):
				break;
			case (Net.InPacketType.ChatMessage):
				break;
			case (Net.InPacketType.UseEntity):
				break;
			case (Net.InPacketType.Player):
				break;
			case (Net.InPacketType.PlayerPos):
				break;
			case (Net.InPacketType.PlayerLook):
				break;
			case (Net.InPacketType.PlayerPosLook):
				break;
			case (Net.InPacketType.PlayerDigging):
				break;
			case (Net.InPacketType.PlayerBlkPlace):
				break;
			case (Net.InPacketType.HeldItemChg):
				break;
			case (Net.InPacketType.Animation):
				break;
			case (Net.InPacketType.EntityAction):
				break;
			case (Net.InPacketType.SteerVehicle):
				break;
			case (Net.InPacketType.ClickWindow):
				break;
			case (Net.InPacketType.CloseWindow):
				break;
			case (Net.InPacketType.ConfirmTransAct):
				break;
			case (Net.InPacketType.CreateInvAct):
				break;
			case (Net.InPacketType.EnchantItem):
				break;
			case (Net.InPacketType.UpdateSign):
				break;
			case (Net.InPacketType.PlayerAbilities):
				break;
			case (Net.InPacketType.TabComplete):
				break;
			case (Net.InPacketType.ClientSettings):
				break;
			case (Net.InPacketType.ClientStatus):
				break;
			case (Net.InPacketType.PluginMessage):
				break;
			default:
				XMC.Log ("[Packet] " + _Player.Username + " sent Unknown packet (Wrong Version?)" + type);
				break;
			}
		}

		private void TransmitRaw (byte[] packet)
		{
			try {
				_Client.GetStream ().Write (packet, 0, packet.Length);
			} catch (Exception) {
				_Client.Close ();
				_Running = false;
			}
		}

		#endregion Methods
	}
}