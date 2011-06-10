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

using System;
namespace XMC
{
	public abstract class Entity
	{
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
		
		public Entity ()
		{
			EntityID = Toolbox.RandNum.Next();
		}
		
		public string GetID ()
		{
			return EntityID.ToString();
		}
	}
}

