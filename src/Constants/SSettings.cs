#region Header

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

#endregion Header

namespace OpenMC.Constants
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    static class SSettings
    {
        #region Fields

        public static bool checkUpdates = true;
        public static string InternalVersion = "v 0.0.8a";
        public static string ListenAddr = Configuration.Get("listenAddr","0.0.0.0");
        public static int ListenPort = Configuration.GetInt("port", 25565);
        public static bool LoggingEnabled = true;
        public static int MaxPlayers = Configuration.GetInt("max-players", 8);
        public static string MOTD = Configuration.Get("motd", "Powered by " + Color.Blue + "OpenMC");
        public static string MySQLDatabaseName = Configuration.Get("Database","openmc");
        public static string MySQLHost = Configuration.Get("DBhost","127.0.0.1");
        public static string MySQLPassword = Configuration.Get("Password","");
        public static string MySQLUsername = Configuration.Get("User","root");
        public static string PlayerSavePath = "./players/";
        public static bool pub = true;
        public static string ServerName = Configuration.Get("server-name", "Minecraft Server");
        public static bool verify = true;
        public static string WorldName = Configuration.Get("world", "world");

        #endregion Fields
    }
}