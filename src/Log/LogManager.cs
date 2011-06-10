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

namespace XMC.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class LogManager
    {
        static LogWorker _logger = new LogWorker();
		
		
        public static void Dispose() { _logger.Dispose(); }
        public static void LogError(Exception ex) { _logger.LogError(ex); }
        public static void LogNotice(string str) { _logger.LogMessage(str); }
        public static void LogWarrning(string str) { _logger.LogWarrning(str); }
    }
}