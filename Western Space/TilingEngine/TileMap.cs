using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.TilingEngine
{
    public class TileMap : DrawableGameObject
    {
        private SpriteBatch sb;

        private int gridCellHeight;

        private int gridCellWidth;

        private Tile[,] tiles;

        private ICamera camera;

        public TileMap(Game game, int cellX, int cellY, int tileWidth, int tileHeight)
            : base(game)
        {
            this.Enabled = false;

            sb = new SpriteBatch(game.GraphicsDevice);

            camera = (ICamera)game.Services.GetService(typeof(ICamera));

            tiles = new Tile[cellX,cellY];
            gridCellWidth = tileWidth;
            gridCellHeight = tileHeight;
        }

        public void SetTile(Tile tile, int x, int y)
        {
            tiles[x,y] = tile;
        }

        public override void Draw(GameTime gameTime)
        {
            sb.Begin();

            for (int x = 0, y = 0; x < tiles.GetLength(0) && y < tiles.GetLength(1); x++)
            {
                Vector2 position = new Vector2(x * gridCellWidth, y * gridCellHeight);

                
                tiles[x, y].Draw(sb, position);

                if (x != 0 && x % (tiles.GetLength(0) - 1) == 0)
                {
                    y++;
                    x = -1;
                }
            }

            sb.End();

            base.Draw(gameTime);
        }
    }
}
