using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WesternSpace.Utility;

namespace WesternSpace.Screens
{
    public class StoryboardInformation
    {
        private Texture2D sceneTexture;

        public Texture2D SceneTexture
        {
            get { return sceneTexture; }
        }

        private string sceneText;

        public string SceneText
        {
            get { return sceneText; }
            set { sceneText = value; }
        }

        private Vector2 sceneTextPosition;

        public Vector2 SceneTextPosition
        {
            get { return sceneTextPosition; }
        }

        private bool isTransitionAutomatic;

        public bool IsTransitionAutomatic
        {
            get { return isTransitionAutomatic; }
        }

        private bool isReadyToTransition;

        public bool IsReadyToTransition
        {
            get { return isReadyToTransition; }
        }

        private bool isTimerStarted;

        public bool IsTimerStarted
        {
            get { return isTimerStarted; }
        }

        private Timer timer;

        public StoryboardInformation(Screen screen, Texture2D texture, string text, Vector2 textPosition, bool isTransitionAutomatic, int automaticTransitionDelay)
        {
            this.sceneTexture = texture;
            this.sceneText = text;
            this.sceneTextPosition = textPosition;
            this.isTransitionAutomatic = isTransitionAutomatic;
            isTimerStarted = false;

            if (isTransitionAutomatic)
            {
                timer = new Timer(screen, automaticTransitionDelay);
                timer.TimeHasElapsed += new EventHandler<EventArgs>(timer_TimeHasElapsed);
            }
        }

        public void StartTimer()
        {
            if (timer != null)
            {
                timer.ResetTimer();
                timer.StartTimer();

                isTimerStarted = true;
            }
        }

        public void StopTimer()
        {
            if (timer != null)
            {
                timer.RemoveTimer();
                this.isTimerStarted = false;
            }
        }

        void timer_TimeHasElapsed(object sender, EventArgs e)
        {
            isReadyToTransition = true;
        }
    }
}
