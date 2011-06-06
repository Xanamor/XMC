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

namespace OpenMC.Plugins.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using MyCraft;

    public class AssemblyList : System.Collections.CollectionBase
    {
        #region Methods

        //A Simple Home-brew class to hold some info about our Available Plugins
        /// <summary>
        /// Add a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToAdd">The Plugin to Add</param>
        public void Add(PluginObject pluginToAdd)
        {
            this.List.Add(pluginToAdd);
        }

        /// <summary>
        /// Finds a plugin in the available Plugins
        /// </summary>
        /// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
        /// <returns>Available Plugin, or null if the plugin is not found</returns>
        public PluginObject Find(string pluginNameOrPath)
        {
            PluginObject toReturn = null;

            //Loop through all the plugins
            foreach (PluginObject pluginOn in this.List)
            {
                //Find the one with the matching name or filename
                if ((pluginOn.Instance.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
                {
                    toReturn = pluginOn;
                    break;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Remove a Plugin to the collection of Available plugins
        /// </summary>
        /// <param name="pluginToRemove">The Plugin to Remove</param>
        public void Remove(PluginObject pluginToRemove)
        {
            this.List.Remove(pluginToRemove);
        }

        #endregion Methods
    }

    /// <summary>
    /// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
    /// </summary>
    public class PluginObject
    {
        #region Fields

        private string myAssemblyPath = "";

        //This is the actual AvailablePlugin object..
        //Holds an instance of the plugin to access
        //ALso holds assembly path... not really necessary
        private IPlugin myInstance = null;

        #endregion Fields

        #region Properties

        public string AssemblyPath
        {
            get { return myAssemblyPath; }
            set { myAssemblyPath = value; }
        }

        public IPlugin Instance
        {
            get { return myInstance; }
            set { myInstance = value; }
        }

        #endregion Properties
    }
}