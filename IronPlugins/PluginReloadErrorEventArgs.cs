using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins
{
    /// <summary>
    /// Provides data for a plugin reload error event.
    /// </summary>
    public class PluginReloadErrorEventArgs : PluginReloadEventArgs
    {
        Exception _error;

        /// <summary>
        /// Get the error that was caused during reload.
        /// </summary>
        public Exception Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Create an instance of PluginReloadErrorEventArgs specifying the plugin that caused
        /// the error while reloading, and the error itself.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="error"></param>
        public PluginReloadErrorEventArgs(Plugin.IIronPlugin plugin, Exception error)
            : base(plugin)
        {
            this._error = error;
        }
    }
}
