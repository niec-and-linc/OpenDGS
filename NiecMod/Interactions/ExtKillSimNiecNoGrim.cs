/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 27/09/2018
 * Time: 4:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NiecMod.Nra;
using NRaas;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using NiecMod.KillNiec;

namespace NiecMod.Interactions
{
    /// <summary>
    /// Fix no Exit ExtKillSimNiec to Focre Kill NPC ONLY
    /// KillSim Is Helper By Niec
    /// Support Killing Baby The Sims 3 is Modified Support Killing Babies By Niec
    /// </summary>
    public sealed class ExtKillSimNiecNoGrim : Interaction<Sim, Sim>
    {
        // Token: 0x0600A7E2 RID: 42978 RVA: 0x002FB164 File Offset: 0x002FA164
        public override bool Run()
        {
            try
            {
                ExtKillSimNiec extKillSimNiec = ExtKillSimNiec.Singleton.CreateInstance(Actor, Actor, KillSimNiecX.DGSAndNonDGSPriority(), false, false) as ExtKillSimNiec;
                extKillSimNiec.simDeathType = this.simDeathType;
                extKillSimNiec.PlayDeathAnimation = this.PlayDeathAnimation;
                extKillSimNiec.MustRun = true;
                //Actor.InteractionQueue.AddNext(extKillSimNiec);
                return extKillSimNiec.RunInteraction();
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception)
            {
                return true;
            }
        }

        // Token: 0x0600A7E3 RID: 42979 RVA: 0x002FB788 File Offset: 0x002FA788
        public override void Cleanup()
        {
            if (this.FixNotExit)
            {
                if (!Actor.IsInActiveHousehold)
                {
                    this.DeathTypeFix = false;
                    this.ActiveFix = false;
                    this.Actor.MoveInventoryItemsToAFamilyMember();
                    Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, true);
                }
            }
            this.Actor.RemoveInteractionByType(Sim.DeathReaction.Singleton);
            if (this.CancelDeath)
            {
                SimDescription.DeathType deathStyle = this.Actor.SimDescription.DeathStyle;
                SimDescription simDescription = this.Actor.SimDescription;

                if (!simDescription.IsEP11Bot)
                {
                    simDescription.AgingEnabled = true;
                    if (simDescription.DeathStyle == SimDescription.DeathType.OldAge)
                    {
                        simDescription.AgingState.ResetAndExtendAgingStage(0f);
                    }
                }
                simDescription.ShowSocialsOnSim = true;
                World.ObjectSetGhostState(this.Actor.ObjectId, 0u, (uint)simDescription.AgeGenderSpecies);
                simDescription.IsNeverSelectable = false;
                if (this.ActiveFix)
                {
                    if (!Actor.IsInActiveHousehold)
                    {
                        //Actor.SimDescription.Contactable = false;
                        this.Actor.MoveInventoryItemsToAFamilyMember();
                        this.DeathTypeFix = false;
                        Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, true);
                    }
                }
                if (this.DeathTypeFix)
                {
                    simDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                }
            }
            if (this.SocialJig != null)
            {
                this.SocialJig.Destroy();
                this.SocialJig = null;
            }
            base.Cleanup();
        }

        // Token: 0x0600A7E4 RID: 42980 RVA: 0x002FB840 File Offset: 0x002FA840
        private void EventCallbackCreateAshPile(StateMachineClient sender, IEvent evt)
        {
            this.mAshPile = (GlobalFunctions.CreateObjectOutOfWorld("AshPile") as AshPile);
            if (this.Actor.SimDescription.IsMummy || this.Actor.SimDescription.DeathStyle == SimDescription.DeathType.MummyCurse)
            {
                this.mAshPile.SetMaterial("mummy");
            }
            this.mAshPile.AddToWorld();
            this.mAshPile.SetPosition(this.Actor.Position);
            this.mAshPile.SetTooltipText(this.Actor.SimDescription.FirstName + " " + this.Actor.SimDescription.LastName);
            LotLocation invalid = LotLocation.Invalid;
            World.GetLotLocation(this.mAshPile.Position, ref invalid);
            FireManager.BurnTile(this.mAshPile.LotCurrent.LotId, invalid);
            SimDescription.DeathType deathStyle = this.Actor.SimDescription.DeathStyle;
            float fadeTime = (deathStyle == SimDescription.DeathType.MummyCurse || deathStyle == SimDescription.DeathType.Thirst) ? 1.2f : GameObject.kGlobalObjectFadeTime;
            this.Actor.FadeOut(false, false, fadeTime);
        }

        // Token: 0x0600A7E5 RID: 42981 RVA: 0x002FB957 File Offset: 0x002FA957
        private void EventCallbackHideSim(StateMachineClient sender, IEvent evt)
        {
            this.Actor.SetHiddenFlags((HiddenFlags)4294967295u);
        }

        // Token: 0x0600A7E6 RID: 42982 RVA: 0x002FB968 File Offset: 0x002FA968
        private void EventCallbackTurnToGhost(StateMachineClient sender, IEvent evt)
        {
            if (!this.Actor.SimDescription.IsEP11Bot)
            {
                uint deathStyle = (uint)this.Actor.SimDescription.DeathStyle;
                World.ObjectSetGhostState(this.Actor.ObjectId, deathStyle, (uint)this.Actor.SimDescription.AgeGenderSpecies);
                return;
            }
            World.ObjectSetGhostState(this.Actor.ObjectId, 23u, (uint)this.Actor.SimDescription.AgeGenderSpecies);
        }

        // Token: 0x0600A7E7 RID: 42983 RVA: 0x002FB9DC File Offset: 0x002FA9DC
        private void EventCallbackFadeOutSim(StateMachineClient sender, IEvent evt)
        {
            this.Actor.FadeOut();
        }

        // Token: 0x04005BED RID: 23533
        public static readonly InteractionDefinition Singleton = new Definition();

        public static readonly InteractionDefinition NiecDefinitionDeathInteractionSocialSingleton = new SocialInteractionB.DefinitionDeathInteraction();

        // Token: 0x04005BEE RID: 23534
        private AshPile mAshPile;

        // Token: 0x04005BEF RID: 23535
        public bool CancelDeath = true;

        public bool ActiveFix = true;

        public bool DeathTypeFix = true;

        public bool FixNotExit = true;

        // Token: 0x04005BF0 RID: 23536
        public SocialJig SocialJig;

        // Token: 0x04005BF1 RID: 23537
        public SimDescription.DeathType simDeathType;

        // Token: 0x04005BF2 RID: 23538
        public bool PlayDeathAnimation = true;


        [DoesntRequireTuning]

        private sealed class Definition : InteractionDefinition<Sim, Sim, ExtKillSimNiecNoGrim>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            // Token: 0x0600A7EA RID: 42986 RVA: 0x002FBA0B File Offset: 0x002FAA0B
            public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
            {
                return Localization.LocalizeString("Gameplay/Actors/Sim/ReapSoul:InteractionName", new object[0]);
                //return "Exipre";
            }

            // Token: 0x0600A7EB RID: 42987 RVA: 0x002FBA23 File Offset: 0x002FAA23
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;
                //if (a.IsInActiveHousehold) return false;
                return true;
            }
            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
            public override string[] GetPath(bool bPath)
            {
                return new string[] { "Test myMod" };
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance interactionInstance = base.CreateInstance(ref parameters);
                interactionInstance.MustRun = true;
                return interactionInstance;
            }
        }
    }
}
