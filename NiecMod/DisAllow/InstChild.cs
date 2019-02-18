/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 15/09/2018
 * Time: 1:35
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.Controller;
using NiecMod.Nra;

namespace Sims3.Gameplay.NiecMod.Helper.Children
{
	// Token: 0x02000002 RID: 2
	public class CarryChildren
	{
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;
        private static EventListener sSimInstantiatedListener = null;
        private static EventListener sSimAgedUpListener = null;
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		static CarryChildren()
		{
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002078 File Offset: 0x00000278
        public static void OnWorldLoadFinishedHandler(object sender, System.EventArgs e)
		{
			try
			{
                foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                {
                    if (sim != null)
                    {
                        AddInteractions(sim);
                    }
                }
				sSimInstantiatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, new ProcessEventDelegate(CarryChildren.OnSimInstantiated));
				sSimAgedUpListener = EventTracker.AddListener(EventTypeId.kSimAgeTransition, new ProcessEventDelegate(CarryChildren.OnSimInstantiated));
			}
			catch (Exception exception)
			{
				Exception(exception);
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002108 File Offset: 0x00000308
        public static bool Exception(Exception exception)
        {
            try
            {
                return ((IScriptErrorWindow)AppDomain.CurrentDomain.GetData("ScriptErrorWindow")).DisplayScriptError(null, exception);
            }
            catch
            {
                WriteLog(exception);
                return true;
            }
        }

		// Token: 0x06000004 RID: 4 RVA: 0x00002154 File Offset: 0x00000354
        public static bool WriteLog(Exception exception)
        {
            try
            {
                new ScriptError(null, exception, 0).WriteMiniScriptError();
                return true;
            }
            catch
            {
                return false;
            }
        }

		// Token: 0x06000005 RID: 5 RVA: 0x0000218C File Offset: 0x0000038C
		protected static ListenerAction OnSimInstantiated(Event e)
		{
            try
            {
                Sim sim = e.TargetObject as Sim;
                if (sim != null)
                {
                    AddInteractions(sim);
                }
            }
			catch (Exception exception)
			{
                NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                Exception(exception);
			}
			return ListenerAction.Keep;
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000021DC File Offset: 0x000003DC
		public static void AddInteractions(Sim sim)
		{
			sim.AddInteraction(CarryChildren.PickUpChild.Singleton);
		}

		// Token: 0x04000001 RID: 1
		

		// Token: 0x02000003 RID: 3
		public class PickUpChild : SocialInteraction, IPopCaneAndUmbrellaPostureOnInteractionRouteEnd, IPopCanePostureOnInteractionRouteEnd
		{
			// Token: 0x06000008 RID: 8 RVA: 0x000021F8 File Offset: 0x000003F8
			public override void Cleanup()
			{
				base.Cleanup();
				bool flag = this.BabyJig != null;
				if (flag)
				{
					Simulator.DestroyObject(this.BabyJig.ObjectId);
					this.BabyJig = null;
				}
				bool flag2 = !this.Actor.HasBeenDestroyed;
				if (flag2)
				{
					this.Actor.LookAtManager.EnableLookAts();
				}
			}

			// Token: 0x06000009 RID: 9 RVA: 0x0000225C File Offset: 0x0000045C
			public override string GetInteractionName()
			{
				bool flag = this.OverrideInteractionName == null;
				string result;
				if (flag)
				{
					result = base.GetInteractionName();
				}
				else
				{
					result = this.OverrideInteractionName;
				}
				return result;
			}

			// Token: 0x0600000A RID: 10 RVA: 0x0000228C File Offset: 0x0000048C
			public override bool Run()
			{
                try
                {
                    this.Actor.LookAtManager.DisableLookAts();
                    string angle = "";
                    bool flag = this.Actor.IsActiveFirefighter && Occupation.SimIsWorkingAtJobOnLot(this.Actor, this.Target.LotCurrent);
                    if (flag)
                    {
                        base.SetPriority(new InteractionPriority(InteractionPriorityLevel.Fire, float.MinValue));
                        base.RequestWalkStyle(Sim.WalkStyle.FastJog);
                    }
                    bool baby = this.Target.SimDescription.Baby;
                    SocialJigTwoPerson socialJigTwoPerson;
                    if (baby)
                    {
                        Route route = this.Actor.CreateRoute();
                        route.PlanToPointRadialRange(this.Target, this.Target.Posture.Container.Position, 0f, SocialInteraction.kSocialRouteMinDist, Vector3.UnitZ, 360f, RouteDistancePreference.PreferNearestToRouteOrigin, RouteOrientationPreference.TowardsObject, this.Target.LotCurrent.LotId, new int[]
					{
						this.Target.RoomId
					});
                        bool flag2 = !this.Actor.DoRoute(route);
                        if (flag2)
                        {
                            return false;
                        }
                        bool flag3 = !CarryChildren.ChildUtils.StartObjectInteractionWithChild(this, this.Target, CommodityKind.Standing, "BePickedUp");
                        if (flag3)
                        {
                            return false;
                        }
                        bool flag4 = !this.Actor.RouteToObjectRadius(this.Target, 0.7f);
                        if (flag4)
                        {
                            return false;
                        }
                        this.SocialJig = (GlobalFunctions.CreateObjectOutOfWorld("SocialJigBabyToddler") as SocialJigTwoPerson);
                        this.SocialJig.SetOpacity(0f, 0f);
                        Vector3 position = this.Actor.Position;
                        Vector3 vector = this.Actor.ForwardVector;
                        this.SocialJig.SetPosition(position);
                        this.SocialJig.SetForward(vector);
                        this.SocialJig.RegisterParticipants(this.Actor, this.Target);
                        this.SocialJig.AddToWorld();
                        Vector3 forwardOfSlot = this.SocialJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
                        Vector3 forwardVector = this.Target.ForwardVector;
                        double num = (double)forwardOfSlot.x;
                        double num2 = (double)forwardOfSlot.z;
                        double num3 = (double)forwardVector.x;
                        double num4 = (double)forwardVector.z;
                        double x = num * num3 + num2 * num4;
                        double y = num * num4 - num2 * num3;
                        float num5 = (float)Math.Atan2(y, x);
                        for (num5 -= 0.7853982f; num5 < 0f; num5 += 6.283185f)
                        {
                        }
                        float num6 = 0f;
                        num5 = 2f * num5 / 3.141593f;
                        bool flag5 = num5 < 1f;
                        if (flag5)
                        {
                            angle = "_270";
                            num6 = 1.570796f;
                        }
                        else
                        {
                            bool flag6 = num5 < 2f;
                            if (flag6)
                            {
                                angle = "_180";
                                num6 = 3.141593f;
                            }
                            else
                            {
                                bool flag7 = num5 < 3f;
                                if (flag7)
                                {
                                    angle = "_90";
                                    num6 = -1.570796f;
                                }
                                else
                                {
                                    angle = "";
                                }
                            }
                        }
                        socialJigTwoPerson = (this.BabyJig = (GlobalFunctions.CreateObjectOutOfWorld("SocialJigBabyToddler") as SocialJigTwoPerson));
                        socialJigTwoPerson.SetOpacity(0f, 0f);
                        vector = this.SocialJig.ForwardVector;
                        num5 = (float)Math.Atan2(y, x);
                        vector = Quaternion.MakeFromEulerAngles(0f, num5 - num6, 0f).ToMatrix().TransformVector(vector);
                        socialJigTwoPerson.SetPosition(this.Target.Position - vector * 0.7f);
                        socialJigTwoPerson.SetForward(vector);
                        socialJigTwoPerson.AddToWorld();
                    }
                    else
                    {
                        bool flag8 = this.Actor.IsAtHome && this.Actor.LotCurrent.HasVirtualResidentialSlots && (!this.Actor.IsInPublicResidentialRoom || this.Actor.Level != this.Target.Level) && !this.Actor.RouteToObjectRadius(this.Target, 0.7f);
                        if (flag8)
                        {
                            return false;
                        }
                        socialJigTwoPerson = (socialJigTwoPerson = (GlobalFunctions.CreateObjectOutOfWorld("SocialJigBabyToddler") as SocialJigTwoPerson));
                        this.SocialJig.SetOpacity(0f, 0f);
                        string name = CarryChildren.ChildUtils.Localize(this.Target.SimDescription.IsFemale, "BePickedUp", new object[0]);
                        bool flag9 = !base.BeginSocialInteraction(new SocialInteractionB.NoAgeOrClosedVenueCheckDefinition(name, false), true, false);
                        if (flag9)
                        {
                            return false;
                        }
                    }
                    bool flag10 = this.LinkedInteractionInstance.InstanceActor.CurrentInteraction != this.LinkedInteractionInstance;
                    bool result;
                    if (flag10)
                    {
                        result = false;
                    }
                    else
                    {
                        base.BeginCommodityUpdates();
                        CarryChildren.ChildUtils.PickUpChild(this.Actor, this.Target, socialJigTwoPerson, angle);
                        bool flag11 = GameUtils.IsInstalled(ProductVersion.EP8) && this.Target.SimDescription.Toddler && this.Target.CurrentOutfitCategory != OutfitCategories.Outerwear && this.Target.IsOutside && SeasonsManager.Temperature <= ChangeToddlerClothes.kTemperatureToSwitchToddlerToOuterwear;
                        if (flag11)
                        {
                            ChangeToddlerClothes changeToddlerClothes = ChangeToddlerClothes.Singleton.CreateInstance(this.Target, this.Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer) as ChangeToddlerClothes;
                            changeToddlerClothes.Reason = Sim.ClothesChangeReason.TemperatureTooCold;
                            changeToddlerClothes.StartBuildingOutfit();
                            base.TryPushAsContinuation(changeToddlerClothes);
                        }
                        base.FinishLinkedInteraction();
                        base.EndCommodityUpdates(true);
                        base.WaitForSyncComplete();
                        result = true;
                    }
                    return result;
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                    return true;
                }
			}

			// Token: 0x04000004 RID: 4
			public static InteractionDefinition Singleton = new CarryChildren.PickUpChild.Definition();

			// Token: 0x04000005 RID: 5
			public string OverrideInteractionName;

			// Token: 0x04000006 RID: 6
			public SocialJigTwoPerson BabyJig;

			// Token: 0x02000007 RID: 7
			public class Definition : InteractionDefinition<Sim, Sim, CarryChildren.PickUpChild>, IOverridesAgeTests, IAllowedOnClosedVenues
			{
				// Token: 0x06000035 RID: 53 RVA: 0x00003550 File Offset: 0x00001750
				public static bool CanPickUpBabyOrToddler(ref InteractionInstanceParameters parameters)
				{
					bool effectivelyAutonomous = parameters.EffectivelyAutonomous;
					Sim sim = parameters.Actor as Sim;
					Sim sim2 = parameters.Target as Sim;
					bool flag = sim.IsEP11Bot && !sim.TraitManager.HasElement(TraitNames.RoboNannyChip);
					bool result;
					if (flag)
					{
						result = false;
					}
					else
					{
						bool flag2 = !effectivelyAutonomous && sim2 != null && !IUtil.IsPass(InteractionDefinitionUtilities.RunSpecialCaseAgeTests(ref parameters, sim, sim2, SpecialCaseAgeTests.Standard));
						result = !flag2;
					}
					return result;
				}

				// Token: 0x06000036 RID: 54 RVA: 0x000035DC File Offset: 0x000017DC
				public static bool CanPickUpBabyOrToddlerObjectInteraction(Sim toddler, ref InteractionInstanceParameters objectInteractionParameters)
				{
					InteractionInstanceParameters interactionInstanceParameters = new InteractionInstanceParameters(new InteractionObjectPair(CarryChildren.PickUpChild.Singleton, toddler), objectInteractionParameters.Actor, objectInteractionParameters.Priority, objectInteractionParameters.Autonomous, objectInteractionParameters.CancellableByPlayer);
					return CarryChildren.PickUpChild.Definition.CanPickUpBabyOrToddler(ref interactionInstanceParameters);
				}

				// Token: 0x06000037 RID: 55 RVA: 0x00003620 File Offset: 0x00001820
				public static string CantPickUpGreyedOutTooltipFemale()
				{
					return CarryChildren.ChildUtils.Localize(true, "CantPickUpRelationshipNotPositive", new object[0]);
				}

				// Token: 0x06000038 RID: 56 RVA: 0x00003644 File Offset: 0x00001844
				public static string CantPickUpGreyedOutTooltipMale()
				{
					return CarryChildren.ChildUtils.Localize(false, "CantPickUpRelationshipNotPositive", new object[0]);
				}

				// Token: 0x06000039 RID: 57 RVA: 0x00003668 File Offset: 0x00001868
				public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
				{
                    /*
					return CarryChildren.ChildUtils.Localize(target.IsFemale, "PickUp" + target.SimDescription.Age, new object[]
					{
						target
					});
                     */
                    return "Pick Up By Niec";
				}

				// Token: 0x0600003A RID: 58 RVA: 0x000036AC File Offset: 0x000018AC
				public SpecialCaseAgeTests GetSpecialCaseAgeTests()
				{
					return SpecialCaseAgeTests.None;
				}



                /*
				public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					bool flag = parameters.EffectivelyAutonomous && parameters.Priority.Level < InteractionPriorityLevel.ESRB;
					if (flag)
					{
						Sim sim = parameters.Actor as Sim;
						bool flag2 = sim != null && sim.CarryingChildPosture != null;
						if (flag2)
						{
							return InteractionTestResult.GenericFail;
						}
					}
					InteractionTestResult interactionTestResult = base.Test(ref parameters, ref greyedOutTooltipCallback);
					bool flag3 = !InteractionDefinitionUtilities.IsPass(interactionTestResult);
					InteractionTestResult result;
					if (flag3)
					{
						result = interactionTestResult;
					}
					else
					{
						bool flag4 = CarryChildren.PickUpChild.Definition.CanPickUpBabyOrToddler(ref parameters);
						if (flag4)
						{
							result = InteractionTestResult.Pass;
						}
						else
						{
							bool isFemale = ((Sim)parameters.Actor).IsFemale;
							if (isFemale)
							{
								greyedOutTooltipCallback = new GreyedOutTooltipCallback(CarryChildren.PickUpChild.Definition.CantPickUpGreyedOutTooltipFemale);
							}
							else
							{
								greyedOutTooltipCallback = new GreyedOutTooltipCallback(CarryChildren.PickUpChild.Definition.CantPickUpGreyedOutTooltipMale);
							}
							result = InteractionTestResult.GenericFail;
						}
					}
					return result;
				}
                */

				// Token: 0x0600003C RID: 60 RVA: 0x0000378C File Offset: 0x0000198C
				public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
                    /*
                    try
                    {
                        bool toddlerOrBelow = actor.SimDescription.ToddlerOrBelow;
                        bool result;
                        if (toddlerOrBelow)
                        {
                            result = false;
                        }
                        else
                        {
                            bool flag = target.Posture.Container != target;
                            if (flag)
                            {
                                result = false;
                            }
                            else
                            {
                                bool flag2 = actor.SimDescription.Age == target.SimDescription.Age;
                                if (flag2)
                                {
                                    result = false;
                                }
                                else
                                {
                                    bool flag3 = actor.SimDescription.TeenOrAbove && target.SimDescription.Child;
                                    if (flag3)
                                    {
                                        result = false;
                                    }
                                    else
                                    {
                                        bool flag4 = isAutonomous && actor.CarryingChildPosture != null;
                                        if (flag4)
                                        {
                                            result = false;
                                        }
                                        else
                                        {
                                            bool flag5 = target == actor.Posture.Container;
                                            if (flag5)
                                            {
                                                result = false;
                                            }
                                            else
                                            {
                                                bool flag6 = target.Posture.Satisfies(CommodityKind.InFairyHouse, null);
                                                result = (!flag6 && target.SimDescription.ChildOrBelow);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        return result;
                    }
                    catch (Exception)
                    {
                        return true;
                    }
                     */
                    if (isAutonomous) return false;
                    return true;
				}
			}
		}

		// Token: 0x02000004 RID: 4
		public class PickUpChildOutside : SocialInteraction
		{
			// Token: 0x0600000D RID: 13 RVA: 0x00002820 File Offset: 0x00000A20
			public override bool Run()
			{
				return this.Actor.InteractionQueue.PushAsContinuation(GoInsideLot.Singleton, this.Actor.LotHome, true) != null;
			}

			// Token: 0x04000007 RID: 7
			public static InteractionDefinition Singleton = new CarryChildren.PickUpChildOutside.Definition();

			// Token: 0x02000008 RID: 8
			public class Definition : SocialInteraction.SocialInteractionDefinition<CarryChildren.PickUpChildOutside>
			{
				// Token: 0x0600003E RID: 62 RVA: 0x00003880 File Offset: 0x00001A80
				public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
				{
					string text = "Gameplay/ActorSystems/Children:PickUp";
					bool baby = target.SimDescription.Baby;
					if (baby)
					{
						text += "Baby";
					}
					else
					{
						text += "Toddler";
					}
					return Localization.LocalizeString(actor.IsFemale, text, new object[]
					{
						target
					});
				}
                
				// Token: 0x0600003F RID: 63 RVA: 0x000038DC File Offset: 0x00001ADC
				public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return InteractionDefinitionUtilities.FromBool(CarryChildren.PickUpChild.Definition.CanPickUpBabyOrToddler(ref parameters));
				}

				// Token: 0x06000040 RID: 64 RVA: 0x000038FC File Offset: 0x00001AFC
				public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					InteractionInstanceParameters interactionInstanceParameters = new InteractionInstanceParameters(new InteractionObjectPair(CarryChildren.PickUpChildOutside.Singleton, target), actor, new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true);
					return InteractionDefinitionUtilities.IsPass(this.Test(ref interactionInstanceParameters, ref greyedOutTooltipCallback));
				}
			}
		}

