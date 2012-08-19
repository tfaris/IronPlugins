/// Class       : nichevision.forensics.armedxpert.Scripting.PythonPluginException
/// Created On  : 4/30/2012 5:03:23 PM
/// Created By  : tfaris
/// Organization: 
///
/// Description : 

using System;
using System.Collections.Generic;
using System.Text;

namespace IronPlugins.Plugin
{
    /// <summary>
    /// Represents a runtime error raised by a python plugin.
    /// </summary>
    public class PythonPluginException : Exception
    {
        IronPython.Runtime.Exceptions.TraceBack pythonTraceback;
        string errorName, message;

        /// <summary>
        /// Get the python traceback for the error.
        /// </summary>
        public IronPython.Runtime.Exceptions.TraceBack PythonTraceback
        {
            get { return pythonTraceback; }
        }

        /// <summary>
        /// Get the name of the python error.
        /// </summary>
        public string ErrorName
        {
            get { return errorName; }
            protected set { errorName = value; }
        }

        /// <summary>
        /// Get a message that describes the error.
        /// </summary>
        public override string Message
        {
            get
            {                
                return message;
            }
        }

        /// <summary>
        /// Create an instance of PythonPluginException that considers the specified exception
        /// an error coming from python. Attempts to extract more detailed information from the specified error.
        /// </summary>
        /// <param name="error"></param>
        public PythonPluginException(Exception error) : base(null,error)
        {
            IronPython.Runtime.PythonTuple errorTuple =
                IronPython.Runtime.Operations.PythonOps.GetExceptionInfoLocal(IronPython.Runtime.DefaultContext.Default, error);
            if (errorTuple != null)
            {
                for (int i = 0; i < errorTuple.Count; i++)
                {
                    object errorTupleObject = errorTuple[i];
                    if (errorTupleObject is IronPython.Runtime.Types.PythonType)
                    {
                        try
                        {
                            object typeName = IronPython.Runtime.Operations.PythonOps.GetBoundAttr(IronPython.Runtime.DefaultContext.Default,
                                                                                                   errorTupleObject,
                                                                                                   "__name__");
                            if (typeName != null)
                                ErrorName = typeName.ToString();
                            else
                                ErrorName = "PythonPluginException";
                        }
                        catch (Exception)
                        {
                            ErrorName = "PythonPluginException";
                        }
                    }
                    else if (errorTupleObject is IronPython.Runtime.Exceptions.PythonExceptions.BaseException)
                    {
                        IronPython.Runtime.Exceptions.PythonExceptions.BaseException baseException =
                            (IronPython.Runtime.Exceptions.PythonExceptions.BaseException)errorTupleObject;
                        if (baseException.clsException != null)
                        {
                            if (baseException.clsException is Microsoft.Scripting.SyntaxErrorException)
                            {
                                Microsoft.Scripting.SyntaxErrorException syntxError =
                                    (Microsoft.Scripting.SyntaxErrorException)error;
                                this.message = string.Format("Error: {0}\r\nSource: {1}\r\nSpan: {2}",
                                    syntxError.Message,
                                    System.IO.Path.GetFileName(syntxError.SourcePath),
                                    syntxError.RawSpan);
                                this.ErrorName = string.Format("Syntax error - {0}", syntxError.Severity);
                            }
                            else
                            {
                                this.message = baseException.message != null ? baseException.message.ToString() : "No detailed error information available.";
                            }
                        }
                    }
                    else if (errorTupleObject is IronPython.Runtime.Exceptions.TraceBack)
                    {
                        pythonTraceback = (IronPython.Runtime.Exceptions.TraceBack)errorTupleObject;
                        StringBuilder trace = BuildTraceback(pythonTraceback);
                        this.message = string.Format("Error: {0}\r\n{1}", error.Message, trace);
                    }
                }
            }
        }

        /// <summary>
        /// Recurively builds an stack-trace string from the IronPython traceback.
        /// </summary>
        /// <param name="traceback"></param>
        /// <returns></returns>
        private StringBuilder BuildTraceback(IronPython.Runtime.Exceptions.TraceBack traceback)
        {
            StringBuilder traceBuilder = new StringBuilder();
            if (traceback == null)
                return traceBuilder;
            if (traceback.tb_next != null)
            {
                traceBuilder.AppendFormat("{0}\r\n", BuildTraceback(traceback.tb_next));
            }
            if (traceback.tb_frame != null && traceback.tb_frame is IronPython.Runtime.Exceptions.TraceBackFrame)
            {
                IronPython.Runtime.Exceptions.TraceBackFrame frame = (IronPython.Runtime.Exceptions.TraceBackFrame)traceback.tb_frame;
                if (frame.f_code != null)
                {
                    traceBuilder.AppendFormat("at {0} Line {1}",
                                              frame.f_code.co_filename,
                                              traceback.tb_lineno);
                }
            }
            return traceBuilder;
        }
    }
}