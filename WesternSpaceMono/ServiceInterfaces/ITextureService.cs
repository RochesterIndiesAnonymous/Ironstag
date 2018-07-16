using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// The interface for accessing the texture service
    /// </summary>
    public interface ITextureService
    {
        /// <summary>
        /// The collection of currently loaded textures. This used as a cacheing mechanism to 
        /// avoid loading multiple textures twice.
        /// </summary>
        Dictionary<string, Texture2D> Textures
        {
            get;
        }

        /// <summary>
        /// The collection of currently loaded SubTextureSheets. This used as a cacheing mechanism to 
        /// avoid loading multiple sheets twice.
        /// </summary>
        Dictionary<string, SubTextureSheet> Sheets
        {
            get;
        }

        SubTextureSheet[] SheetsArray
        {
            get;
        }

        /// <summary>
        /// Gets a texture from the content directory. Uses the cache if it has already been loaded
        /// </summary>
        /// <param name="assetName">The name of the texture that is needed</param>
        /// <returns>The texture that was requested</returns>
        Texture2D GetTexture(string assetName);

        /// <summary>
        /// Gets a subTextureSheet from the content directory. Uses the cache if it has already been loaded
        /// </summary>
        /// <param name="assetName">The name of the sheet that is needed</param>
        /// <returns>The SubTextureSheet that was requested</returns>
        SubTextureSheet GetSheet(string assetName);
    }
}
