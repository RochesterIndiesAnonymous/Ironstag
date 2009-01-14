using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Drawing;
using WesternSpace.Interfaces;

namespace WesternSpace.ServiceInterfaces
{
    interface ICameraService
    {
        Vector2 Position
        {
            get;
            set;
        }

        Vector2 ScreenPosition
        {
            get;
        }

        RectangleF VisibleArea
        {
            get;
        }

        Matrix CurrentViewMatrix
        {
            get;
        }
    }
}
