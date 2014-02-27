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

namespace Net
{
	using System;

	#region Enumerations
	public enum OutPacketType : byte
	{
		/// <summary>
		/// The handshake. Also KeepAlive
		/// </summary>
		Handshake = 0x00,
		JoinGame = 0x01,
		ChatMsg = 0x02,
		TimeUpdate = 0x03,
		EntityEquipment = 0x04,
		SpawnPosition = 0x05,
		UpdateHealth = 0x06,
		Respawn = 0x07,
		PlayerPos = 0x08,
		HeldItemChg = 0x09,
		UseBed = 0x0A,
		Animation = 0x0B,
		SpawnPlayer = 0x0C,
		CollectItem = 0x0D,
		SpawnObject = 0x0E,
		SpawnMob = 0x0F,
		SpawnPainting	= 0x10,
		SpawnXPOrb = 0x11,
		EntityVelocity	= 0x12,
		DestroyEntites	= 0x13,
		Entity = 0x14,
		EntityRelMove	= 0x15,
		EntityLook = 0x16,
		EntityLookMove	= 0x17,
		EntityTeleport	= 0x18,
		EntityHeadLook	= 0x19,
		EntityStatus	= 0x1A,
		AttachEntity	= 0x1B,
		EntityMetadata	= 0x1C,
		EntityEffect	= 0x1D,
		RmEntityEffect	= 0x1D,
		SetXP = 0x1F,
		EntityProps = 0x20,
		ChunkData = 0x21,
		MultiBlockChg	= 0x22,
		BlockChg = 0x23,
		BlockAction = 0x24,
		BlockBreakAnim	= 0x25,
		MapChunkBulk	= 0x26,
		Explosion = 0x27,
		PlayEffect = 0x28,
		PlaySound = 0x29,
		ChgGameState	= 0x2B,
		SpawnGlbEntity	= 0x2C,
		OpenWindow = 0x2D,
		CloseWindow = 0x2E,
		SetSlot = 0x2F,
		WindowItems = 0x30,
		WindowProperty	= 0x31,
		ConfirmTransact	= 0x32,
		UpdateSign = 0x33,
		Maps = 0x34,
		UpdateBlkEntity	= 0x35,
		SignEditorOpen	= 0x36,
		Statistics = 0x37,
		PlayerList = 0x38,
		PlayerAbilities = 0x39,
		TabComplete = 0x3A,
		ScoreboardObj	= 0x3B,
		UpdateScore = 0x3C,
		DisplayScore	= 0x3D,
		TeamFunc = 0x3E,
		Disconnect = 0x40
	}

	public enum InPacketType : byte
	{
		KeepAlive = 0x00,
		ChatMessage = 0x01,
		UseEntity = 0x02,
		Player = 0x03,
		PlayerPos = 0x04,
		PlayerLook = 0x05,
		PlayerPosLook = 0x06,
		PlayerDigging = 0x07,
		PlayerBlkPlace = 0x08,
		HeldItemChg = 0x09,
		Animation = 0x0A,
		EntityAction	= 0x0B,
		SteerVehicle	= 0x0C,
		ClickWindow = 0x0D,
		CloseWindow = 0x0E,
		ConfirmTransAct	= 0x0F,
		CreateInvAct	= 0x10,
		EnchantItem = 0x11,
		UpdateSign = 0x12,
		PlayerAbilities	= 0x13,
		TabComplete = 0x14,
		ClientSettings	= 0x15,
		ClientStatus	= 0x16,
		PluginMessage	= 0x17,
	}
	#endregion Enumerations
	public static class PacketStructure
	{
		#region Fields

		// b - byte(1)
		// s - short(2)
		// i - int(4)
		// l - long(8)
		// f - float(4)
		// d - double(8)
		// t - string - short-prefixed
		// x - bytearray - special handling
		// M - entity metadata - special handling
		// I - inventory item - special handling (short; then if not -1: byte, short)
		public static string[] Data = {
            
		};

		#endregion Fields
	}
}