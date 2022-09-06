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
    public class PressableButton : IPressable
    {
        private Buttons button;

        public Buttons Button
        {
            get { return button; }
        }

        public PressableButton(Buttons button)
        {
            this.button = button;
            this.pressedTime = 0;
        }

        #region IPressable Members

        public bool IsPressed
        {
            get
            {
                return InputMonitor.Instance.CurrentPadState.IsButtonDown(Button);
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
                  (!InputMonitor.Instance.PreviousPadState.IsButtonDown(Button) &&
                    InputMonitor.Instance.CurrentPadState.IsButtonDown(Button));
            }
        }

        public bool WasJustReleased
        {
            get
            {
                return
                    (InputMonitor.Instance.PreviousPadState.IsButtonDown(Button) &&
                    !InputMonitor.Instance.CurrentPadState.IsButtonDown(Button));
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
