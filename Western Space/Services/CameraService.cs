using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Drawing;
using WesternSpace.Interfaces;

namespace WesternSpace.Services
{
    /// <summary>
    /// The implementation of the camera service
    /// </summary>
    class CameraService : GameObject, ICameraService
    {
        /// <summary>
        /// The position of the camera within the world
        /// </summary>
        private Vector2 position;

        /// <summary>
        /// The visible area of the world in the form of a rectangle
        /// </summary>
        private RectangleF visibleArea;

        private Matrix viewMatrix;
        private ILayerService layerService;

        private IInputManagerService inputManager;
        private IScreenResolutionService resolutionService;

        private const int SCROLL_SPEED = 14;

        #region ICamera Members

        /// <summary>
        /// The position of the camera within the world
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                UpdateVisibleArea();
            }
        }

        /// <summary>
        /// The screen position of the camera.
        /// </summary>
        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        /// <summary>
        /// The visible area of the world in the form of a rectangle
        /// </summary>
        public RectangleF VisibleArea
        {
            get
            {
                return visibleArea;
            }
        }

        /// <summary>
        /// The current view transformation matrix for sprite batches
        /// </summary>
        public Matrix CurrentViewMatrix
        {
            get { return viewMatrix; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game this camera is associated with</param>
        public CameraService(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes the internal state of the camera
        /// </summary>
        public override void Initialize()
        {
            position = Vector2.Zero;
            viewMatrix = Matrix.Identity;

            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));
            layerService = (ILayerService)this.Game.Services.GetService(typeof(ILayerService));
            resolutionService = (IScreenResolutionService)this.Game.Services.GetService(typeof(IScreenResolutionService));

            Viewport vp = this.Game.GraphicsDevice.Viewport;

            visibleArea = new RectangleF(position.X, position.Y, 
                resolutionService.StartTextureWidth, resolutionService.StartTextureHeight);

            base.Initialize();
        }

        /// <summary>
        /// Updates the camera position based on input
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (inputManager.KeyboardState.IsKeyDown(Keys.D) || inputManager.KeyboardState.IsKeyDown(Keys.Right))
            {
                float newX = position.X + SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newX = MathHelper.Clamp(newX, map.MinimumX, map.MaximumX - this.visibleArea.Width);
                    this.Position = new Vector2(newX, this.Position.Y);
                }
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.A) || inputManager.KeyboardState.IsKeyDown(Keys.Left))
            {
                float newX = position.X - SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newX = MathHelper.Clamp(newX, map.MinimumX, map.MaximumX - this.visibleArea.Width);
                    this.Position = new Vector2(newX, this.Position.Y);
                }
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.W) || inputManager.KeyboardState.IsKeyDown(Keys.Up))
            {
                float newY = position.Y - SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newY = MathHelper.Clamp(newY, map.MinimumY, map.MaximumY - this.visibleArea.Height);
                    this.Position = new Vector2(this.Position.X, newY);
                }
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.S) || inputManager.KeyboardState.IsKeyDown(Keys.Down))
            {
                float newY = position.Y + SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newY = MathHelper.Clamp(newY, map.MinimumY, map.MaximumY - this.visibleArea.Height);
                    this.Position = new Vector2(this.Position.X, newY);
                }
            }

            // XBOX360 controllers
            if (inputManager.GamePadState.ThumbSticks.Left.X != 0)
            {
                float newX = position.X + inputManager.GamePadState.ThumbSticks.Left.X * SCROLL_SPEED;
                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newX = MathHelper.Clamp(newX, map.MinimumX, map.MaximumX - this.visibleArea.Width);
                    this.Position = new Vector2(newX, this.Position.Y);
                }
            }

            if (inputManager.GamePadState.ThumbSticks.Left.Y != 0)
            {
                float newY = position.Y - inputManager.GamePadState.ThumbSticks.Left.Y * SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newY = MathHelper.Clamp(newY, map.MinimumY, map.MaximumY - this.visibleArea.Height);
                    this.Position = new Vector2(this.Position.X, newY);
                }
            }

            CreateViewTransformationMatrix();

            base.Update(gameTime);
        }

        /// <summary>
        /// Updates the visible area rectangle of the camera
        /// </summary>
        private void UpdateVisibleArea()
        {
            visibleArea = new RectangleF(position.X, position.Y, resolutionService.StartTextureWidth, resolutionService.StartTextureHeight);
        }

        /// <summary>
        /// Updates the view transformation matrix
        /// </summary>
        private void CreateViewTransformationMatrix()
        {
            Vector3 matrixRotationOrigin = new Vector3(Position, 0);
            Vector3 matrixScreenPosition = new Vector3(ScreenPosition, 0.0f);

            viewMatrix = Matrix.CreateTranslation(-matrixRotationOrigin) * Matrix.CreateTranslation(matrixScreenPosition);
        }
    }
}
