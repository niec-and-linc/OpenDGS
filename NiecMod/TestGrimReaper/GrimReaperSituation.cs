/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 15/09/2018
 * Time: 10:16
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
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Moving;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.StoryProgression;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.Visa;
using Sims3.Gameplay.Services;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Controller;
using NiecMod.Utilities;
namespace NiecMod.TestGrimReaper
{
	// Token: 0x020012BB RID: 4795
	public class GrimReaperSituation : ServiceSituation
	{
		// Token: 0x1700168F RID: 5775
		// (get) Token: 0x06009AC3 RID: 39619 RVA: 0x0005D58B File Offset: 0x0005B78B
		public static ReactionBroadcasterParams ScaredParams
		{
			get
			{
				return GrimReaperSituation.kScaredParams;
			}
		}

		// Token: 0x06009AC4 RID: 39620 RVA: 0x0005D592 File Offset: 0x0005B792
		public GrimReaperSituation()
		{
		}

		// Token: 0x06009AC5 RID: 39621 RVA: 0x0034F4E4 File Offset: 0x0034D6E4
		public GrimReaperSituation(Service service, Lot lot, Sim worker, int cost) : base(service, lot, worker, cost)
		{
			worker.AssignRole(this);
			this.Worker.Autonomy.Motives.MaxEverything();
			this.Worker.Autonomy.Motives.FreezeDecayEverythingExcept(new CommodityKind[0]);
			this.Worker.Autonomy.IncrementAutonomyDisabled();
			this.Worker.CanIndividualSimReact = false;
			worker.SetHiddenFlags((HiddenFlags)4294967295u);
			base.SetState(new GrimReaperSituation.WaitToAppear(this));
			this.mbServiceStarted = true;
			this.Worker.EnableSocialsOnSim();
			this.Worker.Autonomy.DecrementAutonomyDisabled();
			SimDescription simDescription = this.Worker.SimDescription;
			if (simDescription.FavoriteFood == FavoriteFoodType.None)
			{
				simDescription.FavoriteFood = GrimReaper.kFavoriteFoods[RandomUtil.GetInt(0, GrimReaper.kFavoriteFoods.Length - 1)];
			}
			if (simDescription.FavoriteMusic == FavoriteMusicType.None)
			{
				simDescription.FavoriteMusic = GrimReaper.kFavoriteMusic[RandomUtil.GetInt(0, GrimReaper.kFavoriteMusic.Length - 1)];
			}
			if (simDescription.FavoriteColor == Color.Preset.None)
			{
				simDescription.FavoriteColor = new CASCharacter.NameColorPair("Black", new Color(0, 0, 0)).mColor;
			}
		}

		// Token: 0x06009AC6 RID: 39622 RVA: 0x0005D5AC File Offset: 0x0005B7AC
		public override void SetToSocializing()
		{
			base.SetState(new GrimReaperSituation.Socializing(this));
		}

		// Token: 0x06009AC7 RID: 39623 RVA: 0x0005D5BA File Offset: 0x0005B7BA
		public override void SetToLeave()
		{
			base.SetState(new GrimReaperSituation.Leave(this));
		}

		// Token: 0x06009AC8 RID: 39624 RVA: 0x0005D5C8 File Offset: 0x0005B7C8
		public override void SetToFire(Sim serviceSim, Sim firer)
		{
			base.SetToFire(serviceSim, firer);
			base.SetState(new GrimReaperSituation.Leave(this));
			this.Service.FireSim(serviceSim);
		}

		// Token: 0x06009AC9 RID: 39625 RVA: 0x0005D5EA File Offset: 0x0005B7EA
		public void RestoreAutonomy()
		{
			this.Worker.Motives.RestoreDecays();
			this.Worker.SimDescription.MotivesDontDecay = false;
			this.Worker.CanIndividualSimReact = true;
			this.Worker.Autonomy.DecrementAutonomyDisabled();
		}

		// Token: 0x06009ACA RID: 39626 RVA: 0x0005D629 File Offset: 0x0005B829
		public override void EndService()
		{
			if (!(base.Child is GrimReaperSituation.Leave))
			{
				base.SetState(new GrimReaperSituation.Leave(this));
			}
		}

		// Token: 0x06009ACB RID: 39627 RVA: 0x0005D644 File Offset: 0x0005B844
		public override void CleanUp()
		{
			if (this.ScaredReactionBroadcaster != null)
			{
				this.ScaredReactionBroadcaster.Dispose();
				this.ScaredReactionBroadcaster = null;
			}
			if (this.ReaperLoop != null)
			{
				this.ReaperLoop.Dispose();
				this.ReaperLoop = null;
			}
			base.CleanUp();
		}

		// Token: 0x06009ACC RID: 39628 RVA: 0x0034F61C File Offset: 0x0034D81C
		public Sim FindClosestDeadSim()
		{
			Sim closestObject = GlobalFunctions.GetClosestObject<Sim>(this.Worker, false, true, true, 0, this.ignoreList, new GlobalFunctions.FindObjectValidityTest(this.VerifyDeadSimSkipSelected));
			if (closestObject == null)
			{
				closestObject = GlobalFunctions.GetClosestObject<Sim>(this.Worker, false, true, true, 0, this.ignoreList, new GlobalFunctions.FindObjectValidityTest(this.VerifyDeadSim));
			}
			return closestObject;
		}

		// Token: 0x06009ACD RID: 39629 RVA: 0x0034F674 File Offset: 0x0034D874
		public bool VerifyDeadSim(IGameObject simToCheck, ref float score)
		{
			Sim sim = simToCheck as Sim;
			return sim != null && (sim.SimDescription.DeathStyle != SimDescription.DeathType.None && !sim.SimDescription.IsGhost);
		}

		// Token: 0x06009ACE RID: 39630 RVA: 0x0034F6AC File Offset: 0x0034D8AC
		public bool VerifyDeadSimSkipSelected(IGameObject simToCheck, ref float score)
		{
			if (this.VerifyDeadSim(simToCheck, ref score))
			{
				Sim sim = simToCheck as Sim;
				return !sim.IsActiveSim;
			}
			return false;
		}

		// Token: 0x06009ACF RID: 39631 RVA: 0x0034F6D8 File Offset: 0x0034D8D8
		public Sim FindClosestDeadDivingSim()
		{
			Sim closestObject = GlobalFunctions.GetClosestObject<Sim>(this.Worker, false, true, true, 0, this.ignoreList, new GlobalFunctions.FindObjectValidityTest(this.VerifyDeadDivingSimSkipSelected));
			if (closestObject == null)
			{
				closestObject = GlobalFunctions.GetClosestObject<Sim>(this.Worker, false, true, true, 0, this.ignoreList, new GlobalFunctions.FindObjectValidityTest(this.VerifyDeadDivingSim));
			}
			return closestObject;
		}

		// Token: 0x06009AD0 RID: 39632 RVA: 0x0034F730 File Offset: 0x0034D930
		public bool VerifyDeadDivingSim(IGameObject simToCheck, ref float score)
		{
			Sim sim = simToCheck as Sim;
			return sim != null && (sim.Posture is ScubaDiving && sim.SimDescription.DeathStyle != SimDescription.DeathType.None && !sim.SimDescription.IsGhost);
		}

		// Token: 0x06009AD1 RID: 39633 RVA: 0x0034F774 File Offset: 0x0034D974
		public bool VerifyDeadDivingSimSkipSelected(IGameObject simToCheck, ref float score)
		{
			if (this.VerifyDeadDivingSim(simToCheck, ref score))
			{
				Sim sim = simToCheck as Sim;
				return !sim.IsActiveSim;
			}
			return false;
		}

		// Token: 0x06009AD2 RID: 39634 RVA: 0x0034F7A0 File Offset: 0x0034D9A0
		private IChessTable FindClosestChessTable(Sim sim)
		{
			return GlobalFunctions.GetClosestObject<IChessTable>(sim, true, true, null, new GlobalFunctions.FindObjectValidityTest(this.IsChessTableValid));
		}

