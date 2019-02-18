/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 29/09/2018
 * Time: 4:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.Gameplay.Tutorial;
using NiecMod.Nra;

namespace NiecMod.Interactions
{
    public sealed class ResetIntroTutorial : Interaction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run()
        {
            IntroTutorial.StartGameplayTutorial(Actor);
            Actor.Motives.MaxEverything();
            //SpeedTrap.Sleep(0u);
            IntroTutorial.ForceExitTutorial();
            return true;
        }

        [DoesntRequireTuning]

        private sealed class Definition : InteractionDefinition<Sim, Sim, ResetIntroTutorial>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "Reset Tutorial :)";
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
            public override string[] GetPath(bool bPath)
            {
                return new string[] { "NiecMod..." };
            }
        }
    }
}
