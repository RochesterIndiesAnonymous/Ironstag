using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using WesternSpace.Input;
using WesternSpace.Screens;

namespace WesternSpace.Services
{
    /// <summary>
    /// The implementation of the input manager service
    /// </summary>
    public class InputManagerService : GameComponent, IInputManagerService
    {
        /// <summary>
        /// The old keyboard state before the latest update
        /// </summary>
        private KeyboardState oldKeyboardState;

        /// <summary>
        /// The old mouse state before the latest update
        /// </summary>
        private MouseState oldMouseState;

        private BetterMouse betterMouse;

        /// <summary>
        /// The old game pad state before the latest update
        /// </summary>
        private GamePadState oldGamePadState;

        #region IInputManagerService Members

        /// <summary>
        /// The current state of the keyboard
        /// </summary>
        public Microsoft.Xna.Framework.Input.KeyboardState KeyboardState
        {
            get { return oldKeyboardState; }
        }

        /// <summary>
        /// The current state of the mouse
        /// </summary>
        public Microsoft.Xna.Framework.Input.MouseState MouseState
        {
            get { return oldMouseState; }
        }

        public BetterMouse BetterMouse
        {
            get { return betterMouse; }
        }

        /// <summary>
        /// The current state of the game pad
        /// </summary>
        public Microsoft.Xna.Framework.Input.GamePadState GamePadState
        {
            get { return oldGamePadState; }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The game this input manager is assocaiated with</param>
        public InputManagerService(Game game)
            : base(game)
        {
        }

        /// <summary>
        /// Initializes the internal state of the service
        /// </summary>
        public override void Initialize()
        {
            oldKeyboardState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();
            oldGamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            betterMouse = new BetterMouse(Game);
            betterMouse.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// Updates the input state of the service
        /// </summary>
        /// <param name="gameTime">The time relative to the game</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            KeyboardState newKeyboardState = Keyboard.GetState();
            MouseState newMouseState = Mouse.GetState();
            GamePadState newGamePadState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);

            // Allows the game to exit
            if (newGamePadState.Buttons.Back == ButtonState.Pressed || newKeyboardState.IsKeyDown(Keys.Escape))
                this.Game.Exit();

            if (newKeyboardState.IsKeyDown(Keys.F12))
            {
                if (!oldKeyboardState.IsKeyDown(Keys.F12))
                {
                    ScreenManager.Instance.RemoveScreenFromDisplay(GameScreen.ScreenName);
                    ScreenManager.Instance.AddScreenToDisplay(EditorScreen.ScreenName);
                }
            }

            if (newKeyboardState.IsKeyDown(Keys.F11))
            {
                if (!oldKeyboardState.IsKeyDown(Keys.F11))
                {
                    ScreenManager.Instance.RemoveScreenFromDisplay(EditorScreen.ScreenName);
                    ScreenManager.Instance.AddScreenToDisplay(GameScreen.ScreenName);
                }
            }

            oldKeyboardState = newKeyboardState;
            oldMouseState = newMouseState;
            oldGamePadState = newGamePadState;

            betterMouse.Update(gameTime);

            base.Update(gameTime);
        }
    }
}