		// Token: 0x06009AD3 RID: 39635 RVA: 0x0034F7C4 File Offset: 0x0034D9C4
		private bool IsChessTableValid(IGameObject chessTable, ref float score)
		{
			Slot[] containmentSlotNames = Slots.GetContainmentSlotNames(chessTable.ObjectId);
			foreach (Slot slotName in containmentSlotNames)
			{
				IGameObject containedObject = chessTable.GetContainedObject(slotName);
				if (containedObject == null || containedObject.InUse)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06009AD4 RID: 39636 RVA: 0x0034F814 File Offset: 0x0034DA14
		private IVendingMachine FindDeadlyVendingMachine()
		{
			return GlobalFunctions.GetClosestObject<IVendingMachine>(this.Worker, false, true, true, 0, null, new GlobalFunctions.FindObjectValidityTest(this.IsVendingMachineValid));
		}

		// Token: 0x06009AD5 RID: 39637 RVA: 0x0034F840 File Offset: 0x0034DA40
		private bool IsVendingMachineValid(IGameObject machineToCheck, ref float score)
		{
			IVendingMachine vendingMachine = machineToCheck as IVendingMachine;
			return vendingMachine != null && vendingMachine.KilledASim;
		}

		// Token: 0x06009AD6 RID: 39638 RVA: 0x0034F860 File Offset: 0x0034DA60
		public void CheckIfEveryoneIsReaped(Sim actor, float x)
		{
			if (this.ChessOpponent != null)
			{
				base.SetState(new GrimReaperSituation.PlayChess(this));
				return;
			}
			if (this.PetSavior != null)
			{
				base.SetState(new GrimReaperSituation.PetSaveFromDeath(this));
				return;
			}
			Sim sim = this.FindClosestDeadSim();
			this.VendingMachine = this.FindDeadlyVendingMachine();
			if (sim != null)
			{
				if (this.FadeDeathIn)
				{
					this.Worker.FadeIn(false, 2f);
					this.StartGrimReaperSmoke();
					this.FadeDeathIn = false;
				}
				base.SetState(new GrimReaperSituation.Reaping(this));
				return;
			}
			if (this.VendingMachine != null)
			{
				base.SetState(new GrimReaperSituation.UseVendingMachine(this));
				return;
			}
			LotManager.SetAutoGameSpeed(ServiceType.GrimReaper);
			float num = GrimReaper.HangOutBaseChance;
			if (this.FadeDeathIn || this.Lot.IsDivingLot)
			{
				num = 0f;
			}
			else
			{
				num += (float)actor.LotCurrent.NumDeathsOnLot * GrimReaper.HangOutIncreasePerDeath;
				if (num > GrimReaper.HangOutMaxChance)
				{
					num = GrimReaper.HangOutMaxChance;
				}
			}
			if (num > 0f && RandomUtil.RandomChance(num))
			{
				base.SetState(new GrimReaperSituation.Socializing(this));
				return;
			}
			base.SetState(new GrimReaperSituation.Leave(this));
		}

		// Token: 0x06009AD7 RID: 39639 RVA: 0x0005D680 File Offset: 0x0005B880
		public void ClearChessData(Sim actor, float x)
		{
			this.Worker.SimDescription.ShowSocialsOnSim = false;
			this.ChessOpponent = null;
			this.ChessTable = null;
			this.LastGraveCreated = null;
			this.ChessMatchWon = false;
			this.CheckIfEveryoneIsReaped(actor, x);
		}

		// Token: 0x06009AD8 RID: 39640 RVA: 0x0005D6B7 File Offset: 0x0005B8B7
		public void ClearHarassData(Sim actor, float x)
		{
			this.LastGraveCreated.RemoveFromUseList(this.PetSavior);
			this.PetSavior = null;
			this.LastGraveCreated = null;
			this.DeathCheckForAbandonedChildren(actor);
			this.CheckIfEveryoneIsReaped(actor, x);
		}

		// Token: 0x06009AD9 RID: 39641 RVA: 0x0005D6E7 File Offset: 0x0005B8E7
		private void StartGrimReaperSmoke()
		{
			if (this.mGrimReaperSmoke == null)
			{
				this.mGrimReaperSmoke = VisualEffect.Create("reaperSmokeConstant");
				this.mGrimReaperSmoke.ParentTo(this.Worker, Sim.FXJoints.Pelvis);
				this.mGrimReaperSmoke.Start();
			}
		}

		// Token: 0x06009ADA RID: 39642 RVA: 0x0005D724 File Offset: 0x0005B924
		private void StopGrimReaperSmoke()
		{
			if (this.mGrimReaperSmoke != null)
			{
				this.mGrimReaperSmoke.Stop();
				this.mGrimReaperSmoke.Dispose();
				this.mGrimReaperSmoke = null;
			}
		}

		// Token: 0x06009ADB RID: 39643 RVA: 0x0034F970 File Offset: 0x0034DB70
		public static void ScaredDelegate(Sim sim, ReactionBroadcaster broadcaster)
		{
			if (sim.HasTrait(TraitNames.Coward))
			{
				sim.BuffManager.AddElement(BuffNames.Scared, Origin.FromGrimReaper);
				BuffScared.BuffInstanceScared buffInstanceScared = sim.BuffManager.GetElement(BuffNames.Scared) as BuffScared.BuffInstanceScared;
				if (buffInstanceScared != null)
				{
					buffInstanceScared.ScaryObject = (broadcaster.BroadcastingObject as GameObject);
					InteractionPriority priority = new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior);
					sim.InteractionQueue.AddNext(TraitFunctions.CowardTraitFaint.Singleton.CreateInstance(sim, sim, priority, false, true));
				}
			}
		}

		// Token: 0x06009ADC RID: 39644 RVA: 0x0034FA00 File Offset: 0x0034DC00
		private static void GetHouseholdInfo(Household household, out bool teenOrUpInHousehold, out bool childInHousehold, out bool teenOrUpAtHome, out bool childAtHome, out bool petInHousehold)
		{
			teenOrUpInHousehold = false;
			childInHousehold = false;
			petInHousehold = false;
			teenOrUpAtHome = false;
			childAtHome = false;
			foreach (SimDescription simDescription in household.AllSimDescriptions)
			{
				if (simDescription.IsPet)
				{
					petInHousehold = true;
				}
				else if (simDescription.ChildOrBelow)
				{
					childInHousehold = true;
					Sim createdSim = simDescription.CreatedSim;
					if (createdSim != null && createdSim.LotCurrent == createdSim.LotHome)
					{
						childAtHome = true;
					}
				}
				else if (simDescription.TeenOrAbove && (simDescription.DeathStyle == SimDescription.DeathType.None || simDescription.IsPlayableGhost))
				{
					teenOrUpInHousehold = true;
					Sim createdSim2 = simDescription.CreatedSim;
					if (createdSim2 != null && createdSim2.LotCurrent == createdSim2.LotHome)
					{
						teenOrUpAtHome = true;
					}
				}
			}
		}

		// Token: 0x06009ADD RID: 39645 RVA: 0x0034FACC File Offset: 0x0034DCCC
		private static void ScheduleSocialWorkerOrSitterIfNecessary(Lot lot, bool teenOrUpInHousehold, bool childInHousehold, bool teenOrUpAtHome, bool childAtHome, bool petInHousehold)
		{
			if (!teenOrUpInHousehold && (childInHousehold || petInHousehold))
			{
				SocialWorkerChildAbuse instance = SocialWorkerChildAbuse.Instance;
				if (instance != null)
				{
					BoardingSchool.RemoveAllSimsFromBoardingSchool(lot.Household);
					instance.MakeServiceRequest(lot, true, ObjectGuid.InvalidObjectGuid);
					return;
				}
			}
			else if (!teenOrUpAtHome && childAtHome)
			{
				Babysitter instance2 = Babysitter.Instance;
				if (instance2 != null)
				{
					instance2.MakeServiceRequest(lot, true, ObjectGuid.InvalidObjectGuid, true);
				}
			}
		}

		// Token: 0x06009ADE RID: 39646 RVA: 0x0034FB24 File Offset: 0x0034DD24
		public static void CheckForAbandonedChildren(Household household)
		{
			if (household != null)
			{
				bool teenOrUpInHousehold;
				bool childInHousehold;
				bool teenOrUpAtHome;
				bool childAtHome;
				bool petInHousehold;
				GrimReaperSituation.GetHouseholdInfo(household, out teenOrUpInHousehold, out childInHousehold, out teenOrUpAtHome, out childAtHome, out petInHousehold);
				GrimReaperSituation.ScheduleSocialWorkerOrSitterIfNecessary(household.LotHome, teenOrUpInHousehold, childInHousehold, teenOrUpAtHome, childAtHome, petInHousehold);
			}
		}

		// Token: 0x06009ADF RID: 39647 RVA: 0x0034FB58 File Offset: 0x0034DD58
		private void DeathCheckForAbandonedChildren(Sim sim)
		{
			if (sim.Household != null)
			{
				bool flag;
				bool flag2;
				bool teenOrUpAtHome;
				bool childAtHome;
				bool petInHousehold;
				GrimReaperSituation.GetHouseholdInfo(sim.Household, out flag, out flag2, out teenOrUpAtHome, out childAtHome, out petInHousehold);
				if (GameStates.IsOnVacation)
				{
					if (!flag && flag2 && this.FindClosestDeadSim() == null)
					{
						HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
						if (hudModel != null)
						{
							SimpleMessageDialog.Show(TravelUtil.LocalizeString("TripOverCaption", new object[0]), TravelUtil.LocalizeString("AdultDiedOnVacation", new object[]
							{
								sim
							}), ModalDialog.PauseMode.PauseSimulator);
						}
						GameStates.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
						return;
					}
				}
				else if (this.PetSavior == null && sim.LotHome != null)
				{
					GrimReaperSituation.ScheduleSocialWorkerOrSitterIfNecessary(sim.LotHome, flag, flag2, teenOrUpAtHome, childAtHome, petInHousehold);
				}
			}
		}

		// Token: 0x06009AE0 RID: 39648 RVA: 0x0005D74C File Offset: 0x0005B94C
		private bool IsValidSaviorPet(Sim pet)
		{
			return (pet.IsADogSpecies || pet.IsCat || pet.IsHorse) && (!pet.IsPuppy && !pet.IsKitten) && !pet.IsFoal;
		}

		// Token: 0x06009AE1 RID: 39649 RVA: 0x0034FC08 File Offset: 0x0034DE08
		private bool CheckAndSetPetSavior(Sim sim)
		{
			foreach (Sim sim2 in sim.Household.Pets)
			{
				if (this.IsValidSaviorPet(sim2) && sim2.GetDistanceToObject(sim) <= GrimReaperSituation.kMaxPetSaveDistance)
				{
					Relationship relationship = Relationship.Get(sim2, sim, false);
					if (relationship != null && !relationship.HasSavedFromDeath && relationship.CurrentLTR >= LongTermRelationshipTypes.BestFriend)
					{
						float num = GrimReaperSituation.kMaxPetSaveBaseChance;
						foreach (TraitNames guid in GrimReaperSituation.kPositiveSaviorPetTraits)
						{
							if (sim2.TraitManager.HasElement(guid))
							{
								num += GrimReaperSituation.kPositiveTraitBonus;
							}
						}
						foreach (TraitNames guid2 in GrimReaperSituation.kNegativeSaviorPetTraits)
						{
							if (sim2.TraitManager.HasElement(guid2))
							{
								num -= GrimReaperSituation.kNegativeTraitPenalty;
							}
						}
						if (RandomUtil.RandomChance(num * 100f))
						{
							this.PetSavior = sim2;
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x06009AE2 RID: 39650 RVA: 0x0005D783 File Offset: 0x0005B983
		private void ExitSituation(Sim actor, float x)
		{
			this.StopGrimReaperSmoke();
			if (GameStates.IsOnVacation && !GameUtils.IsFutureWorld() && GameStates.CurrentDayOfTrip > GameStates.TripLength && Household.ActiveHousehold != null)
			{
				GameStates.UpdateTelemetryAndTriggerTravelBackToHomeWorld();
			}
			base.Exit();
		}

		// Token: 0x06009AE3 RID: 39651 RVA: 0x0034FD38 File Offset: 0x0034DF38
		private void ExitSituationFailure(Sim actor, float x)
		{
			GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(actor) as GrimReaperSituation;
			actor.InteractionQueue.CancelAllInteractions();
			this.ExitTutorial();
			if (grimReaperSituation.LastSimOfHousehold != null)
			{
				GrimReaperSituation.LastSimCleanup(grimReaperSituation);
			}
			Urnstone.FogEffectTurnAllOff(actor.LotCurrent);
			actor.FadeOut();
			this.ExitSituation(actor, x);
		}

		// Token: 0x06009AE4 RID: 39652 RVA: 0x0034FD8C File Offset: 0x0034DF8C
		private void ExitTutorial()
		{
			if (PlumbBob.SelectedActor == this.LastSimOfHousehold && IntroTutorial.IsRunning)
			{
				IntroTutorial.ForceExitTutorial();
				int num = this.LastSimOfHousehold.Household.NumMembers - this.LastSimOfHousehold.Household.GetNumberOfRoommates();
				if (num > 1)
				{
					this.LastSimOfHousehold = null;
				}
			}
		}

		// Token: 0x06009AE5 RID: 39653 RVA: 0x0005D7B7 File Offset: 0x0005B9B7
		public static bool ShouldDoDeathEvent(Sim deadSim)
		{
			return deadSim != null && (deadSim.Household.IsActive || deadSim.LotCurrent == LotManager.ActiveLot || deadSim.LotCurrent == PlumbBob.SelectedActor.LotHome);
		}

		// Token: 0x06009AE6 RID: 39654 RVA: 0x0034FDE0 File Offset: 0x0034DFE0
		private static void LastSimCleanup(GrimReaperSituation situation)
		{
			if (situation.LastSimOfHousehold == null)
			{
				return;
			}
			if (GameUtils.IsOnVacation() || GameUtils.IsUniversityWorld() || GameUtils.IsFutureWorld())
			{
				GameStates.sOldActiveHousehold = Household.ActiveHousehold;
			}
			situation.LastGraveCreated.GhostCleanup(situation.LastSimOfHousehold, true, !GameUtils.IsAnyTravelBasedWorld());
			UserToolUtils.OnClose();
			situation.LastSimOfHousehold.Destroy();
			MovingSituation.CleanUpLotForMoveOut(situation.Lot);
			ServiceSituation.sGotoFamilySelectionReason = ServiceSituation.FamilySelectionReason.Reaper;
			situation.mCleanupBehavior = ServiceSituation.ServiceCleanupBehavior.GameEntry;
			if (GameUtils.IsAnyTravelBasedWorld())
			{
				foreach (ulong simDescriptionID in GameStates.TravelerIds)
				{
					Urnstone urnstone = Urnstone.FindGhostsGrave(simDescriptionID);
					if (urnstone != null)
					{
						GrimReaperSituation.AddGraveToRandomMausoleum(urnstone);
					}
				}
				bool flag = true;
				if (Simulator.CheckYieldingContext(false))
				{
					string entryKey = "Gameplay/Situations/GrimReaper:LastSimDiedOnVacationDialog";
					if (GameUtils.IsUniversityWorld())
					{
						entryKey = "Gameplay/Situations/GrimReaper:LastSimDiedOnEP09UniversityDialog";
					}
					else if (GameUtils.IsFutureWorld())
					{
						entryKey = "Gameplay/Situations/GrimReaper:LastSimDiedOnEP11FutureDialog";
					}
					flag = TwoButtonDialog.Show(Localization.LocalizeString(entryKey, new object[0]), Localization.LocalizeString("Gameplay/Situations/GrimReaper:MainMenuButton", new object[0]), Localization.LocalizeString("Gameplay/Situations/GrimReaper:ReturnHomeButton", new object[0]));
				}
				if (flag)
				{
					situation.mCleanupBehavior = ServiceSituation.ServiceCleanupBehavior.MainMenu;
				}
				else
				{
					TravelUtil.PlayerMadeTravelRequest = true;
				}
			}
			situation.LastSimOfHousehold = null;
		}

		// Token: 0x06009AE7 RID: 39655 RVA: 0x0034FF28 File Offset: 0x0034E128
		public static void AddGraveToRandomMausoleum(Urnstone urn)
		{
			List<IMausoleum> list = new List<IMausoleum>(Sims3.Gameplay.Queries.GetObjects<IMausoleum>());
			if (list.Count > 0)
			{
				IMausoleum randomObjectFromList = RandomUtil.GetRandomObjectFromList<IMausoleum>(list);
				if (randomObjectFromList != null)
				{
					foreach (IActor actor in urn.ReferenceList)
					{
						actor.InteractionQueue.PurgeInteractions(urn);
					}
					if (randomObjectFromList.Inventory.TryToAdd(urn))
					{
					}
				}
			}
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
		private VisualEffect mGrimReaperSmoke;

		// Token: 0x04007643 RID: 30275
		private bool mIsFirstSim = true;

		// Token: 0x04007644 RID: 30276
		private SocialJig mScubaDeathJig;

		// Token: 0x04007645 RID: 30277
		[Tunable]
		[TunableComment("Broadcaster for pushing the 'Scared' debuff on Coward Sims when he/she spots the Grim Reaper.")]
		private static ReactionBroadcasterParams kScaredParams = new ReactionBroadcasterParams();

		// Token: 0x04007646 RID: 30278
		[TunableComment("Distance pet must be within to possibly save a sim from death")]
		[Tunable]
		private static float kMaxPetSaveDistance = 50f;

		// Token: 0x04007647 RID: 30279
		[Tunable]
		[TunableComment("Base Chance of pet saving BFF sim")]
		private static float kMaxPetSaveBaseChance = 0.1f;

		// Token: 0x04007648 RID: 30280
		[TunableComment("Positively affecting traits for pet to save sim")]
		[Tunable]
		private static TraitNames[] kPositiveSaviorPetTraits = new TraitNames[]
		{
			TraitNames.AggressivePet,
			TraitNames.LoyalPet,
			TraitNames.BravePet
		};

		// Token: 0x04007649 RID: 30281
		[Tunable]
		[TunableComment("Bonus for each good trait the pet has")]
		private static float kPositiveTraitBonus = 0.05f;

		// Token: 0x0400764A RID: 30282
		[Tunable]
		[TunableComment("Negativly affecting traits for pet to save sim")]
		private static TraitNames[] kNegativeSaviorPetTraits = new TraitNames[]
		{
			TraitNames.CluelessPet,
			TraitNames.LazyPet,
			TraitNames.NervousPet,
			TraitNames.SkittishPet
		};

		// Token: 0x0400764B RID: 30283
		[Tunable]
		[TunableComment("Penalty for bad each trait")]
		private static float kNegativeTraitPenalty = 0.02f;

		// Token: 0x0400764C RID: 30284
		private List<Sim> ignoreList = new List<Sim>();

		// Token: 0x020012BC RID: 4796
		public class ReapSoul : SocialInteraction
		{
			// Token: 0x17001690 RID: 5776
			// (get) Token: 0x06009AE9 RID: 39657 RVA: 0x0035005C File Offset: 0x0034E25C
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
				this.mSMCDeath.EnterState("x", "Enter");
				base.Cleanup();
			}

			// Token: 0x06009AEB RID: 39659 RVA: 0x00350290 File Offset: 0x0034E490
			public override bool Run()
			{
				this.mSMCDeath = this.mSituation.SMCDeath;
				this.mSMCDeath.SetActor("y", this.Target);
				this.mSMCDeath.SetActor("grave", this.mGrave);
				this.mSMCDeath.EnterState("y", "Enter");
				if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.Drown)
				{
					if (this.Actor.IsInActiveHousehold)
					{
						this.Actor.RouteTurnToFace(this.Target.Position);
					}
				}
				else if (this.Actor.IsInActiveHousehold)
				{
					Route route = this.Actor.CreateRoute();
					route.PlanToPointRadialRange(this.Target.Position, 1f, 5f, RouteDistancePreference.PreferNearestToRouteDestination, RouteOrientationPreference.TowardsObject, this.Target.LotCurrent.LotId, null);
					this.Actor.DoRoute(route);
				}
				this.PlaceGraveStone();
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GravePlaced;
				this.Actor.RouteTurnToFace(this.Target.Position);
				PetStartleBehavior.CheckForStartle(this.Actor, StartleType.GrimReaperAppear);
				if (this.Actor.IsInActiveHousehold)
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
						if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.Thirst)
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
					if (this.Target.SimDescription.DeathStyle == SimDescription.DeathType.Ranting && this.Target.TraitManager != null && !this.Target.TraitManager.HasElement(TraitNames.ThereAndBackAgain))
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
				return true;
			}

			// Token: 0x06009AEC RID: 39660 RVA: 0x00350F18 File Offset: 0x0034F118
			protected bool CreateGraveStone()
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
				this.mSituation = (ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation);
				this.mSituation.LastGraveCreated = this.mGrave;
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GraveCreated;
				return true;
			}
			
			// Token: 0x06009AED RID: 39661 RVA: 0x00351000 File Offset: 0x0034F200
			protected virtual void PlaceGraveStone()
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
			protected Vector3 GetPositionForGhost(Sim ghost, Urnstone grave)
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
			protected virtual void EventCallbackFadeBodyFromPose(StateMachineClient sender, IEvent evt)
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

			// Token: 0x06009AF1 RID: 39665 RVA: 0x0035126C File Offset: 0x0034F46C
			protected void EventCallbackSimToGhostEffectNoFadeOut(StateMachineClient sender, IEvent evt)
			{
				this.mDeathEffect = this.mGrave.GetSimToGhostEffect(this.Target, this.mGhostPosition);
				this.mSMCDeath.SetEffectActor("deathEffect", this.mDeathEffect);
				this.mDeathEffect.Start();
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
			protected void ReapSoulCallback()
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
			protected virtual void EventCallbackResurrectSimDeathFlower(StateMachineClient sender, IEvent evt)
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
			protected void GrimReaperPostSequenceCleanup()
			{
				this.Actor.Posture = this.Actor.Standing;
			}

			// Token: 0x06009AFB RID: 39675 RVA: 0x003516EC File Offset: 0x0034F8EC
			protected void FinalizeDeath()
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

			// Token: 0x0400764D RID: 30285
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.ReapSoul.Definition();

			// Token: 0x0400764E RID: 30286
			protected StateMachineClient mSMCDeath;

			// Token: 0x0400764F RID: 30287
			protected Urnstone mGrave;

			// Token: 0x04007650 RID: 30288
			protected Vector3 mGhostPosition;

			// Token: 0x04007651 RID: 30289
			protected VisualEffect mDeathEffect;

			// Token: 0x04007652 RID: 30290
			protected Household mDeadSimsHousehold;

			// Token: 0x04007653 RID: 30291
			protected GrimReaperSituation mSituation;

			// Token: 0x04007654 RID: 30292
			protected GrimReaperSituation.ReapSoul.DeathProgress mDeathProgress;

			// Token: 0x04007655 RID: 30293
			protected bool mWasMemberOfActiveHousehold = true;

			// Token: 0x04007656 RID: 30294
			protected DeathFlower mDeathFlower;

			// Token: 0x04007657 RID: 30295
			private VisualEffect mGhostExplosion;

			// Token: 0x020012BD RID: 4797
			[DoesntRequireTuning]
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.ReapSoul>, IScubaDivingInteractionDefinition, IUsableDuringFire
			{
				// Token: 0x06009B01 RID: 39681 RVA: 0x0005D83B File Offset: 0x0005BA3B
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					if (isAutonomous) return false;
					if (target.IsInActiveHousehold) return false;
					return true;
				}
				public override string[] GetPath(bool bPath)
				{
					return new string[] { "NiecModReap..." };
				}
			}

			// Token: 0x020012BE RID: 4798
			protected enum DeathProgress
			{
				// Token: 0x04007659 RID: 30297
				None,
				// Token: 0x0400765A RID: 30298
				GraveCreated,
				// Token: 0x0400765B RID: 30299
				GravePlaced,
				// Token: 0x0400765C RID: 30300
				DeathFlowerStarted,
				// Token: 0x0400765D RID: 30301
				DeathFlowerPostEvent,
				// Token: 0x0400765E RID: 30302
				UnluckyStarted,
				// Token: 0x0400765F RID: 30303
				UnluckyPostEvent,
				// Token: 0x04007660 RID: 30304
				NormalStarted,
				// Token: 0x04007661 RID: 30305
				Complete
			}
		}

		// Token: 0x020012BF RID: 4799
		public class ReapPetSoul : GrimReaperSituation.ReapSoul
		{
			// Token: 0x06009B03 RID: 39683 RVA: 0x00351C88 File Offset: 0x0034FE88
			public override bool Run()
			{
				Urnstone.KillSim killSim = this.Target.CurrentInteraction as Urnstone.KillSim;
				if (killSim == null || killSim.SocialJig == null)
				{
					return false;
				}
				this.SocialJig = killSim.SocialJig;
				killSim.SocialJig = null;
				this.SocialJig.ClearParticipants();
				this.SocialJig.RegisterParticipants(this.Actor, this.Target);
				if (!base.CreateGraveStone())
				{
					return false;
				}
				this.mSMCDeath = this.mSituation.SMCDeath;
				this.mSMCDeath.SetActor("y", this.Target);
				this.mSMCDeath.SetActor("grave", this.mGrave);
				this.mSMCDeath.EnterState("y", "Enter");
				this.PlaceGraveStone();
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GravePlaced;
				if (!this.mSituation.mIsFirstSim)
				{
					this.mSMCDeath.EnterState("x", "Enter");
				}
				this.mSMCDeath.RequestState(true, "x", "CreateTombstone");
				Lot lotCurrent = this.Target.LotCurrent;
				if (lotCurrent != this.Target.LotHome && lotCurrent.Household != null)
				{
					this.Target.GreetSimOnLot(lotCurrent);
				}
				bool flag = false;
				string name = Localization.LocalizeString(this.Target.SimDescription.IsFemale, "Gameplay/Actors/Sim/ReapSoul:InteractionName", new object[0]);
				if (base.BeginSocialInteraction(new SocialInteractionB.DefinitionDeathInteraction(name, false), true, this.Target.GetSocialRadiusWith(this.Actor), false))
				{
					this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
					if (this.Target.IsHorse)
					{
						this.mSMCDeath.AddOneShotScriptEventHandler(112u, new SacsEventHandler(this.FadeDeathEventCallBack));
					}
					this.mSMCDeath.AddOneShotScriptEventHandler(111u, new SacsEventHandler(this.FadeDeathEventCallBack));
					this.mSMCDeath.RequestState(false, "x", "Pet");
					this.mSMCDeath.RequestState(true, "y", "Pet");
					this.mSMCDeath.RequestState(false, "x", "ReapPet");
					this.mSMCDeath.RequestState(true, "y", "ReapPet");
					this.mSMCDeath.RequestState(false, "x", "Exit");
					this.mSMCDeath.RequestState(true, "y", "Exit");
					flag = true;
				}
				else
				{
					this.Target.FadeOut(false, false, 0f);
				}
				this.Target.AddExitReason(ExitReason.HigherPriorityNext);
				this.mGrave.GhostSetup(this.Target, false);
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
				base.FinalizeDeath();
				base.GrimReaperPostSequenceCleanup();
				this.Target.StartOneShotFunction(new Sims3.Gameplay.Function(base.ReapSoulCallback), GameObject.OneShotFunctionDisposeFlag.OnDispose);
				if (flag && this.Target.IsHorse)
				{
					GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation;
					grimReaperSituation.FadeDeathIn = true;
				}
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
				return true;
			}

			// Token: 0x06009B04 RID: 39684 RVA: 0x00351F60 File Offset: 0x00350160
			private void FadeDeathEventCallBack(StateMachineClient sender, IEvent evt)
			{
				switch (evt.EventId)
				{
				case 111u:
					this.Target.SetOpacity(0f, 0.4f);
					return;
				case 112u:
				{
					this.Actor.SetOpacity(0f, 0.4f);
					GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation;
					grimReaperSituation.StopGrimReaperSmoke();
					return;
				}
				default:
					return;
				}
			}

			// Token: 0x04007662 RID: 30306
			public static readonly InteractionDefinition PetSingleton = new GrimReaperSituation.ReapPetSoul.PetDefinition();

			// Token: 0x020012C0 RID: 4800
			protected class PetDefinition : InteractionDefinition<Sim, Sim, GrimReaperSituation.ReapPetSoul>, IUsableDuringFire
			{
				// Token: 0x06009B07 RID: 39687 RVA: 0x0005D83B File Offset: 0x0005BA3B
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return a.Service is GrimReaper && target.SimDescription.DeathStyle != SimDescription.DeathType.None;
				}
			}
		}

		// Token: 0x020012C1 RID: 4801
		private sealed class ChessChallenge : Interaction<Sim, Sim>
		{
			// Token: 0x06009B09 RID: 39689 RVA: 0x00351FC8 File Offset: 0x003501C8
			public override bool Run()
			{
				GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(this.Target) as GrimReaperSituation;
				if (grimReaperSituation == null)
				{
					return false;
				}
				IChessTable chessTable = grimReaperSituation.FindClosestChessTable(this.Actor);
				if (chessTable == null)
				{
					return false;
				}
				grimReaperSituation.ChessOpponent = this.Actor;
				grimReaperSituation.ChessTable = chessTable;
				chessTable.PushPlayChessWithReaper(this.Actor);
				return true;
			}

			// Token: 0x04007663 RID: 30307
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.ChessChallenge.Definition();

			// Token: 0x020012C2 RID: 4802
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.ChessChallenge>
			{
				// Token: 0x06009B0C RID: 39692 RVA: 0x00352020 File Offset: 0x00350220
				public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
				{
					GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(target) as GrimReaperSituation;
					return Localization.LocalizeString(grimReaperSituation.LastGraveCreated.DeadSimsDescription.IsFemale, "Gameplay/Actors/Sim/ChessChallenge:InteractionName", new object[]
					{
						grimReaperSituation.LastGraveCreated.DeadSimsDescription
					});
				}

				// Token: 0x06009B0D RID: 39693 RVA: 0x0035206C File Offset: 0x0035026C
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					if (target.Service is GrimReaper)
					{
						GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(target) as GrimReaperSituation;
						if (grimReaperSituation != null && grimReaperSituation.ChessOpponent == null)
						{
							return grimReaperSituation.FindClosestChessTable(a) != null;
						}
					}
					return false;
				}
			}
		}

		// Token: 0x020012C3 RID: 4803
		public class WaitToAppear : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B0F RID: 39695 RVA: 0x0005D895 File Offset: 0x0005BA95
			private WaitToAppear()
			{
			}

			// Token: 0x06009B10 RID: 39696 RVA: 0x0005D8B0 File Offset: 0x0005BAB0
			public WaitToAppear(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B11 RID: 39697 RVA: 0x0005D8CC File Offset: 0x0005BACC
			public override void Init(GrimReaperSituation parent)
			{
				this.mAlarmHandle = base.AlarmManager.AddAlarm(GrimReaper.DelayBeforeArriving, TimeUnit.Hours, new AlarmTimerCallback(this.OKToAppear), "Grim Reaper waiting to route", AlarmType.DeleteOnReset, parent.Worker);
			}

			// Token: 0x06009B12 RID: 39698 RVA: 0x003520AC File Offset: 0x003502AC
			private void OKToAppear()
			{
				base.AlarmManager.RemoveAlarm(this.mAlarmHandle);
				this.mAlarmHandle = base.AlarmManager.AddAlarmRepeating(0f, TimeUnit.Minutes, new AlarmTimerCallback(this.TimeToAppear), 1f, TimeUnit.Minutes, "Grim Reaper waiting to appear", AlarmType.AlwaysPersisted, this.Parent.Worker);
			}

			// Token: 0x06009B13 RID: 39699 RVA: 0x00352104 File Offset: 0x00350304
			private void TimeToAppear()
			{
				if (this.Parent.Lot.IsFireOnLot() && this.maxDelay > 0)
				{
					this.maxDelay--;
					return;
				}
				this.Parent.Lot.RemoveAllFires();
				if (this.Parent.Lot.IsDivingLot)
				{
					this.Parent.SetState(new GrimReaperSituation.AppearOnDivingLot(this.Parent));
					return;
				}
				this.Parent.SetState(new GrimReaperSituation.AppearOnLot(this.Parent));
			}

			// Token: 0x06009B14 RID: 39700 RVA: 0x0005D8FD File Offset: 0x0005BAFD
			public override void CleanUp()
			{
				base.AlarmManager.RemoveAlarm(this.mAlarmHandle);
				this.mAlarmHandle = AlarmHandle.kInvalidHandle;
				base.CleanUp();
			}

			// Token: 0x04007664 RID: 30308
			private AlarmHandle mAlarmHandle = AlarmHandle.kInvalidHandle;

			// Token: 0x04007665 RID: 30309
			private int maxDelay = 15;
		}

		// Token: 0x020012C4 RID: 4804
		public class AppearOnDivingLot : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B15 RID: 39701 RVA: 0x0005D921 File Offset: 0x0005BB21
			private AppearOnDivingLot()
			{
			}

			// Token: 0x06009B16 RID: 39702 RVA: 0x0005D929 File Offset: 0x0005BB29
			public AppearOnDivingLot(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B17 RID: 39703 RVA: 0x0035218C File Offset: 0x0035038C
			public override void Init(GrimReaperSituation parent)
			{
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				base.ForceSituationSpecificInteraction(parent.Worker, parent.Worker, GrimReaperSituation.GrimReaperAppearDiving.Singleton, null, new Callback(this.BeginReaping), new Callback(parent.CheckIfEveryoneIsReaped), priority);
			}

			// Token: 0x06009B18 RID: 39704 RVA: 0x0005D932 File Offset: 0x0005BB32
			public void BeginReaping(Sim actor, float x)
			{
				this.Parent.SetState(new GrimReaperSituation.ReapingDiving(this.Parent));
			}
		}

		// Token: 0x020012C5 RID: 4805
		public class AppearOnLot : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B19 RID: 39705 RVA: 0x0005D921 File Offset: 0x0005BB21
			private AppearOnLot()
			{
			}

			// Token: 0x06009B1A RID: 39706 RVA: 0x0005D929 File Offset: 0x0005BB29
			public AppearOnLot(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B1B RID: 39707 RVA: 0x003521DC File Offset: 0x003503DC
			public override void Init(GrimReaperSituation parent)
			{
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				base.ForceSituationSpecificInteraction(parent.Worker, parent.Worker, GrimReaperSituation.GrimReaperAppear.Singleton, null, new Callback(this.BeginReaping), new Callback(parent.CheckIfEveryoneIsReaped), priority);
			}

			// Token: 0x06009B1C RID: 39708 RVA: 0x0005D94A File Offset: 0x0005BB4A
			public void BeginReaping(Sim actor, float x)
			{
				this.Parent.SetState(new GrimReaperSituation.Reaping(this.Parent));
			}
		}

		// Token: 0x020012C6 RID: 4806
		private sealed class GrimReaperAppearDiving : Interaction<Sim, Sim>, IScubaDivingInteractionDefinition
		{
			// Token: 0x06009B1D RID: 39709 RVA: 0x0035222C File Offset: 0x0035042C
			public override bool Run()
			{
				this.mSituation = (ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation);
				this.mSituation.AddRelationshipWithEverySimInHousehold();
				this.Actor.SetOpacity(0f, 0f);
				this.Actor.SetPosition(this.mSituation.Lot.Position);
				this.Actor.RequestWalkStyle(Sim.WalkStyle.ScubaDive);
				this.mSituation.ScaredReactionBroadcaster = new ReactionBroadcaster(this.Actor, GrimReaperSituation.ScaredParams, new ReactionBroadcaster.BroadcastCallback(GrimReaperSituation.ScaredDelegate));
				SimDescription.DeathType deathType = SimDescription.DeathType.ScubaDrown;
				Vector3 vector = Vector3.Invalid;
				Vector3 forward = Vector3.Invalid;
				Sim sim = this.mSituation.FindClosestDeadDivingSim();
				if (sim != null)
				{
					deathType = sim.SimDescription.DeathStyle;
					vector = sim.Position;
					vector.y = World.GetLevelHeight(vector.x, vector.z, 0);
					forward = sim.ForwardVector;
					if (this.mSituation.mScubaDeathJig == null)
					{
						this.mSituation.mScubaDeathJig = (GlobalFunctions.CreateObjectOutOfWorld("UnderwaterSocial_Jig", ProductVersion.EP10) as SocialJig);
						vector.y = World.GetLevelHeight(vector.x, vector.z, 0);
						if (GlobalFunctions.FindGoodLocationNearby(this.mSituation.mScubaDeathJig, new ObjectGuid[]
						{
							sim.ObjectId,
							this.Actor.ObjectId
						}, ref vector, ref forward, 0f, GlobalFunctions.FindGoodLocationStrategies.All, FindGoodLocationBooleans.AllowOnSlopes | FindGoodLocationBooleans.AllowInSea))
						{
							this.mSituation.mScubaDeathJig.SetPosition(vector);
							this.mSituation.mScubaDeathJig.SetForward(forward);
							this.Actor.SetPosition(this.mSituation.mScubaDeathJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimA));
							this.Actor.SetForward(this.mSituation.mScubaDeathJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimA));
						}
					}
				}
				this.Actor.GreetSimOnLot(this.mSituation.Worker.LotCurrent);
				this.mSituation.SMCDeath = StateMachineClient.Acquire(this.Actor, "DeathSequenceScuba");
				this.mSituation.SMCDeath.SetActor("x", this.Actor);
				this.mSituation.SMCDeath.EnterState("x", "Enter");
				Urnstone.FogEffectTurnAllOn(this.mSituation.Worker.LotCurrent);
				this.mDeathEffect = Urnstone.ReaperApperEffect(this.Actor, deathType);
				if (GrimReaperSituation.ShouldDoDeathEvent(sim))
				{
					Audio.StartSound("sting_death");
				}
				this.mSituation.SMCDeath.AddOneShotScriptEventHandler(103u, new SacsEventHandler(this.FadeInReaper));
				this.mSituation.SMCDeath.RequestState("x", "DiveDown");
				this.mDeathEffect.Stop();
				this.mDeathEffect.Dispose();
				this.mSituation.ReaperLoop = new ObjectSound(this.Actor.ObjectId, "death_reaper_lp");
				this.mSituation.ReaperLoop.Start(true);
				return true;
			}

