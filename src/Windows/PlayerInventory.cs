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

namespace OpenMC
{
	public class PlayerInventory : Window
	{
		private Player _Player;
		
		public PlayerInventory(Player player) : base(0, "Inventory") {
			_Player = player;
			ID = 0;
			slots = new InventoryItem[45];
			for (int i = 0; i < 45; ++i) {
				slots[i].Type = -1;
			}
			_Viewers.Add(_Player);
		}
		
		override public void Open(Player player) { /* nil */ }
		
		override public void Close(Player player) {
			// TODO: Jettison items in crafting slots.
		}
		
		override public bool Click(Player p, short slot, byte type, InventoryItem item) {
			InventoryItem holding = p.WindowHolding;
			if (slot < 0) {
				// outside window
				return false;
			} else if (slot == 0) {
				// crafting slot
				return true;
			} else {
				if (!item.Equals(slots[slot])) return false;
				p.SetHolding(slots[slot]);
				SetSlot(slot, holding);
				return true;
			}
		}
		
		private bool TryAddItem(short i, InventoryItem item) {
			if (slots[i].Type == -1) {
				SetSlot(i, item);
				return true;
			} else if (slots[i].Type == item.Type) {
				if (slots[i].Count + item.Count <= 64) {
					slots[i].Count += item.Count;
					SetSlot(i, slots[i]);
					return true;
				}
			}
			return false;
		}
		
		public bool AddItem(InventoryItem item) {
			// TODO: Better logic.
			for (short i = 36; i < 45; ++i) {
				if (TryAddItem(i, item)) return true;
			}
			for (short i = 9; i < 36; ++i) {
				if (TryAddItem(i, item)) return true;
			}
			return false;
		}
	}
}
