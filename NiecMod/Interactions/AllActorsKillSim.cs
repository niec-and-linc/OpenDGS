/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 29/09/2018
 * Time: 5:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Autonomy;
using Sims3.Gameplay.Controllers;
using Sims3.Gameplay.Interactions;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Interfaces;
using Sims3.SimIFace;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Services;
using NRaas.CommonSpace.Helpers;
using NiecMod.Nra;
using NRaas;
using Sims3.UI;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay;
using Sims3.Gameplay.Careers;
using Sims3.SimIFace.CAS;
using Sims3.Gameplay.Utilities;
using Sims3.Gameplay.UI;
using NiecMod.Helpers.MakeSimPro;
using Sims3.NiecHelp.Tasks;

namespace NiecMod.Interactions
{
    public sealed class AllActorsKillSim : ImmediateInteraction<Sim, Sim>
    {
        public static readonly InteractionDefinition Singleton = new Definition();

        

        public static void Sleep(double numSeconds)
        {
            try
            {
                DateTime t = DateTime.Now.AddSeconds(numSeconds);
                while (DateTime.Now < t)
                {
                    Simulator.Sleep(0u);
                }
            }
            catch
            { }
            
        }


        public static Urnstone FindGhostsGraveX(SimDescription sim)
        {
            try
            {
                throw new Exception();
            }
            catch (Exception ex)
            {

                NiecException.WriteLog("Debug" + " FindGhostsGraveX: " + NiecException.NewLine + NiecException.LogException(ex), true, false, true);
            }
            foreach (Urnstone urnstone in Sims3.Gameplay.Queries.GetObjects<Urnstone>())
            {
                if (object.ReferenceEquals(urnstone.DeadSimsDescription, sim))
                {
                    return urnstone;
                }
            }

            return null;
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

        public static Urnstone PrivateCreateGrave(SimDescription corpse)
        {
            string style;
            ProductVersion version = ProductVersion.BaseGame;
            switch (corpse.Species)
            {
                case CASAgeGenderFlags.Dog:
                case CASAgeGenderFlags.LittleDog:
                    style = "tombstoneDog";
                    version = ProductVersion.EP5;
                    break;

                case CASAgeGenderFlags.Horse:
                    if (corpse.IsUnicorn)
                    {
                        style = "tombstoneUnicorn";
                    }
                    else
                    {
                        style = "tombstoneHorse";
                    }
                    version = ProductVersion.EP5;
                    break;

                case CASAgeGenderFlags.Cat:
                    style = "tombstoneCat";
                    version = ProductVersion.EP5;
                    break;

                default:
                    ulong lifetimeHappiness = corpse.LifetimeHappiness;
                    if (lifetimeHappiness >= Urnstone.LifetimeHappinessWealthyTombstone)
                    {
                        style = "UrnstoneHumanWealthy";
                    }
                    else if (lifetimeHappiness < Urnstone.LifetimeHappinessPoorTombstone)
                    {
                        style = "UrnstoneHumanPoor";
                    }
                    else
                    {
                        style = "UrnstoneHuman";
                    }
                    break;
            }

            Urnstone stone = GlobalFunctions.CreateObject(style, version, Vector3.OutOfWorld, 0, Vector3.UnitZ, null, null) as Urnstone;
            if (stone == null)
            {
                return null;
            }

            try
            {
                corpse.Fixup();
            }
            catch
            { }
            

            stone.SetDeadSimDescription(corpse);

            stone.mPlayerMoveable = true;

            return stone;
        }


        //public Household mhousehold;

        public override bool Run() // Run
        {
            try
            {
                if (Autonomous || Actor.IsNPC) return false;
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
                                                Urnstone urnstone = FindGhostsGrave(murnstone);
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
                    if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions.Count == 0) return Aitoat();
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
                                Urnstone urnstone = FindGhostsGrave(description);
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

                        Sleep(3.0);
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
                        Sleep(3.0);
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
                        Sleep(3.0);
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
                                Urnstone urnstone = FindGhostsGrave(description);
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
                                optionsModel.SaveName = "ClearSave " + Actor.SimDescription.LastName;
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
                                        optionsModel.SaveName = "ClearSave " + Actor.SimDescription.LastName;
                                        optionsModel.SaveGame(true, true, false);
                                    }
                                }
                                catch
                                { }
                               
                            }
                            finally
                            { PlumbBob.ForceSelectActor(Actor); }
                           
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

