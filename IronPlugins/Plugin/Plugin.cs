using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Plugin
{
    /// <summary>
    /// Represents a plugin with a script source and context which can be accessed
    /// dynamically.
    /// </summary>
    public abstract class Plugin : System.Dynamic.DynamicObject, IIronPlugin
    {
        Microsoft.Scripting.Hosting.ScriptSource _source;
        Microsoft.Scripting.Hosting.CompiledCode _compiled;
        Dictionary<string, dynamic> _contextVariables;
        Guid _guid = Guid.NewGuid();

        /// <summary>
        /// Get a globally-unique identifier created when this
        /// Plugin was created.
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
        }

        /// <summary>
        /// Get or set the context value of the specified member.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public dynamic this[string key]
        {
            get
            {
                return _contextVariables[key];
            }
            set
            {
                _contextVariables[key] = value;
            }
        }

        /// <summary>
        /// Create a default instance of Plugin.
        /// </summary>
        public Plugin()
        {
            _contextVariables = new Dictionary<string, dynamic>();
        }
        
        /// <summary>
        /// Create and return the script source to be used for the plugin.
        /// </summary>
        /// <returns></returns>
        protected abstract Microsoft.Scripting.Hosting.ScriptSource InitializeSource();
        /// <summary>
        /// Compile the script source to be used for the plugin.
        /// </summary>
        /// <returns></returns>
        protected virtual Microsoft.Scripting.Hosting.CompiledCode CompileSource()
        {
            return _source.Compile();
        }

        /// <summary>
        /// Make sure our code is ready.
        /// </summary>
        private void CheckInitialized()
        {
            if (_source == null)
            {
                _source = InitializeSource();
                _compiled = null; // If source is null, we definitely need to compile (even if _compiled exists somehow)
            }
            if (_compiled == null)
                _compiled = CompileSource();
        }

        /// <summary>
        /// Add one or more context variables to the plugin context.
        /// </summary>
        /// <param name="vars"></param>
        public void AddContextVariables(params KeyValuePair<string, object>[] vars)
        {
            foreach (KeyValuePair<string, object> var in vars)
                _contextVariables[var.Key] = var.Value;
        }

        /// <summary>
        /// Execute the plugin, placing any existing variables onto the current context and
        /// returning values if available.
        /// </summary>
        /// <returns></returns>
        public dynamic RunPlugin()
        {
            Microsoft.Scripting.Hosting.ScriptScope scope = Engine.ScriptEngine.CreateScope();
            return RunPlugin(scope);
        }

        /// <summary>
        /// Execute the plugin code with the defined scope, placing any existing variables onto the current context and
        /// returning values if available. Context variables override existing scope variables.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public dynamic RunPlugin(Microsoft.Scripting.Hosting.ScriptScope scope)
        {
            CheckInitialized();
            foreach (KeyValuePair<string, dynamic> variable in _contextVariables)
            {
                // If we try to set a variable to null, we get an execption, but casting
                // null to System.Object solves that.
                scope.SetVariable(variable.Key, (Object)variable.Value);
            }
            dynamic retVal = _compiled.Execute(scope);
            foreach (string scopeVar in scope.GetVariableNames())
                _contextVariables[scopeVar] = scope.GetVariable(scopeVar);
            return retVal;
        }

        /// <summary>
        /// Compute a hash of the compiled script code.
        /// </summary>
        /// <returns></returns>
        public string ComputeHash()
        {
            return _compiled.GetHashCode().ToString();
        }

        #region Dynamics

        /// <summary>
        /// Dynamically invoke a context member.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryInvokeMember(System.Dynamic.InvokeMemberBinder binder, object[] args, out object result)
        {
            dynamic val;
            if (_contextVariables.TryGetValue(binder.Name, out val))
            {
                if (Engine.ScriptEngine.Operations.IsCallable(val))
                {
                    if (val is IronPython.Runtime.PythonFunction)
                    {
                        result = Engine.ScriptEngine.Operations.Invoke(val, args);
                    }
                    else if (val is IronPython.Runtime.Types.PythonType)
                    {
                        result = Engine.ScriptEngine.Operations.Invoke(val, args);
                    }
                    else
                    {
                        result = Engine.ScriptEngine.Operations.InvokeMember(val, binder.Name, args);
                    }
                }
                else
                {
                    result = val;
                }
            }
            return base.TryInvokeMember(binder, args, out result);
        }

        /// <summary>
        /// Dynamically get a context member.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (_contextVariables.TryGetValue(binder.Name, out result))
            {
                return true;
            }
            return base.TryGetMember(binder, out result);
        }

        /// <summary>
        /// Dynamically set a context member.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }

        #endregion
    }
}
