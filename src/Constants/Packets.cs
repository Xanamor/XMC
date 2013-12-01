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

    #region Enumerations

    public enum PacketType : byte
    {
        KeepAlive = 0x00,           
        LoginRequest = 0x01,       
        Handshake = 0x02,           
        ChatMsg = 0x03,             
        TimeUpdate = 0x04,          
        EntityEquipment = 0x05,     
        SpawnPosition = 0x06,       
        UseEntity = 0x07,		
        UpdateHealth = 0x08,		
        Respawn = 0x09,				
        Player = 0x0A,              
        PlayerPosition = 0x0B,      
        PlayerLook = 0x0C,          
        PlayerPositionLook = 0x0D,  
        PlayerDigging = 0x0E,       
        PlayerBlockPlace = 0x0F,    
        PlayerHolding = 0x10,       
        PlayerUseBed  = 0x11,       
        ArmAnimation = 0x12,        
        EntityAction = 0x13,		
        NamedEntitySpawn = 0x14,    
        //SpawnDropedItem = 0x15,         
        CollectItem = 0x16,         
        VehicleSpawn = 0x17,        
        MobSpawn = 0x18,            
        PaintingSpawn = 0x19,		
        SpawnExpOrb = 0x1A,
        SteerVehicle = 0x1B,
        EntityVelocity = 0x1C,		
        DestroyEntity = 0x1D,       
        Entity = 0x1E,              
        EntityRelativeMove = 0x1F,  
        EntityLook = 0x20,         
        EntityLookAndMove = 0x21,   
        EntityTeleport = 0x22,      
	    EntityHeadLook = 0x23,
        EntityStatus = 0x26,		
        AttachEntity = 0x27,		
        EntityMetadata = 0x28,		
        EntityEffect = 0x29,
	    RemoveEntityEffect = 0x2A,
	    SetExpOrb = 0x2B,
        EntityProperties = 0x2C,
        //MapColumnAllocation = 0x32,            
        MapChunkData = 0x33,            
        MultiBlockChange = 0x34,    
        BlockChange = 0x35,         
        BlockAction = 0x36,
		BlockBreakAnim = 0x37,
        MapChunkBulk = 0x38,
        Explosion = 0x3C,		
	    SoundParticleEffect = 0x3D,
        NamedSndEffect = 0x3E,
        Particle = 0x3F,
	    ChangeGameState = 0x46,
	    SpawnGlobalEntity = 0x47,
        OpenWindow = 0x64,						
        WindowClick	= 0x66,			
        WindowSetSlot = 0x67,		
        WindowItems = 0x68,			
        WindowProgress = 0x69,		
        Transaction = 0x6A,			
        CreativeInventoryAction = 0x6B,
	    EnchantItem = 0x6C,
        UpdateSign = 0x82,
	    ItemData = 0x83,
	    UpdateTileEntity = 0x84,
        TileEditorOpen = 0x85,
	    IncrementStatistic = 0xC8,
	    PlayerListItem = 0xC9,
	    PlayerAbilities = 0xCA,
        TabComplete = 0xCB,
        ClientSettings = 0xCC,
        ClientStatuses = 0xCD,
        UpdateScore = 0xCF,
        DisplayScore = 0xD0,
        Teams = 0xD1,
	    PluginMessage = 0xFA,
	    EncryptionKeyResponse = 0xFC,
	    EncryptionKeyRequest = 0xFD,
	    ServerListPing = 0xFE,
        Disconnect = 0xFF         
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
            "bi",			// keep alive - 0x00
            "bitbbbbb",			// login request - 0x01
            "bbtti",			// handshake - 0x02
            "bt",			// chat message 0x03
            "bll",			// time update 0x04
            "bisss",			// entity equipment 0x05
            "biii",			// spawn position 0x06
            "biib",			// interact entity 0x07
            "bssf",			// update health 0x08
            "bibbst",			// respawn 0x09
            "bb",			// player base 0x0a
            "bddddb",			// player position 0x0b
            "bffb",			// player look 0x0c
            "bddddffb",			// player position+look 0x0d
            "bbibib",			// player digging 0x0e
            "bibibI",			// player block place 0x0f
            "bs",			// player holding 0x10
            "bibibi",			// Use Bed 0x11
            "bib",			// arm animation 0x12
            "bib",			// entity action 0x13
            "bitiiibbs",		// named entity spawn 0x14
            "bisbsiiibbb",		// pickup spawn 0x15
            "bii",			// collect item 0x16
            "bibiiiisss",		// vehicle spawn 0x17
            "bibiiibbbM",		// mob spawn 0x18
            "bisiiii",			// painting spawn 0x19
            "biiiis",			// Spawn Exp orb 0x1A
            "",				// 0x1B
            "bisss",			// entity velocity 0x1C
            "bi",			// destroy entity 0x1D
            "bi",			// entity base 0x1E
            "bibbb",			// entity relative move 0x1F
            "bibb",			// entity look 0x20
            "bibbbbb",			// entity look+move 0x21
            "biiiibb",			// entity teleport 0x22
            "bib", 			// Entity Head Look 0x23
            "", 			// Unused 0x24
	    "", 			// Unused 0x25
            "bib",			// entity status 0x26
            "bii",			// attach entity 0x27
            "biM",			// entity metadata 0x28
            "bibbs", 			// entity effect 0x29
	    "bib", 			// remove entity effect 0x2A
	    "", 			// Set exp bar 0x2B
	    "",		        	// 0x2C
            "", 			// 0x2D
	    "", 			// 0x2E
	    "", 			// 0x2F
	    "", 			// 0x30
	    "",         		// 0x31
            "biib",			// map column allocation 0x32
            "biibssiix",		// mapchunk 0x33
            "biisxxx",			// multi-block change 0x34
            "bibibb",			// block change 0x35
            "bisibb",			// block action 0x36
            "", 			// 0x37
	    "", 			// 0x38
	    "",				// 0x39
	    "",				// 0x3A
	    "",				// 0x3B
            "bdddfix",			// explosion 0x3C
            "biibii", 			// Sound/Particle effect 0x3D
	    "", 			// 0x3E
	    "", 			// 0x3F
	    "", 			// 0x40
	    "",				// 0x41
            "", 			// 0x42
	    "", 			// 0x43
	    "", 			// 0x44
	    "", 			// 0x45
	    "bbb",			// Change game state 0x46
            "bibiii", 			// Thunderbolt 0x47
	    "",				// 0x48
	    "",				// 0x49
	    "", 			// 0x4A
	    "",				// 0x4B
            "", 			// 0x4C
	    "", 			// 0x4D
	    "", 			// 0x4E
	    "", 			// 0x4F
	    "",				// 0x50
            "", 			// 0x51
	    "", 			// 0x52
	    "", 			// 0x53
	    "", 			// 0x54
	    "",				// 0x55
            "", 			// 0x56
	    "", 			// 0x57
	    "", 			// 0x58
	    "", 			// 0x59
	    "", 			// 0x5A
	    "",				// 0x5B
	    "", 			// 0x5C
	    "", 			// 0x5D
	    "",				// 0x5E
            "", 			// 0x5F
	    "",				// 0x60
	    "",				// 0x61
	    "",		   		// 0x62
            "",				// 0x63  
            "bbbtb",			// open window 0x64
            "bb",			// close window 0x65
            "bbsbsbI",			// click window 0x66
            "bbsI",			// set slot 0x67
            "bbsX",			// set Window items 0x68
            "bbss", 			// Update Window Property 0x69
	    "bbsb", 			// Confirm Transaction 0x6A
	    "bsI", 			// Creative Inventory Action 0x6B
	    "bbb", 			// Enchant Item 0x6C
	    "",				// 0x6D
            "", "", "", "", "",         // 0x6E -> 0x72
            "", "", "", "", "",		// 0x73 -> 0x77
            "", "", "", "", "",		// 0x78 -> 0x7C
	    "", "", "", "", "",		// 0x7D -> 0x81
            "bisitttt",			// update sign 0x82
	    "bssbX",			// Item Data 0x83
	    "bisibiii",			// Update Tile Entity 0x84
	    "", "", "", "", "",		// 0x85 -> 0x89
	    "", "", "", "", "",		// 0x8A -> 0x8E
	    "", "", "", "", "",		// 0x8F -> 0x94
	    "", "", "", "", "",		// 0x95 -> 0x99
	    "", "", "", "", "",		// 0x9A -> 0x9E
	    "", "", "", "", "",		// 0x9F -> 0xA4
	    "", "", "", "", "",		// 0xA5 -> 0xA9
	    "", "", "", "", "",		// 0xAA -> 0xAE
	    "", "", "", "", "",		// 0xAF -> 0xB4
	    "", "", "", "", "",		// 0xB5 -> 0xB9
	    "", "", "", "", "",		// 0xBA -> 0xBE
	    "", "", "", "", "",		// 0xBF -> 0xC4
	    "", "", "", 		// 0xC5 -> 0xC7
	    "bib", 			// Increment Statistic 0xC8
	    "",				// Player List Item 0xC9
	    "",				// Player Abilitys 0xCA
	    "", "", "", "", "",		// 0xCB -> 0xCF
	    "", "", "", "", "",		// 0xD0 -> 0xD4
	    "", "", "", "", "",		// 0xD5 -> 0xD9
	    "", "", "", "", "",		// 0xDA -> 0xDE
	    "", "", "", "", "",		// 0xDF -> 0xE3
	    "", "", "", "", "",		// 0xE4 -> 0xE8
	    "", "", "", "", "",		// 0xE9 -> 0xED
	    "", "", "", "", "",		// 0xEE -> 0xF2
	    "", "", "", "", "",		// 0xF3 -> 0xF7
	    "", "", 			// 0xF8, 0xF9
	    "btsX", 			// Plugin Message 0xFA   
	    "", "",			// 0xFB, 0xFC
	    "", 			// 0xFD
	    "",				// Server List Ping 0xFE -- Server Description + \x00A7 + Number Of Users + \x00A7 + Number of Slots
	    "bt",			// Disconnect/Kick 0xFF
            // special handling for disconnect
        };

        #endregion Fields
    }
}