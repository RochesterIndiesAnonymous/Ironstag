using System;
using Microsoft.Xna.Framework;


namespace WesternSpace.ServiceInterfaces
{
    public interface IScreenResolutionService
    {
        Rectangle ScaleRectangle { get; set; }
        int StartTextureHeight { get; set; }
        int StartTextureWidth { get; set; }
        int ScaleFactor { get; }
    }
}
