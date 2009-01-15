using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;
using WesternSpace.Interfaces;

namespace WesternSpace.TilingEngine
{
    // TileMaps are merely the *data* of a grid of tiles.
    // It knows nothing about how it should be drawn.
    public class TileMap
    {
        public readonly int gridCellHeight;
        public readonly int gridCellWidth;

        private int layerCount;
        private int subLayerCount;

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
            gridCellWidth = tileWidth;
            gridCellHeight = tileHeight;
        }

        public void SetTile(Tile tile, int x, int y)
        {
            tiles[x,y] = tile;
        }

    }
}
