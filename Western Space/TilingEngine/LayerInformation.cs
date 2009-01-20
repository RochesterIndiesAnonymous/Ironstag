using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.TilingEngine
{
    /// <summary>
    /// Used as an information holder for loading a layer
    /// </summary>
    internal struct LayerInformation
    {
        /// <summary>
        /// The name of the texture that needs to be used for the associated color
        /// </summary>
        internal string Name;

        /// <summary>
        /// The color of the associated texture in the tile map
        /// </summary>
        internal Color Color;
    }
}
