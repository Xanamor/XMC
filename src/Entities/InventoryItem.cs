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

    public struct InventoryItem
    {
        #region Fields

        public byte Count;
        public short Damage;
        public short Type;

        #endregion Fields

        #region Constructors

        public InventoryItem(short type, byte count, short damage)
        {
            Type = type;
            Count = count;
            Damage = damage;
        }

        public InventoryItem(short type, short damage)
        {
            Type = type;
            Count = 1;
            Damage = damage;
        }

        public InventoryItem(short type)
        {
            Type = type;
            Count = 1;
            Damage = 0;
        }

        #endregion Constructors

        #region Methods

        public override string ToString()
        {
            string name = "[InvItem ";
            if (Type == -1) {
                name += "Nil";
            } else if (Type < 256) {
                name += "Block." + ((Block)Type).ToString();
            } else {
                name += "Item." + ((Item)Type).ToString();
            }
            if (Type > 0) {
                if (Count != 1) {
                    name += " x" + Count;
                }
                if (Damage != 0) {
                    name += " d" + Damage;
                }
            }
            name += "]";
            return name;
        }

        #endregion Methods
    }
}