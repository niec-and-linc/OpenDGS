/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 29/09/2018
 * Time: 5:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.Gameplay.Interfaces;
using NiecMod.Nra;

namespace NiecMod.Interactions
{
    public sealed class TheNiecReapSoul : SocialInteraction
    {
        public ulong GraveObjectId
        {
            get
            {
                return this.mGrave.ObjectId.Value;
            }
        }

        // Token: 0x06006FAD RID: 28589 RVA: 0x0026B994 File Offset: 0x0026A994
        public override void Cleanup()
        {
            try
            {
                SMCDeath.EnterState("x", "Enter");
            }
            catch (Exception exception)
            {
                NiecException.PrintMessage("EnterStateCleanup " + exception.Message + NiecException.NewLine + exception.StackTrace);
            }
            base.Cleanup();
        }

        // Token: 0x06006FAE RID: 28590 RVA: 0x0026BB9C File Offset: 0x0026AB9C
        //[DoesntRequireTuning]
        public override bool Run()
        {
            try
            {
                try
                {
                    if (!this.CreateGraveStone())
                    {
                        return false;
                    }
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("CreateGraveStone " + exception.Message + NiecException.NewLine + exception.StackTrace);
                }

                try
                {
                    SMCDeath = StateMachineClient.Acquire(this.Actor, "DeathSequence");

                    this.mSMCDeath = SMCDeath;
                    this.mSMCDeath.SetActor("y", this.Target);
                    this.mSMCDeath.SetActor("grave", this.mGrave);
                    this.mSMCDeath.EnterState("y", "Enter");
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("SMCDeath " + exception.Message + NiecException.NewLine + exception.StackTrace);
                }
                
                if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.Drown)
                {
                    if (!this.RouteGrimReaperToEdgeOfTargetPool())
                    {
                        this.Actor.RouteTurnToFace(this.Target.Position);
                    }
                }
                else if (!this.mSituation.mIsFirstSim)
                {
                    Route route = this.Actor.CreateRoute();
                    route.PlanToPointRadialRange(this.Target.Position, 1f, 5f, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, this.Target.LotCurrent.LotId, null);
                    this.Actor.DoRoute(route);
                }
                else
                {
                    this.mSituation.mIsFirstSim = false;
                }
                this.PlaceGraveStone();
                this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GravePlaced;
                this.Actor.RouteTurnToFace(this.Target.Position);
                PetStartleBehavior.CheckForStartle(this.Actor, StartleType.GrimReaperAppear);
                if (!this.mSituation.mIsFirstSim)
                {
                    this.mSMCDeath.EnterState("x", "Enter");
                }
                this.mSMCDeath.RequestState(true, "x", "CreateTombstone");
                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_moodlet_mourning", this.Target.GetThoughtBalloonThumbnailKey());
                balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                balloonData.mPriority = ThoughtBalloonPriority.High;
                this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                this.mSMCDeath.RequestState(true, "x", "ReaperPointAtGrave");
                this.mSMCDeath.RequestState(false, "x", "ReaperFloating");
                this.mGhostPosition = this.GetPositionForGhost(this.Target, this.mGrave);
                Lot lotCurrent = this.Target.LotCurrent;
                if (lotCurrent != this.Target.LotHome && lotCurrent.Household != null)
                {
                    this.Target.GreetSimOnLot(lotCurrent);
                }
                base.RequestWalkStyle(this.Target, Sim.WalkStyle.GhostWalk);
                switch (this.Target.SimDescription.DeathStyle)
                {
                    case SimDescription.DeathType.Drown:
                        if (this.Target.BridgeOrigin != null)
                        {
                            this.Target.BridgeOrigin.MakeRequest();
                        }
                        this.Target.PopPosture();
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackSimToGhostEffectNoFadeOut));
                        this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.RequestState(true, "y", "PoseStarvation");
                        this.mSMCDeath.RequestState(true, "y", "StarvationToFloat");
                        this.mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    case SimDescription.DeathType.Starve:
                        this.StartGhostExplosion();
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.RequestState(true, "y", "PoseStarvation");
                        this.mSMCDeath.RequestState(true, "y", "StarvationToFloat");
                        this.mSMCDeath.RequestState(false, "y", "GhostFloating");
                        this.StopGhostExplosion();
                        break;
                    case SimDescription.DeathType.Electrocution:
                    case SimDescription.DeathType.BluntForceTrauma:
                    case SimDescription.DeathType.Ranting:
                        this.StartGhostExplosion();
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.RequestState(true, "y", "PoseElectrocution");
                        this.mSMCDeath.RequestState(true, "y", "ElectrocutionToFloat");
                        this.mSMCDeath.RequestState(false, "y", "GhostFloating");
                        this.StopGhostExplosion();
                        break;
                    case SimDescription.DeathType.Burn:
                    case SimDescription.DeathType.MummyCurse:
                    case SimDescription.DeathType.Meteor:
                    case SimDescription.DeathType.Thirst:
                    case SimDescription.DeathType.Transmuted:
                    case SimDescription.DeathType.HauntingCurse:
                    case SimDescription.DeathType.JellyBeanDeath:
                    case SimDescription.DeathType.Jetpack:
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackSimToGhostEffectNoFadeOut));
                        this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.RequestState(true, "y", "PoseElectrocution");
                        this.mSMCDeath.RequestState(true, "y", "ElectrocutionToFloat");
                        this.mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    case SimDescription.DeathType.Freeze:
                        this.StartGhostExplosion();
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
                        this.mSMCDeath.RequestState(true, "y", "PoseFreeze");
                        this.mSMCDeath.RequestState(true, "y", "FreezeToFloat");
                        this.mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    case SimDescription.DeathType.MermaidDehydrated:
                        this.MermaidDehydratedToGhostSequence();
                        break;
                }
                if (this.Target.SimDescription.DeathStyle != SimDescription.DeathType.OldAge)
                {
                    this.Target.SetPosition(this.mGhostPosition);
                }
                this.mGrave.GhostSetup(this.Target, false);
                this.Target.SetHiddenFlags(HiddenFlags.Nothing);
                this.Target.FadeIn();
                base.RequestWalkStyle(Sim.WalkStyle.DeathWalk);
                string name = Localization.LocalizeString(this.Target.SimDescription.IsFemale, "Gameplay/Actors/Sim/ReapSoul:InteractionName", new object[0]);
                if (base.BeginSocialInteraction(new SocialInteractionB.DefinitionDeathInteraction(name, false), true, 1.25f, false))
                {
                    if (this.Target.SimDescription.DeathStyle != (SimDescription.DeathType)69u && this.Target.Household != null && this.Target.IsInActiveHousehold)
                    {
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.UnluckyStarted;
                        if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.OldAge)
                        {
                            this.Target.Motives.SetValue(CommodityKind.VampireThirst, -50f);
                        }
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackResurrectSimUnlucky));
                        this.mSMCDeath.RequestState(false, "x", "Unlucky");
                        this.mSMCDeath.RequestState(true, "y", "Unlucky");
                        this.mSMCDeath.RequestState(false, "x", "Exit");
                        this.mSMCDeath.RequestState(true, "y", "Exit");
                        this.GrimReaperPostSequenceCleanup();
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
                        return true;
                    }
                    if (this.Target.SimDescription.DeathStyle == (SimDescription.DeathType)76u && this.Target.TraitManager != null && !this.Target.TraitManager.HasElement(TraitNames.ThereAndBackAgain))
                    {
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.UnluckyStarted;
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackResurrectSimRanting));
                        this.mSMCDeath.RequestState(false, "x", "Unlucky");
                        this.mSMCDeath.RequestState(true, "y", "Unlucky");
                        this.mSMCDeath.RequestState(false, "x", "Exit");
                        this.mSMCDeath.RequestState(true, "y", "Exit");
                        this.GrimReaperPostSequenceCleanup();
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
                        return true;
                    }
                    this.mDeathFlower = this.Target.Inventory.Find<DeathFlower>();
                    if (this.mDeathFlower != null)
                    {
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.DeathFlowerStarted;
                        this.Target.Inventory.RemoveByForce(this.mDeathFlower);
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackResurrectSimDeathFlower));
                        this.mSMCDeath.RequestState(false, "x", "DeathFlower");
                        this.mSMCDeath.RequestState(true, "y", "DeathFlower");
                        this.mDeathFlower.Destroy();
                        this.mDeathFlower = null;
                        this.mSMCDeath.RequestState(false, "x", "Exit");
                        this.mSMCDeath.RequestState(true, "y", "Exit");
                        this.GrimReaperPostSequenceCleanup();
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
                        return true;
                    }
                    if (this.Target.SimDescription.DeathStyle != SimDescription.DeathType.OldAge)
                    {
                        this.Actor.AddInteraction(GrimReaperSituation.ChessChallenge.Singleton);
                    }
                    this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
                    if (RandomUtil.CoinFlip())
                    {
                        this.mSMCDeath.RequestState(false, "x", "Accept");
                        this.mSMCDeath.RequestState(true, "y", "Accept");
                        Route route2 = this.Target.CreateRouteTurnToFace(this.mGrave.Position);
                        route2.ExecutionFromNonSimTaskIsSafe = true;
                        this.Target.DoRoute(route2);
                    }
                    else if (RandomUtil.CoinFlip())
                    {
                        this.mSMCDeath.RequestState(false, "x", "Reject");
                        this.mSMCDeath.RequestState(true, "y", "Reject");
                        Route route3 = this.Target.CreateRouteTurnToFace(this.mGrave.Position);
                        route3.ExecutionFromNonSimTaskIsSafe = true;
                        this.Target.DoRoute(route3);
                        this.mSMCDeath.RequestState(false, "x", "GhostJumpInGrave");
                        this.mSMCDeath.RequestState(true, "y", "GhostJumpInGrave");
                    }
                    else
                    {
                        this.mSMCDeath.RequestState(false, "x", "GhostKickedDive");
                        this.mSMCDeath.RequestState(true, "y", "GhostKickedDive");
                        this.mSMCDeath.RequestState(false, "x", "Kicked");
                        this.mSMCDeath.RequestState(true, "y", "Kicked");
                    }
                    this.mSMCDeath.RequestState(false, "y", "Exit");
                    this.mSMCDeath.RequestState(true, "x", "Exit");
                }
                else
                {
                    this.mDeathFlower = this.Target.Inventory.Find<DeathFlower>();
                    if (this.Target.SimDescription.DeathStyle != (SimDescription.DeathType)69u && this.Target.Household != null && this.Target.IsInActiveHousehold)
                    {
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.UnluckyStarted;
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackResurrectSimUnlucky));
                        this.mSMCDeath.RequestState(false, "x", "Unlucky");
                        this.mSMCDeath.RequestState(true, "y", "Unlucky");
                        this.mSMCDeath.RequestState(false, "x", "Exit");
                        this.mSMCDeath.RequestState(true, "y", "Exit");
                        this.GrimReaperPostSequenceCleanup();
                        this.Target.AddExitReason(ExitReason.HigherPriorityNext);
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
                        return true;
                    }
                    if (this.mDeathFlower != null)
                    {
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.DeathFlowerStarted;
                        this.Target.Inventory.RemoveByForce(this.mDeathFlower);
                        this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackResurrectSimDeathFlower));
                        this.mSMCDeath.RequestState(false, "x", "Unlucky");
                        this.mSMCDeath.RequestState(true, "y", "Unlucky");
                        this.mDeathFlower.Destroy();
                        this.mDeathFlower = null;
                        this.mSMCDeath.RequestState(false, "x", "Exit");
                        this.mSMCDeath.RequestState(true, "y", "Exit");
                        this.GrimReaperPostSequenceCleanup();
                        this.Target.AddExitReason(ExitReason.HigherPriorityNext);
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
                        return true;
                    }
                    this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
                    this.mSMCDeath.RequestState("x", "ExitNoSocial");
                    this.Target.FadeOut();
                    this.mDeathEffect = this.mGrave.GetSimToGhostEffect(this.Target, this.mGrave.Position);
                    this.mSMCDeath.SetEffectActor("deathEffect", this.mDeathEffect);
                    this.mDeathEffect.Start();
                }
                this.mSituation.CheckAndSetPetSavior(this.Target);
                this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
                this.FinalizeDeath();
                this.GrimReaperPostSequenceCleanup();
                this.Target.StartOneShotFunction(new Sims3.Gameplay.Function(this.ReapSoulCallback), GameObject.OneShotFunctionDisposeFlag.OnDispose);
                this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
                
            }
            catch (Exception exception)
            {
                NiecException.PrintMessage("Sxception " + exception.Message + NiecException.NewLine + exception.StackTrace);
            }
            return true;
        }

        // Token: 0x06006FAF RID: 28591 RVA: 0x0026C808 File Offset: 0x0026B808
        public bool CreateGraveStone()
        {
            Urnstone.KillSim killSim = this.Target.CurrentInteraction as Urnstone.KillSim;
            if (killSim != null)
            {
                killSim.CancelDeath = false;
            }
            this.mWasMemberOfActiveHousehold = (this.Target.Household == Household.ActiveHousehold);
            if (this.Target.DeathReactionBroadcast == null)
            {
                Urnstone.CreateDeathReactionBroadcaster(this.Target);
            }
            this.Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
            this.Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
            this.mGrave = Urnstone.CreateGrave(this.Target.SimDescription, true, false);
            if (this.mGrave == null)
            {
                return false;
            }
            this.mDeadSimsHousehold = this.Target.Household;
            this.mGrave.AddToUseList(this.Actor);
            this.mSituation.LastGraveCreated = this.mGrave;
            this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GraveCreated;
            return true;
        }

        // Token: 0x06006FB0 RID: 28592 RVA: 0x0026C8F0 File Offset: 0x0026B8F0
        public void PlaceGraveStone()
        {
            this.mGrave.SetOpacity(0f, 0f);
            SimDescription.DeathType deathStyle = this.Target.SimDescription.DeathStyle;
            World.FindGoodLocationParams fglParams;
            if (deathStyle == SimDescription.DeathType.Drown)
            {
                fglParams = new World.FindGoodLocationParams(this.Actor.Position);
            }
            else
            {
                fglParams = new World.FindGoodLocationParams(this.Target.Position);
            }
            fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
            if (!GlobalFunctions.PlaceAtGoodLocation(this.mGrave, fglParams, false))
            {
                fglParams.BooleanConstraints = FindGoodLocationBooleans.None;
                if (!GlobalFunctions.PlaceAtGoodLocation(this.mGrave, fglParams, false))
                {
                    this.mGrave.SetPosition(this.Target.Position);
                }
            }
            this.mGrave.OnHandToolMovement();
            this.mGrave.FadeIn(false, 10f);
            this.mGrave.FogEffectStart();
        }

        // Token: 0x06006FB1 RID: 28593 RVA: 0x0026C9C0 File Offset: 0x0026B9C0
        public Vector3 GetPositionForGhost(Sim ghost, Urnstone grave)
        {
            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(grave.Position);
            fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
            Vector3 position;
            Vector3 vector;
            if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out position, out vector))
            {
                Simulator.Sleep(0u);
                fglParams.BooleanConstraints &= ~FindGoodLocationBooleans.StayInRoom;
                if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out position, out vector))
                {
                    Simulator.Sleep(0u);
                    fglParams.BooleanConstraints &= ~FindGoodLocationBooleans.Routable;
                    if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out position, out vector))
                    {
                        Simulator.Sleep(0u);
                        fglParams.BooleanConstraints = FindGoodLocationBooleans.None;
                        if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out position, out vector))
                        {
                            position = ghost.Position;
                        }
                    }
                }
            }
            return position;
        }

        // Token: 0x06006FB2 RID: 28594 RVA: 0x0026CA60 File Offset: 0x0026BA60
        private bool RouteGrimReaperToEdgeOfTargetPool()
        {
            Pool pool = this.Target.Posture.Container as Pool;
            return pool != null && pool.RouteToEdge(this.Actor);
        }

        // Token: 0x06006FB3 RID: 28595 RVA: 0x0026CA94 File Offset: 0x0026BA94
        public void EventCallbackFadeBodyFromPose(StateMachineClient sender, IEvent evt)
        {
            switch (evt.EventId)
            {
                case 101u:
                    this.Target.FadeOut();
                    this.mDeathEffect = this.mGrave.GetSimToGhostEffect(this.Target, this.mGhostPosition);
                    this.mSMCDeath.SetEffectActor("deathEffect", this.mDeathEffect);
                    this.mDeathEffect.Start();
                    return;
                case 102u:
                    this.Target.SetPosition(this.mGhostPosition);
                    this.mGrave.GhostSetup(this.Target, false);
                    this.Target.SetHiddenFlags(HiddenFlags.Nothing);
                    if (this.mDeathEffect != null)
                    {
                        this.mDeathEffect.Stop();
                        this.mDeathEffect = null;
                    }
                    this.Target.FadeIn();
                    return;
                default:
                    return;
            }
        }

        // Token: 0x06006FB4 RID: 28596 RVA: 0x0026CB5C File Offset: 0x0026BB5C
        public void EventCallbackSimToGhostEffectNoFadeOut(StateMachineClient sender, IEvent evt)
        {
            this.mDeathEffect = this.mGrave.GetSimToGhostEffect(this.Target, this.mGhostPosition);
            this.mSMCDeath.SetEffectActor("deathEffect", this.mDeathEffect);
            this.mDeathEffect.Start();
        }

        // Token: 0x06006FB5 RID: 28597 RVA: 0x0026CBA8 File Offset: 0x0026BBA8
        private void StartGhostExplosion()
        {
            if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.Freeze)
            {
                this.mGhostExplosion = VisualEffect.Create("ashesToAshesFreeze");
            }
            else
            {
                this.mGhostExplosion = VisualEffect.Create("ashesToAshes");
            }
            this.mGhostExplosion.SetPosAndOrient(this.Target.Position, this.Target.ForwardVector, this.Target.UpVector);
            this.mGhostExplosion.Start();
        }

        // Token: 0x06006FB6 RID: 28598 RVA: 0x0026CC23 File Offset: 0x0026BC23
        private void StopGhostExplosion()
        {
            if (this.mGhostExplosion != null)
            {
                this.mGhostExplosion.Stop();
                this.mGhostExplosion = null;
            }
        }

        // Token: 0x06006FB7 RID: 28599 RVA: 0x0026CC40 File Offset: 0x0026BC40
        public void ReapSoulCallback()
        {
            int num = 0;
            do
            {
                Simulator.Sleep(10u);
                num++;
            }
            while (this.Target.ReferenceList.Count > 0 && num < 30);
            this.CleanupAndDestroyDeadSim(false);
        }

        // Token: 0x06006FB8 RID: 28600 RVA: 0x0026CC7C File Offset: 0x0026BC7C
        private void CleanupAndDestroyDeadSim(bool forceCleanup)
        {
            if (this.mDeathEffect != null)
            {
                this.mDeathEffect.Stop();
                this.mDeathEffect = null;
            }
            this.Target.ClearReferenceList();
            if (this.mSituation.LastSimOfHousehold == null || this.mSituation.LastSimOfHousehold != this.Target || forceCleanup)
            {
                if (this.Target.Household == null || this.Target.Household.IsPetHousehold)
                {
                    SimDescription simDescription = this.Target.SimDescription;
                    PetPoolType petPoolType = PetPoolType.None;
                    CASAgeGenderFlags species = simDescription.Species;
                    if (species <= CASAgeGenderFlags.Cat)
                    {
                        if (species != CASAgeGenderFlags.Horse)
                        {
                            if (species == CASAgeGenderFlags.Cat)
                            {
                                petPoolType = PetPoolType.StrayCat;
                            }
                        }
                        else if (simDescription.IsUnicorn)
                        {
                            petPoolType = PetPoolType.Unicorn;
                        }
                        else
                        {
                            petPoolType = PetPoolType.WildHorse;
                        }
                    }
                    else if (species == CASAgeGenderFlags.Dog || species == CASAgeGenderFlags.LittleDog)
                    {
                        petPoolType = PetPoolType.StrayDog;
                    }
                    if (PetPoolManager.IsPetInPoolType(simDescription, petPoolType))
                    {
                        PetPoolManager.RemovePet(petPoolType, simDescription);
                    }
                }
                this.mGrave.GhostCleanup(this.Target, true);
                if (this.Target.Autonomy != null)
                {
                    this.Target.Autonomy.DecrementAutonomyDisabled();
                }
                this.Target.SimDescription.ShowSocialsOnSim = true;
                if (!this.mWasMemberOfActiveHousehold && Household.ActiveHousehold != null && this.Actor.LotCurrent != Household.ActiveHousehold.LotHome)
                {
                    this.mGrave.FadeOut(false, 5f, new AlarmTimerCallback(this.HandleNPCGrave));
                }
                this.Target.Destroy();
            }
        }

        // Token: 0x06006FB9 RID: 28601 RVA: 0x0026CDEC File Offset: 0x0026BDEC
        private void EventCallbackResurrectSim()
        {
            bool bResetAge = false;
            if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.OldAge)
            {
                bResetAge = true;
            }
            this.mGrave.GhostToSim(this.Target, bResetAge, false);
            this.Actor.ClearSynchronizationData();
            this.mGrave.FadeOut(false, true);
            this.Target.SimDescription.IsNeverSelectable = false;
            this.Target.SimDescription.ShowSocialsOnSim = true;
            this.Target.Autonomy.DecrementAutonomyDisabled();
        }

        // Token: 0x06006FBA RID: 28602 RVA: 0x0026CE70 File Offset: 0x0026BE70
        public void EventCallbackResurrectSimDeathFlower(StateMachineClient sender, IEvent evt)
        {
            EventTracker.SendEvent(EventTypeId.kGotSavedByDeathFlower, this.Target);
            this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.DeathFlowerPostEvent;
            if (GrimReaperSituation.ShouldDoDeathEvent(this.Target))
            {
                StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Services/GrimReaper:DeathFlower1", new object[]
			{
				this.Target
			}), this.Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                StyledNotification.Show(format);
            }
            ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.BalloonData(this.mDeathFlower.GetThoughtBalloonThumbnailKey());
            balloonData.mPriority = ThoughtBalloonPriority.High;
            balloonData.Duration = ThoughtBalloonDuration.Medium;
            balloonData.mCoolDown = ThoughtBalloonCooldown.Medium;
            balloonData.LowAxis = ThoughtBalloonAxis.kLike;
            this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
            this.EventCallbackResurrectSim();
        }

        // Token: 0x06006FBB RID: 28603 RVA: 0x0026CF1C File Offset: 0x0026BF1C
        private void EventCallbackResurrectSimUnlucky(StateMachineClient sender, IEvent evt)
        {
            this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.UnluckyPostEvent;
            if (GrimReaperSituation.ShouldDoDeathEvent(this.Target))
            {
                StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Services/GrimReaper:Unlucky1", new object[]
			{
				this.Target
			}), this.Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                StyledNotification.Show(format);
            }
            this.EventCallbackResurrectSim();
        }

        // Token: 0x06006FBC RID: 28604 RVA: 0x0026CF78 File Offset: 0x0026BF78
        private void EventCallbackResurrectSimRanting(StateMachineClient sender, IEvent evt)
        {
            this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.UnluckyPostEvent;
            if (GrimReaperSituation.ShouldDoDeathEvent(this.Target))
            {
                StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Services/GrimReaper:RantingWarning", new object[]
			{
				this.Target
			}), this.Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                StyledNotification.Show(format);
            }
            this.Target.BuffManager.AddElement(BuffNames.ThereAndBackAgain, Origin.FromRanting);
            this.EventCallbackResurrectSim();
        }

        // Token: 0x06006FBD RID: 28605 RVA: 0x0026CFF7 File Offset: 0x0026BFF7
        public void GrimReaperPostSequenceCleanup()
        {
            this.Actor.Posture = this.Actor.Standing;
        }

        // Token: 0x06006FBE RID: 28606 RVA: 0x0026D010 File Offset: 0x0026C010
        public void FinalizeDeath()
        {
            this.MakeHouseholdHorsesGoHome();
            if (this.Target.SimDescription.IsEnrolledInBoardingSchool())
            {
                this.Target.SimDescription.BoardingSchool.OnRemovedFromSchool();
            }
            Urnstone.FinalizeSimDeath(this.Target.SimDescription, this.Target.Household, this.mSituation.PetSavior == null);
            int minuteOfDeath = (int)Math.Floor((double)SimClock.ConvertFromTicks(SimClock.CurrentTime().Ticks, TimeUnit.Minutes)) % 60;
            this.mGrave.MinuteOfDeath = minuteOfDeath;
            if (this.Target.DeathReactionBroadcast != null)
            {
                this.Target.DeathReactionBroadcast.Dispose();
                this.Target.DeathReactionBroadcast = null;
            }
            this.Target.SetHiddenFlags((HiddenFlags)4294967295u);
            Household household = this.Target.Household;
            if (household != null)
            {
                if (household.IsActive)
                {
                    this.Target.MoveInventoryItemsToAFamilyMember();
                }
                this.Target.LotCurrent.LastDiedSim = this.Target.SimDescription;
                this.Target.LotCurrent.NumDeathsOnLot++;
                this.Actor.ClearSynchronizationData();
                this.mSituation.DeathCheckForAbandonedChildren(this.Target);
                if (this.Target.SimDescription.DeathStyle != SimDescription.DeathType.OldAge)
                {
                    this.Actor.RemoveInteractionByType(GrimReaperSituation.ChessChallenge.Singleton);
                }
                if (BoardingSchool.ShouldSimsBeRemovedFromBoardingSchool(household))
                {
                    BoardingSchool.RemoveAllSimsFromBoardingSchool(household);
                }
                if (household.IsActive && !this.Target.BuffManager.HasElement(BuffNames.Ensorcelled))
                {
                    int num = 0;
                    foreach (Sim sim in household.AllActors)
                    {
                        if (sim.BuffManager.HasElement(BuffNames.Ensorcelled))
                        {
                            num++;
                        }
                    }
                    if (household.AllActors.Count == num + 1)
                    {
                        foreach (Sim sim2 in household.AllActors)
                        {
                            if (sim2.BuffManager.HasElement(BuffNames.Ensorcelled))
                            {
                                sim2.BuffManager.RemoveElement(BuffNames.Ensorcelled);
                            }
                        }
                    }
                }
                int num2 = household.AllActors.Count - household.GetNumberOfRoommates();
                if (household.IsActive && num2 == 1 && !Household.RoommateManager.IsNPCRoommate(this.Target))
                {
                    this.mSituation.LastSimOfHousehold = this.Target;
                }
                else
                {
                    if (this.Target.IsActiveSim)
                    {
                        LotManager.SelectNextSim();
                    }
                    if (this.mWasMemberOfActiveHousehold)
                    {
                        household.RemoveSim(this.Target);
                    }
                }
            }
            this.mGrave.RemoveFromUseList(this.Actor);
            Ocean singleton = Ocean.Singleton;
            if (singleton != null && singleton.IsActorUsingMe(this.Target))
            {
                singleton.RemoveFromUseList(this.Target);
                this.Target.Posture = null;
            }
        }

        // Token: 0x06006FBF RID: 28607 RVA: 0x0026D324 File Offset: 0x0026C324
        private void HandleNPCGrave()
        {
            if (this.mGrave.InInventory)
            {
                this.mGrave.SetOpacity(1f, 0f);
                return;
            }
            bool flag = false;
            Household activeHousehold = Household.ActiveHousehold;
            if (activeHousehold != null)
            {
                flag = ((this.Target.SimDescription.GetMiniSimForProtection().ProtectionFlags & MiniSimDescription.ProtectionFlag.PartialFromPlayer) == MiniSimDescription.ProtectionFlag.PartialFromPlayer || Notifications.HasSignificantRelationship(activeHousehold, this.Target.SimDescription));
            }
            if (flag)
            {
                GrimReaperSituation.AddGraveToRandomMausoleum(this.mGrave);
                return;
            }
            this.mGrave.Destroy();
        }

        // Token: 0x06006FC0 RID: 28608 RVA: 0x0026D3A8 File Offset: 0x0026C3A8
        private void MakeHouseholdHorsesGoHome()
        {
            if (this.mWasMemberOfActiveHousehold || this.Target.IsAtHome)
            {
                return;
            }
            Lot lotCurrent = this.Target.LotCurrent;
            if (lotCurrent.IsWorldLot)
            {
                return;
            }
            List<Sim> list = null;
            foreach (Sim sim in this.Target.Household.AllActors)
            {
                if (sim.LotCurrent == lotCurrent && sim != this.Target)
                {
                    if (sim.IsHuman)
                    {
                        if (sim.SimDescription.ChildOrAbove)
                        {
                            return;
                        }
                    }
                    else if (sim.IsHorse)
                    {
                        Lazy.Add<List<Sim>, Sim>(ref list, sim);
                    }
                }
            }
            if (list != null)
            {
                foreach (Sim sim2 in list)
                {
                    Sim.MakeSimGoHome(sim2, false);
                }
            }
        }

        // Token: 0x06006FC1 RID: 28609 RVA: 0x0026D4A8 File Offset: 0x0026C4A8
        private void MermaidDehydratedToGhostSequence()
        {
            this.StartGhostExplosion();
            this.mSMCDeath.RequestState(true, "y", "PoseDehydrate");
            this.Target.FadeOut();
            this.mDeathEffect = this.mGrave.GetSimToGhostEffect(this.Target, this.mGhostPosition);
            if (this.mDeathEffect != null)
            {
                this.mSMCDeath.SetEffectActor("deathEffect", this.mDeathEffect);
                this.mDeathEffect.Start();
            }
            this.Target.SetPosition(this.mGhostPosition);
            this.mGrave.GhostSetup(this.Target, false);
            this.Target.SetHiddenFlags(HiddenFlags.Nothing);
            if (this.mDeathEffect != null)
            {
                this.mDeathEffect.Stop();
                this.mDeathEffect = null;
            }
            this.Target.FadeIn();
            this.mSMCDeath.RequestState(true, "y", "DehydrateToFloat");
            this.mSMCDeath.RequestState(false, "y", "GhostFloating");
            this.StopGhostExplosion();
        }

        // Token: 0x04005A95 RID: 23189
        public static readonly InteractionDefinition Singleton = new Definition();

        public StateMachineClient SMCDeath;


        // Token: 0x04005A96 RID: 23190
        public StateMachineClient mSMCDeath;

        // Token: 0x04005A97 RID: 23191
        public Urnstone mGrave;

        // Token: 0x04005A98 RID: 23192
        public Vector3 mGhostPosition;

        // Token: 0x04005A99 RID: 23193
        public VisualEffect mDeathEffect;

        // Token: 0x04005A9A RID: 23194
        public Household mDeadSimsHousehold;

        // Token: 0x04005A9B RID: 23195
        public GrimReaperSituation mSituation;

        // Token: 0x04005A9C RID: 23196
        public GrimReaperSituation.ReapSoul.DeathProgress mDeathProgress;

        // Token: 0x04005A9D RID: 23197
        public bool mWasMemberOfActiveHousehold = false;

        // Token: 0x04005A9E RID: 23198
        public DeathFlower mDeathFlower;

        // Token: 0x04005A9F RID: 23199
        private VisualEffect mGhostExplosion;

        // Token: 0x02001DA9 RID: 7593
        [DoesntRequireTuning]
        private sealed class Definition : InteractionDefinition<Sim, Sim, TheNiecReapSoul>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            // Token: 0x0600EC76 RID: 60534 RVA: 0x00484CD3 File Offset: 0x00483CD3
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return true;
            }
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "ReapSoul Master";
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

        // Token: 0x02001DAA RID: 7594
        public enum DeathProgress
        {
            // Token: 0x0400B324 RID: 45860
            None,
            // Token: 0x0400B325 RID: 45861
            GraveCreated,
            // Token: 0x0400B326 RID: 45862
            GravePlaced,
            // Token: 0x0400B327 RID: 45863
            DeathFlowerStarted,
            // Token: 0x0400B328 RID: 45864
            DeathFlowerPostEvent,
            // Token: 0x0400B329 RID: 45865
            UnluckyStarted,
            // Token: 0x0400B32A RID: 45866
            UnluckyPostEvent,
            // Token: 0x0400B32B RID: 45867
            NormalStarted,
            // Token: 0x0400B32C RID: 45868
            Complete
        }
    }
}
