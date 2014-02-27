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

	public abstract class Entity
	{
		#region Fields

		public Chunk CurrentChunk;
		public int EntityID;
		public sbyte Pitch;
		public double X;
		public double Y;
		public sbyte Yaw;
		public double Z;
		protected sbyte _LastPitch;
		protected double _LastX;
		protected double _LastY;
		protected sbyte _LastYaw;
		protected double _LastZ;

		#endregion Fields

		#region Constructors

		public Entity ()
		{
			CurrentChunk = null;
			EntityID = XMC.Random.Next ();
		}

		#endregion Constructors

		#region Methods

		public virtual void Despawn ()
		{
			if (CurrentChunk != null) {
				CurrentChunk.Entities.Remove (this);
				CurrentChunk = null;
			}
		}

		public override string ToString ()
		{
			return "[Entity " + EntityID + "]";
		}

		public virtual void Update ()
		{
			Chunk oldChunk = CurrentChunk;
			Chunk newChunk = XMC.Server.World.GetChunkAt ((int)X, (int)Z);
			if (oldChunk != newChunk) {
				if (oldChunk != null)
					oldChunk.Entities.Remove (this);
				newChunk.Entities.Add (this);
				CurrentChunk = newChunk;
			}

			double dx = X - _LastX, dy = Y - _LastY, dz = Z - _LastZ;
			bool rotchanged = (Yaw != _LastYaw || Pitch != _LastPitch);
			_LastX = X;
			_LastY = Y;
			_LastZ = Z;
			_LastYaw = Yaw;
			_LastPitch = Pitch;
			if (dx != 0 || dy != 0 || dz != 0 || rotchanged) {
				foreach (Player p in XMC.Server.PlayerList) {
					if (p != this && p.VisibleEntities.Contains (this))
						p.UpdateEntity (this, dx, dy, dz, rotchanged, false);
				}
			}
		}

		#endregion Methods
	}
}