﻿#region Header

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

namespace OpenMC.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using MyCraft;

    class PluginHost : IPluginHost
    {
        #region Methods

        public void SayToServer(string message)
        {
            OpenMC.Server.MessageAll(message);
        }

        public void ScriptLog(string message)
        {
            OpenMC.ScriptLog(message);
        }

        #endregion Methods
    }
}