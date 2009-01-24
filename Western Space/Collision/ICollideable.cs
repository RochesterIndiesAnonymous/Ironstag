using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Collision
{
    public interface ICollideable
    {
        CollisionHotspot[] Hotspots
        {
            get;
        }
        void SpriteTileCollision();
        void SpriteSpriteCollision();
    }
}