		// Token: 0x02000005 RID: 5
		public class ChildUtilsCustom
		{
			// Token: 0x06000010 RID: 16 RVA: 0x00002864 File Offset: 0x00000A64
			public static void StartCarry(Sim parent, Sim child, StateMachineClient carryToddlerGraph, bool bAlreadySynchronized)
			{
				if (bAlreadySynchronized)
				{
					carryToddlerGraph.RequestState(null, "Carry");
				}
				else
				{
					carryToddlerGraph.RequestState(false, "x", "Carry");
					carryToddlerGraph.RequestState(true, "y", "Carry");
				}
				bool toddler = child.SimDescription.Toddler;
				if (toddler)
				{
					Slots.AttachToSlot(child.ObjectId, parent.ObjectId, (uint)CarryChildren.ChildUtilsCustom.ToddlerCarrySlot, true, false);
				}
				CarryingChildPosture carryingChildPosture = new CarryingChildPosture(parent, child, carryToddlerGraph);
				BeingCarriedPosture posture = new BeingCarriedPosture(parent, child, carryToddlerGraph);
				parent.Posture = carryingChildPosture;
				child.Posture = posture;
				carryingChildPosture.AddSocialBoost();
				child.SimRoutingComponent.DisableDynamicFootprint();
				carryingChildPosture.AddSocialBoost();
				EventTracker.SendEvent(EventTypeId.kPickedUpSim, parent, child);
			}

