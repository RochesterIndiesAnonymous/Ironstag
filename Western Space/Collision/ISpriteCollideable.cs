using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.DrawableComponents.Actors;
using WesternSpace.AnimationFramework;
using Microsoft.Xna.Framework.Graphics;

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
        SpriteEffects collideableFacing
        {
            get;
            set;
        }
        Boolean removeFromRegistrationList
        {
            get;
            set;
        }
        void OnSpriteCollision(ISpriteCollideable characterCollidedWith);
    }
}
