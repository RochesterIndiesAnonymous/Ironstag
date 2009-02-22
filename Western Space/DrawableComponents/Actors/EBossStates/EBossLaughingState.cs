using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WesternSpace.DrawableComponents.Actors.EBossStates
{
    internal class EBossLaughingState : EBossState
    {
        internal EBossLaughingState(EBoss boss)
            : base(boss)
        {

        }

        internal override void Update()
        {
            base.Update();

            this.Boss.ChangeState("Laughing");
        }
    }
}
