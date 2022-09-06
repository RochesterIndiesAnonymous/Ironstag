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
        /// The name of the file that contains all information about each SubTextureSheet that gets used
        /// in the game.
        /// </summary>
        public static string SHEET_INFO_FILENAME = "Textures\\Sheets";

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

            XDocument sheetInfos = ScreenManager.Instance.Content.Load<XDocument>(SHEET_INFO_FILENAME);
            IEnumerable<XElement> sheetElements = sheetInfos.Descendants("Sh");
            foreach (XElement sheetElement in sheetElements)
            {
                int subTexWidth = Int32.Parse(sheetElement.Attribute("w").Value);
                int subTexHeight = Int32.Parse(sheetElement.Attribute("h").Value);
                string textureName = sheetElement.Attribute("n").Value;

                this.Sheets[sheetElement.Attribute("n").Value] = new SubTextureSheet(GetTexture(textureName), 
                                                                                        subTexWidth, subTexHeight);
            }
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

        // Primarily used in the editor; easier to switch through:
        public SubTextureSheet[] SheetsArray
        {
            get { return sheets.Values.ToArray<SubTextureSheet>(); }
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
            return this.Sheets[assetName];
        }

        #endregion
    }
}
