using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using NiecMod.Interactions.Objects;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Academics;
using Sims3.Gameplay.ActiveCareer;
using Sims3.Gameplay.ActiveCareer.ActiveCareers;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Actors;
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
using Sims3.Gameplay.Objects.Decorations;
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
//using NiecAutoKill.KillSimNiec;
using NiecMod.KillNiec;
using NiecMod.CopMod;
using NiecMod.Nra;


namespace Sims3.Gameplay.Objects.Niec
{
    /// <summary>
    /// Test
    /// </summary>
    public class NiecAutoKill : GameObject
    {

        public enum AutoNiecKill { AutoHouseNiecKill };

        //public enum InteractionPriorityLevel{};

        // Methods
        public override void OnStartup()
        {
            base.OnStartup();

            base.AddInteraction(KillSimNiec.Singleton);
            
            base.AddInteraction(NiecObjectAsktoAdd.Singleton);

            base.AddInventoryInteraction(KillSimNiec.Singleton);
            
            base.AddInventoryInteraction(NiecObjectAsktoAdd.Singleton);
            
		}
        public void ApplyRandomBuff(Sim actor, JellyBeanBush.JellyBeanBuffType buffType)
		{
			BuffNames buffNames = BuffNames.Undefined;
			switch (buffType)
			{
			case JellyBeanBush.JellyBeanBuffType.NormalPositive:
				buffNames = RandomUtil.GetRandomObjectFromList<BuffNames>(JellyBeanBush.kPositiveBuffs);
				break;
			case JellyBeanBush.JellyBeanBuffType.NormalNegative:
				buffNames = RandomUtil.GetRandomObjectFromList<BuffNames>(JellyBeanBush.kNegativeBuffs);
				break;
			case JellyBeanBush.JellyBeanBuffType.TransPositive:
				buffNames = RandomUtil.GetRandomObjectFromList<BuffNames>(JellyBeanBush.kPosTransBuffs);
				break;
			case JellyBeanBush.JellyBeanBuffType.TransNegative:
				buffNames = RandomUtil.GetRandomObjectFromList<BuffNames>(JellyBeanBush.kNegTransBuffs);
				break;
			}
			if (buffNames == BuffNames.Undefined)
			{
				return;
			}
			if (!actor.BuffManager.AddElement(buffNames, Origin.FromMagicJellyBean))
			{
				return;
			}
			BuffInstance element = actor.BuffManager.GetElement(buffNames);
			if (element == null)
			{
				return;
			}
			ThoughtBalloonManager.BalloonData balloonData;
			if (element.ThumbString == "moodlet_terrified")
			{
				balloonData = new ThoughtBalloonManager.BalloonData("ep7_balloon_moodlet_terrified");
			}
			else if (element.ThumbString == "moodlet_blueblitz")
			{
				balloonData = new ThoughtBalloonManager.BalloonData("ep7_balloon_moodlet_blueblitz");
			}
			else if (element.ThumbString == "moodlet_allglowyontheoutside")
			{
				balloonData = new ThoughtBalloonManager.BalloonData("ep7_balloon_moodlet_allglowyontheoutside");
			}
			else
			{
				balloonData = new ThoughtBalloonManager.BalloonData(element.ThumbString);
			}
			balloonData.mPriority = ThoughtBalloonPriority.High;
			actor.ThoughtBalloonManager.ShowBalloon(balloonData);
			if (JellyBeanBush.mJellyBeanBuffs.ContainsKey(buffNames))
			{
				JellyBeanBush.JellyBeanBuffStruct jellyBeanBuffStruct = JellyBeanBush.mJellyBeanBuffs[buffNames];
				float value = 0f;
				MotiveTuning motiveTuning = actor.GetMotiveTuning(jellyBeanBuffStruct.commodity);
				if (motiveTuning != null)
				{
					if (jellyBeanBuffStruct.value > 0f)
					{
						value = MathUtils.Clamp(jellyBeanBuffStruct.value + JellyBeanBush.kMotiveDelta, motiveTuning.Min, motiveTuning.Max);
					}
					else
					{
						value = MathUtils.Clamp(jellyBeanBuffStruct.value - JellyBeanBush.kMotiveDelta, motiveTuning.Min, motiveTuning.Max);
					}
				}
				actor.Motives.SetValue(jellyBeanBuffStruct.commodity, value);
			}
		}

