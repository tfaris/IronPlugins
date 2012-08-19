using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Plugin
{
    /// <summary>
    /// Represents a plugin created from a file.
    /// </summary>
    public class FilePlugin : PluginBase
    {
        string _scriptPath;

        /// <summary>
        /// Get the file path to the script.
        /// </summary>
        public string ScriptPath
        {
            get { return _scriptPath; }
        }

        /// <summary>
        /// Create an instance of FilePlugin initialized with the
        /// specified script.
        /// </summary>
        /// <param name="scriptFilePath"></param>
        public FilePlugin(string scriptFilePath) : base()
        {
            this._scriptPath = scriptFilePath;
        }

        /// <summary>
        /// Create an instance of FilePlugin initialized with the
        /// specified script and Guid.
        /// </summary>
        /// <param name="scriptFilePath"></param>
        /// <param name="pluginGuid"></param>
        public FilePlugin(string scriptFilePath,Guid pluginGuid) : base(pluginGuid)
        {
            this._scriptPath = scriptFilePath;
        }

        /// <summary>
        /// Create a ScriptSource from the script file.
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Scripting.Hosting.ScriptSource InitializeSource()
        {
            return Engine.ScriptEngine.CreateScriptSourceFromFile(ScriptPath);
        }

        /// <summary>
        /// Returns true if the specified plugin is a FilePlugin and points to the same file.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool Equals(PluginBase p)
        {
            if (p is FilePlugin)
                return ((FilePlugin)p).ScriptPath == ScriptPath;
            else
                return false;
        }
    }
}
