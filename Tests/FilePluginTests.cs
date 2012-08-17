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
        /// <summary>
        /// Create a file plugin from our test script.
        /// </summary>
        /// <returns></returns>
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
            IronPlugins.Plugin.Plugin plugin = CreateFilePlugin();
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
    }

}
