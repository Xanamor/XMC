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

	public static class Color
	{
		#region Fields

		public const string Announce = Yellow;
		public const string Black = Signal + "0";
		public const string Blue = Signal + "9";
		public const string CommandError = DarkRed;
		public const string CommandResult = Teal;
		public const string DarkBlue = Signal + "1";
		public const string DarkGray = Signal + "8";
		public const string DarkGreen = Signal + "2";
		public const string DarkRed = Signal + "4";
		public const string DarkTeal = Signal + "3";
		public const string DarkYellow = Signal + "6";
		public const string Gray = Signal + "7";
		public const string Green = Signal + "a";
		public const string Pink = Signal + "d";
		public const string PrivateMsg = Purple;
		public const string Purple = Signal + "5";
		public const string Red = Signal + "c";
		public const string Signal = "\x00A7";
		// section symbol §
		public const string Teal = Signal + "b";
		public const string White = Signal + "f";
		public const string Yellow = Signal + "e";

		#endregion Fields
	}
}