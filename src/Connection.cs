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

        public void Disconnect(string message)
        {
            _QuitMsg = message;
            lock (_TransmitQueue) {
                _TransmitQueue.Clear();
            }
            Transmit(Net.OutPacketType.Disconnect, message);
            lock (_TransmitQueue) {
                TransmitRaw(_TransmitQueue.Dequeue());
            }
            _Client.GetStream().Flush();
            _Client.Close();
        }

        public void SendChunk(Chunk chunk)
        {
         
        }

        public void Stop()
        {
            _Running = false;
        }

        public void Transmit(PacketType type, params object[] args)
        {
	    XMC.Log("Transmitting: " + type + "(" + (byte)type + ")");
            string structure = (type == PacketType.Disconnect ? "bt" : PacketStructure.Data[(byte) type]);

            Builder<Byte> packet = new Builder<Byte>();
            packet.Append((byte) type);

            byte[] bytes;
            int current = 1;
            try {
                for (int i = 1; i < structure.Length; ++i) {
                    current = i;
                    switch (structure[i]) {
                        case 'b':		// sbyte(1)
                            packet.Append((byte) (sbyte) args[i-1]);
                            break;

                        case 's':		// short(2)
                            packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short) args[i-1])));
                            break;

                        case 'f':		// float(4)
                            bytes = BitConverter.GetBytes((float) args[i-1]);
                            for (int j = 3; j >= 0; --j) {
                                packet.Append(bytes[j]);
                            }
                            //packet.Append(bytes);
                            break;

                        case 'i':		// int(4)
                            packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int) args[i-1])));
                            break;

                        case 'd':		// double(8)
                            bytes = BitConverter.GetBytes((double) args[i-1]);
                            for (int j = 7; j >= 0; --j) {
                                packet.Append(bytes[j]);
                            }
                            //packet.Append(bytes);
                            break;

                        case 'l':		// long(8)
                            packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long) args[i-1])));
                            break;

                        case 't':		// string
                            bytes = Encoding.BigEndianUnicode.GetBytes((string)args[i-1]);
                            packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)bytes.Length)));
                            packet.Append(bytes);
                            break;

                        case 'x':		// byte array
                            packet.Append((byte[]) args[i-1]);
                            break;

                        case 'I':		// inventory entity
                            // short; if nonnegative, byte then short.
                            InventoryItem item = (InventoryItem) args[i - 1];
                            packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(item.Type)));
                            if (item.Type >= 0) {
                                packet.Append(item.Count);
                                packet.Append(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(item.Damage)));
                            }
                            break;

                        default:
                            throw new NotImplementedException("Unimplemented data type (transmit)");
                    }
                }
            }
            catch (InvalidCastException) {
                XMC.Log("[Error] Transmitting " + type + ": expected '" + structure[current] +
                    "', got " + args[current - 1].GetType().ToString() + " for argument " + current + " (format: " + structure + ")");
                throw;
            }
            lock (_TransmitQueue) {
                _TransmitQueue.Enqueue(packet.ToArray());
            }
        }

    	private Pair<int, object[]> CheckCompletePacket () {
	    	Pair<int, object[]> nPair = new Pair<int, object[]> (0, null);

	    	Net.InPacketType type = (PacketType)_Buffer [0];
			if (_Buffer [0] >= PacketStructure.Data.Length && _Buffer [0] != 0xFF) {
				XMC.Log ("Got invalid packet: " + _Buffer [0]);
				return nPair;
	    }

	    string structure = (type == PacketType.Disconnect ? "bt" : PacketStructure.Data [_Buffer [0]]);
	    int bufPos = 0;
	    Builder<object> data = new Builder<object> ();
	    byte[] bytes = new byte[8];

	    for (int i = 0; i < structure.Length; ++i) {
		switch (structure [i]) {
		case 'b':		// sbyte(1)
		    if ((bufPos + 1) > _Buffer.Length)
			return nPair;
		    if (i == 0)
			data.Append ((byte)_Buffer [bufPos]);
		    else
			data.Append ((sbyte)_Buffer [bufPos]);
		    bufPos += 1;
		    break;

		case 's':		// short(2)
		    if ((bufPos + 2) > _Buffer.Length)
			return nPair;
		    data.Append ((short)IPAddress.NetworkToHostOrder (BitConverter.ToInt16 (
			_Buffer,
			bufPos
		    )
		    )
		    );
		    bufPos += 2;
		    break;

		case 'f':		// float(4)
		    if ((bufPos + 4) > _Buffer.Length)
			return nPair;
		    for (int j = 0; j < 4; ++j) {
			bytes [j] = _Buffer [bufPos + 3 - j];
		    }
		    data.Append ((float)BitConverter.ToSingle (bytes, 0));
		    bufPos += 4;
		    break;
		case 'i':		// int(4)
		    if ((bufPos + 4) > _Buffer.Length)
			return nPair;
		    data.Append ((int)IPAddress.NetworkToHostOrder (BitConverter.ToInt32 (
			_Buffer,
			bufPos
		    )
		    )
		    );
		    bufPos += 4;
		    break;

		case 'd':		// double(8)
		    if ((bufPos + 8) > _Buffer.Length)
			return nPair;
		    for (int j = 0; j < 8; ++j) {
			bytes [j] = _Buffer [bufPos + 7 - j];
		    }
		    data.Append ((double)BitConverter.ToDouble (bytes, 0));
		    bufPos += 8;
		    break;
		case 'l':		// long(8)
		    if ((bufPos + 8) > _Buffer.Length)
			return nPair;
		    data.Append ((long)IPAddress.NetworkToHostOrder (BitConverter.ToInt64 (
			_Buffer,
			bufPos
		    )
		    )
		    );
		    bufPos += 8;
		    break;

		case 't':		// string
		    if ((bufPos + 2) > _Buffer.Length)
			return nPair;
		    short len = IPAddress.NetworkToHostOrder (BitConverter.ToInt16 (
			_Buffer,
			bufPos
		    )
		    );
		    if ((bufPos + 2 + len) > _Buffer.Length)
			return nPair;
		    data.Append ((string)Encoding.BigEndianUnicode.GetString(_Buffer, bufPos + 2, len));
                        bufPos += (2 + len);
                        break;

                    case 'I':		// inventory entity
                        // short; if nonnegative, byte then short.
                        InventoryItem item;
                        if ((bufPos + 2) > _Buffer.Length) return nPair;
                        item.Type = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_Buffer, bufPos));
                        if (item.Type >= 0) {
                            if ((bufPos + 3) > _Buffer.Length) return nPair;
                            item.Count = _Buffer[bufPos + 2];
                            item.Damage = (short) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(_Buffer, bufPos + 3));
                            bufPos += 5;
                        } else {
                            item.Count = 0;
                            item.Damage = 0;
                            bufPos += 2;
                        }
                        data.Append(item);
                        break;

                    default:
                        throw new NotImplementedException("Unimplemented data type (recieve)");
                }
            }

            return new Pair<int, object[]>(bufPos, data.ToArray());
        }

        private void ConnectionThread()
        {
            Stopwatch clock = new Stopwatch();
            clock.Start();
            double lastKeepAlive = 0;

            while (_Running) {
                try {
                    while (_TransmitQueue.Count > 0) {
                        byte[] next;
                        lock (_TransmitQueue) {
                            next = _TransmitQueue.Dequeue();
                        }
                        TransmitRaw(next);
                    }

                    if (!_Client.Connected) {
                        _Client.Close();
                        _Running = false;
                        break;
                    }

                    if (_Client.GetStream().DataAvailable) {
                        IncomingData();
                    }

                    if (lastKeepAlive + 20 < clock.Elapsed.TotalSeconds) {
                        Transmit(PacketType.KeepAlive);
                        lastKeepAlive = clock.Elapsed.TotalSeconds;
                    }

                    Thread.Sleep(30);
                }
                catch (Exception e) {
						XMC.LogError(e);
                        Disconnect("Server error: " + e.Message);
						_Running = false;
                    }
            }
            if (_Player.Spawned) {
                XMC.Log(_Player.Username + " has left (" + _QuitMsg + ")");
                _Player.Despawn();
            } else {
                XMC.Log("/" + IPString + " disconnected (" + _QuitMsg + ")");
            }
        }

        private void GetOffsetPos(ref int x, ref sbyte y, ref int z, int face)
        {
            switch(face) {
                case 0: --y; break;
                case 1: ++y; break;
                case 2: --z; break;
                case 3: ++z; break;
                case 4: --x; break;
                case 5: ++x; break;
                default: break;
            }
        }

        private void IncomingData()
        {
            NetworkStream stream = _Client.GetStream();
            Builder<byte> buffer = new Builder<byte>();
            buffer.Append(_Buffer);

            while (stream.DataAvailable) {
                buffer.Append((byte) stream.ReadByte());
            }

            _Buffer = buffer.ToArray();
            buffer = null;

            while (_Buffer.Length > 0) {
                Pair<int, object[]> pair = CheckCompletePacket();
                int length = pair.First;
                if (length > 0) {
                    byte[] newBuffer = new byte[_Buffer.Length - length];
                    Array.Copy(_Buffer, length, newBuffer, 0, _Buffer.Length - length);
                    _Buffer = newBuffer;

                    ProcessPacket(pair.Second);
                } else {
                    break;
                }
            }
        }
private void ProcessPacket (object[] packet) {
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
                    XMC.Log("[Packet] " + _Player.Username + " sent Unknown packet (Wrong Version?)" + type);
                    break;
            }
        }

        private void TransmitRaw(byte[] packet)
        {
            try {
                _Client.GetStream().Write(packet, 0, packet.Length);
            }
            catch (Exception) {
                _Client.Close();
                _Running = false;
            }
        }

        #endregion Methods
    }
}