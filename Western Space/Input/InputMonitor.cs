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
#if !XBOX
        private KeyboardState previousKeyboardState;

        public KeyboardState PreviousKeyboardState
        {
            get { return previousKeyboardState; }
        }


        private KeyboardState currentKeyboardState;

        public KeyboardState CurrentKeyboardState
        {
            get { return currentKeyboardState; }
        }
#endif

        private GamePadState previousPadState;

        public GamePadState PreviousPadState
        {
            get { return previousPadState; }
        }


        private GamePadState currentPadState;

        public GamePadState CurrentPadState
        {
            get { return currentPadState; }
        }


        private Dictionary<string, List<IPressable>> pressables;

        public Dictionary<string, List<IPressable>> Pressables
        {
            get { return pressables; }
        }

        private Dictionary<string, float> leftJoystickConfig = new Dictionary<string, float>();

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

        private static InputMonitor instance;

        public static InputMonitor Instance
        {
            get 
            {
                if (instance == null)
                    instance = new InputMonitor(ScreenManager.Instance);
                return instance; 
            }
        }

        /**
         * Constructor
         * Configures the default key configuration
         */
        public InputMonitor(Game game): base(game)
        {
            this.pressables = new Dictionary<string, List<IPressable>>();
            // Configure Default Gamepad Mapping
            this.AssignPressable(JUMP, new PressableButton(Buttons.A));
            this.AssignPressable(SHOOT, new PressableButton(Buttons.X));
            this.AssignPressable(LEFT, new PressableButton(Buttons.DPadLeft));
            this.AssignPressable(RIGHT, new PressableButton(Buttons.DPadRight));
            this.AssignPressable(UP, new PressableButton(Buttons.DPadUp));
            this.AssignPressable(DOWN, new PressableButton(Buttons.DPadDown));
            this.AssignPressable(ROLL, new PressableButton(Buttons.LeftTrigger));
            this.AssignPressable(TRANSFORM, new PressableButton(Buttons.RightTrigger));
            this.AssignPressable(PAUSE, new PressableButton(Buttons.Start));

            this.AssignLeftJoystick(LEFT, -0.5f);
            this.AssignLeftJoystick(RIGHT, 0.5f);

#if !XBOX
            // Configure Default Keyboard Mapping
            this.AssignPressable(JUMP, new PressableKey(Keys.Z));
            this.AssignPressable(SHOOT, new PressableKey(Keys.X));
            this.AssignPressable(LEFT, new PressableKey(Keys.Left));
            this.AssignPressable(RIGHT, new PressableKey(Keys.Right));
            this.AssignPressable(UP, new PressableKey(Keys.Up));
            this.AssignPressable(DOWN, new PressableKey(Keys.Down));
            this.AssignPressable(ROLL, new PressableKey(Keys.LeftControl));
            this.AssignPressable(TRANSFORM, new PressableKey(Keys.A));
            this.AssignPressable(PAUSE, new PressableKey(Keys.Space));
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

            foreach (List<IPressable> pressableList in Pressables.Values)
            {
                foreach (IPressable pressable in pressableList)
                {
                    pressable.Update(gameTime);
                }
            }

            #region FOR DEBUG
            /*
            foreach (string command in Pressables.Keys)
            {
                if (IsPressed(command))
                    System.Console.WriteLine(command + ": " + GetPressedTime(command));
            }
            */
            #endregion
        }

        /**
         * Function for assigning key functions.
         * Example: command could be "Jump" and pressable could be any pressable we pass in.
         * Any number of commands can be stored, and have any number of pressables associated
         * with them.
         */
        public void AssignPressable(string command, IPressable pressable)
        {
            if (!pressables.ContainsKey(command))
            {
                pressables[command] = new List<IPressable>();
            }
            pressables[command].Add(pressable);
        }

        public void AssignLeftJoystick(string command, float threshold)
        {
            leftJoystickConfig[command] = threshold;
        }

        public bool CheckLeftJoystickOnXAxis(string command)
        {
            if(leftJoystickConfig.ContainsKey(command))
            {
                float threshold = leftJoystickConfig[command];

                if(threshold < 0 && currentPadState.ThumbSticks.Left.X < threshold)
                {
                    return true;
                }
                else if(threshold > 0 && currentPadState.ThumbSticks.Left.X > threshold)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if any pressables associated with a given command string are pressed.
        /// </summary>
        /// <param name="command">The command string to query.</param>
        /// <returns>True if any associated pressable is pressed.</returns>
        public bool IsPressed(string command)
        {
            if (pressables.ContainsKey(command))
            {
                foreach (IPressable pressable in pressables[command])
                {
                    if (pressable.IsPressed)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any pressables associated with a given command string are released.
        /// </summary>
        /// <param name="command">The command string to query.</param>
        /// <returns>True if any associated pressable is released.</returns>
        public bool IsReleased(string command)
        {
            if (pressables.ContainsKey(command))
            {
                foreach (IPressable pressable in pressables[command])
                {
                    if (pressable.IsReleased)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any pressables associated with a given command string have just been pressed.
        /// </summary>
        /// <param name="command">The command string to query.</param>
        /// <returns>True if any associated pressable was just pressed since the last update.</returns>
        public bool WasJustPressed(string command)
        {
            if (pressables.ContainsKey(command))
            {
                foreach (IPressable pressable in pressables[command])
                {
                    if (pressable.WasJustPressed)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Check if any pressables associated with a given command string have just been released.
        /// </summary>
        /// <param name="command">The command string to query.</param>
        /// <returns>True if any associated pressable was just released since the last update.</returns>
        public bool WasJustReleased(string command)
        {
            if (pressables.ContainsKey(command))
            {
                foreach (IPressable pressable in pressables[command])
                {
                    if (pressable.WasJustReleased)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the amount of time any pressables associated with a given command
        /// were held down.
        /// </summary>
        /// <param name="command">The command string to query.</param>
        /// <returns>The maximum time any associated pressables have been pressed for.</returns>
        public int GetPressedTime(string command)
        {
            int max = 0;
            if (pressables.ContainsKey(command))
            {
                foreach (IPressable pressable in pressables[command])
                {
                    if (max < pressable.PressedTime)
                        max = pressable.PressedTime;
                }
            }
            return max;
        }

        /// <summary>
        /// Get the amount of time any pressables associated with a given command
        /// have been released for.
        /// </summary>
        /// <param name="command">The command string to query.</param>
        /// <returns>The maximum time any associated pressables have been released for.</returns>
        public int GetReleasedTime(string command)
        {
            int max = 0;
            if (pressables.ContainsKey(command))
            {
                foreach (IPressable pressable in pressables[command])
                {
                    if (max < pressable.ReleasedTime)
                        max = pressable.ReleasedTime;
                }
            }
            return max;
        }
    }
}
