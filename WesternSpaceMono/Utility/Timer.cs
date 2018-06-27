using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using WesternSpace.Screens;

namespace WesternSpace.Utility
{
    public class Timer : GameObject
    {
        public event EventHandler<EventArgs> TimeHasElapsed;

        private int timeToWait;
        private int elaspedTime;
        private bool paused;

        public Timer(Screen parentScreen, int timeToWait)
            : base(parentScreen)
        {
            paused = false;
            elaspedTime = 0;
            this.timeToWait = timeToWait;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!paused)
            {
                elaspedTime += gameTime.ElapsedGameTime.Milliseconds;

                if (elaspedTime >= timeToWait)
                {
                    EventHandler<EventArgs> handler = TimeHasElapsed;

                    if (handler != null)
                    {
                        TimeHasElapsed(this, EventArgs.Empty);
                    }
                }
            }
        }

        public void StartTimer()
        {
            this.ParentScreen.Components.Add(this);
        }

        public void RemoveTimer()
        {
            this.ParentScreen.Components.Remove(this);
        }

        public void PauseTimer()
        {
            this.paused = true;
        }

        public void ResumeTimer()
        {
            this.paused = false;
        }

        public void ResetTimer()
        {
            this.elaspedTime = 0;
        }
    }
}
