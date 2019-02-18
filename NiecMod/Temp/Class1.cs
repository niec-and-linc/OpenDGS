using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects.Elevator;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using DGSCore.Common;

namespace DGSCore.Override
{
    public sealed class DGSGoToVirtualHome : Interaction<Sim, Sim>, IRouteFromInventoryOrSelfWithoutCarrying
    {
        public bool SocializationAllowed(Sim simA, Sim simB)
        {
            return SocialInteraction.SocializationAllowedCommonTest(simA, simB);
        }

        public string GreyedOutTooltipText(Sim simA, Sim simB)
        {
            string entryKey = "Gameplay/Socializing/SocialInteractionA:CannotSocializeWhileGoingHome";
            return Localization.LocalizeString(simB.IsFemale, entryKey, new object[]
		{
			simB.SimDescription
		});
        }

        public override ThumbnailKey GetIconKey()
        {
            return new ThumbnailKey(new ResourceKey(Sim.GoToVirtualHome.kIconNameHash, 796721156u, 0u), ThumbnailSize.Medium);
        }

        public override bool Run()
        {
            VisitSituation.OnSimLeavingLot(this.Actor);
            Lot lot = this.Actor.SimDescription.VirtualLotHome;
            if (lot == null)
            {
                List<Lot> list = new List<Lot>();
                foreach (object obj in LotManager.AllLotsWithoutCommonExceptions)
                {
                    Lot lot2 = (Lot)obj;
                    if (lot2 != this.Actor.LotCurrent && lot2.LotId != 18446744073709551615UL)
                    {
                        list.Add(lot2);
                    }
                }
                if (list.Count > 0)
                {
                    lot = RandomUtil.GetRandomObjectFromList<Lot>(list);
                }
            }
            GroupingSituation.RemoveSimFromGroupSituation(this.Actor);
            if (this.Actor.LotCurrent.CommercialLotSubType == CommercialLotSubType.kEP10_Resort && this.Actor.IsHuman && this.Actor.SimDescription.ChildOrAbove && (this.Actor.SimDescription.AssignedRole == null || this.Actor.SimDescription.AssignedRole.Type != Role.RoleType.Paparazzi))
            {
                IResortTower[] objects = this.Actor.LotCurrent.GetObjects<IResortTower>();
                if (objects.Length > 0)
                {
                    IResortTower randomObjectFromList = RandomUtil.GetRandomObjectFromList<IResortTower>(objects);
                    InteractionInstance instance = randomObjectFromList.GetEnterTowerDefinition().CreateInstance(randomObjectFromList, this.Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), base.Autonomous, false);
                    if (this.Actor.InteractionQueue.PushAsContinuation(instance, false))
                    {
                        return true;
                    }
                }
            }
            InteractionInstance interactionInstance = null;
            if (lot != null)
            {
                DGSGoToVirtualHomeInternal goToVirtualHomeInternal;
                if (this.SuccessCallbackForSequence != null || this.FailureCallbackForSequence != null)
                {
                    goToVirtualHomeInternal = (DGSGoToVirtualHomeInternal.Singleton.CreateInstanceWithCallbacks(lot, this.Actor, this.Actor.InheritedPriority(), base.Autonomous, base.CancellableByPlayer, null, this.SuccessCallbackForSequence, this.FailureCallbackForSequence) as DGSGoToVirtualHomeInternal);
                }
                else
                {
                    goToVirtualHomeInternal = (DGSGoToVirtualHomeInternal.Singleton.CreateInstance(lot, this.Actor, this.Actor.InheritedPriority(), base.Autonomous, base.CancellableByPlayer) as DGSGoToVirtualHomeInternal);
                }
                goToVirtualHomeInternal.SimWalkStyle = this.SimWalkStyle;
                if (base.TryPushAsContinuation(goToVirtualHomeInternal))
                {
                    interactionInstance = goToVirtualHomeInternal;
                }
            }
            if (lot == null || interactionInstance == null || !this.Actor.InteractionQueue.HasInteraction(interactionInstance))
            {
                SimDescription simDescription = this.Actor.SimDescription;
                if (simDescription.Household != null && simDescription.Household.IsTouristHousehold)
                {
                    Sim.GoToVirtualHome.SendTouristHome(simDescription);
                }
                else
                {
                    if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                    {
                        return true;
                    }
                    this.Actor.FadeOut(true, true);
                }
                Simulator.Sleep(uint.MaxValue);
            }
            return true;
        }

        private static void SendTouristHome(SimDescription desc)
        {
            if (desc.AssignedRole != null)
            {
                desc.AssignedRole.RemoveSimFromRole();
            }
            desc.StartPackUpToMiniSimDescriptionTask();
        }

 

        public Sim.WalkStyle SimWalkStyle;

        public Callback SuccessCallbackForSequence;

        public Callback FailureCallbackForSequence;

        public static readonly ISoloInteractionDefinition Singleton = new Definition();

