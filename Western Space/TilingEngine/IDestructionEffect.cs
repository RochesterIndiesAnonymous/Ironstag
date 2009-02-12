using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.TilingEngine
{
    interface IDestructionEffect
    {
        void OnDestruct(DestructableTile destructable);
    }
}
