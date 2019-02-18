/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 13/09/2018
 * Time: 0:20
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
//using Sims3.Gameplay.Services;
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
using NiecMod.Interactions;
using Sims3.Gameplay.Controllers.Niec;
using NiecMod.Nra;
using Sims3.NiecHelp.Tasks;
using Sims3.Gameplay;
using NiecMod.Helpers.MakeSimPro;
using Sims3.Gameplay.Services;
//using NRaas.NiecMod.Interactions;

namespace NiecMod
{
	/// <summary>
	/// Description of Scripting Mod Instantiator, value does not matter, only its existence.
	/// </summary>
    public class Instantiator
    {
        [Tunable, TunableComment("Scripting Mod Instantiator, value does not matter, only its existence")]
        protected static bool kInstantiator = false;
        private static EventListener sSimInstantiatedListener = null;

        public static AlarmHandle mait =  AlarmHandle.kInvalidHandle;

        private static bool AddedNiecCore = false;



        //private static EventListener sSimAgedUpListener = null;       //Optional age transition listener

        static Instantiator()
        {
            World.OnWorldLoadFinishedEventHandler += new EventHandler(OnWorldLoadFinishedHandler);
            World.OnStartupAppEventHandler += OnStartupApp;
            World.OnWorldQuitEventHandler += WriteLogOnWorldQuit;
        }


        public static void WriteLogOnWorldQuit(object sender, EventArgs args)
        {
            string msge = Niec.iCommonSpace.KillPro.LogTraneEx.ToString();
            if (!string.IsNullOrEmpty(msge))
            {
                try
                {
                    
                    NiecException.WriteLog(msge, true, false, false);
                    Niec.iCommonSpace.KillPro.sLogEnumeratorTrane = 0;
                    Niec.iCommonSpace.KillPro.LogTraneEx = null;
                    Niec.iCommonSpace.KillPro.LogTraneEx = new StringBuilder();
                }
                catch
                { }
            }
            try
            {
                Niec.iCommonSpace.KillPro.sLogEnumeratorTrane = 0;
            }
            catch
            { }

        }


