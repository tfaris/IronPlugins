using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronPlugins.Context
{
    /// <summary>
    /// Represents a collection of Variables.
    /// </summary>
    public class VariableList : Dictionary<string,dynamic>, IEnumerable<Variable>
    {
        /// <summary>
        /// Add the specified variable.
        /// </summary>
        /// <param name="var"></param>
        public void Add(Variable var)
        {
            this[var.Name] = var.Value;
        }

        public new IEnumerator<Variable> GetEnumerator()
        {
            List<Variable> variables = new List<Variable>();            
            foreach (KeyValuePair<string, dynamic> kvp in (Dictionary<string,dynamic>)this)
                variables.Add(new Variable(kvp.Key,kvp.Value));
            return variables.GetEnumerator();
        }
    }
}
