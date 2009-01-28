using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using WesternSpace.TilingEngine;

namespace WesternSpace.DrawableComponents.EditorUI
{
    /// <summary>
    /// Any item that should change it's behavior or appearance when a TileSelector selects a different 
    ///  tile/set of tiles.
    /// </summary>
    interface ITilePropertyComponent
    {
        /// <summary>
        /// Any time the TileSelector changes what it has selected, it notifies
        ///  all TilePropertyComponents of what tiles it currently has selected with
        ///  this method.
        /// </summary>
        void OnTileSelectionChange();
    }
}
