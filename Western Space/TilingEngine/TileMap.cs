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
        private int tileWidth;

        public int TileWidth
        {
            get { return tileWidth; }
            set { tileWidth = value; }
        }
        private int tileHeight;

        public int TileHeight
        {
            get { return tileHeight; }
            set { tileHeight = value; }
        }

        private int layerCount;
        private int subLayerCount;

        private ITextureService textureService;

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

        public Tile this[int x, int y]
        {
            get 
            {
                if (x < Width && x >= 0 &&
                    y < Height && y >= 0)
                    return tiles[x, y];
                else
                    return null;
            }
            set
            {
                if (x < Width && x >= 0 &&
                    y < Height && y >= 0)
                    SetTile(value, x, y);
            }
        }

        /// <summary>
        /// Crop this tilemap down to the minimal size needed to contain all non-null tiles in it.
        /// </summary>
        /// <returns>A Rectangle which represents the cropping area, in tile-coordinates.</returns>
        public Rectangle Minimize()
        {
            int xMin, xMax, yMin, yMax;
            xMax = yMax = 0;
            xMin = Width - 1;
            yMin = Height - 1;

            // Find the minimal and maximal tiles that aren't null:
            for (int x = 0; x < Width; ++x)
            {
                for (int y = 0; y < Height; ++y)
                {
                    if (tiles[x, y] != null)
                    {
                        if (x > xMax)
                            xMax = x;
                        if (x < xMin)
                            xMin = x;
                        if (y > yMax)
                            yMax = y;
                        if (y < yMin)
                            yMin = y;
                    }
                }
            }

            // Create a new tilemap that will fit all of them:
            Tile[,] newTiles = new Tile[xMax - xMin+1, yMax - yMin+1];

            // And copy all tiles into the new array:
            //  NOTE: SetTile doesn't need to be used here because each tile's
            //         edge data should be the same since we're only cropping
            //         out nulls, which have no effect on edges.
            for (int xOffset = 0; xOffset < newTiles.GetLength(0); ++xOffset)
            {
                for (int yOffset = 0; yOffset < newTiles.GetLength(1); ++yOffset)
                {
                    newTiles[xOffset, yOffset] = this[xMin + xOffset, yMin + yOffset];
                }
            }
            tiles = newTiles;

            return new Rectangle(xMin, yMin, Width, Height);
        }

        // This is mainly just used for the editor when creating new empty tilemaps.
        public TileMap(int cellX, int cellY, int tileWidth, int tileHeight, int layerCount, int subLayerCount)
        {
            this.layerCount = layerCount;
            this.subLayerCount = subLayerCount;
            this.tiles = new Tile[cellX,cellY];
            this.tileWidth = tileWidth;
            this.tileHeight = tileHeight;

            this.textureService = (ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService));
        }

        public void SetSolid(bool solidOrNot, int x, int y)
        {
            if (tiles[x, y] != null)
            {
                tiles[x, y].SetSolid(solidOrNot);
            }
            else
            {
                tiles[x, y] = new Tile(layerCount, subLayerCount);
            }
            SetTile(tiles[x, y], x, y);
        }

        /// <summary>
        /// "Blit" another TileMap onto this one, replacing all tiles with those from 
        ///  the other TileMap, even if they are null.
        /// Note that each tile is *copied*. They are not references to the same tiles.
        /// 
        /// That's right, "Blit"... this is the closest thing to actual 2D
        ///  graphics programming I've encountered while writing this game. ;)
        /// </summary>
        /// <param name="other">The TileMap to stamp onto ours.</param>
        /// <param name="xOffset">
        /// The x component of the coordinate in this TileMap where the other TileMap
        ///  will be stamped (topleft corner).
        /// </param>
        /// <param name="yOffset">
        /// The y component of the coordinate in this TileMap where the other TileMap
        ///  will be stamped (topleft corner).
        /// </param>
        public void BlitTileMap(TileMap other, int xOffset, int yOffset) 
        {
            int startY = yOffset < 0 ? -yOffset : 0;
            Tile tile;

            for (int i = xOffset < 0 ? -xOffset : 0; i < other.Width && i + xOffset < Width; ++i)
            {
                for (int j = startY; j < other.Height && j + yOffset < Height; ++j)
                { 
                    tile = other[i,j];
                    if(tile == null)
                    {
                        SetTile(null, i + xOffset, j + yOffset);
                    }
                    else
                    {
                        SetTile(new Tile(other[i, j]), i + xOffset, j + yOffset);
                    }
                }
            }
        }

        /// <summary>
        /// Given a list of tile coordinates, create a new TileMap from this
        ///  TileMap that contains copies of each Tile that corresponds to every
        ///  coordinate in the list.
        /// </summary>
        /// <param name="tileCoordinates">
        /// A List of 2D coordinate sets, each represented as a 2-element array of integers.
        /// Format: 
        ///  int[0] => Tile X coordinate.
        ///  int[1] => Tile Y coordinate.
        /// This List represents each Tile you want to copy into the new TileMap that will be
        ///  returned from this function.
        /// </param>
        /// <returns>
        /// The resulting TileMap will be just big enough to fit all Tiles
        ///  specified by the tileCoordinates List, and all in-between Tiles
        ///  that aren't specified in the list will be null.
        /// </returns>
        public TileMap SubTileMapFromCoordList(List<int[]> tileCoordinates)
        {
            TileMap subTileMap;
            if (tileCoordinates.Count > 0)
            {
                // First, compute the minimum required width and height:
                int minX, minY, maxX, maxY, w, h;
                minX = minY = 999999999;
                maxX = maxY = 0;

                foreach (int[] tileCoord in tileCoordinates)
                {
                    minX = Math.Min(tileCoord[0], minX);
                    minY = Math.Min(tileCoord[1], minY);

                    maxX = Math.Max(tileCoord[0], maxX);
                    maxY = Math.Max(tileCoord[1], maxY);
                }
                w = maxX - minX + 1;
                h = maxY - minY + 1;

                subTileMap = new TileMap(w, h, TileWidth, TileHeight, LayerCount, SubLayerCount);
                foreach (int[] tileCoord in tileCoordinates)
                {
                    Tile tile = this[tileCoord[0], tileCoord[1]];
                    if (tile != null)
                    {
                        subTileMap.SetTile(tile, tileCoord[0] - minX, tileCoord[1] - minY);
                    }
                }
            }
            else
            {
                subTileMap = new TileMap(0, 0, TileWidth, TileHeight, LayerCount, SubLayerCount);
            }

            return subTileMap;
        }

        /// <summary>
        /// Sets the tile at given x,y.
        /// Automatically removes edges of adjacent tiles.
        /// </summary>
        public void SetTile(Tile tile, int x, int y)
        {
            // Prevent useless tiles from being added:
            if (tile != null && tile.IsEmpty())
                tile = null;

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
                // Special edge cases (no pun intended):
                if (x == 0) // left edge of the map
                {
                    tile.LeftEdge = false;
                }
                else if (x == (Width - 1)) // right edge of the map
                {
                    tile.RightEdge = false;
                }

                if (y == 0) // top edge of the map
                {
                    tile.TopEdge = false;
                }
                else if (y == (Height - 1)) // bottom edge of the map
                {
                    tile.BottomEdge = false;
                }

                // Else, we're adding a tile, so we clear all adjacent
                //  edges that we have solid edges for.
                if (above != null)
                {
                    if (tile.InitialTopEdge && above.InitialBottomEdge)
                    {
                        tile.TopEdge = false;
                        above.BottomEdge = false;
                    }
                    else
                    {
                        above.BottomEdge = above.InitialBottomEdge;
                    }
                }

                if (left != null)
                {
                    if (tile.InitialLeftEdge && left.InitialRightEdge)
                    {
                        tile.LeftEdge = false;
                        left.RightEdge = false;
                    }
                    else
                    {
                        left.RightEdge = left.InitialRightEdge;
                    }
                }

                if (below != null)
                {
                    if (tile.InitialBottomEdge && below.InitialTopEdge)
                    {
                        tile.BottomEdge = false;
                        below.TopEdge = false;
                    }
                    else
                    {
                        below.TopEdge = below.InitialTopEdge;
                    }
                }

                if (right != null)
                {
                    if (tile.InitialRightEdge && right.InitialLeftEdge)
                    {
                        tile.RightEdge = false;
                        right.LeftEdge = false;
                    }
                    else
                    {
                        right.LeftEdge = right.InitialLeftEdge;
                    }
                }

            }
            tiles[x, y] = tile;
        }

        public void RemoveTile(int x, int y)
        {
            SetTile(null, x, y);
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

            IEnumerable<XElement> allSheets = fileContents.Descendants("sh");

            SubTextureSheet[] sheets = new SubTextureSheet[allSheets.Count<XElement>()];
            Dictionary<string, int> sheetIndex = new Dictionary<string, int>();

            int i;
            foreach (XElement sheet in allSheets)
            {
                Int32.TryParse(sheet.Attribute("i").Value, out i);
                string sheetFileName = sheet.Attribute("fn").Value;
                sheets[i] = textureService.GetSheet(sheetFileName);
                sheetIndex.Add(sheetFileName, i);
            }

            IEnumerable<XElement> allTileElements = fileContents.Descendants("T");

            // This is kind of dangerous, here. We assume that the XML is properly formatted.
            //  If it is not, our program will crash. This is not a huge deal, but if the debugger
            //  is highlighting a line below, you can blame Lou, unless you tried to hack a TileMap XML file by hand.
            int j = 0;
            i = 0;
            foreach (XElement tileElement in allTileElements)
            {
                this.SetTile(TileFromXElement(tileElement, sheets), i, j);

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

        private XElement TileToXElement(Tile tile, Dictionary<string, int> sheetIndex)
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
                    edges |= (int)Math.Pow(2, power);
                }
                ++power;
            }

            XElement returnVal = new XElement("T", new XAttribute("e", edges));

            int z = 0; // The index into the Tile.Textures array where this SubTexture belongs.
            foreach (SubTexture sub in tile.Textures)
            {
                // Note: Texture2D.Name is useless normally, but the TextureService will
                //       automatically set the Name parameter to be the asset name so they can
                //       easily be retrieved.
                // "z" attribute: the index into the tile's Textures[,] array. Converted to single int to save space.
                // "s" attribute: the SubTextureSheet number to use. Each tilemap keeps a list of these.
                // "i" attribute: the index into the SubTextureSheet representing a SubTexture of this tile.
                if (sub != null)
                {
                    returnVal.Add(new XElement("x", new XAttribute("z", z), new XAttribute("s", sheetIndex[sub.Sheet.Name]), new XAttribute("i", sub.Index)));
                }
                ++z;
            }
            return returnVal;
        }

        private Tile TileFromXElement(XElement xelement, SubTextureSheet[] sheets)
        {
            if (xelement.Attribute("e").Value == "-1")
            {
                return null;
            }

            SubTexture[,] subTexturesArray = new SubTexture[layerCount, subLayerCount];

            IEnumerable<XElement> subTextureElements = xelement.Descendants("x");


            int i = 0, j = 0;
            //int index = 0; // Was needed when changing format.
            foreach( XElement subTextureElement in subTextureElements)
            {
                i = Int32.Parse(subTextureElement.Attribute("z").Value) / SubLayerCount;
                j = Int32.Parse(subTextureElement.Attribute("z").Value) % SubLayerCount;
                SubTexture subTex = new SubTexture(sheets[Int32.Parse(subTextureElement.Attribute("s").Value)], Int32.Parse(subTextureElement.Attribute("i").Value));
                subTexturesArray[i, j] = subTex;
            }

            int edgeInt;
            Int32.TryParse(xelement.Attribute("e").Value, out edgeInt);

            bool[] edges = new bool[4];
            for (int k = 0; k < 4; ++k)
            {
                edges[k] = (edgeInt & (int)Math.Pow(2, k)) != 0;
            }

            return new Tile(subTexturesArray, edges);
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

            XElement sheetIndexElement = new XElement("SheetIndex");
            returnValue.Add(sheetIndexElement);

            Dictionary<string, int> sheetIndex = new Dictionary<string, int>();
            foreach (Tile tile in tiles)
            {
                if (tile != null)
                {
                    foreach (SubTexture subTexture in tile.Textures)
                    {
                        if (subTexture != null && !sheetIndex.Keys.Contains<string>(subTexture.Sheet.Name))
                        {
                            sheetIndex.Add(subTexture.Sheet.Name, sheetIndex.Count);
                        }
                    }
                }
                returnValue.Add(TileToXElement(tile, sheetIndex));
            }

            SubTextureSheet[] sheets = new SubTextureSheet[sheetIndex.Count];
            foreach (string sheetName in sheetIndex.Keys)
            {
                sheets[sheetIndex[sheetName]] = textureService.GetSheet(sheetName);
            }

            foreach (SubTextureSheet sheet in sheets)
            { 
                sheetIndexElement.Add(new XElement("sh", new XAttribute("fn", sheet.Name), new XAttribute("i", sheetIndex[sheet.Name])));
            }

            return returnValue;
        }

        #endregion
    }
}