			// Token: 0x04000008 RID: 8
			public static Slot ToddlerCarrySlot = Sim.ContainmentSlots.RightCarry;
		}

		// Token: 0x02000006 RID: 6
		public class ChildUtils
		{
			// Token: 0x06000014 RID: 20 RVA: 0x00002944 File Offset: 0x00000B44
			public static StateMachineClient AcquireCarryStateMachine(Sim parent, Sim child)
			{
				StateMachineClient stateMachineClient = StateMachineClient.Acquire(parent, "CarryToddler");
				stateMachineClient.SetActor("x", parent);
				stateMachineClient.SetActor("y", child);
				stateMachineClient.EnterState("x", "Enter");
				stateMachineClient.EnterState("y", "Enter");
				return stateMachineClient;
			}

			// Token: 0x06000015 RID: 21 RVA: 0x000029A0 File Offset: 0x00000BA0
			public static void AddBuffToParentAndChild(Sim Actor, BuffNames buff, Origin origin)
			{
				Actor.BuffManager.AddElement(buff, origin);
				bool flag = Actor.CarryingChildPosture != null;
				if (flag)
				{
					Actor.CarryingChildPosture.Child.BuffManager.AddElement(buff, origin);
				}
			}

			// Token: 0x06000016 RID: 22 RVA: 0x000029E4 File Offset: 0x00000BE4
			public static List<BuffNames> AddStuffedAnimalSleepBuffsAsPaused(Sim sleeper)
			{
				BuffManager buffManager = sleeper.BuffManager;
				List<BuffNames> list = new List<BuffNames>();
				foreach (IStuffedAnimal stuffedAnimal in sleeper.Inventory.FindAll<IStuffedAnimal>(false))
				{
					BuffNames buffNames;
					Origin origin;
					bool flag = stuffedAnimal.ShouldAddBuffWhenKidSleeps(sleeper, out buffNames, out origin);
					if (flag)
					{
						buffManager.AddElementPaused(buffNames, origin);
						bool flag2 = !list.Contains(buffNames);
						if (flag2)
						{
							list.Add(buffNames);
						}
					}
				}
				return list;
			}

			// Token: 0x06000017 RID: 23 RVA: 0x00002A8C File Offset: 0x00000C8C
			public static bool CanCarryChild(InteractionObjectPair iop)
			{
				PreconditionOptions preconditionOptions = new PreconditionOptions(iop.Tuning.PosturePreconditions);
				return preconditionOptions.ContainsPosture(CommodityKind.CarryingChild);
			}

			// Token: 0x06000018 RID: 24 RVA: 0x00002ABC File Offset: 0x00000CBC
			public static void CarryChild(Sim Actor, Sim Child, bool startIdles)
			{
				StateMachineClient carryToddlerGraph = CarryChildren.ChildUtils.AcquireCarryStateMachine(Actor, Child);
				CarryChildren.ChildUtils.StartCarry(Actor, Child, carryToddlerGraph, false);
				if (startIdles)
				{
					Actor.LoopIdle();
					Child.SetPostureIdleBridgeOrigin(Child.Posture.Idle());
				}
				bool flag = Child.DeepSnowEffectManager != null;
				if (flag)
				{
					Child.DeepSnowEffectManager.StopAllDeepSnowEffects(true);
				}
			}

