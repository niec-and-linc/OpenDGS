/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 20/10/2018
 * Time: 2:26
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
using NiecMod.Interactions;
using NiecMod.KillNiec;
using Sims3.Gameplay.Controllers.Niec;
using NRaas;
using NRaas.CommonSpace.Helpers;
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
using Sims3.Gameplay.StoryProgression;
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
using Sims3.Gameplay;
using Sims3.Gameplay.CAS.Locale;
using Niec.iCommonSpace;
 
namespace NiecMod.Nra
{
    public class HelperNraPro
    {
        /*
        public HelperNraPro()
        {
            if (sHelperNraProList == null) { sHelperNraProList = new List<HelperNraPro>(); }







        }
        [PersistableStatic]
        public static List<HelperNraPro> sHelperNraProList;
        */

        public Sim mSim = null;

        public HelperNraPro test = null;

        public SimDescription mSimdesc = null;

        public SimDescription.DeathType mdeathtype = SimDescription.DeathType.OldAge;

        public Vector3 mHouseVeri3 = Vector3.Invalid;

        public AlarmHandle malarmx = AlarmHandle.kInvalidHandle;

        public Household mHousehold = null;

        /*
        public static HelperNraPro FoundSleep()
        {
            //Type type = this.GetType();
            //HelperNraPro t
            
            //return default;
            return t;
        }

        */

