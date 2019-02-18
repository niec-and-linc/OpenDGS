using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.CookingObjects;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using System;
using System.Collections.Generic;

namespace Sims3.Gameplay.Objects.Arsil
{
    public class JapLowDinnerTable : GameObject, ISurface, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IExportableContent, ISmashableObjectWhileBurning
    {
        public static bool HasJCTrait(Sim actor)
        {
            return actor.HasTrait((TraitNames)15572421960754265911UL);
        }

        public SurfaceTypes GetSoundMaterial(uint index)
        {
            return this.mSurfaceType;
        }

        public void SetSoundMaterial(uint index, SurfaceTypes soundMaterial)
        {
            if (index == 0u)
            {
                this.mSurfaceType = soundMaterial;
            }
        }

        internal static int UMIN(int lenght1, int lenght2)
        {
            if (lenght1 < lenght2)
            {
                return lenght1;
            }
            return lenght2;
        }

        public static void DebugMsg(string message)
        {
            if (JapLowDinnerTable.kDebugging)
            {
                StyledNotification.Show(new StyledNotification.Format("ArsilJapLowDinnerTable Debug\n\n" + message, StyledNotification.NotificationStyle.kGameMessagePositive));
            }
        }

        internal static bool FindClosestFreeSlot(Sim sim, JapLowDinnerTable table, out int slotIndex)
        {
            return sim.RouteToSlotList(table, Slots.GetRoutingSlotNames(table.ObjectId), out slotIndex);
        }

        internal static JapLowDinnerTable FindClosestFreeJPNTable(Sim actor)
        {
            List<JapLowDinnerTable> list = new List<JapLowDinnerTable>(Queries.GetObjects<JapLowDinnerTable>(actor.LotCurrent, actor.RoomId));
            List<JapLowDinnerTable> list2 = new List<JapLowDinnerTable>();
            foreach (JapLowDinnerTable japLowDinnerTable in list)
            {
                ObjectGuid containedObject = Slots.GetContainedObject(japLowDinnerTable.ObjectId, 2820733094u);
                if (containedObject == ObjectGuid.InvalidObjectGuid)
                {
                    list2.Add(japLowDinnerTable);
                }
            }
            list.Clear();
            if (list2.Count == 0)
            {
                list.AddRange(Queries.GetObjects<JapLowDinnerTable>(actor.LotCurrent));
            }
            foreach (JapLowDinnerTable japLowDinnerTable2 in list)
            {
                ObjectGuid containedObject = Slots.GetContainedObject(japLowDinnerTable2.ObjectId, 2820733094u);
                if (containedObject == ObjectGuid.InvalidObjectGuid)
                {
                    list2.Add(japLowDinnerTable2);
                }
            }
            if (list2.Count > 0)
            {
                return GlobalFunctions.GetClosestObject<JapLowDinnerTable>(list2, actor);
            }
            return null;
        }

        public JapLowDinnerTable()
        {
        }

        static JapLowDinnerTable()
        {
            CommodityChange item = new CommodityChange((CommodityKind)286210343, 100f, true, 100f, OutputUpdateType.ImmediateDelta, false, false, UpdateAboveAndBelowZeroType.Either);
            try
            {
                InteractionTuning tuning = AutonomyTuning.GetTuning(typeof(JapLowDinnerTable.Eat.Definition).FullName, typeof(JapLowDinnerTable).FullName);
                tuning.mTradeoff.mOutputs.Add(item);
                tuning = AutonomyTuning.GetTuning(typeof(JapLowDinnerTable.ServeMeal.Definition).FullName, typeof(ServingContainerGroup).FullName);
                tuning.mTradeoff.mOutputs.Add(item);
            }
            catch (Exception ex)
            {
                JapLowDinnerTable.DebugMsg("InteractionTuning(s) not updated\n\n" + ex.StackTrace.Substring(0, JapLowDinnerTable.UMIN(750, ex.StackTrace.Length)));
            }
            JapLowDinnerTable.mSurfaceAddOn = new SurfaceAddOn();
            JapLowDinnerTable.mSurfaceAddOn.AddSurface("ContainmentSlot_0", 1f, SurfaceType.Eating, SurfaceHeight.CoffeeTable, CarrySystemPutDownRule.Default, null);
            World.OnWorldLoadFinishedEventHandler += JapLowDinnerTable.OnWorldLoadFinished;
        }

