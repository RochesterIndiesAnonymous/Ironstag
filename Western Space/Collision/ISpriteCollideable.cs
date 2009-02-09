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
    // Use this interface for any Objects you want to collide with others
    public interface ISpriteCollideable
    {
        // Collision ID's used for tracking the last cells that the
        // Sprite Occupied
        int IdNumber
        {
            set;
            get;
        }
        // Rectangle for the space that the object takes up
        Rectangle Rectangle
        {
            get;
        }
        // Was used to find out the direction the sprite was
        // pointing in so i coul get the correct data dor pixel collision
        // So if a sprite was flipped horizontally i could call the correct
        // function the get the flipped spite color data.
        SpriteEffects collideableFacing
        {
            get;
            set;
        }
        // Boolean flag used to remove a object from the collision manager
        // entirely
        Boolean removeFromRegistrationList
        {
            get;
            set;
        }
        // This is called when a sprite collides with another sprite
        void OnSpriteCollision(ISpriteCollideable characterCollidedWith);
    }
}
