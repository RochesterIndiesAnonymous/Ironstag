using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.ServiceInterfaces
{
    public interface IGraphicsDeviceManagerService
    {
        GraphicsDeviceManager GraphicsDevice
        {
            get;
        }
    }
}
