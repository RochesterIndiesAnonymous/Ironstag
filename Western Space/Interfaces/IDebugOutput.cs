using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Interfaces
{
    /// <summary>
    /// This interface is used to add a string to the debugging area of the screen
    /// </summary>
    public interface IDebugOutput
    {
        /// <summary>
        /// Gets the string to print to the debugging area
        /// </summary>
        string Output
        {
            get;
        }
    }
}
