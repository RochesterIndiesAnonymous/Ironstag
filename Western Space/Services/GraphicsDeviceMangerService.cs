using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework;

namespace WesternSpace.Services
{
    public class GraphicsDeviceMangerService : IGraphicsDeviceManagerService
    {
        private GraphicsDeviceManager graphics;

        public GraphicsDeviceMangerService(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        #region IGraphicsDeviceManagerService Members

        public GraphicsDeviceManager GraphicsDevice
        {
            get { return graphics; }
        }

        #endregion
    }
}
