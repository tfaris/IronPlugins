using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Context
{
    /// <summary>
    /// Represents a plugin context variable.
    /// </summary>
    public class Variable
    {
        dynamic _value;
        string _name;

        /// <summary>
        /// Get the name of the variable
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get or set the value of the variable.
        /// </summary>
        public dynamic Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Create an instance of Variable with the specified name.
        /// </summary>
        /// <param name="variableName"></param>
        public Variable(string variableName)
        {
            _name = variableName;
        }

        /// <summary>
        /// Create an instance of Variable with the specified name and value.
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="value"></param>
        public Variable(string variableName, dynamic value)
            : this(variableName)
        {
            _value = value;
        }
    }
}