        private static void OnWorldLoadFinished(object sender, EventArgs e)
        {
            if (JapLowDinnerTable.firstTimeOnWorldLoadFinished)
            {
                JapLowDinnerTable.firstTimeOnWorldLoadFinished = false;
                ServingContainer.PicnicBasketInteractionProxy.Singleton = new JapLowDinnerTable.PicnicBasketInteractionProxyModded.Definition();
            }
        }

        private bool ChangeIntoOutfit(Sim actor)
        {
            if (actor.BuffManager.HasTransformBuff())
            {
                return false;
            }
            bool result = false;
            actor.RefreshCurrentOutfit(false);
            SimOutfit uniform = null;
            string s = actor.SimDescription.Child ? "c" : "";
            string outfitName = s + "ArsilChopsticksOutfit";
            bool flag = false;
            if (flag || OutfitUtils.TryGenerateSimOutfit(outfitName, ProductVersion.BaseGame, out uniform))
            {
                SimOutfit outfit = null;
                SimOutfit outfit2 = actor.SimDescription.GetOutfit(actor.CurrentOutfitCategory, actor.CurrentOutfitIndex);
                if (flag || OutfitUtils.TryApplyUniformToOutfit(outfit2, uniform, actor.SimDescription, "ArsilChopsticksOutfit", out outfit))
                {
                    actor.SimDescription.AddSpecialOutfit(outfit, "ArsilChopsticksOutfit");
                    int specialOutfitIndexFromKey = actor.SimDescription.GetSpecialOutfitIndexFromKey(ResourceUtils.HashString32("ArsilChopsticksOutfit"));
                    actor.SwitchToOutfitWithoutSpin(OutfitCategories.Special, specialOutfitIndexFromKey);
                    result = true;
                }
            }
            return result;
        }

        internal static bool CheckIfHasFood(PreparedFood food)
        {
            if (food == null)
            {
                return false;
            }
            ServingContainer servingContainer = food as ServingContainer;
            Snack snack = food as Snack;
            return (servingContainer == null || servingContainer.HasFood) && (snack == null || snack.HasFood);
        }

