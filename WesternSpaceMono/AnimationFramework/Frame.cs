using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.AnimationFramework
{
    /// <summary>
    /// Holds information pertainent to the this specific frame in the animation sequence
    /// </summary>
    public struct Frame
    {
        private int sheetIndex;

        /// <summary>
        /// The index of the sprite to display in the sprite sheet
        /// </summary>
        public int SheetIndex
        {
            get { return sheetIndex; }
            set { sheetIndex = value; }
        }

        private int displayTimeInMilliseconds;

        /// <summary>
        /// The amount of time in milliseconds to display the sprite
        /// </summary>
        public int DisplayTimeInMilliseconds
        {
            get { return displayTimeInMilliseconds; }
            set { displayTimeInMilliseconds = value; }
        }

        private int frameIndex;

        /// <summary>
        /// The index of this frame in the animation sequence
        /// </summary>
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        /// <summary>
        /// True if this frame has been drawn. False otherwise.
        /// </summary>
        public bool hasDrawn;

        public bool HasDrawn
        {
            get { return hasDrawn; }
            set { hasDrawn = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sheetIndex">The index of the sprite to display in the sprite sheet</param>
        /// <param name="displayTime">The amount of time in milliseconds to display the sprite</param>
        /// <param name="frameIndex">The index of this frame in the animation sequence</param>
        public Frame(int sheetIndex, int displayTime, int frameIndex)
        {
            this.sheetIndex = sheetIndex;
            this.displayTimeInMilliseconds = displayTime;
            this.frameIndex = frameIndex;
            this.hasDrawn = false;
        }
    }
}
