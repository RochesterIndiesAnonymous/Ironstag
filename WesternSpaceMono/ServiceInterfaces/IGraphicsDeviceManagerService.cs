using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// Provides an interface for accessing the graphics device manager service
    /// </summary>
    public interface IGraphicsDeviceManagerService
    {
        /// <summary>
        /// The manager of the graphics device this service holds
        /// </summary>
        GraphicsDeviceManager GraphicsDevice
        {
            get;
        }
    }
}