			// Token: 0x06000019 RID: 25 RVA: 0x00002B18 File Offset: 0x00000D18
			[Conditional("DEBUG_NOTIFY")]
			public static void DebugNotify(string format, params object[] parameters)
			{
				string titleText = string.Format(format, parameters);
				StyledNotification.Format format2 = new StyledNotification.Format(titleText, StyledNotification.NotificationStyle.kGameMessagePositive);
				StyledNotification.Show(format2);
			}

			// Token: 0x0600001A RID: 26 RVA: 0x00002B40 File Offset: 0x00000D40
			public static void EndRouteSittingCallback(Route r, RouteEvent re)
			{
				Sim sim = r.Follower.Target as Sim;
				WalkingToddlerPosture walkingToddlerPosture = sim.Posture as WalkingToddlerPosture;
				bool flag = walkingToddlerPosture != null;
				if (flag)
				{
					walkingToddlerPosture.EndRouteSitting();
				}
			}

			// Token: 0x0600001B RID: 27 RVA: 0x00002B7C File Offset: 0x00000D7C
			public static bool FinishInteractionWithCarriedChild(SocialInteraction interactionA)
			{
				interactionA.InstanceActor.LoopIdle();
				interactionA.FinishLinkedInteraction();
				interactionA.WaitForSyncComplete();
				return true;
			}

			// Token: 0x0600001C RID: 28 RVA: 0x00002BAC File Offset: 0x00000DAC
			public static bool FinishInteractionWithCarriedPet(SocialInteraction interactionA)
			{
				return CarryChildren.ChildUtils.FinishInteractionWithCarriedChild(interactionA);
			}

			// Token: 0x0600001D RID: 29 RVA: 0x00002BC4 File Offset: 0x00000DC4
			public static bool FinishInteractionWithChildOnFloor(SocialInteraction interactionA)
			{
				interactionA.FinishLinkedInteraction();
				interactionA.WaitForSyncComplete();
				return true;
			}

			// Token: 0x0600001E RID: 30 RVA: 0x00002BE8 File Offset: 0x00000DE8
			public static bool FinishObjectInteractionWithChild(InteractionInstance interactionA, Sim child)
			{
				interactionA.FinishLinkedInteraction();
				interactionA.WaitForSyncComplete();
				return true;
			}

			// Token: 0x0600001F RID: 31 RVA: 0x00002C0C File Offset: 0x00000E0C
			public static bool FixToddlerPosture(Sim Actor, CommodityKind targetPosture, GameObject target)
			{
				bool flag = Actor.Posture.Satisfies(targetPosture, target);
				bool result;
				if (flag)
				{
					result = true;
				}
				else
				{
					bool flag2 = targetPosture == CommodityKind.Standing;
					if (flag2)
					{
						Actor.LoopIdle();
					}
					else
					{
						bool flag3 = targetPosture == CommodityKind.WalkingToddler;
						if (flag3)
						{
							WalkingToddlerPosture walkingToddlerPosture = WalkingToddlerPosture.Create(Actor);
							walkingToddlerPosture.CurrentStateMachine.EnterState("x", "Crawling");
							walkingToddlerPosture.CurrentStateMachine.RequestState("x", "Walking");
							Actor.Posture = walkingToddlerPosture;
						}
					}
					result = Actor.Posture.Satisfies(targetPosture, target);
				}
				return result;
			}

			// Token: 0x06000020 RID: 32 RVA: 0x00002CA8 File Offset: 0x00000EA8
			public static bool InteractionMayRouteBabyOrToddlerOffHomeLot(InteractionInstance interactionInstance)
			{
				return interactionInstance is Pregnancy.HaveBabyHospital;
			}

			// Token: 0x06000021 RID: 33 RVA: 0x00002CC4 File Offset: 0x00000EC4
			public static string Localize(bool isFemaleString, string key, params object[] parameters)
			{
				return Localization.LocalizeString(isFemaleString, CarryChildren.ChildUtils.kLocalizeKey + ":" + key, parameters);
			}

