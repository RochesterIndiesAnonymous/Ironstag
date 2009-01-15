using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;

namespace WesternSpace.Services
{
    class LayerService : ILayerService
    {
        private Dictionary<string, TileMapLayer> layers;

        public LayerService()
        {
            layers = new Dictionary<string, TileMapLayer>();
        }

        #region ILayerService Members

        public Dictionary<string, WesternSpace.TilingEngine.TileMapLayer> Layers
        {
            get { return layers; }
        }

        #endregion
    }
}
