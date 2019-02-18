using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.MapTags;
using Sims3.Gameplay.ObjectComponents;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Opportunities;
using Sims3.Gameplay.RealEstate;
using Sims3.Gameplay.Rewards;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.WorldBuilderUtil;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.CustomContent;
using Sims3.SimIFace.Enums;
using Sims3.SimIFace.RouteDestinations;
using Sims3.UI;
using Sims3.UI.Resort;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sims3.Gameplay.Objects.ResortXXX
{
    public class ResortFrontDesk : GameObject, IResortFrontDesk, IBuildBuyListener, IResortScoreObject, IResortStaffedObject, IGameObject, IScriptObject, IScriptLogic, IHasScriptProxy, IObjectUI, IExportableContent, IHasWorldBuilderData
    {
        private sealed class AskToCheckInDeskProxy : Interaction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : InteractionDefinition<Sim, ResortFrontDesk, AskToCheckInDeskProxy>
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, ResortFrontDesk target, List<InteractionObjectPair> results)
                {
                    Sim simAttendingDesk = target.GetSimAttendingDesk();
                    if (simAttendingDesk != null)
                    {
                        simAttendingDesk.Posture.AddInteractions(actor, simAttendingDesk, results);
                    }
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();
        }

        private sealed class AskToCheckIn : SocialInteraction
        {
            internal class Definition : InteractionDefinition<Sim, Sim, AskToCheckIn>
            {
                public int mDays;

                public int mCost;

                public bool mIsVIP;

                public bool mIsExtend;

                public virtual int MinCost
                {
                    get
                    {
                        return mCost;
                    }
                }

                public Definition()
                {
                }

                public Definition(int days, int cost, bool isVIP, bool isExtend)
                {
                    mDays = days;
                    mCost = cost;
                    mIsVIP = isVIP;
                    mIsExtend = isExtend;
                }

                public virtual Definition CreateDefinition(int days, int cost, bool bVIP, bool isExtend)
                {
                    return new Definition(days, cost, bVIP, isExtend);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    ResortFrontDesk resortFrontDesk = target.Posture.Container as ResortFrontDesk;
                    if (resortFrontDesk == null)
                    {
                        return false;
                    }
                    if (!FrontDeskCheckInTestCommon(a, resortFrontDesk, isAutonomous, mIsExtend, mIsVIP))
                    {
                        return false;
                    }
                    if (isAutonomous != (mDays == 0))
                    {
                        return false;
                    }
                    if (isAutonomous)
                    {
                        if (a.IsSelectable)
                        {
                            return false;
                        }
                        foreach (Sim sim in a.Household.Sims)
                        {
                            if (sim != a && sim.InteractionQueue.HasInteractionOfType(this))
                            {
                                return false;
                            }
                        }
                    }
                    if (a.FamilyFunds < MinCost)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("NotEnoughFundsTooltip"));
                        return false;
                    }
                    return true;
                }

                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    if (mIsVIP)
                    {
                        if (mDays == 1)
                        {
                            return LocalizeString("InteractionNameSingularVIP", mDays, mCost);
                        }
                        return LocalizeString("InteractionNameVIP", mDays, mCost);
                    }
                    if (mDays == 1)
                    {
                        return LocalizeString("InteractionNameSingular", mDays, mCost);
                    }
                    return LocalizeString("InteractionName", mDays, mCost);
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, Sim target, List<InteractionObjectPair> results)
                {
                    ResortFrontDesk resortFrontDesk = target.Posture.Container as ResortFrontDesk;
                    if (resortFrontDesk != null)
                    {
                        ResortManager resortManager = resortFrontDesk.LotCurrent.ResortManager;
                        if (resortManager != null)
                        {
                            resortManager.RefreshResortData();
                            float num = resortManager.GetResortData().PricePerRoom;
                            if ((int)num == 0)
                            {
                                num = 1f;
                            }
                            if (actor.IsSelectable && actor.Household.RealEstateManager.FindVenueProperty(target.LotCurrent) != null)
                            {
                                num = 0f;
                            }
                            Lot lotCurrent = resortFrontDesk.LotCurrent;
                            bool isExtend = resortManager.IsCheckedIn(actor);
                            for (int i = 0; i < 5; i++)
                            {
                                Definition interaction = CreateDefinition(i, i * (int)num, false, isExtend);
                                InteractionObjectPair item = new InteractionObjectPair(interaction, target);
                                results.Add(item);
                            }
                            ResortManager resortManager2 = target.LotCurrent.ResortManager;
                            if (resortManager != null && resortManager.VIPRooms.Count > 0)
                            {
                                num *= kVIPPriceMultiplier;
                                for (int j = 1; j < 5; j++)
                                {
                                    Definition interaction2 = CreateDefinition(j, j * (int)num, true, isExtend);
                                    InteractionObjectPair item2 = new InteractionObjectPair(interaction2, target);
                                    results.Add(item2);
                                }
                            }
                        }
                    }
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[1]
				{
					LocalizeString(mIsExtend ? "PathExtend" : "Path")
				};
                }
            }

            internal sealed class WithDefinition : Definition
            {
                public override int MinCost
                {
                    get
                    {
                        return mCost * 2;
                    }
                }

                public WithDefinition()
                {
                }

                public WithDefinition(int days, int cost, bool isVIP)
                    : base(days, cost, isVIP, false)
                {
                }

                public override Definition CreateDefinition(int days, int cost, bool isVIP, bool isExtend)
                {
                    return new WithDefinition(days, cost, isVIP);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (GetValidSims(a, target).Count == 0)
                    {
                        return false;
                    }
                    return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
                }

                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    if (mIsVIP)
                    {
                        if (mDays == 1)
                        {
                            return LocalizeString("WithInteractionNameVIP", mDays, mCost);
                        }
                        return LocalizeString("WithInteractionNameVIP", mDays, mCost);
                    }
                    if (mDays == 1)
                    {
                        return LocalizeString("WithInteractionNameSingular", mDays, mCost);
                    }
                    return LocalizeString("WithInteractionName", mDays, mCost);
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (mDays != 0)
                    {
                        return new string[1]
					{
						LocalizeString("WithPath")
					};
                    }
                    return base.GetPath(isFemale);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    Sim sim = parameters.Actor as Sim;
                    if (sim.FamilyFunds < mCost * 2)
                    {
                        NumSelectableRows = 0;
                    }
                    else if (mCost == 0)
                    {
                        NumSelectableRows = kMaxNumSimToCheckinWith;
                    }
                    else
                    {
                        NumSelectableRows = sim.FamilyFunds / mCost;
                        NumSelectableRows = Math.Min(kMaxNumSimToCheckinWith, NumSelectableRows);
                    }
                    PopulateSimPicker(ref parameters, out listObjs, out headers, GetValidSims(sim, parameters.Target as Sim), false);
                }

                private List<Sim> GetValidSims(Sim actor, Sim target)
                {
                    ResortFrontDesk resortFrontDesk = target.Posture.Container as ResortFrontDesk;
                    List<Sim> list = new List<Sim>();
                    if (resortFrontDesk != null)
                    {
                        Lot lotCurrent = resortFrontDesk.LotCurrent;
                        {
                            foreach (Sim sim in actor.Household.Sims)
                            {
                                if (FrontDeskCheckInTestCommon(sim, resortFrontDesk, false, mIsExtend, mIsVIP) && sim != actor)
                                {
                                    list.Add(sim);
                                }
                            }
                            return list;
                        }
                    }
                    return list;
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn";

            private bool mHasWaitedInLine;

            public static readonly InteractionDefinition Singleton = new Definition();

            public static readonly InteractionDefinition WithSingleton = new WithDefinition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn:" + name, parameters);
            }

            private static string LocalizeString(bool isFemale, string name, params object[] parameters)
            {
                return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn:" + name, parameters);
            }

            public override bool Run()
            {
                Definition definition = base.InteractionDefinition as Definition;
                bool mIsExtend = definition.mIsExtend;
                ResortManager resortManager = Target.LotCurrent.ResortManager;
                if (base.Autonomous && RandomUtil.RandomChance01(kVIPChance) && resortManager.VIPRooms.Count > 0)
                {
                    foreach (ResortManager.VIPRoomData vIPRoom in resortManager.VIPRooms)
                    {
                        if (vIPRoom.SimDescriptionId == 0)
                        {
                            definition.mIsVIP = true;
                            break;
                        }
                    }
                }
                bool isSelectable = Actor.IsSelectable;
                List<Sim> list;
                if (!mIsExtend)
                {
                    if (isSelectable)
                    {
                        list = ((base.SelectedObjects != null) ? GetSelectedObjectsAsSims() : new List<Sim>());
                        list.Add(Actor);
                    }
                    else
                    {
                        list = new List<Sim>(Actor.Household.Sims);
                    }
                }
                else
                {
                    list = new List<Sim>();
                    list.Add(Actor);
                }
                AttendingResortFrontDesk attendingResortFrontDesk = Target.Posture as AttendingResortFrontDesk;
                if (attendingResortFrontDesk == null)
                {
                    return false;
                }
                ResortFrontDesk resortFrontDesk = attendingResortFrontDesk.Container as ResortFrontDesk;
                mHasWaitedInLine = true;
                if (!resortFrontDesk.Line.WaitForTurn(this, resortFrontDesk, SimQueue.WaitBehavior.CutAheadOfLowerPrioritySims, ExitReason.Default, kTimeToWaitInLine))
                {
                    return false;
                }
                if (!Actor.RouteToSlot(resortFrontDesk, Slot.RoutingSlot_1))
                {
                    return false;
                }
                if (!SafeToSync())
                {
                    Actor.RouteAway(2f, 4f, false, GetPriority(), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                    return false;
                }
                int num = definition.mCost * list.Count;
                if (!BeginSocialInteraction(CheckIn.Singleton, false, 1.32f, false))
                {
                    if (isSelectable && Actor.Household.FamilyFunds < num)
                    {
                        Actor.ShowTNSAndPlayStingIfSelectable(LocalizeString(Target.IsFemale, "NotEnoughFundsTNS", Actor, Target), StyledNotification.NotificationStyle.kSimTalking, Target.SimDescription, Actor.SimDescription, string.Empty);
                    }
                    Actor.RouteAway(2f, 4f, false, GetPriority(), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                    return false;
                }
                int num2 = definition.mDays;
                if (num2 == 0)
                {
                    num2 = RandomUtil.GetInt(3) + 1;
                }
                ulong num3 = 0uL;
                if (!mIsExtend)
                {
                    if (!definition.mIsVIP)
                    {
                        ObjectGuid assignedTower = ObjectGuid.InvalidObjectGuid;
                        Lot lotCurrent = Target.LotCurrent;
                        if (base.Autonomous)
                        {
                            IResortTower[] objects = lotCurrent.GetObjects<IResortTower>();
                            if (objects.Length > 0)
                            {
                                IResortTower randomObjectFromList = RandomUtil.GetRandomObjectFromList(objects);
                                assignedTower = randomObjectFromList.ObjectId;
                            }
                        }
                        foreach (Sim item in list)
                        {
                            lotCurrent.ResortManager.CheckInSim(item, num2, assignedTower, list);
                        }
                    }
                    else if (resortManager != null)
                    {
                        num3 = resortManager.GetOneAvailableVIPRoom();
                        if (num3 != 0)
                        {
                            foreach (Sim item2 in list)
                            {
                                resortManager.CheckInSim(item2, num2, num3, list);
                            }
                            resortManager.UpdateVIPCheckIn(Actor, num3, list);
                        }
                        else
                        {
                            bool flag = false;
                            if (isSelectable && resortManager.VIPRooms.Count > 0)
                            {
                                num3 = 0uL;
                                foreach (ResortManager.VIPRoomData vIPRoom2 in resortManager.VIPRooms)
                                {
                                    SimDescription simDescription = SimDescription.Find(vIPRoom2.SimDescriptionId);
                                    Sim sim = null;
                                    if (simDescription != null)
                                    {
                                        sim = simDescription.CreatedSim;
                                    }
                                    if (sim != null && sim.IsNPC)
                                    {
                                        num3 = vIPRoom2.GroupId;
                                        break;
                                    }
                                }
                                if (num3 != 0)
                                {
                                    resortManager.EvictVIP(num3);
                                    foreach (Sim item3 in list)
                                    {
                                        resortManager.CheckInSim(item3, num2, num3, list);
                                    }
                                    resortManager.UpdateVIPCheckIn(Actor, num3, list);
                                    flag = true;
                                }
                            }
                            if (!flag)
                            {
                                Actor.RouteAway(2f, 4f, false, GetPriority(), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                                return false;
                            }
                        }
                    }
                }
                else if (resortManager != null)
                {
                    num *= resortManager.ExtendStay(Actor, num2);
                }
                if (isSelectable)
                {
                    Actor.Household.ModifyFamilyFunds(-num);
                }
                if (attendingResortFrontDesk.IsDoingStandingIdles)
                {
                    Actor.LoopIdle();
                    attendingResortFrontDesk.PutHandsOnBooth();
                }
                EnterStateMachine("Social_Generic", "Enter", "y", "x");
                StandardEntry();
                BeginCommodityUpdates();
                SetParameter("AnimX", "a_soc_talkTalk_gen_a_x");
                SetParameter("AnimY", "a_soc_talkListen_agreeEager_x");
                SetParameter("AnimW", "a_soc_talkTalk_gen_a_x");
                SetParameter("AnimZ", "a_soc_talkListen_agreeEager_x");
                AnimateJoinSims("social");
                Actor.LookAtManager.EnableLookAts();
                Target.LookAtManager.EnableLookAts();
                EndCommodityUpdates(true);
                FinishLinkedInteraction(true);
                StandardExit();
                WaitForSyncComplete();
                if (definition.mIsVIP)
                {
                    ResortManager.VIPRoomData vIPRoomData = resortManager.FindVIPRoomData(num3);
                    if (vIPRoomData != null)
                    {
                        CommonDoor commonDoor = vIPRoomData.DoorIds[0].ObjectFromId<CommonDoor>();
                        if (commonDoor != null)
                        {
                            InteractionInstance entry = Sim.RouteThroughDoor.Singleton.CreateInstance(commonDoor, Actor, new InteractionPriority(InteractionPriorityLevel.UserDirected), false, false);
                            Actor.InteractionQueue.AddNext(entry);
                        }
                    }
                }
                else
                {
                    Actor.RouteAway(2f, 4f, false, GetPriority(), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                }
                DateAndTime checkOutDateAndTime = resortManager.GetCheckOutDateAndTime(Actor);
                DaysOfTheWeek dayOfWeek = checkOutDateAndTime.DayOfWeek;
                long ticks = checkOutDateAndTime.Ticks - SimClock.CurrentTicks;
                int num4 = (int)SimClock.ConvertFromTicks(ticks, TimeUnit.Days) + 1;
                string empty = string.Empty;
                if (!mIsExtend)
                {
                    string entryKey = (num4 != 1) ? "Gameplay/Excel/Notifications/Notifications:ResortCheckInTNS" : "Gameplay/Excel/Notifications/Notifications:ResortCheckInSingularTNS";
                    Actor.ShowTNSIfSelectable(Localization.LocalizeString(entryKey, resortFrontDesk.LotCurrent.Name, num4, SimClockUtils.GetDayAsText(dayOfWeek)), StyledNotification.NotificationStyle.kSimTalking, Target.ObjectId, Actor.ObjectId);
                }
                else
                {
                    string entryKey = (num2 != 1) ? "Gameplay/Excel/Notifications/Notifications:ResortExtendStayTNS" : "Gameplay/Excel/Notifications/Notifications:ResortExtendStaySingularTNS";
                    Actor.ShowTNSIfSelectable(Localization.LocalizeString(entryKey, num2, SimClockUtils.GetDayAsText(dayOfWeek)), StyledNotification.NotificationStyle.kSimTalking, Target.ObjectId, Actor.ObjectId);
                }
                return true;
            }

            public override void Cleanup()
            {
                if (mHasWaitedInLine && Target != null)
                {
                    AttendingResortFrontDesk attendingResortFrontDesk = Target.Posture as AttendingResortFrontDesk;
                    if (attendingResortFrontDesk != null)
                    {
                        ResortFrontDesk resortFrontDesk = attendingResortFrontDesk.Container as ResortFrontDesk;
                        if (resortFrontDesk != null && resortFrontDesk.Line != null)
                        {
                            resortFrontDesk.Line.RemoveFromQueue(Actor);
                        }
                    }
                }
                base.Cleanup();
            }
        }

        private sealed class AskToCheckInOffLotDisabledInteraction : Interaction<Sim, ResortFrontDesk>
        {
            internal class Definition : InteractionDefinition<Sim, ResortFrontDesk, AskToCheckInOffLotDisabledInteraction>, IDontNeedToBeCheckedInResort
            {
                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (a.LotHome == null)
                    {
                        return false;
                    }
                    Lot lotCurrent = target.LotCurrent;
                    ResortManager resortManager = lotCurrent.ResortManager;
                    if (resortManager == null)
                    {
                        return false;
                    }
                    resortManager.IsCheckedIn(a);
                    if (lotCurrent.LotCurrent.ResortManager.Closed || (target.ResortStaffedObjectComponent.Shifts & ResortWorker.GetShiftForTime(SimClock.CurrentTime())) == SetHoursDialog.Shifts.None)
                    {
                        if (!lotCurrent.IsControllable || lotCurrent.LotCurrent.ResortManager.GetOneAvailableVIPRoom() != 0)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Gameplay/Objects/Miscellaneous/ResortFrontDesk:NobodyManning"));
                        }
                        return false;
                    }
                    return false;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    Lot lotCurrent = target.LotCurrent;
                    ResortManager resortManager = lotCurrent.ResortManager;
                    if (resortManager == null)
                    {
                        return LocalizeString("Path");
                    }
                    return LocalizeString(resortManager.IsCheckedIn(actor) ? "PathExtend" : "Path");
                }
            }

            internal sealed class WithDefinition : Definition
            {
                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (GetValidSims(a, target).Count == 0)
                    {
                        return false;
                    }
                    return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("WithPath");
                }

                private List<Sim> GetValidSims(Sim actor, ResortFrontDesk target)
                {
                    List<Sim> list = new List<Sim>();
                    if (target != null)
                    {
                        Lot lotCurrent = target.LotCurrent;
                        {
                            foreach (Sim sim in actor.Household.Sims)
                            {
                                if (!lotCurrent.IsCheckedInResort(sim) && sim != actor)
                                {
                                    list.Add(sim);
                                }
                            }
                            return list;
                        }
                    }
                    return list;
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn";

            public static readonly Definition Singleton = new Definition();

            public static readonly WithDefinition WithSingleton = new WithDefinition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn:" + name, parameters);
            }

            private static string LocalizeString(bool isFemale, string name, params object[] parameters)
            {
                return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn:" + name, parameters);
            }

            public override bool Run()
            {
                return false;
            }
        }

        private sealed class AskToCheckInOffLot : Interaction<Sim, ResortFrontDesk>
        {
            internal class Definition : InteractionDefinition<Sim, ResortFrontDesk, AskToCheckInOffLot>, IDontNeedToBeCheckedInResort
            {
                public int mDays;

                public int mCost;

                public bool mIsVIP;

                public bool mIsExtend;

                public bool mIgnoreResortWorker;

                public virtual int MinCost
                {
                    get
                    {
                        return mCost;
                    }
                }

                public Definition()
                {
                }

                public Definition(int days, int cost, bool isVIP, bool isExtend, bool ignoreResortWorker)
                {
                    mDays = days;
                    mCost = cost;
                    mIsVIP = isVIP;
                    mIsExtend = isExtend;
                    mIgnoreResortWorker = ignoreResortWorker;
                }

                public virtual Definition CreateDefinition(int days, int cost, bool bVIP, bool isExtend, bool ignoreResortWorker)
                {
                    return new Definition(days, cost, bVIP, isExtend, ignoreResortWorker);
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous)
                    {
                        return false;
                    }
                    if (!mIgnoreResortWorker)
                    {
                        Sim simAttendingDesk = target.GetSimAttendingDesk();
                        if (simAttendingDesk != null && simAttendingDesk.Posture is AttendingResortFrontDesk)
                        {
                            return false;
                        }
                    }
                    if (!FrontDeskCheckInTestCommon(a, target, false, mIsExtend, mIsVIP))
                    {
                        return false;
                    }
                    if (a.FamilyFunds < MinCost)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("NotEnoughFundsTooltip"));
                        return false;
                    }
                    return true;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    if (mIsVIP)
                    {
                        if (mDays == 1)
                        {
                            return LocalizeString("InteractionNameSingularVIP", mDays, mCost);
                        }
                        return LocalizeString("InteractionNameVIP", mDays, mCost);
                    }
                    if (mDays == 1)
                    {
                        return LocalizeString("InteractionNameSingular", mDays, mCost);
                    }
                    return LocalizeString("InteractionName", mDays, mCost);
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, ResortFrontDesk target, List<InteractionObjectPair> results)
                {
                    AddInteractionsHelper(iop, actor, target, false, results);
                }

                public void AddInteractionsHelper(InteractionObjectPair iop, Sim actor, ResortFrontDesk target, bool ignoreResortWorker, List<InteractionObjectPair> results)
                {
                    ResortManager resortManager = target.LotCurrent.ResortManager;
                    if (resortManager != null)
                    {
                        resortManager.RefreshResortData();
                        float num = resortManager.GetResortData().PricePerRoom;
                        if ((int)num == 0)
                        {
                            num = 1f;
                        }
                        if (actor.IsSelectable && actor.Household.RealEstateManager.FindVenueProperty(target.LotCurrent) != null)
                        {
                            num = 0f;
                        }
                        Lot lotCurrent = target.LotCurrent;
                        bool isExtend = resortManager.IsCheckedIn(actor);
                        for (int i = 1; i < 5; i++)
                        {
                            Definition interaction = CreateDefinition(i, i * (int)num, false, isExtend, ignoreResortWorker);
                            InteractionObjectPair item = new InteractionObjectPair(interaction, target);
                            results.Add(item);
                        }
                        ResortManager resortManager2 = target.LotCurrent.ResortManager;
                        if (resortManager != null && resortManager.VIPRooms.Count > 0)
                        {
                            num *= kVIPPriceMultiplier;
                            for (int j = 1; j < 5; j++)
                            {
                                Definition interaction2 = CreateDefinition(j, j * (int)num, true, isExtend, ignoreResortWorker);
                                InteractionObjectPair item2 = new InteractionObjectPair(interaction2, target);
                                results.Add(item2);
                            }
                        }
                    }
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[1]
				{
					LocalizeString(mIsExtend ? "PathExtend" : "Path")
				};
                }
            }

            internal sealed class WithDefinition : Definition
            {
                public override int MinCost
                {
                    get
                    {
                        return mCost * 2;
                    }
                }

                public WithDefinition()
                {
                }

                private WithDefinition(int days, int cost, bool isVIP, bool ignoreResortWorker)
                    : base(days, cost, isVIP, false, ignoreResortWorker)
                {
                }

                public override Definition CreateDefinition(int days, int cost, bool isVIP, bool isExtend, bool ignoreResortWorker)
                {
                    return new WithDefinition(days, cost, isVIP, ignoreResortWorker);
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (GetValidSims(a, target).Count == 0)
                    {
                        return false;
                    }
                    return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    if (mIsVIP)
                    {
                        if (mDays == 1)
                        {
                            return LocalizeString("WithInteractionNameVIP", mDays, mCost);
                        }
                        return LocalizeString("WithInteractionNameVIP", mDays, mCost);
                    }
                    if (mDays == 1)
                    {
                        return LocalizeString("WithInteractionNameSingular", mDays, mCost);
                    }
                    return LocalizeString("WithInteractionName", mDays, mCost);
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (mDays != 0)
                    {
                        return new string[1]
					{
						LocalizeString("WithPath")
					};
                    }
                    return base.GetPath(isFemale);
                }

                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    Sim sim = parameters.Actor as Sim;
                    if (sim.FamilyFunds < mCost * 2)
                    {
                        NumSelectableRows = 0;
                    }
                    else if (mCost == 0)
                    {
                        NumSelectableRows = kMaxNumSimToCheckinWith;
                    }
                    else
                    {
                        NumSelectableRows = sim.FamilyFunds / mCost;
                        NumSelectableRows = Math.Min(kMaxNumSimToCheckinWith, NumSelectableRows);
                    }
                    PopulateSimPicker(ref parameters, out listObjs, out headers, GetValidSims(sim, parameters.Target as ResortFrontDesk), false);
                }

                private List<Sim> GetValidSims(Sim actor, ResortFrontDesk target)
                {
                    List<Sim> list = new List<Sim>();
                    if (target != null)
                    {
                        Lot lotCurrent = target.LotCurrent;
                        {
                            foreach (Sim sim in actor.Household.Sims)
                            {
                                if (FrontDeskCheckInTestCommon(sim, target, false, mIsExtend, mIsVIP) && sim != actor)
                                {
                                    list.Add(sim);
                                }
                            }
                            return list;
                        }
                    }
                    return list;
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn";

            [TunableComment("Range: Positive integers.  Duration in minutes to wait for clerk to show up for checkin")]
            [Tunable]
            private static float kTimeoutDuration = 50f;

            public static readonly Definition Singleton = new Definition();

            public static readonly WithDefinition WithSingleton = new WithDefinition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn:" + name, parameters);
            }

            private static string LocalizeString(bool isFemale, string name, params object[] parameters)
            {
                return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/AskToCheckIn:" + name, parameters);
            }

            public override bool Run()
            {
                Route r = null;
                Lot lotCurrent = Target.LotCurrent;
                Sims3.Gameplay.Abstracts.Door door = lotCurrent.FindFrontDoor();
                if (door != null)
                {
                    bool wantToBeOutside = Target.RoomId == 0;
                    if (!lotCurrent.RouteToFrontDoor(Actor, Sim.MinDistanceFromDoorWhenVisiting, Sim.MaxDistanceFromDoorWhenGoingInside, ref door, wantToBeOutside, ref r, false))
                    {
                        r = null;
                    }
                }
                if (r == null)
                {
                    Actor.RemoveExitReason(ExitReason.RouteFailed);
                    r = Actor.CreateRoute();
                    r.SetRouteMetaType(Route.RouteMetaType.GoCommunityLot);
                    lotCurrent.PlanToLotEx(r);
                }
                if (Actor.LotCurrent == lotCurrent || Actor.DoRouteWithFollowers(r, GetSelectedObjectsAsSims()))
                {
                    if (!Target.RouteInFrontOfDesk(Actor))
                    {
                        Route route = Actor.CreateRoute();
                        route.PlanToPointRadialRange(Target.Position, kMinWaitingDistance, kMaxWaitingDistance);
                        route.DoRouteFail = false;
                        Actor.DoRoute(route);
                        Actor.RemoveExitReason(ExitReason.RouteFailed);
                    }
                    BeginCommodityUpdates();
                    float num = SimClock.ElapsedTime(TimeUnit.Minutes);
                    Sim sim = null;
                    while (!Actor.WaitForExitReason(1f, ExitReason.Default) && !(SimClock.ElapsedTime(TimeUnit.Minutes) - num > kTimeoutDuration))
                    {
                        sim = Target.GetSimAttendingDesk();
                        if (sim != null && sim.Posture is AttendingResortFrontDesk)
                        {
                            break;
                        }
                        sim = null;
                        UpdateLoop();
                    }
                    if (sim == null)
                    {
                        EndCommodityUpdates(false);
                        return false;
                    }
                    Definition definition = base.InteractionDefinition as Definition;
                    if (base.InteractionDefinition is WithDefinition)
                    {
                        AskToCheckIn.WithDefinition withDefinition = new AskToCheckIn.WithDefinition(definition.mDays, definition.mCost, definition.mIsVIP);
                        InteractionInstance interactionInstance = withDefinition.CreateInstance(sim, Actor, mPriority, base.Autonomous, base.CancellableByPlayer);
                        interactionInstance.SelectedObjects = base.SelectedObjects;
                        Actor.InteractionQueue.PushAsContinuation(interactionInstance, false);
                    }
                    else
                    {
                        AskToCheckIn.Definition interactionDefinition = new AskToCheckIn.Definition(definition.mDays, definition.mCost, definition.mIsVIP, definition.mIsExtend);
                        Actor.InteractionQueue.PushAsContinuation(interactionDefinition, sim, false);
                    }
                    EndCommodityUpdates(true);
                    return true;
                }
                return false;
            }
        }

        private sealed class CheckIn : SocialInteractionB
        {
            private sealed class CheckInDefinition : Definition, IAllowedWithSocializationDisabled
            {
                private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/Checkin";

                private static string LocalizeString(bool isFemale, string name, params object[] parameters)
                {
                    return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/Checkin:" + name, parameters);
                }

                public CheckInDefinition()
                    : base(null, "Check In", false)
                {
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return LocalizeString(actor.IsFemale, "InteractionName");
                }
            }

            public static readonly InteractionDefinition Singleton = new CheckInDefinition();
        }

        public abstract class BaseFrontDeskSocialA : SocialInteraction
        {
            public class Definition<TInteraction> : InteractionDefinition<Sim, Sim, TInteraction> where TInteraction : BaseFrontDeskSocialA, new()
            {
                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    ResortFrontDesk resortFrontDesk = target.Posture.Container as ResortFrontDesk;
                    if (resortFrontDesk == null)
                    {
                        return false;
                    }
                    if (a == target)
                    {
                        return false;
                    }
                    if (!resortFrontDesk.LotCurrent.IsCheckedInResort(a))
                    {
                        return false;
                    }
                    return true;
                }
            }

            private bool mHasWaitedInLine;

            public abstract string SocialBInteractionKey
            {
                get;
            }

            public override bool Run()
            {
                AttendingResortFrontDesk attendingResortFrontDesk = Target.Posture as AttendingResortFrontDesk;
                if (attendingResortFrontDesk == null)
                {
                    return false;
                }
                ResortFrontDesk resortFrontDesk = attendingResortFrontDesk.Container as ResortFrontDesk;
                mHasWaitedInLine = true;
                if (!resortFrontDesk.Line.WaitForTurn(this, resortFrontDesk, SimQueue.WaitBehavior.CutAheadOfLowerPrioritySims, ExitReason.Default, kTimeToWaitInLine))
                {
                    return false;
                }
                if (!Actor.RouteToSlot(resortFrontDesk, Slot.RoutingSlot_1))
                {
                    return false;
                }
                if (!SafeToSync())
                {
                    return false;
                }
                if (!BeginSocialInteraction(new ResortFrontDeskSocialB.ResortFrontDeskSocialBDefinition(SocialBInteractionKey), false, 1.32f, false))
                {
                    return false;
                }
                if (attendingResortFrontDesk.IsDoingStandingIdles)
                {
                    Actor.LoopIdle();
                    attendingResortFrontDesk.PutHandsOnBooth();
                }
                EnterStateMachine("Social_Generic", "Enter", "y", "x");
                StandardEntry();
                BeginCommodityUpdates();
                SetParameter("AnimX", "a_soc_talkTalk_gen_a_x");
                SetParameter("AnimY", "a_soc_talkListen_agreeEager_x");
                SetParameter("AnimW", "a_soc_talkTalk_gen_a_x");
                SetParameter("AnimZ", "a_soc_talkListen_agreeEager_x");
                AnimateJoinSims("social");
                OnPostAnimate();
                Actor.LookAtManager.EnableLookAts();
                Target.LookAtManager.EnableLookAts();
                EndCommodityUpdates(true);
                FinishLinkedInteraction(true);
                StandardExit();
                WaitForSyncComplete();
                Actor.RouteAway(2f, 4f, false, GetPriority(), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                return true;
            }

            public override void Cleanup()
            {
                if (mHasWaitedInLine && Target != null)
                {
                    AttendingResortFrontDesk attendingResortFrontDesk = Target.Posture as AttendingResortFrontDesk;
                    if (attendingResortFrontDesk != null)
                    {
                        ResortFrontDesk resortFrontDesk = attendingResortFrontDesk.Container as ResortFrontDesk;
                        if (resortFrontDesk != null && resortFrontDesk.Line != null)
                        {
                            resortFrontDesk.Line.RemoveFromQueue(Actor);
                        }
                    }
                }
                base.Cleanup();
            }

            public virtual void OnPostAnimate()
            {
            }
        }

        public class ResortFrontDeskSocialB : SocialInteractionB
        {
            public sealed class ResortFrontDeskSocialBDefinition : Definition, IAllowedWithSocializationDisabled
            {
                public ResortFrontDeskSocialBDefinition(string interactionName)
                    : base(null, interactionName, false)
                {
                }

                public ResortFrontDeskSocialBDefinition()
                    : base(null, "", false)
                {
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
                {
                    return LocalizeString(actor.IsFemale, Name);
                }
            }
        }

        public class AskAboutAdventure : BaseFrontDeskSocialA
        {
            public class AskAboutAdventureDefinition : Definition<AskAboutAdventure>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    return LocalizeString(actor.IsFemale, "AskAboutAdventure");
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!UnchartedIslandMarker.DoesMarkerOfTypeExist(UnchartedIslandMarker.DiscoveryType.LocalsRefuge))
                    {
                        return false;
                    }
                    if (a.OpportunityManager.HasOpportunity(OpportunityNames.EP10_HiddenIsland_Op1) || a.OpportunityManager.HasOpportunity(OpportunityNames.EP10_HiddenIsland_Op2) || a.OpportunityManager.HasOpportunity(OpportunityNames.EP10_HiddenIsland_Op3) || a.OpportunityManager.HasOpportunity(OpportunityNames.EP10_HiddenIsland_Op4) || a.OpportunityManager.HasOpportunity(OpportunityNames.EP10_HiddenIsland_Op5))
                    {
                        return false;
                    }
                    if (a.OpportunityManager.HasCompletedOpportunity(OpportunityNames.EP10_HiddenIsland_Op5))
                    {
                        return false;
                    }
                    return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
                }
            }

            public static readonly InteractionDefinition Singleton = new AskAboutAdventureDefinition();

            public override string SocialBInteractionKey
            {
                get
                {
                    return "BeAskedAboutAdventure";
                }
            }

            public override void OnPostAnimate()
            {
                if (Actor.IsSelectable && !base.Autonomous)
                {
                    Actor.OpportunityManager.ClearLastOpportunity(OpportunityCategory.Special);
                    Actor.OpportunityManager.AddOpportunityNow(OpportunityNames.EP10_HiddenIsland_Op1, true, false);
                }
            }
        }

        public class AskToCheckOut : BaseFrontDeskSocialA
        {
            [DoesntRequireTuning]
            public class AskToCheckOutDefinition : Definition<AskToCheckOut>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    return LocalizeString(actor.IsFemale, "AskToCheckOut");
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous)
                    {
                        return false;
                    }
                    ResortManager resortManager = target.LotCurrent.ResortManager;
                    if (resortManager != null && resortManager.IsCheckedIn(a))
                    {
                        return base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback);
                    }
                    return false;
                }
            }

            public static readonly InteractionDefinition Singleton = new AskToCheckOutDefinition();

            public override string SocialBInteractionKey
            {
                get
                {
                    return "BeAskedToCheckOut";
                }
            }

            public override void OnPostAnimate()
            {
                if (Actor.IsSelectable && !base.Autonomous)
                {
                    ResortManager resortManager = Target.LotCurrent.ResortManager;
                    resortManager.CheckOutSim(Actor);
                }
            }
        }

        public class AskAboutDivingSpots : BaseFrontDeskSocialA
        {
            public class AskAboutDivingSpotsDefinition : Definition<AskAboutDivingSpots>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    return LocalizeString(actor.IsFemale, "AskAboutDivingSpots");
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }
                    if (!LotManager.sWorldHasDiveLots)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString(a.IsFemale, "NoDiveLots"));
                        return false;
                    }
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new AskAboutDivingSpotsDefinition();

            public override string SocialBInteractionKey
            {
                get
                {
                    return "BeAskedAboutDivingSpots";
                }
            }

            public override void OnPostAnimate()
            {
                if (Actor.IsSelectable && !base.Autonomous)
                {
                    ScubaDivingSkill skill = Actor.SkillManager.GetSkill<ScubaDivingSkill>(SkillNames.ScubaDiving);
                    int num = (skill != null) ? skill.SkillLevel : 0;
                    Lot lot = null;
                    Lot lot2 = null;
                    ulong simDescriptionId = Actor.SimDescription.SimDescriptionId;
                    DivingLotMapTag.DiveLotMapTagData value;
                    if (Household.sHighlightedDiveLots != null && Household.sHighlightedDiveLots.TryGetValue(simDescriptionId, out value))
                    {
                        lot = LotManager.GetLot(value.LotID);
                        if (lot == null)
                        {
                            Household.sHighlightedDiveLots.Remove(simDescriptionId);
                        }
                    }
                    foreach (Lot commercialLot in LotManager.GetCommercialLots(CommercialLotSubType.kEP10_Diving))
                    {
                        if (lot != commercialLot && commercialLot.RequiredDivingLevel <= num && (lot2 == null || lot2.RequiredDivingLevel < commercialLot.RequiredDivingLevel))
                        {
                            lot2 = commercialLot;
                        }
                    }
                    if (lot != null)
                    {
                        if (lot2 != null && lot.RequiredDivingLevel < lot2.RequiredDivingLevel)
                        {
                            Household.AddHighlightedDivingSpot(simDescriptionId, lot2.LotId);
                        }
                    }
                    else if (lot2 != null)
                    {
                        Household.AddHighlightedDivingSpot(simDescriptionId, lot2.LotId);
                    }
                    Actor.ShowTNSIfSelectable(TNSNames.AskAboutDiveLotsTNS, Target, Actor);
                }
            }
        }

        public class PurchaseMosquitoSpray : BaseFrontDeskSocialA
        {
            public class PurchaseMosquitoSprayDefinition : Definition<PurchaseMosquitoSpray>
            {
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    return LocalizeString(actor.IsFemale, "PurchaseMosquitoSpray", kMosquitoSprayCost);
                }

                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (isAutonomous)
                    {
                        return false;
                    }
                    if (!base.Test(a, target, isAutonomous, ref greyedOutTooltipCallback))
                    {
                        return false;
                    }
                    if (a.FamilyFunds < kMosquitoSprayCost)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizationHelper.InsufficientFunds);
                        return false;
                    }
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new PurchaseMosquitoSprayDefinition();

            public override string SocialBInteractionKey
            {
                get
                {
                    return "BeAskedToPurchaseMosquitoSpray";
                }
            }

            public override void OnPostAnimate()
            {
                if (Actor.FamilyFunds < kMosquitoSprayCost)
                {
                    Actor.ShowTNSIfSelectable(TNSNames.MosquitoRepellentInsufficentFunds, Target, Actor, Target, Actor);
                }
                else if (Actor.TryAddObjectToInventory("mosquitoRepellent", ProductVersion.EP10))
                {
                    Actor.ModifyFunds(-kMosquitoSprayCost);
                    Audio.StartObjectSound(Actor.ObjectId, "ui_object_buy", false);
                }
            }
        }

        private sealed class AttendToResortFrontDesk : Interaction<Sim, ResortFrontDesk>
        {
            private sealed class Definition : InteractionDefinition<Sim, ResortFrontDesk, AttendToResortFrontDesk>
            {
                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    ResortStaffedObjectComponent resortStaffedObjectComponent = target.ResortStaffedObjectComponent;
                    if (resortStaffedObjectComponent != null)
                    {
                        ResortWorker service = resortStaffedObjectComponent.GetService();
                        if (service != null)
                        {
                            return service.IsSimWorkingOnObject(a, target);
                        }
                    }
                    return false;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString(actor.IsFemale, "InteractionName");
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/AttendToResortFrontDesk";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/AttendToResortFrontDesk:" + name, parameters);
            }

            private static string LocalizeString(bool isFemale, string name, params object[] parameters)
            {
                return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/AttendToResortFrontDesk:" + name, parameters);
            }

            public override bool Run()
            {
                AttendingResortFrontDesk attendingResortFrontDesk = Actor.Posture as AttendingResortFrontDesk;
                if (attendingResortFrontDesk != null)
                {
                    bool flag = RandomUtil.RandomChance(kChanceNeutralStandIdle);
                    Target.AddToUseList(Actor);
                    if ((flag && !attendingResortFrontDesk.IsDoingStandingIdles) || (!flag && attendingResortFrontDesk.IsDoingStandingIdles))
                    {
                        attendingResortFrontDesk.CurrentStateMachine.RequestState("x", "AttendBooth");
                    }
                    return true;
                }
                int num = 3;
                bool flag2;
                do
                {
                    num--;
                    flag2 = Actor.RouteToSlot(Target, Slot.RoutingSlot_0);
                    if (!flag2)
                    {
                        Simulator.Sleep(90u);
                    }
                }
                while (!flag2 && num > 0);
                if (!flag2)
                {
                    return false;
                }
                if (Target.GetSimAttendingDesk() != Actor)
                {
                    return false;
                }
                if (ResortManager.IsResortWorker(Actor))
                {
                    Actor.SwitchToOutfitWithSpin(Sim.ClothesChangeReason.GoingToWork, OutfitCategories.Career);
                }
                Target.AddToUseList(Actor);
                EnterStateMachine("FrontDesk", "Enter", "x");
                AttendingResortFrontDesk posture = new AttendingResortFrontDesk(Actor, Target, mCurrentStateMachine);
                AnimateSim("AttendBooth");
                Actor.Posture = posture;
                return Actor.Posture.Satisfies(CommodityKind.AttendingServiceObject, Target);
            }
        }

        private sealed class ExitResortFrontDesk : Interaction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : InteractionDefinition<Sim, ResortFrontDesk, ExitResortFrontDesk>
            {
                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a.Posture.Satisfies(CommodityKind.AttendingServiceObject, target);
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString(actor.IsFemale, "InteractionName");
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/ExitFrontDesk";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(bool isFemale, string name, params object[] parameters)
            {
                return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/ExitFrontDesk:" + name, parameters);
            }

            public override bool Run()
            {
                AttendingResortFrontDesk attendingResortFrontDesk = Actor.Posture as AttendingResortFrontDesk;
                if (attendingResortFrontDesk != null)
                {
                    attendingResortFrontDesk.CurrentStateMachine.RequestState("x", "Exit");
                }
                Actor.PopPosture();
                Actor.Posture = Actor.Standing;
                Actor.RouteAway(2f, 4f, false, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), true, true, true, RouteDistancePreference.PreferFurthestFromRouteOrigin);
                Target.RemoveFromUseList(Actor);
                return Actor.Posture.Satisfies(CommodityKind.Standing, Actor);
            }
        }

        public sealed class DoItYourself : ResortStaffedObjectComponent.DoItYourselfBase<ResortFrontDesk>
        {
            private sealed class Definition : InteractionDefinition<Sim, ResortFrontDesk, DoItYourself>
            {
                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return Localization.LocalizeString(actor.IsFemale, "Gameplay/Abstracts/ScriptObject/FrontDeskDoItYourself:InteractionName");
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    Lot lotCurrent = target.LotCurrent;
                    if (!lotCurrent.IsCommunityLotOfType(CommercialLotSubType.kEP10_Resort) || a.Household.RealEstateManager.FindProperty(lotCurrent) == null)
                    {
                        return false;
                    }
                    if (isAutonomous)
                    {
                        return false;
                    }
                    ResortStaffedObjectComponent resortStaffedObjectComponent = target.ResortStaffedObjectComponent;
                    if (resortStaffedObjectComponent == null)
                    {
                        return false;
                    }
                    Sim moonlightingSim = resortStaffedObjectComponent.MoonlightingSim;
                    if (moonlightingSim != null && moonlightingSim != a)
                    {
                        return false;
                    }
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                AttendingResortFrontDesk attendingResortFrontDesk = Actor.Posture as AttendingResortFrontDesk;
                if (attendingResortFrontDesk != null)
                {
                    bool flag = RandomUtil.RandomChance(kChanceNeutralStandIdle);
                    Target.AddToUseList(Actor);
                    if ((flag && !attendingResortFrontDesk.IsDoingStandingIdles) || (!flag && attendingResortFrontDesk.IsDoingStandingIdles))
                    {
                        attendingResortFrontDesk.CurrentStateMachine.RequestState("x", "AttendBooth");
                    }
                    return true;
                }
                ResortStaffedObjectComponent resortStaffedObjectComponent = Target.ResortStaffedObjectComponent;
                if (!ClearWorkingSimAndWait())
                {
                    return false;
                }
                resortStaffedObjectComponent.BeginMoonlightingUse(Actor);
                int num = 3;
                bool flag2;
                do
                {
                    num--;
                    flag2 = Actor.RouteToSlot(Target, Slot.RoutingSlot_0);
                    if (!flag2)
                    {
                        Simulator.Sleep(90u);
                    }
                }
                while (!flag2 && num > 0);
                if (!flag2)
                {
                    resortStaffedObjectComponent.EndMoonlightingUse();
                    return false;
                }
                Target.AddToUseList(Actor);
                EnterStateMachine("FrontDesk", "Enter", "x");
                AttendingResortFrontDesk posture = new AttendingResortFrontDesk(Actor, Target, mCurrentStateMachine);
                AnimateSim("AttendBooth");
                Actor.Posture = posture;
                return Actor.Posture.Satisfies(CommodityKind.AttendingServiceObject, Target);
            }
        }

        private sealed class SetPrices : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, SetPrices>, IResortManagementInteraction
            {
                public ResortManager.Pricing mPricing = ResortManager.Pricing.Medium;

                public Definition()
                {
                }

                public Definition(ResortManager.Pricing pricing)
                {
                    mPricing = pricing;
                }

                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!target.LotCurrent.IsControllable)
                    {
                        return false;
                    }
                    if (mPricing == target.ResortManager.Price)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("CurrentPrice"));
                        return false;
                    }
                    return true;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString(mPricing.ToString());
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, ResortFrontDesk target, List<InteractionObjectPair> results)
                {
                    foreach (ResortManager.Pricing value in Enum.GetValues(typeof(ResortManager.Pricing)))
                    {
                        Definition interaction = new Definition(value);
                        InteractionObjectPair item = new InteractionObjectPair(interaction, target);
                        results.Add(item);
                    }
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (ResortExpenseDialog.sDialog != null)
                    {
                        return new string[1]
					{
						LocalizeString("Path")
					};
                    }
                    return new string[2]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement"),
					LocalizeString("Path")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/SetPrices";

            public static readonly InteractionDefinition Singleton = new Definition();

            public static readonly InteractionDefinition LowpriceSingletion = new Definition(ResortManager.Pricing.Low);

            public static readonly InteractionDefinition MediumpriceSingletion = new Definition(ResortManager.Pricing.Medium);

            public static readonly InteractionDefinition HighriceSingletion = new Definition(ResortManager.Pricing.High);

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/SetPrices:" + name, parameters);
            }

            public override bool Run()
            {
                Target.ResortManager.Price = (base.InteractionDefinition as Definition).mPricing;
                return true;
            }
        }

        private sealed class ToggleOpen : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, ToggleOpen>, IResortManagementInteraction
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!target.LotCurrent.IsControllable)
                    {
                        return false;
                    }
                    ResortManager resortManager = target.ResortManager;
                    if (resortManager.Closed)
                    {
                        if (target.LotCurrent.GetObjects<IResortTower>().Length == 0)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("NoTowerCantOpen"));
                            return false;
                        }
                        if (resortManager.IsBankrupt)
                        {
                            float profitability = resortManager.GetProfitability();
                            if (profitability < 0f)
                            {
                                greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(LocalizeString("BankruptCantOpen", profitability));
                                return false;
                            }
                        }
                    }
                    return true;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    if (target.ResortManager.Closed)
                    {
                        return LocalizeString("OpenName");
                    }
                    return LocalizeString("CloseName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (ResortExpenseDialog.sDialog != null)
                    {
                        return base.GetPath(isFemale);
                    }
                    return new string[1]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/ToggleOpen";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/ToggleOpen:" + name, parameters);
            }

            public override bool Run()
            {
                ResortManager resortManager = Target.ResortManager;
                resortManager.Closed = !resortManager.Closed;
                return true;
            }
        }

        private sealed class ManageCrew : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, ManageCrew>, IResortManagementCrewInteraction
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.LotCurrent.IsControllable;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (ResortExpenseDialog.sDialog != null)
                    {
                        return base.GetPath(isFemale);
                    }
                    return new string[1]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/ManageCrew";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/ManageCrew:" + name, parameters);
            }

            public override bool Run()
            {
                ResortManager resortManager = Target.ResortManager;
                ManageResortCrewDialog.Show(resortManager.MaintenanceCounts, ResortManager.kMaintenanceWages, ResortManager.kMaxMaintenanceWorkers);
                return true;
            }
        }

        private sealed class ViewExpenses : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, ViewExpenses>
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.LotCurrent.IsControllable;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[1]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/ViewExpenses";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/ViewExpenses:" + name, parameters);
            }

            public override bool Run()
            {
                ResortManager resortManager = Target.ResortManager;
                ResortExpenseDialog.Show(resortManager.GetResortData());
                return true;
            }
        }

        private sealed class ViewReviews : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, ViewReviews>
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (!target.LotCurrent.IsControllable)
                    {
                        return false;
                    }
                    ResortManager resortManager = target.ResortManager;
                    if (resortManager != null)
                    {
                        ResortManager.ResortData resortData = resortManager.GetResortData();
                        if (resortData.GetReviews().Count == 0)
                        {
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Ui/Caption/ResortReviewDialog:NoReviewsAvailable"));
                            return false;
                        }
                    }
                    return true;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[1]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/ViewReviews";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/ViewReviews:" + name, parameters);
            }

            public override bool Run()
            {
                ResortManager resortManager = Target.ResortManager;
                ResortReviewDialog.Show(resortManager.GetResortData());
                return true;
            }
        }

        private sealed class Build : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, Build>
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.LotCurrent.IsControllable;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[2]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement"),
					LocalizeString("Path")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/Build";

            public static readonly InteractionDefinition Singleton = new Definition();

            public static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/Build:" + name, parameters);
            }

            public override bool Run()
            {
                Target.LotCurrent.BuildBuyOnThisLotCheatEnabled = true;
                GameStates.TransitionToBuildMode(Target.LotCurrent, null);
                return true;
            }
        }

        private sealed class Buy : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, Buy>
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.LotCurrent.IsControllable;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[2]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement"),
					Build.LocalizeString("Path")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/Buy";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/Buy:" + name, parameters);
            }

            public override bool Run()
            {
                Target.LotCurrent.BuildBuyOnThisLotCheatEnabled = true;
                GameStates.TransitionToBuyMode(Target.LotCurrent, null);
                return true;
            }
        }

        private sealed class Blueprint : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, Blueprint>
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.LotCurrent.IsControllable;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return LocalizeString("InteractionName");
                }

                public override string[] GetPath(bool isFemale)
                {
                    return new string[2]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement"),
					Build.LocalizeString("Path")
				};
                }
            }

            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/Blueprint";

            public static readonly InteractionDefinition Singleton = new Definition();

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/Blueprint:" + name, parameters);
            }

            public override bool Run()
            {
                Target.LotCurrent.BuildBuyOnThisLotCheatEnabled = true;
                GameStates.TransitionToBlueprintMode(Target.LotCurrent, true);
                return true;
            }
        }

        private sealed class AssignResortWorkerUniformForAll : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ActorlessInteractionDefinition<Sim, ResortFrontDesk, AssignResortWorkerUniformForAll>, IResortManagementInteraction
            {
                public int ChosenIndex = -1;

                public Definition()
                {
                }

                public Definition(int index)
                {
                    ChosenIndex = index;
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (ResortExpenseDialog.sDialog != null)
                    {
                        return new string[1]
					{
						Localization.LocalizeString("Gameplay/ResortOutfits:AssignResortUniformForAll")
					};
                    }
                    return new string[2]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement"),
					Localization.LocalizeString("Gameplay/ResortOutfits:AssignResortUniformForAll")
				};
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    return ResortWorker.GetLocalizedOutfitStyleName(ChosenIndex);
                }

                public override void AddInteractions(InteractionObjectPair iop, Sim actor, ResortFrontDesk target, List<InteractionObjectPair> results)
                {
                    int num = ResortWorker.kOutfitStyleNames.Length;
                    for (int i = -1; i < num; i++)
                    {
                        results.Add(new InteractionObjectPair(new Definition(i), iop.Target));
                    }
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return target.LotCurrent.IsControllable;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                Definition definition = base.InteractionDefinition as Definition;
                ResortManager resortManager = Target.LotCurrent.ResortManager;
                resortManager.SetAssignedUniformIndexForAll(definition.ChosenIndex);
                Service[] array = new Service[5]
			{
				ResortWorker.Instance,
				ResortWorkerBar.ResortBartender,
				ResortMaintenanceLow.Instance,
				ResortMaintenanceMedium.Instance,
				ResortMaintenanceHigh.Instance
			};
                Service[] array2 = array;
                foreach (Service service in array2)
                {
                    foreach (Sim item in service.GetSimsAssignedToLot(Target.LotCurrent))
                    {
                        if (!IsWearingCustomUniform(item.SimDescription, item.Service))
                        {
                            Sim.ChangeIntoAssignedResortWorkerUniform changeIntoAssignedResortWorkerUniform = Sim.ChangeIntoAssignedResortWorkerUniform.Singleton.CreateInstance(item, item, GetPriority(), base.Autonomous, true) as Sim.ChangeIntoAssignedResortWorkerUniform;
                            if (changeIntoAssignedResortWorkerUniform != null)
                            {
                                changeIntoAssignedResortWorkerUniform.ChosenIndex = definition.ChosenIndex;
                                changeIntoAssignedResortWorkerUniform.ClearCustomUniform = false;
                                item.InteractionQueue.AddNext(changeIntoAssignedResortWorkerUniform);
                            }
                        }
                    }
                }
                return true;
            }

            private bool IsWearingCustomUniform(SimDescription sd, Service service)
            {
                ResortWorker resortWorker = service as ResortWorker;
                if (resortWorker != null)
                {
                    return resortWorker.IsUsingCustomUniform(sd.SimDescriptionId);
                }
                ResortMaintenance resortMaintenance = service as ResortMaintenance;
                if (resortMaintenance != null)
                {
                    return resortMaintenance.IsUsingCustomUniform(sd, Target.LotCurrent.LotId);
                }
                return false;
            }
        }

        private sealed class CollectFund : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, ResortFrontDesk, CollectFund>, IResortManagementInteraction
            {
                public override bool Test(Sim actor, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    if (target.LotCurrent.IsControllable)
                    {
                        PropertyData propertyData = null;
                        if (actor.Household.RealEstateManager != null)
                        {
                            propertyData = actor.Household.RealEstateManager.FindProperty(target.LotCurrent);
                        }
                        if (propertyData != null)
                        {
                            if (propertyData.CurrentCollectibleFunds > 0)
                            {
                                return true;
                            }
                            greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(Localization.LocalizeString("Ui/Caption/RealEstateDialog/Button:CollectMoneyGreyedOut"));
                            return false;
                        }
                    }
                    return false;
                }

                public override string GetInteractionName(Sim actor, ResortFrontDesk target, InteractionObjectPair iop)
                {
                    PropertyData propertyData = null;
                    if (actor.Household.RealEstateManager != null)
                    {
                        propertyData = actor.Household.RealEstateManager.FindProperty(target.LotCurrent);
                    }
                    int num = 0;
                    if (propertyData != null)
                    {
                        num = Math.Max(0, propertyData.CurrentCollectibleFunds);
                    }
                    return Localization.LocalizeString("Gameplay/Objects/Miscellaneous/ResortFrontDesk:CollectResortProfits", num);
                }

                public override string[] GetPath(bool isFemale)
                {
                    if (ResortExpenseDialog.sDialog != null)
                    {
                        return new string[0];
                    }
                    return new string[1]
				{
					Localization.LocalizeString("Gameplay/Objects/Resort:ResortManagement")
				};
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                PropertyData propertyData = null;
                if (Actor.Household.RealEstateManager != null)
                {
                    propertyData = Actor.Household.RealEstateManager.FindProperty(Target.LotCurrent);
                }
                if (propertyData != null)
                {
                    propertyData.CollectMoney();
                }
                return true;
            }
        }

        private sealed class DEBUG_GetFiveStarResortAward : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, ResortFrontDesk, DEBUG_GetFiveStarResortAward>
            {
                public override string GetInteractionName(Sim a, ResortFrontDesk target, InteractionObjectPair interaction)
                {
                    return "DEBUG Get Five-Star Resort Award";
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                ResortManager.GiveFirstTimeFiveStarAwards();
                return true;
            }
        }

        private sealed class DEBUG_ClearWasGivenFiveStarResortAwards : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, ResortFrontDesk, DEBUG_ClearWasGivenFiveStarResortAwards>
            {
                public override string GetInteractionName(Sim a, ResortFrontDesk target, InteractionObjectPair interaction)
                {
                    return "DEBUG - Clear FiveStarResortAwards flag";
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                Actor.Household.RealEstateManager.WasGivenFiveStarResortAwards = false;
                return true;
            }
        }

        private sealed class DEBUG_GetLocalsRefugeAward : ImmediateInteraction<Sim, ResortFrontDesk>
        {
            [DoesntRequireTuning]
            private sealed class Definition : ImmediateInteractionDefinition<Sim, ResortFrontDesk, DEBUG_GetLocalsRefugeAward>
            {
                public override string GetInteractionName(Sim a, ResortFrontDesk target, InteractionObjectPair interaction)
                {
                    return "DEBUG Get Locals Refuge Award";
                }

                public override bool Test(Sim a, ResortFrontDesk target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }
            }

            public static readonly InteractionDefinition Singleton = new Definition();

            public override bool Run()
            {
                string p = "UnchartedIslandDiscovery, LocalsRefuge, Ui/Caption/CayToTheCityDialog:LocalsRefugeTNS, Ui/Caption/CayToTheCityDialog:LocalsRefugeAlreadyTNS";
                ArrayList arrayList = new ArrayList();
                RewardInfo info;
                RewardsManager.TryParseReward(p, out info);
                arrayList.Add(info);
                RewardsManager.GiveRewards(arrayList, Actor, Origin.None);
                return true;
            }
        }

        private class AttendingResortFrontDesk : Posture
        {
            private const string sLocalizationKey = "Gameplay/Objects/Resort/ResortFrontDesk/AttendingResortFrontDesk";

            private ResortFrontDesk mResortFrontDesk;

            private Sim mAttendant;

            private bool mIsNeutralStandingIdle = true;

            private bool mAutonomyDisabled;

            public bool IsDoingStandingIdles
            {
                get
                {
                    return mIsNeutralStandingIdle;
                }
            }

            public override IGameObject Container
            {
                get
                {
                    return mResortFrontDesk;
                }
            }

            public override string Name
            {
                get
                {
                    return LocalizeString(mAttendant.IsFemale, "PostureName");
                }
            }

            public override bool PerformIdleLogic
            {
                get
                {
                    return false;
                }
            }

            private static string LocalizeString(string name, params object[] parameters)
            {
                return Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk/AttendingResortFrontDesk:" + name, parameters);
            }

            private static string LocalizeString(bool isFemale, string name, params object[] parameters)
            {
                return Localization.LocalizeString(isFemale, "Gameplay/Objects/Resort/ResortFrontDesk/AttendingResortFrontDesk:" + name, parameters);
            }

            private AttendingResortFrontDesk()
            {
            }

            public AttendingResortFrontDesk(Sim attendant, ResortFrontDesk booth, StateMachineClient smc)
                : base(smc)
            {
                mAttendant = attendant;
                mResortFrontDesk = booth;
                mAttendant.Autonomy.IncrementAutonomyDisabled();
                mAutonomyDisabled = true;
                if (mAttendant.IsSelectable)
                {
                    mAttendant.DisableSocialsOnSim();
                }
            }

            private void KickSimOut()
            {
                CancelPosture(mAttendant);
            }

            public override void PopulateInteractions()
            {
                AddInteraction(AskToCheckIn.Singleton, mAttendant);
                AddInteraction(AskToCheckIn.WithSingleton, mAttendant);
                AddInteraction(AskAboutAdventure.Singleton, mAttendant);
                AddInteraction(AskAboutDivingSpots.Singleton, mAttendant);
                AddInteraction(PurchaseMosquitoSpray.Singleton, mAttendant);
                AddInteraction(AskToCheckOut.Singleton, mAttendant);
            }

            public override float Satisfaction(CommodityKind condition, IGameObject target)
            {
                switch (condition)
                {
                    case CommodityKind.AttendingServiceObject:
                        return 1f;
                    case CommodityKind.IsTarget:
                        if (target == mResortFrontDesk)
                        {
                            return 1f;
                        }
                        return 0f;
                    default:
                        return 0f;
                }
            }

            public override InteractionInstance GetStandingTransition()
            {
                return ExitResortFrontDesk.Singleton.CreateInstance(mResortFrontDesk, mAttendant, mAttendant.InheritedPriority(), true, false);
            }

            public override ScriptPosture GetSacsPostureParameter()
            {
                return ScriptPosture.NoAnimation;
            }

            public override float GetAutonomyScoreMultiplierForInteraction(Sim actor, IGameObject target, PosturePreconditionOptionsData options)
            {
                return 0f;
            }

            public override void Shoo(bool yield, List<Sim> shooedSims)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            public override Posture OnReset(IGameObject objectBeingReset)
            {
                if (mAttendant.IsSelectable)
                {
                    mAttendant.EnableSocialsOnSim();
                }
                return null;
            }

            public override bool AllowsReactionOverlay()
            {
                return mIsNeutralStandingIdle;
            }

            public override bool AllowsNormalSocials()
            {
                return !mAttendant.IsSelectable;
            }

            public override bool AllowsRouting()
            {
                return false;
            }

            public override BridgeOrigin Idle()
            {
                if (!mIsNeutralStandingIdle)
                {
                    mIsNeutralStandingIdle = true;
                    CurrentStateMachine.RequestState("x", "LoopIdle");
                }
                else
                {
                    CurrentStateMachine.RequestState("x", "LoopHandsOnIdle");
                    mIsNeutralStandingIdle = false;
                }
                return new BridgeOrigin(CurrentStateMachine, "x", "AttendBooth");
            }

            public void PutHandsOnBooth()
            {
                mIsNeutralStandingIdle = false;
                CurrentStateMachine.RequestState("x", "AttendBooth");
            }

            public override void OnExitPosture()
            {
                if (mAutonomyDisabled)
                {
                    mAttendant.Autonomy.DecrementAutonomyDisabled();
                    mAttendant.SocialComponent.OnlyAllowedSocial = string.Empty;
                }
                if (mAttendant.IsSelectable)
                {
                    mAttendant.EnableSocialsOnSim();
                }
                ResortStaffedObjectComponent resortStaffedObjectComponent = mResortFrontDesk.ResortStaffedObjectComponent;
                if (resortStaffedObjectComponent != null && resortStaffedObjectComponent.MoonlightingSim == mAttendant)
                {
                    resortStaffedObjectComponent.EndMoonlightingUse();
                }
                base.OnExitPosture();
            }
        }

        private const uint kIsMainDesk = 253979799u;

        private const uint kCurrentRating = 253979804u;

        private const uint kMaintenance0 = 253979807u;

        private const uint kMaintenance1 = 253979811u;

        private const uint kMaintenance2 = 253979816u;

        private const uint kClosed = 253979819u;

        private const uint kBankrupt = 253979823u;

        private const uint kPrice = 253979826u;

        private const uint kUniform = 260885917u;

        private const uint kHashCurrentGroupId = 256940870u;

        private const uint kHashVIPRoomData = 256940883u;

        private const uint kShiftKey = 253973685u;

        private const int kUniformVersion = 2;

        private const string kWorldBuilderDataName = "ResortFrontDesk";



        public SimQueue Line = new SimQueueBehindSlotLine(SimQueue.WaitLocation.HangAroundNearObject, kMaximumNumberOfSimsInLine, Slot.RoutingSlot_1, kLineSlotMinDistance, kLineSlotMaxDistance, kLineStartAngle, kLineStartSpacingFactor);

        private bool mMainDesk;

        [Tunable]
        [TunableComment("Range: Positive integers.  Description:  Maximum number of Sims that can wait in line at a time for a particular booth.")]
        private static int kMaximumNumberOfSimsInLine = 3;

        [TunableComment("Range: Positive floats.  Description: Amount of Sim minutes a Sim will wait in line to be kissed before timing out and exiting.")]
        [Tunable]
        private static float kTimeToWaitInLine = 30f;

        [TunableComment("Range: Positive floats (0-100).  Description: Chance that a front desk attendant will decide to do standing idles instead of tending idles. (tending idles are idles are the special hands on desk idles).")]
        [Tunable]
        private static float kChanceNeutralStandIdle = 50f;

        [Tunable]
        [TunableComment("cost for each shift.  Day, night, graveyard")]
        private static float[] kShiftCosts = new float[3]
	{
		50f,
		50f,
		75f
	};

        [TunableComment("score for each shift.  Day, night, graveyard")]
        [Tunable]
        private static float[] kShiftScore = new float[3]
	{
		50f,
		50f,
		25f
	};

        [Tunable]
        [TunableComment("base review string for no day shift")]
        private static string kNoDayShiftBase = "Gameplay/Objects/Miscellaneous/ResortFrontDesk:NoDayShift";

        [Tunable]
        [TunableComment("number of no day shift strings")]
        private static uint kNoDayShiftCount = 1u;

        [Tunable]
        [TunableComment("base review string for no night shift")]
        private static string kNoNightShiftBase = "Gameplay/Objects/Miscellaneous/ResortFrontDesk:NoNightShift";

        [Tunable]
        [TunableComment("number of no night shift strings")]
        private static uint kNoNightShiftCount = 1u;

        [Tunable]
        [TunableComment("base review string for no graveyard shift")]
        private static string kNoGraveyardShiftBase = "Gameplay/Objects/Miscellaneous/ResortFrontDesk:NoGraveyardShift";

        [TunableComment("number of no graveyard shift strings")]
        [Tunable]
        private static uint kNoGraveyardShiftCount = 1u;

        [Tunable]
        [TunableComment("number of Sims a front desk can satisfy")]
        private static uint kSimsServedCapacity = 10u;

        [TunableComment("Cost of Mosquito Spray if purchased at the resort front desk.  Please make sure cost makes sense compaired to price in medator")]
        [Tunable]
        private static int kMosquitoSprayCost = 80;

        [Tunable]
        [TunableComment("Random chance for NPC to check into VIP rooms")]
        private static float kVIPChance = 0.2f;

        [Tunable]
        [TunableComment("Price Multiplier for VIP room")]
        private static float kVIPPriceMultiplier = 2f;

        [TunableComment("Distance from the desks's entry routing slot to the beginning of the head of the line. Initial GPE value: 1.0f")]
        [Tunable]
        private static float kLineSlotMinDistance = 1f;

        [Tunable]
        [TunableComment("Distance from the desks's entry routing slot to the end of the head of the line.  Initial GPE value: 2.0f")]
        private static float kLineSlotMaxDistance = 2f;

        [TunableComment("The angle (width in radians) of the cone shape that represents the head of the line. Initial GPE value: 2.356194 (Pi *3 / 4).")]
        [Tunable]
        private static float kLineStartAngle = 2.3561945f;

        [Tunable]
        [TunableComment("Spacing factor of routing options for line head. Positive float. Init GPE default: 2.0f")]
        private static float kLineStartSpacingFactor = 2f;

        [TunableComment("Max number of people you can check in with")]
        [Tunable]
        private static int kMaxNumSimToCheckinWith = 10;

        [TunableComment("Minimum waiting distance to the front desk for the Sim")]
        [Tunable]
        private static float kMinWaitingDistance = 2f;

        [Tunable]
        [TunableComment("Maximum waiting distance to the front desk for the Sim")]
        private static float kMaxWaitingDistance = 5f;

        [Tunable]
        [TunableComment("Angle of the routing area in front of the front desk (in radius)")]
        private static float kConeAngleForRouting = 2.2f;

        [TunableComment("Spacing factor for routing position.  Positive float.  Init GP default: 6.0f")]
        [Tunable]
        private static float kSpacingFactorRouting = 6f;

        private static readonly string[] kWorldBuilderDataKeyRemap = new string[20]
	{
		253979799u.ToString(),
		"MainDesk",
		253979804u.ToString(),
		"CurrentRating",
		253979807u.ToString(),
		"Maintenance0",
		253979811u.ToString(),
		"Maintenance1",
		253979816u.ToString(),
		"Maintenance2",
		253979819u.ToString(),
		"Closed",
		253979823u.ToString(),
		"Bankrupt",
		253979826u.ToString(),
		"Price",
		253973685u.ToString(),
		"ShiftKey",
		260885917u.ToString(),
		"Uniform"
	};

        public bool IsMainDesk
        {
            get
            {
                return mMainDesk;
            }
        }

        public override SimQueue SimLine
        {
            get
            {
                return Line;
            }
        }

        public ResortObjectType ObjectType
        {
            get
            {
                return ResortObjectType.ResortFrontDesk;
            }
        }

        public float UpkeepCost
        {
            get
            {
                return 0f;
            }
        }

        public float Score
        {
            get
            {
                SetHoursDialog.Shifts moonlightAdjustedShifts = base.ResortStaffedObjectComponent.MoonlightAdjustedShifts;
                float num = 0f;
                if ((moonlightAdjustedShifts & SetHoursDialog.Shifts.Day) != 0)
                {
                    num += kShiftScore[0];
                }
                if ((moonlightAdjustedShifts & SetHoursDialog.Shifts.Night) != 0)
                {
                    num += kShiftScore[1];
                }
                if ((moonlightAdjustedShifts & SetHoursDialog.Shifts.Graveyard) != 0)
                {
                    num += kShiftScore[2];
                }
                return num;
            }
        }

        public uint SimsServed
        {
            get
            {
                return kSimsServedCapacity;
            }
        }

        public float EmployeeCost
        {
            get
            {
                SetHoursDialog.Shifts shifts = base.ResortStaffedObjectComponent.Shifts;
                float cost = 0f;
                if ((shifts & SetHoursDialog.Shifts.Day) != 0)
                {
                    cost += kShiftCosts[0];
                }
                if ((shifts & SetHoursDialog.Shifts.Night) != 0)
                {
                    cost += kShiftCosts[1];
                }
                if ((shifts & SetHoursDialog.Shifts.Graveyard) != 0)
                {
                    cost += kShiftCosts[2];
                }
                base.ResortStaffedObjectComponent.AdjustUpkeepForMoonlightStaffing(kShiftCosts, ref cost);
                return Math.Max(cost, 0f);
            }
        }

        public float MoonlightingAdjustment
        {
            get
            {
                float cost = 0f;
                base.ResortStaffedObjectComponent.AdjustUpkeepForMoonlightStaffing(ShiftCost, ref cost);
                return cost;
            }
        }

        public float[] ShiftCost
        {
            get
            {
                return kShiftCosts;
            }
        }

        private ResortManager ResortManager
        {
            get
            {
                ResortManager resortManager = base.LotCurrent.ResortManager;
                if (resortManager == null)
                {
                    resortManager = new ResortManager(base.LotCurrent);
                    base.LotCurrent.ResortManager = resortManager;
                }
                return resortManager;
            }
        }

        public static string LocalizeString(bool isFemale, string name, params object[] parameters)
        {
            return Localization.LocalizeString(isFemale, sLocalizationKey + name, parameters);
        }

        public override void OnStartup()
        {
            base.OnStartup();
            AddInteraction(AttendToResortFrontDesk.Singleton);
            AddInteraction(DoItYourself.Singleton);
            AddInteraction(AskToCheckInDeskProxy.Singleton);
            AddInteraction(SetPrices.Singleton);
            AddInteraction(ToggleOpen.Singleton);
            AddInteraction(ManageCrew.Singleton);
            AddInteraction(ViewExpenses.Singleton);
            AddInteraction(ViewReviews.Singleton);
            AddInteraction(Blueprint.Singleton);
            AddInteraction(Build.Singleton);
            AddInteraction(Buy.Singleton);
            AddInteraction(AssignResortWorkerUniformForAll.Singleton);
            AddInteraction(CollectFund.Singleton);
            AddInteraction(AskToCheckInOffLotDisabledInteraction.Singleton);
            AddInteraction(AskToCheckInOffLotDisabledInteraction.WithSingleton);
            AddInteraction(AskToCheckInOffLot.Singleton);
            AddInteraction(AskToCheckInOffLot.WithSingleton);
            AddComponent<ResortStaffedObjectComponent>(new object[2]
		{
			ServiceType.Resortworker,
			"FrontDesk"
		});
        }

        public override void AddBuildBuyInteractions(List<InteractionDefinition> buildBuyInteractions)
        {
            buildBuyInteractions.Add(SetPrices.Singleton);
            buildBuyInteractions.Add(ToggleOpen.Singleton);
            buildBuyInteractions.Add(ManageCrew.Singleton);
            buildBuyInteractions.Add(AssignResortWorkerUniformForAll.Singleton);
            base.AddBuildBuyInteractions(buildBuyInteractions);
        }

        public override void AddDebugInteractions(List<InteractionDefinition> debugInteractions)
        {
            base.AddDebugInteractions(debugInteractions);
            debugInteractions.Add(DEBUG_GetFiveStarResortAward.Singleton);
            debugInteractions.Add(DEBUG_ClearWasGivenFiveStarResortAwards.Singleton);
            debugInteractions.Add(DEBUG_GetLocalsRefugeAward.Singleton);
        }

        public List<InteractionObjectPair> GetProxyInteractions(InteractionObjectPair iop, Sim Actor, Lot Target)
        {
            List<InteractionObjectPair> list = new List<InteractionObjectPair>();
            list.Add(new InteractionObjectPair(AttendToResortFrontDesk.Singleton, this));
            list.Add(new InteractionObjectPair(SetPrices.LowpriceSingletion, this));
            list.Add(new InteractionObjectPair(SetPrices.MediumpriceSingletion, this));
            list.Add(new InteractionObjectPair(SetPrices.HighriceSingletion, this));
            list.Add(new InteractionObjectPair(ToggleOpen.Singleton, this));
            list.Add(new InteractionObjectPair(ManageCrew.Singleton, this));
            list.Add(new InteractionObjectPair(ViewExpenses.Singleton, this));
            list.Add(new InteractionObjectPair(ViewReviews.Singleton, this));
            list.Add(new InteractionObjectPair(Build.Singleton, this));
            list.Add(new InteractionObjectPair(Buy.Singleton, this));
            list.Add(new InteractionObjectPair(Blueprint.Singleton, this));
            list.Add(new InteractionObjectPair(AskToCheckInOffLotDisabledInteraction.Singleton, this));
            list.Add(new InteractionObjectPair(AskToCheckInOffLotDisabledInteraction.WithSingleton, this));
            list.Add(new InteractionObjectPair(AssignResortWorkerUniformForAll.Singleton, this));
            AskToCheckInOffLot.Definition singleton = AskToCheckInOffLot.Singleton;
            singleton.AddInteractionsHelper(iop, Actor, this, true, list);
            singleton = AskToCheckInOffLot.WithSingleton;
            singleton.AddInteractionsHelper(iop, Actor, this, true, list);
            return list;
        }

        public override Tooltip CreateTooltip(Vector2 mousePosition, WindowBase mousedOverWindow, ref Vector2 tooltipPosition)
        {
            Sim simAttendingDesk = GetSimAttendingDesk();
            if (simAttendingDesk == null)
            {
                return new SimpleTextTooltip(Localization.LocalizeString("Gameplay/Objects/Resort/ResortFrontDesk:NoAttendant"));
            }
            return base.CreateTooltip(mousePosition, mousedOverWindow, ref tooltipPosition);
        }

        public Sim GetSimAttendingDesk()
        {
            ResortStaffedObjectComponent resortStaffedObjectComponent = base.ResortStaffedObjectComponent;
            Sim sim = resortStaffedObjectComponent.MoonlightingSim;
            if (sim == null)
            {
                ResortWorker service = base.ResortStaffedObjectComponent.GetService();
                if (service != null)
                {
                    sim = service.GetWorker(this);
                }
            }
            return sim;
        }

        public static bool FrontDeskCheckInTestCommon(Sim a, ResortFrontDesk frontDesk, bool isAutonomous, bool isExtend, bool isVIPCheckin)
        {
            if (a.LotHome == null)
            {
                return false;
            }
            Lot lotCurrent = frontDesk.LotCurrent;
            ResortManager resortManager = lotCurrent.ResortManager;
            if (resortManager == null)
            {
                return false;
            }
            bool flag = resortManager.IsCheckedIn(a);
            bool flag2 = resortManager.FindVIPRoomDataBySimId(a.SimDescription.SimDescriptionId) != null;
            if ((!isExtend && flag) || (isExtend && !flag) || (!isVIPCheckin && a.IsSelectable && a.Household.RealEstateManager.FindVenueProperty(lotCurrent) != null))
            {
                return false;
            }
            if (isExtend)
            {
                if ((flag2 && !isVIPCheckin) || (!flag2 && isVIPCheckin))
                {
                    return false;
                }
            }
            else if (isVIPCheckin)
            {
                bool result = false;
                if (lotCurrent.ResortManager.VIPRooms.Count > 0)
                {
                    foreach (ResortManager.VIPRoomData vIPRoom in lotCurrent.ResortManager.VIPRooms)
                    {
                        if (vIPRoom.SimDescriptionId == 0)
                        {
                            return true;
                        }
                        SimDescription simDescription = SimDescription.Find(vIPRoom.SimDescriptionId);
                        if (simDescription != null)
                        {
                            Sim createdSim = simDescription.CreatedSim;
                            if (createdSim != null && createdSim.IsNPC && !isAutonomous)
                            {
                                return true;
                            }
                        }
                    }
                    return result;
                }
                return result;
            }
            return true;
        }

        public bool RouteInFrontOfDesk(Sim actor)
        {
            Route route = actor.CreateRoute();
            RadialRangeDestination radialRangeDestination = new RadialRangeDestination();
            radialRangeDestination.mCenterPoint = Position;
            radialRangeDestination.mConeVector = base.ForwardVector.Normalize();
            radialRangeDestination.mfConeAngle = kConeAngleForRouting;
            radialRangeDestination.mfMinRadius = kMinWaitingDistance;
            radialRangeDestination.mfMaxRadius = kMaxWaitingDistance;
            radialRangeDestination.mfPreferredSpacing = radialRangeDestination.mfMinRadius * radialRangeDestination.mfConeAngle / kSpacingFactorRouting;
            radialRangeDestination.mFacingPreference = RouteOrientationPreference.TowardsObject;
            route.SetOption(Route.RouteOption.DoLineOfSightCheckUserOverride, true);
            route.SetOption(Route.RouteOption.CheckForFootprintsNearGoals, true);
            route.AddDestination(radialRangeDestination);
            if (route.Plan().Succeeded())
            {
                return actor.DoRoute(route);
            }
            return false;
        }

        public bool GetReviewText(ResortReviewQuality quality, ref string baseString, ref uint stringCount)
        {
            SetHoursDialog.Shifts shifts = SetHoursDialog.Shifts.None;
            ResortFrontDesk[] objects = mOwnerLot.GetObjects<ResortFrontDesk>();
            foreach (ResortFrontDesk resortFrontDesk in objects)
            {
                shifts |= resortFrontDesk.ResortStaffedObjectComponent.MoonlightAdjustedShifts;
            }
            List<string> list = new List<string>();
            List<uint> list2 = new List<uint>();
            if ((shifts & SetHoursDialog.Shifts.Day) == SetHoursDialog.Shifts.None)
            {
                list.Add(kNoDayShiftBase);
                list2.Add(kNoDayShiftCount);
            }
            if ((shifts & SetHoursDialog.Shifts.Night) == SetHoursDialog.Shifts.None)
            {
                list.Add(kNoNightShiftBase);
                list2.Add(kNoNightShiftCount);
            }
            if ((shifts & SetHoursDialog.Shifts.Graveyard) == SetHoursDialog.Shifts.None)
            {
                list.Add(kNoGraveyardShiftBase);
                list2.Add(kNoGraveyardShiftCount);
            }
            int count = list.Count;
            if (count == 0)
            {
                return false;
            }
            int @int = RandomUtil.GetInt(count - 1);
            baseString = list[@int];
            stringCount = list2[@int];
            return true;
        }

        private bool IsMainFrontDesk()
        {
            ResortFrontDesk[] objects = base.LotCurrent.GetObjects<ResortFrontDesk>();
            foreach (ResortFrontDesk resortFrontDesk in objects)
            {
                if (resortFrontDesk.mMainDesk)
                {
                    return resortFrontDesk == this;
                }
            }
            mMainDesk = true;
            return true;
        }

        private bool ExportVIPRoomData(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
        {
            if (base.LotCurrent.ResortManager.VIPRooms == null)
            {
                return false;
            }
            ResortManager.ResortManagerPropertyStreamWriter resortManagerPropertyStreamWriter = new ResortManager.ResortManagerPropertyStreamWriter();
            writer.AdoptChild(256940883u, resortManagerPropertyStreamWriter);
            bool result = resortManagerPropertyStreamWriter.Export(resKeyTable, objIdTable, base.LotCurrent.ResortManager.VIPRooms);
            writer.CommitChild();
            return result;
        }

        private bool ImportVIPRoomData(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            ResortManager.ResortManagerPropertyStreamReader resortManagerPropertyStreamReader = new ResortManager.ResortManagerPropertyStreamReader();
            reader.AdoptChild(256940883u, resortManagerPropertyStreamReader);
            return resortManagerPropertyStreamReader.Import(resKeyTable, objIdTable, out base.LotCurrent.ResortManager.VIPRooms);
        }

        public override bool ExportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamWriter writer)
        {
            bool result = base.ExportContent(resKeyTable, objIdTable, writer);
            writer.WriteBool(253979799u, mMainDesk);
            if (mMainDesk)
            {
                Lot lotCurrent = base.LotCurrent;
                if (lotCurrent != null)
                {
                    ResortManager resortManager = lotCurrent.ResortManager;
                    if (resortManager != null)
                    {
                        writer.WriteFloat(253979804u, resortManager.CurrentRating);
                        writer.WriteInt32(253979807u, resortManager.MaintenanceCounts[0]);
                        writer.WriteInt32(253979811u, resortManager.MaintenanceCounts[1]);
                        writer.WriteInt32(253979816u, resortManager.MaintenanceCounts[2]);
                        writer.WriteBool(253979819u, resortManager.Closed);
                        writer.WriteBool(253979823u, resortManager.IsBankrupt);
                        writer.WriteUint32(253979826u, (uint)resortManager.Price);
                        writer.WriteInt32(260885917u, resortManager.AssignedUniformIndex);
                        ExportVIPRoomData(resKeyTable, objIdTable, writer);
                    }
                }
            }
            return result;
        }

        public override bool ImportContent(ResKeyTable resKeyTable, ObjectIdTable objIdTable, IPropertyStreamReader reader)
        {
            bool result = base.ImportContent(resKeyTable, objIdTable, reader);
            reader.ReadBool(253979799u, out mMainDesk, false);
            if (mMainDesk)
            {
                Lot lotCurrent = base.LotCurrent;
                if (lotCurrent != null)
                {
                    ResortManager resortManager = lotCurrent.ResortManager;
                    if (resortManager == null)
                    {
                        resortManager = (lotCurrent.ResortManager = new ResortManager(lotCurrent));
                    }
                    float value;
                    if (reader.ReadFloat(253979804u, out value, 0f))
                    {
                        resortManager.CurrentRating = value;
                    }
                    int value2;
                    if (reader.ReadInt32(253979807u, out value2, 0))
                    {
                        resortManager.MaintenanceCounts[0] = value2;
                    }
                    if (reader.ReadInt32(253979811u, out value2, 0))
                    {
                        resortManager.MaintenanceCounts[1] = value2;
                    }
                    if (reader.ReadInt32(253979816u, out value2, 0))
                    {
                        resortManager.MaintenanceCounts[2] = value2;
                    }
                    if (reader.ReadInt32(260885917u, out value2, -1))
                    {
                        resortManager.SetAssignedUniformIndexForAll(value2);
                    }
                    bool value3;
                    if (reader.ReadBool(253979819u, out value3, false))
                    {
                        resortManager.Closed = value3;
                    }
                    if (reader.ReadBool(253979823u, out value3, false))
                    {
                        resortManager.IsBankrupt = value3;
                    }
                    uint value4;
                    if (reader.ReadUint32(253979826u, out value4, 0u))
                    {
                        resortManager.Price = (ResortManager.Pricing)value4;
                    }
                    ImportVIPRoomData(resKeyTable, objIdTable, reader);
                    resortManager.RefreshResortData();
                    resortManager.RefreshVIPRooms();
                }
            }
            return result;
        }

        public void SaveWorldBuilderData()
        {
            WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "MainDesk", mMainDesk, false);
            if (mMainDesk)
            {
                Lot lotCurrent = base.LotCurrent;
                if (lotCurrent != null)
                {
                    ResortManager resortManager = lotCurrent.ResortManager;
                    if (resortManager != null)
                    {
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "CurrentRating", resortManager.CurrentRating, 0f);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Maintenance0", resortManager.MaintenanceCounts[0], 0);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Maintenance1", resortManager.MaintenanceCounts[1], 0);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Maintenance2", resortManager.MaintenanceCounts[2], 0);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Closed", resortManager.Closed, false);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Bankrupt", resortManager.IsBankrupt, false);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Price", (ushort)resortManager.Price, (ushort)1);
                        WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Uniform", resortManager.AssignedUniformIndex, -1);
                    }
                }
            }
            WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "ShiftKey", (ushort)base.ResortStaffedObjectComponent.Shifts, (ushort)3);
            WorldBuilderData.Set("ResortFrontDesk", base.ObjectId, "Version", 2, 0);
        }

        public void LoadWorldBuilderData()
        {
            int value = 0;
            WorldBuilderData.SetKeyRemap(kWorldBuilderDataKeyRemap);
            WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Version", ref value);
            WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "MainDesk", ref mMainDesk);
            if (mMainDesk)
            {
                Lot lotCurrent = base.LotCurrent;
                if (lotCurrent != null)
                {
                    ResortManager resortManager = lotCurrent.ResortManager;
                    if (resortManager == null)
                    {
                        resortManager = (lotCurrent.ResortManager = new ResortManager(lotCurrent));
                    }
                    float value2 = 0f;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "CurrentRating", ref value2);
                    resortManager.CurrentRating = value2;
                    int value3 = 0;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Maintenance0", ref value3);
                    resortManager.MaintenanceCounts[0] = value3;
                    value3 = 0;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Maintenance1", ref value3);
                    resortManager.MaintenanceCounts[1] = value3;
                    value3 = 0;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Maintenance2", ref value3);
                    resortManager.MaintenanceCounts[2] = value3;
                    if (value >= 2)
                    {
                        value3 = -1;
                        WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Uniform", ref value3);
                        resortManager.SetAssignedUniformIndexForAll(value3);
                    }
                    bool value4 = false;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Closed", ref value4);
                    resortManager.Closed = value4;
                    value4 = false;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Bankrupt", ref value4);
                    resortManager.IsBankrupt = value4;
                    ushort value5 = 1;
                    WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "Price", ref value5);
                    resortManager.Price = (ResortManager.Pricing)value5;
                    resortManager.SetFreeness();
                    resortManager.RefreshResortData();
                }
                ushort value6 = 3;
                WorldBuilderData.Get("ResortFrontDesk", base.ObjectId, "ShiftKey", ref value6);
                base.ResortStaffedObjectComponent.Shifts = (SetHoursDialog.Shifts)value6;
            }
        }

        public void OnEnterBuildBuyMode()
        {
        }

        public void OnExitBuildBuyMode()
        {
            if (IsMainFrontDesk())
            {
                ResortManager resortManager = base.LotCurrent.ResortManager;
                if (resortManager != null)
                {
                    if (base.LotCurrent.GetObjects<IResortTower>().Length == 0)
                    {
                        base.LotCurrent.ResortManager.Inoperative = true;
                        NotificationSystem.Format format = default(NotificationSystem.Format);
                        format.mTarget = mOwnerLot;
                        NotificationSystem.Show(format, TNSNames.ResortNoTowerError, mOwnerLot.Name);
                    }
                    else
                    {
                        base.LotCurrent.ResortManager.Inoperative = false;
                    }
                }
            }
        }
    }

}
