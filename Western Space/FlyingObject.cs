using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace WesternSpace
{
    class FlyingObject : GameObject
    {
        //Constants
        const int FLYING_SPEED = 120;
        const int MOVE_UP = -1;
        const int MOVE_DOWN = 1;
        const int MOVE_LEFT = -1;
        const int MOVE_RIGHT = 1;

        //State
        enum State
        {
            Flying
        }

        //Class Variables
        State currentState = State.Flying;
        Vector2 direction = Vector2.Zero;
        Vector2 speed = Vector2.Zero;
        SoundEffect sideFx;
        SoundEffect topFx;

        public FlyingObject(Game1 game, String contentName, Vector2 initPos, Vector2 initDir, 
            SoundEffect top, SoundEffect side)
            : base(game, contentName)
        {
            sideFx = side;
            topFx = top;

            position = initPos;
            direction = initDir;

            if (direction.X != 0)
            {
                speed.X = FLYING_SPEED;
            }
            if (direction.Y != 0)
            {
                speed.Y = FLYING_SPEED;
            }
        }

        public void UpdateMovement(GameTime gameTime, int viewWidth, int viewHeight)
        {
            if (currentState == State.Flying)
            {
                if (position.X >= (viewWidth - this.size.Width) )
                {
                    direction.X = MOVE_LEFT;

                    if (this.Effect == SpriteEffects.None)
                    {
                        this.Effect = SpriteEffects.FlipHorizontally;
                        sideFx.Play();
                    }
                    else
                    {
                        this.Effect = SpriteEffects.None;
                        sideFx.Play();
                    }

                }
                else if (position.X <= 0)
                {
                    direction.X = MOVE_RIGHT;

                    if (this.Effect == SpriteEffects.None)
                    {
                        this.Effect = SpriteEffects.FlipHorizontally;
                        sideFx.Play();
                    }
                    else
                    {
                        this.Effect = SpriteEffects.None;
                        sideFx.Play();

                    }
                }
                if (position.Y >= viewHeight - this.size.Height)
                {
                    direction.Y = MOVE_UP;
                    topFx.Play();
                }
                else if (position.Y <= 0)
                {
                    direction.Y = MOVE_DOWN;
                    topFx.Play();
                }

                base.Update(gameTime, speed, direction);
            }
        }
    }
}
