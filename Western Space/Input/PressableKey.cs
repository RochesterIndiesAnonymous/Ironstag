using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace WesternSpace.Input
{
    /// <summary>
    /// A Pressable item that uses an XNA Key.
    /// </summary>
    public class PressableKey : IPressable
    {
        private Keys key;

        public Keys Key
        {
            get { return key; }
        }

        public PressableKey(Keys key)
        {
            this.key = key;
            this.pressedTime = 0;
        }

        #region IPressable Members

        public bool IsPressed
        {
            get 
            {
                return InputMonitor.Instance.CurrentKeyboardState.IsKeyDown(Key);
            }
        }

        public bool IsReleased
        {
            get 
            {
                return !IsPressed;
            }
        }

        public bool WasJustPressed
        {
            get 
            {
                return
                  (!InputMonitor.Instance.PreviousKeyboardState.IsKeyDown(Key) &&
                    InputMonitor.Instance.CurrentKeyboardState.IsKeyDown(Key));
            }
        }

        public bool WasJustReleased
        {
            get 
            {
                return
                    (InputMonitor.Instance.PreviousKeyboardState.IsKeyDown(Key) &&
                    !InputMonitor.Instance.CurrentKeyboardState.IsKeyDown(Key));
            }
        }

        private int pressedTime;

        public int PressedTime
        {
            get
            {
                if (pressedTime < 0)
                {
                    return 0;
                }
                else
                {
                    return pressedTime;
                }
            }
        }

        public int ReleasedTime
        {
            get 
            {
                if (pressedTime > 0)
                {
                    return 0;
                }
                else
                {
                    return -pressedTime;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (IsPressed)
            {
                if (pressedTime < 0)
                    pressedTime = 0;
                pressedTime += gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                if (pressedTime > 0)
                    pressedTime = 0;
                pressedTime -= gameTime.ElapsedGameTime.Milliseconds;
            }
        }

        #endregion
    }
}
