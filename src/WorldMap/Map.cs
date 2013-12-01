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
    using System.IO;
    using System.IO.Compression;

    using NBT;

    public class Map
    {
        #region Fields

        public string WorldName;

        private Dictionary<long, Chunk> _Chunks;
        private int _DespawnTimer;
        private BinaryTag _Structure;

        #endregion Fields

        #region Constructors

        public Map(string Name)
        {
            WorldName = Name;
            _DespawnTimer = 0;
            _Chunks = new Dictionary<long, Chunk> ();

            if (!Directory.Exists (Name))
                Directory.CreateDirectory (Name);
            if (File.Exists(Name + "/level.dat"))
                XMC.Log("Found level.dat, Parsing...");
            else {

                FileStream rawWriter = null;
                GZipStream Writer = null;
                LevelDat NbtData;

                XMC.LogWarrning("Could not find level.dat, assuming this is a new world");
                while(rawWriter == null)
                {
                    try {
                       rawWriter = new FileStream (Name + "/level.dat", FileMode.OpenOrCreate);
                    } catch {XMC.LogWarrning("Waiting for file to close..."); }

                    //Give the computer a bit of time to close the files
                    //HACK: Need to put in Async FileIO system
                    System.Threading.Thread.Sleep(500);
                    if(rawWriter != null)
                        Writer = new GZipStream(rawWriter, CompressionMode.Compress);
                }

                NbtData.Time = 0;
                NbtData.LastPlayed = 0;
                NbtData.SpawnX = XMC.Random.Next(-300,300);
                NbtData.SpawnY = 64;
                NbtData.SpawnZ = XMC.Random.Next (-300, 300);;
                NbtData.SizeOnDisk = 0;

                //TODO: write a rand number generator to generate longs
                NbtData.RandomSeed = XMC.Random.Next ();
                NbtData.LevelName = Name;

                //Must be the internal MC revision Notch applys
                NbtData.MCVersion = 74188;

                //These values were pulled from a existing level.dat to ensure compatiblity
                //This is the core part of the level.dat
                NbtData.tmpNbtStruct = new BinaryTag[] {
                    new BinaryTag(TagType.Int,NbtData.SpawnX,"SpawnX"),
                    new BinaryTag(TagType.Int,NbtData.SpawnY,"SpawnY"),
                    new BinaryTag(TagType.Int,NbtData.SpawnZ,"SpawnZ"),
                    new BinaryTag(TagType.Int,NbtData.MCVersion,"Version"),
                    new BinaryTag(TagType.Long,NbtData.LastPlayed,"LastPlayed"),
                    new BinaryTag(TagType.Long,NbtData.Time,"Time"),
                    new BinaryTag(TagType.Long,NbtData.SizeOnDisk ,"SizeOnDisc"),
                    new BinaryTag(TagType.Long,NbtData.RandomSeed,"RandomSeed"),
                    new BinaryTag(TagType.String,NbtData.LevelName,"LevelName"),
                };

                //we do this to make sure the whole thing is wraped with TAG_LIST
                NbtData.NbtStruct = new BinaryTag(TagType.Compound, new BinaryTag[] {
                    new BinaryTag(TagType.Compound, NbtData.tmpNbtStruct,"Data")
                });

                //Write it all to the disk
                try {
                    XMC.Log("Writing level.dat to disc...");
                    while(!rawWriter.CanWrite)
                    {
                        System.Threading.Thread.Sleep(50);
                    }
                    NbtWriter.WriteTagStream(NbtData.NbtStruct, Writer);
                } catch (Exception ex) {
                    throw ex;
                }
                XMC.Log("Level.dat writen successfuly!");
                Writer.Close();
                rawWriter.Close();
            }

            StreamReader rawReader = new StreamReader (Name + "/level.dat");
            GZipStream reader = new GZipStream (rawReader.BaseStream, CompressionMode.Decompress);
            _Structure = NbtParser.ParseTagStream (reader);
            reader.Close ();
            rawReader.Close ();
        }

        #endregion Constructors

        #region Properties

        public int SpawnX
        {
            get { return (int)(_Structure["Data"]["SpawnX"].Payload); }
            set { _Structure["Data"]["SpawnX"].Payload = value; }
        }

        public int SpawnY
        {
            get { return (int)(_Structure["Data"]["SpawnY"].Payload); }
            set { _Structure["Data"]["SpawnY"].Payload = value; }
        }

        public int SpawnZ
        {
            get { return (int)(_Structure["Data"]["SpawnZ"].Payload); }
            set { _Structure["Data"]["SpawnZ"].Payload = value; }
        }

        public long Time
        {
            get { return (long)(_Structure["Data"]["Time"].Payload); }
            set { _Structure["Data"]["Time"].Payload = value; }
        }

        #endregion Properties

        #region Methods

        public List<Entity> EntitiesIn(Chunk c)
        {
            return c.Entities;
        }

        public void ForceSave()
        {
            foreach (KeyValuePair<long, Chunk> kvp in _Chunks) {
                kvp.Value.Save();
            }
        }

        public void Generate()
        {
        }

        public Chunk GetChunk(int chunkX, int chunkZ)
        {
            Builder<byte> b = new Builder<byte>();
            b.Append(BitConverter.GetBytes(chunkX));
            b.Append(BitConverter.GetBytes(chunkZ));
            long index = BitConverter.ToInt64(b.ToArray(), 0);
            if (_Chunks.ContainsKey(index)) {
                return _Chunks[index];
            } else {
                return _Chunks[index] = new Chunk(chunkX, chunkZ, this);
            }
        }

        public Chunk GetChunkAt(int blockX, int blockZ)
        {
            int xNeg = (blockX < 0 ? 1 : 0), zNeg = (blockZ < 0 ? 1 : 0);
            return GetChunk((int)((blockX + xNeg) / 16) - xNeg, (int)((blockZ + zNeg) / 16) - zNeg);
        }

        public List<Chunk> GetChunksInRange(Chunk c)
        {
            List<Chunk> r = new List<Chunk>();
            for (int x = c.ChunkX - 5; x <= c.ChunkX + 5; ++x) {
                for (int z = c.ChunkZ - 5; z <= c.ChunkZ + 5; ++z) {
                    if (Math.Abs(c.ChunkX - x) + Math.Abs(c.ChunkZ - z) < 8) {
                        r.Add(GetChunk(x, z));
                    }
                }
            }
            return r;
        }

        public void Update()
        {
            List<Entity> entities = new List<Entity>();
            foreach (KeyValuePair<long, Chunk> kvp in new Dictionary<long, Chunk>(_Chunks)) {
                foreach (Entity e in kvp.Value.Entities) {
                    entities.Add(e);
                }
            }

            // Update entities as needed
            foreach (Entity e in entities) {

                e.Update();
            }

            // Remove from memory chunks that nobody can see
            _DespawnTimer = (_DespawnTimer + 1) % 20;
            if (_DespawnTimer == 0) {
                List<Chunk> chunksVisible = new List<Chunk>();
                foreach (Entity e in entities) {
                    if (e is Player) {
                        foreach (Chunk c in GetChunksInRange(e.CurrentChunk)) {
                            chunksVisible.Add(c);
                        }
                    }
                }
                foreach (Chunk c in GetChunksInRange(GetChunkAt(SpawnX, SpawnZ))) {
                    chunksVisible.Add(c);
                }
                int i = 0;
                foreach (KeyValuePair<long, Chunk> kvp in new Dictionary<long, Chunk>(_Chunks)) {
                    if (!chunksVisible.Contains(kvp.Value)) {
                        _Chunks.Remove(kvp.Key);
                        ++i;
                    }
                }

            }
        }

        #endregion Methods

        #region Nested Types

        private struct LevelDat
        {
            #region Fields

            public long LastPlayed, Time, SizeOnDisk, RandomSeed;
            public string LevelName;
            public BinaryTag NbtStruct;
            public int SpawnX, SpawnY, SpawnZ, MCVersion;
            public BinaryTag[] tmpNbtStruct;

            #endregion Fields
        }

        #endregion Nested Types
    }
}