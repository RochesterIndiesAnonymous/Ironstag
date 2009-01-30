using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Drawing;
using WesternSpace.Interfaces;

namespace WesternSpace.ServiceInterfaces
{
    /// <summary>
    /// The interface for accessing the camera service
    /// </summary>
    public interface ICameraService
    {
        /// <summary>
        /// The non-rounded position of our camera in the world.
        /// </summary>
        Vector2 RealPosition
        {
            get;
        }

        /// <summary>
        /// The position of the camera within the world, rounded to the nearest integer.
        /// </summary>
        Vector2 Position
        {
            get;
            set;
        }

        /// <summary>
        /// The screen position of the camera.
        /// </summary>
        Vector2 ScreenPosition
        {
            get;
        }

        /// <summary>
        /// The visible area of the world in the form of a rectangle
        /// </summary>
        RectangleF VisibleArea
        {
            get;
        }

        /// <summary>
        /// The current view transformation matrix for sprite batches
        /// </summary>
        Matrix CurrentViewMatrix
        {
            get;
        }
    }
}
