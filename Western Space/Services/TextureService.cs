using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.Services
{
    public class TextureService : ITextureService
    {
        /// <summary>
        /// The collection of currently loaded textures. This used as a cacheing mechanism to 
        /// avoid loading multiple textures twice.
        /// </summary>
        private Dictionary<string, Texture2D> textures;

        /// <summary>
        /// Constructor
        /// </summary>
        public TextureService()
        {
            textures = new Dictionary<string, Texture2D>();
        }

        #region ITextureService Members

        /// <summary>
        /// The collection of currently loaded textures. This used as a cacheing mechanism to 
        /// avoid loading multiple textures twice.
        /// </summary>
        public Dictionary<string, Texture2D> Textures
        {
            get { return textures; }
        }

        /// <summary>
        /// Gets a texture from the content directory. Uses the cache if it has already been loaded
        /// </summary>
        /// <param name="assetName">The name of the texture that is needed</param>
        /// <returns>The texture that was requested</returns>
        public Texture2D GetTexture(string assetName)
        {
            if (!this.Textures.ContainsKey(assetName) || this.Textures[assetName] == null)
            {
                this.Textures[assetName] = ScreenManager.Instance.Content.Load<Texture2D>(assetName);
            }

            return this.Textures[assetName];
        }

        #endregion
    }
}
