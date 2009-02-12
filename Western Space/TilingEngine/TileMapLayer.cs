using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using WesternSpace.ServiceInterfaces;
using WesternSpace.Interfaces;
using WesternSpace.Utility;
using WesternSpace.Screens;

namespace WesternSpace.TilingEngine
{
    public class TileMapLayer : DrawableGameObject, IMapCoordinates
    {
        private bool drawEdgesEnabled;

        public bool DrawEdgesEnabled
        {
            get { return drawEdgesEnabled; }
            set { drawEdgesEnabled = value; }
        }

        private bool drawBlanksEnabled;

        public bool DrawBlanksEnabled
        {
            get { return drawBlanksEnabled; }
            set { drawBlanksEnabled = value; }
        }

        private bool drawDestructablesEnabled;

        public bool DrawDestructablesEnabled
        {
            get { return drawDestructablesEnabled; }
            set { drawDestructablesEnabled = value; }
        }

        private static string spriteBatchName = "Camera Sensitive";

        public static string SpriteBatchName
        {
            get { return TileMapLayer.spriteBatchName; }
            set { TileMapLayer.spriteBatchName = value; }
        }

        private ICameraService camera;

        public ICameraService Camera
        {
            get { return camera; }
        }

        private IScreenResolutionService resolutionService;

        private TileMap tm;

        private float scrollSpeed;

        public float ScrollSpeed
        {
            get { return scrollSpeed; }
            set { scrollSpeed = value; }
        }

        private int layerIndex;

        public TileMap TileMap
        {
            get { return tm; }
        }

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
            get { return tm.TileWidth * tm.Width; }
        }

        public float MinimumY
        {
            get { return 0; }
        }

        public float MaximumY
        {
            get { return tm.TileHeight * tm.Height; }
        }

        #endregion

        public TileMapLayer(Screen parentScreen, SpriteBatch spriteBatch, TileMap tm, int layerIndex)
            : base(parentScreen, spriteBatch, Vector2.Zero)
        {
            this.drawEdgesEnabled = false;
            this.drawBlanksEnabled = false;
            this.drawDestructablesEnabled = false;
            this.layerIndex = layerIndex;
            scrollSpeed = 1.0f;
            this.tm = tm;
        }

