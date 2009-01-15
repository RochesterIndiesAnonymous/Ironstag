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
    class TileMapLayer : DrawableGameObject, IMapCoordinates
    {
        private SpriteBatch sb;
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
            get { return tm.gridCellWidth * tm.Tiles.GetLength(0); }
        }

        public float MinimumY
        {
            get { return 0; }
        }

        public float MaximumY
        {
            get { return tm.gridCellHeight * tm.Tiles.GetLength(1); }
        }

        #endregion

        public TileMapLayer(Game game, TileMap tm, int layerIndex)
            : base(game)
        {
            this.layerIndex = layerIndex;
            scrollSpeed = 1.0f;
            this.tm = tm;
        }

        public TileMapLayer(Game game, TileMap tm, int layerIndex, float scrollSpeed)
            : base(game)
        {
            this.layerIndex = layerIndex;
            this.scrollSpeed = scrollSpeed;
            this.tm = tm;
        }

        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));
            sb = new SpriteBatch(this.Game.GraphicsDevice);
            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            sb.Begin(SpriteBlendMode.None, SpriteSortMode.BackToFront, SaveStateMode.None, camera.CurrentViewMatrix);
            float cam_x = camera.Position.X*scrollSpeed;
            float cam_y = camera.Position.Y*scrollSpeed;
            float cam_w = camera.VisibleArea.Width;
            float cam_h = camera.VisibleArea.Height;

            int start_x, end_x, start_y, end_y;

            start_x = (int)MathHelper.Clamp((float)Math.Floor((cam_x / tm.gridCellWidth)), 0.0f, (float)tm.Width);
            start_y = (int)MathHelper.Clamp((float)Math.Floor((cam_y / tm.gridCellHeight)), 0.0f, (float)tm.Height);

            end_x = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_x + cam_w) / tm.gridCellWidth)), 0.0f, (float)tm.Width);
            end_y = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_y + cam_h) / tm.gridCellWidth)), 0.0f, (float)tm.Height);


            for (int x = start_x; x < end_x; ++x)
            {
                for (int y = start_y; y < end_y; ++y)
                {
                    Vector2 position = new Vector2(x * tm.gridCellWidth, y * tm.gridCellHeight) + (camera.Position - camera.Position*scrollSpeed);
                    for (int subLayerIndex = 0; subLayerIndex < tm.SubLayerCount; ++subLayerIndex)
                    {
                        sb.Draw(tm.Tiles[x, y].Textures[layerIndex, subLayerIndex], position, Color.White);
                    }
                }
            }

            sb.End();

            base.Draw(gameTime);
        }

        #region IMapCoordinates Members

        public Vector2 CalculateMapCoordinatesFromMouse(Vector2 atPoint)
        {
            int x = (int)Math.Floor((camera.Position.X / tm.gridCellWidth) + atPoint.X / tm.gridCellWidth);
            int y = (int)Math.Floor((camera.Position.Y / tm.gridCellHeight) + atPoint.Y / tm.gridCellHeight);
            return new Vector2(x, y);
        }

        #endregion
    }
}
