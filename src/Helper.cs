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
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace XMC
{
	/// <summary>
	/// This class is ment to act as a general "tool box" of methods.
	/// </summary>
	public static class Toolbox
	{
		public static Random RandNum;
		
		public static string Base36Encode (long Input)
		{
			if (input == 0) { return "0"; }
			
            string chars = "0123456789abcdefghijklmnopqrstuvwxyz";
            bool negative = (input < 0);

            StringBuilder sb = new StringBuilder();
            if (negative) {
                input = -input;
                sb.Append("-");
            }
            while (input > 0) {
                sb.Insert((negative ? 1 : 0), chars[(int)(input % 36)]);
                input /= 36;
            }
            return sb.ToString();
		}
		
		
		public static string Base36Encode (string Input)
		{
		}
		
		
		public static string FormatTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
		
		
        public static void Log(string message)
        {
            //LogManager.LogNotice(FormatTime() + "  >   " + message + "\n");
        }
		
		
		public static void LogWarrning(string message)
        {
            //LogManager.LogWarrning(FormatTime() + ">   " + message + "\n");
        }
		
		
        public static void LogError(Exception e)
        {
            //LogManager.LogError(e);
        }
		
		
		public static string MD5sum(string str)
        {
            MD5CryptoServiceProvider crypto = new MD5CryptoServiceProvider();
            byte[] data = Encoding.ASCII.GetBytes(str);
            data = crypto.ComputeHash(data);
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                ret.Append(data[i].ToString("x2").ToLower());
            return ret.ToString();
        }
        
	}
}

