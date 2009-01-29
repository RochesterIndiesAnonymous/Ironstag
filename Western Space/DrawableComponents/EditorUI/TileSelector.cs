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
    // Allows you to "select" any number of tiles on the active tilemap (in the world)
    //  and modify their properties. *shock and awe!*
    public class TileSelector : EditorUIComponent
    {
        #region TilePropertyComponents members

        List<ITilePropertyComponent> tilePropertyComponents;

        internal List<ITilePropertyComponent> TilePropertyComponents
        {
            get { return tilePropertyComponents; }
        }

        private List<SubTextureSelector> subTextureSelectors;

        public List<SubTextureSelector> SubTextureSelectors
        {
            get { return subTextureSelectors; }
        }

        private EdgeToggler edgeToggler;

        public EdgeToggler EdgeToggler
        {
            get { return edgeToggler; }
        }

        #endregion

        #region TileMap related members

        private TileMapLayer tileMapLayer;

        /// <summary>
        /// Currently, a hack to deal with camera coordinates and the likes until that's properly working.
        /// </summary>
        private TileMapLayer TileMapLayer
        {
            get { return tileMapLayer; }
        }

        // Not until we have proper camera motion can we use this:
        //private TileMap tileMap;

        public TileMap TileMap
        {
            get { return tileMapLayer.TileMap; }
        }

        #endregion

        #region Tile Selection related memebers

        /// <summary>
        /// The list of tile coordinates that we're currently selecting, assuming
        ///  that we're in the process of selecting tiles.
        /// </summary>
        private List<int[]> selectingTileCoordinates;

        private List<int[]> selectedTileCoordinates;

        private List<int[]> SelectedTileCoordinates
        {
            get { return selectedTileCoordinates; }
            set 
            {
                selectedTileCoordinates = value;
                foreach (ITilePropertyComponent tilePropertyComponent in tilePropertyComponents)
                {
                    tilePropertyComponent.OnTileSelectionChange();
                }
            }
        }
        

        // Somewhat slow, I know. But for now it works:
        public List<Tile> SelectedTiles
        {
            get 
            {
                return (from coords in selectedTileCoordinates
                        select TileMap.Tiles[coords[0], coords[1]]).ToList<Tile>();
            }
        }

        private void notifyTilePropertyComponents()
        {
            foreach (ITilePropertyComponent tilePropertyComponent in tilePropertyComponents)
            {
                tilePropertyComponent.OnTileSelectionChange();
            }
        }

        // The tile where the user started to select.
        private int[] startingSelectionCoords;

        // The tile where the user's mouse was last over while selecting.
        private int[] previousSelectionCoords;

        // The button the user pressed to start their selection drag.
        // Set to -1 when not selecting.
        private int selectButton;

        #endregion

        #region TILE SELECTION MODIFIERS

        /// <summary>
        /// Select some tiles, clearing our previously selected ones.
        /// </summary>
        /// <param name="coordinateList">
        /// A list of 2-element integer arrays;
        ///     int[0] - tilemap X coord.
        ///     int[1] - tilemap Y coord.
        /// </param>
        public void SelectTiles(List<int[]> coordinateList)
        {
            selectedTileCoordinates.Clear();
            AppendTiles(coordinateList);
        }

        /// <summary>
        /// Append some tiles to our list of selected tiles.
        /// </summary>
        /// <param name="coordinateList">
        /// A list of 2-element integer arrays;
        ///     int[0] - tilemap X coord.
        ///     int[1] - tilemap Y coord.
        /// </param>
        public void AppendTiles(List<int[]> coordinateList)
        {
            foreach (int[] tileMapCoords in coordinateList)
            {
                if (!selectedTileCoordinates.Contains(tileMapCoords))
                {
                    selectedTileCoordinates.Add(tileMapCoords);
                }
            }
            notifyTilePropertyComponents();
        }

        /// <summary>
        /// Invert the selection of our tiles over a list that's given.
        /// Remove any that are in both lists, keep/add any that are only
        ///  in one list. (the other list being selectedTileCoordinates)
        /// </summary>
        /// <param name="coordinateList">
        /// A list of 2-element integer arrays;
        ///     int[0] - tilemap X coord.
        ///     int[1] - tilemap Y coord.
        /// </param>
        public void InvertTiles(List<int[]> coordinateList)
        {
            // TODO
        }

        /// <summary>
        /// Remove some tiles from our list.
        /// </summary>
        /// <param name="coordinateList">
        /// A list of 2-element integer arrays;
        ///     int[0] - tilemap X coord.
        ///     int[1] - tilemap Y coord.
        /// </param>
        public void DeselectTiles(List<int[]> coordinateList)
        {
            foreach (int[] tileMapCoords in coordinateList)
            {
                if (!selectedTileCoordinates.Contains(tileMapCoords)) // May not be neccessary?
                {
                    selectedTileCoordinates.Remove(tileMapCoords);
                }
            }

            notifyTilePropertyComponents();
        }

        #endregion

        #region TILE PROPERTY MODIFIERS

        /// <summary>
        /// Set Textures[layerIndex, subLayerIndex] of all selected tiles to the SubTexture passed in.
        /// </summary>
        /// <param name="subTexture">The SubTexture to set the tiles to (null will clear the texture).</param>
        /// <param name="layerIndex">The first index into each tile's Textures array; the layer index.</param>
        /// <param name="subLayerIndex">The second index into each tile's Textures array; the sub-layer index.</param>
        public void SetSubTexture(SubTexture subTexture, int layerIndex, int subLayerIndex)
        {
            foreach (int[] tileCoord in selectedTileCoordinates)
            {
                Tile tile = TileMap.Tiles[tileCoord[0], tileCoord[1]];
                if (tile == null)
                {
                    // We have an empty tile...
                    if (subTexture != null)
                    {
                        // We need to make a new one to hold this subTexture
                        SubTexture[,] textures = new SubTexture[TileMap.LayerCount, TileMap.SubLayerCount];
                        textures[layerIndex, subLayerIndex] = subTexture;
                        tile = new Tile(textures); // TODO: set the edges to what the edge toggler says the tile's edges should be?
                    }
                    // Otherwise we do nothing.
                }
                else
                {
                    // We have a non-empty tile, so we simply set it's subTexture directly:
                    tile.Textures[layerIndex, subLayerIndex] = subTexture;
                }

                // Finally, we set our tile so the tileMap's edges are properly updated.
                TileMap.SetTile(tile, tileCoord[0], tileCoord[1]);
            }
            notifyTilePropertyComponents();
        }

        /// <summary>
        /// Set all selected tile's edges to be the list of edges passed in.
        /// A copy will be made so their edges aren't actually linked.
        /// </summary>
        /// <param name="edges"></param>
        public void SetEdges(bool[] edges)
        {
            foreach (int[] tileCoord in selectedTileCoordinates)
            {
                Tile tile = TileMap.Tiles[tileCoord[0], tileCoord[1]];
                if (tile == null)
                {
                    tile = new Tile(new SubTexture[TileMap.LayerCount, TileMap.SubLayerCount], edges);
                }
                else
                {
                    edges.CopyTo(tile.InitialEdges, 0);
                    edges.CopyTo(tile.Edges, 0);
                }
                TileMap.SetTile(tile, tileCoord[0], tileCoord[1]);
            }
            notifyTilePropertyComponents();
        }

        /// <summary>
        /// Remove some tiles from the TileMap.
        /// </summary>
        /// <param name="coordinateList">
        /// A list of 2-element integer arrays;
        ///     int[0] - tilemap X coord.
        ///     int[1] - tilemap Y coord.
        /// </param>
        public void RemoveTiles(List<int[]> coordinateList)
        {
            foreach (int[] tileMapCoords in coordinateList)
            {
                this.TileMap.SetTile(null, tileMapCoords[0], tileMapCoords[1]);
            }
            notifyTilePropertyComponents();
        }
        #endregion

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseClick(int button)
        {
            // We ignore other clicks if we're in the middle of dragging:
            if (selectButton < 0)
            {
                Vector2 pos = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);
                int x = (int)Math.Round((double)(pos.X), 0);
                int y = (int)Math.Round((double)(pos.Y), 0);

                startingSelectionCoords = new int[2] { x, y };
                previousSelectionCoords = new int[2] { x, y };

                selectButton = button;
            }

            base.OnMouseClick(button);
        }

        protected override void WhileMouseHeld(int button)
        {
            if (button == selectButton)
            {
                Vector2 pos = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);
                int x = (int)Math.Round((double)(pos.X), 0);
                int y = (int)Math.Round((double)(pos.Y), 0);

                // Determine whether or not we need to change our selection based on 
                //  whether or not we're over a different tile.
                if (previousSelectionCoords[0] != x || previousSelectionCoords[1] != y
                            || selectingTileCoordinates.Count == 0)
                {
                    int minX, minY, maxX, maxY;
                    minX = (int)Math.Min((decimal)x, (decimal)startingSelectionCoords[0]);
                    minY = (int)Math.Min((decimal)y, (decimal)startingSelectionCoords[1]);
                    maxX = (int)Math.Max((decimal)x, (decimal)startingSelectionCoords[0]);
                    maxY = (int)Math.Max((decimal)y, (decimal)startingSelectionCoords[1]);

                    selectingTileCoordinates.Clear();
                    for (int i = minX; i <= maxX; ++i)
                    {
                        for (int j = minY; j <= maxY; ++j)
                        {
                            selectingTileCoordinates.Add(new int[2] { i, j });
                        }
                    }
                }
                
            }
            base.WhileMouseHeld(button);
        }

        protected override void OnMouseUnclick(int button)
        {
            // We ignore unclicks of buttons that dont match our selectButton.
            if (button == selectButton)
            {
                switch (selectButton)
                { 
                    case 0: // Left mouse unclick. Select the tiles.
                        SelectTiles(selectingTileCoordinates);
                        break;
                    case 1: // Middle mouse unclick.
                        AppendTiles(selectingTileCoordinates);
                        break;
                    case 2: // Right mouse unclick. Remove the tiles.
                        RemoveTiles(selectingTileCoordinates);
                        break;
                }
                selectingTileCoordinates.Clear();
                // Reset our selectButton:
                selectButton = -1;
            }
            base.OnMouseUnclick(button);
        }

        protected override void OnMouseUnClickOutside(int button)
        {
            OnMouseUnclick(button);
            base.OnMouseClickOutside(button);
        }

        #endregion

        public TileSelector(Screen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, TileMapLayer tileMapLayer)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.tileMapLayer = tileMapLayer;
            this.tilePropertyComponents = new List<ITilePropertyComponent>();

            this.selectButton = -1;
            this.selectedTileCoordinates = new List<int[]>();
            this.selectingTileCoordinates = new List<int[]>();

            this.subTextureSelectors = new List<SubTextureSelector>();


            for (int i = 0; i < TileMap.LayerCount; ++i)
            {
                for (int j = 0; j < TileMap.SubLayerCount; ++j)
                {
                    int index = i*TileMap.SubLayerCount + j;

                    SubTextureSelector subTexSel = new SubTextureSelector(ParentScreen, SpriteBatch, this, i, j);
                    this.TilePropertyComponents.Add(subTexSel);
                    ParentScreen.Components.Add(subTexSel);
                    SubTextureSelectors.Add(subTexSel);
                }
            }

            RectangleF tmp = ((SubTextureSelector)TilePropertyComponents.Last<ITilePropertyComponent>()).Bounds;
            tmp.Y += 20 + TileMap.TileHeight;
            this.edgeToggler = new EdgeToggler(ParentScreen, SpriteBatch, tmp, this);
            this.tilePropertyComponents.Add(edgeToggler);
            ParentScreen.Components.Add(this.edgeToggler);
        }

        /// <summary>
        /// This is a particularly long and ugly function. There's a lot of crap to draw :(
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Highlight the tile we're hovering over
            Vector2 position = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);

            position = new Vector2((int)position.X * TileMap.TileWidth, (int)position.Y * TileMap.TileHeight)
                                        - tileMapLayer.Camera.Position * tileMapLayer.ScrollSpeed;

            DrawBoundingRect(position, 2, Microsoft.Xna.Framework.Graphics.Color.White);

            foreach (int[] tileCoord in SelectedTileCoordinates)
            {
               // Highlight the tiles we've selected.
                position = new Vector2(tileCoord[0] * TileMap.TileWidth, tileCoord[1] * TileMap.TileHeight)
                                        - tileMapLayer.Camera.Position * tileMapLayer.ScrollSpeed;

                position.X = (int)position.X;
                position.Y = (int)position.Y;

                DrawBoundingRect(position, new Microsoft.Xna.Framework.Graphics.Color(.0f, 1.0f, .0f,0.9f));
            }

            Microsoft.Xna.Framework.Graphics.Color col;

            switch (selectButton)
            {
                case 0:
                    col = new Microsoft.Xna.Framework.Graphics.Color(0.0f, 1.0f, 0.0f, 0.7f);
                    break;
                case 2:
                    col = new Microsoft.Xna.Framework.Graphics.Color(1.0f, 0.0f, 0.0f, 0.9f);
                    break;
                default:
                    col = new Microsoft.Xna.Framework.Graphics.Color(1.0f, 0.0f, 1.0f, 0.7f);
                    break;
            }
            foreach (int[] tileCoord in selectingTileCoordinates)
            {
                // Highlight the tiles we're selecting.
                position = new Vector2(tileCoord[0] * TileMap.TileWidth, tileCoord[1] * TileMap.TileHeight)
                                        - tileMapLayer.Camera.Position * tileMapLayer.ScrollSpeed;

                position.X = (int)position.X;
                position.Y = (int)position.Y;


                if (selectButton == 2)
                {
                    if (TileMap.Tiles[tileCoord[0], tileCoord[1]] != null)
                    {
                        PrimitiveDrawer.Instance.DrawLine(SpriteBatch,
                                                          new Vector2(position.X, position.Y),
                                                          new Vector2(position.X + TileMap.TileWidth, position.Y + TileMap.TileHeight),
                                                          col);
                        PrimitiveDrawer.Instance.DrawLine(SpriteBatch,
                                                          new Vector2(position.X + TileMap.TileWidth, position.Y),
                                                          new Vector2(position.X, position.Y + TileMap.TileHeight),
                                                          col);
                        DrawBoundingRect(position, col);
                    }
                    else
                    {
                        DrawBoundingRect(position, new Microsoft.Xna.Framework.Graphics.Color(1.0f, 0.0f, 0.0f, 0.5f));
                    }
                }
                else
                {
                    DrawBoundingRect(position, col);
                }
            }

            base.Draw(gameTime);
        }

        #region Draw Helpers

        private void DrawBoundingRect(Vector2 position, Microsoft.Xna.Framework.Graphics.Color color)
        {
            DrawBoundingRect(position, 0, color);
        }

        private void DrawBoundingRect(Vector2 position, int margin, Microsoft.Xna.Framework.Graphics.Color color)
        {
            Microsoft.Xna.Framework.Rectangle rect = new Microsoft.Xna.Framework.Rectangle((int)position.X - margin + 1, (int)position.Y - margin, 
                                                            TileMap.TileWidth + 2*margin - 1, TileMap.TileHeight + 2*margin);

            PrimitiveDrawer.Instance.DrawRect(SpriteBatch, rect, color);
        }

        private void MyDrawRect(Vector2 position, int width, int height, Microsoft.Xna.Framework.Graphics.Color color)
        {
            Texture2D tex = new Texture2D(((IGraphicsDeviceManagerService)Game.Services.GetService(typeof(IGraphicsDeviceManagerService))).GraphicsDevice.GraphicsDevice,
                width, height, 1, TextureUsage.None, SurfaceFormat.Color);
            SpriteBatch.Draw(tex, position, color);
        }

        #endregion
    }
}
