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
        string _hash;

        /// <summary>
        /// Get a globally-unique identifier created when this
        /// Plugin was created.
        /// </summary>
        public Guid Guid
        {
            get { return _guid; }
        }

        /// <summary>
        /// Get the ScriptSource of this plugin.
        /// </summary>
        protected Microsoft.Scripting.Hosting.ScriptSource Source
        {
            get { return _source; }
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
                _hash = null;
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
        /// Register a type that can be accessed by, and potentially inherited in, the plugin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterType<T>()
        {
            Type t = typeof(T);
            AddContextVariables(new KeyValuePair<string, object>(t.Name, Engine.GetScriptType<T>()));
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
            dynamic retVal = null;
            try
            {
                CheckInitialized();
                foreach (KeyValuePair<string, dynamic> variable in _contextVariables)
                {
                    // If we try to set a variable to null, we get an execption, but casting
                    // null to System.Object solves that.
                    scope.SetVariable(variable.Key, (Object)variable.Value);
                }
                retVal = _compiled.Execute(scope);
                foreach (string scopeVar in scope.GetVariableNames())
                    _contextVariables[scopeVar] = scope.GetVariable(scopeVar);
            }
            catch (Exception ex)
            {
                if (Engine.IsIPYEngine(Engine.ScriptEngine))
                {
                    throw new Plugins.PythonPluginException(ex);
                }
                else throw ex;                     
            }
            return retVal;
        }

        /// <summary>
        /// Compute a hash of the script code.
        /// </summary>
        /// <returns></returns>
        public virtual string ComputeHash()
        {
            if (_hash == null)
            {
                if (Source != null)
                {
                    string source = Source.GetCode();
                    Encoding sourceEncoding = Source.DetectEncoding() ?? Encoding.UTF8;
                    byte[] bytes = sourceEncoding.GetBytes(source);
                    using (System.Security.Cryptography.HashAlgorithm provider = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                    {
                        _hash = BitConverter.ToString(provider.ComputeHash(bytes)).Replace("-", string.Empty).ToLower();
                    }
                }
                else
                    _hash = string.Empty;
            }
            return _hash;
        }

        public dynamic Invoke(string name, params object[] args)
        {
            dynamic result = null;
            dynamic val;
            if (_contextVariables.TryGetValue(name, out val))
            {
                if (Engine.ScriptEngine.Operations.IsCallable(val))
                {
                    if (val is IronPython.Runtime.PythonFunction)
                    {
                        result = Engine.ScriptEngine.Operations.Invoke(val, args);
                    }
                    else if (val is IronPython.Runtime.Types.PythonType || val is IronPython.Runtime.Types.OldClass)
                    {
                        result = Engine.ScriptEngine.Operations.Invoke(val, args);
                    }
                    else
                    {
                        result = Engine.ScriptEngine.Operations.InvokeMember(val, name, args);
                    }
                }
                else
                {
                    result = val;
                }
            }
            if (result == null)
            {
                throw new System.MissingMemberException(string.Format("No member named \"{0}\" found in plugin context {1}.",name,this.Guid));
            }
            return result;
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
            result = Invoke(binder.Name, args);
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
