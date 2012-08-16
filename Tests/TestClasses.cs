using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
    /// <summary>
    /// A basic class to be overidden by plugin scripts.
    /// </summary>
    public class InheritMe
    {
        public virtual float DoMath(float input)
        {
            return input;
        }
    }
}
