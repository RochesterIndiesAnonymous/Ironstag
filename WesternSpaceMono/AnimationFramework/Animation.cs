﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using WesternSpace.ServiceInterfaces;

namespace WesternSpace.AnimationFramework
{
    // Represents an animated texture and loads essential frame
    // data from an XML file when it is created.
    public class Animation
    {
        // Holds the sprite sheet texture for this Animation. The
        // sprite sheet contains all of the frames of the animation
        // on a single image.
        Texture2D spriteSheet;

        public Texture2D SpriteSheet
        {
            get { return spriteSheet; }
        }
        public Color[] textureColorData
        {
            get
            {
                Color [] newColorData = new Color[frameWidth * frameHeight];
                spriteSheet.GetData(newColorData, 0, (frameWidth * frameHeight));
                return newColorData;
            }
        }        
        public Color[] GetHorizontalTextureDataFlip()
        {
            Color[] oldColorData = textureColorData;
            Color[] newColorData = new Color[frameWidth * frameHeight];
            int x, y;
            for (y = 0; y < frameHeight; y++)
            {
                for (x = 0; x < frameWidth; x++)
                {
                    newColorData[y * frameWidth + (frameWidth - 1 - x)] = oldColorData[y * frameWidth + x];
                }
            }
            return newColorData;
        }

        private int centerOffsetX;
        public int CenterOffsetX
        {
            get { return centerOffsetX; }
        }

        private int centerOffsetY;
        public int CenterOffsetY
        {
            get { return centerOffsetY; }
        }

        public Vector2 CenterOffset
        {
            get { return new Vector2(centerOffsetX, centerOffsetY); }
        }

        // A collection containing all of the frames to be used
        // in this Animation.
        private List<Frame> frames;

        public List<Frame> Frames
        {
            get { return frames; }
            set { frames = value; }
        }

        // Determines if the animation should continue
        // playing from the beginning or end when it 
        // finishes.
        bool isLooping;

        public bool IsLooping
        {
            get { return isLooping; }
        }

        /// <summary>
        /// Determines if the animation is the default 
        /// animation or not.
        /// </summary>
        bool isDefaultAnimation;

        public bool IsDefaultAnimation
        {
            get { return isDefaultAnimation; }
        }

        // The number of frames contained in this animation.
        private int frameCount;

        public int FrameCount
        {
            get { return frameCount; }
            set { frameCount = value; }
        }

        // The width of the frames contained in this
        // animation.
        private int frameWidth;

        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }

        // The height of the frames contained in
        // this animation.
        int frameHeight;

        public int FrameHeight
        {
            get { return frameHeight; }
            set { frameHeight = value; }
        }

        /// <summary>
        /// The string representation of this animation.
        /// </summary>
        public string animationName;

        /// <summary>
        /// The name of the animation that is this animation's parent.
        /// </summary>
        public string parentName;

        public string ParentName
        {
            get { return parentName; }
            set { parentName = value; }
        }

        /// <summary>
        /// The animation to be played after this animation is finished.
        /// </summary>
        public Animation parentAnimation;

        public Animation ParentAnimation
        {
            get { return parentAnimation; }
            set { parentAnimation = value; }
        }

        // Constructor for a new Animation.
        // param: animationXmlFile - The XML file to load Frame information
        //                           from.
        public Animation(string animationXmlFile, string animationName)
        {
            this.animationName = animationName;
            this.spriteSheet = null;
            this.frames = new List<Frame>();
            this.frameHeight = 0;
            this.frameWidth = 0;
            this.frameCount = 0;
            this.isLooping = true;
            parentAnimation = null;

            LoadAnimationXmlFile(animationXmlFile, animationName);
        }

        /// <summary>
        /// Sets the parent animation of this animation to the desired animation.
        /// </summary>
        /// <param name="parent"></param>
        public void setParentAnimation(Animation parent)
        {
            parentAnimation = parent;
        }

        // Loads an animation XML file.
        // param: fileName - The XML file to load Frame information from.
        // param: animationName - The name of the animation you wish to pull from the XML file.
        private void LoadAnimationXmlFile(string fileName, string animationName)
        {
            //Create a new XDocument from the given file name.
            XDocument fileContents = ScreenManager.Instance.Content.Load<XDocument>(fileName);

            IEnumerable<XElement> allAnimations = from animations in fileContents.Descendants("Animation")
                                                  select animations;

            foreach (XElement animation in allAnimations)
            {
                string animationKey = animation.Attribute("AnimationKey").Value;

                //Checks to see if this Animation matches the desired animation
                if (animation.Attribute("AnimationKey").Value == animationName)
                {
                    //Read the XML Attribute "SpriteSheet" from the XML file and save Texture.
                    String spriteSheetName = animation.Attribute("SpriteSheet").Value;
                    ITextureService textureService = (ITextureService)ScreenManager.Instance.Services.GetService(typeof(ITextureService));
                    spriteSheet = textureService.GetTexture(spriteSheetName);

                    //Parse the frame width and height from the XML file and save the values.
                    this.frameWidth = Int32.Parse(animation.Attribute("FrameWidth").Value);
                    this.frameHeight = Int32.Parse(animation.Attribute("FrameHeight").Value);

                    //Sets if this animation is to loop or not.
                    int loopInt = 0;
                    loopInt = Int32.Parse(animation.Attribute("IsLoop").Value);

                    //Gets the Offset of the center to the top left corner of this animation
                    centerOffsetX = Int32.Parse(animation.Attribute("CenterOffsetX").Value);

                    centerOffsetY = Int32.Parse(animation.Attribute("CenterOffsetY").Value);

                    //"Convert" the ints to booleans
                    if (loopInt == 0)
                    {
                        this.isLooping = false;
                    }
                    else
                    {
                        this.isLooping = true;
                    }

                    this.parentName= animation.Attribute("Parent").Value;

                    //Creates a list of frames and populates this Animation's list.
                    XElement[] frames = animation.Descendants("Frame").ToArray();
                    for (int i = 0; i < frames.Length; i++)
                    {
                        int sheetIndex = 0;
                        sheetIndex = Int32.Parse(frames[i].Attribute("SheetIndex").Value);

                        int displayTimeInMs = 0;
                        displayTimeInMs = Int32.Parse(frames[i].Attribute("DisplayTime").Value);

                        this.frames.Add(new Frame(sheetIndex, displayTimeInMs, i));
                    }
                }
            }

            //Sets the number of frames of this animation.
            this.frameCount = this.frames.Count();
        }
    }
}