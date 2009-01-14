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
        private Vector2 offset;
        private RectangleF visibleArea;
        private Matrix viewMatrix;

        private IInputManagerService inputManager;

        private const int SCROLL_SPEED = 5;

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

        public Vector2 Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
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
            offset = Vector2.Zero;
            viewMatrix = Matrix.Identity;

            Viewport vp = this.Game.GraphicsDevice.Viewport;

            visibleArea = new RectangleF(vp.X, vp.Y, vp.Width, vp.Height);

            inputManager = (IInputManagerService)this.Game.Services.GetService(typeof(IInputManagerService));

            base.Initialize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (inputManager.KeyboardState.IsKeyDown(Keys.D) || inputManager.KeyboardState.IsKeyDown(Keys.Right))
            {
                this.offset.X += SCROLL_SPEED;
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.A) || inputManager.KeyboardState.IsKeyDown(Keys.Left))
            {
                this.offset.X -= SCROLL_SPEED;
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.W) || inputManager.KeyboardState.IsKeyDown(Keys.Up))
            {
                this.offset.Y -= SCROLL_SPEED;
            }

            if (inputManager.KeyboardState.IsKeyDown(Keys.S) || inputManager.KeyboardState.IsKeyDown(Keys.Down))
            {
                this.offset.Y += SCROLL_SPEED;
            }

            // XBOX360 controllers
            if (inputManager.GamePadState.ThumbSticks.Left.X != 0)
            {
                this.offset.X += inputManager.GamePadState.ThumbSticks.Left.X * SCROLL_SPEED;
            }

            if (inputManager.GamePadState.ThumbSticks.Left.Y != 0)
            {
                this.offset.Y -= inputManager.GamePadState.ThumbSticks.Left.Y * SCROLL_SPEED;
            }

            UpdateVisibleArea();
            CreateViewTransformationMatrix();

            base.Update(gameTime);
        }

        private void UpdateVisibleArea()
        {
            Viewport vp = this.Game.GraphicsDevice.Viewport;

            float left = position.X + offset.X - visibleArea.Width / 2;
            float top = position.Y + offset.Y - visibleArea.Height / 2;

            visibleArea = new RectangleF(left, top, vp.Width, vp.Height);
        }

        private void CreateViewTransformationMatrix()
        {
            Vector3 matrixRotationOrigin = new Vector3(Position + Offset, 0);
            Vector3 matrixScreenPosition = new Vector3(ScreenPosition, 0.0f);

            viewMatrix = Matrix.CreateTranslation(-matrixRotationOrigin) * Matrix.CreateTranslation(matrixScreenPosition);
        }
    }
}