                        Sleep(3.0);
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
                        Sleep(3.0);
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
                        Sleep(3.0);
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
                    Sleep(3.0);
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
                                Urnstone urnstone = FindGhostsGrave(description);
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
                                optionsModel.SaveName = "ClearSave " + Actor.SimDescription.LastName;
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
                                        optionsModel.SaveName = "ClearSave " + Actor.SimDescription.LastName;
                                        optionsModel.SaveGame(true, true, false);
                                    }
                                }
                                catch
                                { }

                            }
                            finally
                            { PlumbBob.ForceSelectActor(Actor); }

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
                    Lot mlot = Actor.LotCurrent;
                    //Simulator.YieldingDisabled = true;
                    //NiecTask mtask = new NiecTask.AddToSimulator();
                    Nra.NFinalizeDeath.mResetError = true;
                    ObjectGuid sdae = NiecTask.Perform(Nra.NFinalizeDeath.ResetError);
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
                            Sleep(3.0);
                        }
                        
                        //NiecMod.Nra.SpeedTrap.Sleep(0u);
                        mposition = Actor.Position;
                    }

                    catch
                    { }
                     
                    if (Actor.LotCurrent.IsWorldLot)
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
                                    if (!GlobalFunctions.FindGoodLocation(Actor, fglParams, out mposition, out vector2))
                                    {
                                        Actor.SetPosition(Household.ActiveHousehold.LotHome.Position);
                                        mposition = Actor.Position;
                                    }
                                }
                            }

                        }
                        catch
                        { }



                    }
                    else
                    {
                        mposition = Actor.Position;
                    }

                    if (Actor.LotCurrent.IsWorldLot)
                    {
                        Actor.SetPosition(Household.ActiveHousehold.LotHome.Position);
                        mposition = Actor.Position;
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
                                Urnstone urnstone = FindGhostsGrave(description);
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
                                    /*
                                    Sim sim = DGSMakeSim.DGSMakeRandomSimNoCheck(mposition, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                                    if (sim != null)
                                    {

                                        try
                                        {
                                            //sim.SimDescription.Household.SetName("E3Lesa is Good Good Household");
                                            sim.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                            PlumbBob.ForceSelectActor(sim);
                                            //Service.NeedsAssignment(Actor.LotCurrent);
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
                                     */
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
                                Sleep(0.1);
                                OptionsModel optionsModel = Sims3.Gameplay.UI.Responder.Instance.OptionsModel as OptionsModel;
                                if (optionsModel != null)
                                {
                                    optionsModel.SaveName = "ClearSave " + "No Name";

                                }
                                Sleep(0.1);
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

                            Urnstone mGravebackup = PrivateCreateGrave(description);
                            if (mGravebackup != null)
                            {
                                if (!GlobalFunctions.PlaceAtGoodLocation(mGravebackup, new World.FindGoodLocationParams(Actor.Position), false))
                                {
                                    mGravebackup.SetPosition(Actor.Position);
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
                            if (Actor.SimDescription.Elder)
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
                        if (Actor.SimDescription.Elder)
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
                 */

                return true;
            }
            
            catch (Exception ex)
            {

                NiecException.PrintMessage(ex.Message);
                Common.Exception(Actor, ex);
            }
            return false;
        }

        public static bool Aitoat()
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
                    if (description.Elder)
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
            return true;
        }

        public override void Cleanup()
        {
            Nra.NFinalizeDeath.mResetError = false;
            Simulator.YieldingDisabled = false;
            return;
        }

        [DoesntRequireTuning]

        private sealed class Definition : ImmediateInteractionDefinition<Sim, Sim, AllActorsKillSim>, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {
            public override string GetInteractionName(Sim actor, Sim target, InteractionObjectPair interaction)
            {
                return "One World Only Kill Sim By DGS";
            }

            public override bool Test(Sim actor, Sim target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;

                return true;
            }
            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }
            public override string[] GetPath(bool bPath)
            {
                return new string[] { "DGS: Target Kill..." };
            }
        }
    }
}
