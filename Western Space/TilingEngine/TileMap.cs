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

        /// <summary>
        /// Sets the tile at given x,y.
        /// Automatically removes edges of adjacent tiles.
        /// </summary>
        public void SetTile(Tile tile, int x, int y)
        {
            // Modulo math prevents out-of-bounds errors, and makes for
            //  easy "warping". But if things are behaving somewhat unusual,
            //  (ie. tiles from the other side of the map are appearing) it's
            //  because you're indexes are too high/low.

            

            Tile above = y > 0 ? tiles[x, (y - 1)] : null;
            Tile left = x > 0 ? tiles[(x - 1), y] : null;
            Tile below = y < Height - 1 ? tiles[x, (y + 1)] : null;
            Tile right = x < Width - 1 ? tiles[(x + 1), y] : null;
            
            // We're "removing" a tile, so we restore
            //  adjacent tiles' adjacent edges to their initial values.
            if (tile == null)
            {
                if (above != null)
                    above.BottomEdge = above.InitialBottomEdge;
                if (left != null)
                    left.RightEdge = left.InitialRightEdge;
                if (below != null)
                    below.TopEdge = below.InitialTopEdge;
                if (right != null)
                    right.LeftEdge = right.InitialLeftEdge;
            }
            else
            {
                // Else, we're adding a tile, so we clear all adjacent
                //  edges that we have solid edges for.
                if (above != null && tile.TopEdge && above.BottomEdge)
                {
                    tile.TopEdge = false;
                    tiles[x, (y - 1)].BottomEdge = false;
                }

                if (left != null && tile.LeftEdge && tiles[(x - 1), y].RightEdge)
                {
                    tile.LeftEdge = false;
                    tiles[(x - 1), y].RightEdge = false;
                }

                if (below != null && tile.BottomEdge && tiles[x, (y + 1)].TopEdge)
                {
                    tile.BottomEdge = false;
                    tiles[x, (y + 1)].TopEdge = false;
                }

                if (right != null && tile.RightEdge && tiles[(x + 1), y].LeftEdge)
                {
                    tile.RightEdge = false;
                    tiles[(x + 1), y].TopEdge = false;
                }
            }
            tiles[x, y] = tile;
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
            //  If it is not, our program will crash. This is not a huge deal, but if the debugger
            //  is highlighting a line below, you can blame Lou, unless you tried to hack a TileMap XML file by hand.
            foreach (XElement tileElement in allTileElements)
            {
                this.SetTile(TileFromXElement(tileElement), i, j);

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
            // Empty tiles are represented as having -1 edges. (nonsense)
            if (tile == null)
            {
                return new XElement("T", new XAttribute("e", -1));
            }

            // TODO: - add Type attribute (Solid, Empty, or Custom) 
            //         and Top/Bottom/Left/RighEdge attributes when Type="Custom"
            //       - optimize for space/time so saving/loading isn't too slow.
            //         perhaps return a binary string (base64 or the like)
            int power = 0;
            int edges = 0;

            // We save the InitialEdges and let the TileMap take care of the rest.
            foreach (bool edge in tile.InitialEdges)
            {
                if (edge)
                {
                    edges += (int)Math.Pow(2, power);
                }
                ++power;
            }

            XElement returnVal = new XElement("T", new XAttribute("e", edges));
            foreach (Texture2D texture in tile.Textures)
            {
                // Note: Texture2D.Name is useless normally, but the TextureService will
                //       automatically set the Name parameter to be the asset name so they can
                //       easily be retrieved.
                returnVal.Add(new XElement("x", new XAttribute("n", texture.Name) ));
            }
            return returnVal;
        }

        private Tile TileFromXElement(XElement xelement)
        {
            if (xelement.Attribute("e").Value == "-1")
            {
                return null;
            }

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

            int edgeInt;
            Int32.TryParse(xelement.Attribute("e").Value, out edgeInt);

            bool[] edges = new bool[4];
            for (int k = 0; k < 4; ++k)
            {
                edges[k] = (edgeInt & (int)Math.Pow(2, k)) != 0;
            }

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
