using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IronPlugins.Plugin;

namespace IronPlugins
{
    /// <summary>
    /// Represents a collection of plugins coupled with various management-related tasks.
    /// </summary>
    public class PluginManager : IDisposable
    {
        PluginCollection _plugins;
        Dictionary<string, FileSystemWatcher> _watchers;
        Dictionary<string, DateTime> _lastRead;
        Dictionary<string, dynamic> _globalContext;
        bool _monitorFiles = true;

        /// <summary>
        /// Occurs when a plugin is reloaded.
        /// </summary>
        public event PluginReloadedEventHandler PluginReloaded;
        /// <summary>
        /// Occurs when an error is encountered during
        /// a plugin reload.
        /// </summary>
        public event PluginReloadErrorEventHandler PluginReloadedError;

        /// <summary>
        /// Get or set whether the manager should monitor plugins
        /// that were created from file and reload them when
        /// they change.
        /// </summary>
        public bool MonitorFiles
        {
            get { return _monitorFiles; }
            set
            {
                _monitorFiles = value;

                foreach (KeyValuePair<string, FileSystemWatcher> kvp in _watchers)
                {
                    kvp.Value.EnableRaisingEvents = _monitorFiles;
                }
            }
        }

        /// <summary>
        /// Get the collection of plugins managed by this PluginManager.
        /// </summary>
        public PluginCollection Plugins
        {
            get { return _plugins; }
        }

        /// <summary>
        /// Access the global context of the manager. These variables are accessible by
        /// all plugins being managed.
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public dynamic this[string varName]
        {
            get
            {
                return _globalContext[varName];
            }
            set
            {
                _globalContext[varName] = value;
            }
        }

        /// <summary>
        /// Create a default instance of PluginManager.
        /// </summary>
        public PluginManager()
        {
            _plugins = new PluginCollection();
            _watchers = new Dictionary<string, FileSystemWatcher>();
            _lastRead = new Dictionary<string, DateTime>();
            _globalContext = new Dictionary<string, dynamic>();
        }

