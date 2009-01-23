using System;
using Microsoft.Xna.Framework;


namespace WesternSpace.ServiceInterfaces
{
    public interface IScreenResolutionService
    {
        Rectangle ScaleRectangle { get; set; }
        int StartTextureHeight { get; set; }
        int StartTextureWidth { get; set; }

        float AspectRatio { get; }
        int AspectRatioNumerator { get; }
        int AspectRatioDenominator { get; }
        int ScaleFactor { get; }
    }
}
