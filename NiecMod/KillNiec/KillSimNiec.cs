/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 15/09/2018
 * Time: 21:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Reflection;

using NiecMod.Interactions;
using NiecMod.Nra;
using Sims3.Gameplay.Controllers.Niec;

using NRaas;
using NRaas.CommonSpace.Helpers;

using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Interfaces;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.Passport;
using Sims3.Gameplay.Roles;
using Sims3.Gameplay.Scuba;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Situations;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Utilities;

using Sims3.SimIFace;

using Sims3.UI;
using Sims3.UI.Hud;
using NiecS3Mod;
using NiecMod.Interactions.Hidden;
using Sims3.Gameplay.Objects.Vehicles;
using Sims3.Gameplay;
using System.Threading;



using Sims3.Gameplay.Careers;
using Sims3.SimIFace.CAS;
using NiecMod.Helpers.MakeSimPro;
using Sims3.Gameplay.UI;
using Sims3.Gameplay.EventSystem;
using Sims3.Gameplay.Skills;
using Sims3.UI.GameEntry;
using Sims3.Gameplay.Objects.Miscellaneous;
using Sims3.Gameplay.TimeTravel;
using Sims3.Gameplay.Seasons;
using Sims3.Gameplay.Objects.Island;
//using DGSCore.Utilities;
namespace NiecMod.KillNiec
{
    /// <summary>
    /// Copy Code's NRaas
    /// </summary>
    public static class AssemblyCheckByNiec
    {
        static Dictionary<string, bool> sAssemblies = new Dictionary<string, bool>();

        public static string GetNamespace(Assembly assembly)
        {
            Type type = assembly.GetType("NRaas.VersionStamp");
            if (type == null) return null;

            FieldInfo nameSpaceField = type.GetField("sNamespace", BindingFlags.Static | BindingFlags.Public);
            if (nameSpaceField == null) return null;

            return nameSpaceField.GetValue(null) as string;
        }

