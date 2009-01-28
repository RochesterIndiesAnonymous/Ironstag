using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WesternSpace.Screens;

namespace WesternSpace.Interfaces
{
    /// <summary>
    /// Something that has a Screen.
    /// </summary>
    interface IScreenComponent
    {
        Screen ParentScreen
        {
            get;
            set;
        }
    }
}
