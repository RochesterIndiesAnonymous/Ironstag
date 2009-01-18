using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace WesternSpace.AnimationFramework
{
    /// <summary>
    /// Provides a base class to create animated components to be drawn in the world
    /// </summary>
    public abstract class AnimatedComponent : DrawableGameObject
    {
        /// <summary>
        /// The data that contains sequences and sprite sheet data.
        /// </summary>
        private AnimationData animationData;

        protected AnimationData AnimationData
        {
            get { return animationData; }
            set { animationData = value; }
        }

        private string animationKey;

        /// <summary>
        /// The name of the currently playing animation in the sequence dictionary.
        /// </summary>
        public string AnimationKey
        {
            get { return animationKey; }
            set { animationKey = value; }
        }

        private Frame currentFrame;

        /// <summary>
        /// The current frame that is to be drawn to the screen
        /// </summary>
        protected Frame CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        private int amountOfTimeDisplayed;

        /// <summary>
        /// The amount of time in milliseconds that the current frame has been displaying
        /// </summary>
        protected int AmountOfTimeDisplayed
        {
            get { return amountOfTimeDisplayed; }
        }

        /// <summary>
        /// True if the current frame has been displaying for as long as it is specified to. False otherwise.
        /// </summary>
        protected bool IsTimeForNextFrame
        {
            get
            {
                return (amountOfTimeDisplayed >= currentFrame.DisplayTimeInMilliseconds);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">The Game object to be passed to the GameComponent class</param>
        /// <param name="data">The data that this component will use for animation</param>
        public AnimatedComponent(Game game, AnimationData data)
            : base(game)
        {
            this.animationData = data;
            this.animationKey = String.Empty;
            this.amountOfTimeDisplayed = 0;
            this.currentFrame = new Frame(0, 0, 0);
        }

        /// <summary>
        /// Returns a rectangle suitable for drawing in a sprite batch
        /// </summary>
        /// <param name="index">The index of the sprite in the sprite sheet to be drawn</param>
        /// <returns>A rectangle with the proper dimensions to draw the sprite in a sprite batch.</returns>
        protected Rectangle CalculateFrameRectangleFromIndex(int index)
        {
            // this should be a whole number, right?
            int framesPerRow = animationData.SpriteSheet.Width / animationData.FrameWidth;

            return new Rectangle((index % framesPerRow) * animationData.FrameWidth, (index / framesPerRow) * animationData.FrameHeight, animationData.FrameWidth, animationData.FrameHeight);
        }

        /// <summary>
        /// Resets the time that the sprite has been displayed
        /// </summary>
        protected void ResetTime()
        {
            amountOfTimeDisplayed = 0;
        }

        /// <summary>
        /// Moves the animation sequence to the next frame.
        /// </summary>
        protected void IncrementFrame()
        {
            IList<Frame> currentSequence = animationData.Sequences[animationKey];
            currentFrame = currentSequence[(currentFrame.FrameIndex + 1) % currentSequence.Count];
            amountOfTimeDisplayed = 0;
        }

        /// <summary>
        /// Sets the next animation draw cycle to a specific frame in the current sequence
        /// </summary>
        /// <param name="frameIndex">the index of the frame to be drawn</param>
        protected void SetFrame(int frameIndex)
        {
            SetFrame(this.animationKey, frameIndex);
        }

        /// <summary>
        /// Sets the next animation draw cycle to a specific frame
        /// </summary>
        /// <param name="animationKey">The key to the sequence of frames</param>
        /// <param name="frameIndex">The index of the frame to be drawn</param>
        protected void SetFrame(string animationKey, int frameIndex)
        {
            this.animationKey = animationKey;
            currentFrame = this.animationData.Sequences[animationKey][frameIndex];
        }

        /// <summary>
        /// Updates the amount of time the current frame has been displayed
        /// </summary>
        /// <param name="gameTime">The time information relative to the game and real time</param>
        public override void Update(GameTime gameTime)
        {
            amountOfTimeDisplayed += gameTime.ElapsedGameTime.Milliseconds;

            base.Draw(gameTime);
        }
    }
}
