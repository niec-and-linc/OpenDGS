using Sims3.Gameplay;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.ActorSystems.Children;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Careers;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay.PetSystems;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.Utilities;
using Sims3.SimIFace;
using Sims3.SimIFace.CAS;
using Sims3.UI;
using Sims3.UI.CAS;
using Sims3.UI.Dialogs;
using Sims3.UI.Hud;
using System;
using System.Collections.Generic;
using NiecMod.Nra;
using NiecMod.Interactions;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Abstracts;
using NiecMod.KillNiec;
using Sims3.Gameplay.Controllers.Niec;
using Sims3.Gameplay.Passport;
using NRaas.CommonSpace.Helpers;
using NiecMod.Interactions.Hidden;
using NiecS3Mod;
using System.Text;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Scuba;

namespace Niec.iCommonSpace
{
    public class KillPro
    {


        public static bool NonEACanBeKilledPro(Sim targetSim, out string textReason)
        {
            textReason = " None";
            if (targetSim == null)
            {
                textReason = " Sim Is Null";
                return false;
            }
            if (targetSim.SimDescription == null)
            {
                textReason = " Sim doesn't have a description!";
                return false;
            }
            try
            {
                if (targetSim.SimDescription.IsGhost && !targetSim.SimDescription.IsPlayableGhost)
                {
                    textReason = " Sim Is Ghost";
                    return false;
                }
                if (targetSim.SimDescription.IsPlayableGhost)
                {
                    textReason = " Sim Is Playable Ghost";
                    return false;
                }
                if (targetSim.SimDescription.IsDead)
                {
                    textReason = " Sim Is Dead";
                    return false;
                }
                if (targetSim.mInteractionQueue == null)
                {
                    textReason = " Sim don't have a InteractionQueue";
                    return false;
                }



                


                if (targetSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                {
                    textReason = " Sim Is ExtKillSimNiec Found A";
                    return false;
                }

                Type type = ExtKillSimNiec.Singleton.GetType();
                foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.mInteractionList)
                {
                    try
                    {
                        if (interactionInstance.InteractionDefinition.GetType() == type)
                        {
                            textReason = " Sim Is ExtKillSimNiec Found B";
                            return false;
                        }
                    }
                    catch
                    { }

                }

                if (targetSim.InteractionQueue.HasInteractionOfType(typeof(ExtKillSimNiec)))
                {
                    textReason = " Sim Is ExtKillSimNiec Found C";
                    return false;
                }

                if (targetSim.InteractionQueue.HasInteractionOfTypeAndTarget(ExtKillSimNiec.Singleton, targetSim))
                {
                    textReason = " Sim Is ExtKillSimNiec Found D";
                    return false;
                }

                foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.InteractionList)
                {
                    if (interactionInstance is ExtKillSimNiec)
                    {
                        textReason = " Sim Is ExtKillSimNiec Found E";
                        return false;
                    }
                }




                if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                {

                    foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.InteractionList)
                    {
                        if (interactionInstance is Urnstone.KillSim)
                        {
                            textReason = " Sim Is UrnstoneKillSim Found A";
                            return false;
                        }
                    }
                    if (targetSim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton))
                    {
                        textReason = " Sim Is  UrnstoneKillSim Found B";
                        return false;
                    }
                }


