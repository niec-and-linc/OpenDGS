using System;
using System.Collections.Generic;
using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Gardening;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.SimIFace.Enums;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Hud;
using NiecMod.Nra;
using Sims3.Gameplay.Controllers.Niec;
using NiecMod.KillNiec;

namespace Sims3.Gameplay.Objects.Niec
{
    // Token: 0x02000002 RID: 2
    public class DoorOfLifeAndDeath : DoorDouble
    {
        // Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        private static string LocalizeStringDOLAD(string name, params object[] parameters)
        {
            return Localization.LocalizeString(DoorOfLifeAndDeath.sLocalizationKey + ":" + name, parameters);
        }

        // Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
        private static string LocalizeString(bool isFemale, string name, params object[] paramaters)
        {
            return Localization.LocalizeString(isFemale, DoorOfLifeAndDeath.sLocalizationKey + ":" + name, paramaters);
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002084 File Offset: 0x00000284
        public override StateMachineClient AcquireStateMachineClient()
        {
            StateMachineClient stateMachineClient = base.AcquireStateMachineClient();
            stateMachineClient.SetParameter("ignoreTrackMasks", YesOrNo.yes);
            return stateMachineClient;
        }

        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000004 RID: 4 RVA: 0x000020A5 File Offset: 0x000002A5
        public ulong DontFearTheReaper
        {
            get
            {
                return DoorOfLifeAndDeath.kDontFearTheReaper;
            }
        }

        // Token: 0x17000002 RID: 2
        // (get) Token: 0x06000005 RID: 5 RVA: 0x000020AC File Offset: 0x000002AC
        public ulong CrypticWeightGain1
        {
            get
            {
                return DoorOfLifeAndDeath.kCrypticWeightGain1;
            }
        }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x06000006 RID: 6 RVA: 0x000020B3 File Offset: 0x000002B3
        public ulong CrypticWeightGain2
        {
            get
            {
                return DoorOfLifeAndDeath.kCrypticWeightGain2;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x000020BA File Offset: 0x000002BA
        public DoorOfLifeAndDeath()
        {
            DoorOfLifeAndDeath.kFootprint = ResourceUtils.HashString32("doorwayofLnD_ftp_pathingToggle");
        }

        // Token: 0x06000008 RID: 8 RVA: 0x000020D1 File Offset: 0x000002D1
        // Token: 0x06000009 RID: 9 RVA: 0x000020DC File Offset: 0x000002DC
        public override void OnStartup()
        {
            base.OnStartup();
            base.AddInteraction(DoorOfLifeAndDeath.PlayAgainstTheReaper.Singleton);
            //base.AddInteraction(DoorOfLifeAndDeath.Reincarnate.Singleton);
            //base.AddInteraction(DoorOfLifeAndDeath.PitMonster.Singleton);
            //base.AddInteraction(DoorOfLifeAndDeath.SimulateGeneticMerger.Singleton);
            base.AddInteraction(DoorOfLifeAndDeath.CallForTheReaper.Singleton);
            //base.AddInteraction(DoorOfLifeAndDeath.RenameSim.Singleton);
        }

        // Token: 0x0600000A RID: 10 RVA: 0x00002134 File Offset: 0x00000334
        private bool CanSpawnReaper(Sim Actor)
        {
            if (GrimReaper.Instance.NumAssignedSims != 0)
            {
                Actor.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(Actor.IsFemale, "ReaperBusy", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive);
                return false;
            }
            DoorOfLifeAndDeath[] objects = Sims3.Gameplay.Queries.GetObjects<DoorOfLifeAndDeath>();
            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i].mReaper != null)
                {
                    Actor.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(Actor.IsFemale, "ReaperBusy", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive);
                    return false;
                }
            }
            return true;
        }

