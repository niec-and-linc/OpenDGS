/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 27/09/2018
 * Time: 4:38
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

using NiecMod.Nra;
using NiecMod.KillNiec;

using NRaas;
using NRaas.CommonSpace.Helpers;

using Sims3.Gameplay;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.DreamsAndPromises;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Pools;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Tutorial;
using Sims3.Gameplay.Utilities;

using Sims3.SimIFace;
using Sims3.SimIFace.CAS;

using Sims3.UI;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Controllers.Niec;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.ThoughtBalloons;
using Sims3.UI.GameEntry;
using NiecMod.Helpers;



namespace NiecMod.Interactions
{
    /// <summary>
    /// Fix no Exit ExtKillSimNiec to Focre Kill NPC ONLY
    /// KillSim Is Helper By Niec
    /// Support Killing Baby The Sims 3 is Modified Support Killing Babies By Niec
    /// </summary>
    public sealed class ExtKillSimNiec : Interaction<Sim, Sim>, IRouteFromInventoryOrSelfWithoutCarrying
    {
        //[Persistable] // Test?
        public Household homehome = null;

        public AlarmHandle loadalarm = AlarmHandle.kInvalidHandle;


        // Token: 0x0600A7E2 RID: 42978 RVA: 0x002FB164 File Offset: 0x002FA164
        public override bool Run()
        {
            try
            {
                if (Simulator.CheckYieldingContext(false)) { NiecMod.Nra.SpeedTrap.Sleep(); }


                SimDescription simDescription = Actor.SimDescription;
                
                SimDescription.DeathType deathStyle = Actor.SimDescription.DeathStyle;
                if (this.simDeathType == SimDescription.DeathType.None) // Fix Error
                {
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                    return false;
                }
                if (Actor.Service != null && Actor.Service is GrimReaper && !mForceKillGrim)
                {
                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                    this.Actor.Motives.MaxEverything();
                    Actor.BuffManager.RemoveAllElements();
                    Actor.FadeIn();
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    return false;
                }
                try
                {
                    CheckCancelListInteractionByNiec(Actor);
                }
                catch (Exception)
                {
                    try
                    {
                        CheckCancelListInteractionByNiec(Actor);
                    }
                    catch (Exception)
                    { }
                }

                try
                {
                    if (Actor.Household == null && homehome != null)
                    {
                        homehome.Add(Actor.SimDescription);
                    }
                }
                catch (NullReferenceException)
                { }

                // I Know Loaded Save Fix None to Ghost Running TimePortal 
                try
                {
                    if (Actor.SimDescription.DeathStyle != SimDescription.DeathType.None && Actor.SimDescription.IsGhost || Actor.SimDescription.IsDead)
                    {
                        SimDescription simDescriptiongot = Actor.SimDescription;
                        simDescriptiongot.IsGhost = false;
                        World.ObjectSetGhostState(Actor.ObjectId, 0u, (uint)simDescriptiongot.AgeGenderSpecies);
                        Actor.Autonomy.Motives.RemoveMotive(CommodityKind.BeGhostly);
                        simDescriptiongot.SetDeathStyle(SimDescription.DeathType.None, false);
                        simDescriptiongot.ShowSocialsOnSim = true;
                        simDescriptiongot.IsNeverSelectable = false;
                        Actor.Autonomy.AllowedToRunMetaAutonomy = true;
                    }
                }
                catch
                { }

                try
                {
                    StartDeathEffect();
                }
                catch (Exception exception)
                {
                    NiecException.WriteLog("StartDeathEffect " + NiecException.NewLine + NiecException.LogException(exception), true, true, false);
                }
                if (Actor.LotCurrent.IsWorldLot) // Tested
                {
                    bool successFail = false;
                    // Fix NullReferenceException
                    NiecMod.Nra.SpeedTrap.Sleep(3);
                    if (PlumbBob.sSingleton == null)
                    {
                        GlobalFunctions.CreateObjectOutOfWorld("PlumbBob");
                    }
                    if ( /*Sim.ActiveActor == null || */ PlumbBob.sSingleton.mSelectedActor == null)
                    {
                        if (NTunable.kCreatesSim && !Create.RandomActiveHouseholdAndActiveActor() || !Create.CreateActiveHouseholdAndActiveActor()) return false;
                    }

                    if (PlumbBob.Singleton.mSelectedActor.LotHome == null)
                    {
                        try
                        {
                            List<Lot> lieast = new List<Lot>();
                            Lot lot = null;
                            foreach (object obj in LotManager.AllLotsWithoutCommonExceptions)
                            {
                                Lot lot2 = (Lot)obj;
                                lieast.Add(lot2);
                            }
                            try
                            {
                                Lot virtualLotHome = PlumbBob.Singleton.mSelectedActor.Household.VirtualLotHome;
                                if (virtualLotHome != null)
                                {
                                    virtualLotHome.VirtualMoveOut(PlumbBob.Singleton.mSelectedActor.Household);
                                }
                            }
                            catch
                            { }
                            
                            lot = RandomUtil.GetRandomObjectFromList<Lot>(lieast);
                            lot.MoveIn(PlumbBob.Singleton.mSelectedActor.Household);

                        }
                        catch
                        { }
                    }
                    // ActiveHousehold LotHome
                    try
                    {
                        var terraininstance = new Terrain.TeleportMeHere.Definition(false).CreateInstance(Terrain.Singleton, this.Actor, base.GetPriority(), base.Autonomous, base.CancellableByPlayer) as TerrainInteraction;
                        if (terraininstance != null)
                        {
                            Sim activeHouseholdselectedActor = PlumbBob.Singleton.mSelectedActor;
                            if (activeHouseholdselectedActor != null)
                            {
                                Mailbox mailboxOnHomeLot = Mailbox.GetMailboxOnHomeLot(activeHouseholdselectedActor);
                                if (mailboxOnHomeLot != null)
                                {
                                    Vector3 vector2;
                                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(mailboxOnHomeLot.Position);
                                    fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                    fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                                    if (GlobalFunctions.FindGoodLocation(Actor, fglParams, out terraininstance.Destination, out vector2))
                                    {
                                        successFail = terraininstance.RunInteraction();
                                    }
                                    else
                                    {
                                        terraininstance.Destination = mailboxOnHomeLot.Position;
                                        successFail = terraininstance.RunInteraction();
                                    }
                                }
                                else
                                {
                                    Vector3 vector2;
                                    World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(PlumbBob.SelectedActor.LotHome.Position);
                                    fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                    fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                                    if (GlobalFunctions.FindGoodLocation(Actor, fglParams, out terraininstance.Destination, out vector2))
                                    {
                                        successFail = terraininstance.RunInteraction();
                                    }
                                    else
                                    {
                                        terraininstance.Destination = PlumbBob.Singleton.mSelectedActor.LotHome.Position;
                                        successFail = terraininstance.RunInteraction();
                                    }
                                }
                            }
                        }
                    }
                    catch (NullReferenceException)
                    { }
                    


                    // ActiveHousehold
                    try
                    {
                        if (!successFail && Actor.IsInActiveHousehold)
                        {
                            bool successFailx = false;
                            var viasitLot = VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, this.Actor, base.GetPriority(), base.Autonomous, false) as VisitLot;
                            if (!viasitLot.RunInteraction())
                            {
                                //StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " Test? Cancel? :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                if (!Actor.SimDescription.IsEP11Bot)
                                {
                                    if (Actor.SimDescription.Elder)
                                    {
                                        Actor.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                    }
                                }
                                this.DeathTypeFix = false;
                                this.FixNotExit = false;
                                Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                                Actor.BuffManager.RemoveAllElements();
                                Actor.FadeIn();
                                this.Actor.Motives.MaxEverything();
                                return false;
                            }
                            successFailx = true;

                            // Part 2
                            if (!successFailx)
                            {
                                //StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " Test? Cancel? :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                var goToLoAtx = GoToLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, this.Actor, base.GetPriority(), base.Autonomous, false) as GoToLot;
                                //goToLoAtx.RunInteraction();
                                if (!goToLoAtx.RunInteraction())
                                {
                                    if (!Actor.SimDescription.IsEP11Bot)
                                    {
                                        if (Actor.SimDescription.Elder)
                                        {
                                            Actor.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                        }
                                    }
                                    this.DeathTypeFix = false;
                                    this.FixNotExit = false;
                                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                                    Actor.BuffManager.RemoveAllElements();
                                    Actor.FadeIn();
                                    this.Actor.Motives.MaxEverything();
                                    return false;
                                }
                            }
                        }
                        // End If ActiveHousehold





                        // NPC
                        if (!successFail && !Actor.IsInActiveHousehold)
                        {
                            bool successFailx = false;
                            var viasitLot = VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, this.Actor, base.GetPriority(), base.Autonomous, false) as VisitLot;

                            // Start
                            viasitLot.MustRun = true;
                            MustRun = true;
                            if (!viasitLot.RunInteraction())
                            {
                                this.FixNotExit = true;
                                this.Cleanup();
                                return true;

                                /*
                                this.CancelDeath = false;
                                this.DeathTypeFix = false;
                                this.FixNotExit = false;
                                this.ActiveFix = false;
                                try
                                {
                                    this.Actor.MoveInventoryItemsToAFamilyMember();
                                }
                                catch (Exception)
                                { }



                            
                                StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Cancel and Exit ExtKillSimNiec! to Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                // List Mod

                                try
                                {
                                    // Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                                    if (Actor.SimDescription.Household != null)
                                    {
                                        ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                                    }
                                    NFinalizeDeath.FinalizeSimDeathRelationships(Actor.SimDescription, 0);
                                }
                                catch (Exception)
                                {
                                    try
                                    {
                                        Actor.SimDescription.ShowSocialsOnSim = false;
                                        Actor.SimDescription.IsNeverSelectable = true;
                                        Actor.SimDescription.Contactable = false;
                                    }
                                    catch (Exception)
                                    { }
                                    Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                                    return true;
                                }
                                finally
                                {
                                    try
                                    {
                                        Actor.SimDescription.ShowSocialsOnSim = false;
                                        Actor.SimDescription.IsNeverSelectable = true;
                                        Actor.SimDescription.Contactable = false;
                                    }
                                    catch (Exception)
                                    { }
                                    Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                                }
                                return true;
                                // End
                                 */
                            }
                            successFailx = true;




                            // Part 2
                            if (!successFailx)
                            {
                                var goToLoAtx = GoToLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, this.Actor, base.GetPriority(), base.Autonomous, false) as GoToLot;
                                // Start

                                goToLoAtx.MustRun = true;
                                MustRun = true;
                                if (!goToLoAtx.RunInteraction())
                                {


                                    /*

                                    this.CancelDeath = false;
                                    this.DeathTypeFix = false;
                                    this.FixNotExit = false;
                                    this.ActiveFix = false;
                                    try
                                    {
                                        this.Actor.MoveInventoryItemsToAFamilyMember();
                                    }
                                    catch (Exception)
                                    { }

                                    StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Cancel and Exit ExtKillSimNiec! to Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    // List Mod

                                    try
                                    {
                                        // Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                                        if (Actor.SimDescription.Household != null)
                                        {
                                            ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                                        }
                                        NFinalizeDeath.FinalizeSimDeathRelationships(Actor.SimDescription, 0f);

                                    }
                                    catch (Exception)
                                    {
                                        try
                                        {
                                            Actor.SimDescription.ShowSocialsOnSim = false;
                                            Actor.SimDescription.IsNeverSelectable = true;
                                            Actor.SimDescription.Contactable = false;
                                        }
                                        catch (Exception)
                                        { }
                                        Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                                        return true;
                                    }
                                    finally
                                    {
                                        try
                                        {
                                            Actor.SimDescription.ShowSocialsOnSim = false;
                                            Actor.SimDescription.IsNeverSelectable = true;
                                            Actor.SimDescription.Contactable = false;
                                        }
                                        catch (Exception)
                                        { }
                                        Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                                    }
                                     */
                                    this.FixNotExit = true;
                                    this.Cleanup();
                                    return true;
                                }
                                // End
                            }
                        }
                    }
                    // End If NPCHousehold
                    catch (NullReferenceException)
                    { }
                    
                    
                }


