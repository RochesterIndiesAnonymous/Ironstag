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

        /// <summary>
        /// The amount of time to wait before executing the jump decision
        /// in milliseconds
        /// </summary>
        private const int DELAY_BETWEEN_JUMP_DECISION = 2000;

        /// <summary>
        /// The amount of time we turn off tile collision when the boss jumps down
        /// </summary>
        private const int AMOUNT_OF_TIME_TO_TURN_OFF_TILE_COLLISION_DOWN = 500;

        /// <summary>
        /// The amount of time we turn off tile collision when the boss jumps up
        /// </summary>
        private const int AMOUNT_OF_TIME_TO_TURN_OFF_TILE_COLLISION_UP = 500;

        private Timer jumpTimer;
        private Timer fallThroughTimer;
        private Timer jumpThroughTimer;

        private Vector2 lastJumpPosition;

        internal EBossJumpState(Screen parentScreen, EBoss boss)
            : base(boss)
        {
            jumpTimer = new Timer(parentScreen, DELAY_BETWEEN_JUMP_DECISION);
            jumpTimer.TimeHasElapsed += new EventHandler<EventArgs>(jumpTimer_TimeHasElapsed);

            fallThroughTimer = new Timer(parentScreen, AMOUNT_OF_TIME_TO_TURN_OFF_TILE_COLLISION_DOWN);
            fallThroughTimer.TimeHasElapsed += new EventHandler<EventArgs>(fallThroughTimer_TimeHasElapsed);

            jumpThroughTimer = new Timer(parentScreen, AMOUNT_OF_TIME_TO_TURN_OFF_TILE_COLLISION_UP);
            jumpThroughTimer.TimeHasElapsed += new EventHandler<EventArgs>(jumpThroughTimer_TimeHasElapsed);

            lastJumpPosition = new Vector2();
        }

        internal override void Update()
        {
            base.Update();

            jumpTimer.ResetTimer();
            jumpTimer.ResumeTimer();

            fallThroughTimer.ResetTimer();
            fallThroughTimer.PauseTimer();

            jumpThroughTimer.ResetTimer();
            jumpThroughTimer.PauseTimer();
            
            this.IsLogicComplete = false;
        }

        public void StartTimers()
        {
            jumpTimer.PauseTimer();
            jumpTimer.StartTimer();

            fallThroughTimer.PauseTimer();
            fallThroughTimer.StartTimer();

            jumpThroughTimer.PauseTimer();
            jumpThroughTimer.StartTimer();
        }

        public void StopTimers()
        {
            jumpTimer.TimeHasElapsed -= jumpTimer_TimeHasElapsed;
            jumpTimer.PauseTimer();
            jumpTimer.RemoveTimer();

            fallThroughTimer.TimeHasElapsed -= fallThroughTimer_TimeHasElapsed;
            fallThroughTimer.PauseTimer();
            fallThroughTimer.RemoveTimer();

            jumpThroughTimer.TimeHasElapsed -= jumpThroughTimer_TimeHasElapsed;
            jumpThroughTimer.PauseTimer();
            jumpThroughTimer.RemoveTimer();
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
                lastJumpPosition = this.Boss.Position;

                this.Boss.ApplyJump();
                this.Boss.ChangeState("JumpingAscent");
                this.Boss.isOnGround = false;

                this.Boss.TileCollisionDetectionEnabled = false;

                jumpThroughTimer.ResetTimer();
                jumpThroughTimer.ResumeTimer();
            }
        }

        private void JumpDown()
        {
            if (this.Boss.CurrentState.Contains("Idle"))
            {
                lastJumpPosition = this.Boss.Position;

                this.Boss.ApplyJump();
                this.Boss.ChangeState("JumpingAscent");
                this.Boss.isOnGround = false;

                this.Boss.TileCollisionDetectionEnabled = false;

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
            this.Boss.TileCollisionDetectionEnabled = true;

            fallThroughTimer.PauseTimer();
            fallThroughTimer.ResetTimer();
        }

        void jumpThroughTimer_TimeHasElapsed(object sender, EventArgs e)
        {
            this.Boss.TileCollisionDetectionEnabled = true;

            jumpThroughTimer.PauseTimer();
            jumpThroughTimer.ResetTimer();
        }
    }
}
