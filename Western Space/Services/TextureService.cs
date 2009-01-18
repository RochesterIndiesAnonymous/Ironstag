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
        private Dictionary<string, Texture2D> textures;

        public TextureService()
        {
            textures = new Dictionary<string, Texture2D>();
        }

        #region ITextureService Members

        public Dictionary<string, Texture2D> Textures
        {
            get { return textures; }
        }

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
