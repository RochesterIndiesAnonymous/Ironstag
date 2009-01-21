using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.Interfaces;
using WesternSpace.ServiceInterfaces;
using System.Xml.Linq;

namespace WesternSpace.TilingEngine
{
    public class Tile
    {
        private Texture2D[,] textures;

        public Texture2D[,] Textures
        {
            get { return textures; }
            set { textures = value; }
        }

        public int LayerCount
        {
            get { return textures.GetLength(0); }
        }

        public int SubLayerCount
        {
            get { return textures.GetLength(1); }
        }

        public Tile()
        {
        }

        public Tile(Texture2D[,] textures)
        {
            this.textures = textures;
        }
    }
}