			// Token: 0x06009B1E RID: 39710 RVA: 0x0005D962 File Offset: 0x0005BB62
			private void FadeInReaper(StateMachineClient sender, IEvent evt)
			{
				if (this.mDeathEffect != null)
				{
					this.mDeathEffect.Start();
				}
				if (this.mSituation != null)
				{
					this.mSituation.StartGrimReaperSmoke();
				}
				this.Actor.FadeIn(false, 3f);
			}

			// Token: 0x06009B1F RID: 39711 RVA: 0x0005D99C File Offset: 0x0005BB9C
			public override void Cleanup()
			{
				if (this.mSituation != null && this.mSituation.SMCDeath != null)
				{
					this.mSituation.SMCDeath.RemoveEventHandler(new SacsEventHandler(this.FadeInReaper));
				}
				base.Cleanup();
			}

			// Token: 0x04007666 RID: 30310
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.GrimReaperAppearDiving.Definition();

			// Token: 0x04007667 RID: 30311
			private VisualEffect mDeathEffect;

			// Token: 0x04007668 RID: 30312
			private GrimReaperSituation mSituation;

			// Token: 0x020012C7 RID: 4807
			[DoesntRequireTuning]
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.GrimReaperAppearDiving>, IScubaDivingInteractionDefinition
			{
				// Token: 0x06009B22 RID: 39714 RVA: 0x0005D9E2 File Offset: 0x0005BBE2
				public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
				{
					return "GrimReaperAppearDiving";
				}

				// Token: 0x06009B23 RID: 39715 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012C8 RID: 4808
		public class ReapingDiving : ChildSituation<GrimReaperSituation>, IScubaDivingInteractionDefinition
		{
			// Token: 0x06009B25 RID: 39717 RVA: 0x0005D921 File Offset: 0x0005BB21
			private ReapingDiving()
			{
			}

			// Token: 0x06009B26 RID: 39718 RVA: 0x0005D929 File Offset: 0x0005BB29
			public ReapingDiving(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B27 RID: 39719 RVA: 0x0035252C File Offset: 0x0035072C
			public override void Init(GrimReaperSituation parent)
			{
				Sim sim = parent.FindClosestDeadDivingSim();
				if (sim == null)
				{
					this.Parent.SetState(new GrimReaperSituation.Leave(this.Parent));
					return;
				}
			}
		}

		// Token: 0x020012C9 RID: 4809
		private sealed class GrimReaperAppear : Interaction<Sim, Sim>
		{
			// Token: 0x06009B28 RID: 39720 RVA: 0x00352594 File Offset: 0x00350794
			public override bool Run()
			{
				GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation;
				grimReaperSituation.AddRelationshipWithEverySimInHousehold();
				this.Actor.SetPosition(grimReaperSituation.Lot.Position);
				grimReaperSituation.ScaredReactionBroadcaster = new ReactionBroadcaster(this.Actor, GrimReaperSituation.ScaredParams, new ReactionBroadcaster.BroadcastCallback(GrimReaperSituation.ScaredDelegate));
				SimDescription.DeathType deathType = SimDescription.DeathType.Drown;
				Sim sim = grimReaperSituation.FindClosestDeadSim();
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
				this.Actor.GreetSimOnLot(grimReaperSituation.Worker.LotCurrent);
				grimReaperSituation.SMCDeath = StateMachineClient.Acquire(this.Actor, "DeathSequence");
				grimReaperSituation.SMCDeath.SetActor("x", this.Actor);
				grimReaperSituation.SMCDeath.EnterState("x", "Enter");
				Urnstone.FogEffectTurnAllOn(grimReaperSituation.Worker.LotCurrent);
				VisualEffect visualEffect = Urnstone.ReaperApperEffect(this.Actor, deathType);
				visualEffect.Start();
				grimReaperSituation.StartGrimReaperSmoke();
				if (GrimReaperSituation.ShouldDoDeathEvent(sim))
				{
					Audio.StartSound("sting_death");
				}
				grimReaperSituation.SMCDeath.AddOneShotScriptEventHandler(666u, new SacsEventHandler(this.EventCallbackFadeInReaper));
				grimReaperSituation.SMCDeath.RequestState("x", "ReaperBrushingHimself");
				visualEffect.Stop();
				grimReaperSituation.ReaperLoop = new ObjectSound(this.Actor.ObjectId, "death_reaper_lp");
				grimReaperSituation.ReaperLoop.Start(true);
				this.Actor.Posture = new SimCarryingObjectPosture(this.Actor, null);
				return true;
			}

			// Token: 0x06009B29 RID: 39721 RVA: 0x0005D9F1 File Offset: 0x0005BBF1
			private void EventCallbackFadeInReaper(StateMachineClient sender, IEvent evt)
			{
				this.Actor.FadeIn(false, 3f);
			}

			// Token: 0x04007669 RID: 30313
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.GrimReaperAppear.Definition();

			// Token: 0x020012CA RID: 4810
			[DoesntRequireTuning]
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.GrimReaperAppear>
			{
				// Token: 0x06009B2C RID: 39724 RVA: 0x0005DA10 File Offset: 0x0005BC10
				public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
				{
					return "GrimReaperAppear";
				}

				// Token: 0x06009B2D RID: 39725 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012CB RID: 4811
		public class Reaping : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B2F RID: 39727 RVA: 0x0005D921 File Offset: 0x0005BB21
			private Reaping()
			{
			}

			// Token: 0x06009B30 RID: 39728 RVA: 0x0005D929 File Offset: 0x0005BB29
			public Reaping(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B31 RID: 39729 RVA: 0x00352764 File Offset: 0x00350964
			public override void Init(GrimReaperSituation parent)
			{
				Sim sim = parent.FindClosestDeadSim();
				if (sim == null)
				{
					this.Parent.SetState(new GrimReaperSituation.Leave(this.Parent));
					return;
				}
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				if (!sim.IsHuman)
				{
					base.ForceSituationSpecificInteraction(sim, parent.Worker, GrimReaperSituation.ReapPetSoul.PetSingleton, null, new Callback(parent.CheckIfEveryoneIsReaped), new Callback(parent.CheckIfEveryoneIsReaped), priority);
					return;
				}
				if (sim.Posture is ScubaDiving && !sim.SimDescription.IsMermaid)
				{
					base.ForceSituationSpecificInteraction(sim, parent.Worker, GrimReaperSituation.ReapSoulDiving.DivingSingleton, null, new Callback(parent.CheckIfEveryoneIsReaped), new Callback(parent.CheckIfEveryoneIsReaped), priority);
					return;
				}
				base.ForceSituationSpecificInteraction(sim, parent.Worker, GrimReaperSituation.ReapSoul.Singleton, null, new Callback(parent.CheckIfEveryoneIsReaped), new Callback(parent.CheckIfEveryoneIsReaped), priority);
			}

			// Token: 0x06009B32 RID: 39730 RVA: 0x0005DA1F File Offset: 0x0005BC1F
			public override void CleanUp()
			{
				base.CleanUp();
			}
		}

		// Token: 0x020012CC RID: 4812
		public class Leave : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B33 RID: 39731 RVA: 0x0005D921 File Offset: 0x0005BB21
			public Leave()
			{
			}

			// Token: 0x06009B34 RID: 39732 RVA: 0x0005D929 File Offset: 0x0005BB29
			public Leave(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B35 RID: 39733 RVA: 0x0035284C File Offset: 0x00350A4C
			public override void Init(GrimReaperSituation parent)
			{
				GrimReaper.Instance.MakeServiceRequest(parent.Lot, false, ObjectGuid.InvalidObjectGuid);
				if (parent.ReaperLoop != null)
				{
					parent.ReaperLoop.Dispose();
					parent.ReaperLoop = null;
				}
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				base.ForceSituationSpecificInteraction(parent.Worker, parent.Worker, GrimReaperSituation.LeaveLot.Singleton, null, new Callback(parent.ExitSituation), new Callback(parent.ExitSituationFailure), priority);
			}
		}

		// Token: 0x020012CD RID: 4813
		public class ReapSoulDiving : GrimReaperSituation.ReapSoul
		{
			// Token: 0x06009B36 RID: 39734 RVA: 0x003528CC File Offset: 0x00350ACC
			protected override void PlaceGraveStone()
			{
				if (this.mSituation.mScubaDeathJig != null)
				{
					this.mGrave.SetOpacity(0f, 0f);
					this.mGrave.SetHiddenFlags(HiddenFlags.Nothing);
					Simulator.Sleep(1u);
					this.mGrave.SetPosition(this.mSituation.mScubaDeathJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimB));
					this.mGrave.SetForward(this.mSituation.mScubaDeathJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB));
					this.mGrave.OnHandToolMovement();
					this.mGrave.FadeIn(false, 5f);
					this.mGrave.FogEffectStart();
				}
			}

			// Token: 0x06009B37 RID: 39735 RVA: 0x00352978 File Offset: 0x00350B78
			protected override void EventCallbackFadeBodyFromPose(StateMachineClient sender, IEvent evt)
			{
				uint eventId = evt.EventId;
				if (eventId == 102u)
				{
					this.Target.SetForward(this.mGhostForward);
				}
				base.EventCallbackFadeBodyFromPose(sender, evt);
			}

			// Token: 0x06009B38 RID: 39736 RVA: 0x0005DA27 File Offset: 0x0005BC27
			protected override void EventCallbackResurrectSimDeathFlower(StateMachineClient sender, IEvent evt)
			{
				base.EventCallbackResurrectSimDeathFlower(sender, evt);
				this.Target.FadeOut(false, false, 0.4f);
			}

			// Token: 0x06009B39 RID: 39737 RVA: 0x003529AC File Offset: 0x00350BAC
			public override bool Run()
			{
				if (!base.CreateGraveStone())
				{
					return false;
				}
				this.mSMCDeath = this.mSituation.SMCDeath;
				this.mSMCDeath.SetActor("y", this.Target);
				this.mSMCDeath.SetActor("grave", this.mGrave);
				this.mSMCDeath.EnterState("y", "Enter");
				ScubaDiving scubaDiving = this.Target.Posture as ScubaDiving;
				if (scubaDiving != null)
				{
					scubaDiving.StopBubbleEffects();
				}
				SimDescription.DeathType deathStyle = this.Target.SimDescription.DeathStyle;
				if (!this.mSituation.mIsFirstSim)
				{
					this.mSMCDeath.EnterState("x", "Enter");
					this.mSMCDeath.RequestState(false, "x", "TreadWater");
					this.mSituation.mScubaDeathJig = (GlobalFunctions.CreateObjectOutOfWorld("UnderwaterSocial_Jig", ProductVersion.EP10) as SocialJig);
					Vector3 position = this.Target.Position;
					Vector3 forwardVector = this.Target.ForwardVector;
					if (!GlobalFunctions.FindGoodLocationNearbyOnLevel(this.mSituation.mScubaDeathJig, 0, ref position, ref forwardVector, FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.AllowOnSlopes | FindGoodLocationBooleans.AllowInSea))
					{
						return false;
					}
					this.mSituation.mScubaDeathJig.SetPosition(position);
					this.mSituation.mScubaDeathJig.SetForward(forwardVector);
					Route route = this.mSituation.mScubaDeathJig.RouteToJigA(this.Actor);
					route.DoRouteFail = false;
					this.Actor.DoRoute(route);
				}
				else
				{
					this.mSituation.mIsFirstSim = false;
				}
				this.Target.FadeOut(false, false, 2f);
				this.PlaceGraveStone();
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.GravePlaced;
				this.mSMCDeath.RequestState(true, "x", "CreateTombstone");
				ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData("balloon_moodlet_mourning", this.Target.GetThoughtBalloonThumbnailKey());
				balloonData.BalloonType = ThoughtBalloonTypes.kSpeechBalloon;
				balloonData.mPriority = ThoughtBalloonPriority.High;
				this.Actor.ThoughtBalloonManager.ShowBalloon(balloonData);
				this.mSMCDeath.RequestState(false, "x", "ReaperFloating");
				this.mGhostPosition = this.mSituation.mScubaDeathJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
				this.mGhostForward = this.mSituation.mScubaDeathJig.GetForwardOfSlot(SocialJigTwoPerson.RoutingSlots.SimB);
				Lot lotCurrent = this.Target.LotCurrent;
				if (lotCurrent != this.Target.LotHome && lotCurrent.Household != null)
				{
					this.Target.GreetSimOnLot(lotCurrent);
				}
				switch (deathStyle)
				{
				case SimDescription.DeathType.Shark:
					this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(base.EventCallbackSimToGhostEffectNoFadeOut));
					this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
					this.mSMCDeath.RequestState(true, "y", "PoseShark");
					this.mSMCDeath.RequestState(true, "y", "SharkToFloat");
					this.mSMCDeath.RequestState(false, "y", "GhostFloating");
					break;
				case SimDescription.DeathType.ScubaDrown:
					this.mSMCDeath.AddOneShotScriptEventHandler(101u, new SacsEventHandler(base.EventCallbackSimToGhostEffectNoFadeOut));
					this.mSMCDeath.AddOneShotScriptEventHandler(102u, new SacsEventHandler(this.EventCallbackFadeBodyFromPose));
					this.mSMCDeath.RequestState(true, "y", "PoseDrown");
					this.mSMCDeath.RequestState(true, "y", "DrownToFloat");
					this.mSMCDeath.RequestState(false, "y", "GhostFloating");
					break;
				}
				this.mDeathFlower = this.Target.Inventory.Find<DeathFlower>();
				GrimReaperSituation.BeReapedDiving beReapedDiving = GrimReaperSituation.BeReapedDiving.Singleton.CreateInstance(this.Actor, this.Target, new InteractionPriority((InteractionPriorityLevel)999), isAutonomous: false, cancellableByPlayer: false) as GrimReaperSituation.BeReapedDiving;
				this.LinkedInteractionInstance = beReapedDiving;
				beReapedDiving.mHadDeathFlower = (this.mDeathFlower != null);
				this.Target.AddExitReason(ExitReason.HigherPriorityNext);
				this.Target.InteractionQueue.AddNext(beReapedDiving);
				if (base.StartSync(true, true, null, 0f, false))
				{
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
						base.FinishLinkedInteraction(true);
						this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
						return true;
					}
					this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
					this.mSMCDeath.RequestState(false, "x", "Accept");
					this.mSMCDeath.RequestState(true, "y", "Accept");
					this.mSMCDeath.RequestState(false, "y", "Exit");
					this.mSMCDeath.RequestState(true, "x", "Exit");
				}
				else
				{
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
						base.FinishLinkedInteraction(true);
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
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.NormalStarted;
				base.FinalizeDeath();
				this.Target.StartOneShotFunction(new Sims3.Gameplay.Function(base.ReapSoulCallback), GameObject.OneShotFunctionDisposeFlag.OnDispose);
				this.mDeathProgress = GrimReaperSituation.ReapSoul.DeathProgress.Complete;
				base.FinishLinkedInteraction(true);
				return true;
			}

			// Token: 0x06009B3A RID: 39738 RVA: 0x0005DA43 File Offset: 0x0005BC43
			public override void Cleanup()
			{
				if (this.mSituation != null && this.mSituation.mScubaDeathJig != null)
				{
					this.mSituation.mScubaDeathJig.Destroy();
					this.mSituation.mScubaDeathJig = null;
				}
				base.Cleanup();
			}

			// Token: 0x0400766A RID: 30314
			public static readonly InteractionDefinition DivingSingleton = new GrimReaperSituation.ReapSoulDiving.Definition();

			// Token: 0x0400766B RID: 30315
			protected Vector3 mGhostForward = Vector3.Invalid;

			// Token: 0x020012CE RID: 4814
			protected class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.ReapSoulDiving>, IUsableDuringFire, IScubaDivingInteractionDefinition
			{
				// Token: 0x06009B3D RID: 39741 RVA: 0x0005D83B File Offset: 0x0005BA3B
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return a.Service is GrimReaper && target.SimDescription.DeathStyle != SimDescription.DeathType.None;
				}
			}
		}