        public void FailedCallBookSleep()
        {
            if (mSimdesc == null) return;
            if (mSimdesc.CreatedSim == null) return;
            if (mSim == null) return;
            if (mSim.InteractionQueue == null) return;
            
            try
            {
                if (!mSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                {
                    return;
                }
            }
            catch
            { }
            
            try
            {
                if (!mSimdesc.IsValidDescription)
                {
                    if (mSimdesc.Genealogy == null)
                    {
                        mSimdesc.mGenealogy = new Genealogy(mSimdesc);
                    }
                    mSimdesc.Fixup();
                }
                
            }
            catch
            { }
            try
            {
                if (mSimdesc.Household == null)
                {
                    if (mHousehold != null)
                    {
                        mHousehold.Add(mSimdesc);
                    }
                    else
                    {
                        Household.NpcHousehold.Add(mSimdesc);
                    }
                }
            }
            catch
            { Household.NpcHousehold.Add(mSimdesc); }

            if (mHouseVeri3 == null || mHouseVeri3 == Vector3.Invalid)
            {


                if (Household.ActiveHousehold != null)
                {
                    try
                    {
                        List<SimDescription> listxt = new List<SimDescription>();
                        foreach (SimDescription simat in Household.ActiveHousehold.SimDescriptions)
                        {
                            listxt.Add(simat);
                        }
                        if (listxt.Count != 0)
                        {
                            foreach (SimDescription item in listxt)
                            {
                                Sim sim = item.CreatedSim;
                                if (sim == null) continue;
                                if (sim.LotCurrent != null && sim.LotCurrent.IsWorldLot) continue;
                                mHouseVeri3 = sim.Position;
                                break;
                            }
                        }
                        else
                        {
                            AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(FailedCallBookSleep), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);
                            return;
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(FailedCallBookSleep), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);
                    return;
                }
            }
            try
            {
                /*
                if (malarmx != AlarmHandle.kInvalidHandle)
                {
                    AlarmManager.Global.RemoveAlarm(malarmx);
                }
                 * */
                try
                {
                    if (mSim.SimDescription.DeathStyle != SimDescription.DeathType.None && mSim.SimDescription.IsGhost || mSim.SimDescription.IsDead)
                    {
                        try
                        {
                            SimDescription simDescriptiongot = mSim.SimDescription;
                            simDescriptiongot.IsGhost = false;
                            World.ObjectSetGhostState(mSim.ObjectId, 0u, (uint)simDescriptiongot.AgeGenderSpecies);
                            mSim.Autonomy.Motives.RemoveMotive(CommodityKind.BeGhostly);
                            simDescriptiongot.SetDeathStyle(SimDescription.DeathType.None, false);
                            simDescriptiongot.ShowSocialsOnSim = true;
                            simDescriptiongot.IsNeverSelectable = false;
                            mSim.Autonomy.AllowedToRunMetaAutonomy = true;
                            SpeedTrap.Sleep();
                        }
                        catch
                        { }

                        finally
                        {
                            if (mdeathtype == SimDescription.DeathType.None)
                            {
                                mSimdesc.SetDeathStyle(SimDescription.DeathType.OldAge, false);
                            }
                            else
                            {
                                mSimdesc.SetDeathStyle(mdeathtype, false);
                            }

                        }
                    }
                }
                catch
                { }
                
                
                if (NTunable.kCheckFailed)
                {
                    if (!GlobalFunctions.PlaceAtGoodLocation(mSim, new World.FindGoodLocationParams(mHouseVeri3), false))
                    {
                        try
                        {
                            if (PlumbBob.Singleton.mSelectedActor != null && !PlumbBob.Singleton.mSelectedActor.LotCurrent.IsWorldLot)
                            {
                                mHouseVeri3 = PlumbBob.Singleton.mSelectedActor.Position;
                                if (!GlobalFunctions.PlaceAtGoodLocation(mSim, new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position), false))
                                {
                                    mSim.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                                }




                                GrimReaper.Instance.MakeServiceRequest(PlumbBob.Singleton.mSelectedActor.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                            }
                            else
                            {
                                bool yesRamdon = false;
                                Sim simrandom = null;
                                if (Household.ActiveHousehold != null && PlumbBob.Singleton.mSelectedActor != null)
                                {
                                    try
                                    {
                                        List<SimDescription> listxt = new List<SimDescription>();
                                        foreach (SimDescription simat in Household.ActiveHousehold.SimDescriptions)
                                        {
                                            listxt.Add(simat);
                                        }
                                        if (listxt.Count != 0)
                                        {
                                            foreach (SimDescription item in listxt)
                                            {
                                                Sim sim = item.CreatedSim;
                                                if (sim == null) continue;
                                                if (sim.LotCurrent != null && sim.LotCurrent.IsWorldLot) continue;
                                                simrandom = RandomUtil.GetRandomObjectFromList(sim);
                                                yesRamdon = true;
                                                break;
                                            }
                                        }
                                    }
                                    catch
                                    { }
                                }

                                if (yesRamdon && simrandom != null)
                                {
                                    mSim.SetPosition(simrandom.Position);
                                }
                                else
                                {
                                    mSim.SetPosition(mHouseVeri3);
                                }

                                if (!mSim.LotCurrent.IsWorldLot)
                                {
                                    GrimReaper.Instance.MakeServiceRequest(mSim.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                                }
                                /*
                                var asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                if (asdoei != null)
                                {
                                    // Add New
                                    if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                    {
                                        Lot lotloot = mSim.LotCurrent;
                                        if (lotloot != null && lotloot.IsDivingLot)
                                        {
                                            ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Ocean.Singleton, mSim);
                                            if (scubaDivinga != null)
                                            {
                                                mSim.Posture = scubaDivinga;
                                                scubaDivinga.StartBubbleEffects();
                                            }
                                        }
                                    }
                                }
                                 * */
                            }
                        }
                        catch
                        { }
                    }
                }
                else
                {
                    if (PlumbBob.Singleton.mSelectedActor != null && !PlumbBob.Singleton.mSelectedActor.LotCurrent.IsWorldLot)
                    {
                        if (!GlobalFunctions.PlaceAtGoodLocation(mSim, new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position), false))
                        {
                            try
                            {
                                if (PlumbBob.Singleton.mSelectedActor != null && !PlumbBob.Singleton.mSelectedActor.LotCurrent.IsWorldLot)
                                {
                                    mHouseVeri3 = PlumbBob.Singleton.mSelectedActor.Position;
                                    if (!GlobalFunctions.PlaceAtGoodLocation(mSim, new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position), false))
                                    {
                                        mSim.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                                    }
                                    GrimReaper.Instance.MakeServiceRequest(PlumbBob.Singleton.mSelectedActor.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                                }
                                else
                                {
                                    bool yesRamdon = false;
                                    Sim simrandom = null;
                                    if (Household.ActiveHousehold != null && PlumbBob.Singleton.mSelectedActor != null)
                                    {
                                        try
                                        {
                                            List<SimDescription> listxt = new List<SimDescription>();
                                            foreach (SimDescription simat in Household.ActiveHousehold.SimDescriptions)
                                            {
                                                listxt.Add(simat);
                                            }
                                            if (listxt.Count != 0)
                                            {
                                                foreach (SimDescription item in listxt)
                                                {
                                                    Sim sim = item.CreatedSim;
                                                    if (sim == null) continue;
                                                    if (sim.LotCurrent != null && sim.LotCurrent.IsWorldLot) continue;
                                                    simrandom = RandomUtil.GetRandomObjectFromList(sim);
                                                    yesRamdon = true;
                                                    break;
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                    }

                                    if (yesRamdon && simrandom != null)
                                    {
                                        mSim.SetPosition(simrandom.Position);
                                    }
                                    else
                                    {
                                        mSim.SetPosition(mHouseVeri3);
                                    }
                                   
                                    if (!mSim.LotCurrent.IsWorldLot)
                                    {
                                        GrimReaper.Instance.MakeServiceRequest(mSim.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                                    }
                                    /*
                                    var asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                    if (asdoei != null)
                                    {
                                        // Add New
                                        if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                        {
                                            Lot lotloot = mSim.LotCurrent;
                                            if (lotloot != null && lotloot.IsDivingLot)
                                            {
                                                ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Ocean.Singleton, mSim);
                                                if (scubaDivinga != null)
                                                {
                                                    mSim.Posture = scubaDivinga;
                                                    scubaDivinga.StartBubbleEffects();
                                                }
                                            }
                                        }
                                    }
                                     * */
                                }
                            }
                            catch
                            { }
                        }
                    }
                    
                }

                

                try
                {
                    ExtKillSimNiec asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                    if (asdoei != null)
                    {
                        // Add New
                        if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                        {
                            Lot lotloot = mSim.LotCurrent;
                            if (lotloot != null && lotloot.IsDivingLot)
                            {
                                ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Ocean.Singleton, mSim);
                                if (scubaDivinga != null)
                                {
                                    mSim.Posture = scubaDivinga;
                                    scubaDivinga.StartBubbleEffects();
                                }
                            }
                        }
                    }
                }
                catch
                { }
                mSim.FadeIn();
                //mSim.SimRoutingComponent.DisableDynamicFootprint();
                
                //mSim.SimRoutingComponent.EnableDynamicFootprint();
                
                if (!mSim.LotCurrent.IsWorldLot)
                {
                    GrimReaper.Instance.MakeServiceRequest(mSim.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                }
                if (!PlumbBob.Singleton.mSelectedActor.LotCurrent.IsWorldLot)
                {
                    GrimReaper.Instance.MakeServiceRequest(PlumbBob.Singleton.mSelectedActor.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                }
            }
            catch (Exception gers)
            {
                NiecException.PrintMessage("FailedCallBookSleep Error");
                NiecException.WriteLog("FailedCallBookSleep: " + NiecException.NewLine + NiecException.LogException(gers), true, false, false);
            }

            AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(FailedCallBookSleep), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);

            return;
        }
    }
    //[Persistable]
    public class HelperNra
    {
        public AlarmHandle malarm = AlarmHandle.kInvalidHandle;

        public AlarmHandle malarmx = AlarmHandle.kInvalidHandle;

        public static Urnstone TFindGhostsGrave(SimDescription sim)
        {
            foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
            {
                if (object.ReferenceEquals(urnstone.DeadSimsDescription, sim))
                {
                    return urnstone;
                }
            }

            return null;
        }

        public Sim mSim = null;

        public SimDescription mSimdesc = null;

        public SimDescription.DeathType mdeathtype = SimDescription.DeathType.OldAge;

        public Vector3 mHouseVeri3 = Vector3.Invalid;

        public void CheckKillSimNotUrnstone()
        {
            //if (mSim == null) return;
            //NFinalizeDeath.DKill(mSim.SimDescription);
            //StyledNotification.Show(new StyledNotification.Format("HelperNra: " + mSim.Name + " ??? Check ??? :)", StyledNotification.NotificationStyle.kGameMessagePositive));
            
            Urnstone urnstone = Urnstone.CreateGrave(mSim.SimDescription, false, false);
            if (urnstone == null)
            {
                return;
            }
            if (!GlobalFunctions.PlaceAtGoodLocation(urnstone, new World.FindGoodLocationParams(mSim.Position), true))
            {
                return;
            }
            urnstone.OnHandToolMovement();
            
        }


        public void FailedCallBookSleep()
        {
            if (mSimdesc == null) return;
            if (mSim == null) return;
            try
            {
                AlarmManager.Global.RemoveAlarm(malarm);
                mSim.SimRoutingComponent.DisableDynamicFootprint();
                mSim.SetPosition(mHouseVeri3);
                mSim.SimRoutingComponent.EnableDynamicFootprint();

            }
            catch
            { }

            finally
            {
                malarmx = AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(FailedCallBookSleep), "FailedCallBookSleep", AlarmType.AlwaysPersisted, null);

            }

            return;
        }

        public void ResetReset()
        {
            try
            {
                if (mSimdesc == null) return;
                if (mSim == null) return;
                Household household = mSim.SimDescription.Household;
                if (household == Household.ActiveHousehold)
                {
                    /*
                    if (malarm != AlarmHandle.kInvalidHandle)
                    {
                        AlarmManager.Global.RemoveAlarm(malarm);
                    }
                     * */



                    mSimdesc.AgingEnabled = false;
                    mSimdesc.AgingState.ResetAndExtendAgingStage(0f);
                    if (mSimdesc.CreatedSim != null)
                    {
                        mSimdesc.CreatedSim.Autonomy.IncrementAutonomyDisabled();
                    }
                    
                    /*malarm = */AlarmManager.Global.AddAlarm(1f, TimeUnit.Hours, new AlarmTimerCallback(ResetReset), "Helper Name IRESET", AlarmType.AlwaysPersisted, null);
                }
                else
                {
                    return;
                }
            }
            catch (NullReferenceException)
            { }
            
        }

        public void CheckKillSimNotUrnstonePro()
        {
            if (mSimdesc == null) return;
            //if (mSim == null) return;
            
            try
            {
                if (mSim.InteractionQueue != null && mSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                {
                    AlarmManager.Global.AddAlarm(30f, TimeUnit.Hours, new AlarmTimerCallback(CheckKillSimNotUrnstonePro), "CheckKillSimNotUrnstonePro " + mSim.Name, AlarmType.AlwaysPersisted, null);
                    return;
                }
            }
            catch
            { }
            
            try
            {
                if (NTunable.kHelperNraNoCheckKillSim)
                {
                    try
                    {
                        Household household = mSimdesc.Household;
                        if (household != Household.ActiveHousehold)
                        {
                            mSimdesc.SetDeathStyle(mdeathtype, false);

                            mSimdesc.IsGhost = true;
                            mSimdesc.ShowSocialsOnSim = false;
                            mSimdesc.IsNeverSelectable = true;
                            mSimdesc.Contactable = false;
                            Urnstones.CreateGrave(mSim.SimDescription, mdeathtype, true, false);
                            StyledNotification.Show(new StyledNotification.Format("CheckKillSimNotUrnstonePro: " + mSim.SimDescription.FullName + " Ok :)", StyledNotification.NotificationStyle.kGameMessagePositive));

                        }
                    }
                    catch (NullReferenceException)
                    {
                        mSimdesc.SetDeathStyle(mdeathtype, false);

                        mSimdesc.IsGhost = true;
                        mSimdesc.ShowSocialsOnSim = false;
                        mSimdesc.IsNeverSelectable = true;
                        mSimdesc.Contactable = false;
                        Urnstones.CreateGrave(mSimdesc, mdeathtype, true, false);
                        StyledNotification.Show(new StyledNotification.Format("CheckKillSimNotUrnstonePro: " + mSimdesc.FullName + " Ok :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                    
                }
                else
                {
                    try
                    {
                        Household household = mSim.SimDescription.Household;
                        if (household != Household.ActiveHousehold)
                        {
                            if (mSimdesc.DeathStyle == SimDescription.DeathType.None)
                            {
                                try
                                {
                                    KillPro.CleanseGenealogy(mSimdesc);
                                    KillPro.RemoveSimDescriptionRelationships(mSimdesc);
                                }
                                catch (NullReferenceException)
                                { }
                                
                                mSimdesc.SetDeathStyle(mdeathtype, false);

                                mSimdesc.IsGhost = true;
                                mSimdesc.ShowSocialsOnSim = false;
                                mSimdesc.IsNeverSelectable = true;
                                mSimdesc.Contactable = false;
                                Urnstones.CreateGrave(mSim.SimDescription, mdeathtype, true, false);
                                StyledNotification.Show(new StyledNotification.Format("CheckKillSimNotUrnstonePro: " + mSim.SimDescription.FullName + " Ok :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }

                        }
                    }
                    catch (NullReferenceException)
                    {
                        mSimdesc.SetDeathStyle(mdeathtype, false);
                        mSimdesc.IsGhost = true;
                        mSimdesc.ShowSocialsOnSim = false;
                        mSimdesc.IsNeverSelectable = true;
                        mSimdesc.Contactable = false;
                        Urnstones.CreateGrave(mSimdesc, mdeathtype, true, false);
                        StyledNotification.Show(new StyledNotification.Format("CheckKillSimNotUrnstonePro: " + mSimdesc.FullName + " Ok :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                }
                
                
                /*
                Urnstone urnstone = TFindGhostsGrave(mSim.SimDescription);
                if (urnstone == null)
                {
                    Urnstones.CreateGrave(mSim.SimDescription, mdeathtype, true, false);
                }
                */
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception gers)
            {
                
                if (gers is ResetException)
                {
                    throw;
                }
                StyledNotification.Show(new StyledNotification.Format("CheckKillSimNotUrnstonePro: " + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                NiecException.WriteLog("CheckKillSimNotUrnstonePro: " + NiecException.NewLine + NiecException.LogException(gers), true, false, false);
            }

        }

        public ThumbnailKey GetThumbnailForGameObjectATSARS(ObjectGuid objGuid, ThumbnailSize size, int index, bool bForceUseSimDescription)
        {
            GameObject @object = GameObject.GetObject<GameObject>(objGuid);
            if (@object == null)
            {
                return new ThumbnailKey(ResourceKey.kInvalidResourceKey, ThumbnailSize.Medium);
            }
            Sim sim = @object as Sim;
            if (sim == null || sim.SimDescription == null)
            {
                ThumbnailKey thumbnailKey = @object.GetThumbnailKey();
                thumbnailKey.mSize = size;
                thumbnailKey.mCamera = ThumbnailCamera.Body;
                return thumbnailKey;
            }
            if (bForceUseSimDescription)
            {
                SimOutfit currentOutfit = sim.CurrentOutfit;
                ThumbnailKey result = new ThumbnailKey(currentOutfit, 0, ThumbnailSize.Large, ThumbnailCamera.Default);
                if (sim.SimDescription.IsGhost)
                {
                    result.mIndex = (int)(5u + sim.SimDescription.DeathStyle);
                }
                return result;
            }
            return sim.SimDescription.GetThumbnailKey(ThumbnailSize.Large, index);
        }
    }
    /// <summary>
    /// This a Helper Morun By Niec
    /// </summary>
    public static class NFinalizeDeath
    {




        public static bool IsCanBeKilled(Sim targetSim)
        {
            if (targetSim == null) return false;
            if (targetSim.SimDescription == null) return false;
            try
            {
                if (targetSim.SimDescription.IsGhost) return false;
                if (targetSim.SimDescription.IsPlayableGhost) return false;
                if (targetSim.SimDescription.IsDead) return false;
                if (targetSim.mInteractionQueue == null)
                {
                    try
                    {
                        targetSim.mInteractionQueue = new Sims3.Gameplay.ActorSystems.InteractionQueue(targetSim);
                        targetSim.mInteractionQueue.OnLoadFixup();
                    }
                    catch (Exception exception)
                    {
                        NiecException.WriteLog("mInteractionQueue " + NiecException.NewLine + NiecException.LogException(exception), true, true, false);
                    }

                    if (targetSim.mInteractionQueue == null) return false;
                }



                Type type = ExtKillSimNiec.Singleton.GetType();
                foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.mInteractionList)
                {
                    try
                    {
                        if (interactionInstance.InteractionDefinition.GetType() == type)
                        {
                            return false;
                        }
                    }
                    catch
                    { }

                }


                if (targetSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton)) return false;
                if (targetSim.InteractionQueue.HasInteractionOfType(typeof(ExtKillSimNiec))) return false;

                if (targetSim.InteractionQueue.HasInteractionOfTypeAndTarget(ExtKillSimNiec.Singleton, targetSim)) return false;

                foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.InteractionList)
                {
                    if (interactionInstance is ExtKillSimNiec)
                    {
                        return false;
                    }
                }




                if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                {

                    foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.InteractionList)
                    {
                        if (interactionInstance is Urnstone.KillSim)
                        {
                            return false;
                        }
                    }
                    if (targetSim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton)) return false;
                }
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                //return true;
            }


            return true;
        }










        public static bool SimIsActiveHouseholdWithoutDGSCore(Sim Target)
        {
            if (Target == null) return false;
            //return PlumbBob.Singleton != null && PlumbBob.Singleton.mSelectedActor != null && ;
            if (PlumbBob.Singleton == null || PlumbBob.Singleton.mSelectedActor == null) return false;
            return PlumbBob.Singleton.mSelectedActor.Household == Target.Household;
        }

        public static bool SimDescriptionIsActiveHouseholdWithoutDGSCore(SimDescription Target)
        {
            if (Target == null) return false;
            return PlumbBob.sSingleton != null && PlumbBob.sSingleton.mSelectedActor != null && Target.Household == PlumbBob.sSingleton.mSelectedActor.Household;
        }





        public static bool IsSimFastActiveHousehold(Sim Target)
        {
            if (Target == null) return false;
            return ActiveHouseholdWithoutDGSCore == Target.Household;
        }


        public static bool IsSimDescriptionFastActiveHousehold(SimDescription Target)
        {
            if (Target == null) return false;
            return ActiveHouseholdWithoutDGSCore == Target.Household;
        }


        public static Household ActiveHouseholdWithoutDGSCore
        {
            get
            {
                if (PlumbBob.Singleton == null || PlumbBob.Singleton.mSelectedActor == null) return null;
                return PlumbBob.Singleton.mSelectedActor.Household;
            }

        }








        public static bool CheckKillSimInteractionPro(Sim Target, InteractionInstance checkin)
        {
            if (Target == null) return false;
            try
            {
                if (checkin is ExtKillSimNiec) return true;
                if (checkin is Urnstone.KillSim) return true;
                if (Target.InteractionQueue.HasInteractionOfType(NiecMod.KillNiec.KillSimNiecX.NiecDefinitionDeathInteraction.SocialSingleton)) return true;
                if (Target.InteractionQueue.HasInteractionOfType(typeof(Sims3.Gameplay.Socializing.SocialInteractionB.DefinitionDeathInteraction))) return true;
            }
            catch
            { }

            return false;
        }















        public static void ForceCancelAllInteractionsPro(Sim Target)
        {
            checked
            {
                for (int i = Target.InteractionQueue.mInteractionList.Count - 1; i >= 0; i--)
                {
                    /*
                    //InteractionInstance interactionInstance = Target.InteractionQueue.mInteractionList[i];
                    //if (!(mInteractionList[i] is ExtKillSimNiec))
                    if (!CheckKillSimInteractionPro(Target, Target.InteractionQueue.mInteractionList[i]))
                    {
                        try
                        {
                            //mActor.AddExitReason(ExitReason.SuspensionRequested);
                            //CancelNthInteraction(i, false, ExitReason.Default);
                            if (!Target.InteractionQueue.IsRunning(Target.InteractionQueue.mInteractionList[i], true))
                            {
                                Target.InteractionQueue.mInteractionList[i].OnRemovedFromQueue(i == 0);
                                Target.InteractionQueue.mInteractionList.RemoveAt(i);
                            }

                            //else
                            //{
                            //     mActor.AddExitReason(ExitReason.Default);
                            //}
                        }
                        catch
                        { 

                    }
                     */
                    try
                    {
                        if (!CheckKillSimInteractionPro(Target, Target.InteractionQueue.mInteractionList[i]))
                        {
                            if (Target.InteractionQueue.IsRunning(Target.InteractionQueue.mInteractionList[i], true) && (Target.InteractionQueue.mIsHeadInteractionLocked || Target.InteractionQueue.mCurrentTransitionInteraction != null))
                            {
                                Target.AddExitReason(ExitReason.SuspensionRequested);
                            }
                            else
                            {
                                RemoveInteraction( Target,  i, false, true      );
                            }
                        }
                    }
                    catch
                    {
                        
                        
                    }
                    
                }
                try
                {
                    if (!Target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton) && !Target.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton))
                    {
                        Target.AddExitReason(ExitReason.Canceled);
                        Target.AddExitReason(ExitReason.StageComplete);
                        Target.AddExitReason(ExitReason.Default);
                        Target.AddExitReason(ExitReason.BuffFailureState);
                        Target.AddExitReason(ExitReason.HigherPriorityNext);
                        Target.AddExitReason(ExitReason.CanceledByScript);
                    }

                }
                catch
                { }
            }
        }

        

        public static void ForceCancelAllInteractionsProPartA(Sim Target)
        {
            checked
            {
                for (int i = Target.InteractionQueue.mInteractionList.Count - 1; i >= 0; i--)
                {
                    /*
                    //InteractionInstance interactionInstance = Target.InteractionQueue.mInteractionList[i];
                    //if (!(mInteractionList[i] is ExtKillSimNiec))
                    if (!CheckKillSimInteractionPro(Target, Target.InteractionQueue.mInteractionList[i]))
                    {
                        try
                        {
                            //mActor.AddExitReason(ExitReason.SuspensionRequested);
                            //CancelNthInteraction(i, false, ExitReason.Default);
                            if (!Target.InteractionQueue.IsRunning(Target.InteractionQueue.mInteractionList[i], true))
                            {
                                Target.InteractionQueue.mInteractionList[i].OnRemovedFromQueue(i == 0);
                                Target.InteractionQueue.mInteractionList.RemoveAt(i);
                            }

                            //else
                            //{
                            //     mActor.AddExitReason(ExitReason.Default);
                            //}
                        }
                        catch
                        { 

                    }
                     */
                    try
                    {
                        if (!CheckKillSimInteractionPro(Target, Target.InteractionQueue.mInteractionList[i]))
                        {
                            if (Target.InteractionQueue.IsRunning(Target.InteractionQueue.mInteractionList[i], true) && (Target.InteractionQueue.mIsHeadInteractionLocked || Target.InteractionQueue.mCurrentTransitionInteraction != null))
                            {
                                Target.AddExitReason(ExitReason.SuspensionRequested);
                            }
                            else
                            {
                                RemoveInteraction(Target, i, false, true);
                            }
                        }
                    }
                    catch
                    {


                    }

                }
            }
        }


        public static void ForceCancelAllInteractionsProOld(Sim Target)
        {
            checked
            {
                for (int i = Target.InteractionQueue.mInteractionList.Count - 1; i >= 0; i--)
                {
                    InteractionInstance interactionInstance = Target.InteractionQueue.mInteractionList[i];
                    //if (!(mInteractionList[i] is ExtKillSimNiec))
                    if (!CheckKillSimInteractionPro(Target, interactionInstance))
                    {
                        try
                        {
                            //mActor.AddExitReason(ExitReason.SuspensionRequested);
                            //CancelNthInteraction(i, false, ExitReason.Default);
                            if (!Target.InteractionQueue.IsRunning(interactionInstance, true))
                            {
                                interactionInstance.OnRemovedFromQueue(i == 0);
                                Target.InteractionQueue.mInteractionList.RemoveAt(i);
                            }

                            //else
                            //{
                            //     mActor.AddExitReason(ExitReason.Default);
                            //}
                        }
                        catch
                        { }

                    }
                }
                try
                {
                    if (!Target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton) && !Target.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton))
                    {
                        Target.AddExitReason(ExitReason.Canceled);
                        Target.AddExitReason(ExitReason.StageComplete);
                        Target.AddExitReason(ExitReason.Default);
                        Target.AddExitReason(ExitReason.BuffFailureState);
                        Target.AddExitReason(ExitReason.HigherPriorityNext);
                        Target.AddExitReason(ExitReason.CanceledByScript);
                    }

                }
                catch
                { }
            }
        }


        public static bool mResetError = false;


        public static void ResetError()
        {
            while (mResetError)
            {
                try
                {
                    Simulator.YieldingDisabled = false;
                    //Simulator.Sleep(0);
                    Nra.SpeedTrap.ForceSleep();
                }
                catch
                {}
            }
        }

        public delegate void Function();

        public static void CancelAllInteractions(Sim Target)
        {
            checked
            {
                for (int i = Target.InteractionQueue.mInteractionList.Count - 1; i >= 0; i--)
                {
                    if (!(Target.InteractionQueue.mInteractionList[i] is ExtKillSimNiec))
                    {
                        try
                        {
                            Target.AddExitReason(ExitReason.SuspensionRequested);
                            Target.InteractionQueue.CancelNthInteraction(i, false, ExitReason.Default);
                        }
                        catch
                        { }

                    }
                }
                try
                {
                    if (!Target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                    {
                        Target.AddExitReason(ExitReason.Canceled);
                        Target.AddExitReason(ExitReason.StageComplete);
                        Target.AddExitReason(ExitReason.Default);
                        Target.AddExitReason(ExitReason.BuffFailureState);
                        Target.AddExitReason(ExitReason.HigherPriorityNext);
                        Target.AddExitReason(ExitReason.CanceledByScript);
                    }
                    
                }
                catch
                { }
            }
        }


        public static void ForceCancelAllInteractionsWithoutCleanup(Sim Target)
        {
            try
            {
                Target.InteractionQueue.mInteractionList.Clear();
            }
            catch
            { }
            
        }

        public static void ForceCancelAllInteractions(Sim Target)
        {
            
            checked
            {
                for (int i = Target.InteractionQueue.mInteractionList.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        Target.AddExitReason(ExitReason.SuspensionRequested);
                        Target.InteractionQueue.CancelNthInteraction(i, false, ExitReason.Default);
                    }
                    catch
                    { }
                }
            }
             
            //Target.InteractionQueue.mInteractionList.Clear();
        }

        

        public static void RemoveInteraction(Sim Target, int index, bool stopImmediately, bool succeeded)
        {
            if (index >= 0 && index < Target.InteractionQueue.mInteractionList.Count)
            {
                InteractionInstance interactionInstance = Target.InteractionQueue.mInteractionList[index];
                if (Target.InteractionQueue.mIsHeadInteractionLocked && index == 0)
                {
                    throw new ApplicationException("Sim: " + interactionInstance.InstanceActor.SimDescription.FullName + " is removing head interaction: " + interactionInstance.GetInteractionName() + " while it is locked.");
                    //return;
                }
                interactionInstance.OnRemovedFromQueue(index == 0);
                Target.InteractionQueue.mInteractionList.RemoveAt(index);
                /*
                if (!interactionInstance.InstanceActor.HasExitReason(ExitReason.SuspensionRequested))
                {
                    CleanupInteraction(Target, interactionInstance, stopImmediately, succeeded);
                }
                 */
                if (succeeded || Target.InteractionQueue.mBabyOrToddlerTransitionTargetInteraction == interactionInstance || Target.InteractionQueue.mInteractionList.Count == 0)
                {
                    Target.InteractionQueue.mBabyOrToddlerTransitionTargetInteraction = null;
                }
                
            }
        }

        public static void CleanupInteraction(Sim Target, InteractionInstance i, bool stopImmediately, bool succeeded)
        {
            if (i.InteractionObjectPair != null)
            {
                InteractionInstance linkedInteractionInstance = i.LinkedInteractionInstance;
                if (linkedInteractionInstance != null)
                {
                    i.LinkedInteractionInstance = null;
                    Sim instanceActor = linkedInteractionInstance.InstanceActor;
                    if (instanceActor != null && !instanceActor.HasBeenDestroyed)
                    {
                        if (instanceActor.InteractionQueue.IsRunning(linkedInteractionInstance, false) && stopImmediately)
                        {
                            instanceActor.SetObjectToReset();
                        }
                        else
                        {
                            instanceActor.InteractionQueue.CancelInteraction(linkedInteractionInstance, succeeded);
                        }
                    }
                }
                if (succeeded && !stopImmediately)
                {
                    i.CallCallbackOnCompletion(Target);
                }
                else
                {
                    i.CallCallbackOnFailure(Target);
                }
                i.Cleanup();
            }
        }

        public static void ForceChangeState(Relationship relation, LongTermRelationshipTypes state)
        {
            LongTermRelationship.InteractionBits bits = relation.LTR.LTRInteractionBits & (LongTermRelationship.InteractionBits.HaveBeenBestFriends | LongTermRelationship.InteractionBits.HaveBeenFriends | LongTermRelationship.InteractionBits.HaveBeenPartners);

            LTRData data = LTRData.Get(state);
            if (relation.LTR.RelationshipIsInappropriate(data))
            {
                relation.LTR.ChangeBitsForState(state);
                relation.LTR.ChangeState(state);
                relation.LTR.UpdateUI();
            }
            else
            {
                relation.LTR.ForceChangeState(state);
            }

            if (state == LongTermRelationshipTypes.Spouse)
            {
                relation.SimDescriptionA.Genealogy.Marry(relation.SimDescriptionB.Genealogy);

                MidlifeCrisisManager.OnBecameMarried(relation.SimDescriptionA, relation.SimDescriptionB);

                relation.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                relation.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Marry);

                relation.SetMarriedInGame();

                if (SeasonsManager.Enabled)
                {
                    relation.WeddingAnniversary = new WeddingAnniversary(SeasonsManager.CurrentSeason, (int)SeasonsManager.DaysElapsed);
                    relation.WeddingAnniversary.SimA = relation.SimDescriptionA;
                    relation.WeddingAnniversary.SimB = relation.SimDescriptionB;
                    relation.WeddingAnniversary.CreateAlarm();
                }
            }

            relation.LTR.AddInteractionBit(bits);
        }


        public static SimDescription MakeSim(SimBuilder builder, CASAgeGenderFlags age, CASAgeGenderFlags gender, ResourceKey skinTone, float skinToneIndex, Color[] hairColors, WorldName homeWorld, uint outfitCategoriesToBuild, bool isAlien)
        {
            if (age == CASAgeGenderFlags.None)
            {
                return null;
            }
            if (builder == null)
            {
                builder = new SimBuilder();
                builder.Age = age;
                builder.Gender = gender;
                builder.Species = CASAgeGenderFlags.Human;
                builder.SkinTone = skinTone;
                builder.SkinToneIndex = skinToneIndex;
                builder.TextureSize = 1024u;
                builder.UseCompression = true;
            }
            if (hairColors.Length == 9)
            {
                Color[] array = new Color[10];
                hairColors.CopyTo(array, 0);
                array[9] = hairColors[0];
                hairColors = array;
            }
            if (hairColors.Length != 10)
            {
                hairColors = Genetics.Black1;
            }
            Color[] array2 = new Color[4];
            Array.Copy(hairColors, 5, array2, 0, 4);
            Color activeEyebrowColor = hairColors[4];
            SimDescriptionCore simDescriptionCore = new SimDescriptionCore();
            simDescriptionCore.HomeWorld = homeWorld;
            bool useDyeColor = age == CASAgeGenderFlags.Elder;
            GeneticColor[] hairColors2 = simDescriptionCore.HairColors;
            for (int i = 0; i < 4; i++)
            {
                hairColors2[i].UseDyeColor = useDyeColor;
            }
            simDescriptionCore.HairColors = hairColors2;
            simDescriptionCore.ActiveHairColors = hairColors;
            simDescriptionCore.EyebrowColor.UseDyeColor = useDyeColor;
            simDescriptionCore.ActiveEyebrowColor = activeEyebrowColor;
            simDescriptionCore.BodyHairColor.UseDyeColor = useDyeColor;
            simDescriptionCore.ActiveBodyHairColor = hairColors[9];
            GeneticColor[] facialHairColors = simDescriptionCore.FacialHairColors;
            for (int j = 0; j < 4; j++)
            {
                facialHairColors[j].UseDyeColor = useDyeColor;
            }
            simDescriptionCore.FacialHairColors = facialHairColors;
            simDescriptionCore.ActiveFacialHairColors = array2;
            Dictionary<ResourceKey, float> dictionary = new Dictionary<ResourceKey, float>();
            if (LocaleConstraints.GetFacialShape(ref dictionary, homeWorld))
            {
                foreach (KeyValuePair<ResourceKey, float> keyValuePair in dictionary)
                {
                    builder.SetFacialBlend(keyValuePair.Key, keyValuePair.Value);
                }
            }
            OutfitUtils.AddMissingParts(builder, (OutfitCategories)2097154u, true, simDescriptionCore, isAlien);
            Genetics.SleepIfPossible();
            OutfitUtils.AddMissingParts(builder, OutfitCategories.Everyday, true, simDescriptionCore, isAlien);
            Genetics.SleepIfPossible();
            ResourceKey key = default(ResourceKey);
            if (LocaleConstraints.GetUniform(ref key, homeWorld, age, gender, OutfitCategories.Everyday))
            {
                OutfitUtils.SetOutfit(builder, new SimOutfit(key), simDescriptionCore);
            }
            OutfitUtils.SetAutomaticModifiers(builder);
            ResourceKey key2 = builder.CacheOutfit(string.Format("Genetics.MakeSim_{0}_{1}_{2}", builder.Age, Simulator.TicksElapsed(), OutfitCategories.Everyday));
            if (key2.InstanceId == 0UL)
            {
                return null;
            }
            OutfitCategories[] array3 = new OutfitCategories[]
			{
				OutfitCategories.Naked,
				OutfitCategories.Athletic,
				OutfitCategories.Formalwear,
				OutfitCategories.Sleepwear,
				OutfitCategories.Swimwear
			};
            SimOutfit simOutfit = new SimOutfit(key2);
            SimDescription simDescription = new SimDescription(simOutfit);
            simDescription.HomeWorld = simDescriptionCore.HomeWorld;
            simDescription.HairColors = simDescriptionCore.HairColors;
            simDescription.FacialHairColors = simDescriptionCore.FacialHairColors;
            simDescription.EyebrowColor = simDescriptionCore.EyebrowColor;
            simDescription.BodyHairColor = simDescriptionCore.BodyHairColor;
            simDescription.AddOutfit(simOutfit, OutfitCategories.Everyday, true);
            foreach (OutfitCategories outfitCategories in array3)
            {
                if ((outfitCategoriesToBuild & (uint)outfitCategories) != 0u)
                {
                    OutfitUtils.MakeCategoryAppropriate(builder, outfitCategories, simDescription);
                    if (LocaleConstraints.GetUniform(ref key, homeWorld, age, gender, outfitCategories))
                    {
                        OutfitUtils.SetOutfit(builder, new SimOutfit(key), simDescriptionCore);
                    }
                    ResourceKey key3 = builder.CacheOutfit(string.Format("Genetics.MakeSim_{0}_{1}_{2}", builder.Age, Simulator.TicksElapsed(), outfitCategories));
                    simDescription.AddOutfit(new SimOutfit(key3), outfitCategories);
                    Genetics.SleepIfPossible();
                }
            }
            simDescription.RandomizePreferences();
            TraitNames cultureSpecificTrait = Genetics.GetCultureSpecificTrait(homeWorld);
            if (cultureSpecificTrait == TraitNames.FutureSim)
            {
                simDescription.TraitManager.AddHiddenElement(cultureSpecificTrait);
                Skill skill = simDescription.SkillManager.AddElement(SkillNames.Future);
                if ((skill.AvailableAgeSpecies & simDescription.GetCASAGSAvailabilityFlags()) != CASAGSAvailabilityFlags.None)
                {
                    while (simDescription.SkillManager.GetSkillLevel(SkillNames.Future) < skill.MaxSkillLevel)
                    {
                        simDescription.SkillManager.ForceGainPointsForLevelUp(SkillNames.Future);
                    }
                }
            }
            else if (cultureSpecificTrait != TraitNames.Unknown)
            {
                simDescription.TraitManager.AddHiddenElement(cultureSpecificTrait);
            }
            builder.Dispose();
            builder = null;
            return simDescription;
        }


        public static List<SimDescription> TattoaX()
        {
            List<SimDescription> list = new List<SimDescription>();



            List<Sim> asdo = new List<Sim>();
            try
            {

                try
                {
                    try
                    {
                        foreach (Sim simau in Sims3.Gameplay.Queries.GetObjects<Sim>())
                        {
                            try
                            {
                                if (!asdo.Contains(simau))
                                    asdo.Add(simau);
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }


                    try
                    {
                        foreach (Sim simau in LotManager.Actors)
                        {
                            try
                            {
                                if (!asdo.Contains(simau))
                                    asdo.Add(simau);
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }

                }
                catch
                { }





                try
                {
                    foreach (Sim simaue in asdo)
                    {
                        SimDescription checkkillsim = simaue.SimDescription;
                        if (checkkillsim != null && !list.Contains(checkkillsim))
                            list.Add(checkkillsim);
                    }
                }
                catch
                { }
            }
            catch
            { }




            try
            {
                try
                {

                    //list.Add(SimDescription.sLoadedSimDescriptions)
                    //list.AddRange(SimDescription.sLoadedSimDescriptions);
                    foreach (SimDescription sdra in SimDescription.sLoadedSimDescriptions)
                    {
                        if (sdra == null) continue;
                        if (!list.Contains(sdra))
                            list.Add(sdra);
                    }
                }
                catch
                { }
                try
                {
                    foreach (Household household in Household.sHouseholdList)
                    {
                        foreach (SimDescription sda in household.AllSimDescriptions)
                        {
                            if (sda == null) continue;
                            if (!list.Contains(sda))
                            list.Add(sda);
                        }
                    }
                }
                catch
                { }
                

                try
                {
                    Urnstone[] objects = Sims3.Gameplay.Queries.GetObjects<Urnstone>();
                    foreach (Urnstone urnstone in objects)
                    {
                        if (urnstone.DeadSimsDescription != null && !list.Contains(urnstone.DeadSimsDescription))
                        {
                            
                            list.Add(urnstone.DeadSimsDescription);
                        }
                    }
                }
                catch
                { }
                
                try
                {
                    foreach (SimDescription sd in Household.EverySimDescription())
                    {
                        if (sd == null) continue;
                        if (!list.Contains(sd))
                        list.Add(sd);
                    }
                }
                catch
                { }
                
                try
                {
                    if (GameStates.sTravelData != null && GameStates.sTravelData.mEarlyDepartures != null)
                    {
                        foreach (Sim sirtyrtym in GameStates.sTravelData.mEarlyDepartures)
                        {
                            if (sirtyrtym == null) continue;
                            if (!list.Contains(sirtyrtym.SimDescription))
                            list.Add(sirtyrtym.SimDescription);
                        }
                    }
                }
                catch
                { }
                
                try
                {
                    if (IntroTutorial.SimsRemovedFromHousehold != null)
                    {
                        foreach (Sim simta in IntroTutorial.SimsRemovedFromHousehold)
                        {
                            if (simta == null) continue;
                            if (!list.Contains(simta.SimDescription))
                            list.Add(simta.SimDescription);
                        }
                    }
                }
                catch
                { }



                try
                {
                    foreach (SimDescription simDescripaation in SimDescription.GetAll(SimDescription.Repository.Household))
                    {
                        if (simDescripaation == null) continue;
                        if (!list.Contains(simDescripaation))
                            list.Add(simDescripaation);
                    }
                }
                catch
                { }


            }

            catch
            { }
            return list;
        }

        public static List<SimDescription> Tattoa()
        {
            List<SimDescription> list = new List<SimDescription>();
            list.AddRange(ATTSE());
            list.AddRange(ATTSS());
            return list;
        }

        public static List<SimDescription> ATTSS()
        {
            List<SimDescription> list = new List<SimDescription>();
            Urnstone[] objects = Sims3.Gameplay.Queries.GetObjects<Urnstone>();
            for (int i = 0; i < objects.Length; i++)
            {
                SimDescription deadSimsDescription = objects[i].DeadSimsDescription;
                if (deadSimsDescription != null)
                {
                    list.Add(deadSimsDescription);
                }
            }
            return list;
        }

        public static List<SimDescription> ATTSE()
        {
            List<SimDescription> list = new List<SimDescription>();
            foreach (Household household in Household.sHouseholdList)
            {
                list.AddRange(household.AllSimDescriptions);
            }
            return list;
        }

        public static bool IsExtKillSimNiecAndYLevel(Sim actor)
        {

            

            /*
            if (actor == null)
            {
                //throw new ArgumentNullException("actor");
                return false;
            }
            */
            /*
            if (actor.InteractionQueue == null) return false;
            */
            if (actor.SimDescription == null) return false;


            try
            {
                if (actor.SimDescription.Household == Household.ActiveHousehold)
                {
                    return true;
                }
            }
            catch (Exception)
            { }

            if (AssemblyCheckByNiec.IsInstalled("NiecMod"))
            {
                try
                {
                    if (actor.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton)) return true;
                }
                catch (Exception)
                { }

                try
                {
                    foreach (InteractionInstance interactionInstance in actor.InteractionQueue.InteractionList)
                    {
                        if (interactionInstance is ExtKillSimNiec)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                { }

                try
                {
                    foreach (InteractionInstance interactionInstance in actor.InteractionQueue.InteractionList)
                    {
                        if (interactionInstance.InteractionDefinition == ExtKillSimNiec.Singleton)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                { }

            }
            try
            {
                if (!(actor.Service is Sims3.Gameplay.Services.GrimReaper))
                {
                    foreach (InteractionInstance interactionInstance in actor.InteractionQueue.InteractionList)
                    {
                        if (interactionInstance.GetPriority().Level > (InteractionPriorityLevel)100)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            { }


            return false;
        
        }



        public static bool Adertyrty = false;

        public static StringBuilder LogHelper = new StringBuilder();
        /// <summary>
        /// This a Aotoa
        /// </summary>
        public enum APACheck
        {
            Aota,
            Aots
        }
        /// <summary>
        /// Check Has
        /// </summary>
        public static void CheckMorun()
        {

        }
        // UnTested
        /*
        public static InteractionPriority AutonomousCheckWithActiveHousehold()
        {
            try
            {
                Sim a = null;
                InteractionPriority autonomous = new InteractionPriority(InteractionPriorityLevel.Autonomous, 0f);
                //if (Sim.ActiveActor != null && Sim.ActiveActor.Household != null)
                if (Household.ActiveHousehold != null)
                if (a.Household != Household.ActiveHousehold)
                //if (Aicota.IsInActiveHousehold)
                {
                    StyledNotification.Show(new StyledNotification.Format("AutonomousCheckWithActiveHousehold: Done", StyledNotification.NotificationStyle.kGameMessagePositive));
                    autonomous = new InteractionPriority((InteractionPriorityLevel)14, 0f);
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("AutonomousCheckWithActiveHousehold: Failed", StyledNotification.NotificationStyle.kGameMessagePositive));
                    autonomous = new InteractionPriority(InteractionPriorityLevel.Autonomous, 0f);
                }
                return autonomous;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                StyledNotification.Show(new StyledNotification.Format("AutonomousCheckWithActiveHousehold: Catch", StyledNotification.NotificationStyle.kGameMessagePositive));
                InteractionPriority autonomous = new InteractionPriority(InteractionPriorityLevel.Autonomous, 0f);
                return autonomous;
            }
        }
        */
        /// <summary>
        /// This a CheckPregnancy 
        /// </summary>
        /// <param name="target"></param>
        public static void CheckPregnancy(Sim target)
        {
            if (target.SimDescription.IsPregnant)
            {
                target.SimDescription.Pregnancy.ClearPregnancyData();
                if (target.SimDescription.Pregnancy == null)
                {
                    StyledNotification.Show(new StyledNotification.Format(target.Name + Localization.LocalizeString("cmarXmods/PregControl/PregNotice:NoMorePreg", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format(Localization.LocalizeString("cmarXmods/PregControl/PregNotice:TerminationFail", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                }
            }
        }
        /// <summary>
        /// This a DKill
        /// </summary>
        /// <param name="simd"></param>
        /// <returns></returns>
        public static bool DKill(SimDescription simd)
        {
            Sim sime = simd.CreatedSim;
            /*
            if (simd.CreatedSim == null)
            {
                return false;
            }
            if (sime == null)
            {
                return false;
            }
            */
            if (simd == null)
            {
                return false;
            }
            try
            {
                if (IsExtKillSimNiecAndYLevel(sime))
                {
                    return true;
                }
            }
            catch (Exception)
            { }

            try
            {
                throw new NiecModException("Not Error Show Sim Description");
            }
            catch (NiecModException ex)
            {
                NiecException.WriteLog("DKill: " + NiecException.NewLine + NiecException.LogException(ex), true, false, true);
            }
            try
            {
                

                try
                {
                    CheckPregnancy(sime);
                }
                catch (Exception)
                { }

                if (!simd.IsGhost || !simd.IsPlayableGhost || !simd.IsDead)
                {
                    try
                    {

                        if (simd.Household != null)
                        {
                            ExtKillSimNiec.ListMorunExtKillSim(sime, SimDescription.DeathType.Shark);
                        }
                        FinalizeSimDeathRelationships(simd, 0f);

                    }
                    catch (Exception ex)
                    {
                        NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                        try
                        {
                            simd.ShowSocialsOnSim = false;
                            simd.IsNeverSelectable = true;
                            simd.Contactable = false;
                        }
                        catch (Exception)
                        { }
                    }
                    finally
                    {
                        try
                        {
                            simd.ShowSocialsOnSim = false;
                            simd.IsNeverSelectable = true;
                            simd.Contactable = false;
                        }
                        catch (Exception)
                        { }
                    }
                    Urnstones.CreateGrave(simd, SimDescription.DeathType.Shark, true, false);
                    StyledNotification.Show(new StyledNotification.Format("DKill: " + simd.FullName + " Done Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("DKill: " + simd.FullName + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                return true;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                StyledNotification.Show(new StyledNotification.Format("DKill: " + simd.FullName + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                return true;
            }
        }
        /// <summary>
        /// This a SKill
        /// </summary>
        /// <param name="sims"></param>
        /// <returns></returns>

        public static void DKillVoid(SimDescription simd)
        {
            Sim sime = simd.CreatedSim;
            try
            {
                throw new NiecModException("Not Error Show Sim Description");
            }
            catch (NiecModException ex)
            {
                Common.Exception(sime, ex);
                NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
            }
            try
            {
                try
                {
                    CheckPregnancy(sime);
                }
                catch (Exception)
                { }
                if (!simd.IsGhost || !simd.IsPlayableGhost || !simd.IsDead)
                {
                    try
                    {

                        if (simd.Household != null)
                        {
                            ExtKillSimNiec.ListMorunExtKillSim(sime, SimDescription.DeathType.Shark);
                        }
                        FinalizeSimDeathRelationships(simd, 0f);

                    }
                    catch (Exception ex)
                    {
                        NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                        try
                        {
                            simd.ShowSocialsOnSim = false;
                            simd.IsNeverSelectable = true;
                            simd.Contactable = false;
                        }
                        catch (Exception)
                        { }
                    }
                    finally
                    {
                        try
                        {
                            simd.ShowSocialsOnSim = false;
                            simd.IsNeverSelectable = true;
                            simd.Contactable = false;
                        }
                        catch (Exception)
                        { }
                    }
                    Urnstones.CreateGrave(simd, SimDescription.DeathType.Shark, true, false);
                    StyledNotification.Show(new StyledNotification.Format("DKill: " + simd.FullName + " Done Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("DKill: " + simd.FullName + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                StyledNotification.Show(new StyledNotification.Format("DKill: " + simd.FullName + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
            }
        }

        public static bool SKill(Sim sims)
        {

            try
            {
                if (!sims.SimDescription.IsGhost || !sims.SimDescription.IsPlayableGhost || !sims.SimDescription.IsDead)
                {
                    Urnstones.CreateGrave(sims.SimDescription, SimDescription.DeathType.Shark, true, false);
                    StyledNotification.Show(new StyledNotification.Format("SKill: " + sims.Name + " Done Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                else
                {
                    StyledNotification.Show(new StyledNotification.Format("SKill: " + sims.Name + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                }
                return true;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                StyledNotification.Show(new StyledNotification.Format("SKill: " + sims.Name + " Failed :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                return true;
            }
        }

        private static void Told(Household newHousehold, bool bForce)
        {
            if (newHousehold != null)
            {
                Lot.UpdatePlayerNeighbors();
                TombRoomManager.OnChangeHousehold(newHousehold);
            }
        }

        public static Sim MakeRandomSimWithFutureWorld(Vector3 point, CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            LotLocation lotLocation = default(LotLocation);
            ulong lotLocation2 = World.GetLotLocation(point, ref lotLocation);
            Lot lot = LotManager.GetLot(lotLocation2);
            if ((age & (CASAgeGenderFlags.Baby | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child)) != CASAgeGenderFlags.None)
            {
                bool flag = false;
                if (lot != null && lot.Household != null)
                {
                    foreach (SimDescription simDescription in lot.Household.SimDescriptions)
                    {
                        if (simDescription.TeenOrAbove)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    return null;
                }
            }
            SimDescription simDescription2 = Genetics.MakeSim(age, gender, WorldName.FutureWorld, uint.MaxValue);
            Genetics.AssignRandomTraits(simDescription2);
            if (lot != null)
            {
                if (lot.Household == null)
                {
                    Household household = Household.Create();
                    lot.MoveIn(household);
                }
                lot.Household.Add(simDescription2);
            }
            else
            {
                Household household2 = Household.Create();
                household2.SetName("Default");
                household2.Add(simDescription2);
            }
            WorldName currentWorld = GameUtils.GetCurrentWorld();
            simDescription2.FirstName = SimUtils.GetRandomGivenName(simDescription2.IsMale, currentWorld);
            simDescription2.LastName = SimUtils.GetRandomFamilyName(currentWorld);
            return simDescription2.Instantiate(point);
        }

        public static Sim MakeRandomSimWithChina(Vector3 point, CASAgeGenderFlags age, CASAgeGenderFlags gender)
        {
            LotLocation lotLocation = default(LotLocation);
            ulong lotLocation2 = World.GetLotLocation(point, ref lotLocation);
            Lot lot = LotManager.GetLot(lotLocation2);
            if ((age & (CASAgeGenderFlags.Baby | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child)) != CASAgeGenderFlags.None)
            {
                bool flag = false;
                if (lot != null && lot.Household != null)
                {
                    foreach (SimDescription simDescription in lot.Household.SimDescriptions)
                    {
                        if (simDescription.TeenOrAbove)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (!flag)
                {
                    return null;
                }
            }
            SimDescription simDescription2 = Genetics.MakeSim(age, gender, WorldName.China, uint.MaxValue);
            Genetics.AssignRandomTraits(simDescription2);
            if (lot != null)
            {
                if (lot.Household == null)
                {
                    Household household = Household.Create();
                    lot.MoveIn(household);
                }
                lot.Household.Add(simDescription2);
            }
            else
            {
                Household household2 = Household.Create();
                household2.SetName("Default");
                household2.Add(simDescription2);
            }
            WorldName currentWorld = WorldName.China;
            simDescription2.FirstName = SimUtils.GetRandomGivenName(simDescription2.IsMale, currentWorld);
            simDescription2.LastName = SimUtils.GetRandomFamilyName(currentWorld);
            return simDescription2.Instantiate(point);
        }

        
        /*
        public static WorldName GetCurrentWorldAST()
        {
            if (GameUtils.CheatOverrideCurrentWorld != WorldName.Undefined)
            a
            return GameUtils.gGameUtils.GetCurrentWorldName();{
                return GameUtils.CheatOverrideCurrentWorld;
            }
        }
        */

        public static WorldName GetCurrentWorldLAYOPA()
        {
            WorldName anychild = WorldName.China;
            return anychild;
        }
        /// <summary>
        /// This a Finalize Anti-NPC
        /// </summary>
        /// <param name="deadSim">Dead Sim</param>
        /// <param name="timeoutToAdd">TimeOut</param>
        public static void FinalizeSimDeathRelationships(SimDescription deadSim, float timeoutToAdd)
        {
            bool flag = timeoutToAdd != 0f;
            foreach (Relationship item in Relationship.Get(deadSim))
            {
                if (item != null)
                {
                    SimDescription otherSimDescription = item.GetOtherSimDescription(deadSim);
                    Sim createdSim = otherSimDescription.CreatedSim;
                    if (createdSim != null)
                    {
                        LTRData.RelationshipClassification relationshipClass = LTRData.Get(item.LTR.CurrentLTR).RelationshipClass;
                        if (item.AreRomantic() && item.LTR.IsPositive && (relationshipClass == LTRData.RelationshipClassification.High || (item.LTR.Liking > BuffHeartBroken.LikingValueForPartner && relationshipClass == LTRData.RelationshipClassification.Medium)))
                        {
                            BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken;
                            if (flag)
                            {
                                buffInstanceHeartBroken = (createdSim.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                                if (buffInstanceHeartBroken == null || buffInstanceHeartBroken.BuffOrigin != Origin.FromWitnessingDeath || buffInstanceHeartBroken.TimeoutCount < timeoutToAdd)
                                {
                                    createdSim.BuffManager.AddElement(BuffNames.HeartBroken, timeoutToAdd, Origin.FromWitnessingDeath);
                                }
                            }
                            else
                            {
                                createdSim.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                            }
                            buffInstanceHeartBroken = (createdSim.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                            if (buffInstanceHeartBroken != null)
                            {
                                buffInstanceHeartBroken.MissedSim = deadSim;
                            }
                        }
                        else if (item.LTR.Liking > Urnstone.kMinLikingAddMourning || otherSimDescription.Household == deadSim.Household)
                        {
                            BuffMourning.BuffInstanceMourning buffInstanceMourning;
                            if (flag)
                            {
                                buffInstanceMourning = (createdSim.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                                if (buffInstanceMourning == null || buffInstanceMourning.TimeoutCount < timeoutToAdd)
                                {
                                    createdSim.BuffManager.AddElement(BuffNames.Mourning,  timeoutToAdd, Origin.FromWitnessingDeath);
                                }
                            }
                            else
                            {
                                createdSim.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                            }
                            buffInstanceMourning = (createdSim.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                            if (buffInstanceMourning != null)
                            {
                                buffInstanceMourning.MissedSim = deadSim;
                            }
                        }
                    }

                    //deadSim.LotCurrent.LastDiedSim = deadSim;

                    if (deadSim.Partner == otherSimDescription)
                    {
                        if (item.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Marry))
                        {
                            item.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Marry);
                            item.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                            WeddingAnniversary weddingAnniversary = item.WeddingAnniversary;
                            if (weddingAnniversary != null)
                            {
                                AlarmManager.Global.RemoveAlarm(weddingAnniversary.AnniversaryAlarm);
                                item.WeddingAnniversary = null;
                            }
                            SocialCallback.BreakUpDescriptionsShared(deadSim, otherSimDescription);
                        }
                        else if (item.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Propose))
                        {
                            item.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Propose);
                            item.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.BreakUp);
                        }
                        else if (item.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment))
                        {
                            item.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment);
                            item.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.BreakUp);
                        }
                        deadSim.ClearPartner();
                    }
                }
            }
        }
    }
}
