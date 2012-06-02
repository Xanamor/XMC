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

namespace OpenMC.Logger
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Constants;

    public static class LogManager
    {
        #region Fields

        static LogWorker _logger = new LogWorker();

        #endregion Fields

        #region Methods

        public static void Dispose()
        {
            _logger.Dispose();
        }

        public static void LogError(Exception ex)
        {
            //if (SSettings.LoggingEnabled)
                _logger.LogError(ex);
        }

        public static void LogNotice(string str)
        {
            //if(SSettings.LoggingEnabled)
                _logger.LogMessage(str);
        }

        public static void LogScriptMessage(string str)
        {
            //if (SSettings.LoggingEnabled)
            _logger.LogScriptMessage(str);
        }

        public static void LogWarrning(string str)
        {
            //if (SSettings.LoggingEnabled)
                _logger.LogWarrning(str);
        }

        #endregion Methods
    }
}