		// Token: 0x020012CF RID: 4815
		private sealed class LeaveLot : Interaction<Sim, Sim>
		{
			// Token: 0x06009B3F RID: 39743 RVA: 0x0035302C File Offset: 0x0035122C
			public override bool Run()
			{
				GrimReaperSituation grimReaperSituation = ServiceSituation.FindServiceSituationInvolving(this.Actor) as GrimReaperSituation;
				this.Actor.InteractionQueue.CancelAllInteractions();
				grimReaperSituation.ExitTutorial();
				if (grimReaperSituation.LastSimOfHousehold != null)
				{
					GrimReaperSituation.LastSimCleanup(grimReaperSituation);
					this.Target.Autonomy.DecrementAutonomyDisabled();
					this.Target.SimDescription.ShowSocialsOnSim = true;
					if (!GameUtils.IsAnyTravelBasedWorld())
					{
						SimpleMessageDialog.Show(Localization.LocalizeString("Gameplay/Situations/GrimReaper:FinalDeathTitle", new object[0]), Localization.LocalizeString("Gameplay/Situations/GrimReaper:FinalDeath", new object[0]), ModalDialog.PauseMode.PauseSimulator);
					}
					grimReaperSituation.Lot.FireManager.RemoveAllFires(true);
				}
				else if (grimReaperSituation.Lot.IsFireOnLot() && grimReaperSituation.Lot.IsActive)
				{
					StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString("Gameplay/Services/GrimReaper:LeavingWithFire", new object[0]), this.Actor.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
					StyledNotification.Show(format);
				}
				Urnstone.FogEffectTurnAllOff(this.Actor.LotCurrent);
				grimReaperSituation.StopGrimReaperSmoke();
				if (!grimReaperSituation.FadeDeathIn)
				{
					this.Actor.FadeOut();
					this.Actor.PlaySoloAnimation("a_wave_Rarm", true);
				}
				return true;
			}