        // Token: 0x0600000B RID: 11 RVA: 0x000021AC File Offset: 0x000003AC
        private bool SummonReaper(Sim Actor)
        {
            if (!Actor.IsInActiveHousehold) return false;
            if (this.mReaperSimDesciption == null || this.mReaperSimDesciption.IsGhost)
            {
                SimUtils.SimCreationSpec simCreationSpec = new SimUtils.SimCreationSpec();
                simCreationSpec.Normalize();
                simCreationSpec.Age = CASAgeGenderFlags.Elder;
                simCreationSpec.Gender = CASAgeGenderFlags.Male;
                if (kAddActiveHouseholdWithGrimReaper)
                {
                    simCreationSpec.GivenName = "Grim"; //StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperFirstName");
                    simCreationSpec.FamilyName = "Reaper"; //StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperLastName");
                }
                else
                {
                    simCreationSpec.GivenName = StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperFirstName");
                    simCreationSpec.FamilyName = StringTable.GetLocalizedString("Gameplay/SimNames/Custom:GrimReaperLastName");
                }
                
                this.mReaperSimDesciption = simCreationSpec.Instantiate();
                this.mReaperSimDesciption.Marryable = false;
                this.mReaperSimDesciption.CanBeKilledOnJob = false;
                this.mReaperSimDesciption.AgingEnabled = false;
                this.mReaperSimDesciption.Contactable = false;
                this.mReaperSimDesciption.SetBodyShape(0f, 0f);
                this.mReaperSimDesciption.VoiceVariation = VoiceVariationType.C;
                if (kAddActiveHouseholdWithGrimReaper)
                {
                    Household.ActiveHousehold.Add(this.mReaperSimDesciption);
                }
                else
                {
                    Household.NpcHousehold.Add(this.mReaperSimDesciption);
                }
            }
            Sim sim = this.mReaperSimDesciption.Instantiate(Vector3.OutOfWorld);
            sim.DisableSocialsOnSim();
            sim.AddAlarm(DoorOfLifeAndDeath.kReaperLeaves, TimeUnit.Hours, new AlarmTimerCallback(this.CleanupReaperAnimation), "Reaper Clean Exit", AlarmType.AlwaysPersisted);
            sim.AddAlarm(DoorOfLifeAndDeath.kReaperAlarm, TimeUnit.Hours, new AlarmTimerCallback(sim.Destroy), "Failsafe Reaper Cleanup", AlarmType.AlwaysPersisted);
            sim.GreetSimOnLot(base.LotCurrent);
            sim.SetPosition(Vector3.OutOfWorld);
            sim.SetForward(Vector3.OutOfWorld);
            ResourceKey key = ResourceKey.CreateOutfitKey("YmDeath", 0u);
            SimOutfit outfit = new SimOutfit(key);
            SimBuilder simBuilder = new SimBuilder();
            simBuilder.UseCompression = true;
            OutfitUtils.SetOutfit(simBuilder, outfit, null);
            ResourceKey key2 = simBuilder.CacheOutfit("DEBUG_CreatePlayableReaper");
            SimOutfit outfit2 = new SimOutfit(key2);
            this.mReaperSimDesciption.AddOutfit(outfit2, OutfitCategories.Everyday, true);
            Sim.SwitchOutfitHelper switchOutfitHelper = new Sim.SwitchOutfitHelper(sim, OutfitCategories.Everyday, 0);
            switchOutfitHelper.Start();
            switchOutfitHelper.Wait(false);
            switchOutfitHelper.ChangeOutfit();
            switchOutfitHelper.Dispose();
            this.mDeathSmoke = VisualEffect.Create("reaperSmokeConstant");
            this.mDeathSmoke.ParentTo(sim, Sim.FXJoints.Pelvis);
            this.mDeathSmoke.SetAutoDestroy(true);
            this.mDeathSmoke.Start();
            sim.SimDescription.TraitManager.RemoveAllElements();
            sim.SimDescription.TraitManager.AddElement(TraitNames.Brave);
            sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
            sim.SimDescription.TraitManager.AddElement(TraitNames.Lucky);
            sim.SimDescription.TraitManager.AddElement(TraitNames.Athletic);
            sim.SimDescription.TraitManager.AddElement(TraitNames.Perfectionist);
            var entry = DoorOfLifeAndDeath.ReaperInteraction.Singleton.CreateInstance(this, sim, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false) as DoorOfLifeAndDeath.ReaperInteraction;
            sim.InteractionQueue.AddNext(entry);
            this.mReaper = sim;
            this.mSummoningSim = Actor;
            return true;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x000023EB File Offset: 0x000005EB
        private void PositionReaper()
        {
            this.PositionReaper(true);
        }

        // Token: 0x0600000D RID: 13 RVA: 0x000023F4 File Offset: 0x000005F4
        private void PositionReaper(bool Fade)
        {
            if (this.mReaper == null)
            {
                return;
            }
            if (Fade)
            {
                this.mReaper.SetOpacity(0f, 0f);
            }
            this.mReaper.SetPosition(base.GetPositionOfSlot(Slot.RoutingSlot_1));
            this.mReaper.SetForward(base.GetForwardOfSlot(Slot.RoutingSlot_1));
            if (Fade)
            {
                this.mReaper.FadeIn();
            }
            this.mReaper.AddToWorld();
        }

        // Token: 0x0600000E RID: 14 RVA: 0x00002468 File Offset: 0x00000668
        private void CleanupReaper()
        {
            if (this.mDeathSmoke != null)
            {
                this.mDeathSmoke.Stop();
                this.mDeathSmoke.Dispose();
                this.mDeathSmoke = null;
            }
            if (this.mReaper != null)
            {
                this.mReaper.Destroy();
                this.mReaper = null;
            }
            this.mSummoningSim = null;
        }

        // Token: 0x0600000F RID: 15 RVA: 0x000024BC File Offset: 0x000006BC
        private void CleanWanderingReaper()
        {
            if (this.mReaper == null)
            {
                return;
            }
            if (this.mReaper.InteractionQueue == null)
            {
                this.CleanupReaper();
                return;
            }
            if (!this.mReaper.InteractionQueue.HasInteractionOfType(DoorOfLifeAndDeath.ReaperInteraction.Singleton) && this.mReaper.LotCurrent != base.LotCurrent)
            {
                this.CleanupReaper();
            }
        }

        // Token: 0x06000010 RID: 16 RVA: 0x00002518 File Offset: 0x00000718
        private void CleanupReaperAnimation()
        {
            if (this.mSummoningSim != null && this.mReaper != null)
            {
                string name = string.Empty;
                StyledNotification.NotificationStyle style = StyledNotification.NotificationStyle.kGameMessagePositive;
                if (this.mSummoningSim.GetRelationship(this.mReaper, true).CurrentLTRLiking > DoorOfLifeAndDeath.kLTRForLike)
                {
                    name = "ReaperLikes";
                }
                else
                {
                    name = "ReaperDislikes";
                    style = StyledNotification.NotificationStyle.kGameMessageNegative;
                }
                this.mSummoningSim.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(this.mSummoningSim.IsFemale, name, new object[0]), style, this.mReaper.ObjectId);
            }
            DoorOfLifeAndDeath.ReaperLeave entry = DoorOfLifeAndDeath.ReaperLeave.Singleton.CreateInstance(this, this.mReaper, new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior), false, false) as DoorOfLifeAndDeath.ReaperLeave;
            this.mReaper.InteractionQueue.AddNext(entry);
            this.mSummoningSim = null;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x000025D4 File Offset: 0x000007D4
        private void BlockDoor(Sim Actor)
        {
            List<Sim> list = new List<Sim>();
            list.Add(this.mReaper);
            base.EnableFootprintAndPushSims(DoorOfLifeAndDeath.kFootprint, Actor, list);
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00002601 File Offset: 0x00000801
        private void UnblockDoor()
        {
            base.DisableFootprint(DoorOfLifeAndDeath.kFootprint);
        }

        // Token: 0x06000013 RID: 19 RVA: 0x0000260F File Offset: 0x0000080F
        private void CleanupGuitars()
        {
            if (this.mSimGuitar != null)
            {
                this.mSimGuitar.Destroy();
                this.mSimGuitar = null;
            }
            if (this.mReaperGuitar != null)
            {
                this.mReaperGuitar.Destroy();
                this.mReaperGuitar = null;
            }
        }

        // Token: 0x06000014 RID: 20 RVA: 0x00002645 File Offset: 0x00000845
        private void SetupSimSound()
        {
            this.CleanupSound();
            this.mSimGuitarSound = new ObjectSound(this.mSimGuitar.ObjectId, "guitar_e_master_group");
            this.mSimGuitarSound.Start(true);
        }

        // Token: 0x06000015 RID: 21 RVA: 0x00002675 File Offset: 0x00000875
        private void SetupReaperSound()
        {
            this.CleanupSound();
            this.mReaperGuitarSound = new ObjectSound(this.mReaperGuitar.ObjectId, "guitar_e_master_group");
            this.mReaperGuitarSound.Start(true);
        }

        // Token: 0x06000016 RID: 22 RVA: 0x000026A8 File Offset: 0x000008A8
        private void CleanupSound()
        {
            if (this.mSimGuitarSound != null)
            {
                this.mSimGuitarSound.Stop();
                this.mSimGuitarSound.Dispose();
                this.mSimGuitarSound = null;
            }
            if (this.mReaperGuitarSound != null)
            {
                this.mReaperGuitarSound.Stop();
                this.mReaperGuitarSound.Dispose();
                this.mReaperGuitarSound = null;
            }
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000026FF File Offset: 0x000008FF
        public override void DoReset(GameObject.ResetInformation resetInformation)
        {
            this.UnblockDoor();
            this.CleanupGuitars();
            this.CleanupReaper();
            this.CleanupSound();
            base.DoReset(resetInformation);
        }

        // Token: 0x04000002 RID: 2
        private static ulong kDontFearTheReaper = 5462586355023928368UL;

        // Token: 0x04000003 RID: 3
        private static ulong kCrypticWeightGain1 = 5462585255512300608UL;

        // Token: 0x04000004 RID: 4
        private static ulong kCrypticWeightGain2 = 5462585255512300624UL;

        // Token: 0x04000005 RID: 5
        private Sim mReaper;

        // Token: 0x04000006 RID: 6
        private SimDescription mReaperSimDesciption;

        // Token: 0x04000007 RID: 7
        private Sim mSummoningSim;

        // Token: 0x04000008 RID: 8
        private VisualEffect mDeathSmoke;

        // Token: 0x04000009 RID: 9
        private static uint kFootprint;

        // Token: 0x0400000A RID: 10
        protected DoorGuitar mSimGuitar;

        // Token: 0x0400000B RID: 11
        protected DoorGuitar mReaperGuitar;

        // Token: 0x0400000C RID: 12
        private ObjectSound mSimGuitarSound;

        // Token: 0x0400000D RID: 13
        private ObjectSound mReaperGuitarSound;

        // Token: 0x0400000E RID: 14
        [Tunable]
        [TunableComment("Duration a Sim will play while playing against the reaper")]
        private static float kPlayAgainstTheReaperPlayTime = 15f;

        // Token: 0x0400000F RID: 15
        [Tunable]
        [TunableComment("Chance to win against the reaper at guitar levels")]
        private static float[] kChanceToBeatReaper = new float[]
		{
			0f,
			10f,
			20f,
			30f,
			40f,
			50f,
			60f,
			70f,
			80f,
			90f,
			100f
		};

        // Token: 0x04000010 RID: 16
        [Tunable]
        [TunableComment("Chance to get the occult on the baby for male pregnancy.")]
        private static float kChanceMalesGetOccultChild = 75f;

        // Token: 0x04000011 RID: 17
        [TunableComment("Chance to escape the pit monster")]
        [Tunable]
        private static float kChanceToEscapePitMonster = 50f;

        // Token: 0x04000012 RID: 18
        [Tunable]
        [TunableComment("Minimum skill drop when a sim reincarnates")]
        private static int kMinSkillDrop = 1;

        // Token: 0x04000013 RID: 19
        [TunableComment("Maximum skill drop when a sim reincarnates")]
        [Tunable]
        private static int kMaxSkillDrop = 3;

        // Token: 0x04000014 RID: 20
        [Tunable]
        [TunableComment("LTR to get Reaper Likes you TNS")]
        private static float kLTRForLike = 0f;

        // Token: 0x04000015 RID: 21
        [TunableComment("Reaper Leave after so many sim Hours,  Animates Exit")]
        [Tunable]
        private static float kReaperLeaves = 2f;

        // Token: 0x04000016 RID: 22
        [Tunable]
        [TunableComment("Reaper Failsale cleanup after so many sim hours.")]
        private static float kReaperAlarm = 6f;

        // Token: 0x04000017 RID: 23
        [TunableComment("Occult Types for Simulate Genetic Merger")]
        [Tunable]
        private static OccultTypes[] kSGMOccultTypes = new OccultTypes[]
		{
			OccultTypes.None,
			OccultTypes.Fairy,
			OccultTypes.Frankenstein,
			OccultTypes.Genie,
			OccultTypes.Mermaid,
			OccultTypes.PlantSim,
			OccultTypes.Vampire,
			OccultTypes.Vampire,
			OccultTypes.Werewolf,
			OccultTypes.Witch
		};

        // Token: 0x04000018 RID: 24
        [Tunable]
        [TunableComment("Occult Types EP for Simulate Genetic Merger")]
        private static ProductVersion[] kSGMEP = new ProductVersion[]
		{
			ProductVersion.BaseGame,
			ProductVersion.EP7,
			ProductVersion.EP2,
			ProductVersion.EP6,
			ProductVersion.EP10,
			ProductVersion.EP9,
			ProductVersion.EP3,
			ProductVersion.EP7,
			ProductVersion.EP7,
			ProductVersion.EP7
		};

        // Token: 0x04000019 RID: 25
        [TunableComment("Chance to change species if pets is installed.")]
        [Tunable]
        private static float kChanceOfChangeSpecies = 75f;

        // Token: 0x0400001A RID: 26
        [TunableComment("Chance to fail the call for the reaper interaction")]
        [Tunable]
        private static float kChanceToGetPitMonsterOnCallForReaper = 10f;

        // Token: 0x0400001B RID: 27


        // Token: 0x02000003 RID: 3
        private sealed class PlayAgainstTheReaper : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x06000019 RID: 25 RVA: 0x000028BC File Offset: 0x00000ABC
            public override bool Run()
            {
                try
                {
                    if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                    {
                        return false;
                    }
                    if (!this.Target.SummonReaper(this.Actor))
                    {
                        return false;
                    }
                    this.Target.BlockDoor(this.Actor);
                    base.StandardEntry();
                    base.EnterStateMachine("DOLAD_store", "Enter", "x");
                    base.SetActor("door", this.Target);
                    this.Target.mSimGuitar = (GlobalFunctions.CreateObjectOutOfWorld("accessoryDWguitar", (ProductVersion)4294967295u) as DoorGuitar);
                    this.Target.mSimGuitar.SetGeometryState("default");
                    this.Target.mReaperGuitar = (GlobalFunctions.CreateObjectOutOfWorld("accessoryDWguitar", (ProductVersion)4294967295u) as DoorGuitar);
                    this.Target.mReaperGuitar.SetGeometryState("deaths");
                    this.mCurrentStateMachine.SetPropActor("accessorydwguitar", this.Target.mSimGuitar.ObjectId);
                    this.mCurrentStateMachine.SetPropActor("deathGuitar", this.Target.mReaperGuitar.ObjectId);
                    base.SetActor("y", this.Target.mReaper);
                    base.EnterState("y", "Enter");
                    base.BeginCommodityUpdates();
                    base.AnimateJoinSims("PrepPATR");
                    this.Target.PositionReaper();
                    base.AnimateSim("PlayAgainstReaper");
                    this.Target.SetupSimSound();
                    base.DoTimedLoop(DoorOfLifeAndDeath.kPlayAgainstTheReaperPlayTime);
                    base.AnimateSim("EndSimTurn");
                    this.Target.CleanupSound();
                    base.AnimateSim("PrepDeathturn");
                    this.Target.SetupReaperSound();
                    base.AnimateSim("DeathsTurn");
                    base.AnimateSim("Finisher");
                    this.Target.CleanupSound();
                    if (RandomUtil.RandomChance(DoorOfLifeAndDeath.kChanceToBeatReaper[Math.Max(0, this.Actor.SkillManager.GetSkillLevel(SkillNames.Guitar))]))
                    {
                        this.Actor.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(this.Actor.IsFemale, "BeatTheReaper", new object[]
					{
						this.Actor
					}), StyledNotification.NotificationStyle.kGameMessagePositive, this.Actor.ObjectId, this.Target.mReaper.ObjectId);
                        this.Actor.BuffManager.AddElement(this.Target.DontFearTheReaper, Origin.None);
                        base.AnimateSim("WinAgainstReaper");
                        this.mFlower = this.SpawnDeathFlower();
                        if (this.mFlower != null)
                        {
                            this.mFlower.SetOpacity(0f, 0f);
                            this.mFlower.SetPosition(this.Target.GetPositionOfSlot(Slot.RoutingSlot_0));
                            this.mFlower.SetForward(this.Target.GetForwardOfSlot(Slot.RoutingSlot_0));
                            VisualEffect.FireOneShotEffect("store_doorwayLnD_lightning", this.Target, Slot.RoutingSlot_0, VisualEffect.TransitionType.SoftTransition);
                            this.mFlower.AddToWorld();
                            this.mFlower.FadeIn(true);
                        }
                        base.AnimateSim("PrepPATRExit");
                        if (this.mFlower != null)
                        {
                            this.mFlower.FadeOut();
                            this.Actor.Inventory.TryToAdd(this.mFlower);
                        }
                    }
                    else
                    {
                        base.AnimateSim("LoseToReaper");
                        this.Actor.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(this.Actor.IsFemale, "LostToReaper", new object[]
					{
						this.Actor
					}), StyledNotification.NotificationStyle.kGameMessagePositive, this.Actor.ObjectId, this.Target.mReaper.ObjectId);
                        if (RandomUtil.RandomChance(DoorOfLifeAndDeath.kChanceToEscapePitMonster) || this.Actor.BuffManager.HasAnyElement(new BuffNames[]
					{
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain1,
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain2
					}) || !this.Actor.CanBeKilled())
                        {
                            base.AnimateSim("Live");
                        }
                        else
                        {
                            base.AnimateSim("Die");
                            this.mKillSim = true;
                        }
                        base.AnimateSim("PrepPATRDeathExit");
                    }
                    this.Target.mReaper.FadeOut();
                    base.AnimateJoinSims("Exit");
                    if (this.mKillSim)
                    {
                        this.Actor.Kill(SimDescription.DeathType.Drown, this.Target, false);
                        this.Actor.SetOpacity(0f, 0f);
                    }
                    base.EndCommodityUpdates(true);
                    base.StandardExit();
                    return true;
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                    return true;
                }
            }

