using System;
using Microsoft.Xna.Framework;


namespace WesternSpace.ServiceInterfaces
{
    public interface IScreenResolutionService
    {
        Rectangle ScaleRectangle { get; }
        int StartTextureHeight { get; }
        int StartTextureWidth { get; }
        int ScaleFactor { get; }
    }
}
