using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.Screens
{
    public class ScreenTransitionState
    {
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

        public float FadeAlphaStep
        {
            get { return fadeAlphaStep; }
        }

        private float brightenAlphaStep;

        public float BrightenAlphaStep
        {
            get { return brightenAlphaStep; }
        }

        private ScreenTransitionStateProgess currentProgress;

        public ScreenTransitionStateProgess CurrentProgress
        {
            get { return currentProgress; }
            set { currentProgress = value; }
        }

        private float currentAlphaValue;

        public float CurrentAlphaValue
        {
            get { return currentAlphaValue; }
            set { currentAlphaValue = value; }
        }

        private bool resetGame;

        public bool ResetGame
        {
            get { return resetGame; }
            set { resetGame = value; }
        }

        public ScreenTransitionState(string fromScreenName, string toScreenName, float fadeAlphaStep, float brightenAlphaStep, bool resetGame)
        {
            this.fromScreenName = fromScreenName;
            this.toScreenName = toScreenName;
            this.fadeAlphaStep = fadeAlphaStep;
            this.brightenAlphaStep = brightenAlphaStep;
            this.currentAlphaValue = 1.0f;
            this.currentProgress = ScreenTransitionStateProgess.Fading;
            this.resetGame = resetGame;
        }
    }
}
