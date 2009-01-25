using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.Interfaces;
using WesternSpace.ServiceInterfaces;
using System.Xml.Linq;

namespace WesternSpace
{
    // AKA Sprite-sheet.
    public class SubTextureSheet : IXElementOutput
    {
        // By default, undefined. To be set by the TextureService.
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private Texture2D texture;

        public Texture2D Texture
        {
            get { return texture; }
        }

        private SubTexture[] subTextures;

        public SubTexture[] SubTextures
        {
            get { return subTextures; }
            set { subTextures = value; }
        }

        public Rectangle Rectangle
        {
            get 
            {
                return new Rectangle(0,0, Texture.Width, Texture.Height);
            }
        }

        private Rectangle[] rectangles;

        public Rectangle[] Rectangles
        {
            get { return rectangles; }
        }

        private int width;

        public int Width
        {
            get { return width; }
         }

        private int height;

        public int Height
        {
            get { return height; }
        }

        private int subTextureWidth;

        public int SubTextureWidth
        {
            get { return subTextureWidth; }
        }

        private int subTextureHeight;

        public int SubTextureHeight
        {
            get { return subTextureHeight; }
        }

        public SubTextureSheet(XElement xelement)
        {
            ITextureService textureService = (ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService));
            this.texture = textureService.GetTexture(xelement.Attribute("n").Value);

            Int32.TryParse(xelement.Attribute("w").Value, out this.subTextureWidth);
            Int32.TryParse(xelement.Attribute("h").Value, out this.subTextureHeight);

            this.width = texture.Width / this.subTextureWidth;
            this.height = texture.Width / this.subTextureHeight;

            subTextures = new SubTexture[width * height];
            rectangles = new Rectangle[width * height];

            // Now we generate our textures:
            for (int index = 0; index < (width * height); ++index)
            {
                rectangles[index] = new Rectangle((index % width) * subTextureWidth, (index / width) * subTextureHeight, 
                                                  subTextureWidth, subTextureHeight);
                subTextures[index] = new SubTexture(this, index);
            }
        }

        #region IXElementOutput Members

        public System.Xml.Linq.XElement ToXElement()
        {
            return new XElement("Sh", new XAttribute("w", subTextureWidth), new XAttribute("h", SubTextureHeight));
        }

        #endregion
    }
}