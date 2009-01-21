using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Interfaces;
using System.Xml.Linq;

namespace WesternSpace.TilingEngine
{
    // TileMaps are merely the *data* of a grid of tiles.
    // It knows nothing about how it should be drawn.
    public class TileMap : IXElementOutput
    {
        public readonly int tileWidth;
        public readonly int tileHeight;

        private int layerCount;
        private int subLayerCount;

        ITextureService textureService;

        public int LayerCount
        {
            get { return layerCount; }
        }

        public int SubLayerCount
        {
            get { return subLayerCount; }
        }

        public int Width 
        { 
            get { return tiles.GetLength(0); }
        }

        public int Height 
        {
            get { return tiles.GetLength(1); }
        }

        private Tile[,] tiles;

        public Tile[,] Tiles
        {
            get { return tiles; }
        }

        public TileMap(int cellX, int cellY, int tileWidth, int tileHeight, int layerCount, int subLayerCount)
        {
            this.layerCount = layerCount;
            this.subLayerCount = subLayerCount;
            tiles = new Tile[cellX,cellY];
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;
        }

        public void SetTile(Tile tile, int x, int y)
        {
            tiles[x,y] = tile;
        }

        public TileMap(string fileName)
        { 
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);
            int width, height;

            Int32.TryParse(fileContents.Root.Attribute("Width").Value, out width);
            Int32.TryParse(fileContents.Root.Attribute("Height").Value, out height);

            Int32.TryParse(fileContents.Root.Attribute("TileWidth").Value, out tileWidth);
            Int32.TryParse(fileContents.Root.Attribute("TileHeight").Value, out tileHeight);

            Int32.TryParse(fileContents.Root.Attribute("LayerCount").Value, out layerCount);
            Int32.TryParse(fileContents.Root.Attribute("SubLayerCount").Value, out subLayerCount);

            tiles = new Tile[width, height];

            textureService = (ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService));

            IEnumerable<XElement> allTileElements = fileContents.Descendants("T");

            int i = 0, j = 0;
            
            // This is kind of dangerous, here. We assume that the XML is properly formatted.
            //  If it is not, our program will crash. This is not a huge deal
            foreach (XElement tileElement in allTileElements)
            {
                tiles[i, j] = TileFromXElement(tileElement);

                if (j < height - 1)
                {
                    ++j;
                }
                else
                {
                    ++i;
                    j = 0;
                }
            }
        }

        #region XML Helper Methods

        private XElement TileToXElement(Tile tile)
        {
            // TODO: - add Type attribute (Solid, Empty, or Custom) 
            //         and Top/Bottom/Left/RighEdge attributes when Type="Custom"
            //       - optimize for space/time so saving/loading isn't too slow.
            //         perhaps return a binary string (base64 or the like)
            XElement returnVal = new XElement("T");

            foreach (Texture2D texture in tile.Textures)
            {
                int power = 1;
                int edges = 0;
                foreach (bool edge in tile.Edges)
                {
                    if (edge)
                    {
                        edges += (int)Math.Pow(2, power);
                    }
                    ++power;
                }

                // Note: Texture2D.Name is useless normally, but the TextureService will
                //       automatically set the Name parameter to be the asset name so they can
                //       easily be retrieved.
                returnVal.Add(new XElement("x", new XAttribute("n", texture.Name), new XAttribute("e", edges)));
            }
            return returnVal;
        }

        private Tile TileFromXElement(XElement xelement)
        {
            IEnumerable<Texture2D> textures = from element in xelement.Descendants("x") 
                                   select textureService.GetTexture(element.Attribute("n").Value);

            Texture2D[,] texturesArray = new Texture2D[layerCount, subLayerCount];

            int i = 0, j = 0;

            foreach( Texture2D texture in textures)
            {
                texturesArray[i, j] = texture;
                if (j < subLayerCount - 1)
                { 
                    ++j; 
                }
                else 
                {
                    ++i;
                    j = 0;
                }
            }
            // TODO: ADD REAL EDGE SUPPORT LOL
            bool[] edges = new bool[4];

            edges[0] = edges[1] = edges[2] = edges[3] = true;
            return new Tile(texturesArray, edges);
        }

        #endregion


        #region IXElementOutput Members

        public System.Xml.Linq.XElement ToXElement()
        {
            XElement returnValue = new XElement("TileMap", 
                                                new XAttribute("Width", Width),
                                                new XAttribute("Height", Height),
                                                new XAttribute("TileWidth", tileWidth),
                                                new XAttribute("TileHeight", tileHeight),
                                                new XAttribute("LayerCount", layerCount),
                                                new XAttribute("SubLayerCount", subLayerCount)
                                                );
            foreach (Tile tile in tiles)
            {
                returnValue.Add(TileToXElement(tile));
            }

            return returnValue;
        }

        #endregion
    }
}
