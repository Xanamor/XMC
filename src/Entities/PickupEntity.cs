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

    public class PickupEntity : Entity
    {
        #region Fields

        public InventoryItem Item;

        #endregion Fields

        #region Constructors

        public PickupEntity(int x, int y, int z, InventoryItem item)
        {
            X = x + .5; Y = y + .5; Z = z + .5;
            Item = item;
            base.Update();
        }

        #endregion Constructors

        #region Methods

        public override void Despawn()
        {
            base.Despawn();
        }

        public override string ToString()
        {
            return "[Entity.Pickup " + EntityID + ": " + Item + "]";
        }

        public override void Update()
        {
            Pair<int, int> pos = CurrentChunk.GetChunkPos(X, Z);
            if (CurrentChunk.GetBlock(pos.First, (int)(Y - 0.25), pos.Second) == Block.Air) {
                Y -= 0.2;
            }
            base.Update();
            foreach (Player p in OpenMC.Server.PlayerList) {
                if (Math.Abs(p.X - X) < 1 && Math.Abs(p.Z - Z) < 1 && Y <= p.Y + 2 && Y >= p.Y) {
                    if (p.Inventory.AddItem(Item)) {
                        foreach (Player p2 in OpenMC.Server.PlayerList) {
                            if (p2.VisibleEntities.Contains(this)) p2.PickupCollected(this, p);
                        }
                        Despawn();
                        break;
                    }
                }
            }
        }

        #endregion Methods
    }
}