			// Token: 0x06000022 RID: 34 RVA: 0x00002CF0 File Offset: 0x00000EF0
			public static bool LotContainsToddler(Lot lot)
			{
				foreach (Sim sim in lot.GetSims())
				{
					bool toddler = sim.SimDescription.Toddler;
					if (toddler)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x06000023 RID: 35 RVA: 0x00002D54 File Offset: 0x00000F54
			public static void PickUpChild(Sim parent, Sim child, SocialJig jig, string angle)
			{
				parent.PopCanePostureIfNecessary();
				parent.PopBackpackPostureIfNecessary();
				parent.PopJetpackPostureIfNecessary();
				bool flag = child.DeepSnowEffectManager != null;
				if (flag)
				{
					child.DeepSnowEffectManager.StopAllDeepSnowEffects(false);
				}
				StateMachineClient stateMachineClient = CarryChildren.ChildUtils.AcquireCarryStateMachine(parent, child);
				stateMachineClient.SetActor("socialTemplate", jig);
				stateMachineClient.SetParameter("Angle", angle);
				stateMachineClient.RequestState(false, "y", "PickUp");
				stateMachineClient.RequestState(true, "x", "PickUp");
				stateMachineClient.RemoveActor("socialTemplate");
				CarryChildren.ChildUtils.StartCarry(parent, child, stateMachineClient, true);
			}

			// Token: 0x06000024 RID: 36 RVA: 0x00002DF0 File Offset: 0x00000FF0
			public static void PrepareToddlerForPossibleWalkingRoute(Sim Actor)
			{
				bool flag = CarryChildren.ChildUtils.ToddlerCanWalk(Actor);
				if (flag)
				{
					CarryChildren.ChildUtils.FixToddlerPosture(Actor, CommodityKind.WalkingToddler, Actor);
				}
			}

			// Token: 0x06000025 RID: 37 RVA: 0x00002E17 File Offset: 0x00001017
			public static void ReenqueueWithCarryingChildPrecondition(InteractionInstance interaction)
			{
				CarryChildren.ChildUtils.SetPosturePrecondition(interaction, CommodityKind.CarryingChild, new CommodityKind[]
				{
					CommodityKind.IsTarget
				});
				interaction.InstanceActor.InteractionQueue.PushAsContinuation(interaction, false);
			}

			// Token: 0x06000026 RID: 38 RVA: 0x00002E48 File Offset: 0x00001048
			public static void SetPosturePrecondition(InteractionInstance interaction, CommodityKind posture, params CommodityKind[] checks)
			{
				PosturePreconditionSetData posturePreconditionSetData = new PosturePreconditionSetData(posture, 1f);
				foreach (CommodityKind postureCheck in checks)
				{
					posturePreconditionSetData.AddCheck(postureCheck);
				}
				PosturePreconditionOptionsData posturePreconditionOptionsData = new PosturePreconditionOptionsData();
				posturePreconditionOptionsData.AddOption(posturePreconditionSetData);
				interaction.PosturePreconditions = new PreconditionOptions(posturePreconditionOptionsData);
			}

			// Token: 0x06000027 RID: 39 RVA: 0x00002EA0 File Offset: 0x000010A0
			public static bool ShouldApplyAutonomousOffLotToddlerRoutingRules(Sim parent, InteractionInstance next)
			{
				bool flag = !next.Autonomous;
				if (flag)
				{
					bool flag2 = next.InteractionDefinition is IAlwaysUseAutonomousOffLotToddlerRoutingRules;
					if (flag2)
					{
						return true;
					}
					bool flag3 = !parent.IsNPC;
					if (flag3)
					{
						return CaregiverRoutingMonitor.TreatPlayerSimsLikeNPCs;
					}
				}
				return true;
			}

			// Token: 0x06000028 RID: 40 RVA: 0x00002EF0 File Offset: 0x000010F0
			public static bool SimCanTalk(Sim Actor)
			{
				CASAgeGenderFlags age = Actor.SimDescription.Age;
				return age != CASAgeGenderFlags.Baby && (age != CASAgeGenderFlags.Toddler || CarryChildren.ChildUtils.ToddlerCanTalk(Actor));
			}

			// Token: 0x06000029 RID: 41 RVA: 0x00002F28 File Offset: 0x00001128
			public static void StartCarry(Sim parent, Sim child, StateMachineClient carryToddlerGraph, bool bAlreadySynchronized)
			{
				if (bAlreadySynchronized)
				{
					carryToddlerGraph.RequestState(null, "Carry");
				}
				else
				{
					carryToddlerGraph.RequestState(false, "x", "Carry");
					carryToddlerGraph.RequestState(true, "y", "Carry");
				}
				bool flag = child.SimDescription.Toddler || child.SimDescription.Child;
				if (flag)
				{
					Slots.AttachToSlot(child.ObjectId, parent.ObjectId, (uint)CarryChildren.ChildUtils.ToddlerCarrySlot, true, false);
				}
				CarryingChildPosture carryingChildPosture = new CarryingChildPosture(parent, child, carryToddlerGraph);
				BeingCarriedPosture posture = new BeingCarriedPosture(parent, child, carryToddlerGraph);
				parent.Posture = carryingChildPosture;
				child.Posture = posture;
				carryingChildPosture.AddSocialBoost();
				child.SimRoutingComponent.DisableDynamicFootprint();
				carryingChildPosture.AddSocialBoost();
				EventTracker.SendEvent(EventTypeId.kPickedUpSim, parent, child);
			}

			// Token: 0x0600002A RID: 42 RVA: 0x00002FF8 File Offset: 0x000011F8
			public static bool StartInteractionWithCarriedChild(SocialInteraction interactionA, SocialInteractionB interactionB)
			{
				interactionB.LinkedInteractionInstance = interactionA;
				interactionA.Target.InteractionQueue.Add(interactionB);
				bool flag = !interactionB.CancelNonBSocialInteractionsFromQueue(interactionB.Actor);
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					interactionA.SetInitialRouteComplete();
					while (interactionA.Target.CurrentInteraction != interactionB)
					{
						bool flag2 = interactionA.InstanceActor.HasExitReason() || !interactionA.Target.InteractionQueue.HasInteraction(interactionB) || !interactionA.Test();
						if (flag2)
						{
							return false;
						}
						Simulator.Sleep(0u);
					}
					interactionA.Actor.SynchronizationRole = Sim.SyncRole.Initiator;
					interactionA.Actor.SynchronizationTarget = interactionA.Target;
					interactionA.Actor.SynchronizationLevel = Sim.SyncLevel.Started;
					bool flag3 = !interactionA.Actor.WaitForSynchronizationLevelWithSim(interactionA.Target, Sim.SyncLevel.Started, 10f);
					if (flag3)
					{
						interactionA.Actor.ClearSynchronizationData();
						result = false;
					}
					else
					{
						interactionA.Actor.SynchronizationLevel = Sim.SyncLevel.Committed;
						bool flag4 = !interactionA.Actor.WaitForSynchronizationLevelWithSim(interactionA.Target, Sim.SyncLevel.Committed, 10f);
						if (flag4)
						{
							interactionA.Actor.ClearSynchronizationData();
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
				return result;
			}

			// Token: 0x0600002B RID: 43 RVA: 0x00003138 File Offset: 0x00001338
			public static bool StartInteractionWithCarriedChild(SocialInteraction interactionA, string receptiveInteractionNameKey)
			{
				SocialInteractionB interactionB = new CarriedChildInteractionB.Definition(CarryChildren.ChildUtils.Localize(interactionA.Target.IsFemale, receptiveInteractionNameKey, new object[0])).CreateInstance(interactionA.Actor, interactionA.Target, interactionA.GetPriority(), interactionA.EffectivelyAutonomous, interactionA.CancellableByPlayer) as SocialInteractionB;
				return CarryChildren.ChildUtils.StartInteractionWithCarriedChild(interactionA, interactionB);
			}

			// Token: 0x0600002C RID: 44 RVA: 0x00003198 File Offset: 0x00001398
			public static bool StartInteractionWithCarriedPet(SocialInteraction interactionA, string receptiveInteractionNameKey)
			{
				SocialInteractionB interactionB = new CarriedChildInteractionB.Definition(Localization.LocalizeString(interactionA.Target.IsFemale, "Gameplay/Excel/Socializing/Action:" + receptiveInteractionNameKey, new object[0])).CreateInstance(interactionA.Actor, interactionA.Target, interactionA.GetPriority(), interactionA.EffectivelyAutonomous, interactionA.CancellableByPlayer) as SocialInteractionB;
				return CarryChildren.ChildUtils.StartInteractionWithCarriedChild(interactionA, interactionB);
			}

			// Token: 0x0600002D RID: 45 RVA: 0x00003200 File Offset: 0x00001400
			public static CarryChildren.ChildUtils.Return StartInteractionWithChildOnFloor(SocialInteraction interactionA, string receptiveInteractionNameKey)
			{
				bool flag = interactionA.Target.Posture.Container == interactionA.Actor;
				CarryChildren.ChildUtils.Return result;
				if (flag)
				{
					bool flag2 = !CarryChildren.ChildUtils.StartInteractionWithCarriedChild(interactionA, receptiveInteractionNameKey);
					if (flag2)
					{
						result = CarryChildren.ChildUtils.Return.False;
					}
					else
					{
						bool flag3 = interactionA.SocialJig == null;
						if (flag3)
						{
							interactionA.SocialJig = (GlobalFunctions.CreateObjectOutOfWorld("SocialJigBabyToddler") as SocialJigTwoPerson);
						}
						CarryChildren.ChildUtils.PutDownClass putDownClass = new CarryChildren.ChildUtils.PutDownClass(interactionA.Actor, interactionA.Target, CarryChildren.ChildUtils.Where.ClosestSpot, Vector3.Zero, null, interactionA.SocialJig as SocialJigTwoPerson);
						bool flag4 = !putDownClass.PutDownChild();
						if (flag4)
						{
							interactionA.FinishLinkedInteraction();
							interactionA.WaitForSyncComplete();
							result = CarryChildren.ChildUtils.Return.False;
						}
						else
						{
							result = CarryChildren.ChildUtils.Return.Continue;
						}
					}
				}
				else
				{
					bool flag5 = !interactionA.Target.Posture.Satisfies(CommodityKind.Standing, null);
					if (flag5)
					{
						bool teenOrAbove = interactionA.Actor.SimDescription.TeenOrAbove;
						if (teenOrAbove)
						{
							CarryChildren.ChildUtils.ReenqueueWithCarryingChildPrecondition(interactionA);
							result = CarryChildren.ChildUtils.Return.True;
						}
						else
						{
							result = CarryChildren.ChildUtils.Return.False;
						}
					}
					else
					{
						bool flag6 = !interactionA.BeginSocialInteraction(new SocialInteractionB.NoAgeCheckDefinition(CarryChildren.ChildUtils.Localize(interactionA.Target.IsFemale, receptiveInteractionNameKey, new object[0]), false), true, false);
						if (flag6)
						{
							result = CarryChildren.ChildUtils.Return.False;
						}
						else
						{
							result = CarryChildren.ChildUtils.Return.Continue;
						}
					}
				}
				return result;
			}

			// Token: 0x0600002E RID: 46 RVA: 0x00003338 File Offset: 0x00001538
			public static bool StartObjectInteractionWithChild(InteractionInstance interactionA, Sim child, CommodityKind childPosture, string receptiveInteractionNameKey)
			{
				bool flag = child == null || child.HasBeenDestroyed;
				bool result;
				if (flag)
				{
					result = false;
				}
				else
				{
					bool flag2 = !interactionA.SafeToSync();
					if (flag2)
					{
						result = false;
					}
					else
					{
						Sim instanceActor = interactionA.InstanceActor;
						InteractionInstance interactionInstance = new CarryChildren.ChildUtils.ChildPlaceholderInteraction.Definition(CarryChildren.ChildUtils.Localize(child.IsFemale, receptiveInteractionNameKey, new object[0])).CreateInstance(instanceActor, child, interactionA.GetPriority(), interactionA.Autonomous, interactionA.CancellableByPlayer);
						interactionInstance.LinkedInteractionInstance = interactionA;
						CarryChildren.ChildUtils.SetPosturePrecondition(interactionInstance, childPosture, new CommodityKind[0]);
						bool flag3 = !child.InteractionQueue.Add(interactionInstance);
						if (flag3)
						{
							result = false;
						}
						else
						{
							bool flag4 = !interactionA.StartSync(true);
							if (flag4)
							{
								result = false;
							}
							else
							{
								instanceActor.SynchronizationLevel = Sim.SyncLevel.Committed;
								bool flag5 = !instanceActor.WaitForSynchronizationLevelWithSim(child, Sim.SyncLevel.Committed, SocialInteraction.kSocialSyncGiveupTime);
								if (flag5)
								{
									instanceActor.ClearSynchronizationData();
									result = false;
								}
								else
								{
									result = true;
								}
							}
						}
					}
				}
				return result;
			}

			// Token: 0x0600002F RID: 47 RVA: 0x00003420 File Offset: 0x00001620
			public static bool TestInteractionWithChildOnFloor(Sim actor, Sim target, bool isAutonomous)
			{
				return target.Posture.Satisfies(CommodityKind.Standing, null) || actor.SimDescription.TeenOrAbove;
			}

			// Token: 0x06000030 RID: 48 RVA: 0x00003454 File Offset: 0x00001654
			public static bool ToddlerCanTalk(Sim Actor)
			{
				return 0 < Actor.SkillManager.GetSkillLevel(SkillNames.LearnToTalk);
			}

			// Token: 0x06000031 RID: 49 RVA: 0x00003478 File Offset: 0x00001678
			public static bool ToddlerCanWalk(Sim Actor)
			{
				return 0 < Actor.SkillManager.GetSkillLevel(SkillNames.LearnToWalk);
			}

			// Token: 0x06000032 RID: 50 RVA: 0x0000349C File Offset: 0x0000169C
			public static void UnpauseStuffedAnimalSleepBuffs(Sim sleeper, List<BuffNames> buffs)
			{
				bool flag = buffs != null;
				if (flag)
				{
					BuffManager buffManager = sleeper.BuffManager;
					foreach (BuffNames buffName in buffs)
					{
						buffManager.UnpauseBuff(buffName);
					}
				}
			}

			// Token: 0x06000033 RID: 51 RVA: 0x00003504 File Offset: 0x00001704
			public static void UpdateRelationshipWithChild(Sim actor, Sim child, float relationshipChange)
			{
				Relationship relationship = Relationship.Get(actor, child, true);
				LongTermRelationshipTypes currentLTR = relationship.CurrentLTR;
				relationship.LTR.UpdateLiking(relationshipChange);
				LongTermRelationshipTypes currentLTR2 = relationship.CurrentLTR;
				SocialComponent.SetSocialFeedback(CommodityTypes.Friendly, actor, true, 0, currentLTR2, currentLTR);
				SocialComponent.SetSocialFeedback(CommodityTypes.Friendly, child, true, 0, currentLTR2, currentLTR);
			}

			// Token: 0x04000009 RID: 9
			public static string kLocalizeKey = "Gameplay/ActorSystems/Children";

			// Token: 0x0400000A RID: 10
			public static Slot ToddlerCarrySlot = Sim.ContainmentSlots.RightCarry;

			// Token: 0x02000009 RID: 9
			public class ChildPlaceholderInteraction : Interaction<Sim, Sim>
			{
				// Token: 0x06000042 RID: 66 RVA: 0x00003944 File Offset: 0x00001B44
				public override bool Run()
				{
					bool flag = !base.StartSync(false);
					bool result;
					if (flag)
					{
						result = false;
					}
					else
					{
						this.Actor.SynchronizationLevel = Sim.SyncLevel.Committed;
						bool flag2 = !this.Actor.WaitForSynchronizationLevelWithSim(this.Target, Sim.SyncLevel.Committed, SocialInteraction.kSocialSyncGiveupTime);
						if (flag2)
						{
							result = false;
						}
						else
						{
							bool flag3 = base.DoLoop(ExitReason.Default);
							base.CopyExitReasonToLinkedInteraction();
							base.WaitForMasterInteractionToFinish();
							result = (base.WaitForSyncComplete() && flag3);
						}
					}
					return result;
				}

				// Token: 0x0200000D RID: 13
				[DoesntRequireTuning]
				public class Definition : InteractionDefinition<Sim, Sim, CarryChildren.ChildUtils.ChildPlaceholderInteraction>, IUsableDuringFire, IOverridesAgeTests
				{
					// Token: 0x0600004A RID: 74 RVA: 0x00004353 File Offset: 0x00002553
					public Definition()
					{
					}

					// Token: 0x0600004B RID: 75 RVA: 0x0000435D File Offset: 0x0000255D
					public Definition(string localizedName)
					{
						this.Name = localizedName;
					}

					// Token: 0x0600004C RID: 76 RVA: 0x00004370 File Offset: 0x00002570
					public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
					{
						return this.Name;
					}

					// Token: 0x0600004D RID: 77 RVA: 0x00004388 File Offset: 0x00002588
					public SpecialCaseAgeTests GetSpecialCaseAgeTests()
					{
						return SpecialCaseAgeTests.None;
					}

					// Token: 0x0600004E RID: 78 RVA: 0x0000439C File Offset: 0x0000259C
					public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
					{
						return true;
					}

					// Token: 0x0400001C RID: 28
					public string Name;
				}
			}

			// Token: 0x0200000A RID: 10
			[Persistable]
			public class PutDownClass
			{
				// Token: 0x06000044 RID: 68 RVA: 0x000039C3 File Offset: 0x00001BC3
				public PutDownClass()
				{
					this.mStrategies = GlobalFunctions.FindGoodLocationStrategies.All;
				}

				// Token: 0x06000045 RID: 69 RVA: 0x000039D5 File Offset: 0x00001BD5
				public PutDownClass(Sim Adult, Sim Child, CarryChildren.ChildUtils.Where putDownSpot, Vector3 putDownTargetSpot, InteractionDefinition socialToPushToTarget, SocialJigTwoPerson jig)
				{
					this.mStrategies = GlobalFunctions.FindGoodLocationStrategies.All;
					this.Actor = Adult;
					this.Target = Child;
					this.mPutDownType = putDownSpot;
					this.mPutDownTargetLocation = putDownTargetSpot;
					this.mSocialToPushToTarget = socialToPushToTarget;
					this.SocialJig = jig;
				}

				// Token: 0x06000046 RID: 70 RVA: 0x00003A14 File Offset: 0x00001C14
				public PutDownClass(Sim Adult, Sim Child, CarryChildren.ChildUtils.Where putDownSpot, Vector3 putDownTargetSpot, InteractionDefinition socialToPushToTarget, SocialJigTwoPerson jig, GlobalFunctions.FindGoodLocationStrategies strategies) : this(Adult, Child, putDownSpot, putDownTargetSpot, socialToPushToTarget, jig)
				{
					this.mStrategies = strategies;
				}

				// Token: 0x06000047 RID: 71 RVA: 0x00003A30 File Offset: 0x00001C30
				public void PushSocialB(SocialInteraction socialInteraction)
				{
					bool flag = this.mSocialToPushToTarget != null;
					if (flag)
					{
						SocialInteractionB socialInteractionB = this.mSocialToPushToTarget.CreateInstance(socialInteraction.Actor, socialInteraction.Target, socialInteraction.GetPriority(), socialInteraction.Autonomous, socialInteraction.CancellableByPlayer) as SocialInteractionB;
						bool flag2 = socialInteractionB != null;
						if (flag2)
						{
							socialInteraction.LinkedInteractionInstance = null;
							socialInteraction.LinkedInteractionInstance = socialInteractionB;
							socialInteractionB.SocialJig = this.SocialJig;
							socialInteraction.Target.InteractionQueue.Add(socialInteractionB);
						}
					}
				}

				// Token: 0x06000048 RID: 72 RVA: 0x00003AB4 File Offset: 0x00001CB4
				public bool PutDownChild()
				{
					CarryingChildPosture carryingChildPosture = this.Actor.CarryingChildPosture;
					bool flag = carryingChildPosture == null;
					bool result;
					if (flag)
					{
						result = false;
					}
					else
					{
						CarryChildren.ChildUtils.PutDownClass.Phase phase = CarryChildren.ChildUtils.PutDownClass.Phase.Nearby;
						bool flag2 = false;
						this.mPutDownActualLocation = Vector3.Zero;
						this.mPutDownActualForward = Vector3.UnitZ;
						while (phase <= CarryChildren.ChildUtils.PutDownClass.Phase.Corner3 && !flag2)
						{
							this.Actor.RemoveExitReason(ExitReason.RouteFailed);
							bool flag3 = this.Actor.HasExitReason(ExitReason.Default);
							if (flag3)
							{
								break;
							}
							switch (this.mPutDownType)
							{
							case CarryChildren.ChildUtils.Where.BestSpot:
							case CarryChildren.ChildUtils.Where.ClosestSpot:
								this.mPutDownActualLocation = this.Actor.Position;
								this.mPutDownActualForward = this.Actor.ForwardVector;
								break;
							case CarryChildren.ChildUtils.Where.SpecificSpot:
								this.mPutDownActualLocation = World.SnapToFloor(this.mPutDownTargetLocation);
								this.mPutDownActualForward = this.mPutDownActualLocation - this.Actor.Position;
								this.mPutDownActualForward.y = 0f;
								this.mPutDownActualForward.Normalize();
								break;
							}
							bool flag4 = this.Actor.LotCurrent.IsFireOnLot() && !this.Actor.IsActiveFirefighter;
							if (flag4)
							{
								Route route = this.Actor.CreateRoute();
								route.PlanToPointRadialRange(this.Actor.LotCurrent.FireManager.FireCentroid, FireManager.kMinBabyPutDownDistance, FireManager.kMaxBabyPutDownDistance, RouteDistancePreference.PreferFurthestFromRouteDestination, RouteOrientationPreference.NoPreference, this.Actor.LotCurrent.LotId, null);
								bool flag5 = route.PlanResult.Succeeded();
								if (flag5)
								{
									this.mPutDownActualLocation = route.PlanResult.mDestination;
								}
							}
							bool flag6 = true;
							bool tempPlacement = true;
							switch (phase)
							{
							case CarryChildren.ChildUtils.PutDownClass.Phase.Nearby:
							{
								flag6 = false;
								bool flag7 = this.mPutDownType == CarryChildren.ChildUtils.Where.SpecificSpot;
								if (flag7)
								{
									phase = CarryChildren.ChildUtils.PutDownClass.Phase.Corner3;
								}
								break;
							}
							case CarryChildren.ChildUtils.PutDownClass.Phase.FrontDoor:
							{
								Door door = this.Actor.LotCurrent.FindFrontDoor();
								bool flag8 = door != null;
								if (flag8)
								{
									flag6 = false;
									tempPlacement = false;
									this.mPutDownActualLocation = door.Position;
									this.mPutDownActualForward = door.ForwardVector;
								}
								break;
							}
							case CarryChildren.ChildUtils.PutDownClass.Phase.Mailbox:
							{
								Mailbox mailbox = this.Actor.LotCurrent.FindMailbox();
								bool flag9 = mailbox != null;
								if (flag9)
								{
									flag6 = false;
									this.mPutDownActualLocation = mailbox.Position;
									this.mPutDownActualForward = mailbox.ForwardVector;
								}
								break;
							}
							case CarryChildren.ChildUtils.PutDownClass.Phase.Corner0:
							case CarryChildren.ChildUtils.PutDownClass.Phase.Corner1:
							case CarryChildren.ChildUtils.PutDownClass.Phase.Corner2:
							case CarryChildren.ChildUtils.PutDownClass.Phase.Corner3:
								flag6 = false;
								this.mPutDownActualLocation = this.Actor.LotCurrent.Corners[phase - CarryChildren.ChildUtils.PutDownClass.Phase.Corner0];
								this.mPutDownActualForward = Vector3.UnitZ;
								break;
							}
							bool flag10 = this.mPutDownActualForward.LengthSqr() < 0.01f;
							if (flag10)
							{
								this.mPutDownActualForward = this.Actor.ForwardVector;
							}
							bool flag11 = flag6;
							if (flag11)
							{
								this.mPutDownActualLocation = Vector3.Zero;
							}
							else
							{
								bool flag12 = !GlobalFunctions.FindGoodLocationNearby(this.SocialJig, ref this.mPutDownActualLocation, ref this.mPutDownActualForward, this.mStrategies, tempPlacement);
								if (flag12)
								{
									Simulator.Sleep(0u);
									this.mPutDownActualLocation = Vector3.Zero;
								}
							}
							bool flag13 = this.mPutDownActualLocation != Vector3.Zero;
							if (flag13)
							{
								this.SocialJig.SetPosition(this.mPutDownActualLocation);
								this.SocialJig.SetForward(this.mPutDownActualForward);
								this.SocialJig.SetOpacity(0f, 0f);
								this.SocialJig.AddToWorld();
								bool flag14 = (this.Actor.Position - this.mPutDownActualLocation).Length() < 0.1f;
								if (flag14)
								{
									Route route2 = this.Actor.RoutingComponent.CreateRouteTurnToFace(this.SocialJig.GetSlotPosition(SocialJigTwoPerson.RoutingSlots.SimB));
									route2.AddObjectToIgnoreForRoute(this.SocialJig.ObjectId);
									flag2 = this.Actor.DoRoute(route2);
								}
								else
								{
									Route route3 = this.SocialJig.RouteToJigA(this.Actor);
									route3.DoRouteFail = (phase == CarryChildren.ChildUtils.PutDownClass.Phase.Corner3);
									flag2 = this.Actor.DoRoute(route3);
								}
							}
							phase++;
						}
						bool flag15 = !flag2;
						if (flag15)
						{
							result = false;
						}
						else
						{
							carryingChildPosture.RemoveSocialBoost();
							bool toddler = this.Target.SimDescription.Toddler;
							if (toddler)
							{
								Slots.DetachFromSlot(this.Target.ObjectId);
								this.Target.SetPosition(this.SocialJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimB));
								this.Target.SetForward(this.SocialJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB));
							}
							carryingChildPosture.CurrentStateMachine.SetActor("socialTemplate", this.SocialJig);
							carryingChildPosture.CurrentStateMachine.RequestState(null, "PutDown");
							carryingChildPosture.CurrentStateMachine.RequestState(null, "Exit");
							carryingChildPosture.CurrentStateMachine.RemoveActor("socialTemplate");
							bool baby = this.Target.SimDescription.Baby;
							if (baby)
							{
								this.Target.UnParent();
								this.Target.SetPosition(this.SocialJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimB));
								this.Target.SetForward(this.SocialJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB));
								bool flag16 = DeepSnowEffectManager.ShouldPlayDeepSnowEffectsNow(this.Target);
								if (flag16)
								{
									this.Target.DeepSnowEffectManager.PlayDeepSnowEffect(false, true);
								}
							}
							this.Target.Posture = this.Target.Standing;
							this.Actor.Posture = this.Actor.Standing;
							this.Actor.SimRoutingComponent.EnableDynamicFootprint();
							this.Actor.SimRoutingComponent.ForceUpdateDynamicFootprint();
							this.Target.SimRoutingComponent.EnableDynamicFootprint();
							this.Target.SimRoutingComponent.ForceUpdateDynamicFootprint();
							bool flag17 = this.mSocialToPushToTarget != null;
							if (flag17)
							{
								SocialInteraction socialInteraction = (this.Actor.InteractionQueue.TransitionInteraction ?? this.Actor.CurrentInteraction) as SocialInteraction;
								socialInteraction.FinishLinkedInteraction();
								socialInteraction.WaitForSyncComplete();
								this.Actor.ClearExitReasons();
								this.PushSocialB(socialInteraction);
								this.Actor.SynchronizationRole = Sim.SyncRole.Initiator;
								this.Actor.SynchronizationLevel = Sim.SyncLevel.Started;
								this.Actor.SynchronizationTarget = this.Target;
								bool flag18 = !this.Actor.WaitForSynchronizationLevelWithSim(this.Target, Sim.SyncLevel.Started, 15f);
								if (flag18)
								{
									return false;
								}
								this.Actor.SynchronizationLevel = Sim.SyncLevel.Routed;
								bool flag19 = !this.Actor.WaitForSynchronizationLevelWithSim(this.Target, Sim.SyncLevel.Routed, 15f);
								if (flag19)
								{
									return false;
								}
								this.Actor.SynchronizationLevel = Sim.SyncLevel.Committed;
								bool flag20 = !this.Actor.WaitForSynchronizationLevelWithSim(this.Target, Sim.SyncLevel.Committed, 15f);
								if (flag20)
								{
									return false;
								}
							}
							result = true;
						}
					}
					return result;
				}

				// Token: 0x06000049 RID: 73 RVA: 0x000041BC File Offset: 0x000023BC
				public void RouteCallbackPairedSocial(Route r, RouteEvent re)
				{
					Sim sim = r.Follower.Target as Sim;
					bool flag = sim != null;
					if (flag)
					{
						Sim target = this.Target;
						bool flag2 = target == null;
						if (flag2)
						{
							sim.LogDebugOutput(sim.Name, "Socializing", "Interaction", "Failed because target Sim does not exist", "");
							sim.AddExitReason(ExitReason.RouteFailed);
						}
						else
						{
							bool flag3 = this.SocialJig != null;
							if (flag3)
							{
								bool flag4 = !SocialComponent.DebugShowSocialJig;
								if (flag4)
								{
									this.SocialJig.SetOpacity(0f, 0f);
								}
								this.SocialJig.AddToWorld();
								Vector3 vector;
								bool destPoint = r.GetDestPoint(out vector);
								bool flag5;
								if (destPoint)
								{
									flag5 = this.SocialJig.PlaceJigAlongRoute(r, this.mPutDownActualLocation);
								}
								else
								{
									flag5 = this.SocialJig.PlaceJig(sim, target);
								}
								bool flag6 = flag5;
								if (flag6)
								{
									r = this.SocialJig.RouteToJigA(sim);
								}
								else
								{
									sim.LogDebugOutput(sim.Name, "Socializing", "Interaction", "Failed because jig couldn't be placed", "");
									sim.AddExitReason(ExitReason.RouteFailed);
									target.LogDebugOutput(target.Name, "Socializing", "Interaction", "Failed because jig couldn't be placed", "");
								}
							}
							else
							{
								sim.LogDebugOutput(sim.Name, "Socializing", "Interaction", "Failed because jig is null", "");
								sim.LogDebugOutput(sim.Name, "Socializing", "Interaction", "Failed because jig is null", "");
							}
						}
					}
				}

				// Token: 0x0400000B RID: 11
				public Sim Actor;

				// Token: 0x0400000C RID: 12
				public Sim Target;

				// Token: 0x0400000D RID: 13
				public CarryChildren.ChildUtils.Where mPutDownType;

				// Token: 0x0400000E RID: 14
				public Vector3 mPutDownTargetLocation;

				// Token: 0x0400000F RID: 15
				public Vector3 mPutDownActualLocation;

				// Token: 0x04000010 RID: 16
				public Vector3 mPutDownActualForward;

				// Token: 0x04000011 RID: 17
				public InteractionDefinition mSocialToPushToTarget;

				// Token: 0x04000012 RID: 18
				public SocialJigTwoPerson SocialJig;

				// Token: 0x04000013 RID: 19
				public GlobalFunctions.FindGoodLocationStrategies mStrategies;

				// Token: 0x0200000E RID: 14
				public enum Phase
				{
					// Token: 0x0400001E RID: 30
					Nearby,
					// Token: 0x0400001F RID: 31
					FrontDoor,
					// Token: 0x04000020 RID: 32
					Mailbox,
					// Token: 0x04000021 RID: 33
					Corner0,
					// Token: 0x04000022 RID: 34
					Corner1,
					// Token: 0x04000023 RID: 35
					Corner2,
					// Token: 0x04000024 RID: 36
					Corner3,
					// Token: 0x04000025 RID: 37
					First = 0,
					// Token: 0x04000026 RID: 38
					Last = 6
				}
			}

			// Token: 0x0200000B RID: 11
			public enum Return
			{
				// Token: 0x04000015 RID: 21
				False,
				// Token: 0x04000016 RID: 22
				True,
				// Token: 0x04000017 RID: 23
				Continue
			}

			// Token: 0x0200000C RID: 12
			public enum Where
			{
				// Token: 0x04000019 RID: 25
				BestSpot,
				// Token: 0x0400001A RID: 26
				ClosestSpot,
				// Token: 0x0400001B RID: 27
				SpecificSpot
			}
		}
	}
}
