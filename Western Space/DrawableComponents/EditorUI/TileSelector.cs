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
    // Allows you to "click" any number of tiles on the active tilemap (in the world)
    //  and modify their properties.
    public class TileSelector : EditorUIComponent
    {
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

        private TileMapLayer tileMapLayer;

        /// <summary>
        /// Currently, a hack to deal with camera coordinates and the likes until that's properly working.
        /// </summary>
        private TileMapLayer TileMapLayer
        {
            get { return tileMapLayer; }
        }

        /// <summary>
        /// The list of tile coordinates that we're currently highlighting, assuming
        ///  that we're in the process of highlighting tiles.
        /// </summary>
        private List<int[]> highlightedTileCoordinates;

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

        // Not until we have proper camera motion can we use this:
        //private TileMap tileMap;

        public TileMap TileMap
        {
            get { return tileMapLayer.TileMap; }
        }

        /// <summary>
        /// Append some tiles to our list of selected tiles.
        /// </summary>
        /// <param name="coordinateList">
        /// A list of 2-element integer arrays;
        ///     int[0] - tilemap X coord.
        ///     int[1] - tilemap Y coord.
        /// </param>
        public void SelectTiles(List<int[]> coordinateList)
        {
            // TODO: Make sure null tiles are turned into new empty tiles before
            //        they are added to our list, so their properties can change.
            foreach (int[] tileMapCoords in coordinateList)
            {
                if ( tileMapCoords[0] >= 0 &&
                     tileMapCoords[1] >= 0 &&
                     tileMapCoords[0] < TileMap.Width &&
                     tileMapCoords[1] < TileMap.Height &&
                    !selectedTileCoordinates.Contains(tileMapCoords))
                {
                    selectedTileCoordinates.Add(tileMapCoords);
                }
            }

            foreach (ITilePropertyComponent tilePropertyComponent in tilePropertyComponents)
            {
                tilePropertyComponent.OnTileSelectionChange();
            }
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
            // TODO: 
            //       Also, when they're deselected, make sure they're turned back into
            //        null.
            foreach (int[] tileMapCoords in coordinateList)
            {
                if (!selectedTileCoordinates.Contains(tileMapCoords)) // May not be neccessary?
                {
                    selectedTileCoordinates.Remove(tileMapCoords);
                }
            }

            foreach (ITilePropertyComponent tilePropertyComponent in tilePropertyComponents)
            {
                tilePropertyComponent.OnTileSelectionChange();
            }
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseClick(int button)
        {
            Vector2 pos = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);
            int x = (int)Math.Round((double)(pos.X), 0);
            int y = (int)Math.Round((double)(pos.Y), 0);

            List<int[]> tileCoords = new List<int[]>();
            tileCoords.Add(new int[] { x, y });

            if (button == 0) // Left button clicked. Select/Add a tile
            {
                SelectedTileCoordinates = tileCoords;
            }
            else if (button == 1) // Middle button clicked. Toggle a tile's edges
            {
                // TODO!
            }
            else if (button == 2) // Right button clicked. Remove a tile
            {
            }

            base.OnMouseClick(button);
        }

        protected override void OnMouseUnclick(int button)
        {
            base.OnMouseUnclick(button);
        }

        #endregion

        public void SetSubTexture(SubTexture subTexture, int layerIndex, int subLayerIndex)
        {
            foreach (int[] tileCoord in selectedTileCoordinates)
            {
                Tile tile = TileMap.Tiles[tileCoord[0], tileCoord[1]];
                if (subTexture != null && tile == null)
                {
                    SubTexture[,] textures = new SubTexture[TileMap.LayerCount, TileMap.SubLayerCount];
                    textures[layerIndex, subLayerIndex] = subTexture;
                    tile = new Tile(textures); // TODO: set the edges to what the edge toggler says the tile's edges should be!
                    TileMap.SetTile(tile, tileCoord[0], tileCoord[1]);
                }
                else
                {
                    tile.Textures[layerIndex, subLayerIndex] = subTexture;
                }
            }
        }
        
        public TileSelector(Screen parentScreen, SpriteBatch spriteBatch, RectangleF bounds, TileMapLayer tileMapLayer)
            : base(parentScreen, spriteBatch, bounds)
        {
            this.tileMapLayer = tileMapLayer;
            this.tilePropertyComponents = new List<ITilePropertyComponent>();
            this.selectedTileCoordinates = new List<int[]>();
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
            ParentScreen.Components.Add(this.edgeToggler);
        }

        public override void Initialize()
        {
            this.Color = Microsoft.Xna.Framework.Graphics.Color.White;
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            // Highlight the tile we're hovering over
            Vector2 position = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);

            position = new Vector2((int)position.X * TileMap.TileWidth, (int)position.Y * TileMap.TileHeight)
                                        - tileMapLayer.Camera.Position * tileMapLayer.ScrollSpeed;

            DrawBoundingRect(position, 2, Microsoft.Xna.Framework.Graphics.Color.White);

            foreach (int[] tileCoord in SelectedTileCoordinates)
            {
               // Highlight the tile we've selected.
                position = new Vector2(tileCoord[0] * TileMap.TileWidth, tileCoord[1] * TileMap.TileHeight)
                                        - tileMapLayer.Camera.Position * tileMapLayer.ScrollSpeed;

                position.X = (int)position.X;
                position.Y = (int)position.Y;

                DrawBoundingRect(position, new Microsoft.Xna.Framework.Graphics.Color(new Vector3(0, 255, 0)));
            }
            base.Draw(gameTime);
        }

        #region Draw Helpers
        private void DrawBoundingRect(Vector2 position, Microsoft.Xna.Framework.Graphics.Color color)
        {
            DrawBoundingRect(position, 1, color);
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
