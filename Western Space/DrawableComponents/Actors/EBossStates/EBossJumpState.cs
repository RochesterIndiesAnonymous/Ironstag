using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Utility;
using WesternSpace.Screens;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EBossJumpState : EBossState
    {
        /// <summary>
        /// How high above the boss the player needs to be to choose this
        /// AI state
        /// </summary>
        private const float PLAYER_Y_THRESHOLD = 60.0f;

        private Timer jumpTimer;

        internal EBossJumpState(Screen parentScreen, EBoss boss)
            : base(boss)
        {
            jumpTimer = new Timer(parentScreen, 2000);
            jumpTimer.TimeHasElapsed += new EventHandler<EventArgs>(jumpTimer_TimeHasElapsed);
        }

        internal override void Update()
        {
            base.Update();


            jumpTimer.ResetTimer();
            jumpTimer.ResumeTimer();

            this.IsLogicComplete = false;
        }

        public void StartTimers()
        {
            jumpTimer.PauseTimer();
            jumpTimer.StartTimer();
        }

        public void StopTimers()
        {
            jumpTimer.TimeHasElapsed -= jumpTimer_TimeHasElapsed;
            jumpTimer.PauseTimer();
            jumpTimer.RemoveTimer();
        }

        internal bool ShouldBossJumpUp()
        {
            if (!this.Boss.CurrentState.Contains("Shooting") && !this.Boss.CurrentState.Contains("Jumping") && !this.Boss.CurrentState.Contains("Running")
                && this.Boss.isOnGround && this.Boss.World.Player.Position.Y < (this.Boss.Position.Y - PLAYER_Y_THRESHOLD))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called from the AI. If the boss is already
        /// in a jumping state then no action is to occurr.
        /// </summary>
        private void JumpUp()
        {
            if (!this.Boss.CurrentState.Contains("Shooting") && !this.Boss.CurrentState.Contains("Dead") && !this.Boss.CurrentState.Equals("Hit"))
            {
                if (!this.Boss.CurrentState.Contains("Jumping") && !this.Boss.CurrentState.Contains("Falling"))
                {
                    this.Boss.ApplyJump();
                    this.Boss.ChangeState("JumpingAscent");
                    this.Boss.isOnGround = false;
                    this.IsLogicComplete = true;
                }
            }
        }

        private void jumpTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            JumpUp();
        }
    }
}