        private static readonly ulong kIconNameHash = ResourceUtils.HashString64("hud_iq_action_gohome_r2");



        public sealed class Definition : SoloSimInteractionDefinition<DGSGoToVirtualHome>, IAllowedOnClosedVenues
        {
            public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                SimDescription simDescription = a.SimDescription;
                if (simDescription.HasActiveRole && simDescription.AssignedRole.Type == Role.RoleType.Bouncer)
                {
                    return false;
                }
                if (simDescription.IsBonehilda)
                {
                    return false;
                }
                OccultGenie occultGenie = a.OccultManager.GetOccultType(OccultTypes.Genie) as OccultGenie;
                OccultImaginaryFriend occultImaginaryFriend;
                return (occultGenie == null || !occultGenie.IsTiedToLamp) && (!OccultImaginaryFriend.TryGetOccultFromSim(a, out occultImaginaryFriend) || occultImaginaryFriend.IsReal) && (!simDescription.IsUnicorn || !OccultUnicorn.IsNPCPoolUnicorn(a));
            }

            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                return Localization.LocalizeString(actor.IsFemale, "Gameplay/Core/Lot/GoHome:InteractionName", new object[0]);
            }

            public Definition()
            {
            }
        }

        public sealed class DGSGoToVirtualHomeInternal : Interaction<Sim, Lot>
        {
            public bool SocializationAllowed(Sim simA, Sim simB)
            {
                return SocialInteraction.SocializationAllowedCommonTest(simA, simB);
            }

            public string GreyedOutTooltipText(Sim simA, Sim simB)
            {
                string entryKey = "Gameplay/Socializing/SocialInteractionA:CannotSocializeWhileGoingHome";
                return Localization.LocalizeString(simB.IsFemale, entryKey, new object[]
			{
				simB.SimDescription
			});
            }

            public override ThumbnailKey GetIconKey()
            {
                return new ThumbnailKey(new ResourceKey(Sim.GoToVirtualHome.GoToVirtualHomeInternal.kIconNameHash, 796721156u, 0u), ThumbnailSize.Medium);
            }

            public override bool Run()
            {
                bool flag = this.Actor.LotCurrent == this.Target;
                SimDescription simDescription = this.Actor.SimDescription;
                Lot virtualLotHome = simDescription.VirtualLotHome;
                if (this.SimWalkStyle != Sim.WalkStyle.AutoSelect)
                {
                    this.Actor.RequestWalkStyle(this.SimWalkStyle);
                }
                if (!flag)
                {
                    if (virtualLotHome != null)
                    {
                        BuildableShell[] buildableShells = this.Target.BuildableShells;
                        if (buildableShells.Length > 0)
                        {
                            if (!buildableShells[0].RouteInsideShell(this.Actor))
                            {
                                if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                                {
                                    return true;
                                }
                                this.Actor.FadeOut(true, true);
                                Simulator.Sleep(uint.MaxValue);
                                return true;
                            }
                        }
                        else if (!this.Actor.RouteToLot(virtualLotHome.LotId))
                        {
                            if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                            {
                                return true;
                            }
                            this.Actor.FadeOut(true, true);
                            Simulator.Sleep(uint.MaxValue);
                            return true;
                        }
                    }
                    else
                    {
                        Route route = this.Actor.CreateRoute();
                        this.Target.PlanToLotEx(route);
                        if (route.PlanResult.Succeeded())
                        {
                            this.Actor.DoRoute(route);
                        }
                    }
                }
                if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                {
                    return true;
                }
                if (simDescription.Household != null && simDescription.Household.IsTouristHousehold)
                {
                    Sim.GoToVirtualHome.SendTouristHome(simDescription);
                }
                else if (simDescription.Household != null && simDescription.Household.IsAlienHousehold)
                {
                    (Sims3.UI.Responder.Instance.HudModel as HudModel).OnSimCurrentWorldChanged(false, simDescription);
                    if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                    {
                        return true;
                    }
                    this.Actor.FadeOut(true, true);
                }
                else if (virtualLotHome != null)
                {
                    if (!flag && RandomUtil.RandomChance01(this.kAutonomuousReactionChanceOnGoingToVirtualHome))
                    {
                        simDescription.ShowSocialsOnSim = true;
                        simDescription.MotivesDontDecay = false;
                        InteractionInstance interactionInstance = this.Actor.Autonomy.FindBestActionForCommodityOnLot(CommodityKind.None, virtualLotHome, AutonomySearchType.Generic);
                        if (interactionInstance == null)
                        {
                            this.Actor.Motives.SetMotivesToTimedDefaultsAndRemoveBuffs(false);
                            interactionInstance = this.Actor.Autonomy.FindBestActionForCommodityOnLot(CommodityKind.None, virtualLotHome, AutonomySearchType.Generic);
                        }
                        if (interactionInstance != null)
                        {
                            interactionInstance = interactionInstance.InteractionDefinition.CreateInstanceWithCallbacks(interactionInstance.Target, this.Actor, interactionInstance.GetPriority(), interactionInstance.Autonomous, interactionInstance.CancellableByPlayer, null, new Callback(DGSGoToVirtualHome.DGSGoToVirtualHomeInternal.OnAutonomousDone), new Callback(DGSGoToVirtualHome.DGSGoToVirtualHomeInternal.OnAutonomousDone));
                            this.Actor.InteractionQueue.AddNext(interactionInstance);
                            return true;
                        }
                    }
                    Door associatedNPCDoor = virtualLotHome.GetAssociatedNPCDoor(this.Actor.SimDescription);
                    Slot slot;
                    if (associatedNPCDoor != null && this.Actor.RouteToSlotListAndCheckInUse(associatedNPCDoor, new Slot[]
				{
					Door.RoutingSlots.Upgrade_Front,
					Door.RoutingSlots.Upgrade_Rear
				}, out slot))
                    {
                        Audio.StartObjectSound(associatedNPCDoor.ObjectId, "shell_door_apt_open", false);
                        if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                        {
                            return true;
                        }
                        this.Actor.FadeOut(true, true);
                        Simulator.Sleep(uint.MaxValue);
                        return true;
                    }
                    int num = int.MaxValue;
                    Route route2 = null;
                    foreach (ElevatorDoors elevatorDoors in virtualLotHome.GetObjects<ElevatorDoors>())
                    {
                        int num2 = Math.Abs(this.Actor.Level - elevatorDoors.Level);
                        if (num2 < num)
                        {
                            Route route3 = this.Actor.CreateRoute();
                            if (route3.PlanToSlot(elevatorDoors, Slot.RoutingSlot_0).Succeeded())
                            {
                                route2 = route3;
                                num = num2;
                            }
                        }
                    }
                    if (route2 != null)
                    {
                        route2.DoRouteFail = false;
                        this.Actor.DoRoute(route2);
                    }
                    if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                    {
                        return true;
                    }
                    this.Actor.FadeOut(true, true);
                }
                else
                {
                    if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                    {
                        return true;
                    }
                    this.Actor.FadeOut(true, true);
                }
                Simulator.Sleep(uint.MaxValue);
                return true;
            }

            private static void OnAutonomousDone(Sim sim, float f)
            {
                if (DGSCommon.IsExtKillSimNiecAndYLevel(sim))
                {
                    return;
                }
                if (sim.SimDescription.CreatedSim == null)
                {
                    return;
                }
                InteractionInstance headInteraction = sim.InteractionQueue.GetHeadInteraction();
                if (headInteraction == null)
                {
                    Sim.MakeSimGoHome(sim, false);
                    return;
                }
                if (!(headInteraction is DGSGoToVirtualHome))
                {
                    InteractionInstance interactionInstance = headInteraction.InteractionDefinition.CreateInstanceWithCallbacks(headInteraction.Target, sim, headInteraction.GetPriority(), headInteraction.Autonomous, headInteraction.CancellableByPlayer, null, new Callback(DGSGoToVirtualHome.DGSGoToVirtualHomeInternal.OnAutonomousDone), new Callback(DGSGoToVirtualHome.DGSGoToVirtualHomeInternal.OnAutonomousDone));
                    InteractionInstance linkedInteractionInstance = headInteraction.LinkedInteractionInstance;
                    if (linkedInteractionInstance != null)
                    {
                        headInteraction.LinkedInteractionInstance = null;
                        interactionInstance.LinkedInteractionInstance = linkedInteractionInstance;
                    }
                    sim.InteractionQueue.CancelAllInteractions();
                    sim.InteractionQueue.AddNext(interactionInstance);
                }
            }

            public override void Cleanup()
            {
                if (DGSCommon.IsExtKillSimNiecAndYLevel(Actor))
                {
                    return;
                }
                if (this.Actor.IsNPC && this.Actor.CurrentOutfitCategory == OutfitCategories.SkinnyDippingTowel)
                {
                    this.Actor.SwitchToPreviousOutfitWithoutSpin();
                }
                base.Cleanup();
            }


            private const string kGoToNPCHomeAudio = "shell_door_apt_open";

            [TunableComment("chance of doing something in public areas when returning to home lot.  range: 0-1")]
            public float kAutonomuousReactionChanceOnGoingToVirtualHome = 1f;

            public Sim.WalkStyle SimWalkStyle;

            public static readonly InteractionDefinition Singleton = new Definition();

            private static readonly ulong kIconNameHash = ResourceUtils.HashString64("hud_iq_action_gohome_r2");



            public sealed class Definition : InteractionDefinition<Sim, Lot, DGSGoToVirtualHomeInternal>
            {
                public override bool Test(Sim a, Lot target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override string GetInteractionName(Sim actor, Lot target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString(actor.IsFemale, "Gameplay/Core/Lot/GoHome:InteractionName", new object[0]);
                }

                public Definition()
                {
                }
            }
        }
    }
}