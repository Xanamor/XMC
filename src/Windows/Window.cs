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

    public abstract class Window
    {
        #region Fields

        public byte ID;
        public string Title;
        public byte Type;

        protected List<Player> _Viewers;

        private static byte _NextID = 2;

        #endregion Fields

        #region Constructors

        protected Window(byte type, string title)
        {
            _Viewers = new List<Player>();
            ID = _NextID;
            if (++_NextID > 125) _NextID = 2;

            Type = type;
            Title = title;
        }

        #endregion Constructors

        #region Properties

        public InventoryItem[] slots
        {
            get; protected set;
        }

        #endregion Properties

        #region Methods

        public abstract bool Click(Player p, short slot, byte type, InventoryItem item);

        public virtual void Close(Player player)
        {
            _Viewers.Remove(player);
            if (_Viewers.Count == 0) {
                XMC.Server.WindowList.Remove(this);
            }
        }

        public virtual void Open(Player player)
        {
            _Viewers.Add(player);
            player.OpenWindow(this);
        }

        public void SetSlot(short slot, InventoryItem item)
        {
            slots[slot] = item;
            foreach (Player player in _Viewers) {
                player.WindowSetSlot(this, slot, item);
            }
        }

        #endregion Methods
    }
}