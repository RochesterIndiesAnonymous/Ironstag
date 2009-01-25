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

        public Tile Tile
        {
            get { return TileMap.Tiles[tileX, tileY]; }
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

        public void SetTile(int x, int y)
        {
            tileX = x; 
            tileY = y;           
        }

        #region MOUSE EVENT HANDLERS
        public override void OnMouseClick(int button)
        {
            Vector2 pos = TileMapLayer.CalculateMapCoordinatesFromScreenPoint(Mouse.Position);
            SetTile((int)Math.Round((double)(pos.X), 0),
                    (int)Math.Round((double)(pos.Y), 0));
            if (button == 0) // Left button clicked. Select/Add a tile
            {
                if (Tile != null)
                {
                    foreach (SubTextureSelector sts in SubTextureSelectors)
                    {
                        sts.CurrentTexture = Tile.Textures[sts.LayerIndex, sts.SubLayerIndex];
                    }
                }
                if (TileMap.Tiles[tileX, tileY] == null)
                {
                    SubTexture[,] subTextures = new SubTexture[TileMap.LayerCount, TileMap.SubLayerCount];
                    for (int i = 0; i < TileMap.LayerCount; ++i)
                    {
                        for (int j = 0; j < TileMap.SubLayerCount; ++j)
                        {
                            subTextures[i, j] = subTextureSelectors[i * TileMap.SubLayerCount + j].CurrentTexture;
                        }
                    }
                    TileMap.SetTile(new Tile(subTextures), tileX, tileY);
                }
            }
            else if(button == 2) // Right button clicked. Remove a tile
            {
                if (TileMap.Tiles[tileX, tileY] != null)
                {
                    TileMap.RemoveTile(tileX, tileY);
                }
            }

            base.OnMouseClick(button);
        }

        public override void OnMouseUnclick(int button)
        {
            base.OnMouseUnclick(button);
        }

        public override void OnMouseScroll(int amount)
        {
            base.OnMouseEnter();
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
        }

        #endregion

        public TileSelector(Game game, SpriteBatch spriteBatch, RectangleF bounds, TileMapLayer tileMapLayer)
            : base(game, spriteBatch, bounds)
        {
            this.tileMapLayer = tileMapLayer;

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
        #endregion
    }
}
