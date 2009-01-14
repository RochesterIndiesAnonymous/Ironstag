using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Interfaces
{
    interface IMapCoordinates
    {
        Vector2 CalculateMapCoordinatesFromMouse(Vector2 atPoint);

        bool IsValidCameraPosition(Vector2 position);
    }


}
