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
    class CameraService : GameObject, ICameraService
    {
        private Vector2 position;
        private RectangleF visibleArea;
        private Matrix viewMatrix;
        private ILayerService layerService;

        private IInputManagerService inputManager;

        private const int SCROLL_SPEED = 50;

        #region ICamera Members

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

        public Vector2 ScreenPosition
        {
            get
            {
                return new Vector2(0, 0);
            }
        }

        public RectangleF VisibleArea
        {
            get
            {
                return visibleArea;
            }
        }

        public Matrix CurrentViewMatrix
        {
            get { return viewMatrix; }
        }

        #endregion

        public CameraService(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            position = Vector2.Zero;
            viewMatrix = Matrix.Identity;

            Viewport vp = this.Game.GraphicsDevice.Viewport;

            visibleArea = new RectangleF(vp.X, vp.Y, vp.Width, vp.Height);

            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));
            layerService = (ILayerService)this.Game.Services.GetService(typeof(ILayerService));

            base.Initialize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (inputManager.KeyboardState.IsKeyDown(Keys.D) || inputManager.KeyboardState.IsKeyDown(Keys.Right))
            {
                float newX = position.X + SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newX = MathHelper.Clamp(newX, map.MinimumX, map.MaximumX - this.visibleArea.Width);
                    this.position.X = newX;
                }
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.A) || inputManager.KeyboardState.IsKeyDown(Keys.Left))
            {
                float newX = position.X - SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newX = MathHelper.Clamp(newX, map.MinimumX, map.MaximumX - this.visibleArea.Width);
                    this.position.X = newX;
                }
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.W) || inputManager.KeyboardState.IsKeyDown(Keys.Up))
            {
                float newY = position.Y - SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newY = MathHelper.Clamp(newY, map.MinimumY, map.MaximumY - this.visibleArea.Height);
                    this.position.Y = newY;
                }
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.S) || inputManager.KeyboardState.IsKeyDown(Keys.Down))
            {
                float newY = position.Y + SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newY = MathHelper.Clamp(newY, map.MinimumY, map.MaximumY - this.visibleArea.Height);
                    this.position.Y = newY;
                }
            }

            // XBOX360 controllers
            if (inputManager.GamePadState.ThumbSticks.Left.X != 0)
            {
                float newX = position.X + inputManager.GamePadState.ThumbSticks.Left.X * SCROLL_SPEED;
                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newX = MathHelper.Clamp(newX, map.MinimumX, map.MaximumX - this.visibleArea.Width);
                    this.position.X = newX;
                }
            }

            if (inputManager.GamePadState.ThumbSticks.Left.Y != 0)
            {
                float newY = position.Y - inputManager.GamePadState.ThumbSticks.Left.Y * SCROLL_SPEED;

                foreach (IMapCoordinates map in layerService.Layers.Values)
                {
                    newY = MathHelper.Clamp(newY, map.MinimumY, map.MaximumY - this.visibleArea.Height);
                    this.position.Y = newY;
                }
            }

            UpdateVisibleArea();
            CreateViewTransformationMatrix();

            base.Update(gameTime);
        }

        private void UpdateVisibleArea()
        {
            Viewport vp = this.Game.GraphicsDevice.Viewport;

            float left = position.X - visibleArea.Width / 2;
            float top = position.Y - visibleArea.Height / 2;

            visibleArea = new RectangleF(left, top, vp.Width, vp.Height);
        }

        private void CreateViewTransformationMatrix()
        {
            Vector3 matrixRotationOrigin = new Vector3(Position, 0);
            Vector3 matrixScreenPosition = new Vector3(ScreenPosition, 0.0f);

            viewMatrix = Matrix.CreateTranslation(-matrixRotationOrigin) * Matrix.CreateTranslation(matrixScreenPosition);
        }
    }
}
