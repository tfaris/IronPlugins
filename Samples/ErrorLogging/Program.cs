/*
 * IronPlugins.Samples.ErrorLogging 
 * --------------------------------
 * A sample showing how an IronPlugin
 * could be used to attach to a the .NET
 * Trace system and log or display errors,
 * debug info and more.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronPlugins.Plugin;

namespace ErrorLogging
{
    class Program
    {
        static void Main(string[] args)
        {
            IronPlugins.Engine.AddSearchPath(@"C:\Program Files (x86)\IronPython 2.7\Lib");
            System.Diagnostics.Trace.TraceInformation("Pre-plugin load. You shouldn't see this.");
            FilePlugin pluginLog = new FilePlugin("error_logger.py");
            pluginLog.RunPlugin();
            System.Diagnostics.Trace.TraceInformation("Post-plugin load...you should see this.");
            System.Windows.Forms.Application.Run(new Form1());
        }
    }
}
