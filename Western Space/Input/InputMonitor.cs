using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WesternSpace.Input
{
    /**
     * The InputMonitor class allows us to assign functions to 
     * different keys and then check the state of our assigned keys
     */
    public class InputMonitor : GameComponent
    {
        private static KeyboardState previousKeyboardState = new KeyboardState();
        private static KeyboardState currentKeyboardState = new KeyboardState();
        private static GamePadState previousPadState = new GamePadState();
        private static GamePadState currentPadState = new GamePadState();
        private static Dictionary<string, Keys> keyConfig = new Dictionary<string, Keys>();
        private static Dictionary<string, Buttons> buttonConfig = new Dictionary<string, Buttons>();

        // Constants for checking keys
        public static readonly string JUMP = "Jump";
        public static readonly string SHOOT = "Shoot";
        public static readonly string LEFT = "Left";
        public static readonly string RIGHT = "Right";
        public static readonly string UP = "Up";
        public static readonly string DOWN = "Down";
        public static readonly string ROLL = "Roll";
        public static readonly string TRANSFORM = "Transform";
        public static readonly string PAUSE = "Pause";

        /**
         * Constructor
         * Configures the default key configuration
         */
        public InputMonitor(Game game): base(game)
        {
            // Configure Default Gamepad Mapping
            this.AssignButton(JUMP, Buttons.A);
            this.AssignButton(SHOOT, Buttons.X);
            this.AssignButton(LEFT, Buttons.DPadLeft);
            this.AssignButton(RIGHT, Buttons.DPadRight);
            this.AssignButton(UP, Buttons.DPadUp);
            this.AssignButton(DOWN, Buttons.DPadDown);
            this.AssignButton(ROLL, Buttons.LeftTrigger);
            this.AssignButton(TRANSFORM, Buttons.RightTrigger);
            this.AssignButton(PAUSE, Buttons.Start);

#if !XBOX
            // Configure Default Keyboard Mapping
            this.AssignKey(JUMP, Keys.Z);
            this.AssignKey(SHOOT, Keys.X);
            this.AssignKey(LEFT, Keys.Left);
            this.AssignKey(RIGHT, Keys.Right);
            this.AssignKey(UP, Keys.Up);
            this.AssignKey(DOWN, Keys.Down);
            this.AssignKey(ROLL, Keys.LeftControl);
            this.AssignKey(TRANSFORM, Keys.A);
            this.AssignKey(PAUSE, Keys.Space);
#endif
        }

        /**
         * Overrides the Update method of GameObject. 
         * Retieves and stores the state of the Keyboard.
         */
        public override void Update(GameTime gameTime)
        {
            // Store the last GamePad State retrieved
            previousPadState = currentPadState;

            //Get the new GamePad state
            currentPadState = GamePad.GetState(PlayerIndex.One);

#if !XBOX
            // Store the last KeyboardState retrieved
            previousKeyboardState = currentKeyboardState;

            // Get the newest KeyboardState
            currentKeyboardState = Keyboard.GetState();
#endif
        }

        /**
         * Function for assigning key functions.
         * Example: command could be "Jump" and key could be any key we pass in.
         * Any number of commands can be stored. If it already exists, it will
         * just be saved over the old binding
         */
        public void AssignKey(string command, Keys key)
        {
            if (keyConfig.ContainsKey(command))
            {
                keyConfig[command] = key;
            }
            else
            {
                keyConfig.Add(command, key);
            }
        }

        /**
         * Function for assigning button functions.
         * Example: command could be "Jump" and button could be the 'B' button.
         * Any number of commands can be stored. If it already exists, it will
         * just be saved over the old binding
         */
        public void AssignButton(string command, Buttons button)
        {
            if (buttonConfig.ContainsKey(command))
            {
                buttonConfig[command] = button;
            }
            else
            {
                buttonConfig.Add(command, button);
            }
        }

        /**
         * Checks the state of desired key by command name
         */
        public bool CheckKey(string command)
        {

            // If the command exists, check its state
            if (keyConfig.ContainsKey(command))
            {
                // If key is currently down, report
                if (currentKeyboardState.IsKeyDown(keyConfig[command]))
                {
                    return true;
                }
                // Otherwise, just report that the key is up
                else
                {
                    return false;
                }
            }
            // If the command doesn't exists, return NoMatch
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether or not a key associated with a given command has been pressed and released.
        /// </summary>
        /// <param name="command">The command whose associated key is to be checked.</param>
        /// <returns>True if the key has been released, false otherwise.</returns>
        public bool CheckPressAndReleaseKey(string command)
        {
            if (keyConfig.ContainsKey(command))
            {
                if (currentKeyboardState.IsKeyDown(keyConfig[command]))
                {
                    if (!previousKeyboardState.IsKeyDown(keyConfig[command]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether or not a button associated with a given command has been pressed and released.
        /// </summary>
        /// <param name="command">The command whose associated button is to be checked.</param>
        /// <returns>True if the button has been released, false otherwise.</returns>
        public bool CheckPressAndReleaseButton(string command)
        {
            if (buttonConfig.ContainsKey(command))
            {
                if (currentPadState.IsButtonDown(buttonConfig[command]))
                {
                    if (!previousPadState.IsButtonDown(buttonConfig[command]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
