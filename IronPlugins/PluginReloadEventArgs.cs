using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins
{
    /// <summary>
    /// Provides data for a plugin reload event.
    /// </summary>
    public class PluginReloadEventArgs : EventArgs
    {
        Plugin.IIronPlugin _plugin;

        /// <summary>
        /// Get the IIronPlugin that was reloaded.
        /// </summary>
        public Plugin.IIronPlugin Plugin
        {
            get { return _plugin; }
        }

        /// <summary>
        /// Create an instance of PluginReloadEventArgs with the specified
        /// reloaded plugin.
        /// </summary>
        /// <param name="pluginReloaded"></param>
        public PluginReloadEventArgs(Plugin.IIronPlugin pluginReloaded)
        {
            _plugin = pluginReloaded;            
        }
    }
}