        public static bool IsInstalled(string assembly)
        {
            if (string.IsNullOrEmpty(assembly)) return false;

            assembly = assembly.ToLower();

            bool loaded;
            if (sAssemblies.TryGetValue(assembly, out loaded))
            {
                return loaded;
            }

            loaded = (FindAssembly(assembly) != null);

            sAssemblies.Add(assembly, loaded);

            return loaded;
        }
        public static Assembly FindAssembly(string name)
        {
            name = name.ToLower();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetName().Name.ToLower() == name)
                {
                    return assembly;
                }
            }
            return null;
        }


        public static void Atiao()
        {
            try
            {
                if (NTunable.kDedugNiecModExceptionExtKillSimNiec)
                {
                    throw new NiecModException("Atiao: Not Error");
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
                    NiecException.WriteLog("Atiao: " + NiecException.NewLine + NiecException.LogException(ex), true, true, false);
                }
            }
        }
    }
    /// <summary>
    /// This a Helper KillSim
    /// </summary>
    public class KillSimNiecX
    {
        /*
        //public HelperNra helperNra;
        public Sim simaa;

        public Vehicle rtyryds()
        {
            try
            {
                Vehicle vehicle;


                if (!simaa.IsInActiveHousehold)
                {
                    vehicle = Vehicle.CreateTaxi();
                }

                ProductVersion productVersion = ProductVersion.BaseGame;
                string instanceName;

                instanceName = (RandomUtil.CoinFlip() ? "MotorcyleRacing" : "MotorcyleRacing");
                productVersion = ProductVersion.SP2;

                CarOwnable carOwnable = GlobalFunctions.CreateObjectOutOfWorld(instanceName, productVersion) as CarOwnable;
                vehicle = carOwnable;

                CarOwnable carOwnable2 = vehicle as CarOwnable;
                if (carOwnable2 != null)
                {
                    carOwnable2.LotHome = simaa.LotHome;
                }
                return vehicle;
            }
            catch (Exception exception)
            {
                DGSCoreMsg.WriteLog("GetNPCVehicle: " + DGSCoreMsg.NewLine + DGSCoreMsg.LogException(exception), true, true);
                if (!simaa.IsInActiveHousehold)
                {
                    return Vehicle.CreateTaxi();
                }
                CarOwnable carOwnable = GlobalFunctions.CreateObjectOutOfWorld("HoverCarSports", ProductVersion.EP11) as CarOwnable;
                carOwnable.GeneratedOwnableForNpc = false;
                carOwnable.LotHome = simaa.LotHome;
                return carOwnable;
            }
            
        }
        */

        //static HelperNra helperNra  = null;

        public class AlarmTask
        {
            Sims3.Gameplay.Function mFunction;

            AlarmHandle mHandle;

            ObjectGuid mRunningTask = ObjectGuid.InvalidObjectGuid;

            bool mDisposeOnTimer;

            

            static List<AlarmTask> sTasks = new List<AlarmTask>();

            protected AlarmTask(float time, TimeUnit timeUnit)
                : this(time, timeUnit, null)
            { }
            public AlarmTask(float time, TimeUnit timeUnit, Sims3.Gameplay.Function func)
                : this(func)
            {
                mHandle = AlarmManager.Global.AddAlarm(time, timeUnit, OnTimer, "NRaasDelayedFunction", AlarmType.NeverPersisted, null);

                mDisposeOnTimer = true;
            }
            public AlarmTask(float time, TimeUnit timeUnit, float repeatTime, TimeUnit repeatTimeUnit)
                : this(time, timeUnit, null, repeatTime, repeatTimeUnit)
            { }
            public AlarmTask(float time, TimeUnit timeUnit, Sims3.Gameplay.Function func, float repeatTime, TimeUnit repeatTimeUnit)
                : this(func)
            {
                mHandle = AlarmManager.Global.AddAlarmRepeating(time, timeUnit, OnTimer, repeatTime, repeatTimeUnit, "NRaasRepeatFunction", AlarmType.NeverPersisted, null);
            }
            public AlarmTask(float hourOfDay, DaysOfTheWeek days)
                : this(hourOfDay, days, null)
            { }
            public AlarmTask(float hourOfDay, DaysOfTheWeek days, Sims3.Gameplay.Function func)
                : this(func)
            {
                mHandle = AlarmManager.Global.AddAlarmDay(hourOfDay, days, OnTimer, "NRaasDailyFunction", AlarmType.NeverPersisted, null);
            }
            protected AlarmTask(Sims3.Gameplay.Function func)
            {
                sTasks.Add(this);

                if (func == null)
                {
                    func = OnPerform;
                }

                mFunction = func;
            }

            protected virtual void OnPerform()
            { }

            public bool Valid
            {
                get { return (mHandle != AlarmHandle.kInvalidHandle); }
            }

            public void Dispose() 
            {
                Simulator.DestroyObject(mRunningTask);
                mRunningTask = ObjectGuid.InvalidObjectGuid;

                AlarmManager.Global.RemoveAlarm(mHandle);
                mHandle = AlarmHandle.kInvalidHandle;

                sTasks.Remove(this);
            }

            public static float TimeTo(float hourOfDay)
            {
                float time = hourOfDay - SimClock.HoursPassedOfDay;
                if (time < 0f)
                {
                    time += 24f;
                }

                return time;
            }

            public static void DisposeAll()
            {
                List<AlarmTask> tasks = new List<AlarmTask>(sTasks);
                foreach (AlarmTask task in tasks)
                {
                    task.Dispose();
                }

                sTasks.Clear();
            }

            private void OnTimer()
            {
                try
                {
                    if (mDisposeOnTimer)
                    {
                        Dispose();
                    }

                    mRunningTask = FunctionTask.Perform(mFunction);
                }
                catch (Exception exception)
                {
                    NiecException.WriteLog(NiecException.LogException(exception), true, false, false);
                }
            }

            public override string ToString()
            {
                string result = null;
                if (mFunction != null)
                {
                    result += mFunction.Method.ToString();
                    if (mFunction.Target != null)
                    {
                        result = mFunction.Target.GetType() + " : " + result;
                    }
                }

                return result;
            }
        }

        public class FunctionTask : Task
        {
            Sims3.Gameplay.Function mFunction;

            protected FunctionTask()
            {
                mFunction = OnPerform;
            }
            protected FunctionTask(Sims3.Gameplay.Function func)
            {
                mFunction = func;
            }

            public static ObjectGuid Perform(Sims3.Gameplay.Function func)
            {
                return new FunctionTask(func).AddToSimulator();
            }

            public ObjectGuid AddToSimulator()
            {
                return Simulator.AddObject(this);
            }

            protected virtual void OnPerform()
            { }

            public override void Simulate()
            {
                try
                {
                    mFunction();
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    NiecException.WriteLog(NiecException.LogException(exception), true, false, false);
                }
                finally
                {
                    Simulator.DestroyObject(ObjectId);
                }
            }

            public override string ToString()
            {
                if (mFunction == null)
                {
                    return "(OneShotFunctionTask) NULL function";
                }
                else
                {
                    return ("(OneShotFunctionTask) Function method: " + this.mFunction.Method.ToString() + ", Declaring Type: " + this.mFunction.Method.DeclaringType.ToString());
                }
            }
        }


        public class FunctionTaskTest : Task
        {
            NFinalizeDeath.Function mFunction;

            protected FunctionTaskTest()
            {
                mFunction = OnPerform;
            }
            protected FunctionTaskTest(NFinalizeDeath.Function func)
            {
                mFunction = func;
            }

            public static ObjectGuid Perform(NFinalizeDeath.Function func)
            {
                return new FunctionTaskTest(func).AddToSimulator();
            }

            public ObjectGuid AddToSimulator()
            {
                return Simulator.AddObject(this);
            }

            protected virtual void OnPerform()
            { }

            public override void Simulate()
            {
                try
                {
                    mFunction();
                }
                catch (ResetException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    NiecException.WriteLog(NiecException.LogException(exception), true, false, false);
                }
                finally
                {
                    Simulator.DestroyObject(ObjectId);
                }
            }

            public override string ToString()
            {
                if (mFunction == null)
                {
                    return "(OneShotFunctionTask) NULL function";
                }
                else
                {
                    return ("(OneShotFunctionTask) Function method: " + this.mFunction.Method.ToString() + ", Declaring Type: " + this.mFunction.Method.DeclaringType.ToString());
                }
            }
        }


        /// <summary>
        /// This a Toaiyov
        /// </summary>
        /// <returns></returns>
        public bool CanBeKilledX()
        {
            return true;
        }
        /// <summary>
        /// This a Tosp
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deathType"></param>
        /// <returns></returns>
        public static bool MineKill(Sim target, SimDescription.DeathType deathType)
        {
            return MineKill(target, deathType, null, true, false);
        }
        /// <summary>
        /// This a Tos Stosiat
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deathType"></param>
        /// <param name="otarget"></param>
        /// <returns></returns>
        public static bool MineKill(Sim target, SimDescription.DeathType deathType, GameObject otarget)
        {
            return MineKill(target, deathType, otarget, true, true);
        }
        public static bool MineKill(Sim target, SimDescription.DeathType deathType, GameObject otarget, bool playDeathAnim)
        {
            try
            {
                return MineKill(target, deathType, otarget, playDeathAnim, true);
            }
            catch
            { return false; }
            
        }
        /// <summary>
        /// This Aoapc
        /// </summary>
        /// <param name="target"></param>
        /// <param name="deathType"></param>
        /// <param name="obj"></param>
        /// <param name="playDeathAnim"></param>
        /// <returns></returns>


        public static bool Sarae = false;

        public static readonly string NL = System.Environment.NewLine;

        public unsafe static bool MineKill(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes)
        {
            if (target == null) { StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Note: " + NL + "target is Cannot be null.", StyledNotification.NotificationStyle.kGameMessagePositive)); return false; }
            // Fix Crash Game
            bool checkkillsimcrach = false;
            try
            {
                Sim simes = target.SimDescription.CreatedSim;
                if (simes == null)
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
                    Sim asdta = target.SimDescription.Instantiate(pos, outfitKey, false, true);
                    if (asdta == null)
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Failed Instantiate In Fix Crash Game", StyledNotification.NotificationStyle.kGameMessagePositive));
                        return false;
                    }
                    target = asdta;
                    NiecMod.Nra.SpeedTrap.Sleep();
                }
            }
            catch
            { }
            
            /*
            try
            {
                throw new NiecModException("MineKill");
            }
            catch (NiecModException ex)
            {
                try
                {

                    NiecException.WriteLog("Script Error: " + NiecException.NewLine + NiecException.GetDescription(target.SimDescription) + NiecException.NewLine + NiecException.InteractionList(target.SimDescription) + NiecException.NewLine + NiecException.LogException(ex), true, false, true);
                }
                catch
                { NiecException.WriteLog(ex.ToString(), true, false, false); }

            }
            */
























            if (sleepyes)
            {
                NiecMod.Nra.SpeedTrap.Sleep(20);
            }
            try
            {
                if (target.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton)) return true;
                if (target.InteractionQueue.HasInteractionOfType(typeof(ExtKillSimNiec))) return true;
                if (target.InteractionQueue.HasInteractionOfTypeAndTarget(ExtKillSimNiec.Singleton, target)) return true;
            }
            catch
            { }




            
            /*
            try
            {
                if (target.SimDescription.Age == CASAgeGenderFlags.Toddler || target.SimDescription.Age == CASAgeGenderFlags.Baby)
                {
                    ulong newidsimdsc = 0L;
                    SimDescription simdescfixllop = null;
                    ICrib crib = target.Posture.Container as ICrib;
                    if (target.Posture.Satisfies(CommodityKind.InCrib, null) || crib != null)
                    {
                        newidsimdsc = target.SimDescription.SimDescriptionId;

                        try
                        {
                            target.SetObjectToReset();
                        }
                        catch
                        { }

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
                            if (simdescfixllop.CreatedSim == null)
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
                                Sim asdta = simdescfixllop.Instantiate(pos, outfitKey, false, false);
                                if (asdta == null)
                                {
                                    StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "Note: " + NL + "Failed Instantiate InCrib", StyledNotification.NotificationStyle.kGameMessagePositive));
                                    return false;
                                }
                                if (Simulator.CheckYieldingContext(false)) { NiecMod.Nra.SpeedTrap.Sleep(); }
                            }

                            target = simdescfixllop.CreatedSim;
                        }
                    }
                }
            }
            catch (Exception errer)
            {
                NiecException.WriteLog("MineKill In Crib" + NiecException.NewLine + NiecException.LogException(errer), true, true, false);
            }

            
            */










            /*
            try
            {
                FunctionTaskTest.Perform(AssemblyCheckByNiec.Atiao);
            }
            catch (Exception)
            { }
            */

            try
            {
                foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                {
                    if (interactionInstance is ExtKillSimNiec)
                    {
                        return false;
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
                    
                    
                    if (target.mInteractionQueue == null)
                    {
                        target.mInteractionQueue = new Sims3.Gameplay.ActorSystems.InteractionQueue(target);
                    }
                    target.mInteractionQueue.OnLoadFixup();
                }
                catch (Exception)
                { }
                try
                {
                    if (!AssemblyCheckByNiec.IsInstalled("DGSCore"))
                    {
                        if (!target.IsInActiveHousehold)
                        {
                            if (deathType == SimDescription.DeathType.Ranting)
                            {
                                if (target.IsHuman)
                                {
                                    switch (deathType)
                                    {
                                        case SimDescription.DeathType.Ranting:
                                            deathType = SimDescription.DeathType.Electrocution;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                { }


                if (deathType == SimDescription.DeathType.PetOldAgeBad)
                {
                    if (target.IsHuman)
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
                if (deathType == SimDescription.DeathType.PetOldAgeGood)
                {
                    if (target.IsHuman)
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

                if (!AssemblyCheckByNiec.IsInstalled("DGSCore") && target.IsPet)
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
                        catch (Exception)
                        {
                        }
                        Urnstones.CreateGrave(target.SimDescription, deathType, true, false);
                        return true;
                    }
                    if (!CheckNiecKill(target)) return false;
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
                


                // Fake Start
                if (Sarae && target.SimDescription.AssignedRole is NPCAnimal && (target.Household == null && target.Household.LotId == ObjectGuid.InvalidObjectGuid.Value))
                {
                    Lot lot = null;
                    if (lot != null)
                    {
                        List<Sim> allActors = lot.GetAllActors();
                        List<Sim> list = new List<Sim>();
                        List<Vector3> list2 = new List<Vector3>();
                        List<Sim> list3 = new List<Sim>();
                        List<Lot> list4 = new List<Lot>();
                        uint offsetHint = 0u;
                        foreach (Sim item in allActors)
                        {
                            if (item.IsNPC && (item.Household == null || item.Household.LotId == ObjectGuid.InvalidObjectGuid.Value))
                            {
                                item.SetObjectToReset();
                                Simulator.Sleep(0u);
                                item.Destroy();
                            }
                            else if (Sarae)
                            {



                                SimDescription mSimdesc = target.SimDescription;

                                Sim mSim = target;

                                Household mHousehold = target.Household;

                                Vector3 mHouseVeri3 = Vector3.OutOfWorld;

                                SimDescription.DeathType mdeathtype = deathType;

                                try
                                {
                                    if (!mSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                                    {
                                        return true;
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
                                                foreach (SimDescription iteme in listxt)
                                                {
                                                    Sim sim = iteme.CreatedSim;
                                                    if (sim == null) continue;
                                                    if (sim.LotCurrent != null && sim.LotCurrent.IsWorldLot) continue;
                                                    mHouseVeri3 = sim.Position;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(AssemblyCheckByNiec.Atiao), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);
                                                return true;
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(AssemblyCheckByNiec.Atiao), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);
                                        return true;
                                    }
                                }
                                try
                                {



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
                                                NiecMod.Nra.SpeedTrap.Sleep();
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
                                                                foreach (SimDescription iteme in listxt)
                                                                {
                                                                    Sim sim = iteme.CreatedSim;
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

                                                    var asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                                    if (asdoei != null)
                                                    {
                                                        // Add New
                                                        if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                                        {
                                                            Lot lotloot = mSim.LotCurrent;
                                                            if (lotloot != null && lotloot.IsDivingLot)
                                                            {
                                                                ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
                                                                if (scubaDivinga != null)
                                                                {
                                                                    mSim.Posture = scubaDivinga;
                                                                    scubaDivinga.StartBubbleEffects();
                                                                }
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    else if (mSim.SimDescription != null)
                                    {
                                        try
                                        {
                                            foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                                            {
                                                if (interactionInstance.GetPriority().Level > (InteractionPriorityLevel)13) //  DGSCore :)
                                                {
                                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Level UP Level :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        { }

                                        try
                                        {
                                            foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                                            {
                                                if (interactionInstance.GetPriority().Level < (InteractionPriorityLevel)999) // Low Level ReNPCR Loop
                                                {
                                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Level Down Level :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        { }


                                        if (!CheckNiecKill(target))
                                        {
                                            if (deathType == SimDescription.DeathType.OldAge)
                                            {
                                                if (!target.SimDescription.IsEP11Bot)
                                                {
                                                    if (target.SimDescription.Elder)
                                                    {
                                                        target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                    }
                                                }
                                                else if (target.SimDescription.Elder)
                                                {
                                                    target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                }
                                            }
                                            if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                            {
                                                if (deathType == SimDescription.DeathType.Thirst)
                                                {
                                                    if (!target.SimDescription.IsEP11Bot)
                                                    {
                                                        if (target.SimDescription.Elder)
                                                        {
                                                            target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                        }
                                                    }
                                                    else if (target.SimDescription.Elder)
                                                    {
                                                        target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                    }
                                                }
                                            }
                                            target.FadeIn();
                                            if (NTunable.kDedugNotificationCheckNiecKill)
                                            {
                                                StyledNotification.Show(new StyledNotification.Format("MineKill: Sorry, " + target.Name + " No CheckNiecKill", StyledNotification.NotificationStyle.kGameMessagePositive));
                                            }
                                            return false;
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
                                                                    foreach (SimDescription iteme in listxt)
                                                                    {
                                                                        Sim sim = iteme.CreatedSim;
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

                                                        var asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                                        if (asdoei != null)
                                                        {
                                                            // Add New
                                                            if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                                            {
                                                                Lot lotloot = mSim.LotCurrent;
                                                                if (lotloot != null && lotloot.IsDivingLot)
                                                                {
                                                                    ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
                                                                    if (scubaDivinga != null)
                                                                    {
                                                                        mSim.Posture = scubaDivinga;
                                                                        scubaDivinga.StartBubbleEffects();
                                                                    }
                                                                }
                                                                else if (GameUtils.IsInstalled(ProductVersion.EP10))
                                                                {
                                                                    try
                                                                    {
                                                                        bool Autonomous = true;
                                                                        if (Autonomous || target.IsNPC) return false;
                                                                        if (!AcceptCancelDialog.Show("Death Good System: Are You Sure Get Sim Descriptions In World Kill Sim?"))
                                                                        {
                                                                            if (AcceptCancelDialog.Show("Death Good System: CleanUp Dead SimDescription?"))
                                                                            {
                                                                                List<SimDescription> msime = new List<SimDescription>();
                                                                                try
                                                                                {
                                                                                    foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                                                                                    {
                                                                                        if (urnstone.DeadSimsDescription != null && !msime.Contains(urnstone.DeadSimsDescription))
                                                                                        {
                                                                                            msime.Add(urnstone.DeadSimsDescription);
                                                                                        }
                                                                                    }





                                                                                    foreach (SimDescription murnstone in msime)
                                                                                    {
                                                                                        if (murnstone == null) continue;
                                                                                        try
                                                                                        {
                                                                                            if (murnstone.Household == Household.ActiveHousehold) continue;
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                        try
                                                                                        {



                                                                                            if (murnstone.IsDead)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    if (murnstone.CreatedSim != null)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(murnstone.CreatedSim);
                                                                                                        }
                                                                                                        catch
                                                                                                        { }
                                                                                                        try
                                                                                                        {
                                                                                                            murnstone.CreatedSim.Genealogy.ClearAllGenealogyInformation();

                                                                                                        }
                                                                                                        catch
                                                                                                        { }

                                                                                                        try
                                                                                                        {
                                                                                                            murnstone.CreatedSim.Genealogy.ClearMiniSimDescription();
                                                                                                        }
                                                                                                        catch
                                                                                                        { }
                                                                                                        try
                                                                                                        {
                                                                                                            murnstone.CreatedSim.Destroy();
                                                                                                        }
                                                                                                        catch
                                                                                                        { }
                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }










                                                                                                try
                                                                                                {
                                                                                                    murnstone.Genealogy.ClearAllGenealogyInformation();
                                                                                                }
                                                                                                catch
                                                                                                { }

                                                                                                try
                                                                                                {
                                                                                                    Household household = murnstone.Household;
                                                                                                    if (household != null)
                                                                                                    {
                                                                                                        household.Remove(murnstone, !household.IsSpecialHousehold);
                                                                                                    }

                                                                                                }
                                                                                                catch
                                                                                                { }






                                                                                                try
                                                                                                {
                                                                                                    while (true)
                                                                                                    {
                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(murnstone);
                                                                                                        if (urnstone != null)
                                                                                                        {
                                                                                                            try
                                                                                                            {
                                                                                                                urnstone.DeadSimsDescription = null;
                                                                                                                urnstone.Destroy();
                                                                                                            }
                                                                                                            catch
                                                                                                            { }

                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            break;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }
                                                                                                try
                                                                                                {
                                                                                                    murnstone.Dispose();
                                                                                                }
                                                                                                catch
                                                                                                { }










                                                                                            }




                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                    }



                                                                                }
                                                                                catch
                                                                                { }


                                                                                return true;
                                                                            }
                                                                            return false;
                                                                        }
                                                                        bool flag = false;

                                                                        if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0)
                                                                        {
                                                                            try
                                                                            {
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Contains(sdtyf))
                                                                                            Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            flag = true;
                                                                            if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0) return true;
                                                                        }

                                                                        if (!flag && AcceptCancelDialog.Show("Do you Are Add All SimDescriptions?"))
                                                                        {
                                                                            try
                                                                            {
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                        }
                                                                        if (AcceptCancelDialog.Show("Do you Are Delete All SimDescriptions?"))
                                                                        {

                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }


                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    for (int i = 0; i < 10; i++)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }
                                                                            }



                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions?"))
                                                                        {
                                                                            try
                                                                            {
                                                                                ProgressDialog.Show("Processing", false);
                                                                                NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {

                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        allService.ClearServicePool();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Service.Destroy(allService);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }

                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearAllGenealogyInformation();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    for (int i = 0; i < 20; i++)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }
                                                                            }
                                                                            try
                                                                            {
                                                                                ProgressDialog.Close();
                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                {
                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                    StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                    if (optionsModel != null)
                                                                                    {
                                                                                        optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                        PlumbBob.ForceSelectActor(null);
                                                                                        try
                                                                                        {
                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                            if (optionsModel != null)
                                                                                            {
                                                                                                optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }
                                                                                    finally
                                                                                    { PlumbBob.ForceSelectActor(target); }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions? Clear Keep Save"))
                                                                        {
                                                                            try
                                                                            {
                                                                                ProgressDialog.Show("Deleteing SimDescription...", false);
                                                                                NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {

                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        allService.ClearServicePool();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Service.Destroy(allService);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                /*
                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }
                                                                                */
                                                                                try
                                                                                {
                                                                                    while (true)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }
                                                                            }
                                                                            try
                                                                            {
                                                                                try
                                                                                {
                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();
                                                                                }
                                                                                catch
                                                                                { }


                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                {
                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                    StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                    if (optionsModel != null)
                                                                                    {
                                                                                        optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                        ProgressDialog.Close();
                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {

                                                                                    try
                                                                                    {
                                                                                        ProgressDialog.Close();
                                                                                        StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                        PlumbBob.ForceSelectActor(null);
                                                                                        try
                                                                                        {
                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                            if (optionsModel != null)
                                                                                            {
                                                                                                optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }
                                                                                    finally
                                                                                    { PlumbBob.ForceSelectActor(target); }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                        }
                                                                        else if (PlumbBob.Singleton.mSelectedActor != null && NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore") && AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions? Clear Keep Save Not Active"))
                                                                        {
                                                                            bool flagsae = AcceptCancelDialog.Show("Delete All MiniSimDescription?");
                                                                            bool flagsaea = !AcceptCancelDialog.Show("No make sim?");
                                                                            bool flagsa = AcceptCancelDialog.Show("ProgressDialog?");
                                                                            Lot mlot = target.LotCurrent;
                                                                            //Simulator.YieldingDisabled = true;
                                                                            //NiecTask mtask = new NiecTask.AddToSimulator();
                                                                            Nra.NFinalizeDeath.mResetError = true;
                                                                            ObjectGuid sdae = Sims3.NiecHelp.Tasks.NiecTask.Perform(Nra.NFinalizeDeath.ResetError);
                                                                            Sims3.SimIFace.Gameflow.GameSpeed currentGameSpeed = Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed;
                                                                            Vector3 mposition = Vector3.Invalid;



                                                                            try
                                                                            {
                                                                                CommandSystem.ExecuteCommandString("dgsunsafekill false");
                                                                                CommandSystem.ExecuteCommandString("dgspx false");
                                                                            }
                                                                            catch
                                                                            { }




                                                                            try
                                                                            {
                                                                                if (flagsa)
                                                                                {
                                                                                    //ProgressDialog.Show("Deleteing SimDescription...", ModalDialog.PauseMode.PauseSimulator, true, new Vector2(-1f, -1f), true, false, false);
                                                                                    //GameUtils.SetGameTimeSpeedLevel(0);

                                                                                    Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed = Sims3.SimIFace.Gameflow.GameSpeed.Pause;
                                                                                    ProgressDialog.Show("Deleteing SimDescriptions...", false);

                                                                                    //Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Pause, false);
                                                                                    NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                    NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                }

                                                                                //NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                mposition = target.Position;
                                                                            }

                                                                            catch
                                                                            { }

                                                                            if (target.LotCurrent.IsWorldLot)
                                                                            {
                                                                                try
                                                                                {
                                                                                    Sim activeHouseholdselectedActor = Sim.ActiveActor;
                                                                                    if (activeHouseholdselectedActor != null)
                                                                                    {
                                                                                        Mailbox mailboxOnHomeLot = Mailbox.GetMailboxOnHomeLot(activeHouseholdselectedActor);
                                                                                        if (mailboxOnHomeLot != null)
                                                                                        {
                                                                                            Vector3 vector2;
                                                                                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(mailboxOnHomeLot.Position);
                                                                                            fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                                                                            fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                                                                                            if (!GlobalFunctions.FindGoodLocation(target, fglParams, out mposition, out vector2))
                                                                                            {
                                                                                                target.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                                                                                mposition = target.Position;
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }



                                                                            }
                                                                            else
                                                                            {
                                                                                mposition = target.Position;
                                                                            }

                                                                            if (target.LotCurrent.IsWorldLot)
                                                                            {
                                                                                target.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                                                                mposition = target.Position;
                                                                            }
                                                                            try
                                                                            {

                                                                                //Sleep(3.0);
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Contains(sdtyf))
                                                                                            Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                //Sleep(3.0);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!(allService is GrimReaper))
                                                                                        {
                                                                                            allService.ClearServicePool();
                                                                                        }

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                //Sleep(3.0);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!(allService is GrimReaper))
                                                                                        {
                                                                                            Service.Destroy(allService);
                                                                                        }

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            //Sleep(3.0);
                                                                            try
                                                                            {
                                                                                PlumbBob.ForceSelectActor(null);
                                                                            }
                                                                            catch
                                                                            { }





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
                                                                                        try
                                                                                        {
                                                                                            NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(simaue);
                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                        try
                                                                                        {
                                                                                            simaue.Genealogy.ClearAllGenealogyInformation();

                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                        try
                                                                                        {
                                                                                            simaue.Genealogy.ClearMiniSimDescription();
                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                        try
                                                                                        {
                                                                                            simaue.Destroy();
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
                                                                            finally
                                                                            {

                                                                                try
                                                                                {
                                                                                    asdo.Clear();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                asdo = null;
                                                                            }












                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {
                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        //Sleep(0.0);
                                                                                        NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(description.CreatedSim);
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearMiniSimDescription();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearDerivedData();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearSimDescription();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearAllGenealogyInformation();
                                                                                }
                                                                                catch
                                                                                { }




                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                /*
                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }
                                                                                */
                                                                                try
                                                                                {
                                                                                    while (true)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    MiniSimDescription.RemoveMSD(description.SimDescriptionId);
                                                                                }
                                                                                catch
                                                                                { }



                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }

                                                                            }

                                                                            try
                                                                            {
                                                                                try
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        SimDescription.sLoadedSimDescriptions.Clear();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    SimDescription.sLoadedSimDescriptions = null;
                                                                                    SimDescription.sLoadedSimDescriptions = new List<SimDescription>();

                                                                                }
                                                                                catch
                                                                                {
                                                                                    StyledNotification.Format afra = new StyledNotification.Format("Failed 1", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                    StyledNotification.Show(afra);
                                                                                }
                                                                                try
                                                                                {
                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();

                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions = null;
                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions = new List<SimDescription>();
                                                                                }
                                                                                catch
                                                                                {
                                                                                    StyledNotification.Format afra = new StyledNotification.Format("Failed 2", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                    StyledNotification.Show(afra);
                                                                                }
                                                                                if (flagsae)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        //MiniSimDescription.Clear();
                                                                                        MiniSimDescription.sMiniSims.Clear();

                                                                                        MiniSimDescription.sMiniSims = null;
                                                                                        MiniSimDescription.sMiniSims = new Dictionary<ulong, MiniSimDescription>();
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                        StyledNotification.Format afra = new StyledNotification.Format("Failed 3", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                        StyledNotification.Show(afra);
                                                                                    }
                                                                                }
                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                        StyledNotification.Format afra = new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kGameMessageNegative);
                                                                                        afra.mTNSCategory = NotificationManager.TNSCategory.Chatty;
                                                                                        //StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                        StyledNotification.Show(afra);
                                                                                    }
                                                                                    catch
                                                                                    { }



                                                                                    try
                                                                                    {
                                                                                        if (sdae != null)
                                                                                        {
                                                                                            Nra.NFinalizeDeath.mResetError = false;
                                                                                            Simulator.DestroyObject(sdae);
                                                                                        }
                                                                                    }
                                                                                    catch
                                                                                    { }
                                                                                    if (flagsaea)
                                                                                    {
                                                                                        try
                                                                                        {

                                                                                            Sim sim = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                            if (sim != null)
                                                                                            {

                                                                                                try
                                                                                                {
                                                                                                    //sim.SimDescription.Household.SetName("E3Lesa is Good Good Household");
                                                                                                    sim.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                                                                                    PlumbBob.ForceSelectActor(sim);
                                                                                                    //Service.NeedsAssignment(target.LotCurrent);
                                                                                                    try
                                                                                                    {
                                                                                                        if (sim.SimDescription.Child || sim.SimDescription.Teen)
                                                                                                        {
                                                                                                            sim.SimDescription.AssignSchool();
                                                                                                        }

                                                                                                    }
                                                                                                    catch
                                                                                                    { }
                                                                                                    try
                                                                                                    {
                                                                                                        if (sim.IsSelectable)
                                                                                                        {
                                                                                                            sim.OnBecameSelectable();
                                                                                                        }
                                                                                                    }
                                                                                                    catch
                                                                                                    { }

                                                                                                }
                                                                                                catch (Exception ex2)
                                                                                                {
                                                                                                    NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                                }
                                                                                                try
                                                                                                {
                                                                                                    if (sim.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                    {
                                                                                                        sim.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                        sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                                    }
                                                                                                    sim.AddInitialObjects(sim.IsSelectable);
                                                                                                }
                                                                                                catch
                                                                                                { }

                                                                                                if (flagsa)
                                                                                                {
                                                                                                    ProgressDialog.Close();
                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + sim.SimDescription.LastName;

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }

                                                                                            if (NiecMod.Helpers.Create.CreateActiveHouseholdAndActiveActor())
                                                                                            {
                                                                                                if (flagsa)
                                                                                                {

                                                                                                    try
                                                                                                    {
                                                                                                        ProgressDialog.Close();
                                                                                                    }
                                                                                                    catch
                                                                                                    { }
                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + PlumbBob.Singleton.mSelectedActor.SimDescription.LastName;

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }











                                                                                            //else if (Sim.ActiveActor == null)
                                                                                            //if (Sim.ActiveActor == null)
                                                                                            else if (PlumbBob.Singleton.mSelectedActor == null)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                    if (flagsa)
                                                                                                    {

                                                                                                        try
                                                                                                        {
                                                                                                            ProgressDialog.Close();
                                                                                                        }
                                                                                                        catch
                                                                                                        { }

                                                                                                    }
                                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                    if (optionsModel != null)
                                                                                                    {
                                                                                                        optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }
                                                                                                finally
                                                                                                {
                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                }


                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                if (flagsa)
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        ProgressDialog.Close();
                                                                                                    }
                                                                                                    catch
                                                                                                    { }

                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                CommandSystem.ExecuteCommandString("makesim");

                                                                                                if (flagsa)
                                                                                                {
                                                                                                    ProgressDialog.Close();
                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            finally
                                                                                            {
                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                        if (flagsa)
                                                                                        {
                                                                                            ProgressDialog.Close();
                                                                                        }
                                                                                        NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                        if (optionsModel != null)
                                                                                        {
                                                                                            optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                        }
                                                                                        NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                        try
                                                                                        {
                                                                                            GameStates.TransitionToEditTown();
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }







                                                                                }
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                NiecException.PrintMessage("Force Delete All SimDescriptions? Clear Keep Save Not Active " + ex.Message);
                                                                                if (!(ex is ResetException))
                                                                                {
                                                                                    Common.Exception("Force Delete All SimDescriptions? Clear Keep Save Not Active", ex);
                                                                                }
                                                                                if (flagsa)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        ProgressDialog.Close();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }

                                                                                if (Sim.ActiveActor == null)
                                                                                {
                                                                                    Sim simar = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                    if (simar != null)
                                                                                    {

                                                                                        try
                                                                                        {
                                                                                            //sim.SimDescription.Household.SetName(/* "E3Lesa is Good" */ "Good Household");
                                                                                            simar.SimDescription.Household.SetName(simar.SimDescription.LastName);
                                                                                            PlumbBob.ForceSelectActor(simar);
                                                                                        }
                                                                                        catch (Exception ex2)
                                                                                        {
                                                                                            NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                        }
                                                                                        try
                                                                                        {
                                                                                            if (simar.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                            {
                                                                                                simar.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                simar.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                            }
                                                                                            simar.AddInitialObjects(simar.IsSelectable);
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }
                                                                                }
                                                                                return false;
                                                                            }
                                                                            Simulator.YieldingDisabled = false;
                                                                            if (flagsaea)
                                                                            {
                                                                                Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed = currentGameSpeed;
                                                                            }


                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Not UsStone?"))
                                                                        {
                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    try
                                                                                    {
                                                                                        if (description.IsGhost && description.DeathStyle != SimDescription.DeathType.None) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                    {
                                                                                        continue;
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                                                                    listr.Add(SimDescription.DeathType.Drown);
                                                                                    listr.Add(SimDescription.DeathType.Starve);
                                                                                    listr.Add(SimDescription.DeathType.Thirst);
                                                                                    listr.Add(SimDescription.DeathType.Burn);
                                                                                    listr.Add(SimDescription.DeathType.Freeze);
                                                                                    listr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                    listr.Add(SimDescription.DeathType.Shark);
                                                                                    listr.Add(SimDescription.DeathType.Jetpack);
                                                                                    listr.Add(SimDescription.DeathType.Meteor);
                                                                                    listr.Add(SimDescription.DeathType.Causality);
                                                                                    listr.Add(SimDescription.DeathType.Electrocution);
                                                                                    if (description.Elder)
                                                                                    {
                                                                                        listr.Add(SimDescription.DeathType.OldAge);
                                                                                    }
                                                                                    listr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);

                                                                                    Urnstone mGravebackup = HelperNra.TFindGhostsGrave(description);
                                                                                    if (mGravebackup != null)
                                                                                    {
                                                                                        if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(target.Position), false))
                                                                                        {
                                                                                            mGravebackup.SetPosition(target.Position);
                                                                                        }
                                                                                        mGravebackup.OnHandToolMovement();
                                                                                    }



                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                    if (description.DeathStyle == SimDescription.DeathType.None)
                                                                                    {
                                                                                        if (description.IsHuman)
                                                                                        {
                                                                                            switch (randomObjectFromList)
                                                                                            {
                                                                                                case SimDescription.DeathType.None:
                                                                                                case SimDescription.DeathType.PetOldAgeBad:
                                                                                                case SimDescription.DeathType.PetOldAgeGood:
                                                                                                    randomObjectFromList = SimDescription.DeathType.OldAge;
                                                                                                    break;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            switch (randomObjectFromList)
                                                                                            {
                                                                                                case SimDescription.DeathType.None:
                                                                                                case SimDescription.DeathType.OldAge:
                                                                                                    randomObjectFromList = SimDescription.DeathType.PetOldAgeGood;
                                                                                                    break;
                                                                                            }
                                                                                        }

                                                                                        description.SetDeathStyle(randomObjectFromList, true);
                                                                                    }

                                                                                    description.IsNeverSelectable = true;
                                                                                    description.Contactable = false;
                                                                                    description.Marryable = false;


                                                                                    if (description.CreatedSim != null && description.CreatedSim == PlumbBob.SelectedActor)
                                                                                    {
                                                                                        LotManager.SelectNextSim();
                                                                                    }
                                                                                    if (description.CareerManager != null)
                                                                                    {
                                                                                        description.CareerManager.LeaveAllJobs(Career.LeaveJobReason.kDied);
                                                                                    }

                                                                                    mGravebackup.OnHandToolMovement();

                                                                                    int num = ((int)Math.Floor((double)SimClock.ConvertFromTicks(SimClock.CurrentTime().Ticks, TimeUnit.Minutes))) % 60;
                                                                                    mGravebackup.MinuteOfDeath = num;

                                                                                    try
                                                                                    {
                                                                                        Urnstone.FinalizeSimDeath(description, household);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                                catch
                                                                                { }

                                                                            }
                                                                            //return true;
                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Done UsStone?"))
                                                                        {
                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    try
                                                                                    {
                                                                                        if (description.IsGhost && description.DeathStyle != SimDescription.DeathType.None) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                    {
                                                                                        continue;
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                                                                    listr.Add(SimDescription.DeathType.Drown);
                                                                                    listr.Add(SimDescription.DeathType.Starve);
                                                                                    listr.Add(SimDescription.DeathType.Thirst);
                                                                                    listr.Add(SimDescription.DeathType.Burn);
                                                                                    listr.Add(SimDescription.DeathType.Freeze);
                                                                                    listr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                    listr.Add(SimDescription.DeathType.Shark);
                                                                                    listr.Add(SimDescription.DeathType.Jetpack);
                                                                                    listr.Add(SimDescription.DeathType.Meteor);
                                                                                    listr.Add(SimDescription.DeathType.Causality);
                                                                                    listr.Add(SimDescription.DeathType.Electrocution);
                                                                                    if (description.Elder)
                                                                                    {
                                                                                        listr.Add(SimDescription.DeathType.OldAge);
                                                                                    }
                                                                                    listr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);
                                                                                    //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                    //simDeathType = randomObjectFromList;
                                                                                    Urnstones.CreateGrave(description, randomObjectFromList, false, true);
                                                                                }
                                                                                catch
                                                                                { }

                                                                            }
                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Not NiecSimDesc"))
                                                                        {
                                                                            List<SimDescription> lisyet = new List<SimDescription>();
                                                                            foreach (SimDescription sd in NFinalizeDeath.TattoaX())
                                                                            {
                                                                                if (sd.Household != Household.ActiveHousehold && !(sd.Service is GrimReaper) && !sd.IsDead && !sd.IsGhost) //OK
                                                                                {
                                                                                    lisyet.Add(sd);
                                                                                }
                                                                            }
                                                                            if (list.Count > 0)
                                                                            {
                                                                                foreach (SimDescription description in lisyet)
                                                                                {

                                                                                    try
                                                                                    {
                                                                                        //Name is
                                                                                        if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                        {
                                                                                            continue;
                                                                                        }

                                                                                        if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                        {
                                                                                            continue;
                                                                                        }

                                                                                        if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                        {
                                                                                            continue;
                                                                                        }
                                                                                    }
                                                                                    catch (NullReferenceException)
                                                                                    { }

                                                                                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                                                                    listr.Add(SimDescription.DeathType.Drown);
                                                                                    listr.Add(SimDescription.DeathType.Starve);
                                                                                    listr.Add(SimDescription.DeathType.Thirst);
                                                                                    listr.Add(SimDescription.DeathType.Burn);
                                                                                    listr.Add(SimDescription.DeathType.Freeze);
                                                                                    listr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                    listr.Add(SimDescription.DeathType.Shark);
                                                                                    listr.Add(SimDescription.DeathType.Jetpack);
                                                                                    listr.Add(SimDescription.DeathType.Meteor);
                                                                                    listr.Add(SimDescription.DeathType.Causality);
                                                                                    listr.Add(SimDescription.DeathType.Electrocution);
                                                                                    if (target.SimDescription.Elder)
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
                                                                                                                    foreach (SimDescription iteme in listxt)
                                                                                                                    {
                                                                                                                        Sim sim = iteme.CreatedSim;
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

                                                                                                        var easdasdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                                                                                        if (easdasdoei != null)
                                                                                                        {
                                                                                                            // Add New
                                                                                                            if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                                                                                            {
                                                                                                                Lot lotlasdoot = mSim.LotCurrent;
                                                                                                                if (lotlasdoot != null && lotlasdoot.IsDivingLot)
                                                                                                                {
                                                                                                                    ScubaDiving scubaDivinga = new ScubaDiving(easdasdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
                                                                                                                    if (scubaDivinga != null)
                                                                                                                    {
                                                                                                                        mSim.Posture = scubaDivinga;
                                                                                                                        scubaDivinga.StartBubbleEffects();
                                                                                                                    }
                                                                                                                }
                                                                                                                else if (GameUtils.IsInstalled(ProductVersion.EP10))
                                                                                                                {
                                                                                                                    try
                                                                                                                    {
                                                                                                                        bool Autonomasdasdous = true;
                                                                                                                        if (Autonomasdasdous || target.IsNPC) return false;
                                                                                                                        if (!AcceptCancelDialog.Show("Death Good System: Are You Sure Get Sim Descriptions In World Kill Sim?"))
                                                                                                                        {
                                                                                                                            if (AcceptCancelDialog.Show("Death Good System: CleanUp Dead SimDescription?"))
                                                                                                                            {
                                                                                                                                List<SimDescription> msime = new List<SimDescription>();
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                                                                                                                                    {
                                                                                                                                        if (urnstone.DeadSimsDescription != null && !msime.Contains(urnstone.DeadSimsDescription))
                                                                                                                                        {
                                                                                                                                            msime.Add(urnstone.DeadSimsDescription);
                                                                                                                                        }
                                                                                                                                    }





                                                                                                                                    foreach (SimDescription murnstone in msime)
                                                                                                                                    {
                                                                                                                                        if (murnstone == null) continue;
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            if (murnstone.Household == Household.ActiveHousehold) continue;
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }

                                                                                                                                        try
                                                                                                                                        {



                                                                                                                                            if (murnstone.IsDead)
                                                                                                                                            {
                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    if (murnstone.CreatedSim != null)
                                                                                                                                                    {
                                                                                                                                                        try
                                                                                                                                                        {
                                                                                                                                                            NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(murnstone.CreatedSim);
                                                                                                                                                        }
                                                                                                                                                        catch
                                                                                                                                                        { }
                                                                                                                                                        try
                                                                                                                                                        {
                                                                                                                                                            murnstone.CreatedSim.Genealogy.ClearAllGenealogyInformation();

                                                                                                                                                        }
                                                                                                                                                        catch
                                                                                                                                                        { }

                                                                                                                                                        try
                                                                                                                                                        {
                                                                                                                                                            murnstone.CreatedSim.Genealogy.ClearMiniSimDescription();
                                                                                                                                                        }
                                                                                                                                                        catch
                                                                                                                                                        { }
                                                                                                                                                        try
                                                                                                                                                        {
                                                                                                                                                            murnstone.CreatedSim.Destroy();
                                                                                                                                                        }
                                                                                                                                                        catch
                                                                                                                                                        { }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }










                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    murnstone.Genealogy.ClearAllGenealogyInformation();
                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }

                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    Household household = murnstone.Household;
                                                                                                                                                    if (household != null)
                                                                                                                                                    {
                                                                                                                                                        household.Remove(murnstone, !household.IsSpecialHousehold);
                                                                                                                                                    }

                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }






                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    while (true)
                                                                                                                                                    {
                                                                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(murnstone);
                                                                                                                                                        if (urnstone != null)
                                                                                                                                                        {
                                                                                                                                                            try
                                                                                                                                                            {
                                                                                                                                                                urnstone.DeadSimsDescription = null;
                                                                                                                                                                urnstone.Destroy();
                                                                                                                                                            }
                                                                                                                                                            catch
                                                                                                                                                            { }

                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            break;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }
                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    murnstone.Dispose();
                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }










                                                                                                                                            }




                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }
                                                                                                                                    }



                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }


                                                                                                                                return true;
                                                                                                                            }
                                                                                                                            return false;
                                                                                                                        }
                                                                                                                        bool flasdfsdfg = false;

                                                                                                                        if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0)
                                                                                                                        {
                                                                                                                            try
                                                                                                                            {
                                                                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (!Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Contains(sdtyf))
                                                                                                                                            Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            flasdfsdfg = true;
                                                                                                                            if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0) return true;
                                                                                                                        }

                                                                                                                        if (!flasdfsdfg && AcceptCancelDialog.Show("Do you Are Add All SimDescriptions?"))
                                                                                                                        {
                                                                                                                            try
                                                                                                                            {
                                                                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                        }
                                                                                                                        if (AcceptCancelDialog.Show("Do you Are Delete All SimDescriptions?"))
                                                                                                                        {

                                                                                                                            foreach (SimDescription desasdcription in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                            {

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    if (desasdcription == null) continue;
                                                                                                                                    //Name is
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (desasdcription.Household == Household.ActiveHousehold) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }


                                                                                                                                    if (desasdcription.FirstName == "Death" && desasdcription.LastName == "Good System")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                    if (desasdcription.FirstName == "Good System" && desasdcription.LastName == "Death Helper")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                    if (desasdcription.FirstName == "Grim" && desasdcription.LastName == "Reaper")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Household household = desasdcription.Household;
                                                                                                                                    if (desasdcription.CreatedSim != null)
                                                                                                                                    {
                                                                                                                                        desasdcription.CreatedSim.Destroy();
                                                                                                                                    }
                                                                                                                                    if (household != null)
                                                                                                                                    {
                                                                                                                                        household.Remove(desasdcription, !household.IsSpecialHousehold);
                                                                                                                                    }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    desasdcription.Dispose();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    for (int i = 0; i < 10; i++)
                                                                                                                                    {
                                                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(desasdcription);
                                                                                                                                        if (urnstone != null)
                                                                                                                                        {
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Dispose();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Destroy();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }

                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            break;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(desasdcription);
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                            }



                                                                                                                        }
                                                                                                                        else if (AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions?"))
                                                                                                                        {
                                                                                                                            try
                                                                                                                            {
                                                                                                                                ProgressDialog.Show("Processing", false);
                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {

                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                foreach (Service allService in Services.AllServices)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        allService.ClearServicePool();
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                foreach (Service allService in Services.AllServices)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        Service.Destroy(allService);
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }

                                                                                                                            foreach (SimDescription descriptasdwerion in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                            {

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    if (descriptasdwerion == null) continue;
                                                                                                                                    //Name is
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (descriptasdwerion.Household == Household.ActiveHousehold) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Household household = descriptasdwerion.Household;
                                                                                                                                    if (descriptasdwerion.CreatedSim != null)
                                                                                                                                    {
                                                                                                                                        descriptasdwerion.CreatedSim.Destroy();
                                                                                                                                    }
                                                                                                                                    if (household != null)
                                                                                                                                    {
                                                                                                                                        MidlifeCrisisManager.OnSimDied(target.SimDescription);
                                                                                                                                        Urnstone.FinalizeSimDeathRelationships(target.SimDescription, 0f);
                                                                                                                                        if (target.SimDescription.CareerManager != null)
                                                                                                                                        {
                                                                                                                                            target.SimDescription.CareerManager.LeaveAllJobs(Career.LeaveJobReason.kDied);
                                                                                                                                        }
                                                                                                                                        RockBand skill = target.SimDescription.SkillManager.GetSkill<RockBand>(SkillNames.RockBand);
                                                                                                                                        if (skill != null)
                                                                                                                                        {
                                                                                                                                            skill.BandMemberDied();
                                                                                                                                        }
                                                                                                                                        target.SimDescription.Contactable = false;
                                                                                                                                        target.SimDescription.Marryable = false;
                                                                                                                                        Urnstone.CheckForAbandonedChildren(target.SimDescription, null, true);
                                                                                                                                        if (target.SimDescription.DeathStyle == SimDescription.DeathType.MummyCurse && !target.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                                                        {
                                                                                                                                            TraitManager traitManager = target.SimDescription.TraitManager;
                                                                                                                                            if (traitManager.HasElement(TraitNames.Good))
                                                                                                                                            {
                                                                                                                                                traitManager.RemoveElement(TraitNames.Good);
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Trait randomElement = traitManager.GetRandomElement();
                                                                                                                                                if (randomElement != null)
                                                                                                                                                {
                                                                                                                                                    Vector3 forwardVector = target.ForwardVector;
                                                                                                                                                    Lot lotCurrent = target.LotCurrent;
                                                                                                                                                    if (lotCurrent == Household.ActiveHouseholdLot || target.Position != forwardVector || target.SimInRabbitHolePosture)
                                                                                                                                                    {
                                                                                                                                                        return false;
                                                                                                                                                    }
                                                                                                                                                    int num = 0;
                                                                                                                                                    if (lotCurrent.IsWorldLot)
                                                                                                                                                    {
                                                                                                                                                        if (target.IsSelectable)
                                                                                                                                                        {
                                                                                                                                                            
                                                                                                                                                            Vector3 position = target.Position;
                                                                                                                                                            Vector3[] array = null;
                                                                                                                                                            Vector3[] array2 = array;
                                                                                                                                                            foreach (Vector3 v in array2)
                                                                                                                                                            {
                                                                                                                                                                Quaternion q = Quaternion.MakeFromForwardVector(forwardVector);
                                                                                                                                                                Vector3 vec = Quaternion.VRotate(q, v);
                                                                                                                                                                Vector3 destPosition = position + 2f * vec;
                                                                                                                                                                Route route = target.CreateRoute();
                                                                                                                                                                if (route.PlanToPoint(destPosition).Succeeded())
                                                                                                                                                                {
                                                                                                                                                                    return false;
                                                                                                                                                                }
                                                                                                                                                                num++;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        for (int j = 0; j < Sim.kNumberOfPlanAttemptsForStuckSim; j++)
                                                                                                                                                        {
                                                                                                                                                            Vector3 randomPosition = lotCurrent.GetRandomPosition(false, true, TerrainType.WorldTerrain | TerrainType.WorldSea | TerrainType.LotTerrain);
                                                                                                                                                            if (!(randomPosition == Vector3.Invalid))
                                                                                                                                                            {
                                                                                                                                                                Route route2 = target.CreateRoute();
                                                                                                                                                                if (route2.PlanToPoint(randomPosition).Succeeded())
                                                                                                                                                                {
                                                                                                                                                                    return false;
                                                                                                                                                                }
                                                                                                                                                                num++;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    if (num >= 3)
                                                                                                                                                    {
                                                                                                                                                        target.SetObjectToReset();
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        Vector3 randomPosition = lotCurrent.GetRandomPosition(false, true, TerrainType.WorldTerrain | TerrainType.WorldSea | TerrainType.LotTerrain);
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            traitManager.AddElement(TraitNames.Evil);
                                                                                                                                        }
                                                                                                                                        else if (target.SimDescription.DeathStyle == SimDescription.DeathType.PetOldAgeBad)
                                                                                                                                        {
                                                                                                                                            TraitManager traitManager2 = target.SimDescription.TraitManager;
                                                                                                                                            if (!traitManager2.HasElement(TraitNames.NoisyPet))
                                                                                                                                            {
                                                                                                                                                if (traitManager2.HasElement(TraitNames.QuietPet))
                                                                                                                                                {
                                                                                                                                                    traitManager2.RemoveElement(TraitNames.QuietPet);
                                                                                                                                                }
                                                                                                                                                traitManager2.AddElement(TraitNames.NoisyPet);
                                                                                                                                            }
                                                                                                                                            if (!traitManager2.HasElement(TraitNames.AggressivePet))
                                                                                                                                            {
                                                                                                                                                if (traitManager2.HasElement(TraitNames.FriendlyPet))
                                                                                                                                                {
                                                                                                                                                    traitManager2.RemoveElement(TraitNames.FriendlyPet);
                                                                                                                                                }
                                                                                                                                                traitManager2.AddElement(TraitNames.AggressivePet);
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        MiniSimDescription miniSimDescription = MiniSimDescription.Find(target.SimDescription.SimDescriptionId);
                                                                                                                                        if (miniSimDescription != null)
                                                                                                                                        {
                                                                                                                                            miniSimDescription.UpdateForLocalization(target.SimDescription);
                                                                                                                                        }
                                                                                                                                        EventTracker.SendEvent(new SimDescriptionEvent(EventTypeId.kSimDied, target.SimDescription));
                                                                                                                                    }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptasdwerion.Genealogy.ClearAllGenealogyInformation();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptasdwerion.Dispose();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    for (int i = 0; i < 20; i++)
                                                                                                                                    {
                                                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(descriptasdwerion);
                                                                                                                                        if (urnstone != null)
                                                                                                                                        {
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Dispose();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Destroy();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }

                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            break;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(descriptasdwerion);
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                            }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                ProgressDialog.Close();
                                                                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                                                                {
                                                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                    StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                    if (optionsModel != null)
                                                                                                                                    {
                                                                                                                                        optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                                                                        PlumbBob.ForceSelectActor(null);
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                            if (optionsModel != null)
                                                                                                                                            {
                                                                                                                                                optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }

                                                                                                                                    }
                                                                                                                                    finally
                                                                                                                                    { PlumbBob.ForceSelectActor(target); }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                        }
                                                                                                                        else if (AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions? Clear Keep Save"))
                                                                                                                        {
                                                                                                                            try
                                                                                                                            {
                                                                                                                                ProgressDialog.Show("Deleteing SimDescription...", false);
                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {

                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                foreach (Service allService in Services.AllServices)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        allService.ClearServicePool();
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                foreach (Service allService in Services.AllServices)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        Service.Destroy(allService);
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                            foreach (SimDescription descripwerwertion in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                            {

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    if (descripwerwertion == null) continue;
                                                                                                                                    //Name is
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (descripwerwertion.Household == Household.ActiveHousehold) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Household household = descripwerwertion.Household;
                                                                                                                                    if (descripwerwertion.CreatedSim != null)
                                                                                                                                    {
                                                                                                                                        descripwerwertion.CreatedSim.Destroy();
                                                                                                                                    }
                                                                                                                                    if (household != null)
                                                                                                                                    {
                                                                                                                                        household.Remove(descripwerwertion, !household.IsSpecialHousehold);
                                                                                                                                    }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                /*
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    description.Dispose();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                */
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    while (true)
                                                                                                                                    {
                                                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(descripwerwertion);
                                                                                                                                        if (urnstone != null)
                                                                                                                                        {
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Dispose();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Destroy();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }

                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            break;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(descripwerwertion);
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                            }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }


                                                                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                                                                {
                                                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                    StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                    if (optionsModel != null)
                                                                                                                                    {
                                                                                                                                        optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                                                        ProgressDialog.Close();
                                                                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {

                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        ProgressDialog.Close();
                                                                                                                                        StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                                                                        PlumbBob.ForceSelectActor(null);
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                            if (optionsModel != null)
                                                                                                                                            {
                                                                                                                                                optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }

                                                                                                                                    }
                                                                                                                                    finally
                                                                                                                                    { PlumbBob.ForceSelectActor(target); }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                        }
                                                                                                                        else if (PlumbBob.Singleton.mSelectedActor != null && NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore") && AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions? Clear Keep Save Not Active"))
                                                                                                                        {
                                                                                                                            bool flagsae = AcceptCancelDialog.Show("Delete All MiniSimDescription?");
                                                                                                                            bool flagsaea = !AcceptCancelDialog.Show("No make sim?");
                                                                                                                            bool flagsa = AcceptCancelDialog.Show("ProgressDialog?");
                                                                                                                            Lot mlot = target.LotCurrent;
                                                                                                                            //Simulator.YieldingDisabled = true;
                                                                                                                            //NiecTask mtask = new NiecTask.AddToSimulator();
                                                                                                                            Nra.NFinalizeDeath.mResetError = true;
                                                                                                                            ObjectGuid sdae = Sims3.NiecHelp.Tasks.NiecTask.Perform(Nra.NFinalizeDeath.ResetError);
                                                                                                                            Sims3.SimIFace.Gameflow.GameSpeed currentGameSpeed = Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed;
                                                                                                                            Vector3 mposition = Vector3.Invalid;



                                                                                                                            try
                                                                                                                            {
                                                                                                                                CommandSystem.ExecuteCommandString("dgsunsafekill false");
                                                                                                                                CommandSystem.ExecuteCommandString("dgspx false");
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }




                                                                                                                            try
                                                                                                                            {
                                                                                                                                if (flagsa)
                                                                                                                                {
                                                                                                                                    //ProgressDialog.Show("Deleteing SimDescription...", ModalDialog.PauseMode.PauseSimulator, true, new Vector2(-1f, -1f), true, false, false);
                                                                                                                                    //GameUtils.SetGameTimeSpeedLevel(0);

                                                                                                                                    Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed = Sims3.SimIFace.Gameflow.GameSpeed.Pause;
                                                                                                                                    ProgressDialog.Show("Deleteing SimDescriptions...", false);

                                                                                                                                    //Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Pause, false);
                                                                                                                                    NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                                                                    NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                }

                                                                                                                                //NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                                                                mposition = target.Position;
                                                                                                                            }

                                                                                                                            catch
                                                                                                                            { }

                                                                                                                            if (target.LotCurrent.IsWorldLot)
                                                                                                                            {
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Sim activeHouseholdselectedActor = Sim.ActiveActor;
                                                                                                                                    if (activeHouseholdselectedActor != null)
                                                                                                                                    {
                                                                                                                                        Mailbox mailboxOnHomeLot = Mailbox.GetMailboxOnHomeLot(activeHouseholdselectedActor);
                                                                                                                                        if (mailboxOnHomeLot != null)
                                                                                                                                        {
                                                                                                                                            Vector3 vector2;
                                                                                                                                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(mailboxOnHomeLot.Position);
                                                                                                                                            fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                                                                                                                            fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                                                                                                                                            if (!GlobalFunctions.FindGoodLocation(target, fglParams, out mposition, out vector2))
                                                                                                                                            {
                                                                                                                                                target.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                                                                                                                                mposition = target.Position;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }



                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                mposition = target.Position;
                                                                                                                            }

                                                                                                                            if (target.LotCurrent.IsWorldLot)
                                                                                                                            {
                                                                                                                                target.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                                                                                                                mposition = target.Position;
                                                                                                                            }
                                                                                                                            try
                                                                                                                            {

                                                                                                                                //Sleep(3.0);
                                                                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (!Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Contains(sdtyf))
                                                                                                                                            Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                //Sleep(3.0);
                                                                                                                                foreach (Service allService in Services.AllServices)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (!(allService is GrimReaper))
                                                                                                                                        {
                                                                                                                                            allService.ClearServicePool();
                                                                                                                                        }

                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            try
                                                                                                                            {
                                                                                                                                //Sleep(3.0);
                                                                                                                                foreach (Service allService in Services.AllServices)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (!(allService is GrimReaper))
                                                                                                                                        {
                                                                                                                                            Service.Destroy(allService);
                                                                                                                                        }

                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }
                                                                                                                            //Sleep(3.0);
                                                                                                                            try
                                                                                                                            {
                                                                                                                                PlumbBob.ForceSelectActor(null);
                                                                                                                            }
                                                                                                                            catch
                                                                                                                            { }





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
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(simaue);
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            simaue.Genealogy.ClearAllGenealogyInformation();

                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }

                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            simaue.Genealogy.ClearMiniSimDescription();
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            simaue.Destroy();
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
                                                                                                                            finally
                                                                                                                            {

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    asdo.Clear();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                asdo = null;
                                                                                                                            }












                                                                                                                            foreach (SimDescription descriptisdfsdfewron in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                            {
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    if (descriptisdfsdfewron == null) continue;
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    if (descriptisdfsdfewron.CreatedSim != null)
                                                                                                                                    {
                                                                                                                                        //Sleep(0.0);
                                                                                                                                        NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(descriptisdfsdfewron.CreatedSim);
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptisdfsdfewron.Genealogy.ClearMiniSimDescription();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptisdfsdfewron.Genealogy.ClearDerivedData();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptisdfsdfewron.Genealogy.ClearSimDescription();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptisdfsdfewron.Genealogy.ClearAllGenealogyInformation();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }




                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Household household = descriptisdfsdfewron.Household;
                                                                                                                                    if (household != null)
                                                                                                                                    {
                                                                                                                                        household.Remove(descriptisdfsdfewron, !household.IsSpecialHousehold);
                                                                                                                                    }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                /*
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    description.Dispose();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                */
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    while (true)
                                                                                                                                    {
                                                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(descriptisdfsdfewron);
                                                                                                                                        if (urnstone != null)
                                                                                                                                        {
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Dispose();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                urnstone.Destroy();
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }

                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            break;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    MiniSimDescription.RemoveMSD(descriptisdfsdfewron.SimDescriptionId);
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }



                                                                                                                                try
                                                                                                                                {
                                                                                                                                    descriptisdfsdfewron.Dispose();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(descriptisdfsdfewron);
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                            }

                                                                                                                            try
                                                                                                                            {
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        SimDescription.sLoadedSimDescriptions.Clear();
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                    SimDescription.sLoadedSimDescriptions = null;
                                                                                                                                    SimDescription.sLoadedSimDescriptions = new List<SimDescription>();

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                {
                                                                                                                                    StyledNotification.Format afra = new StyledNotification.Format("Failed 1", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                                                                    StyledNotification.Show(afra);
                                                                                                                                }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();

                                                                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions = null;
                                                                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions = new List<SimDescription>();
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                {
                                                                                                                                    StyledNotification.Format afra = new StyledNotification.Format("Failed 2", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                                                                    StyledNotification.Show(afra);
                                                                                                                                }
                                                                                                                                if (flagsae)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        //MiniSimDescription.Clear();
                                                                                                                                        MiniSimDescription.sMiniSims.Clear();

                                                                                                                                        MiniSimDescription.sMiniSims = null;
                                                                                                                                        MiniSimDescription.sMiniSims = new Dictionary<ulong, MiniSimDescription>();
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    {
                                                                                                                                        StyledNotification.Format afra = new StyledNotification.Format("Failed 3", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                                                                        StyledNotification.Show(afra);
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                        StyledNotification.Format afra = new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kGameMessageNegative);
                                                                                                                                        afra.mTNSCategory = NotificationManager.TNSCategory.Chatty;
                                                                                                                                        //StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                                                                        StyledNotification.Show(afra);
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }



                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (sdae != null)
                                                                                                                                        {
                                                                                                                                            Nra.NFinalizeDeath.mResetError = false;
                                                                                                                                            Simulator.DestroyObject(sdae);
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }
                                                                                                                                    if (flagsaea)
                                                                                                                                    {
                                                                                                                                        try
                                                                                                                                        {

                                                                                                                                            Sim sim = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                                                                            if (sim != null)
                                                                                                                                            {

                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    //sim.SimDescription.Household.SetName("E3Lesa is Good Good Household");
                                                                                                                                                    sim.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                                                                                                                                    PlumbBob.ForceSelectActor(sim);
                                                                                                                                                    //Service.NeedsAssignment(target.LotCurrent);
                                                                                                                                                    try
                                                                                                                                                    {
                                                                                                                                                        if (sim.SimDescription.Child || sim.SimDescription.Teen)
                                                                                                                                                        {
                                                                                                                                                            sim.SimDescription.AssignSchool();
                                                                                                                                                        }

                                                                                                                                                    }
                                                                                                                                                    catch
                                                                                                                                                    { }
                                                                                                                                                    try
                                                                                                                                                    {
                                                                                                                                                        if (sim.IsSelectable)
                                                                                                                                                        {
                                                                                                                                                            sim.OnBecameSelectable();
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    catch
                                                                                                                                                    { }

                                                                                                                                                }
                                                                                                                                                catch (Exception ex2)
                                                                                                                                                {
                                                                                                                                                    NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                                                                                }
                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    if (sim.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                                                                    {
                                                                                                                                                        sim.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                                                                        sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                                                                                    }
                                                                                                                                                    sim.AddInitialObjects(sim.IsSelectable);
                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }

                                                                                                                                                if (flagsa)
                                                                                                                                                {
                                                                                                                                                    ProgressDialog.Close();
                                                                                                                                                }
                                                                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                if (optionsModel != null)
                                                                                                                                                {
                                                                                                                                                    optionsModel.SaveName = "ClearSave " + sim.SimDescription.LastName;

                                                                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                                                                }
                                                                                                                                            }

                                                                                                                                            if (NiecMod.Helpers.Create.CreateActiveHouseholdAndActiveActor())
                                                                                                                                            {
                                                                                                                                                if (flagsa)
                                                                                                                                                {

                                                                                                                                                    try
                                                                                                                                                    {
                                                                                                                                                        ProgressDialog.Close();
                                                                                                                                                    }
                                                                                                                                                    catch
                                                                                                                                                    { }
                                                                                                                                                }
                                                                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                if (optionsModel != null)
                                                                                                                                                {
                                                                                                                                                    optionsModel.SaveName = "ClearSave " + PlumbBob.Singleton.mSelectedActor.SimDescription.LastName;

                                                                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                                                                }
                                                                                                                                            }











                                                                                                                                            //else if (Sim.ActiveActor == null)
                                                                                                                                            //if (Sim.ActiveActor == null)
                                                                                                                                            else if (PlumbBob.Singleton.mSelectedActor == null)
                                                                                                                                            {
                                                                                                                                                try
                                                                                                                                                {
                                                                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                                                                    if (flagsa)
                                                                                                                                                    {

                                                                                                                                                        try
                                                                                                                                                        {
                                                                                                                                                            ProgressDialog.Close();
                                                                                                                                                        }
                                                                                                                                                        catch
                                                                                                                                                        { }

                                                                                                                                                    }
                                                                                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                    if (optionsModel != null)
                                                                                                                                                    {
                                                                                                                                                        optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                catch
                                                                                                                                                { }
                                                                                                                                                finally
                                                                                                                                                {
                                                                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                                }


                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                                if (flagsa)
                                                                                                                                                {
                                                                                                                                                    try
                                                                                                                                                    {
                                                                                                                                                        ProgressDialog.Close();
                                                                                                                                                    }
                                                                                                                                                    catch
                                                                                                                                                    { }

                                                                                                                                                }
                                                                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                if (optionsModel != null)
                                                                                                                                                {
                                                                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        {
                                                                                                                                            try
                                                                                                                                            {
                                                                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                                                                CommandSystem.ExecuteCommandString("makesim");

                                                                                                                                                if (flagsa)
                                                                                                                                                {
                                                                                                                                                    ProgressDialog.Close();
                                                                                                                                                }
                                                                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                if (optionsModel != null)
                                                                                                                                                {
                                                                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            catch
                                                                                                                                            { }
                                                                                                                                            finally
                                                                                                                                            {
                                                                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }
                                                                                                                                        if (flagsa)
                                                                                                                                        {
                                                                                                                                            ProgressDialog.Close();
                                                                                                                                        }
                                                                                                                                        NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                        if (optionsModel != null)
                                                                                                                                        {
                                                                                                                                            optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                        }
                                                                                                                                        NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            GameStates.TransitionToEditTown();
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }

                                                                                                                                    }







                                                                                                                                }
                                                                                                                            }
                                                                                                                            catch (Exception ex)
                                                                                                                            {
                                                                                                                                NiecException.PrintMessage("Force Delete All SimDescriptions? Clear Keep Save Not Active " + ex.Message);
                                                                                                                                if (!(ex is ResetException))
                                                                                                                                {
                                                                                                                                    Common.Exception("Force Delete All SimDescriptions? Clear Keep Save Not Active", ex);
                                                                                                                                }
                                                                                                                                if (flagsa)
                                                                                                                                {
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        ProgressDialog.Close();
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }

                                                                                                                                if (Sim.ActiveActor == null)
                                                                                                                                {
                                                                                                                                    Sim simar = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                                                                    if (simar != null)
                                                                                                                                    {

                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            //sim.SimDescription.Household.SetName(/* "E3Lesa is Good" */ "Good Household");
                                                                                                                                            simar.SimDescription.Household.SetName(simar.SimDescription.LastName);
                                                                                                                                            PlumbBob.ForceSelectActor(simar);
                                                                                                                                        }
                                                                                                                                        catch (Exception ex2)
                                                                                                                                        {
                                                                                                                                            NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                                                                        }
                                                                                                                                        try
                                                                                                                                        {
                                                                                                                                            if (simar.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                                                            {
                                                                                                                                                simar.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                                                                simar.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                                                                            }
                                                                                                                                            simar.AddInitialObjects(simar.IsSelectable);
                                                                                                                                        }
                                                                                                                                        catch
                                                                                                                                        { }

                                                                                                                                    }
                                                                                                                                }
                                                                                                                                return false;
                                                                                                                            }
                                                                                                                            Simulator.YieldingDisabled = false;
                                                                                                                            if (flagsaea)
                                                                                                                            {
                                                                                                                                Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed = currentGameSpeed;
                                                                                                                            }


                                                                                                                        }
                                                                                                                        else if (AcceptCancelDialog.Show("Not UsStone?"))
                                                                                                                        {
                                                                                                                            foreach (SimDescription descriptisdfsdfweron in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                            {

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    //Name is
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (descriptisdfsdfweron.Household == Household.ActiveHousehold) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (descriptisdfsdfweron.IsGhost && descriptisdfsdfweron.DeathStyle != SimDescription.DeathType.None) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                    if (descriptisdfsdfweron.FirstName == "Death" && descriptisdfsdfweron.LastName == "Good System")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                    if (descriptisdfsdfweron.FirstName == "Good System" && descriptisdfsdfweron.LastName == "Death Helper")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                    if (descriptisdfsdfweron.FirstName == "Grim" && descriptisdfsdfweron.LastName == "Reaper")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    List<SimDescription.DeathType> lisdfsdfwerstr = new List<SimDescription.DeathType>();
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Drown);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Starve);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Thirst);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Burn);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Freeze);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Shark);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Jetpack);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Meteor);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Causality);
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.Electrocution);
                                                                                                                                    if (descriptisdfsdfweron.Elder)
                                                                                                                                    {
                                                                                                                                        lisdfsdfwerstr.Add(SimDescription.DeathType.OldAge);
                                                                                                                                    }
                                                                                                                                    lisdfsdfwerstr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(lisdfsdfwerstr);

                                                                                                                                    Urnstone mGravebackup = HelperNra.TFindGhostsGrave(descriptisdfsdfweron);
                                                                                                                                    if (mGravebackup != null)
                                                                                                                                    {
                                                                                                                                        if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(target.Position), false))
                                                                                                                                        {
                                                                                                                                            mGravebackup.SetPosition(target.Position);
                                                                                                                                        }
                                                                                                                                        mGravebackup.OnHandToolMovement();
                                                                                                                                    }



                                                                                                                                    Household household = descriptisdfsdfweron.Household;
                                                                                                                                    if (descriptisdfsdfweron.CreatedSim != null)
                                                                                                                                    {
                                                                                                                                        descriptisdfsdfweron.CreatedSim.Destroy();
                                                                                                                                    }
                                                                                                                                    if (household != null)
                                                                                                                                    {
                                                                                                                                        household.Remove(descriptisdfsdfweron, !household.IsSpecialHousehold);
                                                                                                                                    }

                                                                                                                                    if (descriptisdfsdfweron.DeathStyle == SimDescription.DeathType.None)
                                                                                                                                    {
                                                                                                                                        if (descriptisdfsdfweron.IsHuman)
                                                                                                                                        {
                                                                                                                                            switch (randomObjectFromList)
                                                                                                                                            {
                                                                                                                                                case SimDescription.DeathType.None:
                                                                                                                                                case SimDescription.DeathType.PetOldAgeBad:
                                                                                                                                                    if (list.Count > 0)
                                                                                                                                                    {
                                                                                                                                                        foreach (SimDescription descripsdfwertion in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                                                        {

                                                                                                                                                            try
                                                                                                                                                            {
                                                                                                                                                                //Name is
                                                                                                                                                                if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                                                                                                {
                                                                                                                                                                    continue;
                                                                                                                                                                }

                                                                                                                                                                if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                                                                                                {
                                                                                                                                                                    continue;
                                                                                                                                                                }

                                                                                                                                                                if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                                                                                                {
                                                                                                                                                                    MaptagTypes mType = MaptagTypes.ActiveHousehold;
                                                                                                                                                                    MapTagType raADwe = MapTagType.NeighborLot;
                                                                                                                                                                    switch (mType)
                                                                                                                                                                    {
                                                                                                                                                                        case MaptagTypes.ActiveHousehold:
                                                                                                                                                                            return raADwe == MapTagType.ActiveHousehold;
                                                                                                                                                                        case MaptagTypes.SelectedEmptyLot:
                                                                                                                                                                            return raADwe == MapTagType.SelectedEmptyLot;
                                                                                                                                                                        case MaptagTypes.SelectedEmptyHouse:
                                                                                                                                                                            return raADwe == MapTagType.SelectedEmptyHouse;
                                                                                                                                                                        case MaptagTypes.SelectedHousehold:
                                                                                                                                                                            switch (mType)
                                                                                                                                                                            {
                                                                                                                                                                                case MaptagTypes.ActiveHousehold:
                                                                                                                                                                                    return raADwe == MapTagType.ActiveHousehold;
                                                                                                                                                                                case MaptagTypes.SelectedEmptyLot:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedEmptyLot;
                                                                                                                                                                                case MaptagTypes.SelectedEmptyHouse:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedEmptyHouse;
                                                                                                                                                                                case MaptagTypes.SelectedHousehold:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedHousehold;
                                                                                                                                                                                case MaptagTypes.SelectedCommunity:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedCommunity;
                                                                                                                                                                                case MaptagTypes.AvailableEmptyLot:
                                                                                                                                                                                    return raADwe == MapTagType.AvailableEmptyLot;
                                                                                                                                                                                case MaptagTypes.AvailableEmptyHouse:
                                                                                                                                                                                    return raADwe == MapTagType.AvailableEmptyHouse;
                                                                                                                                                                                case MaptagTypes.AvailableHousehold:
                                                                                                                                                                                    return raADwe == MapTagType.AvailableHousehold;
                                                                                                                                                                                case MaptagTypes.UnavailableEmptyLot:
                                                                                                                                                                                    return raADwe == MapTagType.UnavailableEmptyLot;
                                                                                                                                                                                case MaptagTypes.UnavailableEmptyHouse:
                                                                                                                                                                                    return raADwe == MapTagType.UnavailableEmptyHouse;
                                                                                                                                                                                case MaptagTypes.UnavailableHousehold:
                                                                                                                                                                                    return raADwe == MapTagType.UnavailableHousehold;
                                                                                                                                                                                case MaptagTypes.CommunityLot:
                                                                                                                                                                                    return raADwe == MapTagType.CommunityLot;
                                                                                                                                                                                case MaptagTypes.EmptyCommunityLot:
                                                                                                                                                                                    return raADwe == MapTagType.EmptyCommunityLot;
                                                                                                                                                                                case MaptagTypes.BaseCamp:
                                                                                                                                                                                    return raADwe == MapTagType.BaseCamp;
                                                                                                                                                                                case MaptagTypes.OwnableHouse:
                                                                                                                                                                                    return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                                case MaptagTypes.OwnableLot:
                                                                                                                                                                                    return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                                case MaptagTypes.OwnedLot:
                                                                                                                                                                                    return raADwe == MapTagType.OwnedLot;
                                                                                                                                                                                case MaptagTypes.SelectedBaseCamp:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedBaseCamp;
                                                                                                                                                                                case MaptagTypes.SelectedOwnableHouse:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                                case MaptagTypes.SelectedOwnableLot:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                                case MaptagTypes.SelectedOwnedLot:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedOwnedLot;
                                                                                                                                                                                case MaptagTypes.UnavailableOwnableLot:
                                                                                                                                                                                    return raADwe == MapTagType.UnavailableOwnableLot;
                                                                                                                                                                                case MaptagTypes.Dormitory:
                                                                                                                                                                                    return raADwe == MapTagType.Dormitory;
                                                                                                                                                                                case MaptagTypes.SelectedDormitory:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedDormitory;
                                                                                                                                                                                case MaptagTypes.Fraternity:
                                                                                                                                                                                    return raADwe == MapTagType.Fraternity;
                                                                                                                                                                                case MaptagTypes.SelectedFraternity:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedFraternity;
                                                                                                                                                                                case MaptagTypes.Sorority:
                                                                                                                                                                                    return raADwe == MapTagType.Sorority;
                                                                                                                                                                                case MaptagTypes.SelectedSorority:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedSorority;
                                                                                                                                                                                case MaptagTypes.Apartment:
                                                                                                                                                                                    return raADwe == MapTagType.Apartment;
                                                                                                                                                                                case MaptagTypes.SelectedApartment:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedApartment;
                                                                                                                                                                                case MaptagTypes.DivingLot:
                                                                                                                                                                                    return raADwe == MapTagType.DivingLot;
                                                                                                                                                                                case MaptagTypes.UnoccupiedPort:
                                                                                                                                                                                    return raADwe == MapTagType.PortLot;
                                                                                                                                                                                case MaptagTypes.BaseCampFuture:
                                                                                                                                                                                    return raADwe == MapTagType.BaseCampFuture;
                                                                                                                                                                                case MaptagTypes.SelectedBaseCampFuture:
                                                                                                                                                                                    return raADwe == MapTagType.SelectedBaseCampFuture;
                                                                                                                                                                                default:
                                                                                                                                                                                    return raADwe == MapTagType.Venue;
                                                                                                                                                                            }
                                                                                                                                                                        case MaptagTypes.SelectedCommunity:
                                                                                                                                                                            return raADwe == MapTagType.SelectedCommunity;
                                                                                                                                                                        case MaptagTypes.AvailableEmptyLot:
                                                                                                                                                                            return raADwe == MapTagType.AvailableEmptyLot;
                                                                                                                                                                        case MaptagTypes.AvailableEmptyHouse:
                                                                                                                                                                            return raADwe == MapTagType.AvailableEmptyHouse;
                                                                                                                                                                        case MaptagTypes.AvailableHousehold:
                                                                                                                                                                            return raADwe == MapTagType.AvailableHousehold;
                                                                                                                                                                        case MaptagTypes.UnavailableEmptyLot:
                                                                                                                                                                            return raADwe == MapTagType.UnavailableEmptyLot;
                                                                                                                                                                        case MaptagTypes.UnavailableEmptyHouse:
                                                                                                                                                                            bool flagsaea = false;
                                                                                                                                                                            bool flagsa = false;
                                                                                                                                                                            if (flagsaea)
                                                                                                                                                                            {
                                                                                                                                                                                try
                                                                                                                                                                                {

                                                                                                                                                                                    Sim sim = DGSMakeSim.DGSMakeRandomSimNoCheck(Vector3.OutOfWorld, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                                                                                                                    if (sim != null)
                                                                                                                                                                                    {

                                                                                                                                                                                        try
                                                                                                                                                                                        {
                                                                                                                                                                                            //sim.SimDescription.Household.SetName("E3Lesa is Good Good Household");
                                                                                                                                                                                            sim.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                                                                                                                                                                            PlumbBob.ForceSelectActor(sim);
                                                                                                                                                                                            //Service.NeedsAssignment(target.LotCurrent);
                                                                                                                                                                                            try
                                                                                                                                                                                            {
                                                                                                                                                                                                if (sim.SimDescription.Child || sim.SimDescription.Teen)
                                                                                                                                                                                                {
                                                                                                                                                                                                    sim.SimDescription.AssignSchool();
                                                                                                                                                                                                }

                                                                                                                                                                                            }
                                                                                                                                                                                            catch
                                                                                                                                                                                            { }
                                                                                                                                                                                            try
                                                                                                                                                                                            {
                                                                                                                                                                                                if (sim.IsSelectable)
                                                                                                                                                                                                {
                                                                                                                                                                                                    switch (mType)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        case MaptagTypes.ActiveHousehold:
                                                                                                                                                                                                            return raADwe == MapTagType.ActiveHousehold;
                                                                                                                                                                                                        case MaptagTypes.SelectedEmptyLot:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedEmptyLot;
                                                                                                                                                                                                        case MaptagTypes.SelectedEmptyHouse:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedEmptyHouse;
                                                                                                                                                                                                        case MaptagTypes.SelectedHousehold:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedHousehold;
                                                                                                                                                                                                        case MaptagTypes.SelectedCommunity:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedCommunity;
                                                                                                                                                                                                        case MaptagTypes.AvailableEmptyLot:
                                                                                                                                                                                                            return raADwe == MapTagType.AvailableEmptyLot;
                                                                                                                                                                                                        case MaptagTypes.AvailableEmptyHouse:
                                                                                                                                                                                                            return raADwe == MapTagType.AvailableEmptyHouse;
                                                                                                                                                                                                        case MaptagTypes.AvailableHousehold:
                                                                                                                                                                                                            return raADwe == MapTagType.AvailableHousehold;
                                                                                                                                                                                                        case MaptagTypes.UnavailableEmptyLot:
                                                                                                                                                                                                            return raADwe == MapTagType.UnavailableEmptyLot;
                                                                                                                                                                                                        case MaptagTypes.UnavailableEmptyHouse:
                                                                                                                                                                                                            return raADwe == MapTagType.UnavailableEmptyHouse;
                                                                                                                                                                                                        case MaptagTypes.UnavailableHousehold:
                                                                                                                                                                                                            return raADwe == MapTagType.UnavailableHousehold;
                                                                                                                                                                                                        case MaptagTypes.CommunityLot:
                                                                                                                                                                                                            return raADwe == MapTagType.CommunityLot;
                                                                                                                                                                                                        case MaptagTypes.EmptyCommunityLot:
                                                                                                                                                                                                            return raADwe == MapTagType.EmptyCommunityLot;
                                                                                                                                                                                                        case MaptagTypes.BaseCamp:
                                                                                                                                                                                                            return raADwe == MapTagType.BaseCamp;
                                                                                                                                                                                                        case MaptagTypes.OwnableHouse:
                                                                                                                                                                                                            return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                                                        case MaptagTypes.OwnableLot:
                                                                                                                                                                                                            return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                                                        case MaptagTypes.OwnedLot:
                                                                                                                                                                                                            return raADwe == MapTagType.OwnedLot;
                                                                                                                                                                                                        case MaptagTypes.SelectedBaseCamp:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedBaseCamp;
                                                                                                                                                                                                        case MaptagTypes.SelectedOwnableHouse:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                                                        case MaptagTypes.SelectedOwnableLot:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                                                        case MaptagTypes.SelectedOwnedLot:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedOwnedLot;
                                                                                                                                                                                                        case MaptagTypes.UnavailableOwnableLot:
                                                                                                                                                                                                            foreach (SimDescription descripsdfwertwerion in Household.ActiveHousehold.SimDescriptions)
                                                                                                                                                                                                            {

                                                                                                                                                                                                                try
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    //Name is
                                                                                                                                                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        continue;
                                                                                                                                                                                                                    }

                                                                                                                                                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        Sim Target = target;
                                                                                                                                                                                                                        Target.BuffManager.AddElement(BuffNames.Terrified, Origin.FromUnknown);
                                                                                                                                                                                                                        Target.BuffManager.AddElement(BuffNames.TooMuchSun, Origin.FromUnknown);
                                                                                                                                                                                                                        Target.BuffManager.AddElement(BuffNames.WalkOfShame, Origin.FromUnknown);

                                                                                                                                                                                                                        GameObject gameObject = null;
                                                                                                                                                                                                                        Vector3 invalid = Vector3.Invalid;
                                                                                                                                                                                                                        
                                                                                                                                                                                                                        if (target != null)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            target.PushGetStruckByLightning();
                                                                                                                                                                                                                            return true;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        
                                                                                                                                                                                                                        if (target != null)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            switch (mType)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                case MaptagTypes.ActiveHousehold:
                                                                                                                                                                                                                                    return raADwe == MapTagType.ActiveHousehold;
                                                                                                                                                                                                                                case MaptagTypes.SelectedEmptyLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedEmptyLot;
                                                                                                                                                                                                                                case MaptagTypes.SelectedEmptyHouse:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedEmptyHouse;
                                                                                                                                                                                                                                case MaptagTypes.SelectedHousehold:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedHousehold;
                                                                                                                                                                                                                                case MaptagTypes.SelectedCommunity:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedCommunity;
                                                                                                                                                                                                                                case MaptagTypes.AvailableEmptyLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.AvailableEmptyLot;
                                                                                                                                                                                                                                case MaptagTypes.AvailableEmptyHouse:
                                                                                                                                                                                                                                    return raADwe == MapTagType.AvailableEmptyHouse;
                                                                                                                                                                                                                                case MaptagTypes.AvailableHousehold:
                                                                                                                                                                                                                                    return raADwe == MapTagType.AvailableHousehold;
                                                                                                                                                                                                                                case MaptagTypes.UnavailableEmptyLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.UnavailableEmptyLot;
                                                                                                                                                                                                                                case MaptagTypes.UnavailableEmptyHouse:
                                                                                                                                                                                                                                    return raADwe == MapTagType.UnavailableEmptyHouse;
                                                                                                                                                                                                                                case MaptagTypes.UnavailableHousehold:
                                                                                                                                                                                                                                    return raADwe == MapTagType.UnavailableHousehold;
                                                                                                                                                                                                                                case MaptagTypes.CommunityLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.CommunityLot;
                                                                                                                                                                                                                                case MaptagTypes.EmptyCommunityLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.EmptyCommunityLot;
                                                                                                                                                                                                                                case MaptagTypes.BaseCamp:
                                                                                                                                                                                                                                    return raADwe == MapTagType.BaseCamp;
                                                                                                                                                                                                                                case MaptagTypes.OwnableHouse:
                                                                                                                                                                                                                                    List<Lot> lieastERS = new List<Lot>();
                Lot lotERS = null;
                List<Household> newlistERS = new List<Household>(Household.sHouseholdList);
                foreach (Household mhouselist in newlistERS)
                {
                    if (mhouselist == null) continue;
                    try
                    {
                        if (mhouselist == Household.NpcHousehold || mhouselist == Household.PetHousehold || mhouselist == Household.MermaidHousehold || mhouselist == Household.TouristHousehold || mhouselist == Household.PreviousTravelerHousehold || mhouselist == Household.AlienHousehold || mhouselist == Household.ServobotHousehold) continue;
                    }
                    catch
                    { }







                    try
                    {
                        if (mhouselist.LotHome != null) continue;
                    }
                    catch
                    { }


                    lieastERS.Clear();

                    foreach (object oeabj in LotManager.AllLots)
                    {
                        Lot lot2 = (Lot)oeabj;
                        if (!lot2.IsWorldLot && !lot2.IsCommunityLotOfType(CommercialLotSubType.kEP10_Diving) && !UnchartedIslandMarker.IsUnchartedIsland(lot2) && lot2.IsResidentialLot && lot2.Household == null && !World.LotIsEmpty(lot2.LotId) && !lot2.IsLotEmptyFromObjects())
                        {
                            lieastERS.Add(lot2);
                        }
                        if (lieastERS.Count == 0)
                        {
                            if (!lot2.IsWorldLot && !lot2.IsCommunityLotOfType(CommercialLotSubType.kEP10_Diving) && !UnchartedIslandMarker.IsUnchartedIsland(lot2) && lot2.IsResidentialLot && lot2.Household == null)
                            {
                                lieastERS.Add(lot2);
                            }
                        }
                    }

                    if (lieastERS.Count > 0)
                    {
                        lotERS = RandomUtil.GetRandomObjectFromList<Lot>(lieastERS);
                        lotERS.MoveIn(mhouselist);
                    }

                    else return false;


                }
                                                                                                                                                                                                                                    break;
                                                                                                                                                                                                                                case MaptagTypes.OwnableLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                                                                                case MaptagTypes.OwnedLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.OwnedLot;
                                                                                                                                                                                                                                case MaptagTypes.SelectedBaseCamp:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedBaseCamp;
                                                                                                                                                                                                                                case MaptagTypes.SelectedOwnableHouse:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                                                                                case MaptagTypes.SelectedOwnableLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                                                                                case MaptagTypes.SelectedOwnedLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedOwnedLot;
                                                                                                                                                                                                                                case MaptagTypes.UnavailableOwnableLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.UnavailableOwnableLot;
                                                                                                                                                                                                                                case MaptagTypes.Dormitory:
                                                                                                                                                                                                                                    return raADwe == MapTagType.Dormitory;
                                                                                                                                                                                                                                case MaptagTypes.SelectedDormitory:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedDormitory;
                                                                                                                                                                                                                                case MaptagTypes.Fraternity:
                                                                                                                                                                                                                                    return raADwe == MapTagType.Fraternity;
                                                                                                                                                                                                                                case MaptagTypes.SelectedFraternity:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedFraternity;
                                                                                                                                                                                                                                case MaptagTypes.Sorority:
                                                                                                                                                                                                                                    return raADwe == MapTagType.Sorority;
                                                                                                                                                                                                                                case MaptagTypes.SelectedSorority:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedSorority;
                                                                                                                                                                                                                                case MaptagTypes.Apartment:
                                                                                                                                                                                                                                    return raADwe == MapTagType.Apartment;
                                                                                                                                                                                                                                case MaptagTypes.SelectedApartment:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedApartment;
                                                                                                                                                                                                                                case MaptagTypes.DivingLot:
                                                                                                                                                                                                                                    return raADwe == MapTagType.DivingLot;
                                                                                                                                                                                                                                case MaptagTypes.UnoccupiedPort:
                                                                                                                                                                                                                                    return raADwe == MapTagType.PortLot;
                                                                                                                                                                                                                                case MaptagTypes.BaseCampFuture:
                                                                                                                                                                                                                                    return raADwe == MapTagType.BaseCampFuture;
                                                                                                                                                                                                                                case MaptagTypes.SelectedBaseCampFuture:
                                                                                                                                                                                                                                    return raADwe == MapTagType.SelectedBaseCampFuture;
                                                                                                                                                                                                                                default:
                                                                                                                                                                                                                                    return raADwe == MapTagType.Venue;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            
                                                                                                                                                                                                                            flag = true;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        VisualEffect visualEffect = (!CauseEffectService.IsDystopiaWorld) ? VisualEffect.Create("ep8LightningHit") : VisualEffect.Create("ep11LightningHit_main");
                                                                                                                                                                                                                        visualEffect.SetPosAndOrient(Vector3.UnitY, Vector3.UnitX, Vector3.UnitY);
                                                                                                                                                                                                                        visualEffect.SubmitOneShotEffect(VisualEffect.TransitionType.SoftTransition);
                                                                                                                                                                                                                        if (flag)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            LotLocation location = default(LotLocation);
                                                                                                                                                                                                                            ulong lotLocation = World.GetLotLocation(Vector3.UnitY, ref location);
                                                                                                                                                                                                                            if (lotLocation != 0)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                FireManager.BurnTile(lotLocation, location);
                                                                                                                                                                                                                                Lot laweot = LotManager.GetLot(lotLocation);
                                                                                                                                                                                                                                if (lot != null)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    PetStartleBehavior.CheckForStartle(lot, StartleType.Lightning);
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        Simulator.Sleep(30u);
                                                                                                                                                                                                                        if (gameObject != null)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            Meteor.DoDestructiveBehavior(gameObject, true);
                                                                                                                                                                                                                            PetStartleBehavior.CheckForStartle(gameObject, StartleType.Lightning);
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        if (CauseEffectService.IsDystopiaWorld)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            Audio.StartSound("thunder_future", Vector3.UnitY);
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            Audio.StartSound("thunder_near", Vector3.UnitY);
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        float[] cameraShakeTuningValues = SeasonsManager.GetCameraShakeTuningValues();
                                                                                                                                                                                                                        CameraController.Shake(cameraShakeTuningValues[0], cameraShakeTuningValues[1]);
                                                                                                                                                                                                                        return true;
                                                                                                                                                                                                                    }

                                                                                                                                                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        continue;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                                catch (NullReferenceException)
                                                                                                                                                                                                                { }

                                                                                                                                                                                                                List<SimDescription.DeathType> lissdfsdfwertr = new List<SimDescription.DeathType>();
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Drown);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Starve);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Thirst);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Burn);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Freeze);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Shark);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Jetpack);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Meteor);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Causality);
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.Electrocution);
                                                                                                                                                                                                                if (target.SimDescription.Elder)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.OldAge);
                                                                                                                                                                                                                }
                                                                                                                                                                                                                lissdfsdfwertr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                                                                                                                                                
                                                                                                                                                                                                                //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                                                                                                                                                //simDeathType = randomObjectFromList;
                                                                                                                                                                                                                Urnstones.CreateGrave(description, randomObjectFromList, true, true);
                                                                                                                                                                                                            }
                                                                                                                                                                                                            break;
                                                                                                                                                                                                        case MaptagTypes.Dormitory:
                                                                                                                                                                                                            return raADwe == MapTagType.Dormitory;
                                                                                                                                                                                                        case MaptagTypes.SelectedDormitory:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedDormitory;
                                                                                                                                                                                                        case MaptagTypes.Fraternity:
                                                                                                                                                                                                            return raADwe == MapTagType.Fraternity;
                                                                                                                                                                                                        case MaptagTypes.SelectedFraternity:
                                                                                                                                                                                                            if (sim.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                                                                                                                            {
                                                                                                                                                                                                                sim.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                                                                                                                                sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                                                                                                                                            }
                                                                                                                                                                                                            sim.AddInitialObjects(sim.IsSelectable);
                                                                                                                                                                                                            break;
                                                                                                                                                                                                        case MaptagTypes.Sorority:
                                                                                                                                                                                                            return raADwe == MapTagType.Sorority;
                                                                                                                                                                                                        case MaptagTypes.SelectedSorority:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedSorority;
                                                                                                                                                                                                        case MaptagTypes.Apartment:
                                                                                                                                                                                                            return raADwe == MapTagType.Apartment;
                                                                                                                                                                                                        case MaptagTypes.SelectedApartment:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedApartment;
                                                                                                                                                                                                        case MaptagTypes.DivingLot:
                                                                                                                                                                                                            return raADwe == MapTagType.DivingLot;
                                                                                                                                                                                                        case MaptagTypes.UnoccupiedPort:
                                                                                                                                                                                                            return raADwe == MapTagType.PortLot;
                                                                                                                                                                                                        case MaptagTypes.BaseCampFuture:
                                                                                                                                                                                                            return raADwe == MapTagType.BaseCampFuture;
                                                                                                                                                                                                        case MaptagTypes.SelectedBaseCampFuture:
                                                                                                                                                                                                            return raADwe == MapTagType.SelectedBaseCampFuture;
                                                                                                                                                                                                        default:
                                                                                                                                                                                                            return raADwe == MapTagType.Venue;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            catch
                                                                                                                                                                                            { }

                                                                                                                                                                                        }
                                                                                                                                                                                        catch (Exception ex2)
                                                                                                                                                                                        {
                                                                                                                                                                                            NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                                                                                                                        }
                                                                                                                                                                                        try
                                                                                                                                                                                        {
                                                                                                                                                                                            if (sim.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                                                                                                            {
                                                                                                                                                                                                sim.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                                                                                                                sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                                                                                                                            }
                                                                                                                                                                                            sim.AddInitialObjects(sim.IsSelectable);
                                                                                                                                                                                        }
                                                                                                                                                                                        catch
                                                                                                                                                                                        { }

                                                                                                                                                                                        if (flagsa)
                                                                                                                                                                                        {
                                                                                                                                                                                            ProgressDialog.Close();
                                                                                                                                                                                        }
                                                                                                                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                                                        if (optionsModel != null)
                                                                                                                                                                                        {
                                                                                                                                                                                            optionsModel.SaveName = "ClearSave " + sim.SimDescription.LastName;

                                                                                                                                                                                            optionsModel.SaveGame(true, true, false);
                                                                                                                                                                                        }
                                                                                                                                                                                    }

                                                                                                                                                                                    if (NiecMod.Helpers.Create.CreateActiveHouseholdAndActiveActor())
                                                                                                                                                                                    {
                                                                                                                                                                                        if (flagsa)
                                                                                                                                                                                        {

                                                                                                                                                                                            try
                                                                                                                                                                                            {
                                                                                                                                                                                                ProgressDialog.Close();
                                                                                                                                                                                            }
                                                                                                                                                                                            catch
                                                                                                                                                                                            { }
                                                                                                                                                                                        }
                                                                                                                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                                                        if (optionsModel != null)
                                                                                                                                                                                        {
                                                                                                                                                                                            optionsModel.SaveName = "ClearSave " + PlumbBob.Singleton.mSelectedActor.SimDescription.LastName;

                                                                                                                                                                                            optionsModel.SaveGame(true, true, false);
                                                                                                                                                                                        }
                                                                                                                                                                                    }











                                                                                                                                                                                    //else if (Sim.ActiveActor == null)
                                                                                                                                                                                    //if (Sim.ActiveActor == null)
                                                                                                                                                                                    else if (PlumbBob.Singleton.mSelectedActor == null)
                                                                                                                                                                                    {
                                                                                                                                                                                        try
                                                                                                                                                                                        {
                                                                                                                                                                                            CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                                                                                                            if (flagsa)
                                                                                                                                                                                            {

                                                                                                                                                                                                try
                                                                                                                                                                                                {
                                                                                                                                                                                                    ProgressDialog.Close();
                                                                                                                                                                                                }
                                                                                                                                                                                                catch
                                                                                                                                                                                                { }

                                                                                                                                                                                            }
                                                                                                                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                                                            if (optionsModel != null)
                                                                                                                                                                                            {
                                                                                                                                                                                                optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        catch
                                                                                                                                                                                        { }
                                                                                                                                                                                        finally
                                                                                                                                                                                        {
                                                                                                                                                                                            CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                                                                        }


                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                                                                        if (flagsa)
                                                                                                                                                                                        {
                                                                                                                                                                                            try
                                                                                                                                                                                            {
                                                                                                                                                                                                ProgressDialog.Close();
                                                                                                                                                                                            }
                                                                                                                                                                                            catch
                                                                                                                                                                                            { }

                                                                                                                                                                                        }
                                                                                                                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                                                        if (optionsModel != null)
                                                                                                                                                                                        {
                                                                                                                                                                                            optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                                                            optionsModel.SaveGame(true, true, false);
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                catch
                                                                                                                                                                                {
                                                                                                                                                                                    try
                                                                                                                                                                                    {
                                                                                                                                                                                        CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                                                                                                        CommandSystem.ExecuteCommandString("makesim");

                                                                                                                                                                                        if (flagsa)
                                                                                                                                                                                        {
                                                                                                                                                                                            ProgressDialog.Close();
                                                                                                                                                                                        }
                                                                                                                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                                                        if (optionsModel != null)
                                                                                                                                                                                        {
                                                                                                                                                                                            optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                                                            optionsModel.SaveGame(true, true, false);
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    catch
                                                                                                                                                                                    { }
                                                                                                                                                                                    finally
                                                                                                                                                                                    {
                                                                                                                                                                                        CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                try
                                                                                                                                                                                {
                                                                                                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                                                                                                }
                                                                                                                                                                                catch
                                                                                                                                                                                { }
                                                                                                                                                                                if (flagsa)
                                                                                                                                                                                {
                                                                                                                                                                                    ProgressDialog.Close();
                                                                                                                                                                                }
                                                                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                                                                                                if (optionsModel != null)
                                                                                                                                                                                {
                                                                                                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                                                                                                }
                                                                                                                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                                                                                                                try
                                                                                                                                                                                {
                                                                                                                                                                                    GameStates.TransitionToEditTown();
                                                                                                                                                                                }
                                                                                                                                                                                catch
                                                                                                                                                                                { }

                                                                                                                                                                            }
                                                                                                                                                                            break;
                                                                                                                                                                        case MaptagTypes.UnavailableHousehold:
                                                                                                                                                                            return raADwe == MapTagType.UnavailableHousehold;
                                                                                                                                                                        case MaptagTypes.CommunityLot:
                                                                                                                                                                            return raADwe == MapTagType.CommunityLot;
                                                                                                                                                                        case MaptagTypes.EmptyCommunityLot:
                                                                                                                                                                            return raADwe == MapTagType.EmptyCommunityLot;
                                                                                                                                                                        case MaptagTypes.BaseCamp:
                                                                                                                                                                            return raADwe == MapTagType.BaseCamp;
                                                                                                                                                                        case MaptagTypes.OwnableHouse:
                                                                                                                                                                            return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                        case MaptagTypes.OwnableLot:
                                                                                                                                                                            return raADwe == MapTagType.OwnableLot;
                                                                                                                                                                        case MaptagTypes.OwnedLot:
                                                                                                                                                                            return raADwe == MapTagType.OwnedLot;
                                                                                                                                                                        case MaptagTypes.SelectedBaseCamp:
                                                                                                                                                                            return raADwe == MapTagType.SelectedBaseCamp;
                                                                                                                                                                        case MaptagTypes.SelectedOwnableHouse:
                                                                                                                                                                            return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                        case MaptagTypes.SelectedOwnableLot:
                                                                                                                                                                            return raADwe == MapTagType.SelectedOwnableLot;
                                                                                                                                                                        case MaptagTypes.SelectedOwnedLot:
                                                                                                                                                                            return raADwe == MapTagType.SelectedOwnedLot;
                                                                                                                                                                        case MaptagTypes.UnavailableOwnableLot:
                                                                                                                                                                            return raADwe == MapTagType.UnavailableOwnableLot;
                                                                                                                                                                        case MaptagTypes.Dormitory:
                                                                                                                                                                            return raADwe == MapTagType.Dormitory;
                                                                                                                                                                        case MaptagTypes.SelectedDormitory:
                                                                                                                                                                            return raADwe == MapTagType.SelectedDormitory;
                                                                                                                                                                        case MaptagTypes.Fraternity:
                                                                                                                                                                            return raADwe == MapTagType.Fraternity;
                                                                                                                                                                        case MaptagTypes.SelectedFraternity:
                                                                                                                                                                            return raADwe == MapTagType.SelectedFraternity;
                                                                                                                                                                        case MaptagTypes.Sorority:
                                                                                                                                                                            return raADwe == MapTagType.Sorority;
                                                                                                                                                                        case MaptagTypes.SelectedSorority:
                                                                                                                                                                            return raADwe == MapTagType.SelectedSorority;
                                                                                                                                                                        case MaptagTypes.Apartment:
                                                                                                                                                                            return raADwe == MapTagType.Apartment;
                                                                                                                                                                        case MaptagTypes.SelectedApartment:
                                                                                                                                                                            return raADwe == MapTagType.SelectedApartment;
                                                                                                                                                                        case MaptagTypes.DivingLot:
                                                                                                                                                                            return raADwe == MapTagType.DivingLot;
                                                                                                                                                                        case MaptagTypes.UnoccupiedPort:
                                                                                                                                                                            return raADwe == MapTagType.PortLot;
                                                                                                                                                                        case MaptagTypes.BaseCampFuture:
                                                                                                                                                                            return raADwe == MapTagType.BaseCampFuture;
                                                                                                                                                                        case MaptagTypes.SelectedBaseCampFuture:
                                                                                                                                                                            return raADwe == MapTagType.SelectedBaseCampFuture;
                                                                                                                                                                        default:
                                                                                                                                                                            return raADwe == MapTagType.Venue;
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            catch (NullReferenceException)
                                                                                                                                                            { }

                                                                                                                                                            List<SimDescription.DeathType> lissdfsdasdefwertr = new List<SimDescription.DeathType>();
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Drown);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Starve);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Thirst);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Burn);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Freeze);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Shark);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Jetpack);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Meteor);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Causality);
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.Electrocution);
                                                                                                                                                            if (target.SimDescription.Elder)
                                                                                                                                                            {
                                                                                                                                                                lissdfsdasdefwertr.Add(SimDescription.DeathType.OldAge);
                                                                                                                                                            }
                                                                                                                                                            lissdfsdasdefwertr.Add(SimDescription.DeathType.HauntingCurse);

                                                                                                                                                            
                                                                                                                                                            Urnstones.CreateGrave(description, randomObjectFromList, true, true);
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    break;
                                                                                                                                                case SimDescription.DeathType.PetOldAgeGood:
                                                                                                                                                    randomObjectFromList = SimDescription.DeathType.OldAge;
                                                                                                                                                    break;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            switch (randomObjectFromList)
                                                                                                                                            {
                                                                                                                                                case SimDescription.DeathType.None:
                                                                                                                                                case SimDescription.DeathType.OldAge:
                                                                                                                                                    randomObjectFromList = SimDescription.DeathType.PetOldAgeGood;
                                                                                                                                                    break;
                                                                                                                                            }
                                                                                                                                        }

                                                                                                                                        descriptisdfsdfweron.SetDeathStyle(randomObjectFromList, true);
                                                                                                                                    }

                                                                                                                                    descriptisdfsdfweron.IsNeverSelectable = true;
                                                                                                                                    descriptisdfsdfweron.Contactable = false;
                                                                                                                                    descriptisdfsdfweron.Marryable = false;


                                                                                                                                    if (descriptisdfsdfweron.CreatedSim != null && descriptisdfsdfweron.CreatedSim == PlumbBob.SelectedActor)
                                                                                                                                    {
                                                                                                                                        LotManager.SelectNextSim();
                                                                                                                                    }
                                                                                                                                    if (descriptisdfsdfweron.CareerManager != null)
                                                                                                                                    {
                                                                                                                                        descriptisdfsdfweron.CareerManager.LeaveAllJobs(Career.LeaveJobReason.kDied);
                                                                                                                                    }

                                                                                                                                    mGravebackup.OnHandToolMovement();

                                                                                                                                    int num = ((int)Math.Floor((double)SimClock.ConvertFromTicks(SimClock.CurrentTime().Ticks, TimeUnit.Minutes))) % 60;
                                                                                                                                    mGravebackup.MinuteOfDeath = num;

                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        Urnstone.FinalizeSimDeath(descriptisdfsdfweron, household);
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                            }
                                                                                                                            //return true;
                                                                                                                        }
                                                                                                                        else if (AcceptCancelDialog.Show("Done UsStone?"))
                                                                                                                        {
                                                                                                                            foreach (SimDescription descriptiosdferwersn in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                                                                            {

                                                                                                                                try
                                                                                                                                {
                                                                                                                                    //Name is
                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (descriptiosdferwersn.Household == Household.ActiveHousehold) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        if (descriptiosdferwersn.IsGhost && descriptiosdferwersn.DeathStyle != SimDescription.DeathType.None) continue;
                                                                                                                                    }
                                                                                                                                    catch
                                                                                                                                    { }

                                                                                                                                    if (descriptiosdferwersn.FirstName == "Death" && descriptiosdferwersn.LastName == "Good System")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                    if (descriptiosdferwersn.FirstName == "Good System" && descriptiosdferwersn.LastName == "Death Helper")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }

                                                                                                                                    if (descriptiosdferwersn.FirstName == "Grim" && descriptiosdferwersn.LastName == "Reaper")
                                                                                                                                    {
                                                                                                                                        continue;
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }
                                                                                                                                try
                                                                                                                                {
                                                                                                                                    List<SimDescription.DeathType> lisdfsdferstr = new List<SimDescription.DeathType>();
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Drown);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Starve);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Thirst);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Burn);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Freeze);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Shark);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Jetpack);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Meteor);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Causality);
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.Electrocution);
                                                                                                                                    if (descriptiosdferwersn.Elder)
                                                                                                                                    {
                                                                                                                                        lisdfsdferstr.Add(SimDescription.DeathType.OldAge);
                                                                                                                                    }
                                                                                                                                    lisdfsdferstr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(lisdfsdferstr);
                                                                                                                                    //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                                                                    //simDeathType = randomObjectFromList;
                                                                                                                                    Urnstones.CreateGrave(descriptiosdferwersn, randomObjectFromList, false, true);
                                                                                                                                }
                                                                                                                                catch
                                                                                                                                { }

                                                                                                                            }
                                                                                                                        }
                                                                                                                        else if (AcceptCancelDialog.Show("Not NiecSimDesc"))
                                                                                                                        {
                                                                                                                            List<SimDescription> lisyqweqweet = new List<SimDescription>();
                                                                                                                            foreach (SimDescription sd in NFinalizeDeath.TattoaX())
                                                                                                                            {
                                                                                                                                if (sd.Household != Household.ActiveHousehold && !(sd.Service is GrimReaper) && !sd.IsDead && !sd.IsGhost) //OK
                                                                                                                                {
                                                                                                                                    lisyqweqweet.Add(sd);
                                                                                                                                }
                                                                                                                            }
                                                                                                                            if (list.Count > 0)
                                                                                                                            {
                                                                                                                                foreach (SimDescription descripsdfwertion in lisyqweqweet)
                                                                                                                                {

                                                                                                                                    try
                                                                                                                                    {
                                                                                                                                        //Name is
                                                                                                                                        if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                                                                        {
                                                                                                                                            continue;
                                                                                                                                        }

                                                                                                                                        if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                                                                        {
                                                                                                                                            continue;
                                                                                                                                        }

                                                                                                                                        if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                                                                        {
                                                                                                                                            continue;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    catch (NullReferenceException)
                                                                                                                                    { }

                                                                                                                                    List<SimDescription.DeathType> lissdfsdfwertr = new List<SimDescription.DeathType>();
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Drown);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Starve);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Thirst);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Burn);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Freeze);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Shark);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Jetpack);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Meteor);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Causality);
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.Electrocution);
                                                                                                                                    if (target.SimDescription.Elder)
                                                                                                                                    {
                                                                                                                                        lissdfsdfwertr.Add(SimDescription.DeathType.OldAge);
                                                                                                                                    }
                                                                                                                                    lissdfsdfwertr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(lissdfsdfwertr);
                                                                                                                                    //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                                                                    //simDeathType = randomObjectFromList;
                                                                                                                                    Urnstones.CreateGrave(description, randomObjectFromList, true, true);
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            return false;
                                                                                                                        }

                                                                                                                        /*
                                                                        
                                                                                                                         */

                                                                                                                        return true;
                                                                                                                    }

                                                                                                                    catch (Exception ex)
                                                                                                                    {

                                                                                                                        NiecException.PrintMessage(ex.Message);
                                                                                                                        Common.Exception(target, ex);
                                                                                                                    }
                                                                                                                    return false;
                                                                                                                }
                                                                                                            }
                                                                                                        }

                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    listr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                    SimDescription.DeathType randomObjectFrsdfdomList = RandomUtil.GetRandomObjectFromList(listr);
                                                                                    //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                    //simDeathType = randomObjectFromList;
                                                                                    Urnstones.CreateGrave(description, randomObjectFrsdfdomList, true, true);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            return false;
                                                                        }

                                                                        /*
                                                                        
                                                                         */

                                                                        return true;
                                                                    }

                                                                    catch (Exception ex)
                                                                    {

                                                                        NiecException.PrintMessage(ex.Message);
                                                                        Common.Exception(target, ex);
                                                                    }
                                                                    return false;
                                                                }
                                                            }
                                                        }

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
                                                    ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
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
                                    mSim.SimRoutingComponent.DisableDynamicFootprint();

                                    mSim.SimRoutingComponent.EnableDynamicFootprint();

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

                                AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(AssemblyCheckByNiec.Atiao), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);

                                return true;
                            }


                            else
                            {
                                list.Add(item);
                                list2.Add(item.Position);
                                item.SetObjectToReset();
                            }
                        }
                        Simulator.Sleep(0u);
                        int numae = 0;
                        while (numae < list.Count)
                        {
                            Sim sim = list[numae];
                            if (sim.HasBeenDestroyed)
                            {
                                list.RemoveAt(numae);
                                list2.RemoveAt(numae);
                            }
                            else
                            {
                                bool flag = false;
                                if (sim.SimDescription.IsDeer && sim.Household.LotId != ObjectGuid.InvalidObjectGuid.Value && sim.Household.LotId != lot.LotId)
                                {
                                    flag = true;
                                    foreach (Sim item2 in list)
                                    {
                                        if (item2.Household == sim.Household && !item2.SimDescription.IsDeer)
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (flag)
                                    {
                                        list4.Add(sim.LotHome);
                                        list.RemoveAt(numae);
                                        list2.RemoveAt(numae);
                                        numae--;
                                    }
                                }
                                if ((flag) && sim.Household.LotId != ObjectGuid.InvalidObjectGuid.Value && sim.Household.LotId != lot.LotId && (sim.AttemptToPutInSafeLocation(true) || flag))
                                {
                                    Simulator.Sleep(0u);
                                    numae++;
                                }
                                else
                                {
                                    sim.ResetAllAnimation();
                                    sim.StartDefaultPose();
                                    list3.Add(sim);
                                    numae++;
                                }
                            }
                        }
                        if (list3.Count > 0)
                        {
                            Vector3[] posResults;
                            Quaternion[] orientResults;
                            if (World.FindPlacesOnRoad(null, null, list2[0], FindPlaceOnRoadOption.FootpathOrSidewalk, (uint)list3.Count, 1f, out posResults, out orientResults))
                            {
                                int num2 = 0;
                                foreach (Sim item3 in list3)
                                {
                                    item3.SetPosition(posResults[num2++]);
                                }
                            }
                            else
                            {
                                foreach (Sim item4 in list3)
                                {
                                    Vector3 outPos = Vector3.Invalid;
                                    if (LotManager.FindPlaceOutsideLot(lot, ref offsetHint, ref outPos))
                                    {
                                        item4.SetPosition(outPos);
                                    }
                                    else
                                    {
                                        item4.AttemptToPutInSafeLocation(true);
                                    }
                                    Simulator.Sleep(0u);
                                }
                            }
                        }
                        Simulator.Sleep(0u);
                        {
                            foreach (Lot item5 in list4)
                            {
                                item5.CheckIfLotNeedsBabysitter();
                            }
                            return true;
                        }
                    }
                    else if (Sarae) {
                        List<SimDescription> list = new List<SimDescription>();
                        //foreach (SimDescription sd in SimDescription.GetAll(SimDescription.Repository.Household)) // OK Full?
                        //foreach (SimDescription sd in Households.All(mhousehold)) // Failed No List
                        //foreach (SimDescription sd in Household.EverySimDescription()) // Failed Not Full
                        //foreach (SimDescription sd in Household.AllSimsLivingInWorld()) // Failed Sims Live World
                        //foreach (Household h in Household.GetHouseholdsLivingInWorld())
                        //foreach (SimDescription sd in SimDescription.GetSimDescriptionsInWorld()) // OK Full One World!
                        foreach (SimDescription sd in NFinalizeDeath.TattoaX())
                        {
                            if (sd.Household != Household.ActiveHousehold && !(sd.Service is GrimReaper) && !sd.IsDead && !sd.IsGhost) //OK
                            {
                                list.Add(sd);
                            }
                        }
                        if (list.Count > 0)
                        {
                            foreach (SimDescription description in list)
                            {

                                try
                                {
                                    //Name is
                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                    {
                                        continue;
                                    }

                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                    {
                                        continue;
                                    }

                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                    {
                                        continue;
                                    }
                                }
                                catch (NullReferenceException)
                                { }

                                List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                listr.Add(SimDescription.DeathType.Drown);
                                listr.Add(SimDescription.DeathType.Starve);
                                listr.Add(SimDescription.DeathType.Thirst);
                                listr.Add(SimDescription.DeathType.Burn);
                                listr.Add(SimDescription.DeathType.Freeze);
                                listr.Add(SimDescription.DeathType.ScubaDrown);
                                listr.Add(SimDescription.DeathType.Shark);
                                listr.Add(SimDescription.DeathType.Jetpack);
                                listr.Add(SimDescription.DeathType.Meteor);
                                listr.Add(SimDescription.DeathType.Causality);
                                listr.Add(SimDescription.DeathType.Electrocution);
                                if (target.SimDescription.Elder)
                                {
                                    listr.Add(SimDescription.DeathType.OldAge);
                                }
                                listr.Add(SimDescription.DeathType.HauntingCurse);
                                SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);
                                //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                //simDeathType = randomObjectFromList;
                                Urnstones.CreateGrave(description, randomObjectFromList, true, true);
                            }
                        }
                    }
                }
                    // End Fake 
































                if (deathType == SimDescription.DeathType.None)
                {
                    switch (deathType)
                    {
                        case SimDescription.DeathType.None:
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
                            if (!target.SimDescription.IsFrankenstein)
                            {
                                list.Add(SimDescription.DeathType.Electrocution);
                            }
                            list.Add(SimDescription.DeathType.Burn);
                            if (target.SimDescription.Elder)
                            {
                                list.Add(SimDescription.DeathType.OldAge);
                            }
                            if (target.SimDescription.IsWitch)
                            {
                                list.Add(SimDescription.DeathType.HauntingCurse);
                            }
                            SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                            deathType = randomObjectFromList;
                            break;
                    }
                }
            }
            catch (NiecModException ex)
            {
                NiecException.PrintMessage(ex.Message + ex.StackTrace);
                Common.Exception(target, ex);
                DeathTypeNoneNotAllow = false;
                return false;
            }
            catch (Exception ex)
            {
                if (DeathTypeFixMineKill)
                {
                    NiecException.PrintMessage(ex.Message + ex.StackTrace);
                    Common.Exception(target, ex);
                    DeathTypeNoneNotAllow = false;
                    return false;
                }
                return false;
            }



            try
            {
                try
                {
                    if (NTunable.kEACanBeKilledExByNiecNotification && AssemblyCheckByNiec.IsInstalled("DGSCore"))
                    {
                        if (EACanBeKilledExByNiec(target))
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

                if (!CheckNiecKill(target))
                {
                    if (deathType == SimDescription.DeathType.OldAge)
                    {
                        if (!target.SimDescription.IsEP11Bot)
                        {
                            if (target.SimDescription.Elder)
                            {
                                target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                            }
                        }
                    }
                    if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                    {
                        if (deathType == SimDescription.DeathType.Thirst)
                        {
                            if (!target.SimDescription.IsEP11Bot)
                            {
                                if (target.SimDescription.Elder)
                                {
                                    target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                }
                            }
                                // Fake Start Part 2?
                            else if (Sarae)
                            {
                                


                                SimDescription mSimdesc = target.SimDescription;

                                Sim mSim = target;

                                Household mHousehold = target.Household;

                                Vector3 mHouseVeri3 = Vector3.OutOfWorld;

                                SimDescription.DeathType mdeathtype = deathType;

                                try
                                {
                                    if (!mSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton))
                                    {
                                        return true;
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
                                                AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(AssemblyCheckByNiec.Atiao), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);
                                                return true;
                                            }
                                        }
                                        catch
                                        { }
                                    }
                                    else
                                    {
                                        AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(AssemblyCheckByNiec.Atiao), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);
                                        return true;
                                    }
                                }
                                try
                                {
                                    
                                    
                                    
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
                                                NiecMod.Nra.SpeedTrap.Sleep();
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
                                                    
                                                    var asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                                    if (asdoei != null)
                                                    {
                                                        // Add New
                                                        if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                                        {
                                                            Lot lotloot = mSim.LotCurrent;
                                                            if (lotloot != null && lotloot.IsDivingLot)
                                                            {
                                                                ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
                                                                if (scubaDivinga != null)
                                                                {
                                                                    mSim.Posture = scubaDivinga;
                                                                    scubaDivinga.StartBubbleEffects();
                                                                }
                                                            }
                                                        }
                                                    }
                                                     
                                                }
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    else if (mSim.SimDescription != null)
                                    {
                                        try
                                        {
                                            foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                                            {
                                                if (interactionInstance.GetPriority().Level > (InteractionPriorityLevel)13) //  DGSCore :)
                                                {
                                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Level UP Level :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        { }

                                        try
                                        {
                                            foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                                            {
                                                if (interactionInstance.GetPriority().Level < (InteractionPriorityLevel)999) // Low Level ReNPCR Loop
                                                {
                                                    StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Level Down Level :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                                                }
                                            }
                                        }
                                        catch (Exception)
                                        { }


                                        if (!CheckNiecKill(target))
                                        {
                                            if (deathType == SimDescription.DeathType.OldAge)
                                            {
                                                if (!target.SimDescription.IsEP11Bot)
                                                {
                                                    if (target.SimDescription.Elder)
                                                    {
                                                        target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                    }
                                                }
                                                else if (target.SimDescription.Elder)
                                                {
                                                    target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                }
                                            }
                                            if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                            {
                                                if (deathType == SimDescription.DeathType.Thirst)
                                                {
                                                    if (!target.SimDescription.IsEP11Bot)
                                                    {
                                                        if (target.SimDescription.Elder)
                                                        {
                                                            target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                        }
                                                    }
                                                    else if (target.SimDescription.Elder)
                                                    {
                                                        target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                                    }
                                                }
                                            }
                                            target.FadeIn();
                                            if (NTunable.kDedugNotificationCheckNiecKill)
                                            {
                                                StyledNotification.Show(new StyledNotification.Format("MineKill: Sorry, " + target.Name + " No CheckNiecKill", StyledNotification.NotificationStyle.kGameMessagePositive));
                                            }
                                            return false;
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

                                                        var asdoei = mSim.CurrentInteraction as ExtKillSimNiec;
                                                        if (asdoei != null)
                                                        {
                                                            // Add New
                                                            if (GameUtils.IsInstalled(ProductVersion.EP10) && mSim.Posture != null && !(mSim.Posture is ScubaDiving))
                                                            {
                                                                Lot lotloot = mSim.LotCurrent;
                                                                if (lotloot != null && lotloot.IsDivingLot)
                                                                {
                                                                    ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
                                                                    if (scubaDivinga != null)
                                                                    {
                                                                        mSim.Posture = scubaDivinga;
                                                                        scubaDivinga.StartBubbleEffects();
                                                                    }
                                                                }
                                                                else if (GameUtils.IsInstalled(ProductVersion.EP10))
                                                                {
                                                                    try
                                                                    {
                                                                        bool Autonomous = true;
                                                                        if (Autonomous || target.IsNPC) return false;
                                                                        if (!AcceptCancelDialog.Show("Death Good System: Are You Sure Get Sim Descriptions In World Kill Sim?"))
                                                                        {
                                                                            if (AcceptCancelDialog.Show("Death Good System: CleanUp Dead SimDescription?"))
                                                                            {
                                                                                List<SimDescription> msime = new List<SimDescription>();
                                                                                try
                                                                                {
                                                                                    foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
                                                                                    {
                                                                                        if (urnstone.DeadSimsDescription != null && !msime.Contains(urnstone.DeadSimsDescription))
                                                                                        {
                                                                                            msime.Add(urnstone.DeadSimsDescription);
                                                                                        }
                                                                                    }





                                                                                    foreach (SimDescription murnstone in msime)
                                                                                    {
                                                                                        if (murnstone == null) continue;
                                                                                        try
                                                                                        {
                                                                                            if (murnstone.Household == Household.ActiveHousehold) continue;
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                        try
                                                                                        {



                                                                                            if (murnstone.IsDead)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    if (murnstone.CreatedSim != null)
                                                                                                    {
                                                                                                        try
                                                                                                        {
                                                                                                            NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(murnstone.CreatedSim);
                                                                                                        }
                                                                                                        catch
                                                                                                        { }
                                                                                                        try
                                                                                                        {
                                                                                                            murnstone.CreatedSim.Genealogy.ClearAllGenealogyInformation();

                                                                                                        }
                                                                                                        catch
                                                                                                        { }

                                                                                                        try
                                                                                                        {
                                                                                                            murnstone.CreatedSim.Genealogy.ClearMiniSimDescription();
                                                                                                        }
                                                                                                        catch
                                                                                                        { }
                                                                                                        try
                                                                                                        {
                                                                                                            murnstone.CreatedSim.Destroy();
                                                                                                        }
                                                                                                        catch
                                                                                                        { }
                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }










                                                                                                try
                                                                                                {
                                                                                                    murnstone.Genealogy.ClearAllGenealogyInformation();
                                                                                                }
                                                                                                catch
                                                                                                { }

                                                                                                try
                                                                                                {
                                                                                                    Household household = murnstone.Household;
                                                                                                    if (household != null)
                                                                                                    {
                                                                                                        household.Remove(murnstone, !household.IsSpecialHousehold);
                                                                                                    }

                                                                                                }
                                                                                                catch
                                                                                                { }






                                                                                                try
                                                                                                {
                                                                                                    while (true)
                                                                                                    {
                                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(murnstone);
                                                                                                        if (urnstone != null)
                                                                                                        {
                                                                                                            try
                                                                                                            {
                                                                                                                urnstone.DeadSimsDescription = null;
                                                                                                                urnstone.Destroy();
                                                                                                            }
                                                                                                            catch
                                                                                                            { }

                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            break;
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }
                                                                                                try
                                                                                                {
                                                                                                    murnstone.Dispose();
                                                                                                }
                                                                                                catch
                                                                                                { }










                                                                                            }




                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                    }



                                                                                }
                                                                                catch
                                                                                { }


                                                                                return true;
                                                                            }
                                                                            return false;
                                                                        }
                                                                        bool flag = false;

                                                                        if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0)
                                                                        {
                                                                            try
                                                                            {
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Contains(sdtyf))
                                                                                            Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            flag = true;
                                                                            if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0) return true;
                                                                        }

                                                                        if (!flag && AcceptCancelDialog.Show("Do you Are Add All SimDescriptions?"))
                                                                        {
                                                                            try
                                                                            {
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                        }
                                                                        if (AcceptCancelDialog.Show("Do you Are Delete All SimDescriptions?"))
                                                                        {

                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }


                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    for (int i = 0; i < 10; i++)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }
                                                                            }



                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions?"))
                                                                        {
                                                                            try
                                                                            {
                                                                                ProgressDialog.Show("Processing", false);
                                                                                NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {

                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        allService.ClearServicePool();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Service.Destroy(allService);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }

                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearAllGenealogyInformation();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    for (int i = 0; i < 20; i++)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }
                                                                            }
                                                                            try
                                                                            {
                                                                                ProgressDialog.Close();
                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                {
                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                    StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                    if (optionsModel != null)
                                                                                    {
                                                                                        optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                        PlumbBob.ForceSelectActor(null);
                                                                                        try
                                                                                        {
                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                            if (optionsModel != null)
                                                                                            {
                                                                                                optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }
                                                                                    finally
                                                                                    { PlumbBob.ForceSelectActor(target); }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions? Clear Keep Save"))
                                                                        {
                                                                            try
                                                                            {
                                                                                ProgressDialog.Show("Deleteing SimDescription...", false);
                                                                                NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {

                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        allService.ClearServicePool();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        Service.Destroy(allService);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                /*
                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }
                                                                                */
                                                                                try
                                                                                {
                                                                                    while (true)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }
                                                                            }
                                                                            try
                                                                            {
                                                                                try
                                                                                {
                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();
                                                                                }
                                                                                catch
                                                                                { }


                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                {
                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                    StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                    if (optionsModel != null)
                                                                                    {
                                                                                        optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                        ProgressDialog.Close();
                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {

                                                                                    try
                                                                                    {
                                                                                        ProgressDialog.Close();
                                                                                        StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                        PlumbBob.ForceSelectActor(null);
                                                                                        try
                                                                                        {
                                                                                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                            if (optionsModel != null)
                                                                                            {
                                                                                                optionsModel.SaveName = "ClearSave " + target.SimDescription.LastName;
                                                                                                optionsModel.SaveGame(true, true, false);
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }
                                                                                    finally
                                                                                    { PlumbBob.ForceSelectActor(target); }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                        }
                                                                        else if (PlumbBob.Singleton.mSelectedActor != null && NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore") && AcceptCancelDialog.Show("Do you Are Force Delete All SimDescriptions? Clear Keep Save Not Active"))
                                                                        {
                                                                            bool flagsae = AcceptCancelDialog.Show("Delete All MiniSimDescription?");
                                                                            bool flagsaea = !AcceptCancelDialog.Show("No make sim?");
                                                                            bool flagsa = AcceptCancelDialog.Show("ProgressDialog?");
                                                                            Lot mlot = target.LotCurrent;
                                                                            //Simulator.YieldingDisabled = true;
                                                                            //NiecTask mtask = new NiecTask.AddToSimulator();
                                                                            Nra.NFinalizeDeath.mResetError = true;
                                                                            ObjectGuid sdae = Sims3.NiecHelp.Tasks.NiecTask.Perform(Nra.NFinalizeDeath.ResetError);
                                                                            Sims3.SimIFace.Gameflow.GameSpeed currentGameSpeed = Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed;
                                                                            Vector3 mposition = Vector3.Invalid;



                                                                            try
                                                                            {
                                                                                CommandSystem.ExecuteCommandString("dgsunsafekill false");
                                                                                CommandSystem.ExecuteCommandString("dgspx false");
                                                                            }
                                                                            catch
                                                                            { }




                                                                            try
                                                                            {
                                                                                if (flagsa)
                                                                                {
                                                                                    //ProgressDialog.Show("Deleteing SimDescription...", ModalDialog.PauseMode.PauseSimulator, true, new Vector2(-1f, -1f), true, false, false);
                                                                                    //GameUtils.SetGameTimeSpeedLevel(0);

                                                                                    Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed = Sims3.SimIFace.Gameflow.GameSpeed.Pause;
                                                                                    ProgressDialog.Show("Deleteing SimDescriptions...", false);

                                                                                    //Sims3.Gameplay.Gameflow.SetGameSpeed(Sims3.SimIFace.Gameflow.GameSpeed.Pause, false);
                                                                                    NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                    NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                }

                                                                                //NiecMod.Nra.SpeedTrap.Sleep(0u);
                                                                                mposition = target.Position;
                                                                            }

                                                                            catch
                                                                            { }

                                                                            if (target.LotCurrent.IsWorldLot)
                                                                            {
                                                                                try
                                                                                {
                                                                                    Sim activeHouseholdselectedActor = Sim.ActiveActor;
                                                                                    if (activeHouseholdselectedActor != null)
                                                                                    {
                                                                                        Mailbox mailboxOnHomeLot = Mailbox.GetMailboxOnHomeLot(activeHouseholdselectedActor);
                                                                                        if (mailboxOnHomeLot != null)
                                                                                        {
                                                                                            Vector3 vector2;
                                                                                            World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(mailboxOnHomeLot.Position);
                                                                                            fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                                                                            fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                                                                                            if (!GlobalFunctions.FindGoodLocation(target, fglParams, out mposition, out vector2))
                                                                                            {
                                                                                                target.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                                                                                mposition = target.Position;
                                                                                            }
                                                                                        }
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }



                                                                            }
                                                                            else
                                                                            {
                                                                                mposition = target.Position;
                                                                            }

                                                                            if (target.LotCurrent.IsWorldLot)
                                                                            {
                                                                                target.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                                                                mposition = target.Position;
                                                                            }
                                                                            try
                                                                            {

                                                                                //Sleep(3.0);
                                                                                foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX())
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Contains(sdtyf))
                                                                                            Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Add(sdtyf);

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                //Sleep(3.0);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!(allService is GrimReaper))
                                                                                        {
                                                                                            allService.ClearServicePool();
                                                                                        }

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            try
                                                                            {
                                                                                //Sleep(3.0);
                                                                                foreach (Service allService in Services.AllServices)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        if (!(allService is GrimReaper))
                                                                                        {
                                                                                            Service.Destroy(allService);
                                                                                        }

                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                            }
                                                                            catch
                                                                            { }
                                                                            //Sleep(3.0);
                                                                            try
                                                                            {
                                                                                PlumbBob.ForceSelectActor(null);
                                                                            }
                                                                            catch
                                                                            { }





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
                                                                                        try
                                                                                        {
                                                                                            NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(simaue);
                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                        try
                                                                                        {
                                                                                            simaue.Genealogy.ClearAllGenealogyInformation();

                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                        try
                                                                                        {
                                                                                            simaue.Genealogy.ClearMiniSimDescription();
                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                        try
                                                                                        {
                                                                                            simaue.Destroy();
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
                                                                            finally
                                                                            {

                                                                                try
                                                                                {
                                                                                    asdo.Clear();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                asdo = null;
                                                                            }












                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {
                                                                                try
                                                                                {
                                                                                    if (description == null) continue;
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        //Sleep(0.0);
                                                                                        NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(description.CreatedSim);
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearMiniSimDescription();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearDerivedData();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearSimDescription();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    description.Genealogy.ClearAllGenealogyInformation();
                                                                                }
                                                                                catch
                                                                                { }




                                                                                try
                                                                                {
                                                                                    Household household = description.Household;
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                }
                                                                                catch
                                                                                { }

                                                                                /*
                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }
                                                                                */
                                                                                try
                                                                                {
                                                                                    while (true)
                                                                                    {
                                                                                        Urnstone urnstone = HelperNra.TFindGhostsGrave(description);
                                                                                        if (urnstone != null)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                urnstone.Dispose();
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            try
                                                                                            {
                                                                                                urnstone.Destroy();
                                                                                            }
                                                                                            catch
                                                                                            { }

                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    MiniSimDescription.RemoveMSD(description.SimDescriptionId);
                                                                                }
                                                                                catch
                                                                                { }



                                                                                try
                                                                                {
                                                                                    description.Dispose();
                                                                                }
                                                                                catch
                                                                                { }

                                                                                try
                                                                                {
                                                                                    NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                                                                                }
                                                                                catch
                                                                                { }

                                                                            }

                                                                            try
                                                                            {
                                                                                try
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        SimDescription.sLoadedSimDescriptions.Clear();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    SimDescription.sLoadedSimDescriptions = null;
                                                                                    SimDescription.sLoadedSimDescriptions = new List<SimDescription>();

                                                                                }
                                                                                catch
                                                                                {
                                                                                    StyledNotification.Format afra = new StyledNotification.Format("Failed 1", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                    StyledNotification.Show(afra);
                                                                                }
                                                                                try
                                                                                {
                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();

                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions = null;
                                                                                    Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions = new List<SimDescription>();
                                                                                }
                                                                                catch
                                                                                {
                                                                                    StyledNotification.Format afra = new StyledNotification.Format("Failed 2", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                    StyledNotification.Show(afra);
                                                                                }
                                                                                if (flagsae)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        //MiniSimDescription.Clear();
                                                                                        MiniSimDescription.sMiniSims.Clear();

                                                                                        MiniSimDescription.sMiniSims = null;
                                                                                        MiniSimDescription.sMiniSims = new Dictionary<ulong, MiniSimDescription>();
                                                                                    }
                                                                                    catch
                                                                                    {
                                                                                        StyledNotification.Format afra = new StyledNotification.Format("Failed 3", StyledNotification.NotificationStyle.kSystemMessage);
                                                                                        StyledNotification.Show(afra);
                                                                                    }
                                                                                }
                                                                                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                        StyledNotification.Format afra = new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kGameMessageNegative);
                                                                                        afra.mTNSCategory = NotificationManager.TNSCategory.Chatty;
                                                                                        //StyledNotification.Show(new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kSystemMessage));
                                                                                        StyledNotification.Show(afra);
                                                                                    }
                                                                                    catch
                                                                                    { }



                                                                                    try
                                                                                    {
                                                                                        if (sdae != null)
                                                                                        {
                                                                                            Nra.NFinalizeDeath.mResetError = false;
                                                                                            Simulator.DestroyObject(sdae);
                                                                                        }
                                                                                    }
                                                                                    catch
                                                                                    { }
                                                                                    if (flagsaea)
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            
                                                                                            Sim sim = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                            if (sim != null)
                                                                                            {

                                                                                                try
                                                                                                {
                                                                                                    //sim.SimDescription.Household.SetName("E3Lesa is Good Good Household");
                                                                                                    sim.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                                                                                    PlumbBob.ForceSelectActor(sim);
                                                                                                    //Service.NeedsAssignment(target.LotCurrent);
                                                                                                    try
                                                                                                    {
                                                                                                        if (sim.SimDescription.Child || sim.SimDescription.Teen)
                                                                                                        {
                                                                                                            sim.SimDescription.AssignSchool();
                                                                                                        }

                                                                                                    }
                                                                                                    catch
                                                                                                    { }
                                                                                                    try
                                                                                                    {
                                                                                                        if (sim.IsSelectable)
                                                                                                        {
                                                                                                            sim.OnBecameSelectable();
                                                                                                        }
                                                                                                    }
                                                                                                    catch
                                                                                                    { }

                                                                                                }
                                                                                                catch (Exception ex2)
                                                                                                {
                                                                                                    NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                                }
                                                                                                try
                                                                                                {
                                                                                                    if (sim.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                                    {
                                                                                                        sim.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                        sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                                    }
                                                                                                    sim.AddInitialObjects(sim.IsSelectable);
                                                                                                }
                                                                                                catch
                                                                                                { }
                                        
                                                                                                if (flagsa)
                                                                                                {
                                                                                                    ProgressDialog.Close();
                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + sim.SimDescription.LastName;

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }
                                                                                            
                                                                                            if (NiecMod.Helpers.Create.CreateActiveHouseholdAndActiveActor())
                                                                                            {
                                                                                                if (flagsa)
                                                                                                {

                                                                                                    try
                                                                                                    {
                                                                                                        ProgressDialog.Close();
                                                                                                    }
                                                                                                    catch
                                                                                                    { }
                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + PlumbBob.Singleton.mSelectedActor.SimDescription.LastName;

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }











                                                                                            //else if (Sim.ActiveActor == null)
                                                                                            //if (Sim.ActiveActor == null)
                                                                                            else if (PlumbBob.Singleton.mSelectedActor == null)
                                                                                            {
                                                                                                try
                                                                                                {
                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                    if (flagsa)
                                                                                                    {

                                                                                                        try
                                                                                                        {
                                                                                                            ProgressDialog.Close();
                                                                                                        }
                                                                                                        catch
                                                                                                        { }

                                                                                                    }
                                                                                                    OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                    if (optionsModel != null)
                                                                                                    {
                                                                                                        optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                        optionsModel.SaveGame(true, true, false);
                                                                                                    }
                                                                                                }
                                                                                                catch
                                                                                                { }
                                                                                                finally
                                                                                                {
                                                                                                    CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                }


                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                                if (flagsa)
                                                                                                {
                                                                                                    try
                                                                                                    {
                                                                                                        ProgressDialog.Close();
                                                                                                    }
                                                                                                    catch
                                                                                                    { }

                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                        catch
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate false");
                                                                                                CommandSystem.ExecuteCommandString("makesim");

                                                                                                if (flagsa)
                                                                                                {
                                                                                                    ProgressDialog.Close();
                                                                                                }
                                                                                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                                if (optionsModel != null)
                                                                                                {
                                                                                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                                    optionsModel.SaveGame(true, true, false);
                                                                                                }
                                                                                            }
                                                                                            catch
                                                                                            { }
                                                                                            finally
                                                                                            {
                                                                                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        try
                                                                                        {
                                                                                            CommandSystem.ExecuteCommandString("dgsnocreate true");
                                                                                        }
                                                                                        catch
                                                                                        { }
                                                                                        if (flagsa)
                                                                                        {
                                                                                            ProgressDialog.Close();
                                                                                        }
                                                                                        NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                        OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                                                                        if (optionsModel != null)
                                                                                        {
                                                                                            optionsModel.SaveName = "ClearSave " + "No Name";

                                                                                        }
                                                                                        NiecMod.Nra.SpeedTrap.Sleep(3);
                                                                                        try
                                                                                        {
                                                                                            GameStates.TransitionToEditTown();
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }







                                                                                }
                                                                            }
                                                                            catch (Exception ex)
                                                                            {
                                                                                NiecException.PrintMessage("Force Delete All SimDescriptions? Clear Keep Save Not Active " + ex.Message);
                                                                                if (!(ex is ResetException))
                                                                                {
                                                                                    Common.Exception("Force Delete All SimDescriptions? Clear Keep Save Not Active", ex);
                                                                                }
                                                                                if (flagsa)
                                                                                {
                                                                                    try
                                                                                    {
                                                                                        ProgressDialog.Close();
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }

                                                                                if (Sim.ActiveActor == null)
                                                                                {
                                                                                    Sim simar = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                                                                    if (simar != null)
                                                                                    {

                                                                                        try
                                                                                        {
                                                                                            //sim.SimDescription.Household.SetName(/* "E3Lesa is Good" */ "Good Household");
                                                                                            simar.SimDescription.Household.SetName(simar.SimDescription.LastName);
                                                                                            PlumbBob.ForceSelectActor(simar);
                                                                                        }
                                                                                        catch (Exception ex2)
                                                                                        {
                                                                                            NiecMod.Nra.NiecException.WriteLog("SetName Household" + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                                                                        }
                                                                                        try
                                                                                        {
                                                                                            if (simar.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                                                                            {
                                                                                                simar.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                                                                simar.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                                                                            }
                                                                                            simar.AddInitialObjects(simar.IsSelectable);
                                                                                        }
                                                                                        catch
                                                                                        { }

                                                                                    }
                                                                                }
                                                                                return false;
                                                                            }
                                                                            Simulator.YieldingDisabled = false;
                                                                            if (flagsaea)
                                                                            {
                                                                                Sims3.Gameplay.UI.Responder.Instance.ClockSpeedModel.CurrentGameSpeed = currentGameSpeed;
                                                                            }


                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Not UsStone?"))
                                                                        {
                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    try
                                                                                    {
                                                                                        if (description.IsGhost && description.DeathStyle != SimDescription.DeathType.None) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                    {
                                                                                        continue;
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                                                                    listr.Add(SimDescription.DeathType.Drown);
                                                                                    listr.Add(SimDescription.DeathType.Starve);
                                                                                    listr.Add(SimDescription.DeathType.Thirst);
                                                                                    listr.Add(SimDescription.DeathType.Burn);
                                                                                    listr.Add(SimDescription.DeathType.Freeze);
                                                                                    listr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                    listr.Add(SimDescription.DeathType.Shark);
                                                                                    listr.Add(SimDescription.DeathType.Jetpack);
                                                                                    listr.Add(SimDescription.DeathType.Meteor);
                                                                                    listr.Add(SimDescription.DeathType.Causality);
                                                                                    listr.Add(SimDescription.DeathType.Electrocution);
                                                                                    if (description.Elder)
                                                                                    {
                                                                                        listr.Add(SimDescription.DeathType.OldAge);
                                                                                    }
                                                                                    listr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);

                                                                                    Urnstone mGravebackup = HelperNra.TFindGhostsGrave(description);
                                                                                    if (mGravebackup != null)
                                                                                    {
                                                                                        if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(target.Position), false))
                                                                                        {
                                                                                            mGravebackup.SetPosition(target.Position);
                                                                                        }
                                                                                        mGravebackup.OnHandToolMovement();
                                                                                    }



                                                                                    Household household = description.Household;
                                                                                    if (description.CreatedSim != null)
                                                                                    {
                                                                                        description.CreatedSim.Destroy();
                                                                                    }
                                                                                    if (household != null)
                                                                                    {
                                                                                        household.Remove(description, !household.IsSpecialHousehold);
                                                                                    }

                                                                                    if (description.DeathStyle == SimDescription.DeathType.None)
                                                                                    {
                                                                                        if (description.IsHuman)
                                                                                        {
                                                                                            switch (randomObjectFromList)
                                                                                            {
                                                                                                case SimDescription.DeathType.None:
                                                                                                case SimDescription.DeathType.PetOldAgeBad:
                                                                                                case SimDescription.DeathType.PetOldAgeGood:
                                                                                                    randomObjectFromList = SimDescription.DeathType.OldAge;
                                                                                                    break;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            switch (randomObjectFromList)
                                                                                            {
                                                                                                case SimDescription.DeathType.None:
                                                                                                case SimDescription.DeathType.OldAge:
                                                                                                    randomObjectFromList = SimDescription.DeathType.PetOldAgeGood;
                                                                                                    break;
                                                                                            }
                                                                                        }

                                                                                        description.SetDeathStyle(randomObjectFromList, true);
                                                                                    }

                                                                                    description.IsNeverSelectable = true;
                                                                                    description.Contactable = false;
                                                                                    description.Marryable = false;


                                                                                    if (description.CreatedSim != null && description.CreatedSim == PlumbBob.SelectedActor)
                                                                                    {
                                                                                        LotManager.SelectNextSim();
                                                                                    }
                                                                                    if (description.CareerManager != null)
                                                                                    {
                                                                                        description.CareerManager.LeaveAllJobs(Career.LeaveJobReason.kDied);
                                                                                    }

                                                                                    mGravebackup.OnHandToolMovement();

                                                                                    int num = ((int)Math.Floor((double)SimClock.ConvertFromTicks(SimClock.CurrentTime().Ticks, TimeUnit.Minutes))) % 60;
                                                                                    mGravebackup.MinuteOfDeath = num;

                                                                                    try
                                                                                    {
                                                                                        Urnstone.FinalizeSimDeath(description, household);
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                }
                                                                                catch
                                                                                { }

                                                                            }
                                                                            //return true;
                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Done UsStone?"))
                                                                        {
                                                                            foreach (SimDescription description in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                                                                            {

                                                                                try
                                                                                {
                                                                                    //Name is
                                                                                    try
                                                                                    {
                                                                                        if (description.Household == Household.ActiveHousehold) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    try
                                                                                    {
                                                                                        if (description.IsGhost && description.DeathStyle != SimDescription.DeathType.None) continue;
                                                                                    }
                                                                                    catch
                                                                                    { }

                                                                                    if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                    {
                                                                                        continue;
                                                                                    }

                                                                                    if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                    {
                                                                                        continue;
                                                                                    }
                                                                                }
                                                                                catch
                                                                                { }
                                                                                try
                                                                                {
                                                                                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                                                                    listr.Add(SimDescription.DeathType.Drown);
                                                                                    listr.Add(SimDescription.DeathType.Starve);
                                                                                    listr.Add(SimDescription.DeathType.Thirst);
                                                                                    listr.Add(SimDescription.DeathType.Burn);
                                                                                    listr.Add(SimDescription.DeathType.Freeze);
                                                                                    listr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                    listr.Add(SimDescription.DeathType.Shark);
                                                                                    listr.Add(SimDescription.DeathType.Jetpack);
                                                                                    listr.Add(SimDescription.DeathType.Meteor);
                                                                                    listr.Add(SimDescription.DeathType.Causality);
                                                                                    listr.Add(SimDescription.DeathType.Electrocution);
                                                                                    if (description.Elder)
                                                                                    {
                                                                                        listr.Add(SimDescription.DeathType.OldAge);
                                                                                    }
                                                                                    listr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);
                                                                                    //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                    //simDeathType = randomObjectFromList;
                                                                                    Urnstones.CreateGrave(description, randomObjectFromList, false, true);
                                                                                }
                                                                                catch
                                                                                { }

                                                                            }
                                                                        }
                                                                        else if (AcceptCancelDialog.Show("Not NiecSimDesc"))
                                                                        {
                                                                            List<SimDescription> list = new List<SimDescription>();
                                                                            foreach (SimDescription sd in NFinalizeDeath.TattoaX())
                                                                            {
                                                                                if (sd.Household != Household.ActiveHousehold && !(sd.Service is GrimReaper) && !sd.IsDead && !sd.IsGhost) //OK
                                                                                {
                                                                                    list.Add(sd);
                                                                                }
                                                                            }
                                                                            if (list.Count > 0)
                                                                            {
                                                                                foreach (SimDescription description in list)
                                                                                {

                                                                                    try
                                                                                    {
                                                                                        //Name is
                                                                                        if (description.FirstName == "Death" && description.LastName == "Good System")
                                                                                        {
                                                                                            continue;
                                                                                        }

                                                                                        if (description.FirstName == "Good System" && description.LastName == "Death Helper")
                                                                                        {
                                                                                            continue;
                                                                                        }

                                                                                        if (description.FirstName == "Grim" && description.LastName == "Reaper")
                                                                                        {
                                                                                            continue;
                                                                                        }
                                                                                    }
                                                                                    catch (NullReferenceException)
                                                                                    { }

                                                                                    List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();
                                                                                    listr.Add(SimDescription.DeathType.Drown);
                                                                                    listr.Add(SimDescription.DeathType.Starve);
                                                                                    listr.Add(SimDescription.DeathType.Thirst);
                                                                                    listr.Add(SimDescription.DeathType.Burn);
                                                                                    listr.Add(SimDescription.DeathType.Freeze);
                                                                                    listr.Add(SimDescription.DeathType.ScubaDrown);
                                                                                    listr.Add(SimDescription.DeathType.Shark);
                                                                                    listr.Add(SimDescription.DeathType.Jetpack);
                                                                                    listr.Add(SimDescription.DeathType.Meteor);
                                                                                    listr.Add(SimDescription.DeathType.Causality);
                                                                                    listr.Add(SimDescription.DeathType.Electrocution);
                                                                                    if (target.SimDescription.Elder)
                                                                                    {
                                                                                        listr.Add(SimDescription.DeathType.OldAge);
                                                                                    }
                                                                                    listr.Add(SimDescription.DeathType.HauntingCurse);
                                                                                    SimDescription.DeathType randomObjectFromList = RandomUtil.GetRandomObjectFromList(listr);
                                                                                    //Target.SimDescription.SetDeathStyle(randomObjectFromList, Target.IsSelectable);
                                                                                    //simDeathType = randomObjectFromList;
                                                                                    Urnstones.CreateGrave(description, randomObjectFromList, true, true);
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            return false;
                                                                        }

                                                                        /*
                                                                        
                                                                         */

                                                                        return true;
                                                                    }

                                                                    catch (Exception ex)
                                                                    {

                                                                        NiecException.PrintMessage(ex.Message);
                                                                        Common.Exception(target, ex);
                                                                    }
                                                                    return false;
                                                                }
                                                            }
                                                        }

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
                                                    ScubaDiving scubaDivinga = new ScubaDiving(asdoei.mCurrentStateMachine, Sims3.Gameplay.Pools.Ocean.Singleton, mSim);
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
                                    mSim.SimRoutingComponent.DisableDynamicFootprint();

                                    mSim.SimRoutingComponent.EnableDynamicFootprint();

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

                                AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(AssemblyCheckByNiec.Atiao), "FailedCallBookSleep " + mSim.FullName, AlarmType.AlwaysPersisted, null);

                                return true;
                            }






                            // End Fake
                        }
                    }
                    target.FadeIn();
                    if (NTunable.kDedugNotificationCheckNiecKill)
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill" + NL + "Name: " + target.Name + NL + "No CheckNiecKill Sorry,", StyledNotification.NotificationStyle.kGameMessagePositive));
                    }
                    if (checkkillsimcrach)
                    {
                        Sim.MakeSimGoHome(target, true);
                    }
                    return false;
                }
                try
                {
                    if (NTunable.kDedugNiecModExceptionMineKill)
                    {
                        throw new NiecModException("MineKill: Not Error");
                    }
                }
                catch (NiecModException ex)
                {
                    if (NTunable.kDedugNiecModExceptionMineKill)
                    {
                        Common.Exception(target, ex);
                        NiecException.PrintMessage(ex.Message + NiecException.NewLine + ex.StackTrace);
                    }
                }

                // DGS: ... 
                // Too Many Add Interaction Toddler InCrib Fix

                try
                {
                    if (target.SimDescription.Age == CASAgeGenderFlags.Toddler || target.SimDescription.Age == CASAgeGenderFlags.Baby)
                    {
                        Sim asdta = null;
                        ulong newidsimdsc = 0L;
                        SimDescription simdescfixllop = null;
                        ICrib crib = target.Posture.Container as ICrib;
                        if (target.Posture.Satisfies(CommodityKind.InCrib, null) || crib != null)
                        {


                            
                            






                            newidsimdsc = target.SimDescription.SimDescriptionId;

                            try
                            {
                                target.SetObjectToReset();
                            }
                            catch
                            { }

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
                                if (simdescfixllop.CreatedSim == null)
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
                                if (simdescfixllop.CreatedSim != null)
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


                    ExtKillSimNiec killSim = ExtKillSimNiec.Singleton.CreateInstance(target, target, DGSAndNonDGSPriority(), false, true) as ExtKillSimNiec;
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


                    killSim.sWasIsActiveHousehold = (NFinalizeDeath.IsSimFastActiveHousehold(target));
                    target.InteractionQueue.AddNext(killSim);

                    // UnTested Active Household Empty Interaction
                    /*
                    CCnlean cCnlean = CCnlean.Singleton.CreateInstance(target, target, NipHelper.AutonomousCheckWithActiveHousehold(), false, true) as CCnlean;
                    target.InteractionQueue.Add(cCnlean);
                    */

                    /*
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
                    */
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
                else
                {
                    //target.AddExitReason(ExitReason.CanceledByScript);
                    try
                    {
                        if (target == null || target.SimDescription.CreatedSim == null)
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
                        catch (Exception)
                        {/* CheckAntiCancel = false; */}

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
                        catch (Exception)
                        {/* CheckAntiCancel = false; */}
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
                        //StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: " + exception.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
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

                    /*
                    try
                    {
                        NiecException.PrintMessage(NiecException.InteractionList(target.SimDescription));
                        target.EnableInteractions();
                    }
                    catch (Exception)
                    { }
                     * 
                    */


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
                    
                    try
                    {

                        HelperNra helperNra = new HelperNra();

                        //helperNra = HelperNra;

                        helperNra.mSim = target;

                        helperNra.mSimdesc = target.SimDescription;

                        helperNra.mdeathtype = deathType;

                        AlarmManager.Global.AddAlarm(1f, TimeUnit.Days, new AlarmTimerCallback(helperNra.CheckKillSimNotUrnstonePro), "MineKillCheckKillSim Name " + target.SimDescription.LastName, AlarmType.AlwaysPersisted, null);



                    }
                    catch (Exception exception)
                    { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }
                    

                    


                    // TESTED!!!!!!!!!!!!!!!!!!!!!!!!!!!! \\
                    /*
                    try
                    {
                        foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                        {
                            if (interactionInstance.GetPriority().Level > (InteractionPriorityLevel)13) //  DGSCore :)
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Level UP Level :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                        }
                    }
                    catch (Exception)
                    {}
                    */

                    /*
                    try
                    {
                        foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList)
                        { 
                            if (interactionInstance.GetPriority().Level < (InteractionPriorityLevel)999) // Low Level ReNPCR Loop
                            {
                                StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Level Down Level :)", StyledNotification.NotificationStyle.kGameMessagePositive));
                            }
                        }
                    }
                    catch (Exception)
                    {}

                    */

                    

                    /*

                    if (!CheckNiecKill(target))
                    {
                        if (deathType == SimDescription.DeathType.OldAge)
                        {
                            if (!target.SimDescription.IsEP11Bot)
                            {
                                if (target.SimDescription.Elder)
                                {
                                    target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                }
                            }
                        }
                        if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                        {
                            if (deathType == SimDescription.DeathType.Thirst)
                            {
                                if (!target.SimDescription.IsEP11Bot)
                                {
                                    if (target.SimDescription.Elder)
                                    {
                                        target.SimDescription.AgingState.ResetAndExtendAgingStage(0f);
                                    }
                                }
                            }
                        }
                        target.FadeIn();
                        if (NTunable.kDedugNotificationCheckNiecKill)
                        {
                            StyledNotification.Show(new StyledNotification.Format("MineKill: Sorry, " + target.Name + " No CheckNiecKill", StyledNotification.NotificationStyle.kGameMessagePositive));
                        }
                        return false;
                    }
                     */
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
                    
                    /*
                    if (!target.Inventory.IsEmpty)
                    {
                        Sim sim = target;
                        foreach (Sim sim2 in Household.ActiveHousehold.Sims)
                        {
                            if (sim2 != target && (sim2.SimDescription.DeathStyle == SimDescription.DeathType.None || sim2.SimDescription.IsPlayableGhost) && (sim == null || sim2.SimDescription.Age > sim.SimDescription.Age) && (Household.RoommateManager == null || Household.RoommateManager.IsNPCRoommate(sim2)))
                            {
                                sim = sim2;
                            }
                        }
                        target.MoveInventoryItemsToSim(sim);
                    }
                    */
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
                    /*
                
                    try
                    {
                        MineKillForceCancelInteraction(target);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    */

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
                        NFinalizeDeath.ForceCancelAllInteractionsPro(target);
                    }
                    catch
                    {
                        try
                        {
                            //target.InteractionQueue.CancelAllInteractions();
                            NFinalizeDeath.ForceCancelAllInteractionsPro(target);
                        }
                        catch
                        { }
                    }

                    try
                    {
                        if (AssemblyCheckByNiec.IsInstalled("DGSCore")) // Anti-CallCallbackOnFailure NPC Only
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
                        NFinalizeDeath.ForceCancelAllInteractionsPro(target);
                    }
                    catch (Exception)
                    {
                        try
                        {
                            //target.InteractionQueue.CancelAllInteractions();
                            NFinalizeDeath.ForceCancelAllInteractionsPro(target);
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


                    
                    
                    /*
                    if (target.Service is GrimReaper)
                    {
                        ExtKillSimNiecNoGrim killSimGrimReaper = ExtKillSimNiecNoGrim.Singleton.CreateInstance(target, target, new InteractionPriority((InteractionPriorityLevel)100, 0f), false, true) as ExtKillSimNiecNoGrim;
                        killSimGrimReaper.simDeathType = deathType;
                        killSimGrimReaper.PlayDeathAnimation = playDeathAnim;
                        killSimGrimReaper.MustRun = true;
                        target.InteractionQueue.AddNextIfPossibleAfterCheckingForDuplicates(killSimGrimReaper);
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
                        return false;
                    }
                     */
                    /*
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
                     * */
                    //ExtKillSimNiec killSim = ExtKillSimNiec.Singleton.CreateInstance(target, target, new InteractionPriority((InteractionPriorityLevel)100, 0f), false, false) as ExtKillSimNiec;

                    try
                    {
                        ExtKillSimNiec killSim = ExtKillSimNiec.Singleton.CreateInstance(target, target, DGSAndNonDGSPriority(), false, false) as ExtKillSimNiec;
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
                        //target.InteractionQueue.PushAsContinuation(killSim, true);

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
                            //Callback failedkill;
                            var killSim2 = ExtKillSimNiec.Singleton.CreateInstance(target, target, DGSAndNonDGSPriority(), false, false) as ExtKillSimNiec;
                            killSim2.simDeathType = deathType;
                            killSim2.PlayDeathAnimation = playDeathAnim;
                            killSim2.MustRun = true;
                            killSim2.Hidden = true;
                            //killSim2.mForceKillGrim = true;
                            try
                            {
                                killSim2.mForceKillGrim = (target.Service is GrimReaper);
                                //failedkill = killSim2.FailedCallBook;
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
                    

                    // UnTested No Active Household
                    /*
                    try
                    {
                        NiecException.PrintMessage(NiecException.InteractionList(target.SimDescription));
                    }
                    catch (Exception)
                    { }
                    */

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
                            {
                            }
                            DeathTypeNoneNotAllow = false;
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
                    /*
                    foreach (InteractionInstance interactionInstance in target.InteractionQueue.InteractionList) // Cant Cancel Fix
                    {
                        //if (!Actor.InteractionQueue.HasInteractionOfType(Singleton) && !Actor.InteractionQueue.HasInteractionOfType(KillSimNiecX.NiecDefinitionDeathInteraction.SocialSingleton) && !Actor.InteractionQueue.HasInteractionOfType(NiecDefinitionDeathInteractionSocialSingleton) && !Actor.InteractionQueue.HasInteractionOfType(GrimReaperSituation.BeReapedDiving.Singleton))
                        if (!(interactionInstance.InteractionDefinition is SocialInteractionB.DefinitionDeathInteraction) && !(interactionInstance.InteractionDefinition is ExtKillSimNiec.Definition) && !(interactionInstance.InteractionDefinition is KillSimNiecX.NiecDefinitionDeathInteraction) && !(interactionInstance.InteractionDefinition is GrimReaperSituation.BeReapedDiving.Definition))
                        {
                            interactionInstance.SetPriority((InteractionPriorityLevel)0, 0f);
                            interactionInstance.MustRun = false;
                            interactionInstance.Hidden = false;
                        }
                    }
                     */
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
                //throw;
            }
            catch (OutOfMemoryException)
            {
                return false;
                //throw;
            }
            
            catch (ResetException)
            {
                //NiecException.WriteLog("MineKill " + NiecException.NewLine + NiecException.LogException(exception), true, true, false);
                //Common.Exception(target, exception);
                if (target.IsInActiveHousehold)
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
                    //StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: " + exception.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
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
                    //Common.Exception(target, exception);
                    if (target.IsInActiveHousehold)
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
                        //StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: " + exception.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
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
                            /*
                            if (target.SimDescription.Household != null)
                            {
                                ExtKillSimNiec.ListMorunExtKillSim(target, deathType);
                            }
                             */
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
                /*
                if (DeathTypeNoneNotAllow)
                {
                    NiecException.PrintMessage(exception.Message + exception.Source + exception.InnerException + exception.StackTrace + exception.Data);
                    Common.Exception(target, exception);
                    if (target.IsInActiveHousehold)
                    {
                        StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: " + exception.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
                        return false;
                    }
                    if (!target.IsInActiveHousehold)
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
                        StyledNotification.Show(new StyledNotification.Format("MineKill: " + target.Name + " Failed Error: " + exception.Message, StyledNotification.NotificationStyle.kGameMessagePositive));
                        target.MoveInventoryItemsToAFamilyMember();
                        target.SimDescription.SetDeathStyle(deathType, false);
                        Urnstone.FinalizeSimDeath(target.SimDescription, target.Household);
                        Urnstones.CreateGrave(target.SimDescription, deathType, true, true);
                    }
                }
                */
                
            }
            //return true;
        }
        public static bool DeathTypeNoneNotAllow = true;
        public static bool DeathTypeFixMineKill = true;

        

        /// <summary>
        /// This Aotsa Aotos
        /// </summary>
        /// <param name="targetSim"></param>
        /// <returns></returns>
        public static bool CheckNiecKill(Sim targetSim)
        {
            try
            {
                if (targetSim.SimDescription.IsGhost) return false;
                if (targetSim.SimDescription.IsPlayableGhost) return false;
                if (targetSim.SimDescription.IsDead) return false;
                if (targetSim.InteractionQueue == null)
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

                    if (targetSim.InteractionQueue == null) return false;
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


                /*
                try
                {
                    var killSim = ExtKillSimNiec.Singleton.CreateInstance(targetSim, targetSim, DGSAndNonDGSPriority(), false, true) as ExtKillSimNiec;
                    if (targetSim.InteractionQueue.HasInteraction(killSim)) return false;
                }
                catch (NullReferenceException)
                { }
                */

                

                //if (targetSim.InteractionQueue.HasInteractionOfType(ExtKillSimNiecNoGrim.Singleton)) return false;
                if (targetSim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton)) return false;

                
                
                foreach (InteractionInstance interactionInstance in targetSim.InteractionQueue.InteractionList)
                {
                    if (interactionInstance is Urnstone.KillSim)
                    {
                        return false;
                    }
                }
                //if (targetSim.LotCurrent.IsWorldLot) return false;
                if (targetSim.Service is GrimReaper)
                {
                    if (!AcceptCancelDialog.Show("CheckNiecKill: Killing the " + targetSim.Name + " [GrimReaper] will prevent souls to cross over to the other side. If this happens, Sims that die from now on will be trapped between this world and the next, and you'll end up with a city full of dead bodies laying around. Are you sure you want to kill Death itself?", true))
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                if (!AcceptCancelDialog.Show("CheckNiecKill: Name (" + targetSim.Name + ") Found Error (" + ex.Message + ") Do want you run? [Yes Run or No Cancel]", true))
                {
                    return false;
                }
            }
            /*
            if (targetSim.IsInActiveHousehold)
            {
                if (!AcceptCancelDialog.Show("Killing the Active Household Sims?", true))
                {
                    return false;
                }
            }
             */

            return true;
        }

        public static bool EACanBeKilledExByNiec(Sim targetSim)
        {
            try
            {
                OccultImaginaryFriend occultImaginaryFriend;
                if (OccultImaginaryFriend.TryGetOccultFromSim(targetSim, out occultImaginaryFriend) && !occultImaginaryFriend.IsReal)
                {
                    return false;
                }
                if (targetSim.SimDescription.AssignedRole is NPCAnimal)
                {
                    return false;
                }
                if (targetSim.OccultManager.HasOccultType(OccultTypes.TimeTraveler))
                {
                    return false;
                }
                if (HolographicProjectionSituation.IsSimHolographicallyProjected(targetSim))
                {
                    return false;
                }
                if (targetSim.SimDescription.IsPregnant || targetSim.SimDescription.IsGhost || targetSim.InteractionQueue == null || targetSim.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton) || (ServiceSituation.IsSimOnJob(targetSim) && !targetSim.SimDescription.CanBeKilledOnJob))
                {
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
                                return true;
                            }
                        }
                    }
                    return false;
                }
                if (targetSim.LotCurrent.IsDivingLot && !(targetSim.Posture is ScubaDiving))
                {
                    return false;
                }
                return !targetSim.LotCurrent.IsWorldLot;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage("CanBeKilled By EA" + ex.Message + "/n " + ex.StackTrace);
                /*
                if (!AcceptCancelDialog.Show("EA CanBeKilled By Niec: Name (" + targetSim.Name + ") Found Error (" + ex.Message + ") Do want you run? [Yes Run or No Cancel]", true))
                {
                    return false;
                }
                */
            }
            return false;
        }
        /// <summary>
        /// This A Focre Cancel
        /// </summary>
        /// <param name="mineactor">Actor and Target Sim</param>
        public static void MineKillForceCancelInteraction(Sim mineactor)
        {
            try
            {
                foreach (InteractionInstance interactionInstance in mineactor.InteractionQueue.InteractionList) // Cant Cancel Fix
                {
                    interactionInstance.MustRun = false;
                    interactionInstance.Hidden = false;
                }

                //mineactor.InteractionQueue.OnReset();
                mineactor.InteractionQueue.CancelAllInteractions();

                // Test 1
                {

                    //mineactor.InteractionQueue.OnReset();

                }

                {
                    mineactor.InteractionQueue.CancelAllInteractions(); // Cancel All Interactions Byasss Anti-Cancel 
                }

                // Test 2




                // Test 3
                {
                    mineactor.InteractionQueue.OnReset(); // Sim Interactions is Reset
                    mineactor.InteractionQueue.CancelAllInteractions();
                }
                // Test 4
                {
                    mineactor.InteractionQueue.CancelAllInteractions(); // Cancel All Interactions Byasss Anti-Cancel 
                    mineactor.InteractionQueue.OnReset();
                }
                foreach (InteractionInstance interactionInstance in mineactor.InteractionQueue.InteractionList) // Cant Cancel Fix
                {
                    interactionInstance.MustRun = false;
                    interactionInstance.Hidden = false;
                }
                {
                    mineactor.InteractionQueue.CancelAllInteractions();
                }

                {
                    mineactor.InteractionQueue.OnReset();
                }

                {
                    mineactor.InteractionQueue.CancelAllInteractions();
                }

                {
                    mineactor.InteractionQueue.CancelAllInteractions();
                }
                {
                    mineactor.InteractionQueue.OnReset();
                }
                //mineactor.OnReset();
            }
            catch (ResetException ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                Common.Exception(mineactor, ex);
                foreach (InteractionInstance interactionInstance in mineactor.InteractionQueue.InteractionList) // Cant Cancel Fix
                {
                    interactionInstance.MustRun = false;
                    interactionInstance.Hidden = false;
                }
                mineactor.InteractionQueue.CancelAllInteractions();
                throw;
            }
            catch (Exception ex)
            {
                NiecException.PrintMessage(ex.Message + " " + ex.StackTrace);
                Common.Exception(mineactor, ex);
                foreach (InteractionInstance interactionInstance in mineactor.InteractionQueue.InteractionList) // Cant Cancel Fix
                {
                    interactionInstance.MustRun = false;
                    interactionInstance.Hidden = false;
                }
                mineactor.InteractionQueue.OnReset();
                mineactor.InteractionQueue.CancelAllInteractions();
                throw;
            }
        }

        public static InteractionPriority DGSAndNonDGSPriority()
        {
            try
            {
                InteractionPriority dgsandnonsgsa = new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f);
                if (AssemblyCheckByNiec.IsInstalled("DGSCore") && !NTunable.kForceMaxDeath)
                {
                    dgsandnonsgsa = new InteractionPriority((InteractionPriorityLevel)100, 0f); // InteractionPriorityLevel.MaxExpireDeathX
                }
                else
                {
                    dgsandnonsgsa = new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f);
                }
                return dgsandnonsgsa;
            }
            catch (Exception)
            {
                InteractionPriority dgsandnonsgsax = new InteractionPriority(InteractionPriorityLevel.MaxDeath, 0f);
                return dgsandnonsgsax;
            }
        }

        [DoesntRequireTuning]

        public class NiecDefinitionDeathInteraction : SocialInteractionB.Definition, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public static readonly InteractionDefinition SocialSingleton = new NiecDefinitionDeathInteraction();
            public NiecDefinitionDeathInteraction(string name, bool allowCarryChild)
                : base(null, name, allowCarryChild)
            {
            }

            public NiecDefinitionDeathInteraction()
            {
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                return InteractionTestResult.Pass;
            }

            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
            public override InteractionInstance CreateInstance(ref InteractionInstanceParameters parameters)
            {
                Sim target = parameters.Target as Sim;
                
                target.EnableInteractions();
                InteractionInstance interactionInstance = base.CreateInstance(ref parameters);
                interactionInstance.MustRun = true;
                interactionInstance.CancellableByPlayer = true;
                return interactionInstance;
            }
        }
    }
}
