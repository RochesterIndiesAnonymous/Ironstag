using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;

namespace WesternSpace.Collision
{
    public interface ISpriteCollideable
    {
        int IdNumber
        {
            set;
            get;
        }
        Rectangle Rectangle
        {
            get;
        }
        void OnSpriteCollision(ISpriteCollideable characterCollidedWith);
    }
}
