using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;

namespace WesternSpace.Services
{
    public class TextureService : ITextureService
    {
        /// <summary>
        /// The collection of currently loaded textures. This used as a cacheing mechanism to 
        /// avoid loading multiple textures twice.
        /// </summary>
        private Dictionary<string, Texture2D> textures;

        private Dictionary<string, SubTextureSheet> sheets;

        /// <summary>
        /// Constructor
        /// </summary>
        public TextureService()
        {
            textures = new Dictionary<string, Texture2D>();
            sheets = new Dictionary<string, SubTextureSheet>();
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
        /// The collection of currently loaded SubTextureSheets. This used as a cacheing mechanism to 
        /// avoid loading multiple sheets twice.
        /// </summary>
        public Dictionary<string, SubTextureSheet> Sheets
        {
            get { return sheets; }
        }

        /// <summary>
        /// Gets a texture from the content directory. Uses the cache if it has already been loaded
        /// </summary>
        /// <param name="assetName">The name of the texture that is needed.</param>
        /// <returns>If assetName is a non-empty string representing the filename of the texture,
        /// then the texture that was requested is returned. Otherwise, null is returned.</returns>
        public Texture2D GetTexture(string assetName)
        {
            if( assetName == null || assetName == "")
            {
                return null;
            }

            if (!this.Textures.ContainsKey(assetName) || this.Textures[assetName] == null)
            {
                this.Textures[assetName] = ScreenManager.Instance.Content.Load<Texture2D>(assetName);
                this.Textures[assetName].Name = assetName;
            }

            return this.Textures[assetName];
        }

        /// <summary>
        /// Gets a subTextureSheet from the content directory. Uses the cache if it has already been loaded
        /// </summary>
        /// <param name="assetName">The name of the sheet that is needed</param>
        /// <returns>The SubTextureSheet that was requested</returns>
        public SubTextureSheet GetSheet(string assetName)
        {
            if (!this.Sheets.ContainsKey(assetName) || this.Sheets[assetName] == null)
            {
                XDocument doc = ScreenManager.Instance.Content.Load<XDocument>(assetName);
                this.Sheets[assetName] = new SubTextureSheet(doc.Root);
                this.Sheets[assetName].Name = assetName;
            }

            return this.Sheets[assetName];
        }

        #endregion
    }
}
