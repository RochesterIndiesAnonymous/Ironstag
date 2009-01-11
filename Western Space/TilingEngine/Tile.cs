using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.TilingEngine
{
    public class Tile
    {
        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Tile()
        {
        }

        public Tile(Texture2D texture)
        {
            this.texture = texture;
        }
    }
}
