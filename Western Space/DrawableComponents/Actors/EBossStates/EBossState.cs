using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EBossState
    {
        private EBoss boss;

        protected EBoss Boss
        {
            get { return boss; }
        }

        protected EBossState(EBoss boss)
        {
            this.boss = boss;
        }

        internal virtual void Update()
        {
            if (boss.World.Player.Position.X < boss.Position.X)
            {
                boss.collideableFacing = SpriteEffects.FlipHorizontally;
            }
            else
            {
                boss.collideableFacing = SpriteEffects.None;
            }
        }
    }
}
