using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace WesternSpace.Interfaces
{
    /// <summary>
    /// This interface allows for an object to be turned into an XElement to be saved in XML
    /// </summary>
    public interface IXElementOutput
    {
        /// <summary>
        /// Returns an XElement representing this object
        /// </summary>
        XElement ToXElement();
    }
}