			// Token: 0x0400766C RID: 30316
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.LeaveLot.Definition();

			// Token: 0x020012D0 RID: 4816
			[DoesntRequireTuning]
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.LeaveLot>, IUsableDuringFire
			{
				// Token: 0x06009B42 RID: 39746 RVA: 0x0005DAAF File Offset: 0x0005BCAF
				public override string GetInteractionName(Sim a, Sim target, InteractionObjectPair interaction)
				{
					return "LeaveLot";
				}

				// Token: 0x06009B43 RID: 39747 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012D1 RID: 4817
		private sealed class BeReapedDiving : Interaction<Sim, Sim>
		{
			// Token: 0x06009B45 RID: 39749 RVA: 0x00353150 File Offset: 0x00351350
			public override bool Run()
			{
				if (!base.StartSync(false, true, null, 0f, false))
				{
					return false;
				}
				base.StandardEntry();
				base.BeginCommodityUpdates();
				bool flag = base.DoLoop(ExitReason.Default);
				base.FinishLinkedInteraction(false);
				base.EndCommodityUpdates(flag);
				base.StandardExit();
				if (this.mHadDeathFlower)
				{
					return this.Actor.InteractionQueue.TryPushAsContinuation(this, GrimReaperSituation.BeUnReaped.Singleton, this.Actor);
				}
				return flag;
			}

			// Token: 0x0400766D RID: 30317
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.BeReapedDiving.Definition();

			// Token: 0x0400766E RID: 30318
			public bool mHadDeathFlower;

			// Token: 0x020012D2 RID: 4818
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.BeReapedDiving>, IScubaDivingInteractionDefinition
			{
				// Token: 0x06009B48 RID: 39752 RVA: 0x0005DACA File Offset: 0x0005BCCA
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return target.Service is GrimReaper;
				}

				// Token: 0x06009B49 RID: 39753 RVA: 0x0005DADA File Offset: 0x0005BCDA
				public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
				{
					return Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/ReapSoulDiving:InteractionName", new object[0]);
				}
			}
		}

