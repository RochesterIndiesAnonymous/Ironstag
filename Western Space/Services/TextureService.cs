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

        #endregion
    }
}
