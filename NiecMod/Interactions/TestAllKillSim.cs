/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 23/09/2018
 * Time: 9:48
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using NRaas.CommonSpace.Helpers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Sims3.Gameplay;
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
using NiecMod.Interactions.Hidden;
using NiecMod.Nra;
using NiecMod.KillNiec;
using Sims3.NiecHelp.Tasks;
using Niec.iCommonSpace;

namespace NiecMod.Interactions
{
    public sealed class TestAllKillSim : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run() // Run
        {
        	if (Autonomous || Actor.IsNPC) return false;
        	if (!AcceptCancelDialog.Show("Are You Sure All Kill Sim?")) return false;
            
            List<Sim> list = new List<Sim>();
            foreach (Sim sim in LotManager.Actors)
            {
                //if (sim.SimDescription.ToddlerOrAbove && !sim.IsInActiveHousehold && sim.LotCurrent != Household.ActiveHousehold.LotHome) //OK
                //if (!sim.IsInActiveHousehold || !(sim.Service is GrimReaper)) //Failed
                //if (sim.IsNPC && !sim.IsInActiveHousehold) //OK
                //if (sim.IsNPC && !sim.IsInActiveHousehold || !(sim.Service is GrimReaper)) // Failed All Sim Not If ||
                if (!(sim.Service is GrimReaper)) // OK
                {
                    //sim.InteractionQueue.AddNext(NotKillSimNPCOnly.Singleton.CreateInstance(sim, sim, new InteractionPriority((InteractionPriorityLevel)12, 1f), false, true));
                    //SpeedTrap.Sleep(1);
                    //if (!AcceptCancelDialog.Show("Done?")) return false;
                    //sim.InteractionQueue.Add(CCnlean.Singleton.CreateInstance(Actor, sim, new InteractionPriority((InteractionPriorityLevel)1, 0f), false, true));
                    list.Add(sim);
                }
            }
            if (list.Count > 0)
            {
                foreach (Sim nlist in list)
                {
                    try
                    {
                        //Name is
                        if (nlist.SimDescription.FirstName == "Death" && nlist.SimDescription.LastName == "Good System")
                        {
                            continue;
                        }

                        if (nlist.SimDescription.FirstName == "Good System" && nlist.SimDescription.LastName == "Death Helper")
                        {
                            continue;
                        }

                        if (nlist.SimDescription.FirstName == "Grim" && nlist.SimDescription.LastName == "Reaper")
                        {
                            continue;
                        }
                    }
                    catch (NullReferenceException)
                     { }
                    //nlist.BuffManager.RemoveAllElements();
                    /*
                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                    listr.Add(SimDescription.DeathType.Drown);
                    listr.Add(SimDescription.DeathType.Starve);
                    listr.Add(SimDescription.DeathType.Thirst);
                    listr.Add(SimDescription.DeathType.Burn);
                    listr.Add(SimDescription.DeathType.Freeze);
                    listr.Add(SimDescription.DeathType.ScubaDrown);
                    listr.Add(SimDescription.DeathType.Shark);
                    listr.Add(SimDescription.DeathType.Jetpack);
                    listr.Add(SimDescription.DeathType.Meteor);
                    listr.Add(SimDescription.DeathType.Causality);
                    listr.Add(SimDescription.DeathType.Electrocution);
                    if (Actor.SimDescription.Elder)
                    {
                        listr.Add(SimDescription.DeathType.OldAge);
                    }
                    listr.Add(SimDescription.DeathType.HauntingCurse);
                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);
                    //Urnstones.CreateGrave(nlist.SimDescription, randomObjectFromList, true, true);
                     */
                    //KillTask kt = new KillTask(Target, RandomUtil.CoinFlip() ? KillTask.GetDGSDeathType(Target) : KillTask.GetDeathType(Target), null, false, false);
                    //kt.AddToSimulator();
                    KillPro.FastKill(nlist, RandomUtil.CoinFlip() ? KillTask.GetDGSDeathType(Target) : KillTask.GetDeathType(Target), Actor, true, false);
                }
                //nlist.InteractionQueue.CancelAllInteractionsByType(NotKillSimNPCOnly.Singleton);
            }
            return true;
        }

        [DoesntRequireTuning]
        
        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, TestAllKillSim>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "All Kill Sim By Niec";
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;
                //if (actor.IsNPC) return false;

                return true;
            }
            public override string[] GetPath(bool bPath)
            {
                return new string[] { "DGS: Target Kill..." };
            }
            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
        }
    }
}
