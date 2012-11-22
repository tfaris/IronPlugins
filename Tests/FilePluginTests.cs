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
        static dynamic pg;
        /// <summary>
        /// Create a file plugin from our test script.
        /// </summary>
        /// <returns></returns>
        public static dynamic CreateFilePlugin()
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
        public void ArbitraryArgumentList()
        {
            dynamic plugin = CreateFilePlugin();
            Assert.IsTrue(plugin.add2(1, 2, 3, 4, 5) == 15);
        }

        [Test]
        public void SimpleOldTypeClass()
        {
            // Plugin scripts should be able to specify classes. IronPython supports old style and new style
            // classes, so we should make sure we can handle both.
            dynamic plugin = CreateFilePlugin();
            dynamic simpleObject = plugin.SimpleClass();
            Assert.IsTrue(simpleObject.hello_simple("from the plugin") == "Hello,from the plugin!");
        }

        [Test]
        public void RuntimeBindException()
        {
            // Invoking a member through the dynamic type that does not exist in the context
            // can only raise a RuntimeBinderException.
            dynamic plugin = CreateFilePlugin();
            dynamic simpleObject = plugin.SimpleClass();
            Assert.Throws<Microsoft.CSharp.RuntimeBinder.RuntimeBinderException>(() => simpleObject.does_not_exist());
        }

        [Test]
        public void MissingMemberException()
        {
            // Calling the Invoke method with a member that does not exist in the context
            // should raise a MissingMemberException.
            IronPlugins.Plugin.PluginBase plugin = CreateFilePlugin();
            Assert.Throws<System.MissingMemberException>(() => plugin.Invoke("does_not_exist"));
        }

        [Test]
        public void Inheritance()
        {
            // We can inherit types in scripts as long as we have their type in context, but for IPY this has to be the PythonType, NOT the CLR type.
            dynamic plugin = CreateFilePlugin();            
            dynamic inherited = plugin.MyInheritedType();
            Assert.IsTrue(inherited.DoMath(5) == 25);
        }

        [Test]
        public void SpecToString()
        {
            // A plugin can override ToString by making sure there is an
            // attribute/function titled ToString on the context
            dynamic plugin = CreateFilePlugin();
            Assert.IsTrue(plugin.ToString() == "From the plugin!");
        }

        [Test]
        public void SpecGuid()
        {
            // A plugin can override Guid by making sure there is an
            // attribute/function titled Guid on the context that returns
            // either a System.Guid, string, or byte array.
            dynamic plugin = CreateFilePlugin();
            Assert.IsTrue(plugin.Guid == new System.Guid("852bb7e1-1839-4a19-a9f0-78c1f6e29053"));
        }
    }

}
