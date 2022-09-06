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
        /// <summary>
        /// The device graphics manager associated with this service instance
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graphics">The graphics manager this service holds</param>
        public GraphicsDeviceMangerService(GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
        }

        #region IGraphicsDeviceManagerService Members

        /// <summary>
        /// The manager of the graphics device this service holds
        /// </summary>
        public GraphicsDeviceManager GraphicsDevice
        {
            get { return graphics; }
        }

        #endregion
    }
}
