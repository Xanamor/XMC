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

namespace OpenMC.Plugins
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using MyCraft;

    using OpenMC.Plugins;

    public class PluginServices
    {
        #region Fields

        private Types.AssemblyList AvailablePlugins = new Types.AssemblyList();
        private PluginHost pluginHost = new PluginHost();
        private string PluginsPath = "./Plugins/";

        #endregion Fields

        #region Constructors

        public PluginServices()
        {
            OpenMC.Log("Loaded PluginMgr");
            if (!Directory.Exists(PluginsPath))
            {
                OpenMC.Log("Creating plugins folder");
                Directory.CreateDirectory(PluginsPath);
            }
        }

        #endregion Constructors

        #region Properties

        public Types.AssemblyList Plugins
        {
            get { return AvailablePlugins; }
            set { AvailablePlugins = value; }
        }

        #endregion Properties

        #region Methods

        public void FindPlugins()
        {
            //empty the current list of plugins incase we are doing a plugin reload
            AvailablePlugins.Clear();

            foreach (string File in Directory.GetFiles(PluginsPath))
            {
                FileInfo file = new FileInfo(File);
                if (file.Extension.Equals(".dll"))
                    this.Load(File);
            }
        }

        public void Load(string AssemblyName)
        {
            Assembly pluginAssembly = Assembly.LoadFrom(AssemblyName);

            foreach (Type pluginType in pluginAssembly.GetTypes())
            {
                if (!pluginType.IsAbstract)
                {
                    Type typeInterface = pluginType.GetInterface("MyCraft.IPlugin", true);

                    if (typeInterface != null)
                    {
                        Types.PluginObject newPlugin = new Types.PluginObject();

                        newPlugin.AssemblyPath = AssemblyName;
                        newPlugin.Instance = (IPlugin)Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                        newPlugin.Instance.Host = pluginHost;
                        newPlugin.Instance.Initialize();
                        AvailablePlugins.Add(newPlugin);
                    }
                }
            }
        }

        public void Unload(string AssemblyName)
        {
        }

        public void UnloadAll()
        {
            OpenMC.ScriptLog("Unloading all plugins");
            foreach (IPlugin plugin in AvailablePlugins)
            {
                OpenMC.ScriptLog("Unloading " + plugin.Name);
                plugin.Dispose();
            }
        }

        #endregion Methods
    }
}