		// Token: 0x020012D3 RID: 4819
		public class PlayChess : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B4B RID: 39755 RVA: 0x0005D921 File Offset: 0x0005BB21
			public PlayChess()
			{
			}

			// Token: 0x06009B4C RID: 39756 RVA: 0x0005D929 File Offset: 0x0005BB29
			public PlayChess(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B4D RID: 39757 RVA: 0x003531C4 File Offset: 0x003513C4
			public override void Init(GrimReaperSituation parent)
			{
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				base.ForceSituationSpecificInteraction(parent.ChessTable, parent.Worker, parent.ChessTable.GetPlayChessWithReaperDefinition(), null, new Callback(this.SetPostChessState), new Callback(parent.ClearChessData), priority);
			}

			// Token: 0x06009B4E RID: 39758 RVA: 0x0005DAFA File Offset: 0x0005BCFA
			private void SetPostChessState(Sim actor, float x)
			{
				this.Parent.SetState(new GrimReaperSituation.PostChessMatch(this.Parent));
			}
		}

		// Token: 0x020012D4 RID: 4820
		public class PostChessMatch : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B4F RID: 39759 RVA: 0x0005D921 File Offset: 0x0005BB21
			public PostChessMatch()
			{
			}

			// Token: 0x06009B50 RID: 39760 RVA: 0x0005D929 File Offset: 0x0005BB29
			public PostChessMatch(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B51 RID: 39761 RVA: 0x00353218 File Offset: 0x00351418
			public override void Init(GrimReaperSituation parent)
			{
				parent.Worker.SimDescription.ShowSocialsOnSim = true;
				parent.Worker.ClearExitReasons();
				string entryKey;
				if (parent.ChessMatchWon)
				{
					entryKey = "Gameplay/Services/GrimReaper:ChessMatchWon";
				}
				else
				{
					entryKey = "Gameplay/Services/GrimReaper:ChessMatchLost";
					Urnstone lastGraveCreated = parent.LastGraveCreated;
					lastGraveCreated.GhostSpawn(false);
					Sim createdSim = lastGraveCreated.DeadSimsDescription.CreatedSim;
					InteractionInstance entry = Urnstone.ResurrectSim.Singleton.CreateInstance(createdSim, createdSim, new InteractionPriority((InteractionPriorityLevel)100), false, true);
					createdSim.InteractionQueue.AddNext(entry);
				}
				StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString(entryKey, new object[]
				{
					parent.ChessOpponent,
					parent.LastGraveCreated.DeadSimsDescription
				}), parent.Worker.ObjectId, StyledNotification.NotificationStyle.kSimTalking);
				StyledNotification.Show(format);
				parent.ClearChessData(parent.Worker, 0f);
			}
		}