        public override void OnStartup()
        {
            base.AddInteraction(JapLowDinnerTable.TakeASpot.Singleton);
            base.AddInteraction(JapLowDinnerTable.Nap.Singleton);
            base.AddInteraction(JapLowDinnerTable.Eat.Singleton);
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(JapLowDinnerTable.DeleteSpecialOutfit.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public SurfacePair Surface
        {
            get
            {
                return new SurfacePair(this.SurfaceAddOn, this);
            }
        }

        public SurfaceAddOn SurfaceAddOn
        {
            get
            {
                return JapLowDinnerTable.mSurfaceAddOn;
            }
        }

        public const ulong JAPCULTURE_TRAIT = 15572421960754265911UL;

        private const int JC_CK = 286210343;

        private const int JC_TRAIT_TIP = 9715;

        public const TraitNames JC_TRAIT_GUID = (TraitNames)15572421960754265911UL;

        private const string OUTFIT_NAME = "ArsilChopsticksOutfit";

        [Tunable]
        internal static bool kDebugging = true;

        [Tunable]
        internal static float kMealDuration = 30f;

        [Tunable]
        internal static float kMealDurationSlobTrait = 20f;

        [Tunable]
        internal static bool kEnableGroupTalkWhileEating = true;

        [Tunable]
        internal static float kGroupTalkMealDurationExtra = 10f;

        [Tunable]
        internal static bool kOnlyRecipesThatUseFork = true;

        [Tunable]
        internal static bool kOnlySimsWithJCTrait = true;

        [Tunable]
        internal static bool kServeOrEatSpoiledFood = true;

        [Tunable]
        internal static bool kServeOrEatBurntFood = true;

        protected SurfaceTypes mSurfaceType;

        protected static SurfaceAddOn mSurfaceAddOn;

        private static bool firstTimeOnWorldLoadFinished = true;

        internal sealed class PicnicBasketInteractionProxyModded : Interaction<Sim, ServingContainer>
        {
            public override bool Run()
            {
                throw new NotImplementedException("This interaction should never be run");
            }

            public PicnicBasketInteractionProxyModded()
            {
            }

            // Note: this type is marked as 'beforefieldinit'.
            static PicnicBasketInteractionProxyModded()
            {
            }

            public static readonly InteractionDefinition Singleton = new JapLowDinnerTable.PicnicBasketInteractionProxyModded.Definition();

            [DoesntRequireTuning]
            internal sealed class Definition : InteractionDefinition<Sim, ServingContainer, JapLowDinnerTable.PicnicBasketInteractionProxyModded>
            {
                public override string GetInteractionName(Sim a, ServingContainer target, InteractionObjectPair interaction)
                {
                    return "Never Displayed: PicnicBasketInteractionProxyModded";
                }

                public override bool Test(Sim Actor, ServingContainer Target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim Actor, ServingContainer Target, List<InteractionObjectPair> results)
                {
                    PicnicBlanket picnicBlanket = Target.Parent as PicnicBlanket;
                    if (picnicBlanket != null)
                    {
                        PicnicBasket basket = picnicBlanket.Basket;
                        if (basket != null)
                        {
                            results.AddRange(basket.GetAllInteractionsForActor(Actor));
                        }
                    }
                    JapLowDinnerTable.ServeMeal.Definition interaction = new JapLowDinnerTable.ServeMeal.Definition();
                    results.Add(new InteractionObjectPair(interaction, Target));
                }

                public Definition()
                {
                }
            }
        }

        public class ServeMeal : Interaction<Sim, ServingContainer>
        {
            public override void Init(ref InteractionInstanceParameters parameters)
            {
                base.Init(ref parameters);
                JapLowDinnerTable.ServeMeal.Definition definition = base.InteractionDefinition as JapLowDinnerTable.ServeMeal.Definition;
                this.mJT = definition.jt;
            }

            public override bool Run()
            {
                if (this.Target.Parent != this.Actor && !CarrySystem.PickUp(this.Actor, this.Target))
                {
                    return false;
                }
                if (this.Actor.GetObjectInRightHand() == null)
                {
                    return false;
                }
                if (this.mJT == null)
                {
                    Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
                    return false;
                }
                if (!this.Actor.RouteToObjectRadialRange(this.mJT, 0.5f, 1.5f))
                {
                    Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
                    return false;
                }
                ObjectGuid containedObject = Slots.GetContainedObject(this.mJT.ObjectId, 2820733094u);
                if (containedObject != ObjectGuid.InvalidObjectGuid)
                {
                    Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
                    return false;
                }
                if (this.Target.ParentToSlot(this.mJT, 2820733094u))
                {
                    CarrySystem.ExitCarry(this.Actor);
                    return true;
                }
                Food.PutHeldObjectDownOnCounterTableOrFloor(this.Actor, SurfaceType.Normal);
                return false;
            }

            public ServeMeal()
            {
            }

            // Note: this type is marked as 'beforefieldinit'.
            static ServeMeal()
            {
            }

            public static readonly InteractionDefinition Singleton = new JapLowDinnerTable.ServeMeal.Definition();

            private JapLowDinnerTable mJT;

            public sealed class Definition : InteractionDefinition<Sim, ServingContainer, JapLowDinnerTable.ServeMeal>
            {
                public Definition()
                {
                    this.jt = null;
                }

                public Definition(JapLowDinnerTable jt_param)
                {
                    this.jt = jt_param;
                }

                public override bool Test(Sim actor, ServingContainer target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!target.HasFood)
                    {
                        return false;
                    }
                    if (!JapLowDinnerTable.kServeOrEatSpoiledFood && target.IsSpoiled)
                    {
                        return false;
                    }
                    if (JapLowDinnerTable.kOnlySimsWithJCTrait && !JapLowDinnerTable.HasJCTrait(actor))
                    {
                        return false;
                    }
                    if (target.Parent is JapLowDinnerTable)
                    {
                        return false;
                    }
                    if (JapLowDinnerTable.kOnlyRecipesThatUseFork && (target.CookingProcess == null || target.CookingProcess.Recipe == null || target.CookingProcess.Recipe.Utensil != "fork"))
                    {
                        return false;
                    }
                    if (!JapLowDinnerTable.kServeOrEatBurntFood && target.CookingProcess != null && target.CookingProcess.FoodState == FoodCookState.Burnt)
                    {
                        return false;
                    }
                    if (this.jt != null)
                    {
                        return true;
                    }
                    JapLowDinnerTable japLowDinnerTable = JapLowDinnerTable.FindClosestFreeJPNTable(actor);
                    if (japLowDinnerTable != null)
                    {
                        this.jt = japLowDinnerTable;
                        return true;
                    }
                    return false;
                }

                public override string GetInteractionName(Sim actor, ServingContainer target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString(17375095599848568652UL);
                }

                internal JapLowDinnerTable jt;
            }
        }

        private sealed class TakeASpot : Interaction<Sim, JapLowDinnerTable>
        {
            public override bool Run()
            {
                int num = -1;
                if (!JapLowDinnerTable.FindClosestFreeSlot(this.Actor, this.Target, out num))
                {
                    return false;
                }
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.AcquireStateMachine("ArsilSeiza");
                base.SetActor("x", this.Actor);
                base.EnterState("x", "Enter");
                base.AnimateSim("Idle");
                bool result = true;
                this.Actor.Posture = new TableSeizaPosture(this.Actor, this.Target, this.mCurrentStateMachine);
                base.StandardExit(false, false);
                return result;
            }

            public TakeASpot()
            {
            }

            // Note: this type is marked as 'beforefieldinit'.
            static TakeASpot()
            {
            }

            public static readonly InteractionDefinition Singleton = new JapLowDinnerTable.TakeASpot.Definition();

            public sealed class Definition : InteractionDefinition<Sim, JapLowDinnerTable, JapLowDinnerTable.TakeASpot>
            {
                public override string GetInteractionName(Sim actor, JapLowDinnerTable target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString("Arsil/JapCulture/Seiza", new object[0]);
                }

                public override bool Test(Sim a, JapLowDinnerTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.ActorsUsingMe.Contains(a))
                    {
                        return false;
                    }
                    if (target.InUse && target.ActorsUsingMe.Count >= 4)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("No Free Spots");
                        return false;
                    }
                    return true;
                }

                public Definition()
                {
                }
            }
        }

