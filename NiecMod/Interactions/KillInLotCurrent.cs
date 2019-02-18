/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 22/10/2018
 * Time: 1:30
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

namespace NiecMod.Interactions
{
    /// <summary>
    /// Summary of KillInLotCurrent
    /// </summary>
    public sealed class KillInLotCurrent : ImmediateInteraction<Sim, Sim>
    {
        public override bool Run()
        {
            var definition = InteractionDefinition as Definition;

            if (Autonomous || Actor.IsNPC) return false;
            if (!AcceptCancelDialog.Show("Are You Sure MineKill Lot Current Get Sims?")) return false;

            try
            {
                var list = new List<Sim>();
                foreach (Sim sim in Target.LotCurrent.GetAllActors())
                {
                    if (!sim.IsInActiveHousehold && !(sim.Service is GrimReaper))
                    {
                        list.Add(sim);
                    }
                }
                if (list.Count > 0)
                {
                    foreach (Sim nlist in list)
                    {
                        KillSimNiecX.MineKill(nlist, definition.death, null, true, false);
                    }
                }
                return true;
            }

            catch (Exception exception)
            {
                NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                return true;
            }

        }

        public static readonly InteractionDefinition Singleton = new Definition();

        [DoesntRequireTuning]

        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, KillInLotCurrent>, IIgnoreIsAllowedInRoomCheck, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
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
                results.Add(new InteractionObjectPair(new Definition("Old Age with Anti-Unlucky", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.OldAge), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Fire", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Burn), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Electrocution", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Electrocution), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Starvation", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Starve), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Mummy's Curse", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.MummyCurse), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Meteor", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Meteor), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Drowning", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Drown), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Haunting Curse", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.HauntingCurse), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Transmuted", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Transmuted), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Jelly Bean Death", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.JellyBeanDeath), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Thirst", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Thirst), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Freezing", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Freeze), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Blunt Force Trauma", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.BluntForceTrauma), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Ranting", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Ranting), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Shark", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Shark), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Scuba Drown", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.ScubaDrown), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Dehydration", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.MermaidDehydrated), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Jetpack (Fall)", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Jetpack), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Causality", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.Causality), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("None Test", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.None), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Old Age (Good) Pet Only", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.PetOldAgeGood), iop.Target));
                results.Add(new InteractionObjectPair(new Definition("Old Age (Bad) Pet Only", "MineKill LotCurrent Get Sims...", SimDescription.DeathType.PetOldAgeBad), iop.Target));
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return this.MenuText;
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

            public string MenuText = string.Empty;

            public string[] MenuPath;

            public SimDescription.DeathType death;
        }
    }
}