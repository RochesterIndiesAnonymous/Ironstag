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
    class Camera : GameObject, ICamera
    {
        private KeyboardState oldKeyboardState;
        private int xCameraOffset;
        private int yCameraOffset;

        private const int SCROLL_SPEED = 5;

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

        #region ICamera Members

        public int XOffset
        {
            get { return xCameraOffset; }
        }

        public int YOffset
        {
            get { return yCameraOffset; }
        }

        #endregion
    }
}
