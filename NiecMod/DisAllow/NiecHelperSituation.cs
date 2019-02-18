namespace Sims3.Gameplay.NiecRoot
{
    #region Using Directives
    using System;
    using Sims3.Gameplay.Actors;
    using Sims3.Gameplay.Autonomy;
    using Sims3.Gameplay.Core;
    //using Sims3.Gameplay.Services;
    using Sims3.Gameplay.Socializing;
    using Sims3.Gameplay.Utilities;
    // New
    using Sims3.Gameplay.Interactions;
    using Sims3.SimIFace;
    using Sims3.Gameplay.CAS;
    using Sims3.Gameplay.ActorSystems;
    using Sims3.Gameplay.Objects;
    using Sims3.Gameplay.Interfaces;
    using System.Collections.Generic;
    using NiecMod.Nra;
    using NiecMod.KillNiec;
    using Sims3.Gameplay.Pools;
    using Sims3.Gameplay.ChildAndTeenUpdates;
    using Sims3.UI;
    using Sims3.Gameplay.EventSystem;
    using Sims3.SimIFace.CAS;
    using Sims3.Gameplay.PetSystems;
    using Sims3.Gameplay.Abstracts;
    using Sims3.Gameplay.ThoughtBalloons;
    using Sims3.Gameplay.Objects.Gardening;
    using Sims3.Gameplay.Controllers;
    using Sims3.Gameplay.Controllers.Niec;
    using NiecMod.Interactions;
    using Sims3.Gameplay.Scuba;

    #endregion

    public class NiecHelperSituation : RootSituation
    {
        public readonly Sim Worker;

        public bool OkSuusse = false;

        public bool OkSuusseD = false;
        // Methods
        protected NiecHelperSituation()
        { }

        public NiecHelperSituation(Lot lot, Sim worker)
            : base(lot)

        {
            Worker = worker;
        }


        public override void OnParticipantDeleted(Sim participant)
        {
            if (participant == Worker)
            {
                base.Exit();
            }
        }

        public static NiecHelperSituation Create(Lot lot, Sim workersim)
        {
            NiecHelperSituation niecHelperSituation = new NiecHelperSituation(lot, workersim);
            niecHelperSituation.SetState(new NiecHelperSituation.Spawn(niecHelperSituation));
            return niecHelperSituation;
        }



        public override void CleanUp()
        {
            try
            {
                OkSuusseD = false;
                OkSuusse = false;
                base.CleanUp();
                Exit();
            }
            catch (Exception)
            { }
            
        }




        // Lib New

        public StateMachineClient SMCDeath;

        public ObjectSound ReaperLoop;

        public List<Sim> ignoreList = new List<Sim>();




        // Lib New 2



        public ReactionBroadcaster ScaredReactionBroadcaster;

        public Urnstone LastGraveCreated;

        public Sim ChessOpponent;

        public IChessTable ChessTable;

        public IVendingMachine VendingMachine;

        public bool ChessMatchWon;

        public bool FadeDeathIn;

        public Sim LastSimOfHousehold;



        public Sim PetSavior;

        public VisualEffect mGrimReaperSmoke;

        public bool mIsFirstSim = true;

        public SocialJig mScubaDeathJig;


        // End Lib 2







        public Sim FindClosestDeadSim()
        {
            Sim closestObject = GlobalFunctions.GetClosestObject(Worker, false, true, true, 0, ignoreList, VerifyDeadSimSkipSelected);
            if (closestObject == null)
            {
                closestObject = GlobalFunctions.GetClosestObject(Worker, false, true, true, 0, ignoreList, VerifyDeadSim);
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


        public static bool ShouldDoDeathEvent(Sim deadSim)
        {
            if (deadSim != null && (deadSim.Household.IsActive || deadSim.LotCurrent == LotManager.ActiveLot || deadSim.LotCurrent == PlumbBob.SelectedActor.LotHome))
            {
                return true;
            }
            return false;
        }


        // End Lib 1








        // New


        public void AddRelationshipWithEverySimInHousehold()
        {
            try
            {
                if (Lot != null && Lot.IsResidentialLot && Lot.EffectiveHousehold != null)
                {
                    foreach (Sim allActor in Lot.EffectiveHousehold.AllActors)
                    {
                        Relationship relationship = Relationship.Get(Worker, allActor, true);
                        if (relationship != null)
                            relationship.MakeAcquaintances();
                    }
                }
            }
            catch
            { }

        }


























        public Sim FindClosestDeadDivingSim()
        {
            Sim closestObject = GlobalFunctions.GetClosestObject(Worker, false, true, true, 0, ignoreList, this.VerifyDeadDivingSimSkipSelected);
            if (closestObject == null)
            {
                closestObject = GlobalFunctions.GetClosestObject(Worker, false, true, true, 0, ignoreList, this.VerifyDeadDivingSim);
            }
            return closestObject;
        }

        public bool VerifyDeadDivingSim(IGameObject simToCheck, ref float score)
        {
            Sim sim = simToCheck as Sim;
            if (sim == null)
            {
                return false;
            }
            if (sim.Posture is ScubaDiving && sim.SimDescription.DeathStyle != 0 && !sim.SimDescription.IsGhost)
            {
                return true;
            }
            return false;
        }

        public bool VerifyDeadDivingSimSkipSelected(IGameObject simToCheck, ref float score)
        {
            if (VerifyDeadDivingSim(simToCheck, ref score))
            {
                Sim sim = simToCheck as Sim;
                return !sim.IsActiveSim;
            }
            return false;
        }




























        private void StartGrimReaperSmoke()
        {
            if (mGrimReaperSmoke == null)
            {
                mGrimReaperSmoke = VisualEffect.Create("reaperSmokeConstant");
                mGrimReaperSmoke.ParentTo(Worker, Sim.FXJoints.Pelvis);
                mGrimReaperSmoke.Start();
            }
        }




        

        private void StopGrimReaperSmoke()
        {
            if (mGrimReaperSmoke != null)
            {
                mGrimReaperSmoke.Stop();
                mGrimReaperSmoke.Dispose();
                mGrimReaperSmoke = null;
            }
        }











        public class ReapSoul : SocialInteraction
        {
            private sealed class Definition : InteractionDefinition<Sim, Sim, ReapSoul>, IDontNeedToBeCheckedInResort, IScubaDivingInteractionDefinition, IIgnoreIsAllowedInRoomCheck, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck,  IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return "ReapSoul Master";
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (actor == null) return false;
                    if (target == null) return false;
                    if (isAutonomous) return false;
                    NiecHelperSituation situationOfType = actor.GetSituationOfType<NiecHelperSituation>();
                    if (situationOfType == null) return false;
                    if (actor.IsSelectable && situationOfType.OkSuusse)
                    {
                        return true;
                    }
                    return false;
                }


                public override string[] GetPath(bool bPath)
                {
                    return new string[] { "Niec Helper Situation" };
                }

                public SpecialCaseAgeTests GetSpecialCaseAgeTests()
                {
                    return SpecialCaseAgeTests.None;
                }

                public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    try
                    {
                        if (!Test(parameters.Actor as Sim, parameters.Target as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                        {
                            return InteractionTestResult.Def_TestFailed;
                        }
                        return InteractionTestResult.Pass;
                    }
                    catch (Exception)
                    { return InteractionTestResult.GenericFail; }

                }
            }



            public enum DeathProgress
            {
                None,
                GraveCreated,
                GravePlaced,
                DeathFlowerStarted,
                DeathFlowerPostEvent,
                UnluckyStarted,
                UnluckyPostEvent,
                NormalStarted,
                Complete
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public StateMachineClient mSMCDeath;

            public Urnstone mGrave;

            public Vector3 mGhostPosition;

            public VisualEffect mDeathEffect;

            public Household mDeadSimsHousehold;

            public NiecHelperSituation mSituation;

            public DeathProgress mDeathProgress;

            public bool mWasMemberOfActiveHousehold = false;

            public DeathFlower mDeathFlower;

            public VisualEffect mGhostExplosion;




            public ulong GraveObjectId
            {
                get
                {
                    return mGrave.ObjectId.Value;
                }
            }
             
            public override void Cleanup()
            {
                if (Notfixdgs)
                {
                    return;
                }
                try
                {
                    /*
                    if (Target.Service is Sims3.Gameplay.Services.GrimReaper)
                    {
                        if (!TwoButtonDialog.Show("ReapSoul CleanUp: Killing the " + Target.Name + " [GrimReaper] will prevent souls to cross over to the other side. If this happens, Sims that die from now on will be trapped between this world and the next, and you'll end up with a city full of dead bodies laying around. Are you sure you want to kill Death itself?", "Yes", "No"))
                        {
                            Notfixdgs = true;
                            mDeathProgress = DeathProgress.Complete;
                            return;
                        }
                    }
                     * */
                    if (Target.SimDescription.DeathStyle == SimDescription.DeathType.None && !Target.IsInActiveHousehold)
                    {
                        List<SimDescription.DeathType> list = new List<SimDescription.DeathType>();
                        list.Add(SimDescription.DeathType.Drown);
                        list.Add(SimDescription.DeathType.Starve);
                        list.Add(SimDescription.DeathType.Thirst);
                        list.Add(SimDescription.DeathType.Burn);
                        list.Add(SimDescription.DeathType.Freeze);
                        list.Add(SimDescription.DeathType.ScubaDrown);
                        list.Add(SimDescription.DeathType.Shark);
                        list.Add(SimDescription.DeathType.Jetpack);
                        list.Add(SimDescription.DeathType.Meteor);
                        list.Add(SimDescription.DeathType.Causality);
                        if (!Target.SimDescription.IsFrankenstein)
                        {
                            list.Add(SimDescription.DeathType.Electrocution);
                        }
                        list.Add(SimDescription.DeathType.Burn);
                        if (Target.SimDescription.Elder)
                        {
                            list.Add(SimDescription.DeathType.OldAge);
                        }
                        if (Target.SimDescription.IsWitch)
                        {
                            list.Add(SimDescription.DeathType.HauntingCurse);
                        }
                        SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                        Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                    }
                    if (mDeathProgress != DeathProgress.Complete)
                    {
                        if (mDeathProgress == DeathProgress.None)
                        {
                            mGrave = Urnstone.CreateGrave(Target.SimDescription, true, false);
                            mDeathProgress = DeathProgress.GraveCreated;
                        }
                        if (mDeathProgress == DeathProgress.GraveCreated)
                        {
                            if (!GlobalFunctions.PlaceAtGoodLocation(mGrave, new World.FindGoodLocationParams(Target.Position), false))
                            {
                                mGrave.SetPosition(Target.Position);
                            }
                            mGrave.OnHandToolMovement();
                            mDeathProgress = DeathProgress.GravePlaced;
                        }
                        if (mDeathProgress == DeathProgress.GravePlaced)
                        {
                            mGrave.GhostSetup(Target, false);
                        }
                        if (mDeathFlower == null)
                        {
                            mDeathFlower = Target.Inventory.Find<DeathFlower>();
                        }
                        if (mDeathProgress == DeathProgress.UnluckyStarted || (Target.Household != null && Target.IsInActiveHousehold && Target.SimDescription.DeathStyle != (SimDescription.DeathType)69u))
                        {
                            EventCallbackResurrectSim();
                            Target.AddExitReason(ExitReason.HigherPriorityNext);
                        }
                        else if (mDeathProgress == DeathProgress.DeathFlowerStarted || mDeathFlower != null && Target.IsInActiveHousehold)
                        {
                            Target.Inventory.RemoveByForce(mDeathFlower);
                            mDeathFlower.Destroy();
                            EventCallbackResurrectSim();
                            Target.AddExitReason(ExitReason.HigherPriorityNext);
                        }
                        else if (mDeathProgress != DeathProgress.UnluckyPostEvent)
                        {
                            if (mDeathProgress == DeathProgress.DeathFlowerPostEvent)
                            {
                                if (Target.Inventory.Contains(mDeathFlower))
                                {
                                    Target.Inventory.RemoveByForce(mDeathFlower);
                                }
                                if (mDeathFlower != null)
                                {
                                    mDeathFlower.Destroy();
                                }
                            }
                            else
                            {
                                mDeathProgress = DeathProgress.NormalStarted;
                            }
                        }
                        if (mDeathProgress == DeathProgress.NormalStarted)
                        {
                            FinalizeDeath();
                            CleanupAndDestroyDeadSim(true);
                        }
                        GrimReaperPostSequenceCleanup();
                        Urnstone.FogEffectTurnAllOff(Actor.LotCurrent);
                        StopGhostExplosion();
                    }
                    if (mSituation.OkSuusse)
                    {
                        mSMCDeath.EnterState("x", "Enter");
                    }
                    
                    Notfixdgxs = true;
                    //mSMCDeath.EnterState("x", "EnterReaperBrushingHimself");
                    base.Cleanup();
                    return;
                }
                catch
                {
                    try
                    {
                        if (mSituation.OkSuusse && !Notfixdgxs)
                        {
                            mSMCDeath.EnterState("x", "Enter");
                        }
                    }
                    catch
                    { }
                    
                    base.Cleanup();
                }

            }


            public void EventCallbackFadeInReaper(StateMachineClient sender, IEvent evt)
            {
                Actor.FadeIn(false, 3f);

            }


            
            public bool Notfixdgs = false;

            public bool Notfixdgxs = false;

            protected Vector3 mGhostForward = Vector3.Invalid;


            private void FadeDeathEventCallBack(StateMachineClient sender, IEvent evt)
            {
                switch (evt.EventId)
                {
                    case 111u:
                        Target.SetOpacity(0f, 0.4f);
                        break;
                    case 112u:
                        {
                            Actor.SetOpacity(0f, 0.4f);
                            NiecHelperSituation grimReaperSituation = mSituation;
                            grimReaperSituation.StopGrimReaperSmoke();
                            break;
                        }
                }
            }



            public bool ReapPetPool()
            {
                if (!CreateGraveStone())
                {
                    return false;
                }
                mSMCDeath = mSituation.SMCDeath;
                mSMCDeath.SetActor("y", Target);
                mSMCDeath.SetActor("grave", mGrave);
                mSMCDeath.EnterState("y", "Enter");
                ScubaDiving scubaDiving = Target.Posture as ScubaDiving;
                if (scubaDiving != null)
                {
                    scubaDiving.StopBubbleEffects();
                }
                SimDescription.DeathType deathStyle = Target.SimDescription.DeathStyle;
                if (!mSituation.mIsFirstSim)
                {
                    mSMCDeath.EnterState("x", "Enter");
                    mSMCDeath.RequestState(false, "x", "TreadWater");
                    mSituation.mScubaDeathJig = (GlobalFunctions.CreateObjectOutOfWorld("UnderwaterSocial_Jig", ProductVersion.EP10) as SocialJig);
                    Vector3 position = Target.Position;
                    Vector3 forward = Target.ForwardVector;
                    if (!GlobalFunctions.FindGoodLocationNearbyOnLevel(mSituation.mScubaDeathJig, 0, ref position, ref forward, FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.AllowOnSlopes | FindGoodLocationBooleans.AllowInSea))
                    {
                        return false;
                    }
                    mSituation.mScubaDeathJig.SetPosition(position);
                    mSituation.mScubaDeathJig.SetForward(forward);
                    Route route = mSituation.mScubaDeathJig.RouteToJigA(Actor);
                    route.DoRouteFail = false;
                    Actor.DoRoute(route);
                }
                else
                {
                    mSituation.mIsFirstSim = false;
                }
                Target.FadeOut(false, false, 2f);
                PlaceGraveStone();
                mDeathProgress = DeathProgress.GravePlaced;
                mSMCDeath.RequestState(true, "x", "CreateTombstone");
                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_moodlet_mourning", Target.GetThoughtBalloonThumbnailKey());
                balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                balloonData.mPriority = ThoughtBalloonPriority.High;
                Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                mSMCDeath.RequestState(false, "x", "ReaperFloating");
                mGhostPosition = mSituation.mScubaDeathJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
                mGhostForward = mSituation.mScubaDeathJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
                Lot lotCurrent = Target.LotCurrent;
                if (lotCurrent != Target.LotHome && lotCurrent.Household != null)
                {
                    Target.GreetSimOnLot(lotCurrent);
                }
                switch (deathStyle)
                {
                    case SimDescription.DeathType.ScubaDrown:
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                        mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                        mSMCDeath.RequestState(true, "y", "PoseDrown");
                        mSMCDeath.RequestState(true, "y", "DrownToFloat");
                        mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    case SimDescription.DeathType.Shark:
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                        mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                        mSMCDeath.RequestState(true, "y", "PoseShark");
                        mSMCDeath.RequestState(true, "y", "SharkToFloat");
                        mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    default:
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                        mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                        mSMCDeath.RequestState(true, "y", "PoseDrown");
                        mSMCDeath.RequestState(true, "y", "DrownToFloat");
                        mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                }
                mDeathFlower = Target.Inventory.Find<DeathFlower>();
               
                //Target.AddExitReason(ExitReason.HigherPriorityNext);
                Target.AddExitReason(ExitReason.CanceledByScript);

                /* error :D
                
                if (StartSync(true, true, null, 0f, false))
                {
                    if (Target.IsInActiveHousehold)
                    {
                        mDeathProgress = DeathProgress.DeathFlowerStarted;
                        Target.Inventory.RemoveByForce(mDeathFlower);
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                        mSMCDeath.RequestState(false, "x", "DeathFlower");
                        mSMCDeath.RequestState(true, "y", "DeathFlower");
                       // mDeathFlower.Destroy();
                       // mDeathFlower = null;
                        mSMCDeath.RequestState(false, "x", "Exit");
                        mSMCDeath.RequestState(true, "y", "Exit");
                        FinishLinkedInteraction(true);
                        mDeathProgress = DeathProgress.Complete;
                        return true;
                    }
                    mDeathProgress = DeathProgress.NormalStarted;
                    mSMCDeath.RequestState(false, "x", "Accept");
                    mSMCDeath.RequestState(true, "y", "Accept");
                    mSMCDeath.RequestState(false, "y", "Exit");
                    mSMCDeath.RequestState(true, "x", "Exit");
                }
                else
                {
                    if (Target.IsInActiveHousehold)
                    {
                        mDeathProgress = DeathProgress.DeathFlowerStarted;
                        Target.Inventory.RemoveByForce(mDeathFlower);
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                        mSMCDeath.RequestState(false, "x", "DeathFlower");
                        mSMCDeath.RequestState(true, "y", "DeathFlower");
                        //mDeathFlower.Destroy();
                        //mDeathFlower = null;
                        mSMCDeath.RequestState(false, "x", "Exit");
                        mSMCDeath.RequestState(true, "y", "Exit");
                        FinishLinkedInteraction(true);
                        mDeathProgress = DeathProgress.Complete;
                        return true;
                    }
                    mDeathProgress = DeathProgress.NormalStarted;
                    mSMCDeath.RequestState("x", "ExitNoSocial");
                    Target.FadeOut();
                    mDeathEffect = mGrave.GetSimToGhostEffect(Target, mGrave.Position);
                    mSMCDeath.SetEffectActor("deathEffect", mDeathEffect);
                    mDeathEffect.Start();
                }
                 */


                Target.FadeIn();
                if (Target.IsInActiveHousehold)
                {
                    mDeathProgress = DeathProgress.DeathFlowerStarted;
                    Target.Inventory.RemoveByForce(mDeathFlower);
                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                    mSMCDeath.RequestState(false, "x", "DeathFlower");
                    mSMCDeath.RequestState(true, "y", "DeathFlower");
                    if (mDeathFlower != null)
                    {
                        mDeathFlower.Destroy();
                        mDeathFlower = null;
                    }

                    mSMCDeath.RequestState(false, "x", "Exit");
                    mSMCDeath.RequestState(true, "y", "Exit");
                    FinishLinkedInteraction(true);
                    mDeathProgress = DeathProgress.Complete;
                    return true;
                }
                mDeathProgress = DeathProgress.NormalStarted;
                mSMCDeath.RequestState(false, "x", "Accept");
                mSMCDeath.RequestState(true, "y", "Accept");
                mSMCDeath.RequestState(false, "y", "Exit");
                mSMCDeath.RequestState(true, "x", "Exit");


                mDeathProgress = DeathProgress.NormalStarted;
                FinalizeDeath();
                Target.StartOneShotFunction(ReapSoulCallback, GameObject.OneShotFunctionDisposeFlag.OnDispose);
                mDeathProgress = DeathProgress.Complete;
                //FinishLinkedInteraction(true);
                Target.FadeIn();
                return true;
                /*
                //string m = SMsg;
                if (!CreateGraveStone())
                {
                    return false;
                }
                mSMCDeath = mSituation.SMCDeath;
                mSMCDeath.SetActor("y", Target);
                mSMCDeath.SetActor("grave", mGrave);
                mSMCDeath.EnterState("y", "Enter");
                SMsg = "Pet Enter";
                SimDescription.DeathType deathStyle = Target.SimDescription.DeathStyle;
                if (!mSituation.mIsFirstSim)
                {
                    SMsg = "Pet Enter 2";
                    //mSMCDeath.EnterState("x", "Enter");
                    mSMCDeath.RequestState(false, "x", "TreadWater");
                    mSituation.mScubaDeathJig = (GlobalFunctions.CreateObjectOutOfWorld("UnderwaterSocial_Jig", ProductVersion.EP10) as SocialJig);
                    Vector3 position = Target.Position;
                    Vector3 forward = Target.ForwardVector;
                    if (!GlobalFunctions.FindGoodLocationNearbyOnLevel(mSituation.mScubaDeathJig, 0, ref position, ref forward, FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.AllowOnSlopes | FindGoodLocationBooleans.AllowInSea))
                    {
                        return false;
                    }
                    mSituation.mScubaDeathJig.SetPosition(position);
                    mSituation.mScubaDeathJig.SetForward(forward);
                    SMsg = "Pet Enter 3";
                    Route route = mSituation.mScubaDeathJig.RouteToJigA(Actor);
                    if (route != null)
                    {
                        route.DoRouteFail = false;
                        Actor.DoRoute(route);
                    }

                    SMsg = "Pet Enter 4";
                }
                else
                {
                    mSituation.mIsFirstSim = false;
                }
                Target.FadeOut(false, false, 2f);
                PlaceGraveStone();
                mDeathProgress = DeathProgress.GravePlaced;
                SMsg = "Pet Enter 5";
                mSMCDeath.RequestState(true, "x", "CreateTombstone");
                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_moodlet_mourning", Target.GetThoughtBalloonThumbnailKey());
                balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                balloonData.mPriority = ThoughtBalloonPriority.High;
                Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                SMsg = "Pet Enter 6";
                mSMCDeath.RequestState(false, "x", "ReaperFloating");
                //mGhostPosition = mSituation.mScubaDeathJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
                //mGhostForward = mSituation.mScubaDeathJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
                try
                {
                    Lot lotCurrent = Target.LotCurrent;
                    if (lotCurrent != Target.LotHome && lotCurrent.Household != null)
                    {
                        Target.GreetSimOnLot(lotCurrent);
                    }
                }
                catch (Exception)
                { }
                SMsg = "Pet Enter 6a";
                switch (deathStyle)
                {
                    case SimDescription.DeathType.ScubaDrown:
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                        mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                        mSMCDeath.RequestState(true, "y", "PoseDrown");
                        mSMCDeath.RequestState(true, "y", "DrownToFloat");
                        mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    case SimDescription.DeathType.Shark:
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                        mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                        mSMCDeath.RequestState(true, "y", "PoseShark");
                        mSMCDeath.RequestState(true, "y", "SharkToFloat");
                        mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                    default:
                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                        mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                        mSMCDeath.RequestState(true, "y", "PoseDrown");
                        mSMCDeath.RequestState(true, "y", "DrownToFloat");
                        mSMCDeath.RequestState(false, "y", "GhostFloating");
                        break;
                }
                mDeathFlower = Target.Inventory.Find<DeathFlower>();
                //BeReapedDiving beReapedDiving = (BeReapedDiving)(LinkedInteractionInstance = (BeReapedDiving.Singleton.CreateInstance(Actor, Target, new InteractionPriority(InteractionPriorityLevel.MaxExpireDeathX, 1f), false, true) as BeReapedDiving));
                //beReapedDiving.mHadDeathFlower = (mDeathFlower != null);
                Target.AddExitReason(ExitReason.HigherPriorityNext);
                //Target.InteractionQueue.AddNext(beReapedDiving);

                SMsg = "Pet Enter 7";
                Target.FadeIn();
                if (Target.IsInActiveHousehold)
                {
                    mDeathProgress = DeathProgress.DeathFlowerStarted;
                    Target.Inventory.RemoveByForce(mDeathFlower);
                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                    mSMCDeath.RequestState(false, "x", "DeathFlower");
                    mSMCDeath.RequestState(true, "y", "DeathFlower");
                    if (mDeathFlower != null)
                    {
                        mDeathFlower.Destroy();
                        mDeathFlower = null;
                    }

                    mSMCDeath.RequestState(false, "x", "Exit");
                    mSMCDeath.RequestState(true, "y", "Exit");
                    FinishLinkedInteraction(true);
                    mDeathProgress = DeathProgress.Complete;
                    return true;
                }
                SMsg = "Pet Enter 8";
                mDeathProgress = DeathProgress.NormalStarted;
                mSMCDeath.RequestState(false, "x", "Accept");
                mSMCDeath.RequestState(true, "y", "Accept");
                mSMCDeath.RequestState(false, "y", "Exit");
                mSMCDeath.RequestState(true, "x", "Exit");

                SMsg = "Pet Enter 9";
                mDeathProgress = DeathProgress.NormalStarted;
                FinalizeDeath();
                Target.StartOneShotFunction(ReapSoulCallback, GameObject.OneShotFunctionDisposeFlag.OnDispose);
                mDeathProgress = DeathProgress.Complete;
                //FinishLinkedInteraction(true);
                Target.FadeIn();
                SMsg = "Pet Enter 10";
                */
                //return true;
            }



            public bool ReapPetSoul()
            {
                var extKillSimNiec = Target.CurrentInteraction as ExtKillSimNiec;
                var killSim = Target.CurrentInteraction as Urnstone.KillSim;
                if (extKillSimNiec != null && extKillSimNiec.SocialJig != null)
                {
                    base.SocialJig = extKillSimNiec.SocialJig;
                    extKillSimNiec.SocialJig = null;
                    SocialJig.ClearParticipants();
                    SocialJig.RegisterParticipants(Actor, Target);
                }
                
                else if (killSim != null && killSim.SocialJig != null)
                {
                    base.SocialJig = killSim.SocialJig;
                    killSim.SocialJig = null;
                    SocialJig.ClearParticipants();
                    SocialJig.RegisterParticipants(Actor, Target);
                }
                





                if (!CreateGraveStone())
                {
                    return false;
                }
                mSMCDeath = mSituation.SMCDeath;
                mSMCDeath.SetActor("y", Target);
                mSMCDeath.SetActor("grave", mGrave);
                mSMCDeath.EnterState("y", "Enter");
                PlaceGraveStone();
                mDeathProgress = DeathProgress.GravePlaced;
                if (!mSituation.mIsFirstSim)
                {
                    mSMCDeath.EnterState("x", "Enter");
                }
                mSMCDeath.RequestState(true, "x", "CreateTombstone");
                Lot lotCurrent = Target.LotCurrent;
                if (lotCurrent != Target.LotHome && lotCurrent.Household != null)
                {
                    Target.GreetSimOnLot(lotCurrent);
                }
                
                RequestWalkStyle(Sim.WalkStyle.DeathWalk);
                string name = Localization.LocalizeString(Target.SimDescription.IsFemale, "Gameplay/Actors/Sim/ReapSoul:InteractionName");
                if (BeginSocialInteraction(new KillSimNiecX.NiecDefinitionDeathInteraction(name, false), true, Target.GetSocialRadiusWith(Actor), false))
                {
                    mDeathProgress = DeathProgress.NormalStarted;
                    if (Target.IsHorse)
                    {
                        mSMCDeath.AddOneShotScriptEventHandler(112u, FadeDeathEventCallBack);
                    }
                    mSMCDeath.AddOneShotScriptEventHandler(111u, FadeDeathEventCallBack);
                    mSMCDeath.RequestState(false, "x", "Pet");
                    mSMCDeath.RequestState(true, "y", "Pet");
                    mSMCDeath.RequestState(false, "x", "ReapPet");
                    mSMCDeath.RequestState(true, "y", "ReapPet");
                    mSMCDeath.RequestState(false, "x", "Exit");
                    mSMCDeath.RequestState(true, "y", "Exit");

                }
                else
                {
                    Target.FadeOut(false, false, 0f);
                }
                Target.AddExitReason(ExitReason.HigherPriorityNext);
                Target.AddExitReason(ExitReason.CanceledByScript);
                mGrave.GhostSetup(Target, false);
                mDeathProgress = DeathProgress.NormalStarted;
                FinalizeDeath();
                GrimReaperPostSequenceCleanup();
                Target.StartOneShotFunction(ReapSoulCallback, GameObject.OneShotFunctionDisposeFlag.OnDispose);
                mDeathProgress = DeathProgress.Complete;
                return true;
            }

            public string SMsg = "Run";


            public  unsafe override bool Run()
            {
                try
                {
                    /*
                    if (Target.Service is Sims3.Gameplay.Services.GrimReaper)
                    {
                        if (!TwoButtonDialog.Show("ReapSoul: Killing the " + Target.Name + " [GrimReaper] will prevent souls to cross over to the other side. If this happens, Sims that die from now on will be trapped between this world and the next, and you'll end up with a city full of dead bodies laying around. Are you sure you want to kill Death itself?", "Yes", "No"))
                        {
                            Notfixdgs = true;
                            mDeathProgress = DeathProgress.Complete;
                            return false;
                        }
                    }
                     */
                    mSituation = Actor.GetSituationOfType<NiecHelperSituation>();
                    if (mSituation != null && mSituation.OkSuusse && Actor.IsSelectable)
                    {
                        
                        
                        {

                            if (Target.SimDescription.DeathStyle == SimDescription.DeathType.None)
                            {
                                List<SimDescription.DeathType> list = new List<SimDescription.DeathType>();
                                list.Add(SimDescription.DeathType.Drown);
                                list.Add(SimDescription.DeathType.Starve);
                                list.Add(SimDescription.DeathType.Thirst);
                                list.Add(SimDescription.DeathType.Burn);
                                list.Add(SimDescription.DeathType.Freeze);
                                list.Add(SimDescription.DeathType.ScubaDrown);
                                list.Add(SimDescription.DeathType.Shark);
                                list.Add(SimDescription.DeathType.Jetpack);
                                list.Add(SimDescription.DeathType.Meteor);
                                list.Add(SimDescription.DeathType.Causality);
                                if (!Target.SimDescription.IsFrankenstein)
                                {
                                    list.Add(SimDescription.DeathType.Electrocution);
                                }
                                list.Add(SimDescription.DeathType.Burn);
                                if (Target.SimDescription.Elder)
                                {
                                    list.Add(SimDescription.DeathType.OldAge);
                                }
                                if (Target.SimDescription.IsWitch)
                                {
                                    list.Add(SimDescription.DeathType.HauntingCurse);
                                }
                                SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                                Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                //Notfixdgs = true;
                                //return true;
                            }

                            Actor.FadeIn(false, 3f);

                            

                            Lot ActorLot = Actor.LotCurrent;
                            Lot TargetLot = Target.LotCurrent;

                            //if (Target.Posture is ScubaDiving && !Target.SimDescription.IsMermaid)
                            if (mSituation.OkSuusseD || ActorLot != null && ActorLot.IsDivingLot || TargetLot != null && TargetLot.IsDivingLot && Target.Posture != null && Target.Posture is ScubaDiving && !Target.SimDescription.IsMermaid)
                            {
                                return ReapPetPool();
                            }

                            if (Target.IsPet && !Target.IsInActiveHousehold)
                            {
                                return ReapPetSoul();
                            }
                            
                            if (!CreateGraveStone())
                            {
                                return false;
                            }
                            mSMCDeath = mSituation.SMCDeath;
                            mSMCDeath.SetActor("y", Target);
                            mSMCDeath.SetActor("grave", mGrave);
                            mSMCDeath.EnterState("y", "Enter");

                            if (Target.SimDescription.DeathStyle == SimDescription.DeathType.Drown)
                            {
                                if (!RouteGrimReaperToEdgeOfTargetPool())
                                {
                                    Actor.RouteTurnToFace(Target.Position);
                                }
                            }
                            else if (!mSituation.mIsFirstSim)
                            {
                                Route route = Actor.CreateRoute();
                                route.PlanToPointRadialRange(Target.Position, 1f, 5f, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, Target.LotCurrent.LotId, null);
                                Actor.DoRoute(route);
                            }
                            /*
                        else
                        {
                            mSituation.mIsFirstSim = false;
                        }
                         */

                            /*
                            Route route = Actor.CreateRoute();
                            route.PlanToPointRadialRange(Target.Position, 1f, 5f, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, Target.LotCurrent.LotId, null);
                            Actor.DoRoute(route);
                            mSituation.mIsFirstSim = true;
                             * */
                            PlaceGraveStone();
                            mDeathProgress = DeathProgress.GravePlaced;
                            Actor.RouteTurnToFace(Target.Position);
                            //PetStartleBehavior.CheckForStartle(Actor, StartleType.GrimReaperAppear);
                            /*
                            if (!mSituation.mIsFirstSim)
                            {
                                mSMCDeath.EnterState("x", "Enter");
                            }
                             * */
                            Actor.FadeIn(false, 3f);
                            mSMCDeath.RequestState(true, "x", "CreateTombstone");
                            ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_moodlet_mourning", Target.GetThoughtBalloonThumbnailKey());
                            balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
                            balloonData.mPriority = ThoughtBalloonPriority.High;
                            Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                            //mSMCDeath.RequestState(true, "x", "ReaperPointAtGrave");
                            mSMCDeath.RequestState(true, "x", "ReaperPointAtGrave");
                            mSMCDeath.RequestState(false, "x", "ReaperFloating");
                            mGhostPosition = GetPositionForGhost(Target, mGrave);
                            Lot lotCurrent = Target.LotCurrent;
                            if (lotCurrent != Target.LotHome && lotCurrent.Household != null)
                            {
                                Target.GreetSimOnLot(lotCurrent);
                            }
                            try
                            {
                                RequestWalkStyle(Target, Sim.WalkStyle.GhostWalk);
                            }
                            catch
                            { }
                            switch (Target.SimDescription.DeathStyle)
                            {

                                case SimDescription.DeathType.Electrocution:
                                case SimDescription.DeathType.BluntForceTrauma:
                                case SimDescription.DeathType.Ranting:
                                    StartGhostExplosion();
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.RequestState(true, "y", "PoseElectrocution");
                                    mSMCDeath.RequestState(true, "y", "ElectrocutionToFloat");
                                    mSMCDeath.RequestState(false, "y", "GhostFloating");
                                    StopGhostExplosion();
                                    break;
                                case SimDescription.DeathType.Starve:
                                    StartGhostExplosion();
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.RequestState(true, "y", "PoseStarvation");
                                    mSMCDeath.RequestState(true, "y", "StarvationToFloat");
                                    mSMCDeath.RequestState(false, "y", "GhostFloating");
                                    StopGhostExplosion();
                                    break;
                                case SimDescription.DeathType.Drown:
                                    if (Target.BridgeOrigin != null)
                                    {
                                        Target.BridgeOrigin.MakeRequest();
                                    }
                                    Target.PopPosture();
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                                    mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.RequestState(true, "y", "PoseStarvation");
                                    mSMCDeath.RequestState(true, "y", "StarvationToFloat");
                                    mSMCDeath.RequestState(false, "y", "GhostFloating");
                                    break;
                                case SimDescription.DeathType.Burn:
                                case SimDescription.DeathType.MummyCurse:
                                case SimDescription.DeathType.Meteor:
                                case SimDescription.DeathType.Thirst:
                                case SimDescription.DeathType.Transmuted:
                                case SimDescription.DeathType.HauntingCurse:
                                case SimDescription.DeathType.JellyBeanDeath:
                                case SimDescription.DeathType.Jetpack:
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackSimToGhostEffectNoFadeOut);
                                    mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.RequestState(true, "y", "PoseElectrocution");
                                    mSMCDeath.RequestState(true, "y", "ElectrocutionToFloat");
                                    mSMCDeath.RequestState(false, "y", "GhostFloating");
                                    break;
                                case SimDescription.DeathType.Freeze:
                                    StartGhostExplosion();
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.AddOneShotScriptEventHandler(102u, EventCallbackFadeBodyFromPose);
                                    mSMCDeath.RequestState(true, "y", "PoseFreeze");
                                    mSMCDeath.RequestState(true, "y", "FreezeToFloat");
                                    mSMCDeath.RequestState(false, "y", "GhostFloating");
                                    break;
                                case SimDescription.DeathType.MermaidDehydrated:
                                    MermaidDehydratedToGhostSequence();
                                    break;
                                default:
                                    if (Target.SimDescription.DeathStyle != SimDescription.DeathType.OldAge)
                                    {
                                        Target.SetPosition(mGhostPosition);
                                    }
                                    mGrave.GhostSetup(Target, false);
                                    Target.SetHiddenFlags(HiddenFlags.Nothing);
                                    Target.FadeIn();
                                    break;
                            }
                            RequestWalkStyle(Sim.WalkStyle.RobotHoverRun);
                            string name = Localization.LocalizeString(Target.SimDescription.IsFemale, "Gameplay/Actors/Sim/ReapSoul:InteractionName");
                            Target.AddExitReason(ExitReason.CanceledByScript);
                            if (Target.IsSelectable)
                            {
                                try
                                {
                                    UnrequestWalkStyle(Target, Sim.WalkStyle.GhostWalk);
                                }
                                catch
                                { }
                                
                                RequestWalkStyle(Target, Sim.WalkStyle.Walk);
                            }
                            if (BeginSocialInteraction(new KillSimNiecX.NiecDefinitionDeathInteraction(name, true), true, 1.25f, false))
                            {
                                if ( /*Target.SimDescription.DeathStyle != (SimDescription.DeathType)69u && */ Target.IsInActiveHousehold)
                                {
                                    /*
                                    mDeathProgress = DeathProgress.UnluckyStarted;
                                    if (Target.SimDescription.DeathStyle == SimDescription.DeathType.OldAge)
                                    {
                                        Target.Motives.SetValue(CommodityKind.VampireThirst, -50f);
                                    }
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimUnlucky);
                                    mSMCDeath.RequestState(false, "x", "Unlucky");
                                    mSMCDeath.RequestState(true, "y", "Unlucky");
                                    mSMCDeath.RequestState(false, "x", "Exit");
                                    mSMCDeath.RequestState(true, "y", "Exit");
                                    GrimReaperPostSequenceCleanup();
                                    mDeathProgress = DeathProgress.Complete;
                                    return true;
                                     */
                                    mDeathProgress = DeathProgress.DeathFlowerStarted;
                                    if (mDeathFlower != null)
                                    {
                                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                                    }
                                    else
                                    {
                                        mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimUnlucky);
                                    }
                                    mSMCDeath.RequestState(false, "x", "DeathFlower");
                                    mSMCDeath.RequestState(true, "y", "DeathFlower");
                                    mSMCDeath.RequestState(false, "x", "Exit");
                                    mSMCDeath.RequestState(true, "y", "Exit");
                                    GrimReaperPostSequenceCleanup();
                                    mDeathProgress = DeathProgress.Complete;
                                    return true;
                                }
                                if (Target.SimDescription.DeathStyle == SimDescription.DeathType.Ranting && Target.IsInActiveHousehold && Target.TraitManager != null && !Target.TraitManager.HasElement(TraitNames.ThereAndBackAgain))
                                {
                                    mDeathProgress = DeathProgress.UnluckyStarted;
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimRanting);
                                    mSMCDeath.RequestState(false, "x", "Unlucky");
                                    mSMCDeath.RequestState(true, "y", "Unlucky");
                                    mSMCDeath.RequestState(false, "x", "Exit");
                                    mSMCDeath.RequestState(true, "y", "Exit");
                                    GrimReaperPostSequenceCleanup();
                                    mDeathProgress = DeathProgress.Complete;
                                    return true;
                                }
                                mDeathFlower = Target.Inventory.Find<DeathFlower>();
                                if (mDeathFlower != null && Target.IsInActiveHousehold)
                                {
                                    mDeathProgress = DeathProgress.DeathFlowerStarted;
                                    Target.Inventory.RemoveByForce(mDeathFlower);
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                                    mSMCDeath.RequestState(false, "x", "DeathFlower");
                                    mSMCDeath.RequestState(true, "y", "DeathFlower");
                                    mDeathFlower.Destroy();
                                    mDeathFlower = null;
                                    mSMCDeath.RequestState(false, "x", "Exit");
                                    mSMCDeath.RequestState(true, "y", "Exit");
                                    GrimReaperPostSequenceCleanup();
                                    mDeathProgress = DeathProgress.Complete;
                                    return true;
                                }
                                /*
                                if (Target.SimDescription.DeathStyle != SimDescription.DeathType.OldAge)
                                {
                                    Actor.AddInteraction(ChessChallenge.Singleton);
                                }
                                 * */
                                mDeathProgress = DeathProgress.NormalStarted;
                                /*
                                if (Target.IsInActiveHousehold)
                                {
                                    mSMCDeath.RequestState(false, "x", "Accept");
                                    mSMCDeath.RequestState(true, "y", "Accept");
                                    Route route2 = Target.CreateRouteTurnToFace(mGrave.Position);
                                    route2.ExecutionFromNonSimTaskIsSafe = true;
                                    Target.DoRoute(route2);
                                }
                                else if (Target.IsInActiveHousehold)
                                {
                                    mSMCDeath.RequestState(false, "x", "Reject");
                                    mSMCDeath.RequestState(true, "y", "Reject");
                                    Route route3 = Target.CreateRouteTurnToFace(mGrave.Position);
                                    route3.ExecutionFromNonSimTaskIsSafe = true;
                                    Target.DoRoute(route3);
                                    mSMCDeath.RequestState(false, "x", "GhostJumpInGrave");
                                    mSMCDeath.RequestState(true, "y", "GhostJumpInGrave");
                                }
                                 * */
                                //else
                                {
                                    mSMCDeath.RequestState(false, "x", "GhostKickedDive");
                                    mSMCDeath.RequestState(true, "y", "GhostKickedDive");
                                    mSMCDeath.RequestState(false, "x", "Kicked");
                                    mSMCDeath.RequestState(true, "y", "Kicked");
                                }
                                mSMCDeath.RequestState(false, "y", "Exit");
                                mSMCDeath.RequestState(true, "x", "Exit");
                            }
                            else
                            {
                                mDeathFlower = Target.Inventory.Find<DeathFlower>();
                                if (Target.SimDescription.DeathStyle != (SimDescription.DeathType)69u && Target.IsInActiveHousehold)
                                {
                                    mDeathProgress = DeathProgress.UnluckyStarted;
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimUnlucky);
                                    mSMCDeath.RequestState(false, "x", "Unlucky");
                                    mSMCDeath.RequestState(true, "y", "Unlucky");
                                    mSMCDeath.RequestState(false, "x", "Exit");
                                    mSMCDeath.RequestState(true, "y", "Exit");
                                    GrimReaperPostSequenceCleanup();
                                    Target.AddExitReason(ExitReason.HigherPriorityNext);
                                    mDeathProgress = DeathProgress.Complete;
                                    return true;
                                }
                                if (mDeathFlower != null && Target.IsInActiveHousehold)
                                {
                                    mDeathProgress = DeathProgress.DeathFlowerStarted;
                                    Target.Inventory.RemoveByForce(mDeathFlower);
                                    mSMCDeath.AddOneShotScriptEventHandler(101u, EventCallbackResurrectSimDeathFlower);
                                    mSMCDeath.RequestState(false, "x", "Unlucky");
                                    mSMCDeath.RequestState(true, "y", "Unlucky");
                                    mDeathFlower.Destroy();
                                    mDeathFlower = null;
                                    mSMCDeath.RequestState(false, "x", "Exit");
                                    mSMCDeath.RequestState(true, "y", "Exit");
                                    GrimReaperPostSequenceCleanup();
                                    Target.AddExitReason(ExitReason.HigherPriorityNext);
                                    mDeathProgress = DeathProgress.Complete;
                                    return true;
                                }
                                mDeathProgress = DeathProgress.NormalStarted;
                                mSMCDeath.RequestState("x", "ExitNoSocial");
                                Target.FadeOut();
                                mDeathEffect = mGrave.GetSimToGhostEffect(Target, mGrave.Position);
                                mSMCDeath.SetEffectActor("deathEffect", mDeathEffect);
                                mDeathEffect.Start();
                            }
                            //mSituation.CheckAndSetPetSavior(Target);
                            mDeathProgress = DeathProgress.NormalStarted;
                            try
                            {
                                FinalizeDeath();
                                GrimReaperPostSequenceCleanup();
                            }
                            catch
                            { }
                            
                            Target.StartOneShotFunction(ReapSoulCallback, GameObject.OneShotFunctionDisposeFlag.OnDispose);
                            mDeathProgress = DeathProgress.Complete;
                        }
                    }
                }
                catch (ResetException) { mSituation.OkSuusse = false; throw; }
                catch (Exception exception)
                {
                    NiecException.PrintMessage(SMsg + " NRP " + exception.Message + NiecException.NewLine + exception.StackTrace);
                    GrimReaperPostSequenceCleanup();
                    if (mSituation != null)
                    {
                        mSituation.OkSuusseD = false;
                        mSituation.OkSuusse = false;
                    }
                    Actor.ResetAllAnimation();
                    NiecMod.Nra.SpeedTrap.Sleep();
                    AnimationUtil.StopAllAnimation(Actor);
                    NiecMod.Nra.SpeedTrap.Sleep();
                    Actor.SetObjectToReset();

                }
                return true;
            }

            protected bool CreateGraveStone()
            {

                Urnstone.KillSim killSim = Target.CurrentInteraction as Urnstone.KillSim;
                if (killSim != null)
                {
                    killSim.CancelDeath = false;
                }

                mWasMemberOfActiveHousehold = ((Target.Household == Household.ActiveHousehold) ? true : false);
                if (Target.DeathReactionBroadcast == null)
                {
                    Urnstone.CreateDeathReactionBroadcaster(Target);
                }
                Actor.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                Target.SynchronizationLevel = Sim.SyncLevel.NotStarted;
                mGrave = Urnstone.CreateGrave(Target.SimDescription, true, false);
                if (mGrave == null)
                {
                    return false;
                }
                mDeadSimsHousehold = Target.Household;
                mGrave.AddToUseList(Actor);
                //mSituation = Actor.GetSituationOfType<NiecHelperSituation>();
                mSituation.LastGraveCreated = mGrave;
                mDeathProgress = DeathProgress.GraveCreated;
                return true;
            }

            protected virtual void PlaceGraveStone()
            {
                mGrave.SetOpacity(0f, 0f);
                SimDescription.DeathType deathStyle = Target.SimDescription.DeathStyle;
                World.FindGoodLocationParams fglParams = (deathStyle == SimDescription.DeathType.Drown) ? new World.FindGoodLocationParams(Actor.Position) : new World.FindGoodLocationParams(Target.Position);
                fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                if (!GlobalFunctions.PlaceAtGoodLocation(mGrave, fglParams, false))
                {
                    fglParams.BooleanConstraints = FindGoodLocationBooleans.None;
                    if (!GlobalFunctions.PlaceAtGoodLocation(mGrave, fglParams, false))
                    {
                        mGrave.SetPosition(Target.Position);
                    }
                }
                mGrave.OnHandToolMovement();
                mGrave.FadeIn(false, 10f);
                mGrave.FogEffectStart();
            }

            protected Vector3 GetPositionForGhost(Sim ghost, Urnstone grave)
            {
                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(grave.Position);
                fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                Vector3 pos;
                Vector3 fwd;
                if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out pos, out fwd))
                {
                    Simulator.Sleep(0u);
                    fglParams.BooleanConstraints &= ~FindGoodLocationBooleans.StayInRoom;
                    if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out pos, out fwd))
                    {
                        Simulator.Sleep(0u);
                        fglParams.BooleanConstraints &= ~FindGoodLocationBooleans.Routable;
                        if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out pos, out fwd))
                        {
                            Simulator.Sleep(0u);
                            fglParams.BooleanConstraints = FindGoodLocationBooleans.None;
                            if (!GlobalFunctions.FindGoodLocation(ghost, fglParams, out pos, out fwd))
                            {
                                return ghost.Position;
                            }
                        }
                    }
                }
                return pos;
            }

            private bool RouteGrimReaperToEdgeOfTargetPool()
            {
                Pool pool = Target.Posture.Container as Pool;
                if (pool != null)
                {
                    return pool.RouteToEdge(Actor);
                }
                return false;
            }

            protected virtual void EventCallbackFadeBodyFromPose(StateMachineClient sender, IEvent evt)
            {
                switch (evt.EventId)
                {
                    case 101u:
                        Target.FadeOut();
                        mDeathEffect = mGrave.GetSimToGhostEffect(Target, mGhostPosition);
                        mSMCDeath.SetEffectActor("deathEffect", mDeathEffect);
                        mDeathEffect.Start();
                        break;
                    case 102u:
                        Target.SetPosition(mGhostPosition);
                        mGrave.GhostSetup(Target, false);
                        Target.SetHiddenFlags(HiddenFlags.Nothing);
                        if (mDeathEffect != null)
                        {
                            mDeathEffect.Stop();
                            mDeathEffect = null;
                        }
                        Target.FadeIn();
                        break;
                }
            }

            protected void EventCallbackSimToGhostEffectNoFadeOut(StateMachineClient sender, IEvent evt)
            {
                mDeathEffect = mGrave.GetSimToGhostEffect(Target, mGhostPosition);
                mSMCDeath.SetEffectActor("deathEffect", mDeathEffect);
                mDeathEffect.Start();
            }

            private void StartGhostExplosion()
            {
                if (Target.SimDescription.DeathStyle == SimDescription.DeathType.Freeze)
                {
                    mGhostExplosion = VisualEffect.Create("ashesToAshesFreeze");
                }
                else
                {
                    mGhostExplosion = VisualEffect.Create("ashesToAshes");
                }
                mGhostExplosion.SetPosAndOrient(Target.Position, Target.ForwardVector, Target.UpVector);
                mGhostExplosion.Start();
            }

            private void StopGhostExplosion()
            {
                if (mGhostExplosion != null)
                {
                    mGhostExplosion.Stop();
                    mGhostExplosion = null;
                }
            }

            protected void ReapSoulCallback()
            {
                int num = 0;
                do
                {
                    Simulator.Sleep(10u);
                    num++;
                }
                while (Target.ReferenceList.Count > 0 && num < 30);
                CleanupAndDestroyDeadSim(false);
            }

            public void CleanupAndDestroyDeadSim(bool forceCleanup)
            {
                if (mDeathEffect != null)
                {
                    mDeathEffect.Stop();
                    mDeathEffect = null;
                }
                Target.ClearReferenceList();
                if (mSituation.LastSimOfHousehold == null || mSituation.LastSimOfHousehold != Target || forceCleanup)
                {
                    if (Target.Household == null || Target.Household.IsPetHousehold)
                    {
                        SimDescription simDescription = Target.SimDescription;
                        PetPoolType petPoolType = PetPoolType.None;
                        switch (simDescription.Species)
                        {
                            case CASAgeGenderFlags.Dog:
                            case CASAgeGenderFlags.LittleDog:
                                petPoolType = PetPoolType.StrayDog;
                                break;
                            case CASAgeGenderFlags.Cat:
                                petPoolType = PetPoolType.StrayCat;
                                break;
                            case CASAgeGenderFlags.Horse:
                                petPoolType = ((!simDescription.IsUnicorn) ? PetPoolType.WildHorse : PetPoolType.Unicorn);
                                break;
                        }
                        if (PetPoolManager.IsPetInPoolType(simDescription, petPoolType))
                        {
                            PetPoolManager.RemovePet(petPoolType, simDescription);
                        }
                    }
                    mGrave.GhostCleanup(Target, true);
                    if (Target.Autonomy != null)
                    {
                        Target.Autonomy.DecrementAutonomyDisabled();
                    }
                    Target.SimDescription.ShowSocialsOnSim = true;
                    if (MoveToMausoleum(mGrave) && !mWasMemberOfActiveHousehold && Household.ActiveHousehold != null && Actor.LotCurrent != Household.ActiveHousehold.LotHome)
                    {
                        mGrave.FadeOut(false, 5f, HandleNPCGrave);
                    }
                    Target.Destroy();
                }
            }

            public static bool MoveToMausoleum(Urnstone urnstone)
            {
                List<IMausoleum> list = new List<IMausoleum>(Sims3.Gameplay.Queries.GetObjects<IMausoleum>());
                if (list.Count == 0)
                {
                    return false;
                }
                if (list.Count > 0)
                {
                    return true;
                }
                return false;
            }

            public void EventCallbackResurrectSim()
            {
                bool bResetAge = false;
                if (Target.SimDescription.DeathStyle == SimDescription.DeathType.OldAge)
                {
                    bResetAge = true;
                }
                mGrave.GhostToSim(Target, bResetAge, false);
                Actor.ClearSynchronizationData();
                mGrave.FadeOut(false, true);
                Target.SimDescription.IsNeverSelectable = false;
                Target.SimDescription.ShowSocialsOnSim = true;
                Target.Autonomy.DecrementAutonomyDisabled();
            }

            public virtual void EventCallbackResurrectSimDeathFlower(StateMachineClient sender, IEvent evt)
            {
                EventTracker.SendEvent(EventTypeId.kGotSavedByDeathFlower, Target);
                mDeathProgress = DeathProgress.DeathFlowerPostEvent;
                if (ShouldDoDeathEvent(Target))
                {
                    StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Services/GrimReaper:DeathFlower1", Target), Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                    StyledNotification.Show(format);
                }
                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.BalloonData(mDeathFlower.GetThoughtBalloonThumbnailKey());
                balloonData.mPriority = ThoughtBalloonPriority.High;
                balloonData.Duration = ThoughtBalloonDuration.Medium;
                balloonData.mCoolDown = ThoughtBalloonCooldown.Medium;
                balloonData.LowAxis = ThoughtBalloonAxis.kLike;
                Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
                EventCallbackResurrectSim();
            }

            public void EventCallbackResurrectSimUnlucky(StateMachineClient sender, IEvent evt)
            {
                mDeathProgress = DeathProgress.UnluckyPostEvent;
                if (ShouldDoDeathEvent(Target))
                {
                    StyledNotification.Format format = new StyledNotification.Format("Good Sim :)", Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                    StyledNotification.Show(format);
                }
                EventCallbackResurrectSim();
            }

            public void EventCallbackResurrectSimRanting(StateMachineClient sender, IEvent evt)
            {
                mDeathProgress = DeathProgress.UnluckyPostEvent;
                if (ShouldDoDeathEvent(Target))
                {
                    StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Services/GrimReaper:RantingWarning", Target), Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
                    StyledNotification.Show(format);
                }
                Target.BuffManager.AddElement(BuffNames.ThereAndBackAgain, Origin.FromRanting);
                EventCallbackResurrectSim();
            }

            public void GrimReaperPostSequenceCleanup()
            {
                Actor.Posture = Actor.Standing;
            }

            public void FinalizeDeath()
            {
                //MakeHouseholdHorsesGoHome();
                if (Target.SimDescription.IsEnrolledInBoardingSchool())
                {
                    Target.SimDescription.BoardingSchool.OnRemovedFromSchool();
                }
                if (Target.Household != null)
                {
                    Urnstone.FinalizeSimDeath(Target.SimDescription, Target.Household, mSituation.PetSavior == null);
                }
                else
                {
                    Urnstone.FinalizeSimDeath(Target.SimDescription, Household.NpcHousehold, mSituation.PetSavior == null);
                }
                int minuteOfDeath = (int)Math.Floor((double)SimClock.ConvertFromTicks(SimClock.CurrentTime().Ticks, TimeUnit.Minutes)) % 60;
                mGrave.MinuteOfDeath = minuteOfDeath;
                if (Target.DeathReactionBroadcast != null)
                {
                    Target.DeathReactionBroadcast.Dispose();
                    Target.DeathReactionBroadcast = null;
                }
                Target.SetHiddenFlags(HiddenFlags.Everything);
                Household household = Target.Household;
                if (household != null)
                {
                    // Moded Not If
                    /*
                    if (household.IsActive)
                    {
                        Target.MoveInventoryItemsToAFamilyMember();
                    }
                    */
                    Target.MoveInventoryItemsToAFamilyMember();
                    Target.LotCurrent.LastDiedSim = Target.SimDescription;
                    Target.LotCurrent.NumDeathsOnLot++;
                    Actor.ClearSynchronizationData();
                    /*
                    if (Target.SimDescription.DeathStyle != SimDescription.DeathType.OldAge)
                    {
                        Actor.RemoveInteractionByType(ChessChallenge.Singleton);
                    }
                     * */
                    if (BoardingSchool.ShouldSimsBeRemovedFromBoardingSchool(household))
                    {
                        BoardingSchool.RemoveAllSimsFromBoardingSchool(household);
                    }
                    if (!Target.BuffManager.HasElement(BuffNames.Ensorcelled))
                    {
                        int num = 0;
                        foreach (Sim allActor in household.AllActors)
                        {
                            if (allActor.BuffManager.HasElement(BuffNames.Ensorcelled))
                            {
                                num++;
                            }
                        }
                        if (household.AllActors.Count == num + 1)
                        {
                            foreach (Sim allActor2 in household.AllActors)
                            {
                                if (allActor2.BuffManager.HasElement(BuffNames.Ensorcelled))
                                {
                                    allActor2.BuffManager.RemoveElement(BuffNames.Ensorcelled);
                                }
                            }
                        }
                    }
                    int num2 = household.AllActors.Count - household.GetNumberOfRoommates();
                    if (household.IsActive && num2 == 1 && !Household.RoommateManager.IsNPCRoommate(Target))
                    {
                        mSituation.LastSimOfHousehold = Target;
                    }
                    else
                    {
                        if (Target.IsActiveSim)
                        {
                            LotManager.SelectNextSim();
                        }
                        household.RemoveSim(Target);
                    }
                }
                mGrave.RemoveFromUseList(Actor);
                Ocean singleton = Ocean.Singleton;
                if (singleton != null && singleton.IsActorUsingMe(Target))
                {
                    singleton.RemoveFromUseList(Target);
                    Target.Posture = null;
                }
            }

            public void HandleNPCGrave()
            {
                if (MoveToMausoleum(mGrave))
                {
                    AddGraveToRandomMausoleum(mGrave);
                    return;
                }
                mGrave.FadeIn();
            }


            public static void AddGraveToRandomMausoleum(Urnstone urn)
            {
                if (!MoveToMausoleum(urn))
                {
                    return;
                }
                List<IMausoleum> list = new List<IMausoleum>(Sims3.Gameplay.Queries.GetObjects<IMausoleum>());
                if (list.Count > 0)
                {
                    IMausoleum randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                    if (randomObjectFromList != null)
                    {
                        foreach (Sim reference in urn.ReferenceList)
                        {
                            reference.InteractionQueue.PurgeInteractions(urn);
                        }
                        if (!randomObjectFromList.Inventory.TryToAdd(urn))
                        {
                            return;
                        }
                    }
                }
            }

            public void MakeHouseholdHorsesGoHome()
            {
                if (!mWasMemberOfActiveHousehold && !Target.IsAtHome)
                {
                    Lot lotCurrent = Target.LotCurrent;
                    if (!lotCurrent.IsWorldLot)
                    {
                        List<Sim> lazyList = null;
                        foreach (Sim allActor in Target.Household.AllActors)
                        {
                            if (allActor.LotCurrent == lotCurrent && allActor != Target)
                            {
                                if (allActor.IsHuman)
                                {
                                    if (allActor.SimDescription.ChildOrAbove)
                                    {
                                        return;
                                    }
                                }
                                else if (allActor.IsHorse)
                                {
                                    Lazy.Add(ref lazyList, allActor);
                                }
                            }
                        }
                        if (lazyList != null)
                        {
                            foreach (Sim item in lazyList)
                            {
                                Sim.MakeSimGoHome(item, false);
                            }
                        }
                    }
                }
            }

            public void MermaidDehydratedToGhostSequence()
            {
                StartGhostExplosion();
                mSMCDeath.RequestState(true, "y", "PoseDehydrate");
                Target.FadeOut();
                mDeathEffect = mGrave.GetSimToGhostEffect(Target, mGhostPosition);
                if (mDeathEffect != null)
                {
                    mSMCDeath.SetEffectActor("deathEffect", mDeathEffect);
                    mDeathEffect.Start();
                }
                Target.SetPosition(mGhostPosition);
                mGrave.GhostSetup(Target, false);
                Target.SetHiddenFlags(HiddenFlags.Nothing);
                if (mDeathEffect != null)
                {
                    mDeathEffect.Stop();
                    mDeathEffect = null;
                }
                Target.FadeIn();
                mSMCDeath.RequestState(true, "y", "DehydrateToFloat");
                mSMCDeath.RequestState(false, "y", "GhostFloating");
                StopGhostExplosion();
            }
        }



        // End





































        // Interaction
        public sealed class NiecAppear : Interaction<Sim, Sim>
        {
            [DoesntRequireTuning]
            private sealed class Definition : InteractionDefinition<Sim, Sim, NiecAppear>, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return "Niec Appear";
                }


                public override string[] GetPath(bool bPath)
                {
                    return new string[] { "Niec Helper Situation" };
                }

                public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    /*
                    if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                    {
                        if (actor == null) throw new ArgumentNullException("actor");
                        if (target == null) throw new ArgumentNullException("target");
                        if (isAutonomous) throw new System.TypeLoadException("Cant Find Autonomous");
                    }
                     * */
                    if (actor == null) return false;
                    if (target == null) return false;
                    if (isAutonomous && actor.IsNPC) return false;
                    //if (actor.IsNPC) return false;
                    NiecHelperSituation situationOfType = actor.GetSituationOfType<NiecHelperSituation>();
                    if (situationOfType == null) return false;
                    if (situationOfType.OkSuusse) return false;
                    //return true;
                    return actor.IsSelectable;
                }
                public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    try
                    {
                        if (!Test(parameters.Actor as Sim, parameters.Target as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                        {
                            return InteractionTestResult.Def_TestFailed;
                        }
                        return InteractionTestResult.Pass;
                    }
                    catch (Exception)
                    { return InteractionTestResult.GenericFail; }

                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();


            public NiecHelperSituation mSituation;




            








            public bool Sitoaito()
            {
                //mSituation = (ServiceSituation.FindServiceSituationInvolving(Actor) as GrimReaperSituation);
                mSituation.AddRelationshipWithEverySimInHousehold();
                Actor.SetOpacity(0f, 0f);
                Actor.SetPosition(mSituation.Lot.Position);
                Actor.RequestWalkStyle(Sim.WalkStyle.ScubaDive);
                SimDescription.DeathType deathType = SimDescription.DeathType.ScubaDrown;
                Vector3 position = Vector3.Invalid;
                Vector3 invalid = Vector3.Invalid;
                Sim sim = Target; //mSituation.FindClosestDeadDivingSim();
                if (sim != null)
                {

                    if (sim.SimDescription.DeathStyle == SimDescription.DeathType.None)
                    {
                        deathType = SimDescription.DeathType.ScubaDrown;
                    }
                    else
                    {
                        deathType = sim.SimDescription.DeathStyle;
                    }
                     
                    //deathType = SimDescription.DeathType.Shark;
                    position = sim.Position;
                    position.y = World.GetLevelHeight(position.x, position.z, 0);
                    Vector3 forward = sim.ForwardVector;
                    if (mSituation.mScubaDeathJig == null)
                    {
                        mSituation.mScubaDeathJig = (GlobalFunctions.CreateObjectOutOfWorld("UnderwaterSocial_Jig", ProductVersion.EP10) as SocialJig);
                        position.y = World.GetLevelHeight(position.x, position.z, 0);
                        if (GlobalFunctions.FindGoodLocationNearby(mSituation.mScubaDeathJig, new ObjectGuid[2]
						{
							sim.ObjectId,
							Actor.ObjectId
						}, ref position, ref forward, 0f, GlobalFunctions.FindGoodLocationStrategies.All, FindGoodLocationBooleans.AllowOnSlopes | FindGoodLocationBooleans.AllowInSea))
                        {
                            mSituation.mScubaDeathJig.SetPosition(position);
                            mSituation.mScubaDeathJig.SetForward(forward);
                            Actor.SetPosition(mSituation.mScubaDeathJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimA));
                            Actor.SetForward(mSituation.mScubaDeathJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimA));
                        }
                    }
                }
                Actor.GreetSimOnLot(mSituation.Worker.LotCurrent);
                mSituation.SMCDeath = StateMachineClient.Acquire(Actor, "DeathSequenceScuba");
                mSituation.SMCDeath.SetActor("x", Actor);
                mSituation.SMCDeath.EnterState("x", "Enter");
                Urnstone.FogEffectTurnAllOn(mSituation.Worker.LotCurrent);
                try
                {
                    mDeathEffect = Urnstone.ReaperApperEffect(Actor, deathType);
                }
                catch (Exception)
                { }
               
                if (ShouldDoDeathEvent(sim))
                {
                    Audio.StartSound("sting_death");
                    //Camera.FocusOnSim(Actor);
                }
                mSituation.SMCDeath.AddOneShotScriptEventHandler(103u, FadeInReaper);
                mSituation.SMCDeath.RequestState("x", "DiveDown");
                try
                {
                    mDeathEffect.Stop();
                    mDeathEffect.Dispose();
                }
                catch (Exception)
                { }
                
                mSituation.ReaperLoop = new ObjectSound(Actor.ObjectId, "death_reaper_lp");
                mSituation.ReaperLoop.Start(true);
                if (Target != Actor)
                {
                    var goToLoAtx = ReapSoul.Singleton.CreateInstance(Target, Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer) as ReapSoul;
                    goToLoAtx.RunInteraction();
                    
                }
                return true;
            }

            public VisualEffect mDeathEffect;


           

            public void FadeInReaper(StateMachineClient sender, IEvent evt)
            {
                if (mDeathEffect != null)
                {
                    mDeathEffect.Start();
                }
                if (mSituation != null)
                {
                    mSituation.StartGrimReaperSmoke();
                }
                Actor.FadeIn(false, 3f);
            }


            public override bool Run()
            {
                NiecHelperSituation niecHelperSituation = Actor.GetSituationOfType<NiecHelperSituation>();
                try
                {
                    if (niecHelperSituation != null && !niecHelperSituation.OkSuusse && Actor.IsSelectable)
                    {
                        mSituation = niecHelperSituation;

                        Lot ActorLot = Actor.LotCurrent;
                        Lot TargetLot = Target.LotCurrent;
                        niecHelperSituation.OkSuusse = true;


                        if (ActorLot != null && ActorLot.IsDivingLot || TargetLot != null && TargetLot.IsDivingLot && Target.Posture != null && Target.Posture is ScubaDiving && !Target.SimDescription.IsMermaid)
                        {
                            niecHelperSituation.OkSuusseD = true;
                            return Sitoaito();
                        }
                        else
                        {
                            niecHelperSituation.OkSuusseD = false;
                        }
                        
                        Actor.SetPosition(niecHelperSituation.Lot.Position);
                        SimDescription.DeathType deathType = SimDescription.DeathType.Drown;
                        Sim sim = Target; //niecHelperSituation.FindClosestDeadSim();
                        if (sim != null)
                        {
                            Vector3 pos = Vector3.Invalid;
                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(sim.Position);
                            Vector3 fwd;
                            if (!GlobalFunctions.FindGoodLocation(Actor, fglParams, out pos, out fwd))
                            {
                                pos = fglParams.StartPosition;
                            }
                            Actor.SetPosition(pos);
                            Actor.RouteTurnToFace(sim.Position);
                            if (sim.SimDescription.DeathStyle == SimDescription.DeathType.None)
                            {
                                List<SimDescription.DeathType> list = new List<SimDescription.DeathType>();
                                list.Add(SimDescription.DeathType.Drown);
                                list.Add(SimDescription.DeathType.Starve);
                                list.Add(SimDescription.DeathType.Thirst);
                                list.Add(SimDescription.DeathType.Burn);
                                list.Add(SimDescription.DeathType.Freeze);
                                list.Add(SimDescription.DeathType.ScubaDrown);
                                list.Add(SimDescription.DeathType.Shark);
                                list.Add(SimDescription.DeathType.Jetpack);
                                list.Add(SimDescription.DeathType.Meteor);
                                list.Add(SimDescription.DeathType.Causality);
                                if (!Target.SimDescription.IsFrankenstein)
                                {
                                    list.Add(SimDescription.DeathType.Electrocution);
                                }
                                list.Add(SimDescription.DeathType.Burn);
                                if (Target.SimDescription.Elder)
                                {
                                    list.Add(SimDescription.DeathType.OldAge);
                                }
                                if (Target.SimDescription.IsWitch)
                                {
                                    list.Add(SimDescription.DeathType.HauntingCurse);
                                }
                                SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                                //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                deathType = randomObjectFromList;
                            }
                            else
                            {
                                deathType = sim.SimDescription.DeathStyle;
                            }
                        }
                        Actor.GreetSimOnLot(niecHelperSituation.Worker.LotCurrent);
                        niecHelperSituation.SMCDeath = StateMachineClient.Acquire(Actor, "DeathSequence");
                        niecHelperSituation.SMCDeath.SetActor("x", Actor);
                        niecHelperSituation.SMCDeath.EnterState("x", "Enter");
                        Urnstone.FogEffectTurnAllOn(niecHelperSituation.Worker.LotCurrent);
                        /*
                        if (deathType == SimDescription.DeathType.None)
                        {
                        }
                         * */
                        VisualEffect visualEffect = Urnstone.ReaperApperEffect(Actor, deathType);
                        visualEffect.Start();
                        //grimReaperSituation.StartGrimReaperSmoke();
                        VisualEffect.FireOneShotEffect("reaperSmokeConstant", Actor, Sim.FXJoints.Pelvis, VisualEffect.TransitionType.HardTransition);
                        if (ShouldDoDeathEvent(sim))
                        {
                            Audio.StartSound("sting_death");
                        }
                        niecHelperSituation.SMCDeath.AddOneShotScriptEventHandler(666u, EventCallbackFadeInReaper);
                        niecHelperSituation.SMCDeath.RequestState("x", "ReaperBrushingHimself");
                        visualEffect.Stop();
                        niecHelperSituation.ReaperLoop = new ObjectSound(Actor.ObjectId, "death_reaper_lp");
                        niecHelperSituation.ReaperLoop.Start(true);
                        if (Target != Actor)
                        {
                            Actor.Posture = new SimCarryingObjectPosture(Actor, null);
                            var goToLoAtx = ReapSoul.Singleton.CreateInstance(Target, Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer) as ReapSoul;
                            goToLoAtx.RunInteraction();
                        }
                        //Actor.Posture = new SimCarryingObjectPosture(Actor, null);
                    }
                    
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage("NHS " + exception.Message + NiecException.NewLine + exception.StackTrace);
                    if (niecHelperSituation != null)
                    {
                        niecHelperSituation.OkSuusseD = false;
                        niecHelperSituation.OkSuusse = false;
                    }
                    Actor.ResetAllAnimation();
                    NiecMod.Nra.SpeedTrap.Sleep();
                    AnimationUtil.StopAllAnimation(Actor);
                    NiecMod.Nra.SpeedTrap.Sleep();
                    Actor.SetObjectToReset();
                }
                
                return true;
            }

            public void EventCallbackFadeInReaper(StateMachineClient sender, IEvent evt)
            {
                Actor.FadeIn(false, 3f);
            }
        }

        // End Interaction







        














        ///// End

        // ChildSituation
        public class Spawn : ChildSituation<NiecHelperSituation>
        {
            private Spawn()
            {
            }

            public Spawn(NiecHelperSituation parent)
                : base(parent)
            {
            }

            public override void Init(NiecHelperSituation parent)
            {
                
                parent.Worker.Autonomy.Motives.MaxEverything();
                //parent.Worker.Autonomy.Motives.FreezeDecayEverythingExcept();
                //ForceSituationSpecificInteraction(priority: new InteractionPriority((InteractionPriorityLevel)350, 999f), x: parent.Worker, s: parent.Worker, i: NiecAppear.Singleton, callbackOnStart: null, callbackOnCompletion: null, callbackOnFailure: NiecCFailed);
                return;
            }

            /*
            public void NiecCFailed(Sim actor, float x)
            {
                ForceSituationSpecificInteraction(priority: new InteractionPriority((InteractionPriorityLevel)350, 999f), x: parent.Worker, s: actor, i: NiecAppear.Singleton, callbackOnStart: null, callbackOnCompletion: null, callbackOnFailure: NiecCFailed);
            }
            */
            public override void CleanUp()
            {
                try
                {
                    base.AlarmManager.RemoveAlarm(mAlarmHandle);
                }
                catch
                { }
                
                base.CleanUp();
            }

            private AlarmHandle mAlarmHandle = AlarmHandle.kInvalidHandle;
        }


        // ChildSituation
        public class Working : ChildSituation<NiecHelperSituation>
        {
            private Working()
            {
            }

            public Working(NiecHelperSituation parent)
                : base(parent)
            {
            }

            public override void Init(NiecHelperSituation parent)
            {
                return;
            }

            public override void CleanUp()
            {
                return;
            }
            
        }
    }
}
