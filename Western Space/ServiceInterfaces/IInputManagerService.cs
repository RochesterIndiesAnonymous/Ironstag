using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace WesternSpace.ServiceInterfaces
{
    interface IInputManagerService
    {
        KeyboardState KeyboardState
        {
            get;
        }

        MouseState MouseState
        {
            get;
        }

        GamePadState GamePadState
        {
            get;
        }
    }
}
