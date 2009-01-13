using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.ServiceInterfaces
{
    interface ICamera
    {
        public int XOffset
        {
            get;
        }

        public int YOffset
        {
            get;
        }
    }
}
