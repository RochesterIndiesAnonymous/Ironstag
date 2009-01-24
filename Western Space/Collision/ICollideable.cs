using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Collision
{
    // ICollidable is a framwork to add actions when objects collide
    public interface ICollideable
    {
        List<CollisionHotspot> Hotspots
        {
            get;
        }
        // Note May need to pass some variable like distance and which edge it collided with
        void SpriteTileCollision();
        void SpriteSpriteCollision();
    }
}
