/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 27/09/2018
 * Time: 6:46
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
using NRaas;

namespace NiecMod.Interactions
{
    public sealed class LincSAT : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public override bool Run() // Run TODO
        {
            try
            {
                
                List<Sim> list = new List<Sim>();
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.IsNPC && !sim.IsInActiveHousehold && !(sim.Service is GrimReaper))
                    {
                        list.Add(sim);
                    }
                }
                //if (list != null)
                if (list.Count > 0)
                {
                    foreach (Sim nlist in list)
                    {
                    	nlist.MoveInventoryItemsToAFamilyMember();
                        nlist.EnableInteractions();
                        BuffMourning.BuffInstanceMourning buffInstanceMourning;
                        buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                        if (buffInstanceMourning == null)
                        {
                            nlist.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                        }
                        buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                        if (buffInstanceMourning != null)
                        {
                            buffInstanceMourning.MissedSim = Actor.SimDescription;
                        }
                        foreach (Relationship relationship in Relationship.Get(Actor.SimDescription))
                        {
                            if (Actor.SimDescription.Partner == nlist.SimDescription)
                            {
                                if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Marry))
                                {
                                    relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Marry);
                                    relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                                    WeddingAnniversary weddingAnniversary = relationship.WeddingAnniversary;
                                    if (weddingAnniversary != null)
                                    {
                                        AlarmManager.Global.RemoveAlarm(weddingAnniversary.AnniversaryAlarm);
                                        relationship.WeddingAnniversary = null;
                                    }
                                    SocialCallback.BreakUpDescriptionsShared(Actor.SimDescription, nlist.SimDescription);
                                }
                                else if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Propose))
                                {
                                    relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Propose);
                                    relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.BreakUp);
                                }
                                else if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment))
                                {
                                    relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment);
                                    relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.BreakUp);
                                }
                                Actor.SimDescription.ClearPartner();
                            }
                        }
                        if (nlist.Occupation != null)
                        {
                            nlist.Occupation.FireSim(false);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                Common.Exception(Actor, ex);
                return false;
            }
        }



        [DoesntRequireTuning]

        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, LincSAT>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "Tyoiy Eruty: Anti-Evil Haha :D";
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;
                //if (actor.IsNPC) return false;

                return true;
            }
            public override string[] GetPath(bool bPath)
            {
                return new string[] { "NiecMod..." };
            }
            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
        }
    }
}
