using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.Services
{
    class Camera : DrawableGameObject, ICamera
    {
        private KeyboardState oldKeyboardState;
        private int xCameraOffset;

        private const int SCROLL_SPEED = 1;

        public Camera(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            oldKeyboardState = Keyboard.GetState();
            xCameraOffset = 0;

            base.Initialize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();

            if (newKeyboardState.IsKeyDown(Keys.Right))
            {
                xCameraOffset += SCROLL_SPEED;
            }

            if (newKeyboardState.IsKeyDown(Keys.Left))
            {
                xCameraOffset -= SCROLL_SPEED;
            }

            oldKeyboardState = newKeyboardState;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Viewport vp = this.Game.GraphicsDevice.Viewport;
            vp.X += xCameraOffset;

            base.Draw(gameTime);
        }
    }
}
