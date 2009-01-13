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
    class Camera : GameObject, ICamera
    {
        private KeyboardState oldKeyboardState;
        private MouseState oldMouseState;
        private Vector2 position;
        private Vector2 offset;
        private RectangleF visibleArea;
        private Matrix viewMatrix;

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
                Viewport vp = this.Game.GraphicsDevice.Viewport;

                //return new Vector2(vp.Width / 2, vp.Height / 2);
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

        public Camera(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            oldKeyboardState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();
            position = Vector2.Zero;
            offset = Vector2.Zero;
            viewMatrix = Matrix.Identity;

            Viewport vp = this.Game.GraphicsDevice.Viewport;

            visibleArea = new RectangleF(vp.X, vp.Y, vp.Width, vp.Height);

            base.Initialize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                offset.X += SCROLL_SPEED;
            }

            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                offset.X -= SCROLL_SPEED;
            }

            if (newKeyboardState.IsKeyDown(Keys.Up))
            {
                offset.Y -= SCROLL_SPEED;
            }

            if (newKeyboardState.IsKeyDown(Keys.Down))
            {
                offset.Y += SCROLL_SPEED;
            }

            oldKeyboardState = newKeyboardState;
            oldMouseState = Mouse.GetState();

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

        #region ICamera Members


        public Vector2 GetMouseWorldCoordinates()
        {
            return new Vector2(Offset.X + oldMouseState.X, Offset.Y + oldMouseState.Y);
        }

        public Vector2 GetScreenCoordinates()
        {
            return new Vector2(oldMouseState.X, oldMouseState.Y);
        }

        public Vector2 GetMapCoordinates(IMapCoordinates coordinateSystem)
        {
            return coordinateSystem.CalculateMapCoordinatesFromMouse(new Vector2(oldMouseState.X, oldMouseState.Y));
        }

        #endregion
    }
}
