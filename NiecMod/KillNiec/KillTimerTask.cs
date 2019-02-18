using System;
using System.Collections.Generic;
using System.Text;
using Sims3.NiecHelp.Tasks;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Abstracts;
using Sims3.SimIFace;

namespace NiecMod.KillNiec
{
    public class KillTimerTask : KillTask
    {
        public KillTimerTask(Sim target)
            : base(target)
        { }

        public KillTimerTask(Sim target, SimDescription.DeathType deathType)
            : base(target, deathType)
        { }

        public KillTimerTask(Sim target, SimDescription.DeathType deathType, GameObject obj)
            : base(target, deathType, obj)
        { }

        public KillTimerTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim)
            : base(target, deathType, obj, playDeathAnim)
        { }

        public KillTimerTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes)
            : base(target, deathType, obj, playDeathAnim, sleepyes)
        { }

        protected override void OnStart()
        {
            if (Simulator.CheckYieldingContext(false))
            {
                Simulator.Sleep(1000);
            }
        }

    }
}
