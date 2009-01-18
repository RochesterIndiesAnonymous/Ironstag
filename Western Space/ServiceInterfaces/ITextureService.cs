using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.ServiceInterfaces
{
    public interface ITextureService
    {
        Dictionary<string, Texture2D> Textures
        {
            get;
        }

        Texture2D GetTexture(string assetName);
    }
}
