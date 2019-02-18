using Sims3.Gameplay.Academics;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NiecMod.Interactions;

namespace Sims3.Gameplay.Interactions
{
    [Persistable]
    public abstract partial class InteractionInstance : IInteractionInstance
    {
        public void CallCallbackOnFailure(Sim actor)
        {
            if (!this.mbOnStopCalled)
            {
                this.mbOnStopCalled = true;
                if (actor != null)
                {
                    try
                    {
                        if (actor.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton)) return;

                        if (!(actor.Service is Sims3.Gameplay.Services.GrimReaper))
                        {
                            foreach (InteractionInstance interactionInstance in actor.InteractionQueue.InteractionList)
                            {
                                if (interactionInstance.GetPriority().Level > (InteractionPriorityLevel)100)
                                {
                                    return;
                                }
                            }
                        }

                    }
                    catch
                    { }

                    /*
                    foreach (InteractionInstance interactionInstance in this.InstanceActor.InteractionQueue.InteractionList)
                    {
                        if (interactionInstance is ExtKillSimNiec)
                        {
                            return;
                        }
                    }
                     * */
                    this.InteractionObjectPair.CallbackOnFailure(actor);
                    actor.OnInteractionEnded(false);
                }
            }
        }
    }
}
