using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.ServiceInterfaces;
using Microsoft.Xna.Framework;

namespace WesternSpace.Screens
{
    public class ScreenTransition
    {
        private Effect alphaEffect;

        private string fromScreenName;

        public string FromScreenName
        {
            get { return fromScreenName; }
        }

        private string toScreenName;

        public string ToScreenName
        {
            get { return toScreenName; }
        }

        private float fadeAlphaStep;

        private float brightenAlphaStep;

        private ScreenTransitionProgess currentProgress;

        public ScreenTransitionProgess CurrentProgress
        {
            get { return currentProgress; }
            set { currentProgress = value; }
        }

        private float currentAlphaValue;

        private bool resetGame;

        public bool ResetGame
        {
            get { return resetGame; }
        }

        private bool resetSpriteBatchService;

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

        public ScreenTransition(string fromScreenName, string toScreenName, float fadeAlphaStep, float brightenAlphaStep, bool resetGame, bool resetSpriteBatchService)
        {
            this.fromScreenName = fromScreenName;
            this.toScreenName = toScreenName;
            this.fadeAlphaStep = fadeAlphaStep;
            this.brightenAlphaStep = brightenAlphaStep;
            this.currentAlphaValue = 1.0f;
            this.currentProgress = ScreenTransitionProgess.Fading;
            this.resetGame = resetGame;
            this.resetSpriteBatchService = resetSpriteBatchService;

            alphaEffect = ScreenManager.Instance.Content.Load<Effect>("System\\Effects\\SetAlphaValue");
        }

        public void Update()
        {
            if (this.CurrentProgress == ScreenTransitionProgess.Fading && this.currentAlphaValue <= 0.0f)
            {
                this.CurrentProgress = ScreenTransitionProgess.Brightening;
                this.currentAlphaValue = 0.0f;

                Screen screenToRemove = (from sc in ScreenManager.Instance.ScreenList
                                         where sc.Name == this.FromScreenName
                                         select sc).First();

                ScreenManager.Instance.RemoveScreenFromDisplay(screenToRemove);

                if (this.resetGame)
                {
                    ScreenManager.Instance.ScreenList.Remove(screenToRemove);

                    Screen gameScreen = new GameScreen(ScreenManager.Instance, GameScreen.ScreenName);
                    ScreenManager.Instance.ScreenList.Add(gameScreen);

                    ICameraService camera = (ICameraService)ScreenManager.Instance.Services.GetService(typeof(ICameraService));
                    camera.Position = new Vector2(0, 0);

                }

                if (this.resetSpriteBatchService)
                {
                    ScreenManager.Instance.UseSpriteBatchService = true;
                }

                Screen screenToAdd = (from sc in ScreenManager.Instance.ScreenList
                                      where sc.Name == this.ToScreenName
                                      select sc).First();

                screenToAdd.Enabled = false;
                ScreenManager.Instance.AddScreenToDisplay(screenToAdd);
            }

            if (this.IsTransitionComplete)
            {
                this.EnableNewScreen();
            }
        }

        public void BeginTransition()
        {
            alphaEffect.Parameters["AlphaValue"].SetValue(this.currentAlphaValue);

            alphaEffect.Begin();
            alphaEffect.CurrentTechnique.Passes[0].Begin();
        }

        public void EndTransition()
        {
            alphaEffect.CurrentTechnique.Passes[0].End();
            alphaEffect.End();

            if (this.CurrentProgress == ScreenTransitionProgess.Fading)
            {
                this.currentAlphaValue -= this.fadeAlphaStep;
            }
            else if (this.CurrentProgress == ScreenTransitionProgess.Brightening)
            {
                this.currentAlphaValue += this.brightenAlphaStep;
            }
        }

        private void EnableNewScreen()
        {
            Screen screenToAdd = (from sc in ScreenManager.Instance.ScreenList
                                  where sc.Name == this.ToScreenName
                                  select sc).First();

            screenToAdd.Enabled = true;
        }
    }
}
