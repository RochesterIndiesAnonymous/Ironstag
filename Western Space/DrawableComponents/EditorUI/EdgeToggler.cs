using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.TilingEngine;
using WesternSpace.Utility;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.EditorUI
{
    /// <summary>
    /// A component who's main purpose is to toggle all edges of a tile.
    /// Doubles as a preview for the final version of a tile based on what
    ///  the SubTextureSelectors' SubTextures are.
    /// </summary>
    public class EdgeToggler : EditorUIComponent, ITilePropertyComponent
    {
        private TileSelector tileSelector;

        private ITextureService textureService;

        public EdgeToggler(Screen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, TileSelector tileSelector)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.tileSelector = tileSelector;
            this.textureService = (ITextureService)Game.Services.GetService(typeof(ITextureService));

            // By default, solid:
            this.edges = new bool[4];
            edges[0] = edges[1] = edges[2] = edges[3] = true;
            edgesConflict = false;
        }

        /// <summary>
        /// The edges this edgetoggler represents
        /// </summary>
        private bool[] edges;

        public bool[] Edges
        {
            get { return edges; }
        }

        private bool edgesConflict;

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseUnclick(int button)
        {
            if (button == 0) // Only handle left clicks, ignore others.
            {
                for (int i = 0; i < 4; ++i)
                {
                    edges[i] = !edges[i];
                }
                tileSelector.SetEdges(edges);
            }
            base.OnMouseUnclick(button);
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            bool mouseInside = MouseIsInside();

            // For now we just pretend tiles can be either solid or not, no in-between:
            if (edgesConflict)
            {
                Color = new Microsoft.Xna.Framework.Graphics.Color(255, 0, 255);
            }
            else if (edges[0] || edges[1] || edges[2] || edges[3])
            {
                if (mouseInside)
                {
                    Color = new Microsoft.Xna.Framework.Graphics.Color(255, 127, 127);
                }
                else
                {
                    Color = new Microsoft.Xna.Framework.Graphics.Color(255, 0, 0);
                }
            }
            else
            {
                Color = new Microsoft.Xna.Framework.Graphics.Color(1.0f, 1.0f, 1.0f, (mouseInside ? 0.8f : 0.5f));
            }

            base.Draw(gameTime);
        }

        #region ITilePropertyComponent Members

        
        public void OnTileSelectionChange()
        {
            List<Tile> selectedTiles = tileSelector.SelectedTiles;
            if (selectedTiles.Count > 0)
            {
                IEnumerable<int> edgesInts = (from tile in selectedTiles
                                              where tile != null
                                              select tile.InitialEdgesInt);

                foreach (int edgesInt in edgesInts)
                {
                    foreach (int otherEdgesInt in edgesInts)
                        if (edgesInt != otherEdgesInt)
                        {
                            // If we run into a mismatch, we set edgesConflict to true.
                            edgesConflict = true;
                            return;
                        }
                }

                edgesConflict = false;
                // If we made it this far, then they all matched up. Copy the first tile's edges
                //  into ours.
                Tile first = selectedTiles.First<Tile>();
                if (first != null)
                {
                    first.InitialEdges.CopyTo(this.edges, 0);
                }
                else
                {
                    edges[0] = edges[1] = edges[2] = edges[3] = false;
                }
            }
        }

        #endregion
    }
}
