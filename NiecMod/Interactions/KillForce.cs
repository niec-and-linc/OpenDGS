/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 05/10/2018
 * Time: 1:29
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.CelebritySystem;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.InteractionsShared;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Academics;
using Sims3.Gameplay.Objects.Alchemy;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Objects.Environment;
using Sims3.Gameplay.Objects.Fishing;
using Sims3.Gameplay.Objects.FoodObjects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Objects.HobbiesSkills;
using Sims3.Gameplay.Objects.Insect;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Objects.Misc;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.Objects.Rewards;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.OnlineGiftingSystem;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.PetObjects;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Routing;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoreSystems;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.TuningValues;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.Gameplay.StoryProgression;

using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.SimIFace.Enums;
using Sims3.SimIFace.RouteDestinations;
using Sims3.SimIFace.SACS;

using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using Sims3.UI.Dialogs;
using Sims3.UI.Hud;
using Sims3.UI.Resort;

using NiecMod.KillNiec;
using NiecMod.Nra;
using Sims3.NiecHelp.Tasks;
using Niec.iCommonSpace;

namespace NiecMod.Interactions
{
    /// <summary>
    /// Summary of KillForce
    /// </summary>
    public sealed class KillForce : ImmediateInteraction<Sim, Sim>
    {
        public override bool Run()
        {
            var definition = base.InteractionDefinition as Definition;



            /*
            if (AcceptCancelDialog.Show("FastKill."))
            {
                if (KillPro.FastKill(Target, definition.death, null, true, false))
                //if (Target.Kill(definition.death))
                {
                    StyledNotification.Show(new StyledNotification.Format("Check Ok", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("Check Failed", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                return true;
            }
            */

            if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
            {
                /*
                if (AcceptCancelDialog.Show("KillTark"))
                {
                    KillTask kt = new KillTask(Target, definition.death, null, true, false);
                    kt.AddToSimulator();
                    StyledNotification.Show(new StyledNotification.Format("Check Ok", StyledNotification.NotificationStyle.kGameMessagePositive));
                    return true;
                }


                if (AcceptCancelDialog.Show("TimerTark"))
                {
                    KillTimerTask kt = new KillTimerTask(Target, definition.death, null, true, false);
                    kt.AddToSimulator();
                    StyledNotification.Show(new StyledNotification.Format("Check Ok", StyledNotification.NotificationStyle.kGameMessagePositive));
                    return true;
                }

                */


                //if (KillSimNiecX.MineKill(Target, definition.death, null, true, false))
                if (KillPro.FastKill(Target, definition.death, null, true, false))
                //if (Target.Kill(definition.death))
                {
                    StyledNotification.Show(new StyledNotification.Format("Check Ok", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("Check Failed", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                return true;
            }

            if (!Target.CanBeKilled()) // My Mod is CanBeKilled Not Modifed The Sims 3 is File Dll Gameplay 
            {
                if (!AcceptCancelDialog.Show("Check CanBeKilled is failed Run MineKill?")) return false;
                KillSimNiecX.MineKill(Target, definition.death);
                return true;
            }

            Target.Kill(definition.death);
            //KillSimNiecX.MineKill(Target, definition.death);
            return true;
        }

        public static readonly InteractionDefinition Singleton = new Definition();

        [DoesntRequireTuning]

        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, KillForce>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public Definition()
            {
            }

            public Definition(string text, string path, SimDescription.DeathType deathType)
            {
                this.MenuText = text;
                this.MenuPath = new string[]
					{
						"DGS: Target Kill...",
						path
					};
                this.death = deathType;
            }

            public override string[] GetPath(bool isFemale)
            {
                return this.MenuPath;
            }

            public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
            {
                if (actor.IsHuman || actor.IsPet)
                {
                    results.Add(new InteractionObjectPair(new Definition("Old Age with Anti-Unlucky", "Die With Anti Unlucky By Death Good System...", SimDescription.DeathType.OldAge), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Fire", "Die By Death Good System...", SimDescription.DeathType.Burn), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Electrocution", "Die By Death Good System...", SimDescription.DeathType.Electrocution), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Starvation", "Die By Death Good System...", SimDescription.DeathType.Starve), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Mummy's Curse", "Die By Death Good System...", SimDescription.DeathType.MummyCurse), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Meteor", "Die By Death Good System...", SimDescription.DeathType.Meteor), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Drowning", "Die By Death Good System...", SimDescription.DeathType.Drown), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Haunting Curse", "Die By Death Good System...", SimDescription.DeathType.HauntingCurse), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Transmuted", "Die By Death Good System...", SimDescription.DeathType.Transmuted), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Jelly Bean Death", "Die By Death Good System...", SimDescription.DeathType.JellyBeanDeath), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Thirst", "Die By Death Good System...", SimDescription.DeathType.Thirst), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Freezing", "Die By Death Good System...", SimDescription.DeathType.Freeze), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Blunt Force Trauma", "Die By Death Good System...", SimDescription.DeathType.BluntForceTrauma), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Ranting", "Die By Death Good System...", SimDescription.DeathType.Ranting), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Shark", "Die By Death Good System...", SimDescription.DeathType.Shark), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Scuba Drown", "Die By Death Good System...", SimDescription.DeathType.ScubaDrown), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Dehydration", "Die By Death Good System...", SimDescription.DeathType.MermaidDehydrated), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Jetpack (Fall)", "Die By Death Good System...", SimDescription.DeathType.Jetpack), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Causality", "Die By Death Good System...", SimDescription.DeathType.Causality), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("None Test", "Die By Death Good System...", SimDescription.DeathType.None), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Old Age (Good) Pet Only", "Die With Anti Unlucky By Death Good System...", SimDescription.DeathType.PetOldAgeGood), iop.Target));
                    results.Add(new InteractionObjectPair(new Definition("Old Age (Bad) Pet Only", "Die With Anti Unlucky By Death Good System...", SimDescription.DeathType.PetOldAgeBad), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return this.MenuText;
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {

                if (!Test(parameters.Actor as Sim, parameters.Target as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                {
                    return InteractionTestResult.Def_TestFailed;
                }
                return InteractionTestResult.Pass;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                return true;
            }

            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }

            public string MenuText = string.Empty;

            public string[] MenuPath;

            public SimDescription.DeathType death;
        }
    }
}