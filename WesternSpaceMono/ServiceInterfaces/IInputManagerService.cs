using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

using WesternSpace.Input;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// The interface for accessing the input manager service
    /// </summary>
    public interface IInputManagerService
    {
        /// <summary>
        /// The current state of the keyboard
        /// </summary>
        KeyboardState KeyboardState
        {
            get;
        }

        /// <summary>
        /// The current state of the mouse
        /// </summary>
        MouseState MouseState
        {
            get;
        }

        BetterMouse BetterMouse
        {
            get;
        }

        /// <summary>
        /// The current state of the game pad
        /// </summary>
        GamePadState GamePadState
        {
            get;
        }
    }
}