            // Token: 0x0600001A RID: 26 RVA: 0x00002D33 File Offset: 0x00000F33f
            public override void Cleanup()
            {
                if (base.StandardEntryCalled)
                {
                    this.Target.UnblockDoor();
                    this.Target.CleanupGuitars();
                    this.Target.CleanupReaper();
                }
                base.Cleanup();
            }

            // Token: 0x0600001B RID: 27 RVA: 0x00002D64 File Offset: 0x00000F64
            private DeathFlower SpawnDeathFlower()
            {
                PlantableNonIngredientData seedDataForName = PlantableNonIngredientData.GetSeedDataForName("Death Flower Bush");
                return PlantableNonIngredient.Create(seedDataForName) as DeathFlower;
            }

            // Token: 0x0400001C RID: 28
            private bool mKillSim;

            // Token: 0x0400001D RID: 29
            private DeathFlower mFlower;

            // Token: 0x0400001E RID: 30
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.PlayAgainstTheReaper.Definition();

            // Token: 0x02000004 RID: 4
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.PlayAgainstTheReaper>
            {
                // Token: 0x0600001E RID: 30 RVA: 0x00002D9C File Offset: 0x00000F9C
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    /*
                    target.CleanWanderingReaper();
                    if (!target.IsAllowedThrough(a))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "DoorIsLockedToYou", new object[]
						{
							a
						}));
                        return false;
                    }
                    if (target.mReaper != null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "ReaperBusy", new object[0]));
                        return false;
                    }
                    if ((float)a.SkillManager.GetSkillLevel(SkillNames.Guitar) < DoorOfLifeAndDeath.kSkillLevelToPlayAgainstReaper)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "NotSkilledEnough", new object[]
						{
							a
						}));
                        return false;
                    }
                    */
                    return true;
                }

