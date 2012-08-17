using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Plugin
{
    /// <summary>
    /// Represents a plugin created from a file.
    /// </summary>
    public class FilePlugin : Plugin
    {
        string _scriptPath;

        /// <summary>
        /// Get the file path to the script.
        /// </summary>
        public string ScriptPath
        {
            get { return _scriptPath; }
        }

        public FilePlugin(string scriptFilePath)
        {
            this._scriptPath = scriptFilePath;
        }

        protected override Microsoft.Scripting.Hosting.ScriptSource InitializeSource()
        {
            return Engine.ScriptEngine.CreateScriptSourceFromFile(ScriptPath);
        }
    }
}
