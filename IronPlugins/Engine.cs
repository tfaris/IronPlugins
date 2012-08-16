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

        /// <summary>
        /// Returns a type that can be used in scripts for the specified generic type
        /// (currently only PythonType).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object GetScriptType<T>()
        {
            return IronPython.Runtime.Types.DynamicHelpers.GetPythonTypeFromType(typeof(T));
        }
    }
}