                // Token: 0x0600001F RID: 31 RVA: 0x00002E43 File Offset: 0x00001043
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    //return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "PlayAgainstTheReaper", new object[0]);
                    return "Play Reaper";
                }
            }
        }

        // Token: 0x02000005 RID: 5
        private sealed class Reincarnate : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x06000021 RID: 33 RVA: 0x00002E64 File Offset: 0x00001064
            public override bool Run()
            {
                if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                {
                    return false;
                }
                Urnstone urnstone = this.PopPicker(this.Actor.Inventory.FindAll<Urnstone>(true));
                if (urnstone == null)
                {
                    return false;
                }
                urnstone.ActorsUsingMe.Add(this.Actor);
                if (!this.Target.SummonReaper(this.Actor))
                {
                    urnstone.RemoveFromUseList(this.Actor);
                    return false;
                }
                Sims3.Gameplay.Gameflow.Singleton.DisableSave(this, "Gameplay/ActorSystems/Pregnancy:DisableSave");
                this.Target.BlockDoor(this.Actor);
                base.StandardEntry();
                base.EnterStateMachine("DOLAD_store", "Enter", "x");
                base.SetActor("door", this.Target);
                base.SetActor("y", this.Target.mReaper);
                base.AnimateSim("PlaceStone");
                this.Target.PositionReaper();
                this.Actor.Inventory.TryToRemove(urnstone);
                urnstone.SetPosition(this.Target.GetPositionOfSlot(Slot.RoutingSlot_0));
                urnstone.SetForward(this.Target.GetForwardOfSlot(Slot.RoutingSlot_0));
                urnstone.AddToWorld();
                urnstone.SetGeometryState("tombstone");
                base.BeginCommodityUpdates();
                this.Target.mReaper.FadeIn();
                base.AnimateSim("Reincarnate");
                Sim sim = this.RunReincarnate(urnstone);
                if (sim == null)
                {
                    base.EndCommodityUpdates(true);
                    base.StandardExit();
                    this.Target.mReaper.FadeOut();
                    base.AnimateSim("Exit");
                    return false;
                }
                this.Actor.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(this.Actor.IsFemale, "ReincarnateTNS", new object[]
				{
					sim
				}), StyledNotification.NotificationStyle.kSimTalking, this.Target.mReaper.ObjectId, sim.ObjectId);
                urnstone.FadeOut(false);
                sim.FadeIn(true);
                urnstone.ActorsUsingMe.Remove(this.Actor);
                urnstone.Destroy();
                base.EndCommodityUpdates(true);
                base.StandardExit();
                this.Target.mReaper.FadeOut();
                base.AnimateSim("Exit");
                Sims3.Gameplay.Gameflow.Singleton.EnableSave(this);
                return true;
            }

            // Token: 0x06000022 RID: 34 RVA: 0x00003096 File Offset: 0x00001296
            public override void Cleanup()
            {
                if (this.Target.mSummoningSim == this.Actor)
                {
                    this.Target.UnblockDoor();
                    this.Target.CleanupReaper();
                }
                base.Cleanup();
            }

            // Token: 0x06000023 RID: 35 RVA: 0x000030C8 File Offset: 0x000012C8
            private Urnstone PopPicker(List<Urnstone> possibleUrns)
            {
                List<ObjectPicker.HeaderInfo> list = new List<ObjectPicker.HeaderInfo>();
                List<ObjectPicker.TabInfo> list2 = new List<ObjectPicker.TabInfo>();
                list.Add(new ObjectPicker.HeaderInfo("Ui/Caption/ObjectPicker:Sim", "Ui/Tooltip/ObjectPicker:LastName", 230));
                if (this.Actor != null)
                {
                    ObjectPicker.TabInfo tabInfo = new ObjectPicker.TabInfo("shop_all_r2", string.Empty, new List<ObjectPicker.RowInfo>());
                    SimDescription simDescription = this.Actor.SimDescription;
                    ulong simDescriptionId = simDescription.SimDescriptionId;
                    foreach (Urnstone urnstone in possibleUrns)
                    {
                        if (urnstone != null && urnstone.DeadSimsDescription.Household != this.Actor.Household)
                        {
                            ObjectPicker.RowInfo rowInfo = new ObjectPicker.RowInfo(urnstone, new List<ObjectPicker.ColumnInfo>());
                            List<ObjectPicker.ColumnInfo> columnInfo = rowInfo.ColumnInfo;
                            columnInfo.Add(new ObjectPicker.TextColumn(urnstone.DeadSimsDescription.FullName));
                            tabInfo.RowInfo.Add(rowInfo);
                        }
                    }
                    list2.Add(tabInfo);
                    List<ObjectPicker.RowInfo> list3 = ObjectPickerDialog.Show(true, ModalDialog.PauseMode.PauseSimulator, DoorOfLifeAndDeath.LocalizeStringDOLAD("PickGhost", new object[0]), Localization.LocalizeString("Ui/Caption/ObjectPicker:OK", new object[0]), Localization.LocalizeString("Ui/Caption/ObjectPicker:Cancel", new object[0]), list2, list, 1, false);
                    Urnstone result = null;
                    if (list3 != null && list3.Count == 1)
                    {
                        result = (list3[0].Item as Urnstone);
                    }
                    return result;
                }
                return null;
            }

            // Token: 0x06000024 RID: 36 RVA: 0x00003230 File Offset: 0x00001430
            private Sim RunReincarnate(Urnstone stone)
            {
                if (stone == null)
                {
                    return null;
                }
                SimDescription deadSimsDescription = stone.DeadSimsDescription;
                bool flag = deadSimsDescription.IsHuman;
                if (RandomUtil.RandomChance(DoorOfLifeAndDeath.kChanceOfChangeSpecies) && GameUtils.IsInstalled(ProductVersion.EP5))
                {
                    flag = !flag;
                }
                SimDescription simDescription = null;
                if (flag)
                {
                    simDescription = Genetics.MakeSim(CASAgeGenderFlags.Baby);
                }
                else
                {
                    CASAgeGenderFlags species = CASAgeGenderFlags.Dog;
                    CASAgeGenderFlags gender = CASAgeGenderFlags.Female;
                    if (RandomUtil.CoinFlip())
                    {
                        species = CASAgeGenderFlags.Cat;
                    }
                    if (RandomUtil.CoinFlip())
                    {
                        gender = CASAgeGenderFlags.Male;
                    }
                    simDescription = GeneticsPet.MakeRandomPet(CASAgeGenderFlags.Child, gender, species);
                }
                if (simDescription == null)
                {
                    return null;
                }
                simDescription.FirstName = deadSimsDescription.FirstName;
                simDescription.LastName = deadSimsDescription.LastName;
                if (simDescription.IsHuman == deadSimsDescription.IsHuman)
                {
                    simDescription.TraitManager.RemoveAllElements();
                    foreach (Trait trait in deadSimsDescription.TraitManager.List)
                    {
                        bool flag2 = false;
                        if (flag && (trait.AgeSpeciesAvailabiltiyFlag & CASAGSAvailabilityFlags.HumanBaby) != CASAGSAvailabilityFlags.None)
                        {
                            flag2 = true;
                        }
                        else if (!flag && simDescription.IsADogSpecies && (trait.AgeSpeciesAvailabiltiyFlag & CASAGSAvailabilityFlags.DogChild) != CASAGSAvailabilityFlags.None)
                        {
                            flag2 = true;
                        }
                        else if (!flag && simDescription.IsCat && (trait.AgeSpeciesAvailabiltiyFlag & CASAGSAvailabilityFlags.CatChild) != CASAGSAvailabilityFlags.None)
                        {
                            flag2 = true;
                        }
                        if (flag2 && simDescription.TraitManager.CanAddTrait((ulong)trait.Guid))
                        {
                            simDescription.TraitManager.AddElement(trait.Guid, trait.IsHidden);
                        }
                    }
                }
                TraitManager traitManager = simDescription.TraitManager;
                int num = traitManager.NumTraitsForAge() - traitManager.CountVisibleTraits();
                if (num > 0)
                {
                    traitManager.AddRandomTrait(num);
                }
                foreach (Skill skill in deadSimsDescription.SkillManager.List)
                {
                    bool flag3 = false;
                    if (flag && (skill.AvailableAgeSpecies & CASAGSAvailabilityFlags.HumanAgeMask) != CASAGSAvailabilityFlags.None)
                    {
                        flag3 = true;
                    }
                    else if (!flag && (skill.AvailableAgeSpecies & CASAGSAvailabilityFlags.AllDomesticAnimalsMask) != CASAGSAvailabilityFlags.None)
                    {
                        flag3 = true;
                    }
                    if (flag3)
                    {
                        Skill skill2 = simDescription.SkillManager.AddElement(skill.Guid);
                        if (skill != null && skill2 != null)
                        {
                            int num2 = Math.Max(0, skill.SkillLevel - RandomUtil.GetInt(DoorOfLifeAndDeath.kMinSkillDrop, DoorOfLifeAndDeath.kMaxSkillDrop));
                            for (int i = skill2.SkillLevel; i < num2; i++)
                            {
                                skill2.ForceGainPointsForLevelUp();
                                if (skill2.GatedFromLevelingUp)
                                {
                                    skill2.ForceLevelUpRequirementsComplete();
                                }
                            }
                        }
                    }
                }
                simDescription.FavoriteColor = deadSimsDescription.FavoriteColor;
                simDescription.FavoriteFood = deadSimsDescription.FavoriteFood;
                simDescription.FavoriteMusic = deadSimsDescription.FavoriteMusic;
                simDescription.Bio = deadSimsDescription.Bio;
                simDescription.Zodiac = deadSimsDescription.Zodiac;
                this.Actor.Household.Add(simDescription);
                Sim sim = simDescription.Instantiate(this.Target.GetPositionOfSlot(Slot.RoutingSlot_0));
                sim.SetOpacity(0f, 0f);
                return sim;
            }

            // Token: 0x0400001F RID: 31
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.Reincarnate.Definition();

            // Token: 0x02000006 RID: 6
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.Reincarnate>
            {
                // Token: 0x06000027 RID: 39 RVA: 0x00003558 File Offset: 0x00001758
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    target.CleanWanderingReaper();
                    if (!target.IsAllowedThrough(a))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "DoorIsLockedToYou", new object[]
						{
							a
						}));
                        return false;
                    }
                    if (target.mReaper != null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "ReaperBusy", new object[0]));
                        return false;
                    }
                    if (!a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "FullFamily", new object[0]));
                        return false;
                    }
                    if (a.Inventory.FindAll<Urnstone>(true).Count > 0)
                    {
                        return true;
                    }
                    greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "needUrnstone", new object[]
					{
						a
					}));
                    return false;
                }

                // Token: 0x06000028 RID: 40 RVA: 0x00003630 File Offset: 0x00001830
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "Reincarnate", new object[]
					{
						actor
					});
                }
            }
        }

        // Token: 0x02000007 RID: 7
        private sealed class PitMonster : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x0600002A RID: 42 RVA: 0x00003664 File Offset: 0x00001864
            public override bool Run()
            {
                if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                {
                    return false;
                }
                this.Target.BlockDoor(this.Actor);
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("DOLAD_store", "Enter", "x");
                base.SetActor("door", this.Target);
                base.AnimateSim("PitMonster");
                if (RandomUtil.RandomChance(DoorOfLifeAndDeath.kChanceToEscapePitMonster) || this.Actor.SimDescription.IsPregnant || this.Actor.BuffManager.HasAnyElement(new BuffNames[]
				{
					(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain1,
					(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain2
				}) || this.Actor.SimDescription.IsGhost || !this.Actor.CanBeKilled())
                {
                    base.AnimateSim("PitMonsterSuccess");
                }
                else
                {
                    base.AnimateSim("PitMonsterFail");
                    if (this.Actor.BuffManager.HasAnyElement(new BuffNames[]
					{
						BuffNames.Zombie,
						BuffNames.PermaZombie
					}))
                    {
                        this.Actor.BuffManager.RemoveElement(BuffNames.PermaZombie);
                        this.Actor.BuffManager.RemoveElement(BuffNames.Zombie);
                        base.AnimateSim("PitMonsterZombie");
                    }
                    else
                    {
                        this.mKillSim = true;
                    }
                }
                base.AnimateSim("Exit");
                if (this.mKillSim)
                {
                    this.Actor.Kill(SimDescription.DeathType.Drown, this.Target, false);
                    this.Actor.SetOpacity(0f, 0f);
                }
                base.EndCommodityUpdates(true);
                base.StandardExit();
                return true;
            }

            // Token: 0x0600002B RID: 43 RVA: 0x0000381D File Offset: 0x00001A1D
            public override void Cleanup()
            {
                this.Target.UnblockDoor();
                base.Cleanup();
            }

            // Token: 0x04000020 RID: 32
            private bool mKillSim;

            // Token: 0x04000021 RID: 33
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.PitMonster.Definition();

            // Token: 0x02000008 RID: 8
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.PitMonster>
            {
                // Token: 0x0600002E RID: 46 RVA: 0x00003844 File Offset: 0x00001A44
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return a != target.mReaper && (a.IsNPC && !a.SimDescription.IsPregnant) && !a.BuffManager.HasAnyElement(new BuffNames[]
					{
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain1,
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain2
					});
                }

                // Token: 0x0600002F RID: 47 RVA: 0x00003899 File Offset: 0x00001A99
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "PitMonster", new object[0]);
                }
            }
        }

        // Token: 0x02000009 RID: 9
        private sealed class SimulateGeneticMerger : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x06000031 RID: 49 RVA: 0x000038BC File Offset: 0x00001ABC
            public override bool Run()
            {
                if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                {
                    return false;
                }
                if (!this.Target.SummonReaper(this.Actor))
                {
                    return false;
                }
                this.Target.BlockDoor(this.Actor);
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("DOLAD_store", "Enter", "x");
                base.SetActor("door", this.Target);
                base.SetActor("y", this.Target.mReaper);
                base.AnimateSim("PrepGeneticMerger");
                this.Target.PositionReaper();
                base.AnimateSim("GeneticMerger");
                VisualEffect.FireOneShotEffect("store_doorwayLnD_lightning", this.Target, Slot.RoutingSlot_0, VisualEffect.TransitionType.SoftTransition);
                CameraController.Shake();
                this.Actor.BuffManager.AddElement(BuffNames.SingedElectricity, Origin.None);
                Sim sim = base.GetSelectedObject() as Sim;
                if (!this.Actor.SimDescription.IsPregnant)
                {
                    if (this.Actor.IsFemale && sim != null)
                    {
                        Pregnancy.Start(this.Actor, sim);
                    }
                    else
                    {
                        this.Actor.BuffManager.AddElement(this.Target.CrypticWeightGain1, Origin.None);
                        DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1 buffInstanceCrypticWeightGain = this.Actor.BuffManager.GetElement(this.Target.CrypticWeightGain1) as DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1;
                        Sim sim2 = base.GetSelectedObject() as Sim;
                        if (buffInstanceCrypticWeightGain != null && sim2 != null)
                        {
                            foreach (object obj in Enum.GetValues(typeof(OccultTypes)))
                            {
                                OccultTypes occultTypes = (OccultTypes)obj;
                                if (sim2.OccultManager.HasOccultType(occultTypes))
                                {
                                    buffInstanceCrypticWeightGain.occultType = occultTypes;
                                    break;
                                }
                            }
                            buffInstanceCrypticWeightGain.mSimDescriptionId = sim2.SimDescription.SimDescriptionId;
                        }
                    }
                }
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);
                base.StandardExit();
                return true;
            }

            // Token: 0x06000032 RID: 50 RVA: 0x00003AD4 File Offset: 0x00001CD4
            public override void Cleanup()
            {
                if (this.Target.mSummoningSim == this.Actor)
                {
                    this.Target.UnblockDoor();
                    this.Target.CleanupReaper();
                }
            }

            // Token: 0x06000033 RID: 51 RVA: 0x00003B00 File Offset: 0x00001D00
            public static List<Sim> GetGeneticMergerSims(Sim Actor, Lot targetLot, OccultTypes occultType)
            {
                List<Sim> list = new List<Sim>();
                List<Sim> list2 = new List<Sim>();
                foreach (object obj in LotManager.AllLots)
                {
                    Lot lot = (Lot)obj;
                    list2.AddRange(lot.GetSims());
                }
                for (int i = 0; i < list2.Count; i++)
                {
                    Sim sim = list2[i];
                    if (sim.SimDescription.YoungAdultOrAbove && (sim.Household != Actor.Household || (!Actor.IsBloodRelated(sim) && !Actor.Genealogy.IsStepRelated(sim.Genealogy))) && sim.OccultManager != null)
                    {
                        if (occultType == OccultTypes.None && !sim.OccultManager.HasAnyOccultType())
                        {
                            list.Add(sim);
                        }
                        else if (sim.OccultManager.HasOccultType(occultType))
                        {
                            list.Add(sim);
                        }
                    }
                }
                return list;
            }

            // Token: 0x04000022 RID: 34
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.SimulateGeneticMerger.Definition();

            // Token: 0x0200000A RID: 10
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.SimulateGeneticMerger>
            {
                // Token: 0x06000036 RID: 54 RVA: 0x00003C1C File Offset: 0x00001E1C
                public Definition()
                {
                }

                // Token: 0x06000037 RID: 55 RVA: 0x00003C24 File Offset: 0x00001E24
                private Definition(OccultTypes occult)
                {
                    this.mOccultType = occult;
                }

                // Token: 0x06000038 RID: 56 RVA: 0x00003C34 File Offset: 0x00001E34
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    target.CleanWanderingReaper();
                    if (!target.IsAllowedThrough(a))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "DoorIsLockedToYou", new object[]
						{
							a
						}));
                        return false;
                    }
                    if (a.SimDescription.IsVisuallyPregnant || a.BuffManager.HasAnyElement(new BuffNames[]
					{
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain1,
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain2
					}))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "CurrentlyPregnant", new object[]
						{
							a
						}));
                        return false;
                    }
                    if (target.mReaper != null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "ReaperBusy", new object[0]));
                        return false;
                    }
                    if (!a.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "FullFamily", new object[0]));
                        return false;
                    }
                    return !a.BuffManager.HasAnyElement(new BuffNames[]
					{
						BuffNames.Zombie,
						BuffNames.PermaZombie
					});
                }

                // Token: 0x06000039 RID: 57 RVA: 0x00003D58 File Offset: 0x00001F58
                public override void PopulatePieMenuPicker(ref InteractionInstanceParameters parameters, out List<ObjectPicker.TabInfo> listObjs, out List<ObjectPicker.HeaderInfo> headers, out int NumSelectableRows)
                {
                    if (parameters.Autonomous)
                    {
                        listObjs = null;
                        headers = null;
                        NumSelectableRows = 0;
                        return;
                    }
                    NumSelectableRows = 1;
                    Sim sim = parameters.Actor as Sim;
                    base.PopulateSimPicker(ref parameters, out listObjs, out headers, DoorOfLifeAndDeath.SimulateGeneticMerger.GetGeneticMergerSims(sim, sim.LotCurrent, this.mOccultType), false);
                }

                // Token: 0x0600003A RID: 58 RVA: 0x00003DA4 File Offset: 0x00001FA4
                public override void AddInteractions(InteractionObjectPair iop, Sim actor, DoorOfLifeAndDeath target, List<InteractionObjectPair> results)
                {
                    bool flag = false;
                    for (int i = 0; i < DoorOfLifeAndDeath.kSGMOccultTypes.Length; i++)
                    {
                        if (GameUtils.IsInstalled(DoorOfLifeAndDeath.kSGMEP[i]) && (DoorOfLifeAndDeath.kSGMOccultTypes[i] != OccultTypes.Vampire || flag))
                        {
                            if (DoorOfLifeAndDeath.kSGMOccultTypes[i] == OccultTypes.Vampire)
                            {
                                flag = true;
                            }
                            results.Add(new InteractionObjectPair(new DoorOfLifeAndDeath.SimulateGeneticMerger.Definition(DoorOfLifeAndDeath.kSGMOccultTypes[i]), iop.Target));
                        }
                    }
                }

                // Token: 0x0600003B RID: 59 RVA: 0x00003E0C File Offset: 0x0000200C
                public override string[] GetPath(bool isFemale)
                {
                    return new string[]
					{
						DoorOfLifeAndDeath.LocalizeString(isFemale, "SimulateGeneticMerger", new object[0]) + Localization.Ellipsis
					};
                }

                // Token: 0x0600003C RID: 60 RVA: 0x00003E3F File Offset: 0x0000203F
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "SGM" + this.mOccultType.ToString(), new object[0]);
                }

                // Token: 0x04000023 RID: 35
                private OccultTypes mOccultType;
            }
        }

        // Token: 0x0200000B RID: 11
        private sealed class CallForTheReaper : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x0600003D RID: 61 RVA: 0x00003E6C File Offset: 0x0000206C
            public override bool Run()
            {
                try
                {
                    if (Actor.IsInActiveHousehold)
                    {
                        if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                        {
                            return false;
                        }

                        bool flag = !RandomUtil.RandomChance(0);

                        
                        bool flagaAT = !RandomUtil.RandomChance(DoorOfLifeAndDeath.kChanceToGetPitMonsterOnCallForReaper);

                        try
                        {
                            if (flag)
                            {
                                if (!this.Target.SummonReaper(this.Actor))
                                {
                                    return false;
                                }
                                this.Target.mReaper.SetOpacity(0f, 0f);
                            }

                            else if (!this.Target.CanSpawnReaper(this.Actor))
                            {
                                return false;
                            }
                        }
                        catch (Exception exception)
                        {
                            NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                            flag = false;
                        }
                        

                        this.Target.BlockDoor(this.Actor);
                        base.StandardEntry();
                        base.BeginCommodityUpdates();
                        base.EnterStateMachine("DOLAD_store", "Enter", "x");
                        base.AddPersistentScriptEventHandler(0u, new SacsEventHandler(this.ProcessEvent));
                        base.SetActor("door", this.Target);
                        base.SetActor("y", this.Target.mReaper);
                        bool flag2 = false;
                        if (!flag && !Actor.IsInActiveHousehold)
                        {
                            base.AnimateSim("PrepCallForReaper");
                            base.AnimateSim("CallForReaperFail");
                            base.AnimateSim("PitMonster");
                            if (RandomUtil.RandomChance(100) || this.Actor.BuffManager.HasAnyElement(new BuffNames[]
					        {
						        (BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain1,
						        (BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain2
					        }))

                            {
                                base.AnimateSim("PitMonsterSuccess");
                            }

                            else
                            {
                                base.AnimateSim("PitMonsterFail");
                                flag2 = true;
                            }

                        }
                        else
                        {
                            base.AnimateSim("PrepCallForReaper");
                            this.Target.PositionReaper(false);
                            base.AnimateSim("CallForReaperSuccess");
                        }
                        base.AnimateSim("Exit");
                        if (flag2)
                        {
                            this.Actor.Kill(SimDescription.DeathType.Drown, this.Target, false);
                            this.Actor.SetOpacity(0f, 0f);
                        }
                        base.EndCommodityUpdates(true);
                        base.StandardExit();
                        if (flag)
                        {
                            this.Target.mReaper.EnableSocialsOnSim();
                            this.Actor.InteractionQueue.Add(new SocialInteractionA.Definition("Chat", new string[0], null, false).CreateInstance(this.Target.mReaper, this.Actor, base.GetPriority(), /*new InteractionPriority(InteractionPriorityLevel.RequiredNPCBehavior),*/ true, true));
                            this.Target.mReaper.InteractionQueue.Add(new SocialInteractionA.Definition("Chat", new string[0], null, false).CreateInstance(this.Actor, this.Target.mReaper,  new InteractionPriority(InteractionPriorityLevel.Autonomous), true, true));
                            this.Actor.Motives.MaxEverything();
                        }
                        return true;
                    }
                    else // If NPC Household
                    {
                        if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                        {
                            return false;
                        }
                        bool flag = !RandomUtil.RandomChance(100);

                        /*
                        if (flag)
                        {
                            if (!this.Target.SummonReaper(this.Actor))
                            {
                                return false;
                            }
                            this.Target.mReaper.SetOpacity(0f, 0f);
                        }
                        */
                        try
                        {
                            this.Target.SummonReaper(this.Actor);
                        }
                        catch (Exception exception)
                        {
                            NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                        }
                        

                        this.Target.BlockDoor(this.Actor);
                        base.StandardEntry();
                        base.BeginCommodityUpdates();
                        base.EnterStateMachine("DOLAD_store", "Enter", "x");
                        base.AddPersistentScriptEventHandler(0u, new SacsEventHandler(this.ProcessEvent));
                        base.SetActor("door", this.Target);
                        base.SetActor("y", this.Target.mReaper);
                        bool flag2 = false;
                        if (!flag)
                        {
                            base.AnimateSim("PrepCallForReaper");
                            base.AnimateSim("CallForReaperFail");
                            base.AnimateSim("PitMonster");
                            if (RandomUtil.RandomChance(0) || this.Actor.BuffManager.HasAnyElement(new BuffNames[]
					    {
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain1,
						(BuffNames)DoorOfLifeAndDeath.kCrypticWeightGain2
					    }))
                            {
                                base.AnimateSim("PitMonsterSuccess");
                            }
                            else
                            {
                                base.AnimateSim("PitMonsterFail");
                                flag2 = true;
                            }
                        }
                        else
                        {
                            base.AnimateSim("PrepCallForReaper");
                            this.Target.PositionReaper(false);
                            base.AnimateSim("CallForReaperSuccess");
                        }
                        base.AnimateSim("Exit");
                        if (flag2)
                        {
                            if (Actor.CanBeKilled())
                            {
                                this.Actor.Kill(SimDescription.DeathType.Drown, Target, false);
                            }
                            else if (KillSimNiecX.CheckNiecKill(Actor))
                            {
                                KillSimNiecX.MineKill(Actor, SimDescription.DeathType.Drown, Target, false);
                            }
                            else
                            {
                                Actor.FadeIn();
                            }
                            this.Actor.SetOpacity(0f, 0f);
                        }
                        base.EndCommodityUpdates(true);
                        base.StandardExit();
                        return true;
                    }
                    //return true;
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    bool flersag = true;
                    NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                    if (Actor.IsInActiveHousehold)
                    {
                        this.Actor.Motives.MaxEverything();
                    }
                    else
                    {
                        if (Actor.CanBeKilled())
                        {
                            this.Actor.Kill(SimDescription.DeathType.Drown, Target, false);
                            flersag = false;
                        }
                        else if (KillSimNiecX.CheckNiecKill(Actor))
                        {
                            KillSimNiecX.MineKill(Actor, SimDescription.DeathType.Drown, Target, false);
                            flersag = false;
                        }
                        else
                        {
                            Actor.FadeIn();
                        }
                        
                    }
                    
                    if (flersag)
                    {
                        Target.SetObjectToReset();
                    }
                    return true;
                }
            }

            // Token: 0x0600003E RID: 62 RVA: 0x0000409E File Offset: 0x0000229E
            public override void Cleanup()
            {
                try
                {
                    this.Target.UnblockDoor();
                    if (this.Target.mReaper != null)
                    {
                        this.Target.mReaper.AddExitReason(ExitReason.Default);
                    }
                    base.Cleanup();
                }
                catch (Exception exception)
                {
                    try
                    {
                        this.Target.UnblockDoor();
                    }
                    catch (Exception)
                    { }
                    NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                    base.Cleanup();
                }
            }

            // Token: 0x0600003F RID: 63 RVA: 0x000040D4 File Offset: 0x000022D4
            public void ProcessEvent(StateMachineClient smc, IEvent evt)
            {
                uint eventId = evt.EventId;
                if (eventId != 666u)
                {
                    return;
                }
                if (this.Target.mReaper != null)
                {
                    this.Target.mReaper.FadeIn();
                }
            }

            // Token: 0x04000024 RID: 36
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.CallForTheReaper.Definition();

            // Token: 0x0200000C RID: 12
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.CallForTheReaper>
            {
                // Token: 0x06000042 RID: 66 RVA: 0x00004124 File Offset: 0x00002324
                public override bool Test(Sim actor, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    /*
                    target.CleanWanderingReaper();
                    if (a == target.mReaper)
                    {
                        return false;
                    }
                    if (!target.IsAllowedThrough(a))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "DoorIsLockedToYou", new object[]
                        {
                            a
                        }));
                        return false;
                    }
                    if (target.mReaper != null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "ReaperBusy", new object[0]));
                        return false;
                    }
                    */
                    if (isAutonomous)
                    {
                        if (actor.IsInActiveHousehold)
                        {
                            if (NTunable.kNoAutonomousWithActiveDOF)
                            {
                                return false;
                            }
                        }
                    }
                    if (actor.Service is GrimReaper) return false;
                    return true;
                }

                // Token: 0x06000043 RID: 67 RVA: 0x0000419A File Offset: 0x0000239A
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return "Call For Reaper";
                }
            }
        }

        [Tunable, TunableComment("No Desc")]
        public static bool kAddActiveHouseholdWithGrimReaper = false;

        // Token: 0x0200000D RID: 13
        private sealed class RenameSim : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x06000045 RID: 69 RVA: 0x000041BC File Offset: 0x000023BC
            public override bool Run()
            {
                if (!UIUtils.IsOkayToStartModalDialog())
                {
                    return false;
                }
                if (!this.Actor.RouteToSlotAndCheckInUse(this.Target, Slot.RoutingSlot_0))
                {
                    return false;
                }
                if (!this.Target.SummonReaper(this.Actor))
                {
                    return false;
                }
                this.Target.BlockDoor(this.Actor);
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("DOLAD_store", "Enter", "x");
                base.SetActor("door", this.Target);
                base.SetActor("y", this.Target.mReaper);
                base.AnimateSim("PrepCallForReaper");
                this.Target.PositionReaper();
                base.AnimateSim("RenameSim");
                if (!UIUtils.IsOkayToStartModalDialog())
                {
                    return false;
                }
                string titleText = Localization.LocalizeString(this.Actor.IsFemale, "Gameplay/Objects/RabbitHoles/CityHall:ChangeNameTitle", new object[0]);
                string promptText = Localization.LocalizeString(this.Actor.IsFemale, "Gameplay/Objects/RabbitHoles/CityHall:ChangeNameFirstName", new object[]
				{
					this.Actor.SimDescription
				});
                string promptText2 = Localization.LocalizeString(this.Actor.IsFemale, "Gameplay/Objects/RabbitHoles/CityHall:ChangeNameLastName", new object[]
				{
					this.Actor.SimDescription
				});
                do
                {
                    this.Actor.SimDescription.FirstName = StringInputDialog.Show(titleText, promptText, this.Actor.SimDescription.FirstName, CASBasics.GetMaxNameLength(), StringInputDialog.Validation.SimNameText);
                }
                while (string.IsNullOrEmpty(this.Actor.SimDescription.FirstName));
                do
                {
                    this.Actor.SimDescription.LastName = StringInputDialog.Show(titleText, promptText2, this.Actor.SimDescription.LastName, CASBasics.GetMaxNameLength(), StringInputDialog.Validation.SimNameText);
                }
                while (string.IsNullOrEmpty(this.Actor.SimDescription.LastName));
                HudModel hudModel = Sims3.UI.Responder.Instance.HudModel as HudModel;
                hudModel.NotifyNameChanged(this.Actor.ObjectId);
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);
                base.StandardExit();
                return true;
            }

            // Token: 0x06000046 RID: 70 RVA: 0x000043C0 File Offset: 0x000025C0
            public override void Cleanup()
            {
                if (this.Target.mSummoningSim == this.Actor)
                {
                    this.Target.UnblockDoor();
                    this.Target.CleanupReaper();
                }
                base.Cleanup();
            }

            // Token: 0x04000025 RID: 37
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.RenameSim.Definition();

            // Token: 0x0200000E RID: 14
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.RenameSim>
            {
                // Token: 0x06000049 RID: 73 RVA: 0x00004408 File Offset: 0x00002608
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    target.CleanWanderingReaper();
                    if (!target.IsAllowedThrough(a))
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "DoorIsLockedToYou", new object[]
						{
							a
						}));
                        return false;
                    }
                    if (target.mReaper != null)
                    {
                        greyedOutTooltipCallback = InteractionInstance.CreateTooltipCallback(DoorOfLifeAndDeath.LocalizeString(a.IsFemale, "ReaperBusy", new object[0]));
                        return false;
                    }
                    return true;
                }

                // Token: 0x0600004A RID: 74 RVA: 0x00004473 File Offset: 0x00002673
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "RenameSim", new object[0]);
                }
            }
        }

        // Token: 0x0200000F RID: 15
        private sealed class ReaperInteraction : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x0600004C RID: 76 RVA: 0x00004494 File Offset: 0x00002694
            public override bool Run()
            {
                base.StandardEntry();
                base.BeginCommodityUpdates();
                bool flag = base.DoLoop(ExitReason.Default);
                base.EndCommodityUpdates(flag);
                base.StandardExit();
                return flag;
            }

            // Token: 0x04000026 RID: 38
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.ReaperInteraction.Definition();

            // Token: 0x02000010 RID: 16
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.ReaperInteraction>
            {
                // Token: 0x0600004F RID: 79 RVA: 0x000044DB File Offset: 0x000026DB
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                // Token: 0x06000050 RID: 80 RVA: 0x000044DE File Offset: 0x000026DE
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "Listening", new object[0]);
                }
            }
        }

        // Token: 0x02000011 RID: 17
        private sealed class ReaperLeave : Interaction<Sim, DoorOfLifeAndDeath>
        {
            // Token: 0x06000052 RID: 82 RVA: 0x00004500 File Offset: 0x00002700
            public override bool Run()
            {
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("DOLAD_store", "Enter", "x");
                base.AnimateSim("DeathLeave");
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);
                base.StandardExit();
                return true;
            }

            // Token: 0x06000053 RID: 83 RVA: 0x00004552 File Offset: 0x00002752
            public override void Cleanup()
            {
                Simulator.AddObject(new Sims3.Gameplay.OneShotFunctionTask(new Sims3.Gameplay.Function(this.Target.CleanupReaper)));
                base.Cleanup();
            }

            // Token: 0x04000027 RID: 39
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.ReaperLeave.Definition();

            // Token: 0x02000012 RID: 18
            private sealed class Definition : InteractionDefinition<Sim, DoorOfLifeAndDeath, DoorOfLifeAndDeath.ReaperLeave>
            {
                // Token: 0x06000056 RID: 86 RVA: 0x0000458A File Offset: 0x0000278A
                public override bool Test(Sim a, DoorOfLifeAndDeath target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                // Token: 0x06000057 RID: 87 RVA: 0x0000458D File Offset: 0x0000278D
                public override string GetInteractionName(Sim actor, DoorOfLifeAndDeath target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "ReturnNether", new object[0]);
                }
            }
        }

        // Token: 0x02000013 RID: 19
        private sealed class MaleHaveBaby : Interaction<Sim, Sim>
        {
            // Token: 0x06000059 RID: 89 RVA: 0x000045B0 File Offset: 0x000027B0
            public override bool Run()
            {
                if (this.Actor.BuffManager.HasAnyElement(new BuffNames[]
				{
					BuffNames.Zombie,
					BuffNames.PermaZombie
				}))
                {
                    this.Actor.SimDescription.SetBodyShape(this.mOldWeight, this.mOldFitness);
                    return false;
                }
                Sim sim = this.SpawnBaby(this.Actor, this.mOccultType);
                if (sim == null)
                {
                    return false;
                }
                TraitManager traitManager = sim.SimDescription.TraitManager;
                int num = traitManager.NumTraitsForAge() - traitManager.CountVisibleTraits();
                if (num > 0)
                {
                    traitManager.AddRandomTrait(num);
                }
                base.StandardEntry();
                base.BeginCommodityUpdates();
                base.EnterStateMachine("DOLAD_store", "Enter", "x");
                base.SetActor("y", sim);
                base.AnimateSim("MaleBirth");
                ChildUtils.CarryChild(this.Actor, sim, true);
                this.Actor.SimDescription.SetBodyShape(this.mOldWeight, this.mOldFitness);
                base.AnimateSim("Exit");
                base.EndCommodityUpdates(true);
                base.StandardExit();
                return true;
            }

            // Token: 0x0600005A RID: 90 RVA: 0x000046C4 File Offset: 0x000028C4
            public override void Cleanup()
            {
                base.Cleanup();
            }

            // Token: 0x0600005B RID: 91 RVA: 0x000046CC File Offset: 0x000028CC
            private Sim SpawnBaby(Sim Actor, OccultTypes newOccult)
            {
                bool flag = false;
                if (RandomUtil.RandomChance(DoorOfLifeAndDeath.kChanceMalesGetOccultChild))
                {
                    flag = true;
                }
                if (!Actor.Household.CanAddSpeciesToHousehold(CASAgeGenderFlags.Human))
                {
                    Actor.ShowTNSIfSelectable(DoorOfLifeAndDeath.LocalizeString(Actor.IsFemale, "FullFamily", new object[0]), StyledNotification.NotificationStyle.kSystemMessage);
                    return null;
                }
                CASAgeGenderFlags gender = RandomUtil.CoinFlip() ? CASAgeGenderFlags.Male : CASAgeGenderFlags.Female;
                SimDescription simDescription = SimDescription.Find(this.mSimDescriptionId);
                SimDescription simDescription2;
                if (flag && newOccult == OccultTypes.PlantSim && simDescription != null)
                {
                    simDescription2 = Genetics.MakePlantSimBaby(simDescription, gender, 100f, new Random());
                }
                else
                {
                    simDescription2 = Genetics.MakeSim(CASAgeGenderFlags.Baby);
                    if (simDescription != null)
                    {
                        simDescription.Genealogy.AddChild(simDescription2.Genealogy);
                    }
                }
                Actor.Genealogy.AddChild(simDescription2.Genealogy);
                if (simDescription2 == null)
                {
                    return null;
                }
                simDescription2.FirstName = SimUtils.GetRandomGivenName(simDescription2.IsMale, GameUtils.GetCurrentWorld());
                simDescription2.LastName = Actor.LastName;
                if (flag && newOccult != OccultTypes.None)
                {
                    simDescription2.OccultManager.AddOccultType(newOccult, true, false, false);
                }
                Actor.Household.Add(simDescription2);
                return simDescription2.Instantiate(Vector3.OutOfWorld);
            }

            // Token: 0x04000028 RID: 40
            public OccultTypes mOccultType;

            // Token: 0x04000029 RID: 41
            public float mOldWeight;

            // Token: 0x0400002A RID: 42
            public float mOldFitness;

            // Token: 0x0400002B RID: 43
            public ulong mSimDescriptionId;

            // Token: 0x0400002C RID: 44
            public static readonly InteractionDefinition Singleton = new DoorOfLifeAndDeath.MaleHaveBaby.Definition();

            // Token: 0x02000014 RID: 20
            private sealed class Definition : InteractionDefinition<Sim, Sim, DoorOfLifeAndDeath.MaleHaveBaby>
            {
                // Token: 0x0600005E RID: 94 RVA: 0x000047F7 File Offset: 0x000029F7
                public override bool Test(Sim a, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
                {
                    return true;
                }

                // Token: 0x0600005F RID: 95 RVA: 0x000047FC File Offset: 0x000029FC
                public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
                {
                    return DoorOfLifeAndDeath.LocalizeString(actor.IsFemale, "havingBaby", new object[]
					{
						actor
					});
                }
            }
        }

        // Token: 0x02000015 RID: 21
        public class DOLAD_CrypticWeightGain1 : Buff
        {
            // Token: 0x06000061 RID: 97 RVA: 0x0000482D File Offset: 0x00002A2D
            public DOLAD_CrypticWeightGain1(Buff.BuffData info)
                : base(info)
            {
            }

            // Token: 0x06000062 RID: 98 RVA: 0x00004838 File Offset: 0x00002A38
            public override void OnAddition(BuffManager bm, BuffInstance bi, bool travelReaddition)
            {
                base.OnAddition(bm, bi, travelReaddition);
                DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1 buffInstanceCrypticWeightGain = bi as DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1;
                if (buffInstanceCrypticWeightGain != null)
                {
                    buffInstanceCrypticWeightGain.oldWeight = bm.Actor.SimDescription.Weight;
                    buffInstanceCrypticWeightGain.oldFitness = bm.Actor.SimDescription.Fitness;
                }
                bm.Actor.SimDescription.SetBodyShape(0.5f, 0f);
            }

            // Token: 0x06000063 RID: 99 RVA: 0x000048A0 File Offset: 0x00002AA0
            public override void OnTimeout(BuffManager bm, BuffInstance bi, Buff.OnTimeoutReasons reason)
            {
                base.OnTimeout(bm, bi, reason);
                bm.AddElement(DoorOfLifeAndDeath.kCrypticWeightGain2, Origin.None);
                DoorOfLifeAndDeath.DOLAD_CrypticWeightGain2.BuffInstanceCrypticWeightGain2 buffInstanceCrypticWeightGain = bm.GetElement(DoorOfLifeAndDeath.kCrypticWeightGain2) as DoorOfLifeAndDeath.DOLAD_CrypticWeightGain2.BuffInstanceCrypticWeightGain2;
                DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1 buffInstanceCrypticWeightGain2 = bi as DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1;
                if (buffInstanceCrypticWeightGain != null && buffInstanceCrypticWeightGain2 != null)
                {
                    buffInstanceCrypticWeightGain.occultType = buffInstanceCrypticWeightGain2.occultType;
                    buffInstanceCrypticWeightGain.oldWeight = buffInstanceCrypticWeightGain2.oldWeight;
                    buffInstanceCrypticWeightGain.oldFitness = buffInstanceCrypticWeightGain2.oldFitness;
                    buffInstanceCrypticWeightGain.mSimDescriptionId = buffInstanceCrypticWeightGain2.mSimDescriptionId;
                }
            }

            // Token: 0x06000064 RID: 100 RVA: 0x00004912 File Offset: 0x00002B12
            public override BuffInstance CreateBuffInstance()
            {
                return new DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1(this, base.BuffGuid, base.EffectValue, base.TimeoutSimMinutes);
            }

            // Token: 0x02000016 RID: 22
            public class BuffInstanceCrypticWeightGain1 : BuffInstance
            {
                // Token: 0x06000065 RID: 101 RVA: 0x0000492C File Offset: 0x00002B2C
                public BuffInstanceCrypticWeightGain1()
                {
                }

                // Token: 0x06000066 RID: 102 RVA: 0x00004934 File Offset: 0x00002B34
                public BuffInstanceCrypticWeightGain1(Buff buff, BuffNames buffGuid, int effectValue, float timeoutCount)
                    : base(buff, buffGuid, effectValue, timeoutCount)
                {
                }

                // Token: 0x06000067 RID: 103 RVA: 0x00004941 File Offset: 0x00002B41
                public override BuffInstance Clone()
                {
                    return new DoorOfLifeAndDeath.DOLAD_CrypticWeightGain1.BuffInstanceCrypticWeightGain1(this.mBuff, this.mBuffGuid, this.mEffectValue, this.mTimeoutCount);
                }

                // Token: 0x0400002D RID: 45
                public OccultTypes occultType;

                // Token: 0x0400002E RID: 46
                public float oldWeight;

                // Token: 0x0400002F RID: 47
                public float oldFitness;

                // Token: 0x04000030 RID: 48
                public ulong mSimDescriptionId;
            }
        }

        // Token: 0x02000017 RID: 23
        public class DOLAD_CrypticWeightGain2 : Buff
        {
            // Token: 0x06000068 RID: 104 RVA: 0x00004960 File Offset: 0x00002B60
            public DOLAD_CrypticWeightGain2(Buff.BuffData info)
                : base(info)
            {
            }

            // Token: 0x06000069 RID: 105 RVA: 0x00004969 File Offset: 0x00002B69
            public override void OnAddition(BuffManager bm, BuffInstance bi, bool travelReaddition)
            {
                base.OnAddition(bm, bi, travelReaddition);
                bm.Actor.SimDescription.SetBodyShape(1f, 0f);
            }

            // Token: 0x0600006A RID: 106 RVA: 0x00004990 File Offset: 0x00002B90
            public override void OnTimeout(BuffManager bm, BuffInstance bi, Buff.OnTimeoutReasons reason)
            {
                base.OnTimeout(bm, bi, reason);
                DoorOfLifeAndDeath.DOLAD_CrypticWeightGain2.BuffInstanceCrypticWeightGain2 buffInstanceCrypticWeightGain = bi as DoorOfLifeAndDeath.DOLAD_CrypticWeightGain2.BuffInstanceCrypticWeightGain2;
                if (buffInstanceCrypticWeightGain != null)
                {
                    DoorOfLifeAndDeath.MaleHaveBaby maleHaveBaby = DoorOfLifeAndDeath.MaleHaveBaby.Singleton.CreateInstance(bm.Actor, bm.Actor, new InteractionPriority(InteractionPriorityLevel.CriticalNPCBehavior), false, false) as DoorOfLifeAndDeath.MaleHaveBaby;
                    maleHaveBaby.mOccultType = buffInstanceCrypticWeightGain.occultType;
                    maleHaveBaby.mOldWeight = buffInstanceCrypticWeightGain.oldWeight;
                    maleHaveBaby.mOldFitness = buffInstanceCrypticWeightGain.oldFitness;
                    maleHaveBaby.mSimDescriptionId = buffInstanceCrypticWeightGain.mSimDescriptionId;
                    bm.Actor.InteractionQueue.AddNext(maleHaveBaby);
                }
            }

            // Token: 0x0600006B RID: 107 RVA: 0x00004A16 File Offset: 0x00002C16
            public override BuffInstance CreateBuffInstance()
            {
                return new DoorOfLifeAndDeath.DOLAD_CrypticWeightGain2.BuffInstanceCrypticWeightGain2(this, base.BuffGuid, base.EffectValue, base.TimeoutSimMinutes);
            }

            // Token: 0x02000018 RID: 24
            public class BuffInstanceCrypticWeightGain2 : BuffInstance
            {
                // Token: 0x0600006C RID: 108 RVA: 0x00004A30 File Offset: 0x00002C30
                public BuffInstanceCrypticWeightGain2()
                {
                }

                // Token: 0x0600006D RID: 109 RVA: 0x00004A38 File Offset: 0x00002C38
                public BuffInstanceCrypticWeightGain2(Buff buff, BuffNames buffGuid, int effectValue, float timeoutCount)
                    : base(buff, buffGuid, effectValue, timeoutCount)
                {
                }

                // Token: 0x0600006E RID: 110 RVA: 0x00004A45 File Offset: 0x00002C45
                public override BuffInstance Clone()
                {
                    return new DoorOfLifeAndDeath.DOLAD_CrypticWeightGain2.BuffInstanceCrypticWeightGain2(this.mBuff, this.mBuffGuid, this.mEffectValue, this.mTimeoutCount);
                }

                // Token: 0x04000031 RID: 49
                public OccultTypes occultType;

                // Token: 0x04000032 RID: 50
                public float oldWeight;

                // Token: 0x04000033 RID: 51
                public float oldFitness;

                // Token: 0x04000034 RID: 52
                public ulong mSimDescriptionId;
            }
        }
    }
}