        /// <summary>
        /// Add the specified plugin to this manager.
        /// </summary>
        /// <param name="plugin"></param>
        public PluginCollection.AddResult AddPlugin(IIronPlugin plugin)
        {
            AddSelfToPluginContext(plugin);
            // Initialize the plugin so we have access to the source
            plugin.RunPlugin();
            PluginCollection.AddResult result = _plugins.AddPlugin(plugin);
            if (result != PluginCollection.AddResult.NotAdded)
            {
                if (plugin.Source != null)
                {
                    if (System.IO.File.Exists(plugin.Source.Path))
                    {
                        string dir = Path.GetDirectoryName(plugin.Source.Path);
                        // This creates the watcher on the directory if it doesn't already exist
                        GetWatcherForPath(dir);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Remove the specified plugin from this manager.
        /// </summary>
        /// <param name="plugin"></param>
        public void RemovePlugin(IIronPlugin plugin)
        {
            if (_plugins.Remove(plugin))
            {
                // If this plugin has a watcher and no other plugins are on the 
                // path of this plugins watcher, remove the watcher.
                FileSystemWatcher watcher = GetWatcherForPath(plugin.Source.Path);
                if (watcher != null)
                {
                    List<IIronPlugin> pluginsOnPath = GetPluginsForPath(plugin.Source.Path);
                    if (pluginsOnPath.Count == 0)
                    {
                        _watchers.Remove(Path.GetDirectoryName(plugin.Source.Path));
                        watcher.Dispose();
                    }
                }
                RemoveSelfFromPluginContext(plugin);
            }
        }

        /// <summary>
        /// Cleanup all resources used by the manager.
        /// </summary>
        public void Dispose()
        {
            foreach (KeyValuePair<string, FileSystemWatcher> kvp in _watchers)
                kvp.Value.Dispose();
            foreach (IIronPlugin ipl in _plugins)
                ipl.Dispose();
        }

        /// <summary>
        /// Get the full path for the specified plugin.
        /// </summary>
        /// <param name="plugin"></param>
        /// <returns></returns>
        private string GetPluginFullPath(IIronPlugin plugin)
        {
            string path = plugin.Source.Path;
            if (string.IsNullOrEmpty(path))
                path = ".";
            return Path.GetFullPath(path);
        }

        /// <summary>
        /// Get all plugins managed by this PluginManager that
        /// are in the the specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private List<IIronPlugin> GetPluginsForPath(string path)
        {
            List<IIronPlugin> plugins = new List<IIronPlugin>();
            string fileName = Path.GetFileName(path);

            if (!string.IsNullOrEmpty(fileName))
            {
                // The path includes a file name.
                foreach (IIronPlugin plg in _plugins)
                {
                    if (Path.GetFileName(plg.Source.Path) == fileName)
                        plugins.Add(plg);
                }
            }
            else
            {
                string dir = path;// Path.GetDirectoryName(path);
                foreach (IIronPlugin plg in _plugins)
                {
                    string plgDir = Path.GetDirectoryName(GetPluginFullPath(plg));
                    if (plgDir == dir)
                        plugins.Add(plg);
                }
            }
            return plugins;
        }

        /// <summary>
        /// Get the FileSystemWatcher on the specified path. The path should
        /// be a directory.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private FileSystemWatcher GetWatcherForPath(string path)
        {
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);

            FileSystemWatcher watcher = null;
            if (!_watchers.TryGetValue(path, out watcher))
            {
                if (string.IsNullOrWhiteSpace(path))
                    path = ".";
                path = Path.GetFullPath(path);
                watcher = _watchers[path] = new FileSystemWatcher(path);
                watcher.EnableRaisingEvents = MonitorFiles;
                watcher.NotifyFilter = NotifyFilters.LastWrite;
                watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            }
            return watcher;
        }

        /// <summary>
        /// Reload the specified plugin.
        /// </summary>
        /// <param name="plugin"></param>
        private void ReloadPlugin(IIronPlugin plugin)
        {
            try
            {
                plugin.ReloadPlugin();
                AddSelfToPluginContext(plugin);
                plugin.RunPlugin();

                OnPluginReloaded(new PluginReloadEventArgs(plugin));
            }
            catch (Exception e)
            {
                if (PluginReloadedError != null)
                {
                    OnPluginReloadError(new PluginReloadErrorEventArgs(plugin, e));
                }
                else throw e;
            }
        }

        /// <summary>
        /// Add this manager to the plugin context.
        /// </summary>
        /// <param name="plugin"></param>
        private void AddSelfToPluginContext(IIronPlugin plugin)
        {
            plugin.AddContextVariables(new Context.Variable("__mgr__", this));
        }

        /// <summary>
        /// Remove this manager from the plugin context.
        /// </summary>
        /// <param name="plugin"></param>
        private void RemoveSelfFromPluginContext(IIronPlugin plugin)
        {
            plugin.RemoveContextVariables(new Context.Variable("__mgr__", this));
        }

        /// <summary>
        /// Raise the PluginReloaded event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPluginReloaded(PluginReloadEventArgs args)
        {
            if (PluginReloaded != null)
                PluginReloaded(this, args);
        }

        /// <summary>
        /// Raise the PluginReloadedError event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPluginReloadError(PluginReloadErrorEventArgs args)
        {
            if (PluginReloadedError != null)
                PluginReloadedError(this, args);
        }

        /// <summary>
        /// File/dir changed on one of our plugin paths.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                try
                {
                    DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath),
                             lastReadTime;
                    bool checkTime = true;

                    // The Changed event often gets written more than once for the same file,
                    // so record the last write time and check if it's the same later, and
                    // don't reload if we don't need to.
                    if (!_lastRead.TryGetValue(e.FullPath, out lastReadTime))
                    {
                        _lastRead[e.FullPath] = lastWriteTime;
                        checkTime = false;
                    }

                    if (lastWriteTime != lastReadTime || !checkTime)
                    {
                        _lastRead[e.FullPath] = lastWriteTime;

                        List<IIronPlugin> plugins = GetPluginsForPath(e.FullPath);
                        if (plugins.Count > 0)
                        {
                            System.Threading.Thread.Sleep(100);
                            foreach (IIronPlugin plugin in plugins)
                            {
                                ReloadPlugin(plugin);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.LogError(ex);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Represents the method that handles the PluginReloaded event of PluginManager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void PluginReloadedEventHandler(object sender, PluginReloadEventArgs args);
        /// <summary>
        /// Represents the method that handles the PluginReloadError event of PluginManager.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void PluginReloadErrorEventHandler(object sender, PluginReloadErrorEventArgs args);
    }
}