        public TileMapLayer(Screen parentScreen, SpriteBatch spriteBatch, TileMap tm, int layerIndex, float scrollSpeed)
            : this(parentScreen, spriteBatch, tm, layerIndex)
        {
            this.drawEdgesEnabled = false;
            this.drawBlanksEnabled = false;
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
            Vector2 camPos = camera.Position;
            float cam_x = camPos.X*scrollSpeed;
            float cam_y = camPos.Y*scrollSpeed;
            float cam_w = camera.VisibleArea.Width;
            float cam_h = camera.VisibleArea.Height;
            

            startX = (int)MathHelper.Clamp((float)Math.Floor((cam_x / tm.TileWidth)), 0.0f, (float)tm.Width);
            startY = (int)MathHelper.Clamp((float)Math.Floor((cam_y / tm.TileHeight)), 0.0f, (float)tm.Height);

            endX = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_x + cam_w) / tm.TileWidth)), 0.0f, (float)tm.Width);
            endY = (int)MathHelper.Clamp((float)Math.Ceiling(((cam_y + cam_h) / tm.TileWidth)), 0.0f, (float)tm.Height);


            for (int x = startX; x < endX; ++x)
            {
                for (int y = startY; y < endY; ++y)
                {
                    Vector2 position = new Vector2(x * tm.TileWidth, y * tm.TileHeight) + 
                        (camPos - camPos*scrollSpeed);
                    //position.X = (int)Math.Round(position.X, 0);
                    //position.Y = (int)Math.Round(position.Y, 0);
                    if (tm[x, y] != null)
                    {
                        for (int subLayerIndex = 0; subLayerIndex < tm.SubLayerCount; ++subLayerIndex)
                        {
                            SubTexture subTexture = tm[x, y].Textures[layerIndex, subLayerIndex];
                            if (subTexture != null)
                            {
                                this.SpriteBatch.Draw(subTexture.Texture, position, subTexture.Rectangle, Color.White);
                            }

                        }

                    }
                    else if (drawBlanksEnabled)
                    {
                        DrawBlank(position);
                    }
                }
            }

            if (drawEdgesEnabled)
            {
                DrawEdges();
            }

            if (drawDestructablesEnabled)
            {
                DrawDestructables();
            }

            base.Draw(gameTime);
        }

        private void DrawDestructables()
        { 
            Vector2 topLeft, topRight, bottomLeft, bottomRight;
            Tile tile;
            PrimitiveDrawer drawer = PrimitiveDrawer.Instance;
            Vector2 camPos = camera.Position;
            Color col = Color.Red;
            for (int x = startX; x < endX; ++x)
            {
                for (int y = startY; y < endY; ++y)
                {
                    topLeft = new Vector2(x * tm.TileWidth, y * tm.TileHeight)+(camPos - camPos * scrollSpeed);
                    topRight = topLeft + (new Vector2((float)tm.TileWidth, .0f));
                    bottomLeft = topLeft + (new Vector2(.0f, (float)tm.TileHeight));
                    bottomRight = bottomLeft + (new Vector2((float)tm.TileWidth, .0f));

                    tile = tm[x, y];
                    if (tile != null && tile is DestructableTile)
                    {
                        /*PrimitiveDrawer.Instance.DrawRect(SpriteBatch,
                                              new Rectangle((int)topLeft.X, (int)topLeft.Y, tm.TileWidth, tm.TileHeight),
                                              col);*/
                        PrimitiveDrawer.Instance.DrawLine(SpriteBatch, topLeft, bottomRight, col);
                        PrimitiveDrawer.Instance.DrawLine(SpriteBatch, topRight, bottomLeft, col);
                    }
                }
            }
        }

        /// <summary>
        /// Possibly the best method name ever?
        /// </summary>
        /// <param name="position">TopLeft of the blank tile.</param>
        private void DrawBlank(Vector2 position)
        {
            position.X += 1;
            Color col = new Color(1,1,1,0.3f);
            PrimitiveDrawer.Instance.DrawRect(SpriteBatch, 
                                              new Rectangle((int)position.X, (int)position.Y, tm.TileWidth, tm.TileHeight),
                                              col);
            col = new Color(1, 1, 1, 0.5f);
            PrimitiveDrawer.Instance.DrawLine(SpriteBatch, position, position + new Vector2(tm.TileWidth, tm.TileHeight), col);
        }

        // Useful for debugging and the Editor, draw all solid edges on our tiles.
        //  Presumably, this will be called after all the tiles themselves are drawn.
        private void DrawEdges()
        {
            Vector2 topLeft, topRight, bottomLeft, bottomRight;
            Tile tile;
            PrimitiveDrawer drawer = PrimitiveDrawer.Instance;
            Vector2 camPos = camera.Position;
            for (int x = startX; x < endX; ++x)
            {
                for (int y = startY; y < endY; ++y)
                {
                    topLeft = new Vector2(x * tm.TileWidth, y * tm.TileHeight)+(camPos - camPos * scrollSpeed);
                    topRight = topLeft + (new Vector2((float)tm.TileWidth, .0f));
                    bottomLeft = topLeft + (new Vector2(.0f, (float)tm.TileHeight));
                    bottomRight = bottomLeft + (new Vector2((float)tm.TileWidth, .0f));

                    tile = tm[x, y];
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
            atPoint = (((atPoint - offset) / resolutionService.ScaleFactor) + camera.Position) / (new Vector2(tm.TileWidth, tm.TileHeight));
            atPoint.X = (int)Math.Floor(atPoint.X);
            atPoint.Y = (int)Math.Floor(atPoint.Y);
            return atPoint;
        }

        #endregion
    }
}
