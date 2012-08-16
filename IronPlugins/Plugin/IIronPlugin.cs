using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Plugin
{    
    /// <summary>
    /// Defines an interface for executing and uniquelfy identifying IronLanguage plugins.
    /// </summary>
    interface IIronPlugin : System.Dynamic.IDynamicMetaObjectProvider
    {
        /// <summary>
        /// Get a globally-unique identifier for the plugin.
        /// </summary>
        Guid Guid { get; }
        /// <summary>
        /// Get or set the context value of the specified member.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        dynamic this[string key] { get; set; }
        /// <summary>
        /// Compute a hash of the compiled script code.
        /// </summary>
        /// <returns></returns>
        string ComputeHash();
        /// <summary>
        /// Add one or more context variables to the plugin context.
        /// </summary>
        /// <param name="vars"></param>
        void AddContextVariables(params KeyValuePair<string, object>[] vars);
        /// <summary>
        /// Register a type that can be accessed by, and potentially inherited in, the plugin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void RegisterType<T>();
        /// <summary>
        /// Execute the plugin, placing any existing variables onto the current context and
        /// returning values if available.
        /// </summary>
        /// <returns></returns>
        dynamic RunPlugin();
        /// <summary>
        /// Execute the plugin code with the defined scope, placing any existing variables onto the current context and
        /// returning values if available. Context variables override existing scope variables.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        dynamic RunPlugin(Microsoft.Scripting.Hosting.ScriptScope scope);
    }
}
