using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace WesternSpace.Services
{
    class InputManagerService : GameObject, IInputManagerService
    {
        private KeyboardState oldKeyboardState;
        private MouseState oldMouseState;
        private GamePadState oldGamePadState;

        #region IInputManagerService Members

        public Microsoft.Xna.Framework.Input.KeyboardState KeyboardState
        {
            get { return oldKeyboardState; }
        }

        public Microsoft.Xna.Framework.Input.MouseState MouseState
        {
            get { return oldMouseState; }
        }

        public Microsoft.Xna.Framework.Input.GamePadState GamePadState
        {
            get { return oldGamePadState; }
        }

        #endregion

        public InputManagerService(Game game)
            : base(game)
        {
        }

        public override void Initialize()
        {
            oldKeyboardState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();
            oldGamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            base.Initialize();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();
            GamePadState newGamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            // Allows the game to exit
            if (newGamePadState.Buttons.Back == ButtonState.Pressed || newKeyboardState.IsKeyDown(Keys.Escape))
                this.Game.Exit();

            oldKeyboardState = newKeyboardState;
            oldMouseState = newMouseState;
            oldGamePadState = newGamePadState;

            base.Update(gameTime);
        }
    }
}
