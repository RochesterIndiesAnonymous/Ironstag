using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.DrawableComponents.Projectiles;
using WesternSpace.Utility;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EBossShootState : EBossState
    {
        /// <summary>
        /// The sound to play when the gun fires
        /// </summary>
        SoundEffect gunShot;

        /// <summary>
        /// The timer used to time the shooting of the boss
        /// </summary>
        private Timer shootTimer;

        private Timer shootDurationTimer;

        private bool isReadyToShoot;

        public bool IsReadyToShoot
        {
            get { return isReadyToShoot; }
        }

        private bool hasTimerStarted;

        public bool HasTimerStarted
        {
            get { return hasTimerStarted; }
        }

        internal EBossShootState(EBoss boss, Screen parentScreen)
            : base(boss)
        {
            gunShot = ScreenManager.Instance.Content.Load<SoundEffect>("System\\Sounds\\flintShot");

            shootTimer = new Timer(parentScreen, 6000);
            shootTimer.TimeHasElapsed += new EventHandler<EventArgs>(shootTimer_TimeHasElapsed);

            shootDurationTimer = new Timer(parentScreen, 3000);
            shootDurationTimer.TimeHasElapsed += new EventHandler<EventArgs>(shootDurationTimer_TimeHasElapsed);

            isReadyToShoot = true;
            hasTimerStarted = false;
        }

        internal override void Update()
        {
            base.Update();

            Shoot();
        }

        internal void StartTimer()
        {
            shootTimer.StartTimer();
            shootDurationTimer.StartTimer();
            hasTimerStarted = true;
        }

        internal void StopTimer()
        {
            shootTimer.TimeHasElapsed -= shootTimer_TimeHasElapsed;
            shootDurationTimer.TimeHasElapsed -= shootDurationTimer_TimeHasElapsed;

            shootTimer.PauseTimer();
            shootDurationTimer.PauseTimer();

            shootTimer.RemoveTimer();
            shootDurationTimer.RemoveTimer();
            hasTimerStarted = false;
        }

        /// <summary>
        /// Causes the boss to generate a projectile and change its state accordingly.
        /// </summary>
        private void Shoot()
        {
            if (!this.Boss.CurrentState.Contains("Dead") && !this.Boss.CurrentState.Equals("Hit"))
            {
                if (!this.Boss.CurrentState.Contains("Shooting"))
                {
                    if (this.Boss.CurrentState.Contains("Running"))
                    {
                        //Change state and animation
                        this.Boss.ChangeState("RunningShooting");
                    }
                    else
                    {
                        //Change state and animation
                        this.Boss.ChangeState("Shooting");
                    }

                    gunShot.Play();

                    shootDurationTimer.ResetTimer();
                    shootDurationTimer.ResumeTimer();

                    //Generate a Bullet
                    GenerateBullet();
                }
            }
        }

        /// <summary>
        /// Creates a bullet object which travels in a straight line.
        /// </summary>
        private void GenerateBullet()
        {
            short direction = 1;
            Vector2 position = this.Boss.Position + new Vector2(23f, -15f);

            if (this.Boss.Facing == SpriteEffects.FlipHorizontally)
            {
                direction = -1;
                position = this.Boss.Position + new Vector2(-23f, -15);
            }

            BanditNormalProjectile proj = new BanditNormalProjectile(this.Boss.World, this.Boss.SpriteBatch, position, this, direction);

            this.isReadyToShoot = false;
            this.shootTimer.ResumeTimer();
        }
        
        /// <summary>
        /// Raised when enough time has elapsed to allow the enemy to shoot.
        /// </summary>
        /// <param name="sender">The timer that raised this event</param>
        /// <param name="e">e is always EventArgs.Empty</param>
        private void shootTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            this.isReadyToShoot = true;
            this.shootTimer.PauseTimer();
            this.shootTimer.ResetTimer();
        }

        /// <summary>
        /// Raised when the shoot animation has occured for the specified time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void shootDurationTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            this.Boss.ChangeState("Idle");
            shootDurationTimer.PauseTimer();
            shootDurationTimer.ResetTimer();
        }
    }
}
