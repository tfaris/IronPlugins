using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    /// <summary>
    /// Tests for StringPlugins.
    /// </summary>
    [TestFixture]
    public class PluginStringTests
    {
        /// <summary>
        /// Create a string plugin with optional context variables.
        /// </summary>
        /// <param name="pluginCode"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public dynamic CreateStringPlugin(string pluginCode,params KeyValuePair<string,object>[] args)
        {
            dynamic plugin = new IronPlugins.Plugin.StringPlugin(pluginCode);
            plugin.AddContextVariables(args);
            plugin.RunPlugin();
            return plugin;
        }

        [Test]
        public void SimpleReturnValueTest()
        {         
            Assert.IsTrue(CreateStringPlugin("5").RunPlugin() == 5);
        }

        [Test]
        public void SimpleAttributeTest()
        {
            string code =
@"SAVEFILE_EXTENSION = '.png'
BITRATE = 160
SOME_CONFIGURATION_LIST = [1,2,3]
";
            dynamic plug = CreateStringPlugin(code);
            // Imagine these are some kind of configuration/settings for your application
            Assert.IsTrue(plug.SAVEFILE_EXTENSION == ".png");
            Assert.IsTrue(plug.BITRATE == 160);
            Assert.IsTrue(plug.SOME_CONFIGURATION_LIST[1] == 2);
        }

        [Test]
        public void SimpleMath()
        {
            dynamic plug = CreateStringPlugin("x = 15 + 1");
            Assert.IsTrue(plug.x == 16);
        }

        [Test]
        public void VariableMath()
        {
            dynamic plug = CreateStringPlugin("x = y + 1", new KeyValuePair<string,object>("y",954));
            //plug.y = 954;
            //plug.RunPlugin();
            Assert.IsTrue(plug.x == 955);
        }

        [Test]
        public void SimpleFunction()
        {
            dynamic plug = CreateStringPlugin(
@"def add():
 return 10+11");
            Assert.IsTrue(plug.add() == 21);
        }

        [Test]
        public void FunctionWithArgs()
        {
            dynamic plug = CreateStringPlugin(
@"def add(x,y):
 return x+y");
            Assert.IsTrue(plug.add(128, 100) == 228);
        }

        [Test]
        public void DynamicClass()
        {
            // Define a python class, create an instance and call a method, passing it an argument
            dynamic plug = CreateStringPlugin(
@"class SimpleClass(object):
    def hello_simple(self,target):
        return 'Hello,%s!' % target");
            string msg = plug.SimpleClass().hello_simple("from the plugin");
            Assert.IsTrue(msg == "Hello,from the plugin!");
        }
    }
}
