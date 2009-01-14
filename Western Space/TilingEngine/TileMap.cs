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
    public class TileMap : DrawableGameObject, IMapCoordinates
    {
        private SpriteBatch sb;

        private int gridCellHeight;

        private int gridCellWidth;

        private Tile[,] tiles;

        private ICameraService camera;

        #region IMapCoordinates Members

        public float MinimumX
        {
            get { return 0; }
        }

        public float MaximumX
        {
            get { return gridCellWidth * tiles.GetLength(0); }
        }

        public float MinimumY
        {
            get { return 0; }
        }

        public float MaximumY
        {
            get { return gridCellHeight * tiles.GetLength(1); }
        }

        #endregion

        public TileMap(Game game, int cellX, int cellY, int tileWidth, int tileHeight)
            : base(game)
        {
            this.Enabled = false;

            sb = new SpriteBatch(game.GraphicsDevice);

            tiles = new Tile[cellX,cellY];
            gridCellWidth = tileWidth;
            gridCellHeight = tileHeight;
        }

        public void SetTile(Tile tile, int x, int y)
        {
            tiles[x,y] = tile;
        }

        public override void Initialize()
        {
            camera = (ICameraService)this.Game.Services.GetService(typeof(ICameraService));

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            sb.Begin(SpriteBlendMode.None, SpriteSortMode.BackToFront, SaveStateMode.None, camera.CurrentViewMatrix);
            float cam_x = camera.Position.X;
            float cam_y = camera.Position.Y;
            float cam_w = camera.VisibleArea.Width;
            float cam_h = camera.VisibleArea.Height;

            int start_x, end_x, start_y, end_y;

            start_x = (int)MathHelper.Clamp((float)Math.Floor((cam_x / gridCellWidth)), 0.0f, (float)tiles.GetLength(0));
            start_y = (int)MathHelper.Clamp((float)Math.Floor((cam_y / gridCellHeight)), 0.0f, (float)tiles.GetLength(1));

            end_x = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_x+cam_w) / gridCellWidth)), 0.0f, (float)tiles.GetLength(0));
            end_y = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_y+cam_h) / gridCellWidth)), 0.0f, (float)tiles.GetLength(1));


            for (int x = start_x; x < end_x; ++x)
            {
                for (int y = start_y; y < end_y; ++y)
                {
                    Vector2 position = new Vector2(x * gridCellWidth, y * gridCellHeight);

                    tiles[x, y].Draw(sb, position);
                }
            }

            sb.End();

            base.Draw(gameTime);
        }

        #region IMapCoordinates Members

        public Vector2 CalculateMapCoordinatesFromMouse(Vector2 atPoint)
        {
            int x = (int)Math.Floor((camera.Position.X / gridCellWidth) + atPoint.X / gridCellWidth);
            int y = (int)Math.Floor((camera.Position.Y / gridCellHeight) + atPoint.Y / gridCellHeight);
            return new Vector2(x, y);
        }

        #endregion
    }
}
