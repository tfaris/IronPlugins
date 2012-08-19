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
    public abstract class PluginBase : System.Dynamic.DynamicObject, IIronPlugin
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
            get
            {
                dynamic plGuid;
                if (_contextVariables.TryGetValue("Guid", out plGuid))
                {
                    if (plGuid is string || plGuid is byte[])
                        return new Guid(plGuid);
                    else if (plGuid is Guid)
                        return plGuid;
                    else
                        throw new InvalidOperationException("The Guid attribute must return a string, byte array or System.Guid instance.");
                }
                else
                    return _guid;
            }
        }

        /// <summary>
        /// Get the ScriptSource of this plugin.
        /// </summary>
        public Microsoft.Scripting.Hosting.ScriptSource Source
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
        public PluginBase()
        {
            _contextVariables = new Dictionary<string, dynamic>();
        }

        /// <summary>
        /// Create an instance of Plugin with the specified Guid.
        /// </summary>
        /// <param name="pluginGuid"></param>
        public PluginBase(Guid pluginGuid) : this()
        {
            _guid = pluginGuid;
        }

        /// <summary>
        /// Create and return the script source to be used for the plugin.
        /// </summary>
        /// <returns></returns>
        protected abstract Microsoft.Scripting.Hosting.ScriptSource InitializeSource();

        /// <summary>
        /// Reload the plugin.
        /// </summary>
        public virtual void ReloadPlugin()
        {
            _source = null;
            _contextVariables.Clear();
        }

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
        public void AddContextVariables(params Context.Variable[] vars)
        {
            foreach (Context.Variable var in vars)
                _contextVariables[var.Name] = var.Value;
        }

        /// <summary>
        /// Remove one or more context variables from the plugin context.
        /// </summary>
        /// <param name="vars"></param>
        public void RemoveContextVariables(params Context.Variable[] vars)
        {
            foreach (Context.Variable var in vars)
                _contextVariables.Remove(var.Name);
        }

        /// <summary>
        /// Get all of the variables on the current plugin context.
        /// </summary>
        /// <returns></returns>
        public Context.VariableList GetContextVariables()
        {
            Context.VariableList list = new Context.VariableList();
            foreach (KeyValuePair<string, dynamic> var in _contextVariables)
            {
                list.Add(new Context.Variable(var.Key, var.Value));
            }
            return list;
        }

        /// <summary>
        /// Register a type that can be accessed by, and potentially inherited in, the plugin.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterType<T>()
        {
            Type t = typeof(T);
            AddContextVariables(new Context.Variable(t.Name, Engine.GetScriptType<T>()));
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
                    throw new Plugin.PythonPluginException(ex);
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

        /// <summary>
        /// Invoke a member of the plugin context with the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <exception cref="System.MissingMemberException">Thrown if the member cannot be found.</exception>
        /// <returns></returns>
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

        /// <summary>
        /// Cleanup the plugin. This will be called when
        /// the plugin will no longer be used.
        /// </summary>
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Return true if the Guid of the specified Plugin is equal
        /// to the current instance's Guid.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public virtual bool Equals(PluginBase p)
        {
            if (p != null)
                return p.Guid.Equals(Guid);
            return false;
        }

        /// <summary>
        /// Return true if the specified object is equal to 
        /// the current instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is PluginBase)
            {
                return Equals((PluginBase)obj);
            }
            return base.Equals(obj);
        }

        /// <summary>
        /// Return the hashcode.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ComputeHash().GetHashCode();
        }

        public override string ToString()
        {
            // If there is a ToString attribute on the plugin context,
            // use that. Otherwise use the source path.
            dynamic toString;
            if (_contextVariables.TryGetValue("ToString", out toString))
            {
                return Invoke("ToString").ToString();
            }
            else
            {
                return Source.Path.ToString();
            }
        }
    }
}
