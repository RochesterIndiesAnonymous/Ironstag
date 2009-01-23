using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.Interfaces;
using WesternSpace.Utility;

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

        private IScreenResolutionService resolutionService;

        private TileMap tm;

        private float scrollSpeed;

        private int layerIndex;


        // The integer index ranges of the last tiles that were drawn.
        private int startX, startY, endX, endY;

        public int StartX
        {
            get { return startX; }
        }

        public int EndX
        {
            get { return endX; }
        }

        public int StartY
        {
            get { return startY; }
        }

        public int EndY
        {
            get { return endY; }
        }

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
            resolutionService = (IScreenResolutionService)this.Game.Services.GetService(typeof(IScreenResolutionService));

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            float cam_x = camera.Position.X*scrollSpeed;
            float cam_y = camera.Position.Y*scrollSpeed;
            float cam_w = camera.VisibleArea.Width;
            float cam_h = camera.VisibleArea.Height;

            startX = (int)MathHelper.Clamp((float)Math.Floor((cam_x / tm.tileWidth)), 0.0f, (float)tm.Width);
            startY = (int)MathHelper.Clamp((float)Math.Floor((cam_y / tm.tileHeight)), 0.0f, (float)tm.Height);

            endX = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_x + cam_w) / tm.tileWidth)), 0.0f, (float)tm.Width);
            endY = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_y + cam_h) / tm.tileWidth)), 0.0f, (float)tm.Height);


            for (int x = startX; x < endX; ++x)
            {
                for (int y = startY; y < endY; ++y)
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

            DrawEdges();

            base.Draw(gameTime);
        }

        // Useful for debugging and the Editor, draw all solid edges on our tiles.
        //  Presumably, this will be called after all the tiles themselves are drawn.
        public void DrawEdges()
        {
            Vector2 topLeft, topRight, bottomLeft, bottomRight;
            Tile tile;
            PrimitiveDrawer drawer = PrimitiveDrawer.Instance;
            for (int x = startX; x < endX; ++x)
            {
                for (int y = startY; y < endY; ++y)
                {
                    topLeft = new Vector2(x * tm.tileWidth, y * tm.tileHeight)+(camera.Position - camera.Position * scrollSpeed);
                    topRight = topLeft + (new Vector2((float)tm.tileWidth, .0f));
                    bottomLeft = topLeft + (new Vector2(.0f, (float)tm.tileHeight));
                    bottomRight = bottomLeft + (new Vector2((float)tm.tileWidth, .0f));

                    tile = tm.Tiles[x, y];
                    if (tile != null) 
                    {
                        if(tile.TopEdge)
                        {
                            drawer.DrawLine(this.SpriteBatch, topLeft, topRight, Color.Red);
                        }
                        if (tile.LeftEdge)
                        {
                            drawer.DrawLine(this.SpriteBatch, topLeft, bottomLeft, Color.Red);
                        }
                        if (tile.BottomEdge)
                        {
                            drawer.DrawLine(this.SpriteBatch, bottomLeft, bottomRight, Color.Red);
                        }
                        if (tile.RightEdge)
                        {
                            drawer.DrawLine(this.SpriteBatch, bottomRight, topRight, Color.Red);
                        }
                    }
                }
            }
        }

        #region IMapCoordinates Members

        public Vector2 CalculateMapCoordinatesFromScreenPoint(Vector2 atPoint)
        {
            Vector2 offset = new Vector2(resolutionService.ScaleRectangle.X, resolutionService.ScaleRectangle.Y);
            atPoint = (((atPoint - offset) / resolutionService.ScaleFactor) + camera.Position)/(new Vector2(tm.tileWidth, tm.tileHeight));
            atPoint.X = (int)Math.Floor(atPoint.X);
            atPoint.Y = (int)Math.Floor(atPoint.Y);
            return atPoint;
        }

        #endregion
    }
}
