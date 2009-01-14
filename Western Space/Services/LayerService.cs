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
        private Dictionary<string, TileMap> layers;

        public LayerService()
        {
            layers = new Dictionary<string, TileMap>();
        }

        #region ILayerService Members

        public Dictionary<string, WesternSpace.TilingEngine.TileMap> Layers
        {
            get { return layers; }
        }

        #endregion
    }
}