		// Token: 0x020012D5 RID: 4821
		public class PetSaveFromDeath : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B52 RID: 39762 RVA: 0x0005D921 File Offset: 0x0005BB21
			public PetSaveFromDeath()
			{
			}

			// Token: 0x06009B53 RID: 39763 RVA: 0x0005D929 File Offset: 0x0005BB29
			public PetSaveFromDeath(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B54 RID: 39764 RVA: 0x003532EC File Offset: 0x003514EC
			public override void Init(GrimReaperSituation parent)
			{
				parent.LastGraveCreated.AddToUseList(parent.PetSavior);
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				base.ForceSituationSpecificInteraction(parent.Worker, parent.PetSavior, GrimReaperSituation.PetHarassDeath.Singleton, null, new Callback(this.PostHarass), new Callback(this.HarassFail), priority);
			}

			// Token: 0x06009B55 RID: 39765 RVA: 0x0035334C File Offset: 0x0035154C
			private void PostHarass(Sim actor, float x)
			{
				this.Parent.PetSavior.InteractionQueue.AddNext(GrimReaperSituation.PetAngryWait.Singleton.CreateInstance(this.Parent.LastGraveCreated, this.Parent.PetSavior, new InteractionPriority((InteractionPriorityLevel)100), false, true));
				this.Parent.SetState(new GrimReaperSituation.DeathResurrect(this.Parent));
			}

			// Token: 0x06009B56 RID: 39766 RVA: 0x0005DB12 File Offset: 0x0005BD12
			private void HarassFail(Sim actor, float x)
			{
				this.Parent.ClearHarassData(actor, x);
			}
		}

		// Token: 0x020012D6 RID: 4822
		private sealed class PetHarassDeath : SocialInteraction
		{
			// Token: 0x06009B57 RID: 39767 RVA: 0x003533B4 File Offset: 0x003515B4
			public override bool Run()
			{
				this.Actor.RequestWalkStyle(Sim.WalkStyle.Run);
				this.SocialJig = SocialJigTwoPerson.CreateJigForTwoPersonSocial(this.Actor, this.Target);
				if (!base.BeginSocialInteraction(new SocialInteractionB.Definition(null, Localization.LocalizeString(this.Target.IsFemale, GrimReaperSituation.PetHarassDeath.kGetHarassedKey, new object[]
				{
					this.Target
				}), false), true, false))
				{
					return false;
				}
				this.Actor.InteractionQueue.CancelAllInteractionsByType(Sim.PetStartle.Singleton);
				base.BeginCommodityUpdates();
				base.EnterStateMachine("HarassDeath", "Enter", "y", "x");
				base.AnimateJoinSims("Accept");
				base.EndCommodityUpdates(true);
				base.FinishLinkedInteraction();
				base.WaitForSyncComplete();
				return true;
			}

			// Token: 0x06009B58 RID: 39768 RVA: 0x0005DB21 File Offset: 0x0005BD21
			public override void Cleanup()
			{
				this.Actor.UnrequestWalkStyle(Sim.WalkStyle.Run);
				base.Cleanup();
			}

			// Token: 0x0400766F RID: 30319
			private static string kGetHarassedKey = "Gameplay/Situations/GrimReaper:GetHarassedByPet";

			// Token: 0x04007670 RID: 30320
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.PetHarassDeath.Definition();

