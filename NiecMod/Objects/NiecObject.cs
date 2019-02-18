using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NiecMod.KillNiec;
using NiecMod.Nra;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
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

namespace Sims3.Gameplay.Objects.Niec
{
    public class NiecObject : StuffedAnimal
    {

        public enum AutoNiec { AutoHouseNiec };

        //public enum InteractionPriorityLevel{};

        // Methods
        public override void OnStartup()
        {
            base.OnStartup();

            base.AddInteraction(Sim_Auto_Niec.Singleton);

            base.AddInventoryInteraction(Sim_Auto_Niec.Singleton);
            
		}
        private sealed class Sim_Auto_Niec : Interaction<Sim, NiecObject>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run() // Run
			{
            	WelcomeWagonSituation.Create(Actor.LotHome);
			    if (Household.ActiveHousehold == null || Household.ActiveHousehold.LotHome == null)
				{
					return false;
				}
				List<Sim> list = new List<Sim>();
				foreach (Sim sim in LotManager.Actors)
				{
					if (sim.SimDescription.ToddlerOrAbove && !sim.IsInActiveHousehold && sim.LotCurrent != Household.ActiveHousehold.LotHome)
					{
						//Urnstone.FinalizeSimDeath(sim.SimDescription, sim.Household, false);
						//sim.InteractionQueue.CancelAllInteractions();
						sim.ModifyFunds(-99999999);
						sim.BuffManager.AddElement(BuffNames.Mourning, Urnstone.CalculateMourningMoodStrength(sim, sim.SimDescription), Origin.FromWitnessingDeath);
                        BuffMourning.BuffInstanceMourning buffInstanceMourning = sim.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                        if (buffInstanceMourning != null)
                        {
                            buffInstanceMourning.MissedSim = Actor.SimDescription;
                        }
                        //sim.Occupation.FireSim(true);
                        //list.Add(sim);
						//sim.Occupation.FireSim(true);
						list.Add(sim);
					}
				}
				if (list.Count > 0)
				{
					Sim randomObjectFromList = RandomUtil.GetRandomObjectFromList<Sim>(list);
					if (!randomObjectFromList.InWorld)
					{
						randomObjectFromList.AddToWorld();
					}
					//randomObjectFromList.SimDescription.Occupation.FireSim(true);
					//Urnstone.FinalizeSimDeath(randomObjectFromList.SimDescription, randomObjectFromList.Household, false);
					randomObjectFromList.InteractionQueue.CancelAllInteractions();
					//Urnstone.FinalizeSimDeath(randomObjectFromList.SimDescription, randomObjectFromList.Household, false);
					//randomObjectFromList.FlagSet(Sim.SimFlags.IgnoreDoorLocks, true);
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					randomObjectFromList.InteractionQueue.Add(VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, randomObjectFromList, new InteractionPriority((InteractionPriorityLevel)349), false, true));
					return true;
				}
				return false;
			}

            public override bool RunFromInventory()
            {
            	if (Actor.IsInActiveHousehold && Autonomous)
            	{
            		this.Actor.LoopIdle();
            		this.Actor.Motives.MaxEverything();
        		    Actor.Autonomy.Motives.FreezeDecayEverythingExcept(new CommodityKind[0]);
            		Simulator.Sleep(1640); // 1 Day
                	return this.Run();
            	}
            	if (Actor.IsInActiveHousehold)
            	{
                	return this.Run();
            	}
            	return false;
            }

            // Nested Types
            //[DoesntRequireTuning]
            private class Definition : InteractionDefinition<Sim, NiecObject, NiecObject.Sim_Auto_Niec>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
            {
                // Methods
            public override string GetInteractionName(Sim actor, NiecObject target, InteractionObjectPair interaction)
            {
                return "Force Visitor Sorry By Niec";
            }
/*
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim != actor && sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }
*/
                public override bool Test(Sim a, NiecObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                	if (a.IsNPC) return false;
                    
                	return true;
                }
                public override string[] GetPath(bool bPath)
                {
                	return new string[] { "Death Good System Options..." };
                }
                public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            	{
                	return SpecialCaseAgeTests.None;
            	}
            }
        }
    }
}