        internal sealed class Nap : Interaction<Sim, JapLowDinnerTable>
        {
            public override void ConfigureInteraction()
            {
                TimedStage timedStage = new TimedStage(this.GetInteractionName(), (float)Sim.NapLengthInMinutes, false, true, true);
                base.Stages = new List<Stage>(new Stage[]
				{
					timedStage
				});
            }

            public override bool Run()
            {
                int num = -1;
                if (!JapLowDinnerTable.FindClosestFreeSlot(this.Actor, this.Target, out num))
                {
                    return false;
                }
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.AcquireStateMachine("ArsilSeiza");
                base.SetActor("x", this.Actor);
                base.EnterState("x", "Enter");
                base.AnimateSim("Idle");
                try
                {
                    base.AnimateSim("PreNap");
                }
                catch (Exception ex)
                {
                    JapLowDinnerTable.DebugMsg(ex.Message + "\n\n" + ex.StackTrace);
                }
                this.Actor.LookAtManager.DisableLookAts();
                this.Actor.SetIsSleeping(true);
                LightGameObject.AutoLightsOff(this.Actor);
                base.AnimateSim("Nap");
                this.mVisualEffect = VisualEffect.Create(this.Actor.OccultManager.GetSleepFXName());
                this.mVisualEffect.ParentTo(this.Actor, Sim.ContainmentSlots.Mouth);
                this.mVisualEffect.Start();
                base.StartStages();
                bool flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.NapLoop), null);
                base.EndCommodityUpdates(flag);
                if (flag)
                {
                    this.Actor.BuffManager.AddElement(BuffNames.HadANiceNap, Origin.FromNapping);
                }
                if (this.mVisualEffect != null)
                {
                    this.mVisualEffect.Stop();
                    this.mVisualEffect.Dispose();
                    this.mVisualEffect = null;
                }
                this.Actor.LookAtManager.EnableLookAts();
                this.Actor.SetIsSleeping(false);
                LightGameObject.AutoLightsOn(this.Actor);
                base.AnimateSim("PostNap");
                base.AnimateSim("Idle");
                base.AnimateSim("Exit");
                base.StandardExit();
                return flag;
            }

            private void NapLoop(StateMachineClient smc, InteractionInstance.LoopData data)
            {
                if (data.mLifeTime > BuffTiredFromMoving.TimeToRemove * 60f)
                {
                    this.Actor.BuffManager.RemoveElement(BuffNames.TiredFromMoving);
                }
                TraitFunctions.BedSnore(this.Actor, data.mLifeTime, ref this.NapSoundTime);
                TraitFunctions.CheckLightSleeperDisturbed(this.Actor);
            }

            public override void Cleanup()
            {
                this.Actor.SetIsSleeping(false);
                if (this.mVisualEffect != null)
                {
                    this.mVisualEffect.Stop();
                    this.mVisualEffect.Dispose();
                    this.mVisualEffect = null;
                }
                if (this.Actor != null)
                {
                    this.Actor.LookAtManager.EnableLookAts();
                }
                base.Cleanup();
            }

            public Nap()
            {
            }

            // Note: this type is marked as 'beforefieldinit'.
            static Nap()
            {
            }

            public static readonly InteractionDefinition Singleton = new JapLowDinnerTable.Nap.Definition();

            private float NapSoundTime = float.MinValue;

            internal VisualEffect mVisualEffect;

            private sealed class Definition : InteractionDefinition<Sim, JapLowDinnerTable, JapLowDinnerTable.Nap>
            {
                public override bool Test(Sim actor, JapLowDinnerTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.ActorsUsingMe.Count < 4 && !target.ActorsUsingMe.Contains(actor) && actor.CarryingChildPosture == null && actor.AbleToNap(isAutonomous, ref greyedOutTooltipCallback, target);
                }

                public override string GetInteractionName(Sim actor, JapLowDinnerTable target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString(2816015966040633415UL);
                }

                public Definition()
                {
                }
            }
        }

        private sealed class Eat : Interaction<Sim, JapLowDinnerTable>
        {
            public override bool Run()
            {
                int num = -1;
                if (!JapLowDinnerTable.FindClosestFreeSlot(this.Actor, this.Target, out num))
                {
                    return false;
                }
                ObjectGuid containedObject = Slots.GetContainedObject(this.Target.ObjectId, 2820733094u);
                if (containedObject == ObjectGuid.InvalidObjectGuid)
                {
                    return false;
                }
                this.mServingContainer = containedObject.ObjectFromId<PreparedFood>();
                if (this.mServingContainer == null || !JapLowDinnerTable.CheckIfHasFood(this.mServingContainer))
                {
                    return false;
                }
                this.mServingContainer.AddToUseList(this.Actor);
                if (this.Actor.HasTrait(TraitNames.Slob))
                {
                    this.mEatingDuration = JapLowDinnerTable.kMealDurationSlobTrait;
                }
                else
                {
                    this.mEatingDuration = JapLowDinnerTable.kMealDuration;
                }
                if (JapLowDinnerTable.kEnableGroupTalkWhileEating)
                {
                    this.mEatingDuration += JapLowDinnerTable.kGroupTalkMealDurationExtra;
                }
                this.mLastReactionTime = SimClock.ElapsedTime(TimeUnit.Minutes);
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.AcquireStateMachine("ArsilSeiza");
                base.SetActor("x", this.Actor);
                base.EnterState("x", "Enter");
                base.AnimateSim("Idle");
                if (!this.Target.ChangeIntoOutfit(this.Actor))
                {
                    base.AnimateSim("Exit");
                    this.Actor.PlayReaction(ReactionTypes.Shrug, ReactionSpeed.Immediate);
                    base.StandardExit();
                    return false;
                }
                Simulator.Sleep(1u);
                bool isSufficientlyFullForStuffed = false;
                bool hasFatDelta = false;
                Food.PreEat(this.Actor, this.mServingContainer, ref isSufficientlyFullForStuffed, ref hasFatDelta);
                base.AnimateSim("PreEat");
                base.AnimateSim("Eat");
                bool result = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.LoopCallback), this.mCurrentStateMachine);
                base.AnimateSim("PostEat");
                if (this.mServingContainer != null && this.mServingContainer.ActorsUsingMe.Contains(this.Actor))
                {
                    this.mServingContainer.ActorsUsingMe.Remove(this.Actor);
                }
                this.Actor.SwitchToPreviousOutfitWithoutSpin();
                base.AnimateSim("Idle");
                base.AnimateSim("Exit");
                Food.PostEat(this.Actor, this.mServingContainer, isSufficientlyFullForStuffed, true, hasFatDelta);
                base.StandardExit();
                return result;
            }

            public override void Cleanup()
            {
                if (this.Actor.IsWearingSpecialOutfit("ArsilChopsticksOutfit"))
                {
                    this.Actor.SwitchToPreviousOutfitWithoutSpin();
                }
                if (this.Target.ActorsUsingMe.Contains(this.Actor))
                {
                    this.Target.ActorsUsingMe.Remove(this.Actor);
                }
                if (this.mServingContainer != null && this.mServingContainer.ActorsUsingMe.Contains(this.Actor))
                {
                    this.mServingContainer.ActorsUsingMe.Remove(this.Actor);
                }
            }

            private void LoopCallback(StateMachineClient smc, InteractionInstance.LoopData ld)
            {
                if (!JapLowDinnerTable.CheckIfHasFood(this.mServingContainer))
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                    return;
                }
                if (this.mServingContainer is ServingContainerSingle)
                {
                    ServingContainerSingle servingContainerSingle = this.mServingContainer as ServingContainerSingle;
                    servingContainerSingle.DecrementFoodUnits();
                }
                else if (this.mServingContainer is Snack)
                {
                    Snack snack = this.mServingContainer as Snack;
                    snack.DecrementFoodUnits();
                }
                this.Actor.Motives.ChangeValue(CommodityKind.Hunger, 10f);
                if (this.Actor.ShouldDoGroupTalk())
                {
                    this.Actor.DoGroupTalk(new Sim.GroupTalkDelegate(this.PauseSim), new Sim.GroupTalkDelegate(this.UnpauseSim), true);
                }
                if (SimClock.ElapsedTime(TimeUnit.Minutes) - this.mLastReactionTime > this.mEatingDuration)
                {
                    this.Actor.AddExitReason(ExitReason.Finished);
                    if (this.mServingContainer is ServingContainerGroup)
                    {
                        ServingContainerGroup servingContainerGroup = this.mServingContainer as ServingContainerGroup;
                        servingContainerGroup.DecrementServings();
                    }
                }
            }

            private void PauseSim(bool isSpeaking)
            {
                if (isSpeaking)
                {
                    base.AnimateSim("Pause");
                }
            }

            private void UnpauseSim(bool isSpeaking)
            {
                if (isSpeaking)
                {
                    this.mCurrentStateMachine.Return("x");
                }
            }

            public Eat()
            {
            }

            // Note: this type is marked as 'beforefieldinit'.
            static Eat()
            {
            }

            public static readonly InteractionDefinition Singleton = new JapLowDinnerTable.Eat.Definition();

            internal float mLastReactionTime;

            internal float mEatingDuration;

            private PreparedFood mServingContainer;

            public sealed class Definition : InteractionDefinition<Sim, JapLowDinnerTable, JapLowDinnerTable.Eat>
            {
                public override string GetInteractionName(Sim actor, JapLowDinnerTable target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString(1324834093644869501UL);
                }

                public override bool Test(Sim a, JapLowDinnerTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    ObjectGuid containedObject = Slots.GetContainedObject(target.ObjectId, 2820733094u);
                    if (containedObject == ObjectGuid.InvalidObjectGuid)
                    {
                        return false;
                    }
                    PreparedFood preparedFood = containedObject.ObjectFromId<PreparedFood>();
                    if (!JapLowDinnerTable.CheckIfHasFood(preparedFood))
                    {
                        return false;
                    }
                    if (!JapLowDinnerTable.kServeOrEatSpoiledFood && preparedFood.IsSpoiled)
                    {
                        return false;
                    }
                    if (JapLowDinnerTable.kOnlyRecipesThatUseFork && (preparedFood.CookingProcess == null || preparedFood.CookingProcess.Recipe == null || preparedFood.CookingProcess.Recipe.Utensil != "fork"))
                    {
                        return false;
                    }
                    if (!JapLowDinnerTable.kServeOrEatBurntFood && preparedFood.CookingProcess != null && preparedFood.CookingProcess.FoodState == FoodCookState.Burnt)
                    {
                        return false;
                    }
                    if (target.ActorsUsingMe.Contains(a))
                    {
                        return false;
                    }
                    if (target.InUse && target.ActorsUsingMe.Count >= 4)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("No Free Spots");
                        return false;
                    }
                    if (isAutonomous)
                    {
                        if (a.SimDescription.IsRobot)
                        {
                            return false;
                        }
                        if (a.BuffManager.HasElement(BuffNames.Stuffed))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                public Definition()
                {
                }
            }
        }

        public class DeleteSpecialOutfit : ImmediateInteraction<IActor, JapLowDinnerTable>
        {
            public override bool Run()
            {
                foreach (Sim sim in Queries.GetObjects<Sim>())
                {
                    try
                    {
                        sim.SimDescription.RemoveSpecialOutfit("ArsilChopsticksOutfit");
                    }
                    catch (Exception)
                    {
                    }
                }
                return true;
            }

            public DeleteSpecialOutfit()
            {
            }

            // Note: this type is marked as 'beforefieldinit'.
            static DeleteSpecialOutfit()
            {
            }

            public static readonly InteractionDefinition Singleton = new JapLowDinnerTable.DeleteSpecialOutfit.Definition();

            public sealed class Definition : ActorlessInteractionDefinition<IActor, JapLowDinnerTable, JapLowDinnerTable.DeleteSpecialOutfit>
            {
                public override bool Test(IActor actor, JapLowDinnerTable target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public Definition()
                {
                }
            }
        }
    }
}