			// Token: 0x020012D7 RID: 4823
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.PetHarassDeath>
			{
				// Token: 0x06009B5B RID: 39771 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012D8 RID: 4824
		private sealed class PetAngryWait : Interaction<Sim, Urnstone>
		{
			// Token: 0x06009B5D RID: 39773 RVA: 0x00353474 File Offset: 0x00351674
			public override bool Run()
			{
				if (!this.Actor.RouteToObjectRadius(this.Target, 1f))
				{
					return false;
				}
				base.BeginCommodityUpdates();
				bool flag = this.DoLoop(ExitReason.Default, new InteractionInstance.InsideLoopFunction(this.ReactionLoop), null);
				base.EndCommodityUpdates(flag);
				return flag;
			}

			// Token: 0x06009B5E RID: 39774 RVA: 0x003534C4 File Offset: 0x003516C4
			private void ReactionLoop(StateMachineClient smc, InteractionInstance.LoopData data)
			{
				if (data.mLifeTime > GrimReaperSituation.PetAngryWait.kTotalPetWaitTime)
				{
					this.Actor.AddExitReason(ExitReason.TimedOut);
					return;
				}
				this.mLastAngryReaction += data.mDeltaTime;
				if (this.mLastAngryReaction >= GrimReaperSituation.PetAngryWait.kTimeBetweenAngryWaitAnims)
				{
					this.Actor.PlayReaction(ReactionTypes.Angry, ReactionSpeed.Immediate);
					this.mLastAngryReaction = 0f;
				}
			}

			// Token: 0x04007671 RID: 30321
			[Tunable]
			[TunableComment("Time between angry animations while pet is waiting for sim to be resurrected")]
			private static float kTimeBetweenAngryWaitAnims = 5f;

			// Token: 0x04007672 RID: 30322
			[Tunable]
			[TunableComment("Total time pet will wait for death to resurrect sim")]
			private static float kTotalPetWaitTime = 60f;

			// Token: 0x04007673 RID: 30323
			private float mLastAngryReaction;

			// Token: 0x04007674 RID: 30324
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.PetAngryWait.Definition();

			// Token: 0x020012D9 RID: 4825
			private sealed class Definition : InteractionDefinition<Sim, Urnstone, GrimReaperSituation.PetAngryWait>
			{
				// Token: 0x06009B61 RID: 39777 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Urnstone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012DA RID: 4826
		public class DeathResurrect : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B63 RID: 39779 RVA: 0x0005D921 File Offset: 0x0005BB21
			public DeathResurrect()
			{
			}

			// Token: 0x06009B64 RID: 39780 RVA: 0x0005D929 File Offset: 0x0005BB29
			public DeathResurrect(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B65 RID: 39781 RVA: 0x00353528 File Offset: 0x00351728
			public override void Init(GrimReaperSituation parent)
			{
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				GrimReaperSituation.DeathResurrectSim deathResurrectSim = base.ForceSituationSpecificInteraction(this.Parent.LastGraveCreated, parent.Worker, GrimReaperSituation.DeathResurrectSim.Singleton, null, new Callback(this.PostResurrect), new Callback(this.ResurrectFail), priority) as GrimReaperSituation.DeathResurrectSim;
				deathResurrectSim.ShouldSendRessurectEvent = false;
			}

			// Token: 0x06009B66 RID: 39782 RVA: 0x0005DB7C File Offset: 0x0005BD7C
			private void PostResurrect(Sim actor, float x)
			{
				this.Parent.SetState(new GrimReaperSituation.PostPetSave(this.Parent));
			}

			// Token: 0x06009B67 RID: 39783 RVA: 0x0005DB12 File Offset: 0x0005BD12
			private void ResurrectFail(Sim actor, float x)
			{
				this.Parent.ClearHarassData(actor, x);
			}
		}

		// Token: 0x020012DB RID: 4827
		private sealed class DeathResurrectSim : Interaction<Sim, Urnstone>
		{
			// Token: 0x06009B68 RID: 39784 RVA: 0x00353588 File Offset: 0x00351788
			public override bool Run()
			{
				this.Actor.RouteToObjectRadius(this.Target, 1f);
				bool flag = true;
				if (this.Target.DeadSimsDescription.CreatedSim != null)
				{
					flag = this.DoLoop(ExitReason.TimedOut | ExitReason.Finished, new InteractionInstance.InsideLoopFunction(this.WaitForSimDeath), null);
				}
				if (flag)
				{
					this.Target.GhostSpawn(false);
					Sim createdSim = this.Target.DeadSimsDescription.CreatedSim;
					Urnstone.ResurrectSim resurrectSim = Urnstone.ResurrectSim.Singleton.CreateInstance(createdSim, createdSim, new InteractionPriority((InteractionPriorityLevel)100), false, true) as Urnstone.ResurrectSim;
					resurrectSim.ShouldSendRessurectEvent = this.ShouldSendRessurectEvent;
					createdSim.InteractionQueue.AddNext(resurrectSim);
				}
				return flag;
			}

			// Token: 0x06009B69 RID: 39785 RVA: 0x0005DB94 File Offset: 0x0005BD94
			private void WaitForSimDeath(StateMachineClient smc, InteractionInstance.LoopData data)
			{
				if (this.Target.DeadSimsDescription.CreatedSim == null)
				{
					this.Actor.AddExitReason(ExitReason.Finished);
					return;
				}
				if (data.mLifeTime > GrimReaperSituation.DeathResurrectSim.kTotalDeathWaitTime)
				{
					this.Actor.AddExitReason(ExitReason.TimedOut);
				}
			}

			// Token: 0x04007675 RID: 30325
			[TunableComment("Total time death will wait for sim to completely die")]
			[Tunable]
			private static float kTotalDeathWaitTime = 60f;

			// Token: 0x04007676 RID: 30326
			public bool ShouldSendRessurectEvent = true;

			// Token: 0x04007677 RID: 30327
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.DeathResurrectSim.Definition();

			// Token: 0x020012DC RID: 4828
			private sealed class Definition : InteractionDefinition<Sim, Urnstone, GrimReaperSituation.DeathResurrectSim>
			{
				// Token: 0x06009B6C RID: 39788 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Urnstone target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012DD RID: 4829
		public class PostPetSave : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B6E RID: 39790 RVA: 0x0005D921 File Offset: 0x0005BB21
			public PostPetSave()
			{
			}

			// Token: 0x06009B6F RID: 39791 RVA: 0x0005D929 File Offset: 0x0005BB29
			public PostPetSave(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B70 RID: 39792 RVA: 0x0035362C File Offset: 0x0035182C
			public override void Init(GrimReaperSituation parent)
			{
				string entryKey = "Gameplay/Services/GrimReaper:SavedByPet";
				StyledNotification.Format format = new StyledNotification.Format(Localization.LocalizeString(entryKey, new object[]
				{
					this.Parent.PetSavior,
					this.Parent.LastGraveCreated.DeadSimsDescription
				}), this.Parent.Worker.ObjectId, StyledNotification.NotificationStyle.kGameMessagePositive);
				StyledNotification.Show(format);
				this.PushPetSocial(this.Parent.PetSavior, this.Parent.LastGraveCreated.DeadSimsDescription.CreatedSim);
				Relationship relationship = Relationship.Get(this.Parent.PetSavior, this.Parent.LastGraveCreated.DeadSimsDescription.CreatedSim, false);
				relationship.HasSavedFromDeath = true;
				EventTracker.SendEvent(EventTypeId.kPetSavedSimFromDeath, this.Parent.PetSavior, this.Parent.LastGraveCreated.DeadSimsDescription.CreatedSim);
				base.ForceSituationSpecificInteraction(parent.Worker, parent.Worker, GrimReaperSituation.DeathReleifReaction.Singleton, null, new Callback(this.Parent.ClearHarassData), new Callback(this.Parent.ClearHarassData));
			}

			// Token: 0x06009B71 RID: 39793 RVA: 0x00353748 File Offset: 0x00351948
			private void PushPetSocial(Sim pet, Sim savedSim)
			{
				if (pet.IsADogSpecies)
				{
					SocialInteractionA.Definition definition = new SocialInteractionA.Definition("LickSim", new string[0], null, false);
					InteractionInstance entry = definition.CreateInstance(savedSim, pet, new InteractionPriority((InteractionPriorityLevel)100), false, true);
					pet.InteractionQueue.AddNext(entry);
					return;
				}
				if (pet.IsCat)
				{
					SocialInteractionA.Definition definition2 = new SocialInteractionA.Definition("PraisePet", new string[0], null, false);
					InteractionInstance entry2 = definition2.CreateInstance(pet, savedSim, new InteractionPriority((InteractionPriorityLevel)100), false, true);
					savedSim.InteractionQueue.Add(entry2);
					return;
				}
				if (pet.IsHorse)
				{
					SocialInteractionA.Definition definition3 = new SocialInteractionA.Definition("Nuzzle", new string[0], null, false);
					InteractionInstance entry3 = definition3.CreateInstance(savedSim, pet, new InteractionPriority((InteractionPriorityLevel)100), false, true);
					pet.InteractionQueue.AddNext(entry3);
				}
			}
		}

		// Token: 0x020012DE RID: 4830
		private sealed class DeathReleifReaction : Interaction<Sim, Sim>
		{
			// Token: 0x06009B72 RID: 39794 RVA: 0x0005DBFD File Offset: 0x0005BDFD
			public override bool Run()
			{
				base.StandardEntry();
				base.EnterStateMachine("DeathRelief", "Enter", "x");
				base.AnimateSim("Exit");
				base.StandardExit();
				return true;
			}

			// Token: 0x04007678 RID: 30328
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.DeathReleifReaction.Definition();

			// Token: 0x020012DF RID: 4831
			private sealed class Definition : SoloSimInteractionDefinition<GrimReaperSituation.DeathReleifReaction>
			{
				// Token: 0x06009B75 RID: 39797 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}
			}
		}

		// Token: 0x020012E0 RID: 4832
		public class Socializing : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B77 RID: 39799 RVA: 0x0005D921 File Offset: 0x0005BB21
			private Socializing()
			{
			}

			// Token: 0x06009B78 RID: 39800 RVA: 0x0005D929 File Offset: 0x0005BB29
			public Socializing(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B79 RID: 39801 RVA: 0x0035380C File Offset: 0x00351A0C
			public override void Init(GrimReaperSituation parent)
			{
				parent.RestoreAutonomy();
				parent.Worker.SimDescription.ShowSocialsOnSim = true;
				VisitSituation.SetVisitToSocializing(parent.Worker);
				this.mAlarmHandle = base.AlarmManager.AddAlarmRepeating(0.05f, TimeUnit.Hours, new AlarmTimerCallback(this.CheckForNextState), 0.05f, TimeUnit.Hours, "Grim Reaper Hanging Out", AlarmType.DeleteOnReset, parent.Worker);
			}

			// Token: 0x06009B7A RID: 39802 RVA: 0x00353870 File Offset: 0x00351A70
			private void CheckForNextState()
			{
				Sim sim = this.Parent.FindClosestDeadSim();
				if (sim != null)
				{
					this.Parent.SetState(new GrimReaperSituation.Reaping(this.Parent));
					return;
				}
				this.mTimeSocializing += 0.05f;
				if (this.mTimeSocializing >= GrimReaper.DelayBeforeLeaving)
				{
					this.Parent.SetState(new GrimReaperSituation.Leave(this.Parent));
				}
			}

			// Token: 0x06009B7B RID: 39803 RVA: 0x003538D8 File Offset: 0x00351AD8
			public override void CleanUp()
			{
				base.AlarmManager.RemoveAlarm(this.mAlarmHandle);
				this.mAlarmHandle = AlarmHandle.kInvalidHandle;
				this.Parent.Worker.SimDescription.MotivesDontDecay = true;
				this.Parent.Worker.SimDescription.ShowSocialsOnSim = false;
				base.CleanUp();
			}

			// Token: 0x04007679 RID: 30329
			private AlarmHandle mAlarmHandle;

			// Token: 0x0400767A RID: 30330
			private float mTimeSocializing;
		}

		// Token: 0x020012E1 RID: 4833
		public class UseVendingMachine : ChildSituation<GrimReaperSituation>
		{
			// Token: 0x06009B7C RID: 39804 RVA: 0x0005D921 File Offset: 0x0005BB21
			public UseVendingMachine()
			{
			}

			// Token: 0x06009B7D RID: 39805 RVA: 0x0005D929 File Offset: 0x0005BB29
			public UseVendingMachine(GrimReaperSituation parent) : base(parent)
			{
			}

			// Token: 0x06009B7E RID: 39806 RVA: 0x00353934 File Offset: 0x00351B34
			public override void Init(GrimReaperSituation parent)
			{
				InteractionPriority priority = new InteractionPriority((InteractionPriorityLevel)100);
				base.ForceSituationSpecificInteraction(parent.VendingMachine, parent.Worker, parent.VendingMachine.GetReaperSodaDefinition(), null, new Callback(this.PostVendingMachine), null, priority);
			}

			// Token: 0x06009B7F RID: 39807 RVA: 0x0035397C File Offset: 0x00351B7C
			private void PostVendingMachine(Sim actor, float x)
			{
				IVendingMachine vendingMachine;
				while ((vendingMachine = this.Parent.FindDeadlyVendingMachine()) != null)
				{
					vendingMachine.KilledASim = false;
				}
				this.Parent.CheckIfEveryoneIsReaped(actor, x);
			}
		}

		// Token: 0x020012E2 RID: 4834
		private sealed class BeUnReaped : Interaction<Sim, Sim>
		{
			// Token: 0x06009B80 RID: 39808 RVA: 0x003539B0 File Offset: 0x00351BB0
			public override bool Run()
			{
				int level = this.Actor.LotCurrent.IsDivingLot ? 1 : 0;
				Vector3 position = this.Actor.Position;
				Vector3 forwardVector = this.Actor.ForwardVector;
				if (!GlobalFunctions.FindGoodLocationNearbyOnLevel(this.Actor, level, ref position, ref forwardVector, FindGoodLocationBooleans.AllowOnSlopes | FindGoodLocationBooleans.AllowInSea))
				{
					this.Actor.SetPosition(this.Actor.LotHome.GetRandomPosition(false, true));
					this.Actor.PopPosture();
					return false;
				}
				this.Actor.SetPosition(position);
				this.Actor.SetForward(forwardVector);
				this.Actor.PlaySoloAnimation("a_scubaDiving_returnToSurface_part2_x", true, ProductVersion.EP10);
				this.Actor.FadeIn(false, 0.4f);
				this.Actor.PopPosture();
				return this.Actor.SwitchBackToSwimOutfitWithSwimmingSpin();
			}

			// Token: 0x0400767B RID: 30331
			public static readonly InteractionDefinition Singleton = new GrimReaperSituation.BeUnReaped.Definition();

			// Token: 0x020012E3 RID: 4835
			private sealed class Definition : InteractionDefinition<Sim, Sim, GrimReaperSituation.BeUnReaped>
			{
				// Token: 0x06009B83 RID: 39811 RVA: 0x000053D8 File Offset: 0x000035D8
				public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
				{
					return true;
				}

				// Token: 0x06009B84 RID: 39812 RVA: 0x0005DADA File Offset: 0x0005BCDA
				public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
				{
					return Localization.LocalizeString(actor.IsFemale, "Gameplay/Actors/Sim/ReapSoulDiving:InteractionName", new object[0]);
				}
			}
		}
	}
}
