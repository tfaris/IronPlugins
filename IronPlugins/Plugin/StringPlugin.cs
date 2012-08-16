using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Plugin
{
    /// <summary>
    /// Represents a plugin script created from an in-memory string.
    /// </summary>
    public class StringPlugin : Plugin
    {        
        string _code;

        /// <summary>
        /// Create an instance of StringPlugin from the specified code.
        /// </summary>
        /// <param name="code"></param>
        public StringPlugin(string code) : base()
        {
            this._code = code;
        }

        /// <summary>
        /// Initialize the source code from the in-memory string.
        /// </summary>
        /// <returns></returns>
        protected override Microsoft.Scripting.Hosting.ScriptSource InitializeSource()
        {
            return Engine.ScriptEngine.CreateScriptSourceFromString(this._code);
        }
    }
}
