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
        private static KeyboardState lastState = new KeyboardState();
        private static KeyboardState currentState = new KeyboardState();
        private static Dictionary<string, Keys> keyConfig = new Dictionary<string, Keys>();

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
            // Configure Default Mapping
            this.AssignKey("Jump", Keys.Z);
            this.AssignKey("Shoot", Keys.X);
            this.AssignKey("Left", Keys.Left);
            this.AssignKey("Right", Keys.Right);
            this.AssignKey("Up", Keys.Up);
            this.AssignKey("Down", Keys.Down);
            this.AssignKey("Roll", Keys.LeftControl);
            this.AssignKey("Transform", Keys.A);
            this.AssignKey("Pause", Keys.Space);
        }

        /**
         * Overrides the Update method of GameObject. 
         * Retieves and stores the state of the Keyboard.
         */
        public override void Update(GameTime gameTime)
        {
            // store the last KeyboardState retrieved
            lastState = currentState;
            // get the newest KeyboardState
            currentState = Keyboard.GetState();
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
         * Checks the state of desired key by command name
         */
        public bool CheckKey(string command)
        {

            // If the command exists, check its state
            if (keyConfig.ContainsKey(command))
            {
                // If key is currently down, report
                if (currentState.IsKeyDown(keyConfig[command]))
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

        public bool CheckPressAndReleaseKey(string command)
        {
            if (keyConfig.ContainsKey(command))
            {
                if (currentState.IsKeyDown(keyConfig[command]))
                {
                    if (!lastState.IsKeyDown(keyConfig[command]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
