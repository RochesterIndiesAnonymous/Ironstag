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
        // -- Constants -- //

        /// <summary>
        /// The default first frame of an animation.
        /// </summary>
        const int DEFAULT_START_FRAME = 0;

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

        /// <summary>
        /// The index which indicates which frame to begin playing on.
        /// </summary>
        private int playStart;

        /// <summary>
        /// The index which indicates which frame to stop playing on.
        /// </summary>
        private int playStop;


        // Constructor for an Animation Player. Sets up the initial paramters such as
        // this player's animation, current frame, and time displayed.
        // param: game - The over-arching Game object.
        // param: spriteBatch - The spriteBatch used to draw the Animation.
        // param: animation - The Animation to play.
        /// <summary>
        /// Constructor for an Animation Player. Sets up the initial parameters, including
        /// the player's animation, current frame, and time displayed.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch used to draw this animation.</param>
        /// <param name="animation">The desired animation to begin playing.</param>
        public AnimationPlayer(SpriteBatch spriteBatch, Animation animation)
        {
            this.animation = animation;
            this.timeDisplayed = 0;
            this.currentFrame = animation.Frames[DEFAULT_START_FRAME];
            playStart = DEFAULT_START_FRAME;
            playStop = animation.FrameCount - 1;


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
                currentFrame = currentAnimation[(currentFrame.FrameIndex + 1) % (playStop+1)];
                ResetTime();
            }
            else
            {
                currentFrame = currentAnimation[Math.Min(currentFrame.FrameIndex + 1, playStop)];

                if (currentFrame.FrameIndex != playStop)
                {
                    ResetTime();
                }
            }
        }

        // Sets the next animation draw cycle to a specific frame in the current sequence
        // param: frameIndex - the index of the Frame to be drawn
        public void SetFrame(int frameIndex)
        {
            currentFrame = Animation.Frames[frameIndex];
        }

        /// <summary>
        /// Begins or continues playback of an animation from the beginning to
        /// the end of an animation.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        public void PlayAnimation(Animation animation)
        {
            // If this animation is already running, do not restart it.
            if (Animation == animation)
                return;

            // Start the new animation.
            this.animation = animation;
            currentFrame = this.animation.Frames[0];
            playStart = DEFAULT_START_FRAME;
            playStop = this.animation.FrameCount - 1;
            ResetTime();
        }

        /// <summary>
        /// Begins or continues playback of an animation from the desired
        /// start and end frames.
        /// </summary>
        /// <param name="animation">The animation to play.</param>
        /// <param name="start">The frame index to start animating from.</param>
        /// <param name="end">The frame index to end animating on.</param>
        public void PlayAnimation(Animation newAnimation, int start, int end)
        {
            // If this animation is already running, do not restart it.
            if (Animation == newAnimation)
                return;

            if ((start >= 0) && (start <= animation.FrameCount - 1) &&
                (end >= 0) && (end <= newAnimation.FrameCount - 1))
            {
                playStart = start;
                playStop = end;
            }
            else
            {
                playStart = DEFAULT_START_FRAME;
                playStop = newAnimation.FrameCount - 1;
            }

            // Start the new animation.
            this.animation = newAnimation;
            currentFrame = this.animation.Frames[0];
            ResetTime();
        }

        // Returns true if the current animation is finished playing.
        public bool isDonePlaying()
        {
            bool returnValue = false;

            if( (currentFrame.FrameIndex == playStop) && !animation.IsLooping && this.IsTimeForNextFrame )
            {
                returnValue = true;
            }

            return returnValue;
        }

        public void Update(GameTime gameTime)
        {
            timeDisplayed += gameTime.ElapsedGameTime.Milliseconds;

            if (this.IsTimeForNextFrame)
            {
                this.IncrementFrame();
            }

            //If the Animation is finished and was a oneshot, play the default animation
            if (Animation.IsOneShot && isDonePlaying())
            {
                PlayAnimation(animation.parentAnimation, currentFrame.FrameIndex, animation.parentAnimation.FrameCount - 1);
            }
        }

        // Advances the time position and draws the current frame of the animation.
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect)
        {
            if (Animation == null)
            {
                throw new NotSupportedException("No animation is currently playing.");
            }

            spriteBatch.Draw(Animation.SpriteSheet, position,
                this.CalculateFrameRectangleFromIndex(this.CurrentFrame.SheetIndex), Color.White, 0.0f, Vector2.Zero, 1.0f, spriteEffect, 0);
        }
    }
}