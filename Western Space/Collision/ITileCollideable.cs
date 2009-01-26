using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.TilingEngine;
using Microsoft.Xna.Framework;

namespace WesternSpace.Collision
{
    // ICollidable is a framwork to add actions when objects collide
    public interface ITileCollideable
    {
        List<CollisionHotspot> Hotspots
        {
            get;
        }
        Vector2 OnTileColision(Tile tile, CollisionHotspot hotSpot, Rectangle tileRectangle);
    }
}
