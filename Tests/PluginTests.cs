using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PluginTests
    {
        private IronPlugins.Plugin.IIronPlugin CreatePlugin()
        {
            Guid guid = new Guid("bb25656b-4f1c-44d0-9824-e52bebf9db32");
            return new IronPlugins.Plugin.StringPlugin("x = 12**2", guid);
        }

        [Test]
        public void CollectionCount()
        {
            IronPlugins.Plugin.PluginCollection col = new IronPlugins.Plugin.PluginCollection();
            col.Add(new IronPlugins.Plugin.StringPlugin("")); // Dummy
            Assert.IsTrue(col.Count == 1);

            // Default list replaces duplicate plugins
            col.Add(CreatePlugin());
            Assert.IsTrue(col.Count == 2);

            col.Add(CreatePlugin());
            Assert.IsTrue(col.Count == 2);
            
            col.DuplicateMode = IronPlugins.Plugin.PluginCollection.DuplicatePluginMode.Allow;
            col.Add(CreatePlugin());
            Assert.IsTrue(col.Count == 3);
        }

        [Test]
        public void ManagerFileWatcher()
        {
            // PluginManager should be able to reload plugins that exist on file when the
            // plugin's file is changed.
            System.Threading.ManualResetEvent reset = new System.Threading.ManualResetEvent(false);
            IronPlugins.PluginManager mgr = new IronPlugins.PluginManager();
            mgr.PluginReloaded += new IronPlugins.PluginManager.PluginReloadedEventHandler(
                delegate(object sender, IronPlugins.PluginReloadEventArgs args)
                {
                    reset.Set();
                }
            );
            IronPlugins.Plugin.FilePlugin filePlugin = new IronPlugins.Plugin.FilePlugin("file_manager_test.py");
            mgr.AddPlugin(filePlugin);
            // Write a new attribute to the file. This imitates someone changing a plugin file.
            using (System.IO.TextWriter writer = new System.IO.StreamWriter("file_manager_test.py"))
            {
                writer.Write("new_attribute = 82");
            }
            reset.WaitOne(); // Wait for the plugin to be reloaded
            Assert.IsTrue(filePlugin.Invoke("new_attribute") == 82); // Check if the new attribute exists

            mgr.Dispose();
        }
    }
}
