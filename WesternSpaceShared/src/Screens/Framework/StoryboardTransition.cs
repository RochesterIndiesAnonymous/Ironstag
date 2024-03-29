﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.Screens
{
    internal class StoryboardTransition
    {
        private Effect alphaEffect;

        private float fadeAlphaStep;

        private float brightenAlphaStep;

        private ScreenTransitionProgess currentProgress;

        public ScreenTransitionProgess CurrentProgress
        {
            get { return currentProgress; }
            set { currentProgress = value; }
        }

        private float currentAlphaValue;

        private StoryboardScreen screen;

        public bool IsTransitionComplete
        {
            get
            {
                if (this.CurrentProgress == ScreenTransitionProgess.Brightening && this.currentAlphaValue >= 1.0f)
                {
                    return true;
                }

                return false;
            }
        }

        internal StoryboardTransition(StoryboardScreen screen, float fadeAlphaStep, float brightenAlphaStep)
        {
            this.fadeAlphaStep = fadeAlphaStep;
            this.brightenAlphaStep = brightenAlphaStep;
            this.currentAlphaValue = 1.0f;
            this.currentProgress = ScreenTransitionProgess.Fading;
            this.screen = screen;
        
            // alphaEffect = ScreenManager.Instance.Content.Load<Effect>("System\\Effects\\SetAlphaValue");
            // need to recompile shaders for mongame3.8 brett 9/6/22
            byte[] bytecode = File.ReadAllBytes( "Content/System/Effects/SetAlphaValue.mgfxd");
            alphaEffect = new Effect(screen.GraphicsDevice, bytecode);
        }

        public void Update()
        {
            if (this.CurrentProgress == ScreenTransitionProgess.Fading && this.currentAlphaValue <= 0.0f)
            {
                this.CurrentProgress = ScreenTransitionProgess.Brightening;
                this.currentAlphaValue = 0.0f;

                screen.CurrentScene = screen.CurrentScene.Next;
            }
        }

        public Effect GetEffect()
        {
           return alphaEffect;
        }

        public void BeginTransition()
        {
            
          alphaEffect.Parameters["AlphaValue"].SetValue(this.currentAlphaValue);
           // alphaEffect.CurrentTechnique.Passes[0].Apply();
            //  alphaEffect.Begin();
            //alphaEffect.CurrentTechnique.Passes[0].Begin();
        }

        public void EndTransition()
        {
            // alphaEffect.CurrentTechnique.Passes[0].End();
            // alphaEffect.End();
           
            if (this.CurrentProgress == ScreenTransitionProgess.Fading)
            {
                this.currentAlphaValue -= this.fadeAlphaStep;
            }
            else if (this.CurrentProgress == ScreenTransitionProgess.Brightening)
            {
                this.currentAlphaValue += this.brightenAlphaStep;
            }
           
        }
    }
}
