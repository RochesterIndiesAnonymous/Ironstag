using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WesternSpace.Utility;
using WesternSpace.Screens;
using Microsoft.Xna.Framework;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EBossJumpState : EBossState
    {
        /// <summary>
        /// How high above the boss the player needs to be to choose this
        /// AI state
        /// </summary>
        private const float PLAYER_Y_THRESHOLD = 80.0f;

        private Timer jumpTimer;

        private Vector2 lastJumpPosition;

        internal EBossJumpState(Screen parentScreen, EBoss boss)
            : base(boss)
        {
            jumpTimer = new Timer(parentScreen, 2000);
            jumpTimer.TimeHasElapsed += new EventHandler<EventArgs>(jumpTimer_TimeHasElapsed);

            lastJumpPosition = new Vector2();
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
            if (this.Boss.CurrentState.Contains("Idle") && this.Boss.isOnGround && this.Boss.World.Player.isOnGround 
                && lastJumpPosition != this.Boss.Position && this.Boss.World.Player.Position.Y < (this.Boss.Position.Y - PLAYER_Y_THRESHOLD))
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
            if (this.Boss.CurrentState.Contains("Idle"))
            {
                this.Boss.ApplyJump();
                this.Boss.ChangeState("JumpingAscent");
                this.Boss.isOnGround = false;

                lastJumpPosition = this.Boss.Position;
            }
        }

        private void jumpTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            JumpUp();

            jumpTimer.PauseTimer();
            jumpTimer.ResetTimer();
        }
    }
}
