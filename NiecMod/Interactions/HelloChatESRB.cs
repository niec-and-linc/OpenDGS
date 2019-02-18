/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 07/10/2018
 * Time: 8:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;

using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Socializing;

using Sims3.SimIFace;

using Sims3.UI;

using NiecMod.Nra;

namespace NiecMod.Interactions
{
    public sealed class HelloChatESRB : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            Sim targetsim = GetSelectedObject() as Sim;
            if (targetsim != null)
            {
                foreach (InteractionInstance interactionInstance in Target.InteractionQueue.InteractionList) // Cant Cancel Fix
                {
                    interactionInstance.MustRun = false;
                    interactionInstance.Hidden = false;
                }
                foreach (InteractionInstance interactionInstance in targetsim.InteractionQueue.InteractionList) // Cant Cancel Fix
                {
                    interactionInstance.MustRun = false;
                    interactionInstance.Hidden = false;
                }
                SpeedTrap.Sleep(0u);
                
                Target.InteractionQueue.CancelAllInteractions();
                targetsim.InteractionQueue.CancelAllInteractions();
                SpeedTrap.Sleep(0u);
                targetsim.EnableInteractions();
                SpeedTrap.Sleep(0u);
                InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.ESRB);
                if (AcceptCancelDialog.Show("Do you want Run Divorce? (Yes Run or No Next)", true))
                {
                    InteractionInstance helloChatESRBi;
                    helloChatESRBi = new SocialInteractionA.Definition("Divorce", null, null, false).CreateInstance(Target, targetsim, priority, false, false);
                    //helloChatESRBi = new SituationSocial.Definition("Fight!", null, null, false).CreateInstance(Target, targetsim, new InteractionPriority(InteractionPriorityLevel.ESRB), false, false);
                    helloChatESRBi.MustRun = true;
                    helloChatESRBi.Hidden = false;
                    targetsim.InteractionQueue.AddNext(helloChatESRBi);
                }
                //
                targetsim.SocialComponent.AddRelationshipUpdate(Target, CommodityTypes.Insulting, -100f, -100f);
                InteractionInstance helloChatESRBi2;
                helloChatESRBi2 = new SocialInteractionA.Definition("Argue", null, null, false).CreateInstance(Target, targetsim, priority, false, false);
                //helloChatESRBi = new SituationSocial.Definition("Fight!", null, null, false).CreateInstance(Target, targetsim, new InteractionPriority(InteractionPriorityLevel.ESRB), false, false);
                helloChatESRBi2.MustRun = true;
                //helloChatESRBi2.Hidden = false;
                targetsim.InteractionQueue.Add(helloChatESRBi2);
                targetsim.InteractionQueue.Add(helloChatESRBi2);
                targetsim.InteractionQueue.Add(helloChatESRBi2);
                targetsim.InteractionQueue.Add(helloChatESRBi2);
                targetsim.InteractionQueue.Add(helloChatESRBi2);
                targetsim.InteractionQueue.Add(helloChatESRBi2);
                //
                InteractionInstance helloChatESRBi3;
                helloChatESRBi3 = new SocialInteractionA.Definition("Declare Nemesis", null, null, false).CreateInstance(Target, targetsim, priority, false, false);
                //helloChatESRBi = new SituationSocial.Definition("Fight!", null, null, false).CreateInstance(Target, targetsim, new InteractionPriority(InteractionPriorityLevel.ESRB), false, false);
                helloChatESRBi3.MustRun = true;
                //helloChatESRBi3.Hidden = false;
                targetsim.InteractionQueue.Add(helloChatESRBi3);
                Sim.ForceSocial(targetsim, Target, "Declare Nemesis", (InteractionPriorityLevel)12, false);
                //
                InteractionInstance helloChatESRBi4;
                helloChatESRBi4 = new SocialInteractionA.Definition("Fight!", null, null, false).CreateInstance(Target, targetsim, priority, false, false);
                //helloChatESRBi = new SituationSocial.Definition("Fight!", null, null, false).CreateInstance(Target, targetsim, new InteractionPriority(InteractionPriorityLevel.ESRB), false, false);
                helloChatESRBi4.MustRun = true;
                //helloChatESRBi4.Hidden = false;
                targetsim.InteractionQueue.Add(helloChatESRBi4);
                Sim.ForceSocial(targetsim, Target, "Fight!", (InteractionPriorityLevel)12, false);
            }
            return true;
        }

        [DoesntRequireTuning]

        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, HelloChatESRB>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "Hello Chat Interaction Priority Level ESRB";
            }

            public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
            {
                NumSelectableRows = 1;
                Sim actor = parameters.Target as Sim;
                List<Sim> sims = new List<Sim>();

                foreach (Sim sim in actor.LotCurrent.GetSims())
                {
                    sims.Add(sim);
                }

                base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
            }


            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                return true;
            }
            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
        }
    }
}