                if (Actor.LotCurrent.IsWorldLot && !Actor.IsInActiveHousehold)
                {
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    try
                    {
                        this.Actor.MoveInventoryItemsToAFamilyMember();
                    }
                    catch (Exception)
                    { }

                    StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Cancel and Exit ExtKillSimNiec! to Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                    // List Mod

                    try
                    {
                        // Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                        ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                        NFinalizeDeath.FinalizeSimDeathRelationships(Actor.SimDescription, 0f);

                    }
                    catch (Exception)
                    {
                        try
                        {
                            Actor.SimDescription.ShowSocialsOnSim = false;
                            Actor.SimDescription.IsNeverSelectable = true;
                            Actor.SimDescription.Contactable = false;
                        }
                        catch (Exception)
                        { }
                        Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                        return true;
                    }
                    finally
                    {
                        try
                        {
                            Actor.SimDescription.ShowSocialsOnSim = false;
                            Actor.SimDescription.IsNeverSelectable = true;
                            Actor.SimDescription.Contactable = false;
                        }
                        catch (Exception)
                        { }
                        
                        Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                    }
                    return true;
                }



                



                try
                {
                    CheckCancelListInteractionByNiec(Actor);
                }
                catch (Exception)
                {
                    try
                    {
                        CheckCancelListInteractionByNiec(Actor);
                    }
                    catch (Exception)
                    { }
                }
                if (Actor.Service != null && Actor.Service is GrimReaper && !mForceKillGrim)
                {
                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                    this.Actor.Motives.MaxEverything();
                    Actor.BuffManager.RemoveAllElements();
                    Actor.FadeIn();
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    return false;
                }
                if (this.simDeathType == SimDescription.DeathType.None) // Fix Error
                {
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                    return false;
                }
                if (this.simDeathType == SimDescription.DeathType.PetOldAgeBad && !Actor.IsPet)
                {
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                    return false;
                }
                if (this.simDeathType == SimDescription.DeathType.PetOldAgeGood && !Actor.IsPet)
                {
                    this.CancelDeath = false;
                    this.DeathTypeFix = false;
                    this.FixNotExit = false;
                    this.ActiveFix = false;
                    Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                    
                    return false;
                }
                if (Actor.IsInActiveHousehold || NFinalizeDeath.IsSimFastActiveHousehold(Actor))
                {
                    //if (this.simDeathType == SimDescription.DeathType.None) return false;
                    //if (NiecException.AcceptCancelDialogWithoutCommonException.Valid && NiecException.AcceptCancelDialogWithoutCommonException.Invoke<bool>(new object[] { "Do you want Force Kill? (Yes Run or No Next)" }))
                    if (AcceptCancelDialog.Show("Do you want Force Kill? (Yes Run or No Next)") /* && Simulator.CheckYieldingContext(false) */)
                    {
                        try
                        {

                            try
                            {

                                HelperNra helperNra = new HelperNra();

                                //helperNra = HelperNra;

                                helperNra.mSim = Actor;

                                helperNra.mSimdesc = Actor.SimDescription;

                                helperNra.mdeathtype = simDeathType;

                                AlarmManager.Global.AddAlarm(1f, TimeUnit.Days, new AlarmTimerCallback(helperNra.CheckKillSimNotUrnstonePro), "Esoiax44", AlarmType.AlwaysPersisted, null);
                            }
                            catch (ResetException) { throw; }
                            catch (Exception exception)
                            { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }

                            //Actor.SimDescription.SetDeathStyle(this.simDeathType, false);

                            //Actor.SimDescription.IsGhost = true;
                            //Actor.SimDescription.ShowSocialsOnSim = false;
                            //Actor.SimDescription.IsNeverSelectable = true;
                            //Actor.SimDescription.Contactable = false;




                            try
                            {
                                Actor.SimDescription.IsGhost = true;
                                Actor.SimDescription.SetDeathStyle(simDeathType, true);
                                Actor.SimDescription.ShowSocialsOnSim = false;
                                Actor.SimDescription.IsNeverSelectable = true;
                                Actor.SimDescription.Contactable = false;


                                if (Actor.SimDescription.Household != null)
                                {
                                    //SimDescription actor = Actor.SimDescription;
                                    List<Sim> listx = new List<Sim>();
                                    foreach (Sim sim in Actor.Household.Sims)
                                    {
                                        if (sim != Actor && !sim.IsSelectable && !(sim.Service is GrimReaper) && !sim.BuffManager.HasElement(BuffNames.Mourning) && !sim.BuffManager.HasElement(BuffNames.HeartBroken))
                                        {
                                            listx.Add(sim);
                                        }
                                    }
                                    if (listx.Count > 0)
                                    {
                                        foreach (Sim nlist in listx)
                                        {
                                            BuffMourning.BuffInstanceMourning buffInstanceMourning;

                                            buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                                            if (buffInstanceMourning == null)
                                            {
                                                nlist.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                                            }
                                            buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                                            if (buffInstanceMourning != null)
                                            {
                                                buffInstanceMourning.MissedSim = Actor.SimDescription;
                                            }

                                            BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken;
                                            buffInstanceHeartBroken = (nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                                            if (buffInstanceHeartBroken == null || buffInstanceHeartBroken.BuffOrigin != Origin.FromWitnessingDeath)
                                            {
                                                nlist.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                                            }
                                            else
                                            {
                                                nlist.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                                            }
                                            buffInstanceHeartBroken = (nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                                            if (buffInstanceHeartBroken != null)
                                            {
                                                buffInstanceHeartBroken.MissedSim = Actor.SimDescription;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (ResetException) { throw; }
                            catch (NullReferenceException) { }





                            try
                            {
                                List<Sim> listxe = new List<Sim>();
                                foreach (Sim sim in Actor.LotCurrent.GetAllActors())
                                {
                                    if (sim != Actor && !sim.IsInActiveHousehold && !(sim.Service is GrimReaper) && !sim.BuffManager.HasElement(BuffNames.Mourning) && !sim.BuffManager.HasElement(BuffNames.HeartBroken))
                                    {
                                        listxe.Add(sim);
                                    }
                                }
                                if (listxe.Count > 0)
                                {
                                    foreach (Sim nlist in listxe)
                                    {
                                        BuffMourning.BuffInstanceMourning buffInstanceMourning;

                                        buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                                        if (buffInstanceMourning == null)
                                        {
                                            nlist.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                                        }
                                        buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                                        if (buffInstanceMourning != null)
                                        {
                                            buffInstanceMourning.MissedSim = Actor.SimDescription;
                                        }

                                        BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken;
                                        buffInstanceHeartBroken = (nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                                        if (buffInstanceHeartBroken == null || buffInstanceHeartBroken.BuffOrigin != Origin.FromWitnessingDeath)
                                        {
                                            nlist.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                                        }
                                        else
                                        {
                                            nlist.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                                        }
                                        buffInstanceHeartBroken = (nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                                        if (buffInstanceHeartBroken != null)
                                        {
                                            buffInstanceHeartBroken.MissedSim = Actor.SimDescription;
                                        }
                                    }
                                }
                            }
                            catch (ResetException) { throw; }
                            catch (NullReferenceException) { }

                            NiecMod.Nra.SpeedTrap.Sleep();
                            Urnstone mGravebackup = Urnstone.CreateGrave(Actor.SimDescription, false, false);
                            if (mGravebackup != null)
                            {
                                if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(Actor.Position), false))
                                {
                                    mGravebackup.SetPosition(Actor.Position);
                                }
                                mGravebackup.OnHandToolMovement();
                                mGravebackup.mHeartBrokenBroadcaster = new ReactionBroadcaster(mGravebackup, Urnstone.kHeartBrokenParams, UrnstoneHeartBrokenReaction);
                                // Test?

                                //Actor.InteractionQueue.AddNext(entry);
                            }




                            if (Actor == PlumbBob.SelectedActor)
                            {
                                if (IntroTutorial.IsRunning)
                                {
                                    IntroTutorial.ForceExitTutorial();
                                }

                            }

                            if (Household.ActiveHousehold != null)
                            {
                                List<SimDescription> householdonly1 = Household.ActiveHousehold.SimDescriptions;
                                if (Actor.Household != null && Actor.Household == Household.ActiveHousehold)
                                {
                                    if (householdonly1.Count == 1)
                                    {
                                        List<Sim> listat = new List<Sim>();
                                        foreach (Sim actor in LotManager.Actors)
                                        {
                                            if (actor.SimDescription.CreatedSim != null && !actor.IsInActiveHousehold && actor.IsHuman && !actor.SimDescription.IsGhost && actor.SimDescription.TeenOrAbove && actor.SimDescription.Household != null && actor.SimDescription.Household != Household.NpcHousehold && actor.SimDescription.Household != Household.PetHousehold && actor.SimDescription.Household != Household.MermaidHousehold && actor.SimDescription.Household != Household.TouristHousehold && actor.SimDescription.Household != Household.PreviousTravelerHousehold && actor.SimDescription.Household != Household.AlienHousehold && actor.SimDescription.Household != Household.ServobotHousehold)
                                            {
                                                listat.Add(actor);
                                            }
                                        }
                                        if (listat.Count == 0)
                                        {
                                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec" + NiecException.NewLine + "Sorry, Found if listat.Count == 0" + NiecException.NewLine + "Note: Cant Find Sims" + NiecException.NewLine + "ExtKillSimNiec is cancelled", StyledNotification.NotificationStyle.kGameMessageNegative));
                                            if (!Actor.SimDescription.IsEP11Bot)
                                            {
                                                if (Actor.SimDescription.Elder)
                                                {
                                                    Actor.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                }
                                            }
                                            DeathTypeFix = false;
                                            FixNotExit = false;
                                            try
                                            {
                                                SimDescription simDescriptiongot = Actor.SimDescription;
                                                simDescriptiongot.IsGhost = false;
                                                World.ObjectSetGhostState(Actor.ObjectId, 0u, (uint)simDescriptiongot.AgeGenderSpecies);
                                                Actor.Autonomy.Motives.RemoveMotive(CommodityKind.BeGhostly);
                                                simDescriptiongot.SetDeathStyle(SimDescription.DeathType.None, false);
                                                Actor.Autonomy.AllowedToRunMetaAutonomy = true;
                                            }
                                            catch
                                            { }
                                            //Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                                            try
                                            {
                                                NiecMod.Nra.SpeedTrap.Sleep();
                                                Actor.BuffManager.RemoveAllElements();
                                                Actor.FadeIn();
                                                Actor.Motives.MaxEverything();
                                                NiecMod.Nra.SpeedTrap.Sleep();
                                                mGravebackup.Destroy();
                                            }
                                            catch
                                            { }
                                            
                                            return false;
                                        }
                                        if (listat.Count > 0)
                                        {
                                            Sim randomObjectFromList = RandomUtil.GetRandomObjectFromList(listat);
                                            /*
                                            if (!randomObjectFromList.InWorld)
                                            {
                                                randomObjectFromList.AddToWorld();
                                            }
                                             * */
                                            PlumbBob.ForceSelectActor(randomObjectFromList);
                                            
                                        }
                                    }
                                }
                            }


                            if (Actor.IsActiveSim)
                            {
                                //UserToolUtils.OnClose();
                                LotManager.SelectNextSim();
                            }


                            if (Actor.SimDescription.IsPregnant)
                            {
                                this.Actor.SimDescription.Pregnancy.ClearPregnancyData();
                                if (this.Actor.SimDescription.Pregnancy == null)
                                {
                                    StyledNotification.Show(new StyledNotification.Format(this.Actor.Name + Localization.LocalizeString("cmarXmods/PregControl/PregNotice:NoMorePreg", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                                else
                                {
                                    StyledNotification.Show(new StyledNotification.Format(Localization.LocalizeString("cmarXmods/PregControl/PregNotice:TerminationFail", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                            }

                            try
                            {
                                Actor.BuffManager.RemoveAllElements();
                                Actor.MoveInventoryItemsToAFamilyMember();
                            }
                            catch (ResetException) { throw; }
                            catch { }


                            this.DeathTypeFix = false;
                            this.ActiveFix = false;
                            this.FixNotExit = false;
                            //Actor.SimDescription.Contactable = false;
                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " Ok Active Household Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));

                            NiecMod.Nra.SpeedTrap.Sleep();

                            try
                            {
                                if (mGravebackup != null)
                                {
                                    if (Actor.SimDescription.Household != null)
                                    {

                                        try
                                        {
                                            Actor.SimDescription.Household.Remove(Actor.SimDescription);
                                            Household.NpcHousehold.Add(Actor.SimDescription);
                                        }
                                        catch
                                        { }

                                    }

                                    NiecMod.Nra.SpeedTrap.Sleep();
                                    InteractionInstance entry = Sims3.Gameplay.Objects.Urnstone.ReturnToGrave.Singleton.CreateInstance(mGravebackup, Actor, new InteractionPriority((InteractionPriorityLevel)8195, 0f), false, true);
                                    entry.MustRun = true;
                                    Actor.FadeOut();
                                    entry.RunInteraction();
                                    Actor.FadeOut();
                                    AlarmManager.Global.AddAlarm(3f, TimeUnit.Minutes, new AlarmTimerCallback(FailedCallBook), "Esoiax44X", AlarmType.AlwaysPersisted, null);
                                    return true;
                                }

                            }
                            catch (ResetException) { throw; }
                            catch { }
                            

                            //return true;
                            //ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                        }
                        catch (NullReferenceException) { }

                        catch (ResetException) { throw; }

                        return false;

                    }
                    if (!AcceptCancelDialog.Show("Do you want Kill? (Yes Run or No Cancel)") /* && Simulator.CheckYieldingContext(false) */)
                    {
                        if (!Actor.SimDescription.IsEP11Bot)
                        {
                            if (Actor.SimDescription.Elder)
                            {
                                Actor.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                            }
                        }
                        DeathTypeFix = false;
                        FixNotExit = false;
                        Actor.SimDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                        Actor.BuffManager.RemoveAllElements();
                        Actor.FadeIn();
                        Actor.Motives.MaxEverything();
                        return false;
                    }
                }
                else
                {
                    try
                    {

                        HelperNraPro helperNra = new HelperNraPro();

                        //helperNra = HelperNra;

                        if (Actor.Household != null)
                        {
                            helperNra.mHousehold = Actor.Household;
                        }
                        

                        helperNra.mSim = Actor;
                        helperNra.mdeathtype = simDeathType;

                        helperNra.mSimdesc = Actor.SimDescription;

                        helperNra.mHouseVeri3 = Actor.Position;

                        //helperNra.mdeathtype = simDeathType;

                        /*helperNra.malarmx =  */
                        AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(helperNra.FailedCallBookSleep), "FailedCallBookSleep " + Actor.Name, AlarmType.AlwaysPersisted, null);
                    }
                    catch (Exception exception)
                    { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }
                }


                




                this.simDeathType = simDescription.SetDeathStyle(this.simDeathType, this.Actor.IsSelectable);
                if (Actor.SimDescription.IsPregnant && !Actor.IsInActiveHousehold)
                {
                    this.Actor.SimDescription.Pregnancy.ClearPregnancyData();
                    if (this.Actor.SimDescription.Pregnancy == null)
                    {
                        StyledNotification.Show(new StyledNotification.Format(this.Actor.Name + Localization.LocalizeString("cmarXmods/PregControl/PregNotice:NoMorePreg", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                    else
                    {
                        StyledNotification.Show(new StyledNotification.Format(Localization.LocalizeString("cmarXmods/PregControl/PregNotice:TerminationFail", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                }

                /*
                if (this.simDeathType == SimDescription.DeathType.ScubaDrown && !(this.Actor.Posture is ScubaDiving)) ... EA Dont Lot ScubaBrown Fix Remove
                {
                    return false;
                }
                */
                //if (Actor.IsNPC)
                if (!Actor.IsInActiveHousehold || !NFinalizeDeath.IsSimFastActiveHousehold(Actor))
                {
                    MustRun = false;
                    try
                    {
                        this.Actor.MoveInventoryItemsToAFamilyMember();
                    }
                    catch (NullReferenceException)
                    { }

                    if (!AssemblyCheckByNiec.IsInstalled("DGSCore"))
                    {
                        if (Actor.SimDescription.TraitManager.HasElement(TraitNames.Unlucky))
                        {
                            this.Actor.SimDescription.TraitManager.RemoveAllElements();
                            this.Actor.SimDescription.TraitManager.AddElement(TraitNames.Brave);
                            this.Actor.SimDescription.TraitManager.AddElement(TraitNames.Evil);
                            this.Actor.SimDescription.TraitManager.AddElement(TraitNames.Lucky);
                            this.Actor.SimDescription.TraitManager.AddElement(TraitNames.Athletic);
                            this.Actor.SimDescription.TraitManager.AddElement(TraitNames.Perfectionist);
                        }
                    }
                    /*
                    if (this.simDeathType == SimDescription.DeathType.Ranting)
                    {
                        this.Actor.BuffManager.AddElement(BuffNames.ThereAndBackAgain, Origin.FromRanting);
                    }
                    */
                }

                try
                {
                    if (NTunable.kCarmeraExtKillSimNiec)
                    {
                        if (!Camera.CameraTargetWithInLot(Actor.LotCurrent))
                        {
                            Camera.CameraNotification.ShowNotificationAndFocusOnSim(Actor.ObjectId, "ExtKillSimNiec: Found " + Actor.Name + " is Died " + simDeathType.ToString(), Actor);
                        }
                    }
                }
                catch (Exception exception)
                {
                    NiecException.PrintMessage(exception.Message + NiecException.NewLine + exception.StackTrace);
                }

                /*
                // Add New
                if (Actor.Posture != null && !(Actor.Posture is ScubaDiving))
                {
                    Lot lotloot = Actor.LotCurrent;
                    if (GameUtils.IsInstalled(ProductVersion.EP10) && lotloot != null && lotloot.IsDivingLot)
                    {
                        ScubaDiving scubaDivinga = new ScubaDiving(mCurrentStateMachine, Ocean.Singleton, Actor);
                        if (scubaDivinga != null)
                        {
                            Actor.Posture = scubaDivinga;
                            scubaDivinga.StartBubbleEffects();
                        }
                    }
                }
                
                */


                // Add New
                if (GameUtils.IsInstalled(ProductVersion.EP10) && Actor.Posture != null && !(Actor.Posture is ScubaDiving))
                {
                    Lot lotloot = Actor.LotCurrent;
                    if (lotloot != null && lotloot.IsDivingLot)
                    {
                        ScubaDiving scubaDivinga = new ScubaDiving(mCurrentStateMachine, Ocean.Singleton, Actor);
                        if (scubaDivinga != null)
                        {
                            Actor.Posture = scubaDivinga;
                            scubaDivinga.StartBubbleEffects();
                        }
                    }
                }


                if (!Actor.LotCurrent.IsWorldLot)
                {
                    GrimReaper.Instance.MakeServiceRequest(Actor.LotCurrent, true, ObjectGuid.InvalidObjectGuid);
                }
                

                if (simDescription.IsMermaid)
                {
                    ScubaDiving scubaDiving = this.Actor.Posture as ScubaDiving;
                    if (scubaDiving != null)
                    {
                        InteractionInstance interactionInstance = EndScubaDive.Singleton.CreateInstance(this.Actor, this.Actor, base.GetPriority(), base.Autonomous, false);
                        interactionInstance.RunInteraction();
                        Lot nearestLot = LotManager.GetNearestLot(this.Actor.Position);
                        if (nearestLot != null)
                        {
                            GoToLot goToLot = VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, this.Actor, base.GetPriority(), base.Autonomous, false) as GoToLot;
                            goToLot.RunInteraction();
                        }
                        else
                        {
                            GoHome goHome = VisitLot.Singleton.CreateInstance(Household.ActiveHousehold.LotHome, this.Actor, base.GetPriority(), base.Autonomous, false) as GoHome;
                            goHome.RunInteraction();
                        }
                    }
                    else
                    {
                        SwimmingInPool swimmingInPool = this.Actor.Posture as SwimmingInPool;
                        if (swimmingInPool != null)
                        {
                            ISwimLocation containerPool = swimmingInPool.ContainerPool;
                            if (containerPool != null)
                            {
                                containerPool.RouteToEdge(this.Actor);
                            }
                        }
                    }
                }


                /*
                if (this.Actor.IsSelectable)
                {
                    EventTracker.SendEvent(new SimDescriptionEvent(EventTypeId.kSelectableSimDied, simDescription));
                }
                */
                if (this.Actor == PlumbBob.SelectedActor)
                {
                    if (IntroTutorial.IsRunning)
                    {
                        IntroTutorial.ForceExitTutorial();
                    }
                }
                /*
                if (this.Actor.LotCurrent == PlumbBob.SelectedActor.LotHome || this.Actor.Household == PlumbBob.SelectedActor.Household)
                {
                    Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Normal, Sims3.Gameplay.Gameflow.SetGameSpeedContext.Gameplay);
                }
                 */

                try
                {
                    CheckCancelListInteractionByNiec(Actor);
                }
                catch (Exception)
                {
                    try
                    {
                        CheckCancelListInteractionByNiec(Actor);
                    }
                    catch (Exception)
                    { }
                }

                try
                {
                    if (Actor.Household != Household.ActiveHousehold)
                    {
                        this.Actor.Autonomy.IncrementAutonomyDisabled();
                    }
                }
                catch
                { }
                
                //this.Actor.Autonomy.IncrementAutonomyDisabled();

                simDescription.IsNeverSelectable = false;
                /*
                if (this.Actor.IsActiveSim)
                {
                    UserToolUtils.OnClose();
                    LotManager.SelectNextSim();
                }
                 */
                simDescription.ShowSocialsOnSim = true;
                TraitFunctions.SearchForUnrelatedEvilSimToTriggerLaugh(this.Actor);
                List<BuffInstance> list = new List<BuffInstance>(this.Actor.BuffManager.List);
                foreach (BuffInstance buffInstance in list)
                {
                    this.Actor.BuffManager.RemoveElement(buffInstance.Guid);
                }
                if (this.simDeathType == SimDescription.DeathType.Meteor)
                {
                    this.Actor.FadeIn();
                }
                /*
                if (!this.Actor.LotCurrent.IsFireOnLot())
                {
                    Urnstone.CreateDeathReactionBroadcaster(this.Actor);
                }
                */
                Urnstone.CreateDeathReactionBroadcaster(this.Actor);
                StateMachineClient stateMachineClient = null;
                this.Actor.AddInteraction(Sim.DeathReaction.Singleton);
                if (this.Actor.IsHuman)
                {

                    if (this.PlayDeathAnimation && !Actor.SimDescription.Baby)
                    {
                        stateMachineClient = StateMachineClient.Acquire(this.Actor, "DeathTypes");
                        stateMachineClient.SetActor("x", this.Actor);
                        stateMachineClient.EnterState("x", "Enter");
                        if (this.simDeathType == SimDescription.DeathType.Burn || this.simDeathType == SimDescription.DeathType.MummyCurse || this.simDeathType == SimDescription.DeathType.Thirst)
                        {
                            int xEventId = (this.simDeathType == SimDescription.DeathType.Burn) ? 100 : 101;
                            stateMachineClient.AddOneShotScriptEventHandler((uint)xEventId, new SacsEventHandler(this.EventCallbackCreateAshPile));
                        }
                        else if (this.simDeathType == SimDescription.DeathType.OldAge)
                        {
                            stateMachineClient.AddOneShotScriptEventHandler(101u, new SacsEventHandler(this.EventCallbackTurnToGhost));
                        }
                        else if (this.simDeathType == SimDescription.DeathType.Drown)
                        {
                            stateMachineClient.AddOneShotScriptEventHandler(100u, new SacsEventHandler(this.EventCallbackFadeOutSim));
                        }
                    }
                    else
                    {
                        try
                        {
                            Actor.LoopIdle();
                        }
                        catch
                        { }
                        
                    }
                    /*
                    if (GrimReaperSituation.ShouldDoDeathEvent(this.Actor))
                    {
                        //Camera.FocusOnSim(this.Actor, Urnstone.kCameraZoomOnDeathLerp, Urnstone.kCameraPitchOnDeathLerp);
                        World.SetWallCutawayFocusPos(this.Actor.Position);
                        if (this.simDeathType == SimDescription.DeathType.Thirst)
                        {
                            Audio.StartObjectSound(this.Actor.ObjectId, "sting_vampire_death", false);
                        }
                    }
                    */
                    World.SetWallCutawayFocusPos(this.Actor.Position);
                    if (this.simDeathType == SimDescription.DeathType.Thirst)
                    {
                        Audio.StartObjectSound(this.Actor.ObjectId, "sting_vampire_death", false);
                    }
                    EventTracker.SendEvent(new DnPSubjectDestroyedEvent(null, this.Actor.SimDescription, false));
                    ScubaDiving scubaDiving2 = this.Actor.Posture as ScubaDiving;
                    if (this.PlayDeathAnimation && !Actor.SimDescription.Baby)
                    {
                        if (scubaDiving2 != null)
                        {
                            scubaDiving2.StopBubbleEffects();
                        }
                        stateMachineClient.RequestState("x", Urnstone.DeathAnimns[(uint)this.simDeathType]);
                        stateMachineClient.RequestState("x", "Exit");
                    }
                    else
                    {
                        try
                        {
                            Actor.LoopIdle();
                        }
                        catch
                        { }

                    }
                    if (scubaDiving2 != null)
                    {
                        this.Actor.AddInteraction(GrimReaperSituation.ReapSoul.Singleton);
                    }
                    else
                    {
                        this.Actor.AddInteraction(GrimReaperSituation.ReapSoul.Singleton);
                    }
                }
                else
                {
                    Audio.StartObjectSound(this.Actor.ObjectId, "sting_pet_death", false);
                    this.SocialJig = SocialJigTwoPerson.CreateJigForTwoPersonSocial(CASAgeGenderFlags.Human, CASAgeGenderFlags.Adult, this.Target.SimDescription.Species, this.Target.SimDescription.Age, 0f);
                    Slot slotForActor = this.SocialJig.GetSlotForActor(this.Actor, false);
                    float num = (this.SocialJig.GetPositionOfSlot(slotForActor) - this.SocialJig.GetPositionOfSlot(SocialJigTwoPerson.RoutingSlots.SimA)).Length();
                    Vector3 forward = -this.Actor.ForwardVector;
                    Vector3 position = this.Actor.Position;
                    position = World.SnapToFloor(position);
                    this.SocialJig.RegisterParticipants(null, this.Actor);
                    if (!GlobalFunctions.FindGoodLocationNearby(this.SocialJig, ref position, ref forward, -num, GlobalFunctions.FindGoodLocationStrategies.All, FindGoodLocationBooleans.PreferEmptyTiles | FindGoodLocationBooleans.Routable | FindGoodLocationBooleans.TemporaryPlacement))
                    {
                        position = World.SnapToFloor(this.Actor.Position) - num;
                    }
                    this.SocialJig.SetPosition(position);
                    this.SocialJig.SetForward(forward);
                    this.SocialJig.SetOpacity(0f, 0f);
                    this.SocialJig.AddToWorld();
                    this.Actor.DoRoute(this.SocialJig.RouteToJigB(this.Actor));
                    EventTracker.SendEvent(new DnPSubjectDestroyedEvent(null, this.Actor.SimDescription, false));
                    this.Actor.AddInteraction(GrimReaperSituation.ReapPetSoul.PetSingleton);
                    this.Actor.LoopIdle();
                }

                //if (!Actor.IsInActiveHousehold)
                if (!Actor.IsInActiveHousehold || !NFinalizeDeath.IsSimFastActiveHousehold(Actor))
                {
                    NiecMod.Nra.SpeedTrap.Sleep(20);
                }

                try
                {
                    CheckCancelListInteractionByNiec(Actor);
                }
                catch (Exception)
                {
                    try
                    {
                        CheckCancelListInteractionByNiec(Actor);
                    }
                    catch (Exception)
                    { }
                }


                NiecMod.Nra.SpeedTrap.Sleep(1);


                if (Actor.IsInActiveHousehold || NFinalizeDeath.IsSimFastActiveHousehold(Actor))// (Actor.IsInActiveHousehold)
                {
                    base.DoLoop(ExitReason.Default);
                }
                else if (Actor.Service is GrimReaper)
                {
                    Actor.ClearExitReasons();
                    base.DoLoop(ExitReason.CanceledByScript);
                }
                else if (!AssemblyCheckByNiec.IsInstalled("DGSCore"))
                {
                    Actor.ClearExitReasons();
                    base.DoLoop(ExitReason.HigherPriorityNext);
                }
                else
                {
                    Actor.ClearExitReasons();
                    //base.DoLoop(ExitReason.CanceledByScript, LoopExtKill, stateMachineClient);
                    base.DoLoop(ExitReason.CanceledByScript);
                }




                if (!Actor.IsInActiveHousehold)
                {
                    NiecMod.Nra.SpeedTrap.Sleep(15);
                }


                try
                {
                    CheckCancelListInteractionByNiec(Actor);
                }
                catch (Exception)
                {
                    try
                    {
                        CheckCancelListInteractionByNiec(Actor);
                    }
                    catch (Exception)
                    { }
                }
                Actor.RemoveInteractionByType(Sim.DeathReaction.Singleton);
                //if (Actor.InteractionQueue.HasInteractionOfType(NiecDefinitionDeathInteractionSocialSingleton) && !Actor.IsInActiveHousehold)
                //if (!Actor.IsInActiveHousehold)
                if (!NFinalizeDeath.IsSimFastActiveHousehold(Actor))
                {
                    if (Actor.InteractionQueue.HasInteractionOfType(KillSimNiecX.NiecDefinitionDeathInteraction.SocialSingleton) || Actor.InteractionQueue.HasInteractionOfType(NiecDefinitionDeathInteractionSocialSingleton) || Actor.InteractionQueue.HasInteractionOfType(GrimReaperSituation.BeReapedDiving.Singleton) || Simulator.CheckYieldingContext(false) && AcceptCancelDialog.Show("Do you want FixNotExit? (Yes Exit or No Run Force Kill)", true))
                    {
                        FixNotExit = false;
                    }
                }
                //this.FixNotExit = false;
                CancelDeath = false;
                /*
                try
                {
                    CheckCancelListInteractionByNiec(Actor);
                }
                catch (Exception)
                {
                    try
                    {
                        CheckCancelListInteractionByNiec(Actor);
                    }
                    catch (Exception)
                    { }
                }
                */
                
                return true;
            }

            catch (ResetException)
            {
                //this.Cleanup();
                // Crash Game
                /*
                try
                {
                    throw new NiecModException("Server Error :D", exae);
                }
                catch (NiecModException exeax)
                {
                    if (exeax.StackTrace != null)
                    {
                        NiecException.WriteLog("ResetException KillSim: " + NiecException.NewLine + NiecException.LogException(exeax), true, false, false);
                    }
                    NiecException.PrintMessage("KillSim Run():" + NiecException.NewLine + "ResetException is Found No: " + NiecException.sLogEnumerator);
                }
                 */
                throw;
            }

            catch (Exception exception)
            {
                NiecException.WriteLog("ExtKillSimNiec: " + NiecException.NewLine + NiecException.LogException(exception), true, true, false);
                if (this.ActiveFix)
                {
                    if (!Actor.IsInActiveHousehold)
                    {
                        if (Actor.SimDescription.IsPregnant)
                        {
                            this.Actor.SimDescription.Pregnancy.ClearPregnancyData();
                            if (this.Actor.SimDescription.Pregnancy == null)
                            {
                                StyledNotification.Show(new StyledNotification.Format(this.Actor.Name + Localization.LocalizeString("cmarXmods/PregControl/PregNotice:NoMorePreg", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                            else
                            {
                                StyledNotification.Show(new StyledNotification.Format(Localization.LocalizeString("cmarXmods/PregControl/PregNotice:TerminationFail", new object[0]), StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                        }
                        try
                        {
                            this.Actor.MoveInventoryItemsToAFamilyMember();
                        }
                        catch (NullReferenceException)
                        { }
                        this.DeathTypeFix = false;
                        this.FixNotExit = false;
                        this.CancelDeath = false;
                        this.ActiveFix = false;
                        StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Failed Error. Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                        try
                        {
                            // Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                            ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                            NFinalizeDeath.FinalizeSimDeathRelationships(Actor.SimDescription, 0);

                        }
                        catch (Exception)
                        {
                            Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " Failed To Catch Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            return false;
                        }
                        finally
                        {
                            Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " Done To Finally Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                        }
                    }
                }
            }
            return false;
        }




        public void LoopExtKill(StateMachineClient smc, LoopData loopData)
        {
            if (Simulator.CheckYieldingContext(false) && NiecMod.Nra.SpeedTrap.SleepBool(71200))
            {
                Actor.AddExitReason(ExitReason.CanceledByScript);
            }
            else
            {
                //Actor.InteractionQueue.DeQueue(false);
               // this.Cleanup();
                Actor.AddExitReason(ExitReason.CanceledByScript);
            }
        }


        public void CheckKillAllSimGraveToPos()
        {
            Sim simgrim = null;
            List<Urnstone> ras = new List<Urnstone>();
            try
            {
                foreach (Urnstone temp in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                {
                    try
                    {
                        if (!ras.Contains(temp))
                            ras.Add(temp);
                    }
                    catch
                    { }
                }
            }
            catch
            { }
            //while (true) // UnTested
            //for (int i = 0; i < 20; i++)
            foreach (Urnstone mGravebackup in ras)
            {
                if (mGravebackup != null && mGravebackup.LotCurrent != null && mGravebackup.LotCurrent.IsWorldLot)
                {
                    List<Lot> lieast = new List<Lot>();
                    Lot loet = null;
                    foreach (Lot obj in LotManager.AllLotsWithoutCommonExceptions)
                    {
                        lieast.Add(obj);
                    }
                    if (lieast.Count != 0)
                    {
                        try
                        {
                            loet = RandomUtil.GetRandomObjectFromList<Lot>(lieast);

                            if (loet != null)
                            {
                                if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(loet.Position), false))
                                {
                                    mGravebackup.SetPosition(loet.Position);
                                }
                            }
                        }
                        catch
                        { }
                    }
                }


                NiecMod.Nra.SpeedTrap.Sleep();
                //Urnstone mGravebackup = Create.GhostsGrave(Actor.SimDescription);
                if (mGravebackup != null && object.ReferenceEquals(mGravebackup.DeadSimsDescription, Actor.SimDescription))
                {
                    try
                    {
                        try
                        {
                            if (simgrim == null)
                            {
                                foreach (Sim sim in LotManager.Actors)
                                {
                                    if (sim.Service != null && (sim.Service is GrimReaper))
                                    {
                                        simgrim = sim;
                                        break;
                                    }
                                }
                            }
                        }
                        catch
                        { }

                        if (simgrim != null && !simgrim.HasBeenDestroyed && !simgrim.LotCurrent.IsWorldLot )
                        {
                            if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(simgrim.Position), false))
                            {
                                mGravebackup.SetPosition(simgrim.Position);
                            }
                        }
                        else if (PlumbBob.Singleton != null && PlumbBob.Singleton.mSelectedActor != null && !PlumbBob.Singleton.mSelectedActor.LotCurrent.IsWorldLot)
                        {
                            if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position), false))
                            {
                                mGravebackup.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                            }
                        }
                        else
                        {
                            Lot lotm = mGravebackup.LotCurrent;
                            if (lotm != null)
                            {
                                if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(lotm.Position), false))
                                {
                                    mGravebackup.SetPosition(lotm.Position);
                                }
                            }
                        }

                        mGravebackup.OnHandToolMovement();
                        mGravebackup.FadeIn(false, 5f);
                    }
                    catch
                    { }

                    try
                    {
                        mGravebackup.OnHandToolMovement();
                        mGravebackup.FadeIn(false, 5f);
                    }
                    catch
                    { }
                }
                //else return;
            }
            AlarmManager.Global.RemoveAlarm(loadalarm);
            loadalarm = AlarmHandle.kInvalidHandle;

        }


        // Token: 0x0600A7E3 RID: 42979 RVA: 0x002FB788 File Offset: 0x002FA788
        public override void Cleanup()
        {
            string msg = " Sim Name Null ";
            string msg2 = " SimDescription Null ";
            try
            {
                if (Actor.SimDescription != null)
                {
                    msg2 = " Found SimDescription ";
                }
            }
            catch
            { }
            try
            {
                if (Actor.Name != null)
                {
                    msg = " Found Actor Name ";
                }
            }
            catch
            { }
            try
            {
                StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: Test?" + msg + msg2, StyledNotification.NotificationStyle.kGameMessagePositive));
            }
            catch
            { }
            
            

            
            try
            {
                int activehouseholdint = 0;
                if (GameStates.IsGameShuttingDown || Responder.Instance.IsGameStateShuttingDown)
                {
                    if (this.SocialJig != null)
                    {
                        this.SocialJig.Destroy();
                        this.SocialJig = null;
                    }
                    base.Cleanup();
                    return;
                }
                 // ????
                if (this.FixNotExit)
                {
                    try
                    {
                        activehouseholdint = NFinalizeDeath.ActiveHouseholdWithoutDGSCore.AllSimDescriptions.Count;
                    }
                    catch
                    { }
                    NiecMod.Nra.SpeedTrap.Sleep(1);
                    //if (!Actor.IsInActiveHousehold)
                    //if (Actor.SimDescription != null && PlumbBob.sSingleton != null && PlumbBob.sSingleton.mSelectedActor != null && PlumbBob.sSingleton.mSelectedActor.SimDescription != null && Actor.SimDescription.Household != PlumbBob.sSingleton.mSelectedActor.SimDescription.Household)
                    if (!NFinalizeDeath.IsSimFastActiveHousehold(Actor))
                    {

                        
                        try
                        {
                            throw new NiecModException("Wait Find XeginSocialInteraction");
                        }
                        catch (NiecModException e)
                        {
                            if (e.StackTrace.Contains("ReapSoul") && e.StackTrace.Contains("BeginSocialInteraction") || e.StackTrace.Contains("ReapPetSoul") && e.StackTrace.Contains("BeginSocialInteraction"))
                            {
                                if (this.SocialJig != null)
                                {
                                    this.SocialJig.Destroy();
                                    this.SocialJig = null;
                                }
                                base.Cleanup();
                                return;
                            }
                        }

                        if (sWasIsActiveHousehold)
                        {

                            if (activehouseholdint == 1)
                            {
                                Actor.SimDescription.ShowSocialsOnSim = true;
                                Actor.SimDescription.IsNeverSelectable = false;
                                if (this.SocialJig != null)
                                {
                                    this.SocialJig.Destroy();
                                    this.SocialJig = null;
                                }
                                base.Cleanup();
                                return;
                            }
                        }

                        
                        try
                        {
                            Actor.SimDescription.IsGhost = true;
                            if (simDeathType == SimDescription.DeathType.None)
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
                                if (!Actor.SimDescription.IsFrankenstein)
                                {
                                    list.Add(SimDescription.DeathType.Electrocution);
                                }
                                list.Add(SimDescription.DeathType.Burn);
                                if (Actor.SimDescription.Elder)
                                {
                                    list.Add(SimDescription.DeathType.OldAge);
                                }
                                if (Actor.SimDescription.IsWitch)
                                {
                                    list.Add(SimDescription.DeathType.HauntingCurse);
                                }
                                SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                                simDeathType = randomObjectFromList;

                            }
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch
                        { }

                        

                        if (!Actor.LotCurrent.IsWorldLot)
                        {
                            try
                            {
                                Actor.SimDescription.SetDeathStyle(this.simDeathType, true);
                                if (Create.GhostsGrave(Actor.SimDescription) == null)
                                {
                                    Urnstone mGrave = Urnstone.CreateGrave(Actor.SimDescription, false, false);
                                    if (mGrave != null)
                                    {
                                        mGrave.SetOpacity(0f, 0f);
                                        SimDescription.DeathType deathStylea = this.simDeathType;
                                        World.FindGoodLocationParams fglParams = (deathStylea == SimDescription.DeathType.Drown) ? new World.FindGoodLocationParams(Actor.Position) : new World.FindGoodLocationParams(Actor.Position);
                                        fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                        if (!GlobalFunctions.PlaceAtGoodLocation(mGrave, fglParams, false))
                                        {
                                            Simulator.Sleep(0u);
                                            fglParams.BooleanConstraints = FindGoodLocationBooleans.None;
                                            if (!GlobalFunctions.PlaceAtGoodLocation(mGrave, fglParams, false))
                                            {
                                                mGrave.SetPosition(Actor.Position);
                                            }
                                        }
                                        mGrave.OnHandToolMovement();
                                        mGrave.FadeIn(false, 10f);
                                        mGrave.FogEffectStart();
                                    }

                                }



                               
                                if (Create.GhostsGrave(Actor.SimDescription) == null)
                                {
                                    Urnstone mGravebackup = Urnstone.CreateGrave(Actor.SimDescription, false, false);
                                    if (mGravebackup != null)
                                    {
                                        if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(Actor.Position), false))
                                        {
                                            mGravebackup.SetPosition(Target.Position);
                                        }
                                        mGravebackup.OnHandToolMovement();
                                    }
                                }

                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch (NullReferenceException)
                            { }
                            try
                            {
                                if (Create.GhostsGrave(Actor.SimDescription) != null)
                                    Urnstone.FinalizeSimDeath(Actor.SimDescription, homehome != null ? homehome : Household.sNpcHousehold, !AssemblyCheckByNiec.IsInstalled("DGSCore"));
                            }
                            catch
                            { }
                        }
                        else
                        {
                            Urnstone mGravebackup = null;
                            try
                            {
                                //List<Sim> sims = new List<Sim>();
                                Sim simgrim = null;
                                //foreach (Sim sim in Actor.LotCurrent.GetAllActors())
                                foreach (Sim sim in LotManager.Actors)
                                {
                                    if (sim.Service != null && (sim.Service is GrimReaper))
                                    {
                                        simgrim = sim;
                                        break;
                                    }
                                }
                                //if (!PlumbBob.Singleton.mSelectedActor.LotCurrent.IsWorldLot)
                                if (Create.GhostsGrave(Actor.SimDescription) == null)
                                {
                                    mGravebackup = Urnstone.CreateGrave(Actor.SimDescription, false, false);
                                    if (mGravebackup != null)
                                    {
                                        /*
                                        if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position), false))
                                        {
                                            mGravebackup.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                                            mGravebackup.SetForward(PlumbBob.Singleton.mSelectedActor.Position);
                                            mGravebackup.OnHandToolMovement();
                                            mGravebackup.FadeIn(true, 5f);
                                            mGravebackup.FogEffectStart();
                                        }
                                         * */
                                        NiecMod.Nra.SpeedTrap.Sleep();
                                        if (simgrim != null && !simgrim.HasBeenDestroyed && !simgrim.LotCurrent.IsWorldLot)
                                        {
                                            if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(simgrim.Position), false))
                                            {
                                                mGravebackup.SetPosition(simgrim.Position);
                                            }
                                        }
                                        else if (PlumbBob.sSingleton != null && PlumbBob.sSingleton.mSelectedActor != null && !PlumbBob.sSingleton.mSelectedActor.LotCurrent.IsWorldLot)
                                        {
                                            if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(PlumbBob.sSingleton.mSelectedActor.Position), false))
                                            {
                                                mGravebackup.SetPosition(PlumbBob.sSingleton.mSelectedActor.Position);
                                            }
                                        }
                                        else
                                        {
                                            Lot lotm = mGravebackup.LotCurrent;
                                            if (lotm != null && !lotm.IsWorldLot)
                                            {
                                                if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(lotm.Position), false))
                                                {
                                                    mGravebackup.SetPosition(lotm.Position);
                                                }
                                            }
                                            
                                        }
                                        
                                        //mGravebackup.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                                        mGravebackup.OnHandToolMovement();
                                        mGravebackup.FadeIn(false, 5f);
                                        mGravebackup.FogEffectStart();
                                    }
                                }
                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch (NullReferenceException)
                            {
                                if (mGravebackup != null)
                                {
                                    mGravebackup.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                                    mGravebackup.OnHandToolMovement();
                                    mGravebackup.FadeIn(false, 5f);
                                    mGravebackup.FogEffectStart();
                                }
                            }
                        }


                        try
                        {
                            if (Actor.Household == null && homehome != null)
                            {
                                homehome.Add(Actor.SimDescription);
                            }
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (NullReferenceException)
                        { }

                        this.CancelDeath = false;
                        this.FixNotExit = false;
                        this.DeathTypeFix = false;
                        this.ActiveFix = false;
                        try
                        {
                            try
                            {
                                if (PlumbBob.Singleton != null && PlumbBob.Singleton.mSelectedActor != null && !PlumbBob.sSingleton.mSelectedActor.LotCurrent.IsWorldLot && PlumbBob.sSingleton.mSelectedActor.SimDescription.Child)
                                {
                                    foreach (Urnstone iteem in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                                    {
                                        //if (iteem.DeadSimsDescription == Actor.SimDescription)
                                        if (iteem != null && object.ReferenceEquals(iteem.DeadSimsDescription, Actor.SimDescription))
                                        {
                                            iteem.SetPosition(PlumbBob.Singleton.mSelectedActor.Position);
                                        }
                                    }
                                    
                                }
                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch
                            { }
                            //Sims3.NiecHelp.Tasks.NiecTask.Perform(CheckKillAllSimGraveToPos);
                            loadalarm = AlarmManager.Global.AddAlarm(5f, TimeUnit.Minutes, new AlarmTimerCallback(CheckKillAllSimGraveToPos), "CheckKillAllSimGraveToPos Name " + Actor.SimDescription.LastName, AlarmType.NeverPersisted, null);
                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Cancel and Exit ExtKillSimNiec! to Run Force Kill :) " + simDeathType.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                            
                            Actor.MoveInventoryItemsToAFamilyMember();
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (NullReferenceException)
                        { }


                       


                        /*
                        try
                        {
                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Cancel and Exit ExtKillSimNiec! to Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            try
                            {
                                Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                            }
                            catch (Exception ex3)
                            { NiecException.WriteLog("CreateGrave: " + NiecException.NewLine + NiecException.LogException(ex3), true, true, false); }
                        }
                        catch (Exception)
                        { }
                        */
                        // List Mod


                        try
                        {
                            if (NTunable.kDedugNiecModExceptionExtKillSimNiec)
                            {
                                throw new NiecModException("ExtKillSimNiec: Not Error");
                            }
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (NiecModException ex)
                        {
                            if (NTunable.kDedugNiecModExceptionExtKillSimNiec)
                            {
                                NiecException.WriteLog("ExtKillSimNiec: " + NiecException.NewLine + NiecException.LogException(ex), true, true, false);
                            }
                        }
                        try
                        {
                            // Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                            ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                            try
                            {
                                Actor.SimDescription.ShowSocialsOnSim = false;
                                Actor.SimDescription.IsNeverSelectable = true;
                                Actor.SimDescription.Contactable = false;
                                if (this.SocialJig != null)
                                {
                                    this.SocialJig.Destroy();
                                    this.SocialJig = null;
                                }
                                Actor.LotCurrent.LastDiedSim = Actor.SimDescription;

                                if (Create.GhostsGrave(Actor.SimDescription) == null)
                                    Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);

                                
                                

                                return;
                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch (NullReferenceException)
                            { }

                            //NFinalizeDeath.FinalizeSimDeathRelationships(Actor.SimDescription, 0);
                        }
                        catch (ResetException)
                        {
                            throw;
                        }
                        catch (Exception)
                        {
                            Actor.SimDescription.ShowSocialsOnSim = false;
                            Actor.SimDescription.IsNeverSelectable = true;
                            Actor.SimDescription.Contactable = false;
                            if (this.SocialJig != null)
                            {
                                this.SocialJig.Destroy();
                                this.SocialJig = null;
                            }

                            if (Create.GhostsGrave(Actor.SimDescription) == null)
                                Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);

                            return;
                        }
                            /*
                        finally
                        {
                            try
                            {
                                Actor.SimDescription.ShowSocialsOnSim = false;
                                Actor.SimDescription.IsNeverSelectable = true;
                                Actor.SimDescription.Contactable = false;
                                if (this.SocialJig != null)
                                {
                                    this.SocialJig.Destroy();
                                    this.SocialJig = null;
                                }
                                Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                            }
                            catch (Exception)
                            { }

                        }
                        // End
                             * */
                    }
                }
            }
            catch (ResetException)
            {
                /*
                try
                {
                    throw new NiecModException();
                }
                catch (NiecModException exeax)
                {
                    if (exeax.StackTrace != null)
                    {
                        NiecException.WriteLog("ResetException FixNotExit: " + NiecException.NewLine + NiecException.LogException(exeax), true, false, false);
                    }
                    NiecException.PrintMessage("FixNotExit:" + NiecException.NewLine + "ResetException is Found No: " + NiecException.sLogEnumerator);
                }
                */
                throw;
            }
            catch (Exception ex)
            { NiecException.WriteLog("FixNotExit: " + NiecException.NewLine + NiecException.LogException(ex), true, true, false); return; }
            
            this.Actor.RemoveInteractionByType(Sim.DeathReaction.Singleton);
            if (this.CancelDeath)
            {
                SimDescription.DeathType deathStyle = this.Actor.SimDescription.DeathStyle;
                SimDescription simDescription = this.Actor.SimDescription;
                simDescription.ShowSocialsOnSim = true;
                World.ObjectSetGhostState(this.Actor.ObjectId, 0u, (uint)simDescription.AgeGenderSpecies);
                simDescription.IsNeverSelectable = false;

                try
                {
                    if (this.ActiveFix)
                    {
                        //if (!Actor.IsInActiveHousehold)
                        if (!NFinalizeDeath.IsSimFastActiveHousehold(Actor))
                        {
                            //Actor.SimDescription.Contactable = false;
                            try
                            {
                                this.Actor.MoveInventoryItemsToAFamilyMember();
                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch (Exception)
                            { }

                            this.DeathTypeFix = false;

                            StyledNotification.Show(new StyledNotification.Format("ExtKillSimNiec: " + Actor.Name + " is Dont Run ExtKillSimNiec! to Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            // List Mod
                            try
                            {
                                if (NTunable.kDedugNiecModExceptionExtKillSimNiec)
                                {
                                    throw new NiecModException("ExtKillSimNiec: Not Error");
                                }
                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch (NiecModException ex)
                            {
                                if (NTunable.kDedugNiecModExceptionExtKillSimNiec)
                                {
                                    Common.Exception(Actor, ex);
                                    NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                                }
                            }

                            try
                            {
                                // Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                                ExtKillSimNiec.ListMorunExtKillSim(Actor, simDeathType);
                                NFinalizeDeath.FinalizeSimDeathRelationships(Actor.SimDescription, 0);
                            }
                            catch (ResetException)
                            {
                                throw;
                            }
                            catch (Exception)
                            {
                                Actor.SimDescription.ShowSocialsOnSim = false;
                                Actor.SimDescription.IsNeverSelectable = true;
                                Actor.SimDescription.Contactable = false;
                                if (this.SocialJig != null)
                                {
                                    this.SocialJig.Destroy();
                                    this.SocialJig = null;
                                }
                                Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                                return;
                            }
                            finally
                            {
                                Urnstones.CreateGrave(Actor.SimDescription, this.simDeathType, true, false);
                            }
                            // End
                        }
                    }
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception aas)
                { NiecException.WriteLog("FixNotExit: " + NiecException.NewLine + NiecException.LogException(aas), true, true, false); }
                
                if (this.DeathTypeFix)
                {
                    if (!simDescription.IsEP11Bot)
                    {
                        simDescription.AgingEnabled = true;
                        if (simDescription.DeathStyle == SimDescription.DeathType.OldAge)
                        {
                            simDescription.AgingState.ResetAndExtendAgingStage(0f);
                        }
                    }
                    simDescription.SetDeathStyle(SimDescription.DeathType.None, false);
                }
            }
            if (this.SocialJig != null)
            {
                this.SocialJig.Destroy();
                this.SocialJig = null;
            }
            base.Cleanup();
        }

        public static void CreateDeathReactionBroadcasterX(Sim corpse)
        {
            corpse.DeathReactionBroadcast = new ReactionBroadcaster(corpse, Urnstone.kDeathReactionBroadcasterParams, new ReactionBroadcaster.BroadcastCallback(Urnstone.DeathReactionCallback));
        }


        public void FailedCallBookSleep()
        {
            while (NiecMod.Nra.SpeedTrap.SleepBool(40))
            {
                Actor.SimRoutingComponent.DisableDynamicFootprint();
                Actor.SetPosition(Actor.Position);
                
                Actor.SimRoutingComponent.EnableDynamicFootprint();
            }
            return;
        }

        public SimDescription TestDesc = null;

        public void FailedCallBook()
        {
            TestDesc = Actor.SimDescription;
            Actor.FadeOut();
            Actor.Destroy();
            Urnstones.CreateGrave(TestDesc, simDeathType, false, false);

            //this.Cleanup();
            return;
        }

        public static void ListMorunExtKillSim(Sim actorlist, SimDescription.DeathType deathType)
        {
            try
            {

                try
                {
                    actorlist.SimDescription.IsGhost = true;
                    actorlist.SimDescription.SetDeathStyle(deathType, true);
                    //Urnstone.FinalizeSimDeath(actorlist.SimDescription, actorlist.Household);
                    //ReactionBroadcaster broadcaster = new ReactionBroadcaster(actorlist, Urnstone.kHeartBrokenParams, new ReactionBroadcaster.BroadcastCallback(Urnstone.UrnstoneHeartBrokenReaction));
                    /*
                    try
                    {

                        HelperNra helperNra = new HelperNra();

                        //helperNra = HelperNra;

                        helperNra.mSim = actorlist;


                        AlarmManager.Global.AddAlarm(30f, TimeUnit.Minutes, new AlarmTimerCallback(helperNra.CheckKillSimNotUrnstone), "MineKillCheckKillSim", AlarmType.NeverPersisted, null);
                    }
                    catch (NullReferenceException exception)
                    { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }
                     */
                }

                catch
                { }

                if (actorlist.SimDescription.Household != null)
                {
                    SimDescription simDescription = actorlist.SimDescription;
                    List<Sim> listx = new List<Sim>();
                    foreach (Sim sim in actorlist.Household.Sims)
                    {
                        if (sim != actorlist && !sim.IsSelectable && !(sim.Service is GrimReaper) && !sim.BuffManager.HasElement(BuffNames.Mourning) && !sim.BuffManager.HasElement(BuffNames.HeartBroken))
                        {
                            listx.Add(sim);
                        }
                    }
                    if (listx.Count > 0)
                    {
                        foreach (Sim nlist in listx)
                        {
                            nlist.EnableInteractions();
                            BuffMourning.BuffInstanceMourning buffInstanceMourning;
                            buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                            if (buffInstanceMourning == null)
                            {
                                nlist.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                            }
                            buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                            if (buffInstanceMourning != null)
                            {
                                buffInstanceMourning.MissedSim = actorlist.SimDescription;
                            }
                            foreach (Relationship relationship in Relationship.Get(actorlist.SimDescription))
                            {
                                if (actorlist.SimDescription.Partner == nlist.SimDescription)
                                {
                                    if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Marry))
                                    {
                                        relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Marry);
                                        relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.Divorce);
                                        WeddingAnniversary weddingAnniversary = relationship.WeddingAnniversary;
                                        if (weddingAnniversary != null)
                                        {
                                            AlarmManager.Global.RemoveAlarm(weddingAnniversary.AnniversaryAlarm);
                                            relationship.WeddingAnniversary = null;
                                        }
                                        SocialCallback.BreakUpDescriptionsShared(actorlist.SimDescription, nlist.SimDescription);
                                    }
                                    else if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.Propose))
                                    {
                                        relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.Propose);
                                        relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.BreakUp);
                                    }
                                    else if (relationship.LTR.HasInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment))
                                    {
                                        relationship.LTR.RemoveInteractionBit(LongTermRelationship.InteractionBits.MakeCommitment);
                                        relationship.LTR.AddInteractionBit(LongTermRelationship.InteractionBits.BreakUp);
                                    }
                                    actorlist.SimDescription.ClearPartner();
                                }
                            }
                        }
                    }
                    //return;
                }


                try
                {
                    List<Sim> listxe = new List<Sim>();
                    foreach (Sim sim in actorlist.LotCurrent.GetAllActors())
                    {
                        if (sim != actorlist && !sim.IsInActiveHousehold && !(sim.Service is GrimReaper) && !sim.BuffManager.HasElement(BuffNames.Mourning) && !sim.BuffManager.HasElement(BuffNames.HeartBroken))
                        {
                            listxe.Add(sim);
                        }
                    }
                    if (listxe.Count > 0)
                    {
                        foreach (Sim nlist in listxe)
                        {
                            BuffMourning.BuffInstanceMourning buffInstanceMourning;

                            buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                            if (buffInstanceMourning == null)
                            {
                                nlist.BuffManager.AddElement(BuffNames.Mourning, Origin.FromWitnessingDeath);
                            }
                            buffInstanceMourning = (nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning);
                            if (buffInstanceMourning != null)
                            {
                                buffInstanceMourning.MissedSim = actorlist.SimDescription;
                            }

                            BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken;
                            buffInstanceHeartBroken = (nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                            if (buffInstanceHeartBroken == null || buffInstanceHeartBroken.BuffOrigin != Origin.FromWitnessingDeath)
                            {
                                nlist.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                            }
                            else
                            {
                                nlist.BuffManager.AddElement(BuffNames.HeartBroken, Origin.FromWitnessingDeath);
                            }
                            buffInstanceHeartBroken = (nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken);
                            if (buffInstanceHeartBroken != null)
                            {
                                buffInstanceHeartBroken.MissedSim = actorlist.SimDescription;
                            }






                        }

                    }
                }
                catch (NullReferenceException)
                { }

                /*
                try
                {
                    List<Sim> listxe = new List<Sim>();
                    foreach (Sim sim in LotManager.Actors)
                    {
                        if (sim != actorlist && !sim.IsInActiveHousehold && !(sim.Service is GrimReaper))
                        {
                            listxe.Add(sim);
                        }
                    }
                    if (listxe.Count > 0)
                    {
                        foreach (Sim nlist in listxe)
                        {
                            
                            if (nlist.BuffManager.HasElement(BuffNames.HeartBroken))
                            {
                                BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken = nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken;
                                ThumbnailKey thumbnailKey = buffInstanceHeartBroken.MissedSim.GetThumbnailKey(ThumbnailSize.Large, 0);
                                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData(thumbnailKey, buffInstanceHeartBroken.ThumbString);
                                balloonData.mPriority = ThoughtBalloonPriority.High;
                                nlist.ThoughtBalloonManager.ShowBalloon(balloonData);
                                nlist.PlayReaction(ReactionTypes.HeartBroken, ReactionSpeed.AfterInteraction);
                            }
                            if (nlist.BuffManager.HasElement(BuffNames.Mourning))
                            {
                                BuffMourning.BuffInstanceMourning buffInstanceMourning = nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                                ThumbnailKey thumbnailKey2 = buffInstanceMourning.MissedSim.GetThumbnailKey(ThumbnailSize.Large, 0);
                                ThoughtBalloonManager.BalloonData balloonData2 = new ThoughtBalloonManager.DoubleBalloonData(thumbnailKey2, buffInstanceMourning.ThumbString);
                                balloonData2.mPriority = ThoughtBalloonPriority.High;
                                nlist.ThoughtBalloonManager.ShowBalloon(balloonData2);
                                nlist.PlayReaction(ReactionTypes.HeartBroken, ReactionSpeed.AfterInteraction);
                            }

                            

                            


                        }

                    }
                }
                catch (NiecModException)
                { }
                
                */

                Urnstone urnstone = FindGhostsGrave(actorlist.SimDescription);
                if (urnstone != null)
                {
                    urnstone.mHeartBrokenBroadcaster = new ReactionBroadcaster(urnstone, Urnstone.kHeartBrokenParams, UrnstoneHeartBrokenReaction);
                }


                //actorlist.LotCurrent.LastDiedSim = actorlist.SimDescription;

            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                if (!(ex is ResetException))
                {
                    Common.Exception(actorlist, ex);
                }

            }
        }


        public static void UrnstoneHeartBrokenReaction(Sim sim, ReactionBroadcaster broadcaster)
        {
            /*
            try
            {
                if (sim.IsInActiveHousehold) return;
            }
            catch
            { }
            */
            if (sim != null)
            {
                if (sim.BuffManager.HasElement(BuffNames.HeartBroken))
                {
                    BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken = sim.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken;
                    if (buffInstanceHeartBroken.MissedSim == (broadcaster.BroadcastingObject as Urnstone).DeadSimsDescription)
                    {
                        ThumbnailKey thumbnailKey = buffInstanceHeartBroken.MissedSim.GetThumbnailKey(ThumbnailSize.Large, 0);
                        ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData(thumbnailKey, buffInstanceHeartBroken.ThumbString);
                        balloonData.mPriority = ThoughtBalloonPriority.High;
                        sim.ThoughtBalloonManager.ShowBalloon(balloonData);
                        sim.PlayReaction(ReactionTypes.HeartBroken, ReactionSpeed.AfterInteraction);
                    }
                }
                if (sim.BuffManager.HasElement(BuffNames.Mourning))
                {
                    BuffMourning.BuffInstanceMourning buffInstanceMourning = sim.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                    if (buffInstanceMourning.MissedSim == (broadcaster.BroadcastingObject as Urnstone).DeadSimsDescription)
                    {
                        ThumbnailKey thumbnailKey2 = buffInstanceMourning.MissedSim.GetThumbnailKey(ThumbnailSize.Large, 0);
                        ThoughtBalloonManager.BalloonData balloonData2 = new ThoughtBalloonManager.DoubleBalloonData(thumbnailKey2, buffInstanceMourning.ThumbString);
                        balloonData2.mPriority = ThoughtBalloonPriority.High;
                        sim.ThoughtBalloonManager.ShowBalloon(balloonData2);
                        sim.PlayReaction(ReactionTypes.HeartBroken, ReactionSpeed.AfterInteraction);
                    }
                }
            }
            
            


            /*
            List<Sim> listxe = new List<Sim>();
            foreach (Sim sime in LotManager.Actors)
            {
                if (!sime.IsInActiveHousehold && !(sime.Service is GrimReaper))
                {
                    listxe.Add(sime);
                }
            }
            if (listxe.Count > 0)
            {
                foreach (Sim nlist in listxe)
                {
                    try
                    {
                        if (nlist.BuffManager.HasElement(BuffNames.HeartBroken))
                        {
                            BuffHeartBroken.BuffInstanceHeartBroken buffInstanceHeartBroken = nlist.BuffManager.GetElement(BuffNames.HeartBroken) as BuffHeartBroken.BuffInstanceHeartBroken;
                            if (buffInstanceHeartBroken.MissedSim == (broadcaster.BroadcastingObject as Urnstone).DeadSimsDescription)
                            {
                                ThumbnailKey thumbnailKey = buffInstanceHeartBroken.MissedSim.GetThumbnailKey(ThumbnailSize.Large, 0);
                                ThoughtBalloonManager.BalloonData balloonData = new ThoughtBalloonManager.DoubleBalloonData(thumbnailKey, buffInstanceHeartBroken.ThumbString);
                                balloonData.mPriority = ThoughtBalloonPriority.High;
                                nlist.ThoughtBalloonManager.ShowBalloon(balloonData);
                                nlist.PlayReaction(ReactionTypes.HeartBroken, ReactionSpeed.AfterInteraction);
                            }
                        }
                        if (nlist.BuffManager.HasElement(BuffNames.Mourning))
                        {
                            BuffMourning.BuffInstanceMourning buffInstanceMourning = nlist.BuffManager.GetElement(BuffNames.Mourning) as BuffMourning.BuffInstanceMourning;
                            if (buffInstanceMourning.MissedSim == (broadcaster.BroadcastingObject as Urnstone).DeadSimsDescription)
                            {
                                ThumbnailKey thumbnailKey2 = buffInstanceMourning.MissedSim.GetThumbnailKey(ThumbnailSize.Large, 0);
                                ThoughtBalloonManager.BalloonData balloonData2 = new ThoughtBalloonManager.DoubleBalloonData(thumbnailKey2, buffInstanceMourning.ThumbString);
                                balloonData2.mPriority = ThoughtBalloonPriority.High;
                                nlist.ThoughtBalloonManager.ShowBalloon(balloonData2);
                                nlist.PlayReaction(ReactionTypes.HeartBroken, ReactionSpeed.AfterInteraction);
                            }
                        }
                    }
                    catch
                    { }
                    
            



                }
            
            }
             * */
        }


        public static Urnstone FindGhostsGrave(SimDescription sim)
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


        public static void CheckCancelListInteractionByNiec(Sim sima)
        {
            if (sima == null) return;
            try
            {
                try
                {
                    if (sima.InteractionQueue.HasInteractionOfType(typeof(GoToSchoolInRabbitHole)) || sima.InteractionQueue.HasInteractionOfType(GoToSchoolInRabbitHole.Singleton)) sima.AddExitReason(ExitReason.StageComplete);

                    //if (sima.InteractionQueue.HasInteractionOfType(GoToSchoolInRabbitHole.Singleton)) sima.AddExitReason(ExitReason.StageComplete);
                }
                catch
                { }
                /*
                sima.AddExitReason(ExitReason.Canceled);
                sima.AddExitReason(ExitReason.StageComplete);
                sima.AddExitReason(ExitReason.Default);
                sima.AddExitReason(ExitReason.BuffFailureState);
                sima.AddExitReason(ExitReason.HigherPriorityNext);
                sima.AddExitReason(ExitReason.CanceledByScript);
                 * */
                foreach (InteractionInstance interactionInstance in sima.InteractionQueue.InteractionList)
                {
                    if (interactionInstance is GoToSchoolInRabbitHole || interactionInstance is ICountsAsWorking)
                    {
                        sima.AddExitReason(ExitReason.StageComplete);
                    }
                }
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
            }
            try
            {
                if (!sima.InteractionQueue.IsInteractionQueueEmpty())
                {

                    foreach (InteractionInstance interactionInstance in sima.InteractionQueue.InteractionList) // Cant Cancel Fix
                    {
                        interactionInstance.MustRun = false;
                        interactionInstance.CancellableByPlayer = true;
                        interactionInstance.Hidden = false;
                    }
                }
            }
            catch
            {
                try
                {
                    if (!sima.InteractionQueue.IsInteractionQueueEmpty())
                    {
                        foreach (InteractionInstance interactionInstance in sima.InteractionQueue.InteractionList) // Cant Cancel Fix
                        {
                            interactionInstance.MustRun = false;
                            interactionInstance.CancellableByPlayer = true;
                            interactionInstance.Hidden = false;
                        }
                    }
                }
                catch
                { }
            }
        }

        // Token: 0x0600A7E4 RID: 42980 RVA: 0x002FB840 File Offset: 0x002FA840
        private void EventCallbackCreateAshPile(StateMachineClient sender, IEvent evt)
        {
            this.mAshPile = (GlobalFunctions.CreateObjectOutOfWorld("AshPile") as AshPile);
            if (this.Actor.SimDescription.IsMummy || this.Actor.SimDescription.DeathStyle == SimDescription.DeathType.MummyCurse)
            {
                this.mAshPile.SetMaterial("mummy");
            }
            this.mAshPile.AddToWorld();
            this.mAshPile.SetPosition(this.Actor.Position);
            this.mAshPile.SetTooltipText(this.Actor.SimDescription.FirstName + " " + this.Actor.SimDescription.LastName);
            LotLocation invalid = LotLocation.Invalid;
            World.GetLotLocation(this.mAshPile.Position, ref invalid);
            FireManager.BurnTile(this.mAshPile.LotCurrent.LotId, invalid);
            SimDescription.DeathType deathStyle = this.Actor.SimDescription.DeathStyle;
            float fadeTime = (deathStyle == SimDescription.DeathType.MummyCurse || deathStyle == SimDescription.DeathType.Thirst) ? 1.2f : GameObject.kGlobalObjectFadeTime;
            this.Actor.FadeOut(false, false, fadeTime);
        }

        // Token: 0x0600A7E5 RID: 42981 RVA: 0x002FB957 File Offset: 0x002FA957
        private void EventCallbackHideSim(StateMachineClient sender, IEvent evt)
        {
            this.Actor.SetHiddenFlags((HiddenFlags)4294967295u);
        }

        // Token: 0x0600A7E6 RID: 42982 RVA: 0x002FB968 File Offset: 0x002FA968
        private void EventCallbackTurnToGhost(StateMachineClient sender, IEvent evt)
        {
            if (!this.Actor.SimDescription.IsEP11Bot)
            {
                uint deathStyle = (uint)this.Actor.SimDescription.DeathStyle;
                World.ObjectSetGhostState(this.Actor.ObjectId, deathStyle, (uint)this.Actor.SimDescription.AgeGenderSpecies);
                return;
            }
            World.ObjectSetGhostState(this.Actor.ObjectId, 23u, (uint)this.Actor.SimDescription.AgeGenderSpecies);
        }

        // Token: 0x0600A7E7 RID: 42983 RVA: 0x002FB9DC File Offset: 0x002FA9DC
        private void EventCallbackFadeOutSim(StateMachineClient sender, IEvent evt)
        {
            this.Actor.FadeOut();
        }

        public void StartDeathEffect()
        {
            try
            {
                if (this.mDeathEffect == null)
                {
                    this.mDeathEffect = VisualEffect.Create("death");
                    this.mDeathEffect.ParentTo(this.Target, Sim.FXJoints.Spine1);
                    this.mDeathEffect.Start();
                }
            }
            catch (Exception exception)
            {
                NiecException.PrintMessage("StartDeathEffect" + exception.Message + NiecException.NewLine + exception.StackTrace);
            }
        }

        public void StopDeathEffect()
        {
            if (this.mDeathEffect != null)
            {
                this.mDeathEffect.Stop();
                this.mDeathEffect.Dispose();
                this.mDeathEffect = null;
            }
        }

        // Token: 0x04005BED RID: 23533
        public static readonly InteractionDefinition Singleton = new Definition();

        public static readonly InteractionDefinition NiecDefinitionDeathInteractionSocialSingleton = new SocialInteractionB.DefinitionDeathInteraction();

        // Token: 0x04005BEE RID: 23534
        private AshPile mAshPile;

        // Token: 0x04005BEF RID: 23535
        public bool CancelDeath = true;


        public bool sWasIsActiveHousehold = true;








        private bool ActiveFix = true;

        private bool DeathTypeFix = true;

        private bool FixNotExit = true;


        public bool mForceKillGrim = false;

        // Token: 0x04005BF0 RID: 23536
        public SocialJig SocialJig;

        private VisualEffect mDeathEffect;

        // Token: 0x04005BF1 RID: 23537
        public SimDescription.DeathType simDeathType = SimDescription.DeathType.None;

        // Token: 0x04005BF2 RID: 23538
        public bool PlayDeathAnimation = true;


        [DoesntRequireTuning]

        private sealed class Definition : InteractionDefinition<Sim, Sim, ExtKillSimNiec>, IDontNeedToBeCheckedInResort, IIgnoreIsAllowedInRoomCheck, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            // Token: 0x0600A7EA RID: 42986 RVA: 0x002FBA0B File Offset: 0x002FAA0B
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair iop)
            {
                try
                {
                    return Localization.LocalizeString("Gameplay/Actors/Sim/ReapSoul:InteractionName", new object[0]);
                }
                catch (Exception)
                {
                    return "Exipre";
                }
            }

            /*
            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (parameters.Autonomous)
                    {
                        return InteractionTestResult.GenericFail;
                    }
                    return InteractionTestResult.Pass;
                }
                catch (Exception)
                { return InteractionTestResult.Pass; }
            }
            */




            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (!Test(parameters.Actor as Sim, parameters.Target as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                    {
                        return InteractionTestResult.Def_TestFailed;
                    }
                }
                catch (Exception)
                { }
                
                return InteractionTestResult.Pass;
            }

            // Token: 0x0600A7EB RID: 42987 RVA: 0x002FBA23 File Offset: 0x002FAA23
            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    //if (actor.Service is GrimReaper && !mForceKillGrim) return false;
                    //if (actor.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton)) return false;
                    if (isAutonomous) return false;
                    return true;
                }
                catch (Exception)
                {
                    return true;
                }
            }
            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
            public override string[] GetPath(bool isFemale)
            {
                return new string[] { "Test myMod" };
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                InteractionInstance interactionInstance = base.CreateInstance(ref parameters);
                interactionInstance.MustRun = true;
                return interactionInstance;
            }

        }
    }
}
