using System;
using System.Collections.Generic;
using System.Text;
using Sims3.NiecHelp.Tasks;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;
using Niec.iCommonSpace;




namespace Sims3.NiecHelp.Tasks
{
    public class KillAnnihilationTask : KillTask
    {
        public KillAnnihilationTask(Sim target)
            : base(target)
        { }

        public KillAnnihilationTask(Sim target, SimDescription.DeathType deathType)
            : base(target, deathType)
        { }

        public KillAnnihilationTask(Sim target, SimDescription.DeathType deathType, GameObject obj)
            : base(target, deathType, obj)
        { }

        public KillAnnihilationTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim)
            : base(target, deathType, obj, playDeathAnim)
        { }

        public KillAnnihilationTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes)
            : base(target, deathType, obj, playDeathAnim, sleepyes)
        { }

        public KillAnnihilationTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes, bool wasisCrib)
            : base(target, deathType, obj, playDeathAnim, sleepyes, wasisCrib)
        { }

        protected override void OnStart()
        {
            IfMineKill = false;
            KillPro.MineKill(mtarget, mdeathType, mobj, mplayDeathAnim, msleepyes, mwasisCrib);
        }
    }
}
