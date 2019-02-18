/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 21/09/2018
 * Time: 1:42
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
using NiecMod.Interactions.Objects;
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
    /// <summary>
    /// Description of Class1.
    /// </summary>
    public class TriasTrvalKi : GameObject, IFutureObject, IGameObject, IDoNotGetFogged, ICannotBeDestroyedByMeteor
    {

        public override bool HandToolAllowUserPickup()
        {
            //return false;
            return true;
        }
    

        public enum TraisKill { TraisKillE };

        //public enum InteractionPriorityLevel{};

        // Methods
        public override void OnStartup()
        {
            base.OnStartup();

            base.AddInteraction(Trias_Time.Singleton);
            
            base.AddInteraction(EnableAndDisble.Singleton);
            
            base.AddInteraction(TriasTrvalKiAsktoAdd.Singleton);

            base.AddInventoryInteraction(Trias_Time.Singleton);

            base.AddInventoryInteraction(TriasTrvalKiAsktoAdd.Singleton);
            
            base.AddInventoryInteraction(EnableAndDisble.Singleton);

        }
        /*
        public static bool PreTimeTravel1(InteractionInstance ths)
        {

            Sim actor = ths.InstanceActor as Sim;

            TravelUtil.PlayerMadeTravelRequest = true;
            OpportunityNames guid = CheckOpportunities(actor);

            //Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Ui/Caption/HUD/DisasterSaveError:Traveling");
            ths.CancellableByPlayer = false;
            if ((guid != OpportunityNames.Undefined) && (actor.OpportunityManager != null))
            {
                actor.OpportunityManager.CancelOpportunity(guid);
            }

            if (actor.OpportunityManager != null)
            {
                if (actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler01))
                {
                    actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler01);
                }

                if (actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler02))
                {
                    actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_HelpingTheTimeTraveler02);
                }

                if (GameUtils.IsFutureWorld() && actor.OpportunityManager.HasOpportunity(OpportunityNames.EP11_RecalibrateDefenseGrid))
                {
                    actor.OpportunityManager.CancelOpportunity(OpportunityNames.EP11_RecalibrateDefenseGrid);
                }
            }

            return true;
        }
        */
        private static OpportunityNames CheckOpportunities(Sim actor)
        {
            for (int i = 0x0; i < TimePortal.kBannedOpportunities.Length; i++)
            {
                if ((actor.OpportunityManager != null) && actor.OpportunityManager.HasOpportunity(TimePortal.kBannedOpportunities[i]))
                {
                    return TimePortal.kBannedOpportunities[i];
                }
            }
            return OpportunityNames.Undefined;
        }


        public static bool PreTrias(InteractionInstance ths)
        {
            //Sim actor = ths.InstanceActor as Sim;

            ths.StandardEntry();
            ths.BeginCommodityUpdates();
            /*
            BuffTransformation transformBuff = actor.BuffManager.TransformBuff;
            if (transformBuff != null)
            {
                actor.BuffManager.RemoveElement(transformBuff.Guid);
            }
            */

            return true;
        }

        public void SwitchActiveState()
        {
            if (this.State != TimePortal.PortalState.Inactive)
            {
                this.State = TimePortal.PortalState.Inactive;
                return;
            }
            if (CauseEffectService.GetInstance().GetCurrentCauseEffectWorldState() == CauseEffectWorldState.kUtopiaState)
            {
                this.State = TimePortal.PortalState.Rainbow;
                return;
            }
            if (CauseEffectService.GetInstance().GetCurrentCauseEffectWorldState() == CauseEffectWorldState.kDystopiaState)
            {
                this.State = TimePortal.PortalState.Distopia;
                return;
            }
            this.State = TimePortal.PortalState.Active;
        }

        public TimePortal.PortalState State
        {
            get
            {
                return this.mCurState;
            }
            set
            {
                this.UpdateState(value);
            }
        }

        public void UpdateState(TimePortal.PortalState newState)
        {
            this.UpdateState(newState, false);
        }

        public void UpdateState(TimePortal.PortalState newState, bool fromFixUp)
        {
            if (this.State == newState && !fromFixUp)
            {
                return;
            }
            this.StopActiveFX();
            switch (newState)
            {
                case TimePortal.PortalState.Active:
                    this.mActiveFX = VisualEffect.Create("ep11Portal_main");
                    this.mActiveFX.SetUseBoneParenting(true);
                    this.mActiveFX.ParentTo(this, TimePortal.kEffectSlot);
                    this.mActiveFX.Start();
                    if (this.State == TimePortal.PortalState.Inactive)
                    {
                        this.AddActivateAlarm();
                    }
                    this.RemoveUnstableAlarm();
                    this.RemoveChargedAlarm();
                    break;
                case TimePortal.PortalState.Charged:
                    this.mActiveFX = VisualEffect.Create("ep11PortalCharged_main");
                    this.mActiveFX.SetUseBoneParenting(true);
                    this.mActiveFX.ParentTo(this, TimePortal.kEffectSlot);
                    this.mActiveFX.Start();
                    this.AddChargedAlarm();
                    this.RemoveUnstableAlarm();
                    break;
                case TimePortal.PortalState.Unstable:
                    this.mActiveFX = VisualEffect.Create("ep11PortalUnstable_main");
                    this.mActiveFX.SetUseBoneParenting(true);
                    this.mActiveFX.ParentTo(this, TimePortal.kEffectSlot);
                    this.mActiveFX.Start();
                    this.AddUnstableAlarm();
                    this.RemoveChargedAlarm();
                    break;
                case TimePortal.PortalState.Rainbow:
                    this.mActiveFX = VisualEffect.Create("ep11PortalRainbow_main");
                    this.mActiveFX.SetUseBoneParenting(true);
                    this.mActiveFX.ParentTo(this, TimePortal.kEffectSlot);
                    this.mActiveFX.Start();
                    if (this.State == TimePortal.PortalState.Inactive)
                    {
                        this.AddActivateAlarm();
                    }
                    this.RemoveUnstableAlarm();
                    this.RemoveChargedAlarm();
                    break;
                case TimePortal.PortalState.Distopia:
                    this.mActiveFX = VisualEffect.Create("ep11PortalDystopia_main");
                    this.mActiveFX.SetUseBoneParenting(true);
                    this.mActiveFX.ParentTo(this, TimePortal.kEffectSlot);
                    this.mActiveFX.Start();
                    if (this.State == TimePortal.PortalState.Inactive)
                    {
                        this.AddActivateAlarm();
                    }
                    this.RemoveUnstableAlarm();
                    this.RemoveChargedAlarm();
                    break;
                default:
                    this.RemoveUnstableAlarm();
                    this.RemoveChargedAlarm();
                    this.InactivePortalGeometry();
                    break;
            }
            this.mCurState = newState;
        }

        public void RemoveActivateAlarm()
        {
            if (this.mActivateAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mActivateAlarm);
                this.mActivateAlarm = AlarmHandle.kInvalidHandle;
            }
        }

        // Token: 0x0600C734 RID: 50996 RVA: 0x0029F4E8 File Offset: 0x0029E4E8
        public void AddActivateAlarm()
        {
            if (this.mActivateAlarm == AlarmHandle.kInvalidHandle)
            {
                this.mActivateAlarm = base.AddAlarm(TimePortal.kPortalActivateTimer, TimeUnit.Minutes, new AlarmTimerCallback(this.ActivePortalGeometry), "Activate timer", AlarmType.AlwaysPersisted);
            }
        }

        // Token: 0x0600C735 RID: 50997 RVA: 0x0029F520 File Offset: 0x0029E520
        public void RemoveUnstableAlarm()
        {
            if (this.mUnstableAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mUnstableAlarm);
                this.mUnstableAlarm = AlarmHandle.kInvalidHandle;
            }
        }

        // Token: 0x0600C736 RID: 50998 RVA: 0x0029F54B File Offset: 0x0029E54B
        public void AddUnstableAlarm()
        {
            if (this.mUnstableAlarm == AlarmHandle.kInvalidHandle)
            {
                this.mUnstableAlarm = base.AddAlarm(TimePortal.kPortalUnstableTimer, TimeUnit.Minutes, new AlarmTimerCallback(this.UnChargePortal), "Charged timer", AlarmType.AlwaysPersisted);
            }
        }

        // Token: 0x0600C737 RID: 50999 RVA: 0x0029F583 File Offset: 0x0029E583
        public void RemoveChargedAlarm()
        {
            if (this.mChargedAlarm != AlarmHandle.kInvalidHandle)
            {
                base.RemoveAlarm(this.mChargedAlarm);
                this.mChargedAlarm = AlarmHandle.kInvalidHandle;
            }
        }

        public void StopActiveFX()
        {
            if (this.mActiveFX != null)
            {
                this.mActiveFX.Stop();
                this.mActiveFX.Dispose();
                this.mActiveFX = null;
            }
        }

        // Token: 0x0600C738 RID: 51000 RVA: 0x0029F5AE File Offset: 0x0029E5AE
        public void AddChargedAlarm()
        {
            if (this.mChargedAlarm == AlarmHandle.kInvalidHandle)
            {
                this.mChargedAlarm = base.AddAlarm(TimePortal.kPortalChargedTimer, TimeUnit.Minutes, new AlarmTimerCallback(this.UnChargePortal), "Charged timer", AlarmType.AlwaysPersisted);
            }
        }

        public void UnChargePortal()
        {
            this.State = TimePortal.PortalState.Active;
        }

        public void ActivePortalGeometry()
        {
            base.SetGeometryState("on");
            this.RemoveActivateAlarm();
        }

        // Token: 0x0600C73C RID: 51004 RVA: 0x0029F60B File Offset: 0x0029E60B
        public void InactivePortalGeometry()
        {
            base.SetGeometryState("off");
            this.RemoveActivateAlarm();
        }

        public TimePortal.PortalState mCurState;

        public VisualEffect mActiveFX;

        // Token: 0x0400544A RID: 21578
        public bool mShowMapTag;

        // Token: 0x0400544B RID: 21579
        public bool mIsBroken;

        // Token: 0x0400544C RID: 21580
        public bool mTimeToStepBack;

        // Token: 0x0400544D RID: 21581
        public AlarmHandle mActivateAlarm = AlarmHandle.kInvalidHandle;

        // Token: 0x0400544E RID: 21582
        public AlarmHandle mUnstableAlarm = AlarmHandle.kInvalidHandle;

        // Token: 0x0400544F RID: 21583
        public AlarmHandle mChargedAlarm = AlarmHandle.kInvalidHandle;

        public static Slot[] kRoutingSlots = new Slot[]
		{
			Slot.RoutingSlot_0,
			Slot.RoutingSlot_1,
			Slot.RoutingSlot_2,
			Slot.RoutingSlot_3
		}; 
        
        public bool Active
        {
            get
            {
                return this.State != TimePortal.PortalState.Inactive;
            }
            set
            {
                this.State = (value ? TimePortal.PortalState.Active : TimePortal.PortalState.Inactive);
            }
        }
        
        
        public sealed class Trias_Time : Interaction<Sim, TriasTrvalKi>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();
            //private SacsEventHandler CameraShakeEvent;

            public void CameraShakeEvent(StateMachineClient smc, IEvent evt)
            {
                this.mPortal.ShakeCamera();
                List<Sim> list = new List<Sim>(1);
                list.Add(this.Actor);
                this.mPortal.MakeNearbySimsReact(list);
            }

            
            // Methods
            public override bool Run() // Run
            {
            	try
                {
                    if (Actor.SimDescription.IsPet || Actor.Service is GrimReaper || Actor.SimDescription.ChildOrBelow)
                    {
                        StyledNotification.Show(new StyledNotification.Format("Sorry, Can't Run Travel to Oais Eirts", StyledNotification.NotificationStyle.kGameMessagePositive));
                        return false;
                    }
                    if (Actor.IsInActiveHousehold)
                    {
                    	if (!Actor.IsSelectable)
                    	{
                    		Actor.SimDescription.IsNeverSelectable = false;
                    		Actor.FadeIn();
                    	}
                    }
                    this.Target.State = TimePortal.PortalState.Active;
                    SetPriority((InteractionPriorityLevel)999, 1f);
                    this.OnPriorityChanged();
                    int num;
                    if (!this.Actor.RouteToSlotList(this.Target, TriasTrvalKi.kRoutingSlots, out num))
                    //if (!this.Actor.RouteToSlotListAndCheckInUse(this.Target, TriasTrvalKi.kRoutingSlots, out num))
                    {
                        return false;
                    }

                    //if (!PreTrias(this)) return true;
                    StandardEntry();
                    BeginCommodityUpdates();
                    EnterStateMachine("timeportal", "Enter", "x", "portal");
                    //AddPersistentScriptEventHandler(0xc9, CameraShakeEvent);
                    AnimateSim("Call Over");
                    /*
                    Sim targetSim = base.GetSelectedObject() as Sim;
                    {
                        targetSim.InteractionQueue.Add(Singleton.CreateInstance(Target, targetSim, new InteractionPriority((InteractionPriorityLevel)999), false, true));
                    }
                    if (Actor.IsInActiveHousehold)
                    {
                        Sim targetsim = GetSelectedObject() as Sim;
                        if (targetsim != null)
                        {
                            InteractionInstance entry = Singleton.CreateInstance(Target, targetsim, Actor.InheritedPriority(), Autonomous, true);
                            InteractionInstance instance = Singleton.CreateInstance(Target, targetsim, Actor.InheritedPriority(), Autonomous, true);
                            entry.LinkedInteractionInstance = instance;
                        }
                    }

                    AnimateSim("Repair");
                    */
                    Skill futureSkill = Actor.SkillManager.AddElement(SkillNames.Future);
                    if (futureSkill.SkillLevel >= 0x3)
                    {
                        AnimateSim("Jump In");
                    }
                    else
                    {
                        AnimateSim("Apprehensive");
                    }
                    if (!Actor.IsInActiveHousehold)
                    {
                        //if (Actor.OccultManager.HasOccultType(OccultTypes.TimeTraveler)) // Not Modifed The Sims 3
                        if (!Actor.CanBeKilled()) // My Mod is CanBeKilled Not Modifed The Sims 3 is File Dll Gameplay 
                        {
                            KillSimNiecX.MineKill(Actor, SimDescription.DeathType.Causality);
                            SpeedTrap.Sleep(10);
                            return false;
                        }
                        Actor.Kill(SimDescription.DeathType.Causality);
                        SpeedTrap.Sleep(10);
                        return false;
                    }
                    SpeedTrap.Sleep(10);
                    MustRun = true;
                    CancellableByPlayer = true;
                    Actor.CanIndividualSimReact = false;
                    Actor.SimDescription.AgingEnabled = false;
                    SpeedTrap.Sleep(1);
                    CameraController.Shake();
                    this.Target.State = TimePortal.PortalState.Inactive;
                    this.Actor.Motives.MaxEverything();
                    Actor.Motives.FreezeDecayEverythingExcept(new CommodityKind[0]);
                    SpeedTrap.Sleep(30);
                    Actor.SimDescription.MotivesDontDecay = true;
                    bool flag = base.DoLoop(ExitReason.UserCanceled);
                    SpeedTrap.Sleep(400);
                    //AnimateSim("ResetTime Continuum");
                    this.Actor.Motives.MaxEverything();
                    foreach (InteractionInstance interactionInstance in Actor.InteractionQueue.InteractionList) // Cant Cancel Fix
                	{
                    	interactionInstance.MustRun = false;
                    	interactionInstance.Hidden = false;
                	}
                    this.Actor.InteractionQueue.CancelAllInteractions();
                    this.Target.State = TimePortal.PortalState.Active;
                    /*
                    if (futureSkill.SkillLevel >= 0x3)
                    {
                        AnimateSim("Exit");
                    }
                    else
                    {
                        AnimateSim("Spit Out");
                    }
                    */
                    //Actor.FadeIn();
                    //Target.SwitchActiveState();

                    //AnimateSim("ResetTime Continuum");
                    //AnimateSim("Exit");
                    AnimateSim("Spit Out");
                    EndCommodityUpdates(true);
                    StandardExit();
                    //base.AddPersistentScriptEventHandler(201u, new SacsEventHandler(this.CameraShakeEvent));
                    Actor.FadeIn();
                    return true;
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage(exception.Message + exception.StackTrace);
                    NRaas.Common.Exception(Actor, exception);
                }
                return false;
            }
            

            public override void Cleanup()
            {
            	Actor.SimDescription.MotivesDontDecay = false;
            	Actor.Motives.RestoreDecays();
                Target.FadeIn();
            	Actor.CanIndividualSimReact = true;
            	this.Target.State = TimePortal.PortalState.Inactive;
                base.Cleanup();
            }


            public override bool RunFromInventory()
            {
                return true;
            }

            public bool CancelTutorial()
            {
                return TwoButtonDialog.Show(Localization.LocalizeString("Gameplay/Tutorial:AreYouSure", new object[0]), Localization.LocalizeString("Gameplay/Tutorial:End", new object[0]), Localization.LocalizeString("Gameplay/Tutorial:Continue", new object[0]), true);
            }

            public void ShakeCamera(Vector3 position)
            {
                float num = (CameraController.GetPosition() - position).Length();
                num = MathUtils.Clamp(num, TimePortal.kCameraShakeDistanceRange[0], TimePortal.kCameraShakeDistanceRange[1]);
                float intensity = TimePortal.kCameraIntensityRange[1] - MathHelpers.LinearInterpolate(TimePortal.kCameraShakeDistanceRange[0], TimePortal.kCameraShakeDistanceRange[1], TimePortal.kCameraIntensityRange[0], TimePortal.kCameraIntensityRange[1], num);
                CameraController.Shake(intensity, TimePortal.kCameraShakeDuration);
            }


            //public TActor Actor;
            
            public ITimePortal mPortal;

            //public new TTarget Target;

            // Nested Types
            
            [DoesntRequireTuning]
            
            private class Definition : InteractionDefinition<Sim, TriasTrvalKi, TriasTrvalKi.Trias_Time>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
            {
                // Methods
                public override string GetInteractionName(Sim actor, TriasTrvalKi target, InteractionObjectPair interaction)
                {
                	return "Travel to the Oais Eirts World";
                    //return "Travel to Future Earth";
                }
                /*
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 2;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim.SimDescription.ToddlerOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }
                */

                public override bool Test(Sim a, TriasTrvalKi target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    //if (a.IsNPC) return false;
                    
                    if (a.SimDescription.IsPet || a.Service is GrimReaper || a.SimDescription.ChildOrBelow)
                    {
                    	greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Sorry, Can't Run Travel to Oais Eirts");
                    	return false;
                	}
                    return true;
                }
                public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            	{
                	return SpecialCaseAgeTests.None;
            	}
            }
        }
    }
}


