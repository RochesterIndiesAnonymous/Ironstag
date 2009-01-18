﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace WesternSpace.AnimationFramework
{
    /// <summary>
    /// Stores essential data about a set of animations for a single sprite sheet.
    /// </summary>
    public struct AnimationData
    {
        private Texture2D spriteSheet;

        /// <summary>
        /// The texture the contains the sprite sheet for this animation sequence
        /// </summary>
        public Texture2D SpriteSheet
        {
            get { return spriteSheet; }
            set { spriteSheet = value; }
        }

        private int frameWidth;

        /// <summary>
        /// The width of each frame in the sprite sheet
        /// </summary>
        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        private int frameHeight;

        /// <summary>
        /// The height of each frame in the sprite sheet
        /// </summary>
        public int FrameHeight
        {
            get { return frameHeight; }
            set { frameHeight = value; }
        }

        private IDictionary<string, IList<Frame>> sequences;

        /// <summary>
        /// Collection of sequences that this sprite sheet supports.
        /// </summary>
        public IDictionary<string, IList<Frame>> Sequences
        {
            get { return sequences; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="spriteSheet">The texure to use for the sprite sheet</param>
        /// <param name="frameWidth">The width of each sprite frame</param>
        /// <param name="frameHeight">The height of each sprite frame</param>
        /// <param name="animationXmlFile">The file name to load the animation sequences from</param>
        public AnimationData(Game game, Texture2D spriteSheet, int frameWidth, int frameHeight, string animationXmlFile)
        {
            this.spriteSheet = spriteSheet;
            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
            this.sequences = new Dictionary<string, IList<Frame>>();

            LoadAnimationXmlFile(game, animationXmlFile);
        }

        private void LoadAnimationXmlFile(Game game, string fileName)
        {
            XDocument fileContents = game.Content.Load<XDocument>(fileName);

            IEnumerable<XElement> allSequences = from sequences in fileContents.Descendants("Sequence")
                                                 select sequences;

            foreach (XElement sequence in allSequences)
            {
                string animationKey = sequence.Attribute("AnimationKey").Value;

                this.sequences[animationKey] = new List<Frame>();

                XElement[] frames = sequence.Descendants("Frame").ToArray();
                for (int i = 0; i < frames.Length; i++)
                {
                    int sheetIndex = 0;
                    Int32.TryParse(frames[i].Attribute("SheetIndex").Value, out sheetIndex);

                    int displayTimeInMs = 0;
                    Int32.TryParse(frames[i].Attribute("DisplayTime").Value, out displayTimeInMs);

                    this.sequences[animationKey].Add(new Frame(sheetIndex, displayTimeInMs, i));
                }
            }
        }
    }
}
