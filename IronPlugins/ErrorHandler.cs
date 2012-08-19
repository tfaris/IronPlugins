using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace IronPlugins
{
    static class ErrorHandler
    {
        static TraceSource trace = new TraceSource("IronPlugins");

        public static void LogError(Exception err)
        {
            trace.TraceEvent(TraceEventType.Error,
                             -1,
                             err.Message);
        }
    }
}
