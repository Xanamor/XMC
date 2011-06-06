#region Header

//    OpenMC, a Minecraft SMP server.
//    Copyright (C) 2011 OpenMC. All rights reserved.
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

namespace OpenMC
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading;

    public class Player : Entity
    {
        #region Fields

        public Rank AccessRights;
        public string ChatTag;
        public CommandHandler CommandInstance;
        public Window CurrentWindow;
        public PlayerInventory Inventory;
        public int SlotSelected;
        public bool Spawned;
        public float TargetBlockHealth;
        public string Username;
        public List<Chunk> VisibleChunks;
        public List<Entity> VisibleEntities;
        public InventoryItem WindowHolding;
        public Connection _Conn;

        private InventoryItem[] _LastEquipment;

        #endregion Fields

        #region Constructors

        public Player(TcpClient client)
        {
            _Conn = new Connection(client, this);
            Username = "";
                    AccessRights = GetRights();
            Spawned = false;
            CurrentChunk = null;
            VisibleChunks = new List<Chunk>();
            VisibleEntities = new List<Entity>();
            Inventory = new PlayerInventory(this);
            CurrentWindow = null;
            WindowHolding = new InventoryItem(-1);
            _LastEquipment = new InventoryItem[5];

                    CommandInstance = null;

            for (int i = 0; i < 5; ++i) {
                _LastEquipment[i] = new InventoryItem(-1);
            }
        }

        #endregion Constructors

        #region Methods

        public void BlockChanged(int x, int y, int z, Block block)
        {
            _Conn.Transmit(PacketType.BlockChange, x, (sbyte) y, z, (sbyte) block, (sbyte) 0);
        }

        public override void Despawn()
        {
            SavePlayer();

            if (!Spawned) return;
            Spawned = false;
            OpenMC.Server.Despawn(this);
            base.Despawn();
                    //Clean the fucking PlayerList up, sick of working with disposed objects kthxbai
            OpenMC.Server.PlayerList.Remove(this);
        }

        public void Disconnect(string message)
        {
            OpenMC.Log("Saving player " + Username + "before disconnect");
            SavePlayer();
            _Conn.Disconnect(message);
        }

        public Rank GetRights()
        {
            string OpList = "rights.cfg";
                string[] PlayerListing = new string[2];
            Rank PlayerRank = Rank.Guest;
                if (File.Exists(OpList))
            {
                    string[] tmpList = File.ReadAllLines(OpList);

                    if (tmpList.Length > 0)
                        {
                for (int i = 0; i < tmpList.Length; i++)
                    {
                                PlayerListing = tmpList[i].Split(' ');
                    if (PlayerListing[0].ToLower() == Username.ToLower())
                        {
                            i = tmpList.Length;  //Make the for loop break after this run so it doesn't keep searching
                            switch(PlayerListing[1])
                            {
                            case "-1":
                                PlayerRank = Rank.Banned;
                                break;
                            case "0":
                                PlayerRank = Rank.Guest;
                                break;
                            case "1":
                                PlayerRank = Rank.Janitor;
                                break;
                            case "2":
                                PlayerRank = Rank.Operator;
                                break;
                            case "3":
                                PlayerRank = Rank.Admin;
                                break;
                            }
                        }
                    }
                }
            }
            return PlayerRank;
        }

        public void LoadPlayer()
        {
            Dictionary<int, string[]> dInventory = new Dictionary<int, string[]>();
            InventoryItem[] LoadedItems = new InventoryItem[45];

            string[] tmpFile;
            string[] tmpBuffer = new string[3];

            string SavePath = Constants.SSettings.PlayerSavePath + this.Username;
            string InventoryFile = "Inventory.dat";
            string InventoryPath = SavePath + "/" + InventoryFile;
            if (!Directory.Exists(SavePath))
            {
                OpenMC.Log("Player Data does not exist for " + Username + " cannot load");
                //just break if the save directory hasn't been made for this user (IE: has not been saved yet)
                return;
            }

            try
            {
                OpenMC.Log("Loading player " + Username);
                tmpFile = File.ReadAllLines(InventoryPath);
                for (int i = 0; i < tmpFile.Length; i++)
                {
                    dInventory.Add(i,tmpFile[i].Split(','));
                }

                for (int i = 0; i < dInventory.Count; i++)
                {
                    tmpBuffer = dInventory[i];

                    LoadedItems[i].Type = Convert.ToInt16(tmpBuffer[0]);
                    LoadedItems[i].Count = Convert.ToByte(tmpBuffer[1]);
                    LoadedItems[i].Damage = Convert.ToInt16(tmpBuffer[2]);

                    Inventory.AddItem(LoadedItems[i]);
                }
            }
            catch (System.Exception ex) { OpenMC.LogError(ex); }
        }

        public void OpenWindow(Window window)
        {
            _Conn.Transmit(PacketType.OpenWindow, (sbyte) window.ID, (sbyte) window.Type,
                window.Title, (sbyte) window.slots.Length);
            // TODO: Transmit items in the window.
        }

        public void PickupCollected(PickupEntity pickup, Player player)
        {
            _Conn.Transmit(PacketType.CollectItem, pickup.EntityID, player.EntityID);
        }

        //this is really to send to the whole server including your self
        public void RecvMessage(string message)
        {
            if (message[0] == '/')
                CommandInstance.Handle(message.Split(' '));
            else
            {
                OpenMC.Log("<" + Username + "> " + message);
                OpenMC.Server.MessageAll(ChatTag + message);
            }
        }

        public void RecvServerMessage(string message)
        {
            _Conn.Transmit(PacketType.Message, "<" + (Color.PrivateMsg + "SERVER" + Color.White) + "> " + message);
        }

        public void SavePlayer()
        {
            #region Varibles
            Queue<string> InventoryQueue = new Queue<string>();

            FileStream fs = null;

            string SavePath = "./players/" + this.Username;
            string InventoryPath = SavePath + "/" + "Inventory.dat";
            string DataPath = SavePath + "/" + "Data.dat";
            #endregion

            #region FileChecking

            if (!Directory.Exists(SavePath))
            {
                OpenMC.Log("Creating new save folder for user: " + Username);
                Directory.CreateDirectory(SavePath);
                File.Create(InventoryPath);
                File.Create(DataPath);
                return;
            }
            #endregion
            OpenMC.Log("Saving " + Username + "...");
                for (int i = 0; i < 45; ++i)
                {
                    InventoryQueue.Enqueue(BuildListing(i));
                }
                try
                {
                    fs = new FileStream(InventoryPath , FileMode.Truncate, FileAccess.Write);

                    while (InventoryQueue.Count > 0)
                    {
                        byte[] tmp = Encoding.Default.GetBytes(InventoryQueue.Dequeue());
                        fs.Write(tmp,0,tmp.Length);
                    }
                    fs.Close();
                }
                catch (System.Exception ex) { OpenMC.LogError(ex); }
                finally
                {
                    if (fs != null)
                        fs.Dispose();
                }
        }

        //This is when you recive a message
        public void SendMessage(string message)
        {
            _Conn.Transmit(PacketType.Message, message);
        }

        public void SetHolding(InventoryItem item)
        {
            // OpenMC.Log(this + " setting holding to " + item);
            _Conn.Transmit(PacketType.WindowSetSlot, (sbyte) -1, (short) -1, item);
            WindowHolding = item;
        }

        public void Spawn()
        {
            if (AccessRights == Rank.Banned)
                {
                this.Disconnect("That which can give life can also take life. (You are banned)");
                return;
            }

            CommandInstance = new CommandHandler(AccessRights, this);

            OpenMC.Server.Spawn(this);
            Spawned = true;
            CurrentChunk = null;

            //Move the spawnees around a bit to prevent people from getting stuck in each other
            X = OpenMC.Server.World.SpawnX + OpenMC.Random.NextDouble();
            Y = OpenMC.Server.World.SpawnY + OpenMC.Random.Next(1,3);
            Z = OpenMC.Server.World.SpawnZ + OpenMC.Random.NextDouble();

                    LoadPlayer();
            Update();

            _Conn.Transmit(PacketType.SpawnPosition, (int)X, (int)Y, (int)Z);
            _Conn.Transmit(PacketType.PlayerPositionLook, X, Y, Y, Z, (float) 0, (float) 0, (sbyte) 1);

            //Send MOTD from SSettings and set things that need to be set after the player is in the game world
            ChatTag = (RankInfo.RankColor(AccessRights) + "<" + Username + ">") + Color.White + " ";
            _Conn.Transmit(PacketType.Message, (Constants.SSettings.MOTD));
        }

        public override string ToString()
        {
            return "[Entity.Player " + EntityID + ": " + Username + "]";
        }

        public override void Update()
        {
            if (!Spawned) { return; }
            Chunk newChunk = OpenMC.Server.World.GetChunkAt((int)X, (int)Z);

            if (newChunk != CurrentChunk) {
                List<Chunk> newVisibleChunks = new List<Chunk>();

                foreach (Chunk c in OpenMC.Server.World.GetChunksInRange(newChunk)) {
                    newVisibleChunks.Add(c);
                }
                foreach (Chunk c in VisibleChunks) {
                    if (!newVisibleChunks.Contains(c)) {
                        _Conn.Transmit(PacketType.PreChunk, c.ChunkX, c.ChunkZ, (sbyte) 0);
                    }
                }
                foreach (Chunk c in newVisibleChunks) {
                    if (!VisibleChunks.Contains(c)) {
                        _Conn.SendChunk(c);
                    }
                }

                VisibleChunks = newVisibleChunks;
            }

            List<Entity> newVisibleEntities = new List<Entity>();
            foreach (Chunk c in VisibleChunks) {
                foreach (Entity e in c.Entities) {
                    newVisibleEntities.Add(e);
                }
            }
            foreach (Entity e in VisibleEntities) {
                if (!newVisibleEntities.Contains(e)) {
                    DespawnEntity(e);
                }
            }
            foreach (Entity e in newVisibleEntities) {
                if (!VisibleEntities.Contains(e)) {
                    SpawnEntity(e);
                }
            }
            VisibleEntities = newVisibleEntities;

            if (Inventory.slots[36 + SlotSelected].Type != _LastEquipment[0].Type) {
                _LastEquipment[0] = Inventory.slots[36 + SlotSelected];
                foreach (Player p in OpenMC.Server.PlayerList) {
                    if (p != this && p.VisibleEntities.Contains(this)) {
                        p._Conn.Transmit(PacketType.EntityEquipment, EntityID,
                            (short) 0, (short) _LastEquipment[0].Type,
                            (short) _LastEquipment[0].Damage);
                    }
                }
            }

            for (int i = 0; i < 4; ++i) {
                if (Inventory.slots[5 + i].Type != _LastEquipment[i + 1].Type) {
                    foreach (Player p in OpenMC.Server.PlayerList) {
                        if (p != this && p.VisibleEntities.Contains(this)) {
                            p._Conn.Transmit(PacketType.EntityEquipment, EntityID,
                                (short) (i + 1), _LastEquipment[i + 1].Type,
                                _LastEquipment[i + 1].Damage);
                        }
                    }
                }
            }

            _Conn.Transmit(PacketType.TimeUpdate, OpenMC.Server.World.Time);
            base.Update();
        }

        public void UpdateEntity(Entity e, double dx, double dy, double dz, bool rotchanged, bool forceabs)
        {
            if (!Spawned) return;
            if (dx == 0 && dy == 0 && dz == 0) {
                if (rotchanged) {
                    _Conn.Transmit(PacketType.EntityLook, e.EntityID, (sbyte) e.Yaw, (sbyte) e.Pitch);
                }
            } else if (Math.Abs(dx) < 4 && Math.Abs(dy) < 4 && Math.Abs(dz) < 4 && !forceabs) {
                if (rotchanged) {
                    _Conn.Transmit(PacketType.EntityLookAndMove, e.EntityID,
                        (sbyte) (dx * 32), (sbyte) (dy * 32), (sbyte) (dz * 32),
                        (sbyte) e.Yaw, (sbyte) e.Pitch);
                } else {
                    _Conn.Transmit(PacketType.EntityRelativeMove, e.EntityID,
                        (sbyte) (dx * 32), (sbyte) (dy * 32), (sbyte) (dz * 32));
                }
            } else {
                _Conn.Transmit(PacketType.EntityTeleport, e.EntityID,
                    (int) (e.X * 32), (int) (e.Y * 32), (int) (e.Z * 32),
                    (sbyte) e.Yaw, (sbyte) e.Pitch);
            }
        }

        public void WindowSetSlot(Window window, short slot, InventoryItem item)
        {
            // OpenMC.Log(this + " setting slot " + slot + " of " + window.ID +  " to " + item);
            _Conn.Transmit(PacketType.WindowSetSlot, (sbyte) window.ID, slot, item);
        }

        private string BuildListing(int InventorySlot)
        {
            string Listing;

            Listing  = Inventory.slots[InventorySlot].Type + ",";
            Listing += Inventory.slots[InventorySlot].Count + ",";
            Listing += Inventory.slots[InventorySlot].Damage + "\n";

            return Listing;
        }

        private void DespawnEntity(Entity e)
        {
            if (!Spawned || e == this) return;
            _Conn.Transmit(PacketType.DestroyEntity, e.EntityID);
        }

        private void SpawnEntity(Entity e)
        {
            if (!Spawned || e == this) return;

            if (e is Player) {
                Player p = (Player) e;
                _Conn.Transmit(PacketType.NamedEntitySpawn, p.EntityID,
                    p.Username, (int)(p.X * 32), (int)(p.Y * 32), (int)(p.Z * 32),
                    p.Yaw, p.Pitch, (short) Block.Brick);
            } else if (e is PickupEntity) {
                PickupEntity p = (PickupEntity) e;
                _Conn.Transmit(PacketType.PickupSpawn, p.EntityID,
                    p.Item.Type, (sbyte) p.Item.Count, p.Item.Damage,
                    (int)(p.X * 32), (int)(p.Y * 32), (int)(p.Z * 32),
                    p.Yaw, p.Pitch, (sbyte) 0);
            } else {
                SendMessage(Color.Purple + "Spawning " + e);
                return;
            }
            _Conn.Transmit(PacketType.Entity, e.EntityID);
        }

        #endregion Methods
    }
}