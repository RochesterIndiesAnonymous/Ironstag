﻿using System;
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
            bool[] edges = new bool[4];

            edges[0] = edges[1] = edges[2] = edges[3] = true;
            this.edges = edges;
            this.initialEdges = this.edges;
            this.textures = textures;
        }

        public Tile(Texture2D[,] textures, bool[] edges)
        {
            this.edges = edges;
            this.initialEdges = this.edges;
            this.textures = textures;
        }
    }
}
