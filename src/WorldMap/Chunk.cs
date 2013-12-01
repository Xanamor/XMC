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
    using System.Text;

    using NBT;

    public class Chunk
    {
        #region Fields

        public int ChunkX;
        public int ChunkZ;
        public List<Entity> Entities;

        private BinaryTag _Structure;
        private Map _World;

        #endregion Fields

        #region Constructors

        public Chunk(int chunkX, int chunkZ, Map world)
        {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
            Entities = new List<Entity>();
            _World = world;
            Load();
        }

        #endregion Constructors

        #region Methods

        public void Generate()
        {
            byte[] blocks = new byte[32768];
            byte[] data = new byte[16384];
            byte[] skylight = new byte[16384];
            byte[] light = new byte[16384];
            byte[] height = new byte[256];
            BinaryTag[] entities = new BinaryTag[0], tileEntities = new BinaryTag[0];

            for (int i = 0; i < 16348; i++) {
                blocks[i] = (byte)Block.Stone;
                blocks[i + 16384] = (byte)Block.Dirt;
                skylight[i] = 0xFF;
                light[i] = 0xFF;
            }

            skylight[16348] = 0xFF;
            light[16348] = 0xFF;
            BinaryTag[] structure = new BinaryTag[] {
                new BinaryTag (TagType.ByteArray, blocks, "Blocks"),
                new BinaryTag (TagType.ByteArray, data, "Data"),
                new BinaryTag (TagType.ByteArray, skylight, "SkyLight"),
                new BinaryTag (TagType.ByteArray, light, "BlockLight"),
                new BinaryTag (TagType.ByteArray, height, "HeightMap"),
                new BinaryTag (TagType.List, entities, "Entities"),
                new BinaryTag (TagType.List, tileEntities, "TileEntities"),
                new BinaryTag (TagType.Long, (long)0, "LastUpdate"),
                new BinaryTag (TagType.Int, (int)ChunkX, "xPos"),
                new BinaryTag (TagType.Int, (int)ChunkZ, "zPos"),
            new BinaryTag (TagType.Byte, (byte)0, "TerrainPopulated")
            };

            _Structure = new BinaryTag (TagType.Compound, new BinaryTag[] {
                new BinaryTag (TagType.Compound, structure, "Level")
            });
            Save ();

            //Set everything higher then the 64th level to air
            for (int y = 64; y < 128; y++)
                for (int x = 0; x < 16; x++)
                    for (int z = 0; z < 16; z++)
                        SetBlock (x, y, z, Block.Air);
            Save ();
        }

        // ====================
        // Tile gets/sets
        public Block GetBlock(int x, int y, int z)
        {
            return (Block) ((byte[])(_Structure["Level"]["Blocks"].Payload))[BlockIndex(x, y, z)];
        }

        public byte[] GetBytes()
        {
            Builder<Byte> builder = new Builder<Byte>();
            builder.Append((byte[]) _Structure["Level"]["Blocks"].Payload);
            builder.Append((byte[]) _Structure["Level"]["Data"].Payload);
            builder.Append((byte[]) _Structure["Level"]["BlockLight"].Payload);
            builder.Append((byte[]) _Structure["Level"]["SkyLight"].Payload);
            return builder.ToArray();
        }

        public Pair<int, int> GetChunkPos(int x, int z)
        {
            return new Pair<int, int>((x - 16*ChunkX) % 16, (z - 16*ChunkZ) % 16);
        }

        public Pair<int, int> GetChunkPos(double x, double z)
        {
            int xNeg = (x < 0 ? 1 : 0), zNeg = (z < 0 ? 1 : 0);
            return new Pair<int, int>(((int)x + xNeg - 16*ChunkX) % 16, ((int)z + zNeg - 16*ChunkZ) % 16);
        }

        public byte GetData(int x, int y, int z)
        {
            return 0;
        }

        public byte GetLight(int x, int y, int z)
        {
            return 0;
        }

        public byte GetSkyLight(int x, int y, int z)
        {
            return 0;
        }

        public void Load()
        {
            try {
                StreamReader rawReader = new StreamReader (CalculateFilename ());
                GZipStream reader = new GZipStream (rawReader.BaseStream, CompressionMode.Decompress);
                _Structure = NbtParser.ParseTagStream(reader);
                reader.Close();
                rawReader.Close();
            }
            catch (FileNotFoundException) {
                Generate();
            }
            catch (DirectoryNotFoundException) {
                Generate();
            }
        }

        public void Save()
        {
            string filename = CalculateFilename();
            int i = filename.LastIndexOfAny(new char[] { '/', '\\', ':' });
            Directory.CreateDirectory(filename.Substring(0, i));

            StreamWriter rawWriter = new StreamWriter(filename);
            GZipStream writer = new GZipStream(rawWriter.BaseStream, CompressionMode.Compress);
            NbtWriter.WriteTagStream(_Structure, writer);
            writer.Close();
        }

        public void SetBlock(int x, int y, int z, Block block)
        {
            ((byte[])(_Structure["Level"]["Blocks"].Payload))[BlockIndex(x, y, z)] = (byte)block;
        }

        public void SetData(int x, int y, int z, byte data)
        {
            // TODO
        }

        public void SetLight(int x, int y, int z, byte data)
        {
            // TODO
        }

        public void SetSkyLight(int x, int y, int z, byte data)
        {
            // TODO
        }

        public override string ToString()
        {
            return "[Chunk at " + ChunkX + ", " + ChunkZ + "]";
        }

        // ====================
        // Helper functions
        private int BlockIndex(int x, int y, int z)
        {
            return y + (z * 128 + (x * 128 * 16));
        }

        private string CalculateFilename()
        {
            int modX = (ChunkX >= 0 ? ChunkX % 64 : 64 - Math.Abs(ChunkX) % 64);
            int modZ = (ChunkZ >= 0 ? ChunkZ % 64 : 64 - Math.Abs(ChunkZ) % 64);
            StringBuilder sb = new StringBuilder();
            return (sb.Append(_World.WorldName).Append("/")
                      .Append(XMC.Base36Encode(modX)).Append("/")
                      .Append(XMC.Base36Encode(modZ)).Append("/")
                      .Append("c.").Append(XMC.Base36Encode(ChunkX))
                      .Append(".").Append(XMC.Base36Encode(ChunkZ))
                      .Append(".dat").ToString());
        }

        #endregion Methods
    }
}