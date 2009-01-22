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

        private bool[] initialEdges;
        private bool[] edges;


        // InitialEdges are the original edge data of the tile, before
        //  the tilemap removed adjacent edges from it. This is so we can
        //  restore it's state when removing an adjacent tile.
        public bool[] InitialEdges
        {
            get { return initialEdges; }
        }

        public bool InitialTopEdge
        {
            get { return initialEdges[0]; }
        }

        public bool InitialLeftEdge
        {
            get { return initialEdges[1]; }
        }

        public bool InitialBottomEdge
        {
            get { return initialEdges[2]; }
        }

        public bool InitialRightEdge
        {
            get { return initialEdges[3]; }
        }

        public bool[] Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        public bool TopEdge
        {
            get { return edges[0]; }
            set { edges[0] = value; }
        }

        public bool LeftEdge
        {
            get { return edges[1]; }
            set { edges[1] = value; }
        }

        public bool BottomEdge
        {
            get { return edges[2]; }
            set { edges[2] = value; }
        }

        public bool RightEdge
        {
            get { return edges[3]; }
            set { edges[3] = value; }
        }

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
            this.initialEdges = new bool[4];
            this.initialEdges[0] = this.initialEdges[1] =
                this.initialEdges[2] = this.initialEdges[3] = true;
            this.edges = new bool[4];
            this.initialEdges.CopyTo(this.edges, 0);
            this.textures = textures;
        }

        public Tile(Texture2D[,] textures, bool[] edges)
        {
            this.initialEdges = new bool[4];
            edges.CopyTo(initialEdges, 0);
            this.edges = new bool[4];
            this.initialEdges.CopyTo(this.edges, 0);
            this.textures = textures;
        }
    }
}
