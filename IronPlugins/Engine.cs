using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Scripting.Hosting;

namespace IronPlugins
{
    /// <summary>
    /// Provides access to (for now) the IronPython script engine.
    /// </summary>
    public static class Engine
    {
        private static ScriptEngine _engine;
        /// <summary>
        /// Get the ScriptEngine in use for plugins.
        /// </summary>
        public static ScriptEngine ScriptEngine
        {
            get
            {
                if (_engine == null)
                    _engine = IronPython.Hosting.Python.CreateEngine();                
                return _engine;                
            }
        }
    }
}
