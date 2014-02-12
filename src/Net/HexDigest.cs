//  Copyright (c) 2014 xinnx
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Security.Cryptography;
using System.Text;

namespace Net
{
	class HexDigest
	{
		public string DoHexDigest(string hash)
		{
			// The test values provided on http://wiki.vg/Protocol_Encryption are:
			// sha1(Notch) :  4ed1f46bbe04bc756bcb17c0c7ce3e4632f06a48
			// sha1(jeb_)  : -7c9d5b0044c130109a5d7b5fb5c317c02b4e28c1
			// sha1(simon) :  88e16a1019277b15d58faf0541e11910eb756f6
			return JavaHexDigest (hash);
		}

		private string JavaHexDigest(string data)
		{
			var sha1 = SHA1.Create();
			byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
			bool negative = (hash[0] & 0x80) == 0x80;
			if (negative) // check for negative hashes
				hash = TwosCompliment(hash);
			// Create the string and trim away the zeroes
			string digest = GetHexString(hash).TrimStart('0');
			if (negative)
				digest = "-" + digest;
			return digest;
		}

		private string GetHexString(byte[] p)
		{
			string result = string.Empty;
			for (int i = 0; i < p.Length; i++)
				result += p[i].ToString("x2"); // Converts to hex string
			return result;
		}
		private byte[] TwosCompliment(byte[] p) // little endian
		{
			int i;
			bool carry = true;
			for (i = p.Length - 1; i >= 0; i--)
			{
				p[i] = (byte)~p[i];
				if (carry)
				{
					carry = p[i] == 0xFF;
					p[i]++;
				}
			}
			return p;
		}
	}
}

