using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using WesternSpace.DrawableComponents.Projectiles;
using WesternSpace.Screens;
using WesternSpace.Utility;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EBossShootState : EBossState
    {
        private const int NUMBER_OF_BULLETS_TO_GENERATE = 6;

        /// <summary>
        /// How high above or below the boss the player needs to be to choose this
        /// a shoot up or shoot down state
        /// </summary>
        private const float PLAYER_Y_THRESHOLD = 40.0f;

        /// <summary>
        /// The sound to play when the gun fires
        /// </summary>
        SoundEffect gunShot;

        /// <summary>
        /// The timer used to time the shooting of the boss
        /// </summary>
        private Timer shootTimer;

        private Timer delayBetweenBullet;

        private Timer laughTimer;

        private Random random = new Random();

        private bool isReadyToShoot;

        private short currentBulletCount;

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

            shootTimer = new Timer(parentScreen, 3000);
            shootTimer.TimeHasElapsed += new EventHandler<EventArgs>(shootTimer_TimeHasElapsed);

            delayBetweenBullet = new Timer(parentScreen, 50);
            delayBetweenBullet.TimeHasElapsed += new EventHandler<EventArgs>(delayBetweenBullet_TimeHasElapsed);

            laughTimer = new Timer(parentScreen, 3000);
            laughTimer.TimeHasElapsed += new EventHandler<EventArgs>(laughTimer_TimeHasElapsed);

            isReadyToShoot = true;
            hasTimerStarted = false;
            currentBulletCount = 0;
        }

        internal override void Update()
        {
            base.Update();
            this.isReadyToShoot = false;
            this.IsLogicComplete = false;

            shootTimer.PauseTimer();
            shootTimer.ResetTimer();

            delayBetweenBullet.ResetTimer();
            delayBetweenBullet.ResumeTimer();
        }

        internal void StartTimer()
        {
            delayBetweenBullet.PauseTimer();
            delayBetweenBullet.StartTimer();
            
            laughTimer.PauseTimer();
            laughTimer.StartTimer();

            shootTimer.StartTimer();

            hasTimerStarted = true;
        }

        internal void StopTimer()
        {
            shootTimer.TimeHasElapsed -= shootTimer_TimeHasElapsed;
            delayBetweenBullet.TimeHasElapsed -= delayBetweenBullet_TimeHasElapsed;
            laughTimer.TimeHasElapsed -= laughTimer_TimeHasElapsed;

            shootTimer.PauseTimer();
            delayBetweenBullet.PauseTimer();
            laughTimer.PauseTimer();

            shootTimer.RemoveTimer();
            delayBetweenBullet.RemoveTimer();
            laughTimer.RemoveTimer();

            hasTimerStarted = false;
        }

        /// <summary>
        /// Causes the boss to generate a projectile and change its state accordingly.
        /// </summary>
        private void Shoot()
        {
            if (!this.Boss.CurrentState.Contains("Dead") && !this.Boss.CurrentState.Equals("Hit") && this.Boss.isOnGround)
            {
                Vector2 projectileVelocityVector;

                if (this.Boss.World.Player.Position.Y < (this.Boss.Position.Y - PLAYER_Y_THRESHOLD))
                {
                    // player is above the boss
                    this.Boss.ChangeState("ShootingUp");
                    projectileVelocityVector = new Vector2(1.0f, -2.0f);
                }
                else if (this.Boss.World.Player.Position.Y > (this.Boss.Position.Y + PLAYER_Y_THRESHOLD))
                {
                    // player is below the boss
                    this.Boss.ChangeState("ShootingDown");
                    projectileVelocityVector = new Vector2(1.0f, 2.0f);
                }
                else if (this.Boss.CurrentState.Contains("Running"))
                {
                    //Change state and animation
                    this.Boss.ChangeState("RunningShooting");
                    projectileVelocityVector = new Vector2(1.0f, 0.0f);
                }
                else
                {
                    // player is on the same level as the boss
                    this.Boss.ChangeState("Shooting");
                    projectileVelocityVector = new Vector2(1.0f, 0.0f);
                }

                gunShot.Play();

                //Generate a Bullet
                GenerateBullet(projectileVelocityVector);
            }
        }

        /// <summary>
        /// Creates a bullet object which travels in a straight line.
        /// </summary>
        private void GenerateBullet(Vector2 projectileVelocityVector)
        {
            short direction = 1;

            int moveBulletDelta = random.Next(5) - random.Next(10);

            Vector2 position = this.Boss.Position + new Vector2(60f + moveBulletDelta, 0f + moveBulletDelta);

            if (this.Boss.Facing == SpriteEffects.FlipHorizontally)
            {
                direction = -1;
                position = this.Boss.Position + new Vector2(-60f + moveBulletDelta, 0f + moveBulletDelta);
            }

            if (this.Boss.CurrentState.Contains("ShootingDown"))
            {
                if (direction == -1)
                {
                    position = this.Boss.Position + new Vector2(-50f + moveBulletDelta, 20f + moveBulletDelta);
                }
                else
                {
                    position = this.Boss.Position + new Vector2(50f + moveBulletDelta, 20f + moveBulletDelta);
                }
            }
            else if (this.Boss.CurrentState.Contains("ShootingUp"))
            {
                if (direction == -1)
                {
                    position = this.Boss.Position + new Vector2(-50f + moveBulletDelta, -50f + moveBulletDelta);
                }
                else
                {
                    position = this.Boss.Position + new Vector2(50f + moveBulletDelta, -50f + moveBulletDelta);
                }
            }

            Vector2 finalVector = new Vector2(BossProjectile.Velocity.X * projectileVelocityVector.X, BossProjectile.Velocity.Y + projectileVelocityVector.Y);

            BossProjectile proj = new BossProjectile(this.Boss.World, this.Boss.SpriteBatch, position, BossProjectile.Texture, direction, finalVector, this, DamageCategory.Player, BossProjectile.Damage);
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
        /// Raised when the shoot animation is ready to generate another bullet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void delayBetweenBullet_TimeHasElapsed(object sender, EventArgs e)
        {
            Shoot();

            delayBetweenBullet.ResetTimer();
            currentBulletCount++;

            if (currentBulletCount == NUMBER_OF_BULLETS_TO_GENERATE)
            {
                delayBetweenBullet.PauseTimer();
                delayBetweenBullet.ResetTimer();

                currentBulletCount = 0;

                this.Boss.ChangeState("Laughing");

                laughTimer.ResetTimer();
                laughTimer.ResumeTimer();
            }
        }

        void laughTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            laughTimer.PauseTimer();
            laughTimer.ResetTimer();

            this.shootTimer.ResetTimer();
            this.shootTimer.ResumeTimer();
            this.Boss.ChangeState("Idle");

            this.IsLogicComplete = true;
        }
    }
}
