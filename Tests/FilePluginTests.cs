using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class FilePluginTests
    {
        dynamic pg;
        public dynamic CreateFilePlugin()
        {
            if (pg == null)
            {
                pg = new IronPlugins.Plugin.FilePlugin("test_script.py");
                pg.RegisterType<InheritMe>();
            }
            pg.RunPlugin();
            return pg;
        }

        [Test]
        public void SimpleFunction()
        {
            dynamic plugin = CreateFilePlugin();
            Assert.IsTrue(plugin.add(10, 20) == 30);
        }

        [Test]
        public void SimpleClass()
        {
            dynamic plugin = CreateFilePlugin();
            dynamic simpleObject = plugin.SimpleClass();
            Assert.IsTrue(simpleObject.hello_simple("from the plugin") == "Hello,from the plugin!");
        }

        [Test]
        public void MissingMethodException()
        {
            // Attempting to call a method that does not exist on a plugin class instance
            dynamic plugin = CreateFilePlugin();
            dynamic simpleObject = plugin.SimpleClass();
            Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() => simpleObject.does_not_exist());
        }

        [Test]
        public void Inheritance()
        {
            // We can inherit types in scripts as long as we have their type, but for IPY this has to be the PythonType, NOT the CLR type
            dynamic plugin = CreateFilePlugin();            
            dynamic inherited = plugin.MyInheritedType();
            Assert.IsTrue(inherited.DoMath(5) == 25);
        }
    }

}