        public sealed class KillSimNiec : Interaction<Sim, NiecAutoKill>
        {
            // Fields
            public bool FromNeutral = true;
            public static readonly InteractionDefinition Singleton = new Definition();

            // Methods
            public override bool Run() // Run
            {
				Slot[] routingSlots = this.Target.GetRoutingSlots();
				bool flag = false;
				Route route = this.Actor.CreateRoute();
				if (route.PlanToSlot(this.Target, routingSlots).Succeeded())
				{
					flag = this.Actor.DoRoute(route);
				}
				else if (this.Actor.RouteToObjectRadius(this.Target, 1f))
				{
					flag = true;
				}
				//SpeedTrap.Sleep(10);
				base.StandardEntry();
				// Pet Only Crazy CLIP Fixed No CLIP
				if (Actor.IsNPC && Actor.SimDescription.IsPet)
            	{
            		this.Actor.Kill(SimDescription.DeathType.PetOldAgeGood);
					//KillSimNiecX.MineKill(Actor, SimDescription.DeathType.PetOldAgeGood, true);
					return false;
            	}
				if (Actor.IsInActiveHousehold && Actor.SimDescription.IsPet)
            	{
        		    this.Actor.Motives.MaxEverything();
        		    Actor.Autonomy.Motives.FreezeDecayEverythingExcept(new CommodityKind[0]);
					return false;
            	}
				base.BeginCommodityUpdates();
				base.AcquireStateMachine("JellyBean");
				base.SetActor("x", this.Actor);
				base.SetActor("magicjellybeanbush", this.Target);
				base.EnterState("x", "JellyBeanEnter");
				if (RandomUtil.RandomChance01(100) && !Actor.IsInActiveHousehold)
                {
					base.AnimateSim("Poisoned");
                    if (!Actor.CanBeKilled()) // My Mod is CanBeKilled Not Modifed The Sims 3 is File Dll Gameplay 
                    {
                        KillSimNiecX.MineKill(Actor, SimDescription.DeathType.Starve);
                        SpeedTrap.Sleep(10);
                        //return false;
                    }
                    Actor.Kill(SimDescription.DeathType.Starve);
                    SpeedTrap.Sleep(10);
                    //return false;
                }
				else
				{
					float @float = RandomUtil.GetFloat(1f);
					if (@float < 100 && this.Actor.IsInActiveHousehold)
					{
						base.AnimateSim("NormalExit");
						{
        		            this.Actor.Motives.MaxEverything();
        		            Actor.Autonomy.Motives.FreezeDecayEverythingExcept(new CommodityKind[0]);
        		            return false;
						}
					}
					else if (@float < JellyBeanBush.kChanceToCatchOnFire + JellyBeanBush.kChanceToBeElectrocuted + JellyBeanBush.kChanceToGetTooSpicy && this.Actor.IsNPC)
					{
						base.AnimateSim("NormalExit");
						this.Actor.BuffManager.AddElement(BuffNames.TooSpicy, Origin.FromMagicJellyBean);
					}
					else if (@float < JellyBeanBush.kChanceToCatchOnFire + JellyBeanBush.kChanceToBeElectrocuted + JellyBeanBush.kChanceToGetTooSpicy + JellyBeanBush.kChanceToGetPositiveBuff && this.Actor.IsNPC)
					{
						base.AnimateSim("NormalExit");
						this.Target.ApplyRandomBuff(this.Actor, JellyBeanBush.JellyBeanBuffType.NormalPositive);
						VisualEffect.FireOneShotEffect("ep7BuffJellyBeanPos_main", this.Actor, Sim.FXJoints.Pelvis, VisualEffect.TransitionType.SoftTransition);
						if (RandomUtil.CoinFlip())
						{
							this.Actor.PlayReaction(ReactionTypes.Excited, ReactionSpeed.NowOrLater);
						}
						else
						{
							this.Actor.PlayReaction(ReactionTypes.Cheer, ReactionSpeed.NowOrLater);
						}
					}
					else if (@float < JellyBeanBush.kChanceToCatchOnFire + JellyBeanBush.kChanceToBeElectrocuted + JellyBeanBush.kChanceToGetTooSpicy + JellyBeanBush.kChanceToGetPositiveBuff + JellyBeanBush.kChanceToGetNegitiveBuff && this.Actor.IsNPC)
					{
						base.AnimateSim("NormalExit");
						this.Target.ApplyRandomBuff(this.Actor, JellyBeanBush.JellyBeanBuffType.NormalNegative);
						VisualEffect.FireOneShotEffect("ep7BuffJellyBeanNeg_main", this.Actor, Sim.FXJoints.Pelvis, VisualEffect.TransitionType.SoftTransition);
						if (RandomUtil.CoinFlip())
						{
							this.Actor.PlayReaction(ReactionTypes.Shocked, ReactionSpeed.NowOrLater);
						}
						else
						{
							this.Actor.PlayReaction(ReactionTypes.HeadPain, ReactionSpeed.NowOrLater);
						}
					}
					else if (@float < JellyBeanBush.kChanceToCatchOnFire + JellyBeanBush.kChanceToBeElectrocuted + JellyBeanBush.kChanceToGetTooSpicy + JellyBeanBush.kChanceToGetPositiveBuff + JellyBeanBush.kChanceToGetNegitiveBuff + JellyBeanBush.kChanceToGetPosTransBuff && this.Actor.IsNPC)
					{
						base.AnimateSim("NormalExit");
						OccultImaginaryFriend occultImaginaryFriend;

						if (!this.Actor.BuffManager.HasTransformBuff() && !this.Actor.IsWearingSpecialOutfit(UniversityMascot.MascotOutfitKey) && !OccultImaginaryFriend.TryGetOccultFromSim(this.Actor, out occultImaginaryFriend) && !this.Actor.IsSimBot)
						{
							this.Target.ApplyRandomBuff(this.Actor, JellyBeanBush.JellyBeanBuffType.TransPositive);
							if (RandomUtil.CoinFlip())
							{
								this.Actor.PlayReaction(ReactionTypes.Excited, ReactionSpeed.AfterInteraction);
							}
							else
							{
								this.Actor.PlayReaction(ReactionTypes.Cheer, ReactionSpeed.AfterInteraction);
							}
						}
					}
					else if (@float < JellyBeanBush.kChanceToCatchOnFire + JellyBeanBush.kChanceToBeElectrocuted + JellyBeanBush.kChanceToGetTooSpicy + JellyBeanBush.kChanceToGetPositiveBuff + JellyBeanBush.kChanceToGetNegitiveBuff + JellyBeanBush.kChanceToGetPosTransBuff + JellyBeanBush.kChanceToGetNegTransBuff && this.Actor.IsNPC)
					{
						base.AnimateSim("NormalExit");
						OccultImaginaryFriend occultImaginaryFriend2;
						if (!this.Actor.BuffManager.HasTransformBuff() && !this.Actor.IsWearingSpecialOutfit(UniversityMascot.MascotOutfitKey) && !OccultImaginaryFriend.TryGetOccultFromSim(this.Actor, out occultImaginaryFriend2) && this.Actor.IsNPC)
						{
							this.Target.ApplyRandomBuff(this.Actor, JellyBeanBush.JellyBeanBuffType.TransNegative);
							if (RandomUtil.CoinFlip())
							{
								this.Actor.PlayReaction(ReactionTypes.Shocked, ReactionSpeed.AfterInteraction);
							}
							else
							{
								this.Actor.PlayReaction(ReactionTypes.HeadPain, ReactionSpeed.AfterInteraction);
							}
						}
					}
					else
					{
						base.AnimateSim("NormalExit");
					}
				}
				base.EndCommodityUpdates(true);
				base.StandardExit();
				EventTracker.SendEvent(EventTypeId.kEatMagicJellyBean, this.Actor);
				return true;
			}
            	/*
            {
				Slot[] routingSlots = this.Target.GetRoutingSlots();
				bool flag = false;
				Route route = this.Actor.CreateRoute();
				if (route.PlanToSlot(this.Target, routingSlots).Succeeded())
				{
					flag = this.Actor.DoRoute(route);
				}
				else if (this.Actor.RouteToObjectRadius(this.Target, 1f))
				{
					flag = true;
				}
				base.StandardEntry();
				base.BeginCommodityUpdates();
				base.AcquireStateMachine("JellyBean");
				base.SetActor("x", this.Actor);
				base.SetActor("magicjellybeanbush", this.Target);
				base.EnterState("x", "JellyBeanEnter");
				float @float = RandomUtil.GetFloat(1f);
				if (@float < 99 != Actor.IsNPC)
				{
					base.AnimateSim("Poisoned");
					this.Actor.Kill(SimDescription.DeathType.Burn);
				}
				base.EndCommodityUpdates(true);
				base.StandardExit();
				EventTracker.SendEvent(EventTypeId.kEatMagicJellyBean, this.Actor);
				return true;
			} */
            	/*
			{
				Slot[] routingSlots = this.Target.GetRoutingSlots();
				bool flag = false;
				Route route = this.Actor.CreateRoute();
				if (route.PlanToSlot(this.Target, routingSlots).Succeeded())
				{
					flag = this.Actor.DoRoute(route);
				}
				else if (this.Actor.RouteToObjectRadius(this.Target, 1f))
				{
					flag = true;
				}
				if (!flag)
				{
					return false;
				}         
				base.StandardEntry();
				base.BeginCommodityUpdates();
				base.AcquireStateMachine("JellyBean");
				base.SetActor("x", this.Actor);
				base.SetActor("magicjellybeanbush", this.Target);
				base.EnterState("x", "JellyBeanEnter");
				
				if (Actor.IsInActiveHousehold)
				{
					
					base.EndCommodityUpdates(true);
				    base.StandardExit();
				    return false;
				}
				
            	if (Actor.IsNPC && Actor.SimDescription.IsGhost)
            	{
				    base.AnimateSim("Poisoned");
				    this.Actor.Kill(SimDescription.DeathType.Burn);
				    return true;
            	}
            	base.EndCommodityUpdates(true);
				base.StandardExit();
            	return true;
			}*/
            // Nested Types
            //[DoesntRequireTuning]
            private class Definition : InteractionDefinition<Sim, NiecAutoKill, NiecAutoKill.KillSimNiec>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
            {
                // Methods
            public override string GetInteractionName(Sim actor, NiecAutoKill target, InteractionObjectPair interaction)
            {
            	return "Eat Jelly";
            	/*
            	if (target.IsInActiveHousehold)
            	{
            		return "DGS: :) Help you Max Mood";
            	}
            	if (target.IsNPC)
            	{
            		return "Auto Kill Sim In NPC Only";
            	}
            	*/
            }
            /*
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    NumSelectableRows = 1;
                    Sim actor = parameters.Actor as Sim;
                    List<Sim> sims = new List<Sim>();

                    foreach (Sim sim in actor.LotCurrent.GetSims())
                    {
                        if (sim != actor && sim.SimDescription.TeenOrAbove)
                        {
                            sims.Add(sim);
                        }
                    }

                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, sims, true);
                }
                */

                public override bool Test(Sim a, NiecAutoKill target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                	if (a.IsInActiveHousehold)
                	{
                		if (isAutonomous)
                		{
                			return false;
                		}
                		greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback("Death Good System: Not Allow Active Household and Allow NPC to Sim");
                	    return true;
                	}
                    if (a.Service is GrimReaper) return false;
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



