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
    public class TileMapLayer : DrawableGameObject, IMapCoordinates
    {
        private static string spriteBatchName = "Camera Sensitive";

        public static string SpriteBatchName
        {
            get { return TileMapLayer.spriteBatchName; }
            set { TileMapLayer.spriteBatchName = value; }
        }

        private ICameraService camera;

        private TileMap tm;

        private float scrollSpeed;

        private int layerIndex;

        public int LayerIndex
        {
            get { return layerIndex; }
        }

        #region IMapCoordinates Members

        public float MinimumX
        {
            get { return 0; }
        }

        public float MaximumX
        {
            get { return tm.tileWidth * tm.Tiles.GetLength(0); }
        }

        public float MinimumY
        {
            get { return 0; }
        }

        public float MaximumY
        {
            get { return tm.tileHeight * tm.Tiles.GetLength(1); }
        }

        #endregion

        public TileMapLayer(Game game, SpriteBatch spriteBatch, TileMap tm, int layerIndex)
            : base(game, spriteBatch, Vector2.Zero)
        {
            this.layerIndex = layerIndex;
            scrollSpeed = 1.0f;
            this.tm = tm;
        }

        public TileMapLayer(Game game, SpriteBatch spriteBatch, TileMap tm, int layerIndex, float scrollSpeed)
            : this(game, spriteBatch, tm, layerIndex)
        {
            this.scrollSpeed = scrollSpeed;
        }

        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            float cam_x = camera.Position.X*scrollSpeed;
            float cam_y = camera.Position.Y*scrollSpeed;
            float cam_w = camera.VisibleArea.Width;
            float cam_h = camera.VisibleArea.Height;

            int start_x, end_x, start_y, end_y;

            start_x = (int)MathHelper.Clamp((float)Math.Floor((cam_x / tm.tileWidth)), 0.0f, (float)tm.Width);
            start_y = (int)MathHelper.Clamp((float)Math.Floor((cam_y / tm.tileHeight)), 0.0f, (float)tm.Height);

            end_x = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_x + cam_w) / tm.tileWidth)), 0.0f, (float)tm.Width);
            end_y = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_y + cam_h) / tm.tileWidth)), 0.0f, (float)tm.Height);


            for (int x = start_x; x < end_x; ++x)
            {
                for (int y = start_y; y < end_y; ++y)
                {
                    Vector2 position = new Vector2(x * tm.tileWidth, y * tm.tileHeight) + 
                        (camera.Position - camera.Position*scrollSpeed);

                    if (tm.Tiles[x, y] != null)
                    {
                        for (int subLayerIndex = 0; subLayerIndex < tm.SubLayerCount; ++subLayerIndex)
                        {
                            this.SpriteBatch.Draw(tm.Tiles[x, y].Textures[layerIndex, subLayerIndex], position, Color.White);
                        }
                    }
                }
            }

            base.Draw(gameTime);
        }

        #region IMapCoordinates Members

        public Vector2 CalculateMapCoordinatesFromMouse(Vector2 atPoint)
        {
            int x = (int)Math.Floor((camera.Position.X / tm.tileWidth) + atPoint.X / tm.tileWidth);
            int y = (int)Math.Floor((camera.Position.Y / tm.tileHeight) + atPoint.Y / tm.tileHeight);
            return new Vector2(x, y);
        }

        #endregion
    }
}
