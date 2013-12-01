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

    public static class Configuration
    {
        #region Fields

        private const string _CONFIG_FILENAME = "configuration.cfg";

        private static Dictionary<string, string> _Config;

        #endregion Fields

        #region Methods

        public static bool Defined(string key)
        {
            return _Config.ContainsKey(key);
        }

        public static string Get(string key, string def)
        {
            if (Defined(key))
                return (_Config[key]);
            else
                XMC.LogWarrning("Could not locate value in config file assuming default value for " + key);
                return def;
        }

        public static bool GetBool(string key, bool def)
        {
            string val = Get(key, def.ToString());
            return StrIsTrue(val);
        }

        public static int GetInt(string key, int def)
        {
            string val = Get(key, null);
            if (val == null)
                return def;
            else
                return Convert.ToInt32(val);
        }

        public static void Load()
        {
            _Config = new Dictionary<string, string>();

            if (!File.Exists(_CONFIG_FILENAME)) {
                XMC.LogWarrning("Generating " + _CONFIG_FILENAME);
                WriteDefaultConfig();
            }

            StreamReader input = new StreamReader(_CONFIG_FILENAME);
            string raw = input.ReadToEnd();
            raw = raw.Replace("\r\n", "\n"); // Just in case we have to deal with silly Windows/UNIX line-endings.
            string[] lines = raw.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            input.Close();

            foreach (var line in lines) {
                if (line[0] == '#')
                    continue;
                int pos = line.IndexOf("=");
                string key = line.Substring(0, pos).Trim();
                string val = line.Substring(pos + 1).Trim();
                _Config[key] = val;
                XMC.Log("Configing: " + key + "=" + val);
            }
        }

        public static string OnOffStr(bool val)
        {
            return (val ? "On" : "Off");
        }

        // helpers
        public static bool StrIsTrue(string val)
        {
            val = val.ToLower().Trim();
            return (val == "1" || val == "yes" || val == "true" || val == "on");
        }

        private static void WriteDefaultConfig()
        {
            StreamWriter fh = new StreamWriter(_CONFIG_FILENAME);
            fh.WriteLine("# XMC default configuration file");
            fh.WriteLine("# This file was auto-generated");
            fh.WriteLine();
                    fh.WriteLine("world = world");
                    fh.WriteLine("listenAddr = 0.0.0.0");
            fh.WriteLine("port = 25565");
            fh.WriteLine("server-name = XMC Server");
            fh.WriteLine("motd = Powered by " + Color.Blue + "XMC " + Constants.SSettings.InternalVersion);
            fh.WriteLine("max-players = 8");
            fh.WriteLine("verify-names = true");
                      /*fh.WriteLine("# MySql Configuration");
                    fh.WriteLine("DBhost = 127.0.0.1");
                    fh.WriteLine("Database = XMC");
                    fh.WriteLine("User = root");
                    fh.WriteLine("Password = none"); */
            fh.Close();
        }

        #endregion Methods
    }
}