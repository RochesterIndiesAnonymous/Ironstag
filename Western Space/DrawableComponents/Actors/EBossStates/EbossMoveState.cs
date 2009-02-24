using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EbossMoveState : EBossState
    {
        /// <summary>
        /// How far away the player needs to be to choose
        /// to move or not
        /// </summary>
        private const float PLAYER_X_THRESHOLD = 180.0f;

        internal EbossMoveState(EBoss boss)
            : base(boss)
        {

        }

        internal override void Update()
        {
            base.Update();

            if (this.Boss.World.Player.Position.X > (this.Boss.Position.X + PLAYER_X_THRESHOLD) ||
               this.Boss.World.Player.Position.X < (this.Boss.Position.X - PLAYER_X_THRESHOLD))
            {
                Move();
            }
            else
            {
                this.Boss.ChangeState("Idle");
                IsLogicComplete = true;
            }
        }

        internal bool ShouldBossMove()
        {
            if (this.Boss.World.Player.Position.X > (this.Boss.Position.X + PLAYER_X_THRESHOLD) ||
               this.Boss.World.Player.Position.X < (this.Boss.Position.X - PLAYER_X_THRESHOLD))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called from the AI to move the boss
        /// </summary>
        private void Move()
        {
            int direction = 1;

            if (!this.Boss.CurrentState.Contains("Dead") && !this.Boss.CurrentState.Equals("Hit"))
            {
                //Calculate Facing
                if (this.Boss.collideableFacing == SpriteEffects.None)
                {
                    direction = 1;
                }
                else
                {
                    direction = -1;
                }

                if (this.Boss.isOnGround)
                {
                    this.Boss.ApplyGroundMove(direction);
                    if (!this.Boss.CurrentState.Contains("Shooting"))
                    {
                        this.Boss.ChangeState("Running");
                    }
                }
                else
                {
                    this.Boss.ApplyAirMove(direction);
                }

                IsLogicComplete = true;
            }
        }
    }
}
