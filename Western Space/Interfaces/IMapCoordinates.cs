using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Interfaces
{
    interface IMapCoordinates
    {
        float MinimumX
        {
            get;
        }

        float MaximumX
        {
            get;
        }

        float MinimumY
        {
            get;
        }

        float MaximumY
        {
            get;
        }

        Vector2 CalculateMapCoordinatesFromMouse(Vector2 atPoint);
    }


}
