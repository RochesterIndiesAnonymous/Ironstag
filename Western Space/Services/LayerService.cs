using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;

namespace WesternSpace.Services
{
    /// <summary>
    /// The implementation of the layer service
    /// </summary>
    public class LayerService : ILayerService
    {
        /// <summary>
        /// The collection of layers that are currently loaded
        /// </summary>
        private Dictionary<string, TileMapLayer> layers;

        /// <summary>
        /// Constructor
        /// </summary>
        public LayerService()
        {
            layers = new Dictionary<string, TileMapLayer>();
        }

        #region ILayerService Members

        /// <summary>
        /// The collection of layers that are currently loaded
        /// </summary>
        public Dictionary<string, WesternSpace.TilingEngine.TileMapLayer> Layers
        {
            get { return layers; }
        }

        #endregion
    }
}