                if (targetSim.Service != null && targetSim.Service is GrimReaper)
                {
                    textReason = " Sim Is Grim Reaper";
                    return false;
                }
            }
            catch (Exception ex)
            {
                //textReason = " Error: " + ex.GetType().FullName + ": " + ex.Message;
                textReason = " Error: " + NL + ex.ToString(); // + ex.GetType().FullName + ": " + ex.Message;
                return false;
            }


            return true;
        }

        public static bool EACanBeKilledPro(Sim targetSim, out string textReason)
        {
            textReason = " None";
            try
            {

                OccultImaginaryFriend occultImaginaryFriend = null;
                if (OccultImaginaryFriend.TryGetOccultFromSim(targetSim, out occultImaginaryFriend) && !occultImaginaryFriend.IsReal)
                {
                    textReason = " Sim Is Occult Imaginary Friend";
                    return false;
                }
                if (targetSim.SimDescription.AssignedRole is NPCAnimal)
                {
                    textReason = " Assigned Role NPC Animal";
                    return false;
                }
                if (targetSim.OccultManager.HasOccultType(OccultTypes.TimeTraveler))
                {
                    textReason = " Sim is TimeTraveler";
                    return false;
                }
                if (HolographicProjectionSituation.IsSimHolographicallyProjected(targetSim))
                {
                    textReason = " Sim is HolographicProjectionSituation";
                    return false;
                }

                if (targetSim.InteractionQueue == null)
                {
                    textReason = " Sim don't have a InteractionQueue";
                    return false;
                }
                if (targetSim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton))
                {
                    textReason = " Kill Sim Interaction Found";
                    return false;
                }
                if (targetSim.SimDescription.IsPregnant )
                {
                    textReason = " Sim Is Pregnant";
                    return false;
                }
                if (targetSim.SimDescription.IsGhost)
                {
                    textReason = " Sim Is Ghost";
                    return false;
                }
                if ((ServiceSituation.IsSimOnJob(targetSim) && !targetSim.SimDescription.CanBeKilledOnJob))
                {
                    textReason = " SimOnJob And Not CanBeKilledOnJob";
                    return false;
                }
                if (targetSim.IsNPC)
                {
                    if (!targetSim.LotCurrent.IsWorldLot)
                    {
                        foreach (Sim sim in targetSim.LotCurrent.GetSims())
                        {
                            if (sim.IsSelectable)
                            {
                                //textReason = " Target Is NPC : Sim Is Selectable";
                                return true;
                            }
                        }
                    }
                    textReason = " SimNPC In LotCurrent Sim Is Selectable Not Found";
                    return false;
                }
                if (targetSim.LotCurrent.IsDivingLot && !(targetSim.Posture is ScubaDiving))
                {
                    textReason = " Sim Is DivingLot Found : ScubaDiving Not Found";
                    return false;
                }
                if (targetSim.LotCurrent.IsWorldLot)
                {
                    textReason = " Sim Is WorldLot Failed";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                textReason = " Error: " + NL + ex.ToString(); // + ex.GetType().FullName + ": " + ex.Message;

                /*
                if (!AcceptCancelDialog.Show("EA CanBeKilled By Niec: Name (" + targetSim.Name + ") Found Error (" + ex.Message + ") Do want you run? [Yes Run or No Cancel]", true))
                {
                    return false;
                }
                */
            }
            return false;
        }








        [Tunable]
        public static bool sLoaderLogTraneEx = true;
        [Persistable(false)]
        public static bool sForceError = false;

        public static string sIForceError = null;


        public static int sLogEnumeratorTrane = 0;

        public static StringBuilder LogTraneEx = new StringBuilder();
         


        private static SimDescription.DeathType GetDeathType(Sim target)
        {
            try
            {
                List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();

                if (target.BuffManager.HasElement(BuffNames.Drowning))
                {
                    listr.Add(SimDescription.DeathType.Drown);

                    if (GameUtils.IsInstalled(ProductVersion.EP10))
                    {
                        listr.Add(SimDescription.DeathType.ScubaDrown);
                        listr.Add(SimDescription.DeathType.Shark);
                    }
                }
                else if (target.BuffManager.HasElement(BuffNames.OnFire))
                {
                    listr.Add(SimDescription.DeathType.Burn);

                    if (GameUtils.IsInstalled(ProductVersion.EP2))
                        listr.Add(SimDescription.DeathType.Meteor);
                }
                else if (target.CurrentOutfitCategory == OutfitCategories.Singed || target.BuffManager.HasElement(BuffNames.SingedElectricity))
                {
                    listr.Add(SimDescription.DeathType.Electrocution);
                }
                else if (target.SimDescription.Elder)
                {
                    if (!target.IsPet)
                        listr.Add(SimDescription.DeathType.OldAge);

                    else
                    {
                        listr.Add(SimDescription.DeathType.PetOldAgeGood);
                        listr.Add(SimDescription.DeathType.PetOldAgeBad);
                    }

                }
                else
                {
                    listr.Add(SimDescription.DeathType.Starve);
                    listr.Add(SimDescription.DeathType.Burn);

                    if (GameUtils.IsInstalled(ProductVersion.EP10) && target.Motives.IsSleepy())
                        listr.Add(SimDescription.DeathType.MermaidDehydrated);
                }
                return Sims3.Gameplay.Core.RandomUtil.GetRandomObjectFromList(listr);
            }
            catch
            {
                return SimDescription.DeathType.Starve;
            }
        }



        /*
        public static void FixIsCrib(Sim target)
        {

        }
        */


        public static readonly string NL = System.Environment.NewLine;


        public static bool MineKill(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes, bool wasisCrib)
        {
            if (target == null) { StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Note: " + NL + "target is Cannot be null.", StyledNotification.NotificationStyle.kGameMessagePositive)); return false; }

            if (deathType == SimDescription.DeathType.None) deathType = GetDeathType(target);








            bool asscheckDGSCore = AssemblyCheckByNiec.IsInstalled("DGSCore");

            bool checkkillsimcrach = false;

            bool canBeKilled = NFinalizeDeath.IsCanBeKilled(target); //CanBeKilledInternal(target);


            try
            {
                if (NTunable.kEACanBeKilledExByNiecNotification && asscheckDGSCore)
                {
                    if (KillSimNiecX.EACanBeKilledExByNiec(target))
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill: Found, " + target.Name + " Check EA CanBeKilled is OK", StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                    else 
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill: Found, " + target.Name + " Check EA CanBeKilled is Failed", StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                }
            }
            catch
            { }






            try
            {
                if (GameStates.IsGameShuttingDown) return true;
                if (Sims3.UI.Responder.Instance.IsGameStateShuttingDown) return true;

                try
                {
                    if (!asscheckDGSCore && !target.IsInActiveHousehold && deathType == SimDescription.DeathType.Ranting && target.SimDescription.IsHuman)
                    {
                        deathType = SimDescription.DeathType.Electrocution;
                    }
                }
                catch
                { }


                if (deathType == SimDescription.DeathType.PetOldAgeBad || deathType == SimDescription.DeathType.PetOldAgeGood)
                {
                    if (target.SimDescription.IsHuman)
                    {
                        switch (deathType)
                        {
                            case SimDescription.DeathType.PetOldAgeBad:
                            case SimDescription.DeathType.PetOldAgeGood:
                                deathType = SimDescription.DeathType.OldAge;
                                break;
                        }
                    }
                }
            }
            catch
            { }

            



            try
            {
                // Fix Crach Game Why?
                // sim is Destroyed

                if (!target.InWorld)
                {
                    target.AddToWorld();
                }

                Sim simes = target.SimDescription.CreatedSim;
                if (simes == null || target.HasBeenDestroyed)
                {

                    

                    Vector3 pos = Vector3.OutOfWorld;
                    checkkillsimcrach = true;
                    ResourceKey outfitKey = target.SimDescription.mDefaultOutfitKey;
                    try
                    {
                        if ((PlumbBob.Singleton != null && PlumbBob.Singleton.mSelectedActor != null))
                        {
                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position);
                            fglParams.InitialSearchDirection = RandomUtil.GetInt(0, 7);
                            Vector3 vector;
                            if (!GlobalFunctions.FindGoodLocation(PlumbBob.Singleton.mSelectedActor, fglParams, out pos, out vector))
                            {
                                pos = PlumbBob.Singleton.mSelectedActor.Position;
                            }
                        }
                    }
                    catch
                    { }
                    Sim fixsim = target.SimDescription.Instantiate(pos, outfitKey, false, true);
                    if (fixsim == null)
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Failed Instantiate In Fix Crash Game", StyledNotification.NotificationStyle.kGameMessagePositive));
                        return false;
                    }
                    target = fixsim;
                    NiecMod.Nra.SpeedTrap.Sleep();
                }
            }
            catch
            { }


            try
            {
                if (!asscheckDGSCore && target.IsPet)
                {
                    bool sata = target.IsInActiveHousehold;
                    if (target.InteractionQueue == null)
                    {
                        if (target.SimDescription.IsPregnant)
                        {
                            target.SimDescription.Pregnancy.ClearPregnancyData();
                            if (target.SimDescription.Pregnancy == null)
                            {
                                StyledNotification.Show(new StyledNotification.Format(target.Name + "OK Clear Pregnancy Data", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                            else
                            {
                                StyledNotification.Show(new StyledNotification.Format("Failed Clear Pregnancy Data", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                        }
                        try
                        {
                            StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note:" + NL + " Wait Move Inventory Items To A Family Member", StyledNotification.NotificationStyle.kGameMessagePositive));
                            target.MoveInventoryItemsToAFamilyMember();
                        }
                        catch
                        { }
                        Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                        return true;
                    }
                    if (!canBeKilled) return false;
                    if (target.SimDescription.IsPregnant && !sata)
                    {
                        target.SimDescription.Pregnancy.ClearPregnancyData();
                        if (target.SimDescription.Pregnancy == null)
                        {
                            StyledNotification.Show(new StyledNotification.Format(target.Name + "OK Clear Pregnancy Data", StyledNotification.NotificationStyle.kGameMessagePositive));
                        }
                        else
                        {
                            StyledNotification.Show(new StyledNotification.Format("Failed Clear Pregnancy Data", StyledNotification.NotificationStyle.kGameMessagePositive));
                        }
                    }
                    if (deathType == SimDescription.DeathType.Electrocution && obj != null)
                    {
                        PetStartleBehavior.CheckForStartle(target, StartleType.Electrocution);
                    }
                    if (Passport.IsPassportSim(target.SimDescription))
                    {
                        if (!Passport.Instance.IsHostedSim(target.SimDescription) && target.SimDescription.mSenderNucleusID == SocialFeatures.Accounts.GetID())
                        {
                            SocialFeatures.Passport.CancelPassport(target.SimDescription.mStoredSlot);
                            Passport.HouseholdSimDied(target.SimDescription);
                        }
                    }
                    else
                    {
                        Passport.HouseholdSimDied(target.SimDescription);
                    }
                    if (target.SimDescription.mReturnSimAlarm != AlarmHandle.kInvalidHandle)
                    {
                        AlarmManager.Global.RemoveAlarm(target.SimDescription.mReturnSimAlarm);
                        target.SimDescription.mReturnSimAlarm = AlarmHandle.kInvalidHandle;
                    }
                    if (sata)
                    {
                        var killSim = Urnstone.KillSim.Singleton.CreateInstance(target, target, new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f), false, true) as Urnstone.KillSim;
                        killSim.simDeathType = deathType;
                        killSim.PlayDeathAnimation = playDeathAnim;
                        target.InteractionQueue.Add(killSim);
                    }
                    else
                    {

                        try
                        {

                            HelperNra helperNra = new HelperNra();

                             

                            helperNra.mSim = target;

                            helperNra.mSimdesc = target.SimDescription;

                            helperNra.mdeathtype = deathType;

                            AlarmManager.Global.AddAlarm(1f, TimeUnit.Days, new AlarmTimerCallback(helperNra.CheckKillSimNotUrnstonePro), "MineKillCheckKillSim Name " + target.SimDescription.LastName, AlarmType.AlwaysPersisted, null);



                        }
                        catch (Exception exception)
                        { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }

                        var killSim = Urnstone.KillSim.Singleton.CreateInstance(target, target, new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f), false, false) as Urnstone.KillSim;
                        killSim.simDeathType = deathType;
                        killSim.PlayDeathAnimation = playDeathAnim;
                        target.InteractionQueue.Add(killSim);
                    }

                    if (GameUtils.IsInstalled(ProductVersion.EP10))
                    {
                        foreach (object objx in LotManager.AllLots)
                        {
                            Lot lot = (Lot)objx;
                            if (lot.CommercialLotSubType == CommercialLotSubType.kEP10_Resort)
                            {
                                lot.ResortManager.CheckOutSim(target);
                            }
                        }
                    }
                    return true;
                }
            }
            catch
            { }


            
            // Start
            try
            {
                if (!canBeKilled)
                {
                    if (deathType == SimDescription.DeathType.OldAge && !target.SimDescription.IsEP11Bot && target.SimDescription.Elder)
                    {
                        target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                    }

                    target.FadeIn();

                    if (NTunable.kDedugNotificationCheckNiecKill)
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "CanBeKilled is False Sorry,", StyledNotification.NotificationStyle.kGameMessagePositive));
                    }

                    if (checkkillsimcrach)
                    {
                        Sim.MakeSimGoHome(target, true);
                    }
                    return false;
                }
                

                else
                {


                    // DGS: ... 
                    // Too Many Add Interaction Toddler InCrib Fix

                    try
                    {



                        try
                        {
                            if (NTunable.kDedugNiecModExceptionMineKill)
                            {
                                throw new NiecModException("MineKill: Not Error");
                            }
                        }
                        catch (NiecModException eex)
                        {
                            if (NTunable.kDedugNiecModExceptionMineKill)
                            {
                                NiecException.PrintMessage(eex.Message + NiecException.NewLine + eex.StackTrace);
                                NiecException.WriteLog("Dedug Exception MineKill" + NiecException.NewLine + NiecException.LogException(eex), true, true, false);
                            }
                        }





                        if (wasisCrib || (target.SimDescription.Age == CASAgeGenderFlags.Toddler || target.SimDescription.Age == CASAgeGenderFlags.Baby))
                        {
                            Sim asdta = null;
                            ulong newidsimdsc = 0L;
                            SimDescription simdescfixllop = null;
                            ICrib crib = target.Posture.Container as ICrib;
                            if (wasisCrib || target.Posture.Satisfies(CommodityKind.InCrib, null) || crib != null)
                            {



                                newidsimdsc = target.SimDescription.SimDescriptionId;
                                try
                                {
                                    NFinalizeDeath.ForceCancelAllInteractionsPro(target);
                                }
                                catch
                                { }
                                try
                                {
                                    target.SetObjectToReset();
                                }
                                catch (Exception errer)
                                {
                                    if (errer is ResetException)
                                    {
                                        try
                                        {
                                            throw new NiecModException("Only assignment, call, increment, decrement, and new object expressions can be used as a statement.", errer);
                                        }
                                        catch (NiecModException)
                                        {
                                            StyledNotification.Show(new StyledNotification.Format("MineKill ResetException" + NL + "Note: " + NL + "SetObjectToReset", StyledNotification.NotificationStyle.kGameMessagePositive));
                                            NiecException.WriteLog("MineKill ResetException In SetObjectToReset" + NiecException.NewLine + NiecException.LogException(errer), true, true, false);
                                        }
                                        
                                    }
                                    else
                                    {
                                        StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Note: " + NL + "Failed SetObjectToReset", StyledNotification.NotificationStyle.kGameMessagePositive));
                                        NiecException.WriteLog("MineKill In SetObjectToReset" + NiecException.NewLine + NiecException.LogException(errer), true, true, false);
                                    }
                                }

                                if (Simulator.CheckYieldingContext(false)) { NiecMod.Nra.SpeedTrap.Sleep(); }

                                simdescfixllop = SimDescription.Find(newidsimdsc);

                                if (simdescfixllop == null)
                                {
                                    foreach (SimDescription iatem in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                    {
                                        if (iatem == null) continue;
                                        if (iatem.SimDescriptionId == newidsimdsc)
                                        {
                                            simdescfixllop = iatem;
                                            break;
                                        }
                                    }
                                }

                                if (Simulator.CheckYieldingContext(false)) { NiecMod.Nra.SpeedTrap.Sleep(); }

                                if (simdescfixllop == null)
                                {
                                    
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Failed simdescfixllop is null", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    return false;
                                }
                                else
                                {
                                    if (simdescfixllop.CreatedSim ==  null)
                                    {
                                        Vector3 vector;
                                        Vector3 pos = Vector3.OutOfWorld;
                                        ResourceKey outfitKey = simdescfixllop.mDefaultOutfitKey;
                                        try
                                        {
                                            if ((PlumbBob.Singleton != null && PlumbBob.Singleton.mSelectedActor != null))
                                            {
                                                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position);
                                                fglParams.InitialSearchDirection = RandomUtil.GetInt(0, 7);

                                                if (!GlobalFunctions.FindGoodLocation(simdescfixllop.LotHome, fglParams, out pos, out vector))
                                                {
                                                    pos = PlumbBob.Singleton.mSelectedActor.Position;
                                                }
                                            }
                                            else if (simdescfixllop.LotHome != null)
                                            {
                                                World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(simdescfixllop.LotHome.Position);
                                                fglParams.InitialSearchDirection = RandomUtil.GetInt(0, 7);

                                                if (!GlobalFunctions.FindGoodLocation(simdescfixllop.LotHome, fglParams, out pos, out vector))
                                                {
                                                    pos = simdescfixllop.LotHome.Position;
                                                }
                                            }
                                        }
                                        catch
                                        { }
                                        asdta = simdescfixllop.Instantiate(pos, outfitKey, false, false);
                                        if (asdta == null)
                                        {
                                            StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + "???" + NL + "Note: " + NL + "Failed Instantiate InCrib", StyledNotification.NotificationStyle.kGameMessagePositive));
                                            return false;
                                        }
                                        if (Simulator.CheckYieldingContext(false)) { NiecMod.Nra.SpeedTrap.Sleep(); }
                                    }
                                    if (simdescfixllop.CreatedSim != null && !simdescfixllop.CreatedSim.HasBeenDestroyed)
                                    {
                                        target = simdescfixllop.CreatedSim;
                                    }
                                    else if (asdta != null)
                                    {
                                        target = asdta;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception errer)
                    {
                        NiecException.WriteLog("MineKill In Crib" + NiecException.NewLine + NiecException.LogException(errer), true, true, false);
                    }


                    // Start Add Interaction
                    try
                    {
                        // Active Household
                        if (target.IsInActiveHousehold)
                        {
                            try
                            {
                                ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                            }
                            catch
                            {
                                try
                                {
                                    ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                                }
                                catch
                                { }
                            }

                            try
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "DeathType: " + deathType.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                            catch
                            { }


                            ExtKillSimNiec killSim = ExtKillSimNiec.Singleton.CreateInstance(target, target, KillSimNiecX.DGSAndNonDGSPriority(), false, true) as ExtKillSimNiec;
                            killSim.simDeathType = deathType;
                            killSim.PlayDeathAnimation = playDeathAnim;
                            killSim.MustRun = false;

                            try
                            {
                                killSim.mForceKillGrim = (target.Service is GrimReaper);
                            }
                            catch
                            { }

                            try
                            {

                                if (target.Household != null)
                                {
                                    killSim.homehome = target.Household;
                                }

                            }
                            catch
                            { }


                            //killSim.sWasIsActiveHousehold = (NFinalizeDeath.IsSimFastActiveHousehold(target));
                            killSim.sWasIsActiveHousehold = (NFinalizeDeath.IsSimFastActiveHousehold(target) || target.IsInActiveHousehold);
                            target.InteractionQueue.AddNext(killSim);


                            try
                            {
                                if (!target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                                {
                                    target.InteractionQueue.Add(killSim);
                                }
                            }
                            catch
                            { }

                            return true;
                        }
                        // Non Active Household
                        else
                        {
                            
                            try
                            {
                                if (target.SimDescription.CreatedSim == null || target.HasBeenDestroyed)
                                {
                                    ResourceKey outfitKey = target.SimDescription.mDefaultOutfitKey;


                                    target.SimDescription.Instantiate(Vector3.OutOfWorld, outfitKey, false, true);
                                }
                            }
                            catch (Exception trtyr)
                            { NiecException.WriteLog("Instantiate " + NiecException.NewLine + NiecException.LogException(trtyr), true, true, false); }


                            try
                            {
                                target.EnableInteractions();
                            }
                            catch (Exception)
                            { }

                            /////////////////////////////////////////////////////

                            bool CheckAntiCancel = false;

                            if (AssemblyCheckByNiec.IsInstalled("NiecS3Mod"))
                            {
                                try
                                {
                                    if (target.InteractionQueue.HasInteractionOfType(PauseNiecIns.Singleton))
                                    {
                                        CheckAntiCancel = true;
                                    }
                                    if (!CheckAntiCancel)
                                    {
                                        foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                                        {
                                            if (interactionInstance is PauseNiecIns)
                                            {
                                                CheckAntiCancel = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch
                                { }

                            }

                            if (!CheckAntiCancel)
                            {
                                try
                                {
                                    if (target.InteractionQueue.HasInteractionOfType(AllPauseNiecDone.Singleton))
                                    {
                                        CheckAntiCancel = true;
                                    }
                                    if (!CheckAntiCancel)
                                    {
                                        foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                                        {
                                            if (interactionInstance is AllPauseNiecDone)
                                            {
                                                CheckAntiCancel = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch 
                                { }
                            }


                            ///////////////////////////////////////////////////

                            if (CheckAntiCancel)
                            {
                                try
                                {
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "DeathType: " + deathType.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                                catch
                                { }

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
                                
                                try
                                {
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note:" + NL + "Wait Move Inventory Items To A Family Member", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    target.MoveInventoryItemsToAFamilyMember();
                                }
                                catch
                                { }




                                try
                                {
                                    target.SimDescription.IsGhost = true;
                                    target.SimDescription.SetDeathStyle(deathType, false);
                                    ExtKillSimNiec.ListMorunExtKillSim(target, deathType);
                                    NFinalizeDeath.FinalizeSimDeathRelationships(target.SimDescription, 0);

                                }
                                catch
                                {
                                    Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Found CheckAntiCancel Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                                finally
                                {
                                    Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Found CheckAntiCancel Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                                return true;
                            }

                            


                            try
                            {
                                if (target.SimDescription.mReturnSimAlarm != AlarmHandle.kInvalidHandle)
                                {
                                    AlarmManager.Global.RemoveAlarm(target.SimDescription.mReturnSimAlarm);
                                    target.SimDescription.mReturnSimAlarm = AlarmHandle.kInvalidHandle;
                                }
                            }
                            catch (Exception passportaaata)
                            { NiecException.WriteLog("MineKill AlarmManager: " + NiecException.NewLine + NiecException.LogException(passportaaata), true, false, false); }

                            if (!(NFinalizeDeath.IsSimFastActiveHousehold(target) || target.IsInActiveHousehold))
                            {
                                try
                                {

                                    HelperNra helperNra = new HelperNra();



                                    helperNra.mSim = target;

                                    helperNra.mSimdesc = target.SimDescription;

                                    helperNra.mdeathtype = deathType;

                                    AlarmManager.Global.AddAlarm(1f, TimeUnit.Days, new AlarmTimerCallback(helperNra.CheckKillSimNotUrnstonePro), "MineKillCheckKillSim Name " + target.SimDescription.LastName, AlarmType.AlwaysPersisted, null);



                                }
                                catch (Exception exception)
                                { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }

                            }

                            try
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "DeathType: " + deathType.ToString(), StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                            catch
                            { }

                            try
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note:" + NL + "Wait Move Inventory Items To A Family Member", StyledNotification.NotificationStyle.kGameMessagePositive));
                                target.MoveInventoryItemsToAFamilyMember();
                            }
                            catch (NullReferenceException)
                            {
                            }

                           
                            StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note:" + NL + "Wait Add Interaction", StyledNotification.NotificationStyle.kGameMessagePositive));
                            try
                            {
                                ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                            }
                            catch
                            {
                                try
                                {
                                    ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                                } 
                                catch
                                { }
                            }
                            
                            try
                            {
                                ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                            }
                            catch
                            {
                                try
                                {
                                    ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                                }
                                catch
                                { }
                            }

                            try
                            {
                                //target.InteractionQueue.CancelAllInteractions();
                                NFinalizeDeath.ForceCancelAllInteractionsProPartA(target);
                            }
                            catch
                            {
                                try
                                {
                                    //target.InteractionQueue.CancelAllInteractions();
                                    NFinalizeDeath.ForceCancelAllInteractionsProPartA(target);
                                }
                                catch
                                { }
                            }

                            try
                            {
                                if (asscheckDGSCore) // Anti-CallCallbackOnFailure NPC Only
                                {
                                    var clla = CCnlean.Singleton.CreateInstance(target, target, new InteractionPriority((InteractionPriorityLevel)100, 0f), false, true) as CCnlean;
                                    clla.MustRun = false;
                                    clla.Hidden = false;
                                    target.InteractionQueue.AddNextIfPossibleAfterCheckingForDuplicates(clla);
                                }
                            }
                            catch (StackOverflowException)
                            {
                                throw;
                            }
                            catch (OutOfMemoryException)
                            {
                                throw;
                            }

                            catch (ApplicationException)
                            { }

                            try
                            {
                                ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                            }
                            catch
                            {
                                try
                                {
                                    ExtKillSimNiec.CheckCancelListInteractionByNiec(target);
                                }
                                catch
                                { }
                            }



                            try
                            {
                                //target.InteractionQueue.CancelAllInteractions();
                                NFinalizeDeath.ForceCancelAllInteractionsProPartA(target);
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    //target.InteractionQueue.CancelAllInteractions();
                                    NFinalizeDeath.ForceCancelAllInteractionsProPartA(target);
                                    //NFinalizeDeath.ForceCancelAllInteractionsPro(target);
                                }
                                catch (Exception)
                                { }
                            }


                            try
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
                            catch (NullReferenceException)
                            { }


                            try
                            {
                                if (deathType == SimDescription.DeathType.Electrocution && obj != null)
                                {
                                    PetStartleBehavior.CheckForStartle(obj, StartleType.Electrocution);
                                }
                            }
                            catch (Exception caheckForStartle)
                            { NiecException.WriteLog("MineKill PetStartleBehavior: " + NiecException.NewLine + NiecException.LogException(caheckForStartle), true, false, false); }

                            try
                            {
                                if (Passport.IsPassportSim(target.SimDescription))
                                {
                                    if (!Passport.Instance.IsHostedSim(target.SimDescription) && target.SimDescription.mSenderNucleusID == SocialFeatures.Accounts.GetID())
                                    {
                                        SocialFeatures.Passport.CancelPassport(target.SimDescription.mStoredSlot);
                                        Passport.HouseholdSimDied(target.SimDescription);
                                    }
                                }
                                else
                                {
                                    Passport.HouseholdSimDied(target.SimDescription);
                                }
                            }
                            catch (Exception passportaaaa)
                            { NiecException.WriteLog("MineKill Passport: " + NiecException.NewLine + NiecException.LogException(passportaaaa), true, false, false); }




                            
                            try
                            {
                                ExtKillSimNiec killSim = ExtKillSimNiec.Singleton.CreateInstance(target, target, KillSimNiecX.DGSAndNonDGSPriority(), false, false) as ExtKillSimNiec;
                                killSim.simDeathType = deathType;
                                killSim.PlayDeathAnimation = playDeathAnim;
                                killSim.MustRun = true;
                                killSim.Hidden = true;

                                try
                                {
                                    killSim.mForceKillGrim = (target.Service is GrimReaper);
                                }
                                catch
                                { }

                                try
                                {

                                    if (target.Household != null)
                                    {
                                        killSim.homehome = target.Household;
                                    }

                                }
                                catch
                                { }
                                target.InteractionQueue.AddNextIfPossibleAfterCheckingForDuplicates(killSim);
                            }
                            catch (StackOverflowException)
                            {
                                throw;
                            }
                            catch (OutOfMemoryException)
                            {
                                throw;
                            }

                            catch (ApplicationException)
                            { }


                            try
                            {
                                if (!target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                                {
                                    
                                    var killSim2 = ExtKillSimNiec.Singleton.CreateInstance(target, target, KillSimNiecX.DGSAndNonDGSPriority(), false, false) as ExtKillSimNiec;
                                    killSim2.simDeathType = deathType;
                                    killSim2.PlayDeathAnimation = playDeathAnim;
                                    killSim2.MustRun = true;
                                    killSim2.Hidden = true;
                                    
                                    try
                                    {
                                        killSim2.mForceKillGrim = (target.Service is GrimReaper);
                                        
                                    }
                                    catch
                                    { }

                                    try
                                    {
                                        if (target.Household != null)
                                        {
                                            killSim2.homehome = target.Household;
                                        }

                                    }
                                    catch
                                    { }

                                    target.InteractionQueue.AddAfterCheckingForDuplicates(killSim2);
                                }
                            }
                            catch (StackOverflowException)
                            {
                                throw;
                            }
                            catch (OutOfMemoryException)
                            {
                                throw;
                            }

                            catch (ApplicationException)
                            { }


                            

                            if (target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Done Added Interaction", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                            else
                            {
                                if (AcceptCancelDialog.Show("MineKill: Failed! Add Interaction do you want Force Kill? (Yes Run or No Cancel)", true))
                                {
                                    try
                                    {
                                        StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note:" + NL + "Wait Move Inventory Items To A Family Member", StyledNotification.NotificationStyle.kGameMessagePositive));
                                        target.MoveInventoryItemsToAFamilyMember();
                                    }
                                    catch (Exception)
                                    { }
                                    
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + " is Died", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    try
                                    {

                                        if (target.SimDescription.Household != null)
                                        {
                                            ExtKillSimNiec.ListMorunExtKillSim(target, deathType);
                                        }
                                        NFinalizeDeath.FinalizeSimDeathRelationships(target.SimDescription, 0);

                                    }
                                    catch (Exception)
                                    {
                                        Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                        StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed To Catch Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                        return false;
                                    }
                                    finally
                                    {
                                        Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                        StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Done To Finally Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    }
                                    return false;
                                }
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Add Interaction Failed", StyledNotification.NotificationStyle.kGameMessagePositive));
                                return false;
                            }
                            
                            try
                            {
                                if (GameUtils.IsInstalled(ProductVersion.EP10))
                                {
                                    foreach (object objx in LotManager.AllLots)
                                    {
                                        Lot lot = (Lot)objx;
                                        if (lot.CommercialLotSubType == CommercialLotSubType.kEP10_Resort)
                                        {
                                            lot.ResortManager.CheckOutSim(target);
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            { }

                            return true;
                        }
                    }
                    catch (StackOverflowException)
                    {
                        return false;
                    }
                    catch (OutOfMemoryException)
                    {
                        return false;
                    }
                    catch (ResetException)
                    {
                        if (NFinalizeDeath.IsSimFastActiveHousehold(target) || target.IsInActiveHousehold) //(target.IsInActiveHousehold)
                        {
                            StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: ", StyledNotification.NotificationStyle.kGameMessagePositive));
                            throw;
                        }
                        else
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
                            
                            try
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Wait Move Inventory Items To A Family Member", StyledNotification.NotificationStyle.kGameMessagePositive));
                                target.MoveInventoryItemsToAFamilyMember();
                            }
                            catch (NullReferenceException)
                            {
                            }
                            target.SimDescription.SetDeathStyle(deathType, false);

                            try
                            {
                                //Found System.NullReferenceException: A null value was found where an object instance was required. Fixed Bug No Household :) 

                                ExtKillSimNiec.ListMorunExtKillSim(target, deathType);
                                NFinalizeDeath.FinalizeSimDeathRelationships(target.SimDescription, 0);

                            }
                            catch (Exception)
                            {
                                Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed To Catch Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                            finally
                            {
                                Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Done To Finally Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                        }
                        throw;
                    }
                    catch (Exception exception)
                    {
                        try
                        {
                            NiecException.WriteLog("MineKill " + NiecException.NewLine + NiecException.LogException(exception), true, true, false);

                            if (NFinalizeDeath.IsSimFastActiveHousehold(target) || target.IsInActiveHousehold)//(target.IsInActiveHousehold)
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: " + exception.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
                                return false;
                            }
                            else
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
                                
                                try
                                {
                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Wait Move Inventory Items To A Family Member", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    target.MoveInventoryItemsToAFamilyMember();
                                }
                                catch (NullReferenceException)
                                {
                                }

                                target.SimDescription.SetDeathStyle(deathType, false);

                                try
                                {
                                    
                                    ExtKillSimNiec.ListMorunExtKillSim(target, deathType);
                                    NFinalizeDeath.FinalizeSimDeathRelationships(target.SimDescription, 0);

                                }
                                catch (Exception)
                                {
                                    Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed To Catch Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    return true;
                                }
                                finally
                                {
                                    Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Done To Finally Run Force Kill :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                }
                            }
                            return true;
                        }
                        catch (Exception exception2Failed)
                        {
                            StyledNotification.Show(new StyledNotification.Format("MineKill Failed Again: " + target.Name + " Error: " + exception2Failed.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
                            NiecException.WriteLog("MineKill Failed Again: " + NiecException.NewLine + NiecException.LogException(exception2Failed), true, true, false);
                            try
                            {

                                target.SimDescription.SetDeathStyle(deathType, false);
                                try
                                {
                                    ExtKillSimNiec.ListMorunExtKillSim(target, deathType);
                                    NFinalizeDeath.FinalizeSimDeathRelationships(target.SimDescription, 0);
                                }
                                finally
                                {
                                    Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                                }
                            }
                            catch (Exception exception3FailedAgain)
                            { NiecException.WriteLog("MineKill Failed Again Again!: " + NiecException.NewLine + NiecException.LogException(exception3FailedAgain), true, true, false); }
                            return false;
                        }
                    }
                }
            }
            catch
            { }
            
            return false;
        }















































        private static bool CanBeKilledInternal(Sim targetSim)
        {
            if (targetSim == null)  return false;
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
                    try
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
                    catch
                    { }
                    
                }
                if (targetSim.Service is GrimReaper)
                {
                    if (!AcceptCancelDialog.Show("CanBeKilledInternal: Killing the " + targetSim.Name + " [GrimReaper] will prevent souls to cross over to the other side. If this happens, Sims that die from now on will be trapped between this world and the next, and you'll end up with a city full of dead bodies laying around. Are you sure you want to kill Death itself?", true))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                if (!AcceptCancelDialog.Show("CanBeKilledInternal: Name (" + targetSim.Name + ") Found Error (" + ex.Message + ") Do want you run? [Yes Run or No Cancel]", true))
                {
                    return false;
                }
            }
            

            return true;
        }




        public static bool DGSIsCrib(Sim targetSim)
        {
            if (targetSim == null) return false;
            try
            {
                // DGS: ... 
                // Too Many Add Interaction Toddler InCrib Fix
                if (targetSim.SimDescription.Age == CASAgeGenderFlags.Toddler || targetSim.SimDescription.Age == CASAgeGenderFlags.Baby)
                {
                    ICrib crib = targetSim.Posture.Container as ICrib;
                    if (targetSim.Posture.Satisfies(CommodityKind.InCrib, null) || crib != null)
                    {
                        return true;
                    }
                }
            }
            catch
            { }
            return false;
        }

        
        public static bool CanBeKilled(Sim targetSim, out bool outInCrib)
        {
            outInCrib = false;
            if (targetSim == null) return false;
            try
            {
                // DGS: ... 
                // Too Many Add Interaction Toddler InCrib Fix
                if (targetSim.SimDescription.Age == CASAgeGenderFlags.Toddler || targetSim.SimDescription.Age == CASAgeGenderFlags.Baby)
                {
                    ICrib crib = targetSim.Posture.Container as ICrib;
                    if (targetSim.Posture.Satisfies(CommodityKind.InCrib, null) || crib != null)
                    {
                        outInCrib = true;
                    }
                }
            }
            catch
            { }
            return CanBeKilledInternal(targetSim);
        }

        public static bool FastKill(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes)
        {
            return FastKill(target, deathType, obj, playDeathAnim, sleepyes, false);
        }

        public static bool FastKill(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes, bool wasisCrib)
        {
            if (target == null) { StyledNotification.Show(new StyledNotification.Format("FastKill" + NL + "Note: " + NL + "target is Cannot be null.", StyledNotification.NotificationStyle.kGameMessagePositive)); return false; }
            bool checkkillsimcrach = false;
            try
            {
                // Fix Crach Game Why?
                // sim is Destroyed
                Sim simes = target.SimDescription.CreatedSim;
                if (simes == null || target.HasBeenDestroyed)
                {
                    Vector3 pos = Vector3.OutOfWorld;
                    checkkillsimcrach = true;
                    ResourceKey outfitKey = target.SimDescription.mDefaultOutfitKey;
                    try
                    {
                        if ((PlumbBob.Singleton != null && PlumbBob.Singleton.mSelectedActor != null))
                        {
                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(PlumbBob.Singleton.mSelectedActor.Position);
                            fglParams.InitialSearchDirection = RandomUtil.GetInt(0, 7);
                            Vector3 vector;
                            if (!GlobalFunctions.FindGoodLocation(PlumbBob.Singleton.mSelectedActor, fglParams, out pos, out vector))
                            {
                                pos = PlumbBob.Singleton.mSelectedActor.Position;
                            }
                        }
                    }
                    catch
                    { }
                    Sim fixsim = target.SimDescription.Instantiate(pos, outfitKey, false, true);
                    if (fixsim == null)
                    {
                        StyledNotification.Show(new StyledNotification.Format("FastKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Failed Instantiate In Fix Crash Game", StyledNotification.NotificationStyle.kGameMessagePositive));
                        return false;
                    }
                    target = fixsim;
                    NiecMod.Nra.SpeedTrap.Sleep();
                }
                LogMineKill(target, deathType, obj, playDeathAnim, sleepyes, DGSIsCrib(target) || wasisCrib);
            }
            catch
            { }

            bool checkcrib;
            if (wasisCrib) {
                if (CanBeKilledInternal(target)) {
                    Sims3.NiecHelp.Tasks.KillAnnihilationTask kt = new Sims3.NiecHelp.Tasks.KillAnnihilationTask(target, deathType, obj, playDeathAnim, sleepyes, wasisCrib);
                    kt.AddToSimulator();
                    //StyledNotification.Show(new StyledNotification.Format("Check Ok", StyledNotification.NotificationStyle.kGameMessagePositive));
                    //LogMineKill(target, deathType, obj, playDeathAnim, sleepyes, wasisCrib);
                    return true;
                } else if (checkkillsimcrach) {
                    Sim.MakeSimGoHome(target, true);
                    return false;
                }
                return false;
            } else if (CanBeKilled(target, out checkcrib)) {
                Sims3.NiecHelp.Tasks.KillAnnihilationTask kt = new Sims3.NiecHelp.Tasks.KillAnnihilationTask(target, deathType, obj, playDeathAnim, sleepyes, checkcrib);
                kt.AddToSimulator();
                //LogMineKill(target, deathType, obj, playDeathAnim, sleepyes, checkcrib);
                //StyledNotification.Show(new StyledNotification.Format("Check Ok", StyledNotification.NotificationStyle.kGameMessagePositive));
                return true;
            } else if (checkkillsimcrach) {
                Sim.MakeSimGoHome(target, true);
                return false;
            }
            else return false;  
        }





        private static bool IsSimDisposed(Sim simd)
        {
            //if (Simulator.CheckYieldingContext(false)) return true;
            return sIForceError.Length == 0 && simd == Sim.ActiveActor;
        }





        public static void LogMineKill(object whereSimOrSimDescription, SimDescription.DeathType deathtype, GameObject obj, bool playDeathAnim, bool sleepyes, bool wasisCrib)
        {
            if (!sLoaderLogTraneEx) return;
            if (whereSimOrSimDescription == null) return;
            //if (sForceError /* && Simulator.CheckYieldingContext(false) */) IsSimDisposed(whereSimOrSimDescription as Sim);
            try
            {
                Sim ForceError = null;
                ForceError.CanBeSold();
            }
            catch (NullReferenceException ex)
            {
                Sim simroot = null;
                /*
                string msg = null;
                msg += sim.FullName + System.Environment.NewLine;
                sLogEnumeratorTrane++;
                LogTraneEx.AppendFormat("Hello World" + ex.ToString() + msg, new object[0]);
                 */



                try
                {
                    simroot = whereSimOrSimDescription as Sim;
                }
                catch
                { }



                string nl = System.Environment.NewLine;
                sLogEnumeratorTrane++;
                string tempst = nl + "MineKill " + "No: " + sLogEnumeratorTrane.ToString() + nl;

                tempst += nl;

                tempst += "Kill:";

                //tempst += nl;

                string reson = "";
                string resonb = "";
                try
                {
                    if (simroot != null)
                    {
                        tempst += nl + " Niec_CanBeKilled: " + NonEACanBeKilledPro(simroot, out resonb) + nl + "   Reason:" + resonb;
                    }
                }
                catch
                { }



                try
                {
                    if (simroot != null)
                    {
                        tempst += nl + " EA_CanBeKilled: " + EACanBeKilledPro(simroot, out reson) + nl + "   Reason:" + reson;
                    }
                }
                catch
                { tempst += nl + " EA_CanBeKilled: " + "False" + nl + "   Reason:" + " Error"; }

                tempst += nl;



                tempst += "End Kill";


                tempst += nl;





                //tempst += nl;

                //SimDescription sima = sim as SimDescription;
                try
                {
                    Sim sima = whereSimOrSimDescription as Sim;
                    if (sima != null && !sima.HasBeenDestroyed && sima.SimDescription != null)
                    {
                        tempst += NiecException.GetDescription(sima.SimDescription);
                        tempst += nl;
                        tempst += nl;
                        tempst += "End SimDescription";
                        if (sima.InteractionQueue != null)
                        {
                            tempst += nl;
                            tempst += NiecException.InteractionListLitePro(sima.SimDescription);
                            tempst += nl;
                            //tempst += nl;
                            tempst += "End Interactions";
                        }
                    }
                    else
                    {
                        SimDescription simdesc = whereSimOrSimDescription as SimDescription;
                        if (simdesc != null)
                        {
                            tempst += NiecException.GetDescription(simdesc);
                            tempst += nl;
                            tempst += nl;
                            tempst += "End SimDescription";
                            if (simdesc.CreatedSim != null && simdesc.CreatedSim.InteractionQueue != null)
                            {
                                tempst += nl;
                                tempst += NiecException.InteractionListLitePro(simdesc);
                                tempst += nl;
                                //tempst += nl;
                                tempst += "End Interactions";
                            }
                        }
                        else
                        {
                            IMiniSimDescription isimdesc = whereSimOrSimDescription as IMiniSimDescription;
                            if (isimdesc != null)
                            {
                                tempst += NiecException.GetDescription(isimdesc);
                                tempst += nl;
                                tempst += nl;
                                tempst += "End SimDescription";
                                SimDescription iSimDescription = isimdesc as SimDescription;
                                if (iSimDescription != null && iSimDescription.CreatedSim != null && iSimDescription.CreatedSim.InteractionQueue != null)
                                {
                                    tempst += nl;
                                    tempst += NiecException.InteractionListLitePro(iSimDescription);
                                    tempst += nl;
                                    //tempst += nl;
                                    tempst += "End Interactions";
                                }
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    tempst += nl + "Error GetDescription: " + err.Message + nl;
                }
                
                tempst += nl;
                tempst += nl;

                try
                {
                    tempst += "Parameters: " + nl;
                    //tempst += nl;

                    tempst += nl + "DeathType: " + deathtype.ToString();

                    if (obj == null) {
                        tempst += nl + "GameObject: " + "None";
                        //tempst += nl;
                    } else {
                        tempst += nl + "GameObject: " + nl + obj.GetObjectLocationInformation();
                        //tempst += nl;
                    }
                    tempst += nl + "PlayDeathAnim: " + playDeathAnim.ToString();

                    tempst += nl + "SleepYes: " + sleepyes.ToString();

                    tempst += nl + "IsCrib: " + wasisCrib.ToString();

                    //tempst += nl + "CancellableByPlayer: " + !HasFlags(InteractionFlags.Uncancellable);


                    /*
                    string reson = "";
                    string resonb = "";
                    try
                    {
                        if (simroot != null)
                        {
                            tempst += nl + "NonEA_CanBeKilled: " + NonEACanBeKilledPro(simroot, out resonb) + "Reason:" + resonb;
                        }
                    }
                    catch
                    { }



                    try
                    {
                        if (simroot != null)
                        {
                            tempst += nl + "EA_CanBeKilled: " + EACanBeKilledPro(simroot, out reson) + "Reason:" + reson;
                        }
                    }
                    catch
                    { tempst += nl + "EA_CanBeKilled: " + "False"  + "Reason:" + " Error"; }
                    */

                    tempst += nl;

                    

                    tempst += nl;
                }
                catch (Exception exx)
                {
                    tempst += nl + "Error: " + exx.Message + nl;
                }

                try
                {
                    //Sims3.OpenDGS.Message.sLogEnumeratorLogHelperEx++;
                    LogTraneEx.AppendFormat(tempst + nl + "Added: " + nl + ex.ToString() + nl, new object[0]);
                }
                catch
                { }
            }
            return;
        }








        public static void RemoveSimDescriptionRelationships(SimDescription x)
        {
            IEnumerable<Relationship> enumerable = Relationship.Get(x);
            if (enumerable != null)
            {
                foreach (Relationship relationship in enumerable)
                {
                    SimDescription otherSimDescription = relationship.GetOtherSimDescription(x);
                    if (x != otherSimDescription)
                    {
                        // Changed
                        if (Relationship.sAllRelationships.ContainsKey(otherSimDescription))
                        {
                            Relationship.sAllRelationships[otherSimDescription].Remove(x);
                        }
                    }
                }
                Relationship.sAllRelationships.Remove(x);
            }
        }

        public static void CleanseGenealogy(IMiniSimDescription me)
        {
            Genealogy genealogy = me.CASGenealogy as Genealogy;
            if (genealogy != null)
            {
                genealogy.ClearAllGenealogyInformation();
            }
        }
    }
}

