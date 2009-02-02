using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.TilingEngine;
using Microsoft.Xna.Framework;

namespace WesternSpace.Collision
{
    // ICollidable is a framwork to add actions when objects collide
    public interface ITileCollidable
    {
        List<CollisionHotspot> Hotspots
        {
            get;
        }

        World World
        {
            get;
        }

        Vector2 Position
        {
            get;
            set;
        }
    }
}
