using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.AnimationFramework
{
    // Handles the drawing and display of a character object's animations.
    public class AnimationPlayer
    {
        // The currently playing animation.
        Animation animation;

        public Animation Animation
        {
            get { return animation; }
        }

        // The frame that is currently being drawn to the screen.
        private Frame currentFrame;

        public Frame CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        // The amount of time in milliseconds that the current frame has been shown for.
        private int timeDisplayed;

        private int TimeDisplayed
        {
            get { return timeDisplayed; }
        }


        // Constructor for an Animation Player. Sets up the initial paramters such as
        // this player's animation, current frame, and time displayed.
        // param: game - The over-arching Game object.
        // param: spriteBatch - The spriteBatch used to draw the Animation.
        // param: animation - The Animation to play.
        public AnimationPlayer(SpriteBatch spriteBatch, Animation animation)
        {
            this.animation = animation;
            this.timeDisplayed = 0;
            this.currentFrame = animation.Frames[0];

        }

        // True if the current frame has been displaying for as long as it is specified to. False otherwise.
        private bool IsTimeForNextFrame
        {
            get
            {
                return (timeDisplayed >= currentFrame.DisplayTimeInMilliseconds);
            }
        }

        // Returns a rectangle suitable for drawing in a sprite batch
        // param: index - The index of the sprite in the sprite sheet to be drawn
        // returns: A rectangle with the proper dimensions to draw the sprite in a sprite batch.
        private Rectangle CalculateFrameRectangleFromIndex(int index)
        {
            int framesPerRow = animation.SpriteSheet.Width / animation.FrameWidth;

            return new Rectangle((index % framesPerRow) * animation.FrameWidth, (index / framesPerRow) * animation.FrameHeight, animation.FrameWidth, animation.FrameHeight);
        }

        // Resets the time that the sprite has been displayed back to zero.
        private void ResetTime()
        {
            timeDisplayed = 0;
        }

        // Changes the Animation's frame to the next frame in the sequence.
        private void IncrementFrame()
        {
            List<Frame> currentAnimation = animation.Frames;

            if (Animation.IsLooping)
            {
                currentFrame = currentAnimation[(currentFrame.FrameIndex + 1) % currentAnimation.Count];
                ResetTime();
            }
            else
            {
                currentFrame = currentAnimation[Math.Min(currentFrame.FrameIndex + 1, currentAnimation.Count - 1)];
                ResetTime();
            }
        }

        // Sets the next animation draw cycle to a specific frame in the current sequence
        // param: frameIndex - the index of the Frame to be drawn
        private void SetFrame(int frameIndex)
        {
            currentFrame = Animation.Frames[frameIndex];
        }


        // Begins or continues playback of an animation.
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation.
            this.animation = animation;
            currentFrame = this.animation.Frames[0];
            ResetTime();
        }

        public void Update(GameTime gameTime)
        {
            timeDisplayed += gameTime.ElapsedGameTime.Milliseconds;

            if (this.IsTimeForNextFrame)
            {
                this.IncrementFrame();
            }
        }

        // Advances the time position and draws the current frame of the animation.
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");

            spriteBatch.Draw(Animation.SpriteSheet, position,
                this.CalculateFrameRectangleFromIndex(this.CurrentFrame.SheetIndex), Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffect, 0);
        }
    }
}