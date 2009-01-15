using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.TilingEngine;

namespace WesternSpace.ServiceInterfaces
{
    interface ILayerService
    {
        Dictionary<string, TileMapLayer> Layers
        {
            get;
        }
    }
}
