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
	using System.Security.Cryptography;
	using System.Text;
	using Logger;
	using Utils;

	public static class XMC
	{
		#region Fields

		//TODO: Relocate and have better error handling for Protocol Versions, maybe multiple clients idk
		public const int ProtocolVersion = 10;
		public static Random Random;
		public static Server Server;

		#endregion Fields

		#region Methods

		public static string Base36Encode (long input)
		{
			if (input == 0)
				return "0";
			string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
			bool negative = (input < 0);

			StringBuilder sb = new StringBuilder ();
			if (negative) {
				input = -input;
				sb.Append ("-");
			}
			while (input > 0) {
				sb.Insert ((negative ? 1 : 0), chars [(int)(input % 36)]);
				input /= 36;
			}
			return sb.ToString ();
		}

		public static string Base64Encode (string str)
		{
			return Convert.ToBase64String (System.Text.Encoding.UTF8.GetBytes (str));
		}

		public static string FormatTime ()
		{
			return DateTime.Now.ToString ("HH:mm:ss");
		}

		public static void Log (string message)
		{
			LogManager.LogNotice (FormatTime () + "  >   " + message + "\n");
		}

		public static void LogError (Exception e)
		{
			LogManager.LogError (e);
		}

		public static void LogWarrning (string message)
		{
			LogManager.LogWarrning (FormatTime () + ">   " + message + "\n");
		}

		public static void Main (string[] args)
		{
			Log ("XMC is starting...");
			//TODO: SQLite
			Configuration.Load ();

			if (Configuration.Defined ("random-seed")) {
				Random = new Random (Configuration.GetInt ("random-seed", 0));
			} else {
				Random = new Random ();
			}

			Server = new Server ();
			try {
				Server.Run ();
			} catch (Exception e) {
				Log ("Fatal uncaught exception: " + e);
			}

			Log ("Bye!");
		}

		public static string MD5sum (string str)
		{
			MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider ();
			byte[] data = Encoding.ASCII.GetBytes (str);
			data = crypto.ComputeHash (data);
			StringBuilder ret = new StringBuilder ();
			for (int i = 0; i < data.Length; i++)
				ret.Append (data [i].ToString ("x2").ToLower ());
			return ret.ToString ();
		}

		#endregion Methods
	}
}