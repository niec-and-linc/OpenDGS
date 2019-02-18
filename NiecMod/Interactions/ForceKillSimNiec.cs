#region Using directives
using NRaas.CommonSpace.Helpers;
using System;
using System.Collections.Generic;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using NiecMod.Nra;
#endregion

namespace NiecMod.Interactions
{
    /// <summary>
    /// This a Focre Kill Sim From NPC Only.
    /// </summary>
    public class ForceKillSimNiec : ImmediateInteraction<Sim, Sim>
    {
    	public override bool Run()
        {
            ForceKillSimNiec.Definition definition = base.InteractionDefinition as ForceKillSimNiec.Definition;
            List<Sim> list = new List<Sim>();
            foreach (Sim sim in LotManager.Actors)
            {
                if (sim.SimDescription.ToddlerOrAbove && !sim.IsInActiveHousehold && sim.LotCurrent != Household.ActiveHousehold.LotHome)
                {
                    //
                    SpeedTrap.Sleep(10);
                    sim.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                    BuffMourning.BuffInstanceMourning buffInstanceMourning = sim.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                    if (buffInstanceMourning != null)
                    {
                        buffInstanceMourning.MissedSim = Target.SimDescription;
                    }
                    //
                    SpeedTrap.Sleep(10);
                    sim.BuffManager.AddElement(BuffNames.HeartBroken, Urnstone.CalculateMourningMoodStrength(sim, sim.SimDescription), Origin.FromWitnessingDeath);
                    BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken = sim.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken;
                    if (buffInstanceHeartBroken != null)
                    {
                        buffInstanceHeartBroken.MissedSim = Target.SimDescription;
                    }
                    //
                    SpeedTrap.Sleep(10);
                    sim.BuffManager.AddElement(BuffNames.Negligent, Origin.FromNeglectingChildren);
                    BuffNegligent.BuffInstanceNegligent buffInstanceNegligent = sim.BuffManager.GetElement(BuffNames.Negligent) as BuffNegligent.BuffInstanceNegligent;
                    if (buffInstanceNegligent != null)
                    {
                        buffInstanceNegligent.MissedSims.Add(Target.SimDescription);
                    }
                }
            }
            SpeedTrap.Sleep(10);
            if (Target.IsNPC)
            {
            	Urnstones.CreateGrave(Target.SimDescription, definition.death, true, true);
            }
            
            return true;
        }
    	/*
        public override bool Run()
        {
            ForceKillSimNiec.Definition definition = base.InteractionDefinition as ForceKillSimNiec.Definition;
            {
                if (Target.IsNPC)
                {
                    {
                        this.Target.BuffManager.AddElement(BuffNames.Mourning, Urnstone.CalculateMourningMoodStrength(this.Target, this.Target.SimDescription), Origin.FromWitnessingDeath);
                        BuffMourning.BuffInstanceMourning buffInstanceMourning = this.Target.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                        if (buffInstanceMourning != null)
                        {
                            buffInstanceMourning.MissedSim = this.Target.SimDescription;
                        }
                    }
                    Urnstone.FinalizeSimDeath(Target.SimDescription, this.Target.Household, false);
                    KillSim.KillSimExecute(Target.SimDescription, definition.death);
                    Urnstones.CreateGrave(Target.SimDescription, definition.death, true, true);
                }
            }
            return true;
        }
        */
        public static readonly InteractionDefinition Singleton = new Definition();

        [DoesntRequireTuning]
        
        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, ForceKillSimNiec>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
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
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Old Age with Anti-Unlucky", "Force Kill Sim", SimDescription.DeathType.OldAge), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Fire", "Force Kill Sim", SimDescription.DeathType.Burn), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Electrocution", "Force Kill Sim", SimDescription.DeathType.Electrocution), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Starvation", "Force Kill Sim", SimDescription.DeathType.Starve), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Mummy's Curse", "Force Kill Sim", SimDescription.DeathType.MummyCurse), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Meteor", "Force Kill Sim", SimDescription.DeathType.Meteor), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Drowning", "Force Kill Sim", SimDescription.DeathType.Drown), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Haunting Curse", "Force Kill Sim", SimDescription.DeathType.HauntingCurse), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Transmuted", "Force Kill Sim", SimDescription.DeathType.Transmuted), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Jelly Bean Death", "Force Kill Sim", SimDescription.DeathType.JellyBeanDeath), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Thirst", "Force Kill Sim", SimDescription.DeathType.Thirst), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Freezing", "Force Kill Sim", SimDescription.DeathType.Freeze), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Blunt Force Trauma", "Force Kill Sim", SimDescription.DeathType.BluntForceTrauma), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Ranting", "Force Kill Sim", SimDescription.DeathType.Ranting), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Shark", "Force Kill Sim", SimDescription.DeathType.Shark), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Scuba Drown", "Force Kill Sim", SimDescription.DeathType.ScubaDrown), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Dehydration", "Force Kill Sim", SimDescription.DeathType.MermaidDehydrated), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Jetpack (Fall)", "Force Kill Sim", SimDescription.DeathType.Jetpack), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Causality", "Force Kill Sim", SimDescription.DeathType.Causality), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Old Age (Good) Pet Only", "Force Kill Sim", SimDescription.DeathType.PetOldAgeGood), iop.Target));
                    results.Add(new InteractionObjectPair(new ForceKillSimNiec.Definition("Old Age (Bad) Pet Only", "Force Kill Sim", SimDescription.DeathType.PetOldAgeBad), iop.Target));
                }
            }

            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return this.MenuText;
            }

            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;
                /*
                if (target.IsInActiveHousehold)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Active Household is Denied");
                    return false;
                }
                */
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