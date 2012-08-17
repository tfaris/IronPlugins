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

        /// <summary>
        /// Add a library search path to the engine.
        /// </summary>
        /// <param name="path"></param>
        public static void AddSearchPath(string path)
        {
            ICollection<string> paths = ScriptEngine.GetSearchPaths();
            paths.Add(path);
            ScriptEngine.SetSearchPaths(paths);
        }

        /// <summary>
        /// Add a range of search paths to the engine.
        /// </summary>
        /// <param name="paths"></param>
        public static void AddSearchPaths(IEnumerable<string> paths)
        {
            ICollection<string> exPaths = ScriptEngine.GetSearchPaths();
            foreach (string p in paths)
                exPaths.Add(p);
            ScriptEngine.SetSearchPaths(exPaths);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        public static bool IsIPYEngine(Microsoft.Scripting.Hosting.ScriptEngine engine)
        {
            var ipyNames =
                from name in engine.Setup.Names
                where name.Contains("Python")
                select name;
            return ipyNames.Count() > 0;
        }
    }
}
