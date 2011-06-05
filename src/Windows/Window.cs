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
using System;
using System.Collections.Generic;

namespace OpenMC
{
	public abstract class Window
	{
		private static byte _NextID = 2;
		protected List<Player> _Viewers;
		
		public byte ID;
		public byte Type;
		public string Title;
		
		public InventoryItem[] slots { get; protected set; }
		
		protected Window(byte type, string title) {
			_Viewers = new List<Player>();
			ID = _NextID;
			if (++_NextID > 125) _NextID = 2;
			
			Type = type;
			Title = title;
		}
		
		virtual public void Open(Player player) {
			_Viewers.Add(player);
			player.OpenWindow(this);
		}
		
		virtual public void Close(Player player) {
			_Viewers.Remove(player);
			if (_Viewers.Count == 0) {
				OpenMC.Server.WindowList.Remove(this);
			}
		}
		
		public abstract bool Click(Player p, short slot, byte type, InventoryItem item);
		
		public void SetSlot(short slot, InventoryItem item) {
			slots[slot] = item;
			foreach (Player player in _Viewers) {
				player.WindowSetSlot(this, slot, item);
			}
		}
	}
}
