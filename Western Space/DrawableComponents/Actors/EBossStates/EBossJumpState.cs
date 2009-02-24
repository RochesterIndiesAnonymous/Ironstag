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
        public const int FALL_THROUGH_FLOOR_UPDATE_CYCLES = 20;

        private bool fallThroughFloor;

        public bool FallThroughFloor
        {
            get { return fallThroughFloor; }
            set { fallThroughFloor = value; }
        }

        private int currentUpdateCyclesSkipped;

        public int CurrentUpdateCyclesSkipped
        {
            get { return currentUpdateCyclesSkipped; }
            set { currentUpdateCyclesSkipped = value; }
        }

        private Timer jumpTimer;
        private Timer fallThroughTimer;

        private Vector2 lastJumpPosition;

        internal EBossJumpState(Screen parentScreen, EBoss boss)
            : base(boss)
        {
            jumpTimer = new Timer(parentScreen, 2000);
            jumpTimer.TimeHasElapsed += new EventHandler<EventArgs>(jumpTimer_TimeHasElapsed);

            fallThroughTimer = new Timer(parentScreen, 100);
            fallThroughTimer.TimeHasElapsed += new EventHandler<EventArgs>(fallThroughTimer_TimeHasElapsed);

            lastJumpPosition = new Vector2();
        }

        internal override void Update()
        {
            base.Update();

            jumpTimer.ResetTimer();
            jumpTimer.ResumeTimer();

            fallThroughTimer.ResetTimer();
            fallThroughTimer.PauseTimer();
            
            this.IsLogicComplete = false;
        }

        public void StartTimers()
        {
            jumpTimer.PauseTimer();
            jumpTimer.StartTimer();

            fallThroughTimer.PauseTimer();
            fallThroughTimer.StartTimer();
        }

        public void StopTimers()
        {
            jumpTimer.TimeHasElapsed -= jumpTimer_TimeHasElapsed;
            jumpTimer.PauseTimer();
            jumpTimer.RemoveTimer();

            fallThroughTimer.TimeHasElapsed -= fallThroughTimer_TimeHasElapsed;
            fallThroughTimer.PauseTimer();
            fallThroughTimer.RemoveTimer();
        }

        internal bool ShouldBossJump()
        {
            if (this.Boss.CurrentState.Contains("Idle") && this.Boss.isOnGround && this.Boss.World.Player.isOnGround 
                && lastJumpPosition != this.Boss.Position && 
                ((this.Boss.World.Player.Position.Y < (this.Boss.Position.Y - PLAYER_Y_THRESHOLD)) ||
                (this.Boss.World.Player.Position.Y > (this.Boss.Position.Y + PLAYER_Y_THRESHOLD))))
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

        private void JumpDown()
        {
            if (this.Boss.CurrentState.Contains("Idle"))
            {
                this.FallThroughFloor = true;
                lastJumpPosition = this.Boss.Position;

                this.Boss.ApplyJump();
                this.Boss.ChangeState("JumpingAscent");
                this.Boss.isOnGround = false;

                fallThroughTimer.ResetTimer();
                fallThroughTimer.ResumeTimer();
            }
        }

        private void jumpTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            if (this.Boss.World.Player.Position.Y < (this.Boss.Position.Y - PLAYER_Y_THRESHOLD))
            {
                JumpUp();
            }
            else if (this.Boss.World.Player.Position.Y > (this.Boss.Position.Y + PLAYER_Y_THRESHOLD))
            {
                JumpDown();
            }
            else
            {
                lastJumpPosition = this.Boss.Position;
                this.IsLogicComplete = true;
            }

            jumpTimer.PauseTimer();
            jumpTimer.ResetTimer();
        }

        void fallThroughTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            this.Boss.Velocity = new Vector2(0f, 0f);

            jumpTimer.PauseTimer();
            jumpTimer.ResetTimer();
        }
    }
}
