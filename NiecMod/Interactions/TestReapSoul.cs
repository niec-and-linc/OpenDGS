/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 17/09/2018
 * Time: 1:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.ChildAndTeenUpdates;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.StoryProgression;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;


using NiecMod.Nra;
using NiecMod.KillNiec;

namespace NiecMod.Interactions
{
    public sealed class ReapSoul : SocialInteraction
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        public ulong GraveObjectId
        {
            get
            {
                return this.mGrave.ObjectId.Value;
            }
        }

        // Token: 0x06009AEA RID: 39658 RVA: 0x0035007C File Offset: 0x0034E27C
        public override void Cleanup()
        {
            try
            {
                if (this.mDeathProgress != GrimReaperSituation.ReapSoul.DeathProgress.Complete)
                {
                    if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.None)
                    {
                        this.mGrave = Urnstone.CreateGrave(this.Target.SimDescription, true, false);
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GraveCreated;
                    }
                    if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.GraveCreated)
                    {
                        if (!GlobalFunctions.PlaceAtGoodLocation(this.mGrave, new World.FindGoodLocationParams(this.Target.Position), false))
                        {
                            this.mGrave.SetPosition(this.Target.Position);
                        }
                        this.mGrave.OnHandToolMovement();
                        this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GravePlaced;
                    }
                    if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.GravePlaced)
                    {
                        this.mGrave.GhostSetup(this.Target, false);
                    }
                    if (this.mDeathFlower == null)
                    {
                        this.mDeathFlower = this.Target.Inventory.Find<DeathFlower>();
                    }
                    if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.UnluckyStarted || (this.Target.Household != null && this.Target.IsInActiveHousehold && this.Target.SimDescription.DeathStyle != (SimDescription.DeathType)69u))
                    {
                        this.EventCallbackResurrectSim();
                        this.Target.AddExitReason(ExitReason.HigherPriorityNext);
                    }
                    else if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.DeathFlowerStarted || this.mDeathFlower != null)
                    {
                        this.Target.Inventory.RemoveByForce(this.mDeathFlower);
                        this.mDeathFlower.Destroy();
                        this.EventCallbackResurrectSim();
                        this.Target.AddExitReason(ExitReason.HigherPriorityNext);
                    }
                    else if (this.mDeathProgress != GrimReaperSituation.ReapSoul.DeathProgress.UnluckyPostEvent)
                    {
                        if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.DeathFlowerPostEvent)
                        {
                            if (this.Target.Inventory.Contains(this.mDeathFlower))
                            {
                                this.Target.Inventory.RemoveByForce(this.mDeathFlower);
                            }
                            if (this.mDeathFlower != null)
                            {
                                this.mDeathFlower.Destroy();
                            }
                        }
                        else
                        {
                            this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
                        }
                    }
                    if (this.mDeathProgress == GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted)
                    {
                        this.FinalizeDeath();
                        this.CleanupAndDestroyDeadSim(true);
                    }
                    this.GrimReaperPostSequenceCleanup();
                    Urnstone.FogEffectTurnAllOff(this.Actor.LotCurrent);
                    this.StopGhostExplosion();
                }
                SMCDeath.EnterState("x", "Enter");
                base.Cleanup();
            }
            catch (Exception exception)
            {
                Target.FadeIn();
                NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                base.Cleanup();
            }
        }

        public Vector3 mGhostForward = Vector3.Invalid;

        // Token: 0x06009AEB RID: 39659 RVA: 0x00350290 File Offset: 0x0034E490
        public override bool Run()
        {
            try
            {
                /*
                this.mSituation = (ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation);
                //this.mSituation.AddRelationshipWithEverySimInHousehold();
                this.Actor.SetPosition(this.mSituation.Lot.Position);
                this.mSituation.ScaredReactionBroadcaster = new ReactionBroadcaster(this.Actor, GrimReaperSituation.ScaredParams, new ReactionBroadcaster.BroadcastCallback(GrimReaperSituation.ScaredDelegate));
                try
                {
                    Sim sim = Target;
                    if (sim != null)
                    {
                        Matrix44 transform = sim.Transform;
                        Matrix44 invalid = Matrix44.Invalid;
                        Vector3 position = Actor.Position;
                        float num = (this.Actor.Position - sim.Position).LengthSqr();
                        if (num < 0.25f || num > 4f)
                        {
                            double @double = RandomUtil.GetDouble(6.2831853071795862);
                            position = sim.Position + new Vector3((float)Math.Sin(@double), 0f, (float)Math.Cos(@double));
                        }
                        this.Actor.SetForward(sim.PositionOnFloor - this.Actor.PositionOnFloor);
                        this.Actor.SetForward(new Vector3(this.Actor.ForwardVector.x, 0f, this.Actor.ForwardVector.z));
                        SimDescription.DeathType deathStyle = sim.SimDescription.DeathStyle;
                    }
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("1a" + exception.Message + NiecException.NewLine + exception.StackTrace);
                }
                
                this.Actor.GreetSimOnLot(this.mSituation.Worker.LotCurrent);
                try
                {
                    this.mSituation.SMCDeath = StateMachineClient.Acquire(this.Actor, "Death");
                    this.mSituation.SMCDeath.SetActor("x", this.Actor);
                    this.mSituation.SMCDeath.EnterState("x", "Enter");
                    this.mSituation.SMCDeath.AddOneShotScriptEventHandler(666u, new SacsEventHandler(this.EventCallbackFadeInReaper));
                    this.mSituation.StartGrimReaperSmoke();
                    this.mSituation.SMCDeath.RequestState("x", "Float");
                    this.Actor.Posture = new SimCarryingObjectPosture(this.Actor, null);
                    this.mWasMemberOfActiveHousehold = (this.Target.Household == Household.ActiveHousehold);
                    if (this.Target.DeathReactionBroadcast == null)
                    {
                        Urnstone.CreateDeathReactionBroadcaster(this.Target);
                    }
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("2" + exception.Message + NiecException.NewLine + exception.StackTrace);
                }
                try
                {
                    this.Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                    this.Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                    this.mDeadSimsHousehold = this.Target.Household;
                    this.mSMCDeath = this.mSituation.SMCDeath;
                    ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("moodlet_mourning", this.Target.GetThoughtBalloonThumbnailKey());
                    balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                    balloonData.mPriority = ThoughtBalloonPriority.High;
                    this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("3" + exception.Message + NiecException.NewLine + exception.StackTrace);
                }
                try
                {
                    var killSim = this.Target.CurrentInteraction as ExtKillSimNiec;
                    if (killSim != null)
                    {
                        killSim.StartDeathEffect();
                    }
                    Audio.StartSound("sting_death", this.Actor.Position);
                    this.mSMCDeath.RequestState("x", "take_sim");
                    this.mSMCDeath.RequestState("x", "Exit");
                    this.Target.FadeOut();
                    this.FinalizeDeath();
                    this.GrimReaperPostSequenceCleanup();
                    this.Target.StartOneShotFunction(new Sims3.Gameplay.Function(this.ReapSoulCallback), GameObject.OneShotFunctionDisposeFlag.OnDispose);
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("4" + exception.Message + NiecException.NewLine + exception.StackTrace);
                }
                return true;
                 */
                //var grimReaperSituation = ServiceSituation.FindServiceSituationInvolving as ;
                //grimReaperSituation.AddRelationshipWithEverySimInHousehold();

                //grimReaperSituation.ScaredReactionBroadcaster = new ReactionBroadcaster(this.Actor, GrimReaperSituation.ScaredParams, new ReactionBroadcaster.BroadcastCallback(GrimReaperSituation.ScaredDelegate));

                this.Actor.SetPosition(Actor.Position);
                
                SimDescription.DeathType deathType = SimDescription.DeathType.Drown;
                
                try
                {
                    Sim sim = FindClosestDeadSim();
                    if (sim != null)
                    {
                        Vector3 position = Vector3.Invalid;
                        World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(sim.Position);
                        Vector3 vector;
                        if (!GlobalFunctions.FindGoodLocation(this.Actor, fglParams, out position, out vector))
                        {
                            position = fglParams.StartPosition;
                        }
                        this.Actor.SetPosition(position);
                        this.Actor.RouteTurnToFace(sim.Position);
                        deathType = sim.SimDescription.DeathStyle;
                    }
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("SAE" + exception.Message + NiecException.NewLine + exception.StackTrace);

                }
                
                //this.Actor.GreetSimOnLot(grimReaperSituation.Worker.LotCurrent);
                SMCDeath = StateMachineClient.Acquire(this.Actor, "DeathSequence");
                SMCDeath.SetActor("x", this.Actor);
                SMCDeath.EnterState("x", "Enter");
                Urnstone.FogEffectTurnAllOn(Actor.LotCurrent);
                VisualEffect visualEffect = Urnstone.ReaperApperEffect(this.Actor, deathType);
                visualEffect.Start();
                //grimReaperSituation.StartGrimReaperSmoke();
                VisualEffect.FireOneShotEffect("reaperSmokeConstant", Actor, Sim.FXJoints.Pelvis, VisualEffect.TransitionType.HardTransition);
                
                SMCDeath.AddOneShotScriptEventHandler(666u, new SacsEventHandler(this.EventCallbackFadeInReaper));
                SMCDeath.RequestState("x", "ReaperBrushingHimself");
                visualEffect.Stop();
                ReaperLoop = new ObjectSound(this.Actor.ObjectId, "death_reaper_lp");
                ReaperLoop.Start(true);
                this.Actor.Posture = new SimCarryingObjectPosture(this.Actor, null);
                return true;
            }
            catch (Exception exception)
            {
                NiecException.PrintMessage("ASF" + exception.Message + NiecException.NewLine + exception.StackTrace);
                return true;
            }
        }

        // Token: 0x06009AEC RID: 39660 RVA: 0x00350F18 File Offset: 0x0034F118
        public bool CreateGraveStone()
        {
            var killSim = this.Target.CurrentInteraction as ExtKillSimNiec;
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
            this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GraveCreated;
            return true;
        }

        // Token: 0x06009AED RID: 39661 RVA: 0x00351000 File Offset: 0x0034F200
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

        // Token: 0x06009AEE RID: 39662 RVA: 0x003510D0 File Offset: 0x0034F2D0
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

        // Token: 0x06009AEF RID: 39663 RVA: 0x00351170 File Offset: 0x0034F370
        private bool RouteGrimReaperToEdgeOfTargetPool()
        {
            Pool pool = this.Target.Posture.Container as Pool;
            return pool != null && pool.RouteToEdge(this.Actor);
        }

        // Token: 0x06009AF0 RID: 39664 RVA: 0x003511A4 File Offset: 0x0034F3A4
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

        public Sim FindClosestDeadSim()
        {
            Sim closestObject = GlobalFunctions.GetClosestObject<Sim>(Actor, false, true, true, 0, this.ignoreList, new GlobalFunctions.FindObjectValidityTest(this.VerifyDeadSimSkipSelected));
            if (closestObject == null)
            {
                closestObject = GlobalFunctions.GetClosestObject<Sim>(Actor, false, true, true, 0, this.ignoreList, new GlobalFunctions.FindObjectValidityTest(this.VerifyDeadSim));
            }
            return closestObject;
        }

        public bool VerifyDeadSim(IGameObject simToCheck, ref float score)
        {
            Sim sim = simToCheck as Sim;
            return sim != null && (sim.SimDescription.DeathStyle != SimDescription.DeathType.None && !sim.SimDescription.IsGhost);
        }

        public bool VerifyDeadSimSkipSelected(IGameObject simToCheck, ref float score)
        {
            if (this.VerifyDeadSim(simToCheck, ref score))
            {
                Sim sim = simToCheck as Sim;
                return !sim.IsActiveSim;
            }
            return false;
        }

        private List<Sim> ignoreList = new List<Sim>();
        // Token: 0x06009AF1 RID: 39665 RVA: 0x0035126C File Offset: 0x0034F46C
        public void EventCallbackSimToGhostEffectNoFadeOut(StateMachineClient sender, IEvent evt)
        {
            this.mDeathEffect = this.mGrave.GetSimToGhostEffect(this.Target, this.mGhostPosition);
            this.mSMCDeath.SetEffectActor("deathEffect", this.mDeathEffect);
            this.mDeathEffect.Start();
        }


        public void EventCallbackFadeInReaper(StateMachineClient sender, IEvent evt)
        {
            try
            {
                this.Actor.FadeIn(false, 3f);
            }
            catch (Exception exception)
            {
                NiecException.PrintMessage("EventCallbackFadeInReaper" + exception.Message + NiecException.NewLine + exception.StackTrace);
            }
            
        }
        // Token: 0x06009AF2 RID: 39666 RVA: 0x003512B8 File Offset: 0x0034F4B8
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

        // Token: 0x06009AF3 RID: 39667 RVA: 0x0005D7EB File Offset: 0x0005B9EB
        private void StopGhostExplosion()
        {
            if (this.mGhostExplosion != null)
            {
                this.mGhostExplosion.Stop();
                this.mGhostExplosion = null;
            }
        }

        // Token: 0x06009AF4 RID: 39668 RVA: 0x00351334 File Offset: 0x0034F534
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

        // Token: 0x06009AF5 RID: 39669 RVA: 0x00351370 File Offset: 0x0034F570
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

        // Token: 0x06009AF6 RID: 39670 RVA: 0x003514E0 File Offset: 0x0034F6E0
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

        // Token: 0x06009AF7 RID: 39671 RVA: 0x00351564 File Offset: 0x0034F764
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

        // Token: 0x06009AF8 RID: 39672 RVA: 0x00351610 File Offset: 0x0034F810
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

        // Token: 0x06009AF9 RID: 39673 RVA: 0x0035166C File Offset: 0x0034F86C
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

        // Token: 0x06009AFA RID: 39674 RVA: 0x0005D808 File Offset: 0x0005BA08
        public void GrimReaperPostSequenceCleanup()
        {
            this.Actor.Posture = this.Actor.Standing;
        }

        // Token: 0x06009AFB RID: 39675 RVA: 0x003516EC File Offset: 0x0034F8EC
        public void FinalizeDeath()
        {
            if (Target.IsInActiveHousehold)
            {
                return;
            }
            this.MakeHouseholdHorsesGoHome();
            if (this.Target.SimDescription.IsEnrolledInBoardingSchool())
            {
                this.Target.SimDescription.BoardingSchool.OnRemovedFromSchool();
            }
            Urnstone.FinalizeSimDeath(this.Target.SimDescription, this.Target.Household, false);
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

        // Token: 0x06009AFC RID: 39676 RVA: 0x00351A00 File Offset: 0x0034FC00
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

        // Token: 0x06009AFD RID: 39677 RVA: 0x00351A84 File Offset: 0x0034FC84
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

        // Token: 0x06009AFE RID: 39678 RVA: 0x00351B84 File Offset: 0x0034FD84
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

        // Token: 0x04007637 RID: 30263
        public ReactionBroadcaster ScaredReactionBroadcaster;

        // Token: 0x04007638 RID: 30264
        public Urnstone LastGraveCreated;

        // Token: 0x04007639 RID: 30265
        public Sim ChessOpponent;

        // Token: 0x0400763A RID: 30266
        public IChessTable ChessTable;

        // Token: 0x0400763B RID: 30267
        public IVendingMachine VendingMachine;

        // Token: 0x0400763C RID: 30268
        public bool ChessMatchWon;

        // Token: 0x0400763D RID: 30269
        public bool FadeDeathIn;

        // Token: 0x0400763E RID: 30270
        public Sim LastSimOfHousehold;

        // Token: 0x0400763F RID: 30271
        public ObjectSound ReaperLoop;

        // Token: 0x04007640 RID: 30272
        public StateMachineClient SMCDeath;

        // Token: 0x04007641 RID: 30273
        public Sim PetSavior;

        // Token: 0x04007642 RID: 30274
        public VisualEffect mGrimReaperSmoke;

        // Token: 0x04007643 RID: 30275
        public bool mIsFirstSim = true;

        // Token: 0x04007644 RID: 30276
        public SocialJig mScubaDeathJig;

        // Token: 0x0400764D RID: 30285

        public StateMachineClient mSptat;

        // Token: 0x0400764E RID: 30286
        public StateMachineClient mSMCDeath;

        // Token: 0x0400764F RID: 30287
        public Urnstone mGrave;

        // Token: 0x04007650 RID: 30288
        public Vector3 mGhostPosition;

        // Token: 0x04007651 RID: 30289
        public VisualEffect mDeathEffect;

        // Token: 0x04007652 RID: 30290
        public Household mDeadSimsHousehold;

        // Token: 0x04007653 RID: 30291
        public GrimReaperSituation mSituation;

        // Token: 0x04007654 RID: 30292
        public GrimReaperSituation.ReapSoul.DeathProgress mDeathProgress;

        // Token: 0x04007655 RID: 30293
        public bool mWasMemberOfActiveHousehold = true;

        // Token: 0x04007656 RID: 30294
        public DeathFlower mDeathFlower;

        // Token: 0x04007657 RID: 30295
        public VisualEffect mGhostExplosion;


        [DoesntRequireTuning]

        private sealed class Definition : InteractionDefinition<Sim, Sim, ReapSoul>, IAllowedOnClosedVenues, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow //ActorTrailer
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "Test ReapSoulX";
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;
                /*
                if (actor.IsNPC)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Death Good System: No Allow NPC");
                    return false;
                }

                if (target.IsInActiveHousehold)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Death Good System: Not Allow Active Household and Allow NPC to Sim");
                    return false;
                }

                if (target.Service is GrimReaper)
                {
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(actor.Name + ": Can't ReapSoul To " + target.Name);
                    return false;
                }
                */
                return true;
            }
            public override string[] GetPath(bool bPath)
            {
                return new string[] { "Death Good System Options..." };
            }
        }
    }
}
