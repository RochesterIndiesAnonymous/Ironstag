using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Drawing;

namespace WesternSpace.ServiceInterfaces
{
    interface ICamera
    {
        Vector2 Position
        {
            get;
            set;
        }

        Vector2 Offset
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