        public static void OnWorldLoadFinishedHandler(object sender, System.EventArgs e)
        {
            try
            {
                //Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();
                if (NFinalizeDeath.Adertyrty)
                {
                    NiecTask.Perform(AintAllDelDescX);
                }
                else
                {
                    foreach (SimDescription sdtyf in NiecMod.Nra.NFinalizeDeath.TattoaX()) // OK Full One World!
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
            }
            catch
            { }
            try
            {
                foreach (Sim sim in Sims3.Gameplay.Queries.GetObjects<Sim>())
                {
                    if (sim != null)
                    {
                        AddInteractions(sim);
                    }
                }
            }  
            catch (Exception exception)
            {
                Exception(exception);
            }
            sSimInstantiatedListener = EventTracker.AddListener(EventTypeId.kSimInstantiated, new ProcessEventDelegate(OnSimInstantiated));  
        }


        public unsafe static int  Size
        {
            get
            {
                return sizeof(void*);
            }
            private set
            {
                int asd = 0;
                asd = value;
            }
        }

        public static void OnStartupApp(object sender, System.EventArgs e)
        {
            if (!AddedNiecCore)
            {
                AddedNiecCore = true;
                Commands.sGameCommands.Register("nhelpernra", "On or Off HelperNra CheckKillSim. Usage: nhelpernra true|false ", Commands.CommandType.General, new CommandHandler(NHelperNra));
                Commands.sGameCommands.Register("nminekill", "On or Off MineKill CanBeKilled. Usage: nminekill true|false ", Commands.CommandType.General, new CommandHandler(NMineKillCanBeKilled));
                Commands.sGameCommands.Register("nmklog", "On or Off MineKill Log Created. Usage: nmklog true|false ", Commands.CommandType.General, new CommandHandler(NiecMineKillLog));
                Commands.sGameCommands.Register("nmkloga", "MineKill Log Add Create. Usage: nmkloga", Commands.CommandType.General, new CommandHandler(NMineKillLogA));
                Commands.sGameCommands.Register("nmklogc", "MineKill Log ClearUp. Usage: nmklogc", Commands.CommandType.General, new CommandHandler(NMineKillLogC));
                Commands.sGameCommands.Register("ncheckfailed", "On or Off FailedCallBookSleep. Usage: ncheckfailed true|false ", Commands.CommandType.General, new CommandHandler(NCheckFailed));
                Commands.sGameCommands.Register("nfailedkill", "nfailedkill is Added. Usage: nfailedkill", Commands.CommandType.General, new CommandHandler(NnFailedcallbooksleep));
                Commands.sGameCommands.Register("nniecdesca", "nniecdesca is Added. Usage: nniecdesca", Commands.CommandType.General, new CommandHandler(NNiecDescAdded));
                Commands.sGameCommands.Register("nniecdescr", "nniecdescr is Removeed. Usage: nniecdescr", Commands.CommandType.General, new CommandHandler(NNiecDescRemoved));
                Commands.sGameCommands.Register("ndelalldesc", "ndelall is Desc Del All. Usage: ndelall", Commands.CommandType.General, new CommandHandler(NNiecDelAllDesc));
                Commands.sGameCommands.Register("ncload", "ncload is Desc Del All. Usage: ncload", Commands.CommandType.General, new CommandHandler(NDelAll));





                //Commands.sGameCommands.Register("nferror", "Usage: nferror. Force Error MineKill", Commands.CommandType.General, new CommandHandler(ForceErrorMineKill));
                Commands.sGameCommands.Register("ntest", "Usage: ncload. Test Fix Error", Commands.CommandType.General, new CommandHandler(TestCommandI));
            }

            
        }



        public static void TestCommand()
        {
            if (AcceptCancelDialog.Show("Test Game Is Stopped Working"))
            {
                return;
            }
            else
            {
                string msg = null;
                msg.ToString();
            }
        }

        public static int TestCommandI(object[] parameters)
        {
            try
            {
                NiecTask.Perform(TestCommand);
            }
            catch
            { }
            return -1;
        }

        public static int ForceErrorMineKill(object[] parameters)
        {
            try
            {
                bool bRet = true;
                ParseParamAsBool(parameters, out bRet, true);
                Niec.iCommonSpace.KillPro.sForceError = bRet;
                return -1;
            }
            catch
            { }
            return -1;
        }






        public static int NNiecDelAllDesc(object[] parameters)
        {
            try
            {
                NiecTask.Perform(AintAllDelDesc);
            }
            catch
            { }
            return -1;
        }

        public static void AITIA(SimDescription sd)
        {
            sd.mGenealogy = null;
        }


        private static void AintAllDelDescX()
        {
            AintAllDelDesc(true);
            /*
            try
            {
                NiecMod.Helpers.Create.CreateActiveHouseholdAndActiveActor();
            }
            catch {
            }
            */
        }
        private static void AintAllDelDesc()
        {
            AintAllDelDesc(false);
        }

        private static void AintAllDelDesc(bool loadworld)
        {
            try
            {
                if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                {
                    //bool checkkillsimxxx = false;
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
                    
                    //Sleep(3.0);

                    try
                    {
                        CommandSystem.ExecuteCommandString("dgsunsafekill false");
                        
                    }
                    catch
                    { }

                    try
                    {
                        CommandSystem.ExecuteCommandString("dgspx false");
                        PlumbBob.ForceSelectActor(null);
                        try
                        {
                            PlumbBob.sSingleton.mSelectedActor = null;
                        }
                        catch
                        { }
                    }
                    catch
                    { }
                    
                    try
                    {
                        List<MiniSimDescription> asdr = new List<MiniSimDescription>(MiniSimDescription.sMiniSims.Values); 
                        foreach (MiniSimDescription esdtyef in asdr)
                        {
                            try
                            {
                                if (esdtyef == null) continue;

                                if (esdtyef.mProtectionFlags != null)
                                    esdtyef.mProtectionFlags.Clear();

                                
                                esdtyef.Instantiated = false;
                                esdtyef.mGenealogy = null;
                                try
                                {
                                    foreach (MiniRelationship miniRelationship in esdtyef.mMiniRelationships)
                                    {
                                        if (miniRelationship == null) continue;
                                        try
                                        {
                                            MiniSimDescription miniSimDescription2 = MiniSimDescription.Find(miniRelationship.SimDescriptionId);
                                            if (miniSimDescription2 != null)
                                            {
                                                if (miniSimDescription2.mProtectionFlags != null)
                                                    miniSimDescription2.mProtectionFlags.Clear();


                                                miniSimDescription2.Instantiated = false;
                                                miniSimDescription2.RemoveMiniRelatioship(esdtyef.mSimDescriptionId);
                                                miniSimDescription2.mGenealogy = null;
                                            }
                                        }
                                        catch
                                        { }
                                        
                                    }
                                }
                                catch
                                { }
                                if (esdtyef.mMiniRelationships != null)
                                    esdtyef.mMiniRelationships.Clear();
                                //MiniSimDescription.sMiniSims.Remove(esdtyef.mSimDescriptionId);
                            }
                            catch
                            { }
                           
                        }
                    }
                    catch
                    { }
                    

                    foreach (SimDescription sdtyef in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                    {

                        try
                        {
                            if (sdtyef == null) continue;

                            //sdtyef.RemoveOutfits(OutfitCategories.All, true);
                            //sdtyef.RemoveOutfit(OutfitCategories.All, 0, true);



                            if (!sdtyef.mIsValidDescription && !loadworld) continue;


                            try
                            {
                                try
                                {
                                    if (sdtyef.OccultManager != null)
                                    {
                                        sdtyef.OccultManager.RemoveAllOccultTypes();
                                    }
                                }
                                catch
                                {

                                }
                                
                                sdtyef.OccultManager = null;

                            }
                            catch
                            { }

                            try
                            {
                                if (sdtyef.IsPregnant)
                                {
                                    NRaas.CommonSpace.Helpers.CASParts.RemoveOutfits(sdtyef, OutfitCategories.All, false);
                                }
                                else
                                {
                                    NRaas.CommonSpace.Helpers.CASParts.RemoveOutfits(sdtyef, OutfitCategories.All, true);
                                }
                            }
                            catch
                            { }
                            sdtyef.Protected = false;
                            MiniSimDescription inim = MiniSimDescription.Find(sdtyef.mSimDescriptionId);
                            if (inim != null)
                            {
                                if (inim.mProtectionFlags != null)
                                inim.mProtectionFlags.Clear();
                                inim.Instantiated = false;
                                inim.mGenealogy = null;
                                inim.ClearMiniRelationships();
                            }
                        }
                        catch
                        { }
                        try
                        {
                            Niec.iCommonSpace.KillPro.RemoveSimDescriptionRelationships(sdtyef);
                        }
                        catch
                        { }
                        try
                        {
                            Niec.iCommonSpace.KillPro.CleanseGenealogy(sdtyef);
                            sdtyef.mGenealogy = null;
                        }
                        catch
                        { }
                        try
                        {
                            Niec.iCommonSpace.KillPro.RemoveSimDescriptionRelationships(sdtyef);
                        }
                        catch
                        { }
                        
                    }

                    List<Sim> asdo = new List<Sim>();
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

                                try
                                {
                                    (simaue as ScriptObject).Destroy();
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

                            if (!description.mIsValidDescription && !loadworld)
                            {

                                try
                                {
                                    while (true)
                                    {
                                        Urnstone urnstone = null;
                                        urnstone = HelperNra.TFindGhostsGrave(description);

                                        if (urnstone != null)
                                        {
                                            urnstone.DeadSimsDescription = null;
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
                                continue;
                            }
                        }
                        catch
                        { }

                        try
                        {
                            if (description.IsPregnant)
                            {
                                NRaas.CommonSpace.Helpers.CASParts.RemoveOutfits(description, OutfitCategories.All, false);
                            }
                            else
                            {
                                NRaas.CommonSpace.Helpers.CASParts.RemoveOutfits(description, OutfitCategories.All, true);
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
                            if (description.CreatedSim != null)
                            {
                                NFinalizeDeath.ForceCancelAllInteractionsWithoutCleanup(description.CreatedSim);
                            }
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

                        try
                        {
                            while (true)
                            {
                                Urnstone urnstone = null;
                                urnstone = HelperNra.TFindGhostsGrave(description);
                                if (urnstone != null)
                                {
                                    urnstone.DeadSimsDescription = null;
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

                        /*
                        try
                        {
                            NRaas.CommonSpace.Helpers.Annihilation.Cleanse(description);
                        }
                        catch
                        { }
                         */
                        description.mIsValidDescription = false;
                    }


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
                        if (!loadworld)
                        {
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
                        }
                        try
                        {
                            MiniSimDescription.sMiniSims.Clear();
                            MiniSimDescription.sMiniSims = null;
                            MiniSimDescription.sMiniSims = new Dictionary<ulong, MiniSimDescription>();
                        }
                        catch
                        {
                            StyledNotification.Format afra = new StyledNotification.Format("Failed 3", StyledNotification.NotificationStyle.kSystemMessage);
                            StyledNotification.Show(afra);
                        }
                        if (NiecMod.KillNiec.AssemblyCheckByNiec.IsInstalled("DGSCore"))
                        {
                            try
                            {
                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                                StyledNotification.Format afra = new StyledNotification.Format("Termination Status: Perfect Execution!", StyledNotification.NotificationStyle.kGameMessageNegative);
                                afra.mTNSCategory = NotificationManager.TNSCategory.Chatty;
                                
                                StyledNotification.Show(afra);
                            }
                            catch
                            { }

                            try
                            {
                                CommandSystem.ExecuteCommandString("dgsnocreate true");
                            }
                            catch
                            { }

                            OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                            if (optionsModel != null)
                            {
                                optionsModel.SaveName = "ClearSave " + "No Name";

                            }
                            try
                            {
                                GameStates.TransitionToEditTown();
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
            
            return;
        }


        public static int NNiecDescRemoved(object[] parameters)
        {
            try
            {
                Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Clear();
            }
            catch
            { }
            return -1;
        }

        public static int NNiecDescAdded(object[] parameters)
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
            return -1;
        }

        public static int NMineKillCanBeKilled(object[] parameters)
        {
            bool bRet = true;
            ParseParamAsBool(parameters, out bRet, true);
            NTunable.kDedugNotificationCheckNiecKill = bRet;
            return -1;
        }



        public static int NiecMineKillLog(object[] parameters)
        {
            bool bRet = true;
            ParseParamAsBool(parameters, out bRet, true);
            Niec.iCommonSpace.KillPro.sLoaderLogTraneEx = bRet;
            return -1;
        }

        public static int NMineKillLogA(object[] parameters)
        {
            try
            {
                if (NiecException.WriteLog(Niec.iCommonSpace.KillPro.LogTraneEx.ToString(), true, false, false))
                {
                    NiecException.PrintMessage("WriteLog" + NiecException.NewLine + "Created MineKillLog No: " + Niec.iCommonSpace.KillPro.sLogEnumeratorTrane);
                }
            }
            catch
            { }

            return 0;
        }

        public static int NMineKillLogC(object[] parameters)
        {
            try
            {
                Niec.iCommonSpace.KillPro.sLogEnumeratorTrane = 0;
                //GC.SuppressFinalize(Niec.iCommonSpace.KillPro.LogTraneEx);
                Niec.iCommonSpace.KillPro.LogTraneEx = null;
                Niec.iCommonSpace.KillPro.LogTraneEx = new StringBuilder();
            }
            catch
            { }
            return -1;
        }



        public static bool CheckKillSim( Sim nlist )
        {
            if (nlist.InteractionQueue.HasInteractionOfType(ExtKillSimNiec.Singleton)) return true;
            if (nlist.InteractionQueue.HasInteractionOfType(Urnstone.KillSim.Singleton)) return true;
            return false;
        }


        public static int NnFailedcallbooksleep(object[] parameters)
        {
            try
            {
                var list = new List<Sim>();
                foreach (Sim sim in LotManager.Actors)
                {
                    if (sim.InteractionQueue != null && CheckKillSim(sim))
                    {
                        list.Add(sim);
                    }
                }
                /*
                if (list.Count == 0)
                {
                    return list.Count;
                }
                 */
                if (list.Count != 0)
                {
                    foreach (Sim nlist in list)
                    {
                        try
                        {

                            
                            HelperNraPro helperNra = new HelperNraPro();

                            //helperNra = HelperNra;

                            helperNra.mSim = nlist;

                            helperNra.mdeathtype = nlist.SimDescription.DeathStyle;

                            if (nlist.Household != null)
                            {
                                helperNra.mHousehold = nlist.Household;
                            }

                            helperNra.mSimdesc = nlist.SimDescription;

                            if (!nlist.LotCurrent.IsWorldLot)
                            {
                                helperNra.mHouseVeri3 = nlist.Position;
                            }

                            else if (Sim.ActiveActor != null && !Sim.ActiveActor.LotCurrent.IsWorldLot)
                            {
                                helperNra.mHouseVeri3 = Sim.ActiveActor.Position;
                            }
                            

                            //helperNra.mdeathtype = simDeathType;

                            /*helperNra.malarmx =  */
                            AlarmManager.Global.AddAlarm(3f, TimeUnit.Hours, new AlarmTimerCallback(helperNra.FailedCallBookSleep), "FailedCallBookSleep " + nlist.Name, AlarmType.AlwaysPersisted, null);
                        }
                        catch (Exception exception)
                        { NiecException.WriteLog("helperNra " + NiecException.NewLine + NiecException.LogException(exception), true, true, false); }
                    }
                }
                //return;
            }
            catch
            { }
            return 1;
        }

        public static int NCheckFailed(object[] parameters)
        {
            bool bRet = true;
            ParseParamAsBool(parameters, out bRet, true);
            NTunable.kCheckFailed = bRet;
            return -1;
        }


        



        public static int NDelAll(object[] parameters)
        {
            bool bRet = true;
            ParseParamAsBool(parameters, out bRet, true);
            NFinalizeDeath.Adertyrty = bRet;
            return -1;
        }



        public static int NHelperNra(object[] parameters)
        {
            bool bRet = true;
            ParseParamAsBool(parameters, out bRet, true);
            NTunable.kHelperNraNoCheckKillSim = bRet;
            return -1;
        }


        private static bool ParseParamAsBool(object[] param, out bool bRet, bool defaultVal)
        {
            bRet = defaultVal;
            /*
            if (param == null)
            {
                throw new ArgumentNullException("param");
            }
             * */
            if (param.Length != 1)
            {
                return false;
            }
            bool result = false;
            if (param[0] is bool)
            {
                bRet = (bool)param[0];
                result = true;
            }
            else if (param[0] is int || param[0] is uint)
            {
                bRet = ((int)param[0] != 0);
                result = true;
            }
            else if (param[0] is string)
            {
                string a = (param[0] as string).ToLower();
                bRet = (a == "on");
                result = (a == "on" || a == "off");
            }
            return result;
        }


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
                Exception(exception);
            }
            return ListenerAction.Keep;
        }

        public static void AddInteractions(Sim sim)
        {
            foreach (InteractionObjectPair pair in sim.Interactions)
            {
            	if (pair.InteractionDefinition.GetType() == ObjectNiec.Singleton.GetType())
            	{
            		return;
            	}
            }
            sim.AddInteraction(ForceEnableSave.Singleton);
            sim.AddInteraction(ForceAddFamily.Singleton);
            sim.AddInteraction(ObjectNiec.Singleton);
            sim.AddInteraction(ForceRequestGrimReaper.Singleton);
            sim.AddInteraction(ReapSoul.Singleton);
            sim.AddInteraction(ForceExitXXX.Singleton);
            sim.AddInteraction(ForceKillSimNiec.Singleton);
            sim.AddInteraction(ForceTestGrim.Singleton);
            sim.AddInteraction(TestAllKillSim.Singleton);
            sim.AddInteraction(AllPauseNiec.Singleton);
            sim.AddInteraction(LincSAT.Singleton);
            sim.AddInteraction(ExtKillSimNiec.Singleton);
            sim.AddInteraction(ResetIntroTutorial.Singleton);
            sim.AddInteraction(AllActorsKillSim.Singleton);
            sim.AddInteraction(TheNiecReapSoul.Singleton);
            sim.AddInteraction(CancelAllInteractions.Singleton);
            sim.AddInteraction(KillForce.Singleton);
            sim.AddInteraction(HelloChatESRB.Singleton);
            sim.AddInteraction(KillInLotCurrent.Singleton);



            //Niec Helper Situation
            sim.AddInteraction(Sims3.Gameplay.NiecRoot.NiecHelperSituation.NiecAppear.Singleton);
            sim.AddInteraction(Sims3.Gameplay.NiecRoot.NiecHelperSituation.ReapSoul.Singleton);
        }

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
    }
}