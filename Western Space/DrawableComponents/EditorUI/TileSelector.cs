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

namespace WesternSpace.DrawableComponents.EditorUI
{
    // Allows you to "click" a tile on the active tilemap (in the world)
    //  to select it and edit it's properties.
    public class TileSelector : EditorUIComponent
    {
        private EdgeToggler edgeToggler;

        public EdgeToggler EdgeToggler
        {
            get { return edgeToggler; }
        }

        private SaveButton saveButton;

        public SaveButton SaveButton
        {
            get { return saveButton; }
        }

        private SubTextureSelector[] subTextureSelectors;

        public SubTextureSelector[] SubTextureSelectors
        {
            get { return subTextureSelectors; }
        }

        private TileMapLayer tileMapLayer;

        public TileMapLayer TileMapLayer
        {
            get { return tileMapLayer; }
        }

        public TileMap TileMap
        {
            get { return tileMapLayer.TileMap; }
        }

        private Tile virtualTile;

        public Tile VirtualTile
        {
            get { return virtualTile; }
            set { virtualTile = value; }
        }

        public Tile Tile
        {
            get 
            {
                //return virtualTile;
                if(tileX < 0 || tileY < 0)
                {
                    return virtualTile;
                }
                else
                {
                    return TileMap.Tiles[tileX, tileY];
                }
            }
        }

        private int tileX;

        public int TileX
        {
            get { return tileX; }
        }

        private int tileY;

        public int TileY
        {
            get { return tileY; }
        }

        public void SelectTile(int x, int y)
        {
            tileX = x; 
            tileY = y;
            if (TileMap.Tiles[x, y] != null)
            {
                virtualTile = TileMap.Tiles[x, y];
            }
        }

        #region MOUSE EVENT HANDLERS

        protected override void OnMouseClick(int button)
        {

            Vector2 pos = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);
            int x = (int)Math.Round((double)(pos.X), 0);
            int y = (int)Math.Round((double)(pos.Y), 0);
            if (button == 0) // Left button clicked. Select/Add a tile
            {
                SelectTile(x, y);
                if (TileMap.Tiles[x, y] == null)
                {
                    TileMap.SetTile(new Tile(virtualTile), x, y);
                }
                SelectTile(x, y);
            }
            else if (button == 1) // Middle button clicked. Toggle a tile's edges
            {
                Tile t = TileMap.Tiles[x, y];
                if (t != null)
                {
                    bool solid = false;
                    foreach (bool edge in t.InitialEdges)
                    {
                        if (edge)
                        {
                            solid = true;
                            break;
                        }
                    }
                    TileMap.SetSolid(!solid, x, y);
                    tileX = tileY = -1;
                }
                else
                {
                    TileMap.SetTile(new Tile(virtualTile.LayerCount, virtualTile.SubLayerCount), x, y);
                }
            }
            else if (button == 2) // Right button clicked. Remove a tile
            {
                SelectTile(x, y);
                TileMap.RemoveTile(x, y);
                tileX = tileY = -1;
            }

            base.OnMouseClick(button);
        }

        #endregion

        public TileSelector(Game game, SpriteBatch spriteBatch, RectangleF bounds, TileMapLayer tileMapLayer)
            : base(game, spriteBatch, bounds)
        {
            this.tileX = this.tileY = -1; // No tile selected.

            this.tileMapLayer = tileMapLayer;

            this.virtualTile = new Tile(TileMap.LayerCount, TileMap.SubLayerCount);

            this.subTextureSelectors = new SubTextureSelector[TileMap.SubLayerCount * TileMap.LayerCount];

            for (int i = 0; i < TileMap.LayerCount; ++i)
            {
                for (int j = 0; j < TileMap.SubLayerCount; ++j)
                {
                    int index = i*TileMap.SubLayerCount + j;

                    SubTextureSelector subTexSel = new SubTextureSelector(Game, SpriteBatch, this, i, j);
                    Game.Components.Add(subTexSel);
                    this.subTextureSelectors[index] = subTexSel;
                }
            }

            RectangleF tmp = SubTextureSelectors.Last<SubTextureSelector>().Bounds;
            tmp.Y += 20 + TileMap.tileHeight;
            this.edgeToggler = new EdgeToggler(Game, SpriteBatch, tmp, this);
            Game.Components.Add(this.edgeToggler);
            
            tmp.Y += 20 + TileMap.tileHeight;
            tmp.X -= 10;
            tmp.Width = 30;
            tmp.Height = 15;
            this.saveButton = new SaveButton(Game, spriteBatch, tmp, this);
            Game.Components.Add(this.saveButton);
        }

        public override void Update(GameTime gameTime)
        {
            if (virtualTile == null || virtualTile.IsEmpty())
                virtualTile = new Tile(TileMap.LayerCount, TileMap.SubLayerCount);

            base.Update(gameTime);
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

            position = new Vector2((int)position.X * TileMap.tileWidth, (int)position.Y * TileMap.tileHeight)
                                        - tileMapLayer.Camera.Position * tileMapLayer.ScrollSpeed;

            DrawBoundingRect(position, 2, Microsoft.Xna.Framework.Graphics.Color.White);

            if (Tile != null)
            {
                // Highlight the tile we've selected.
                position = new Vector2(tileX * TileMap.tileWidth, tileY * TileMap.tileHeight)
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
                                                            TileMap.tileWidth + 2*margin - 1, TileMap.tileHeight + 2*margin);

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
