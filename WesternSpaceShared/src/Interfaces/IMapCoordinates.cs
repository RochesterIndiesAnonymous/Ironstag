using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WesternSpace.Interfaces
{
    /// <summary>
    /// Interface that is used to calculate the coordinates of a map
    /// </summary>
    public interface IMapCoordinates
    {
        /// <summary>
        /// The minimum x value that the map supports
        /// </summary>
        float MinimumX
        {
            get;
        }

        /// <summary>
        /// The maximum x value that the map supports
        /// </summary>
        float MaximumX
        {
            get;
        }

        /// <summary>
        /// The minimum y value that the map supports
        /// </summary>
        float MinimumY
        {
            get;
        }

        /// <summary>
        /// The maximum y value that the map supports
        /// </summary>
        float MaximumY
        {
            get;
        }

        /// <summary>
        /// Calculates the map coordinates based on the point given
        /// </summary>
        /// <param name="atPoint">The point that needs to be converted to map coordinates</param>
        /// <returns>The coordinates of the point within the map</returns>
        Vector2 CalculateMapCoordinatesFromScreenPoint(Vector2 atPoint);
    }


}
