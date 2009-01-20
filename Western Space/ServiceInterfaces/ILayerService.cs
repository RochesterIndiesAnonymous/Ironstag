using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.TilingEngine;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// The interface for accessing the layer interface
    /// </summary>
    public interface ILayerService
    {
        /// <summary>
        /// The collection of layers that are currently loaded
        /// </summary>
        Dictionary<string, TileMapLayer> Layers
        {
            get;
        }
    }
}
