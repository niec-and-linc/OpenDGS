
using System;
using System.Collections.Generic;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.Interactions;
using Sims3.SimIFace;
using Sims3.Gameplay.Utilities;
using System.Text;
using Sims3.UI;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects;
using Sims3.Gameplay.UI;
using Sims3.UI.Hud;
using Sims3.UI.CAS;
using Sims3.Gameplay.Socializing;
using Sims3.Gameplay.Skills;
using Sims3.Gameplay.ActorSystems;
using Sims3.NiecHelp.Tasks;
using Sims3.Gameplay.Controllers.Niec;

namespace NiecMod.Nra
{

    /*
    public class MethodClass
    {

    }
    */

    public static class NiecException
    {


        public static MethodStore AcceptCancelDialogWithoutCommonException = new MethodStore("NiecMod", "NiecMod.Nra.NiecException", "sAcceptCancelDialogWithoutCommonException", new Type[] { typeof(string) });

        public static MethodStore NRaasRegisterCrachGame = new MethodStore("NiecMod", "NRaas.CommonSpace.Helpers.Corrections", "NRaasRegister", new Type[] { typeof(string), typeof(string) });


        


        #region PrintMessage
        /// <summary>
        /// Print message on screen
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message)
        {
            StyledNotification.Show(new StyledNotification.Format(message, StyledNotification.NotificationStyle.kGameMessagePositive));
        }







        public static bool sAcceptCancelDialogWithoutCommonException(string message)
        {
            try
            {
                throw new NiecModException("AcceptCancelDialogWithoutCommonException: Not Error");
            }
            catch (ResetException)
            {
                throw;
            }
            catch (NiecModException ex)
            {
                WriteLog("AcceptCancelDialogWithoutCommonException: " + NiecException.NewLine + NiecException.LogException(ex), true, true, false);
            }
            if (Simulator.CheckYieldingContext(false)) { NiecMod.Nra.SpeedTrap.Sleep(); }
            return AcceptCancelDialog.Show(message);
            //StyledNotification.Show(new StyledNotification.Format(message, StyledNotification.NotificationStyle.kGameMessagePositive));
        }
        #endregion

        public static readonly string NewLine = System.Environment.NewLine;

        public static int sLogEnumerator = 0;

        public static InteractionInstance CreateInstance<INTERACTION>(ref InteractionInstanceParameters parameters) where INTERACTION : InteractionInstance
        {
            InteractionInstance instance = Activator.CreateInstance<INTERACTION>();
            instance.Init(ref parameters);
            return instance;
        }
        public static string GetCanvasSizeLocalizedString(string size)
        {
            string entryKey = "Gameplay/Objects/HobbiesSkills/Easel/CanvasSize:" + size;
            return Localization.LocalizeString(entryKey, new object[0]);
        }

        public static string LogException(Exception ex)
        {
            string entryKey = ex.ToString() + NewLine;
            return entryKey;
        }

        public static string ScriptError()
        {
            string entryKey = "Script Error:" + NewLine;
            return entryKey;
        }

        public static bool WriteLog(string text)
        {
            return WriteLog(text, true, false);
        }

        public static bool WriteLog(string text, bool addHeader)
        {
            return WriteLog(text, addHeader, false);
        }

        public static bool WriteLog(string text, bool addHeader, bool scripterror)
        {
            return WriteLog(text, addHeader, scripterror, false);
        }

        public static bool WriteLog(string text, bool addHeader, bool scripterror, bool printmessage)
        {
            try
            {
                if (string.IsNullOrEmpty(text)) return false;


                
                
                

                if (addHeader)
                {
                    sLogEnumerator++;

                    try
                    {
                        if (printmessage)
                        {
                            PrintMessage("Write Log is Created");
                        }
                        else if (scripterror)
                        {
                            PrintMessage("NiecMod " + NewLine + "Script Error is Found No: " + sLogEnumerator);
                        }
                    }
                    catch (Exception)
                    { }

                    string[] labels = GameUtils.GetGenericString(GenericStringID.VersionLabels).Split(new char[] { '\n' });
                    string[] data = GameUtils.GetGenericString(GenericStringID.VersionData).Split(new char[] { '\n' });

                    string header = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + NewLine;
                    header += "<" + "NiecMod" + ">" + NewLine;
                    try
                    {
                        header += "<ModVersion value=\"" + "1.0" + "\"/>" + NewLine;
                    }
                    catch (Exception)
                    {
                        header += "<ModVersion value=\"" + "1.0" + "\"/>" + NewLine;
                    }
                    

                    int num2 = (labels.Length > data.Length) ? data.Length : labels.Length;
                    for (int j = 0x0; j < num2; j++)
                    {
                        string label = labels[j].Replace(":", "").Replace(" ", "");

                        switch (label)
                        {
                            //case "GameVersion":
                            case "BuildVersion":
                                header += "<" + label + " value=\"" + data[j] + "\"/>" + NewLine;
                                break;
                        }
                    }

                    IGameUtils utils = (IGameUtils)AppDomain.CurrentDomain.GetData("GameUtils");
                    if (utils != null)
                    {
                        ProductVersion version = (ProductVersion)utils.GetProductFlags();

                        header += "<Installed value=\"" + version + "\"/>" + NewLine;
                        
                        
                    }

                    header += "<Enumerator value=\"" + sLogEnumerator + "\"/>" + NewLine;
                    header += "<Content>" + NewLine;
                    if (scripterror)
                    {
                        header += ScriptError() + NewLine;
                    }
                    text = header + text.Replace("&", "&amp;");//.Replace(NewLine, "  <br />" + NewLine);

                    text += NewLine + "</Content>";
                    text += NewLine + "</" + "NiecMod" + ">";
                }

                uint fileHandle = 0x0;
                string str = Simulator.CreateScriptErrorFile(ref fileHandle);
                if (fileHandle != 0x0)
                {
                    CustomXmlWriter xmlWriter = new CustomXmlWriter(fileHandle);

                    xmlWriter.WriteToBuffer(text);

                    xmlWriter.WriteEndDocument();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


        public static string InteractionList(SimDescription simd)
        {
            string logText = null;
            Sim sim = simd.CreatedSim;
            logText += sim.Name + NewLine;
            if (sim.InteractionQueue != null)
            {
                logText += NewLine + "Interactions:";

                int index = 0;
                foreach (InteractionInstance instance in sim.InteractionQueue.InteractionList)
                {
                    if (instance == null)
                    {
                        logText += NewLine + "Empty Interaction";
                    }
                    else
                    {
                        InteractionInstanceParameters parameters = instance.GetInteractionParameters();

                        index++;

                        try
                        {
                            logText += NewLine + index + ": " + instance.GetInteractionName();
                        }
                        catch
                        {
                            logText += NewLine + index + ": error";
                        }

                        if (instance.InteractionDefinition != null)
                        {
                            logText += NewLine + instance.InteractionDefinition.GetType();
                            logText += NewLine + instance.InteractionDefinition;
                            logText += NewLine + instance.GetPriority().Level;
                        }
                        else
                        {
                            logText += NewLine + "Invalid Definition";
                        }
                    }
                }

                logText += NewLine;
            }
            return logText;
        }


        public static string InteractionListLitePro(SimDescription simd)
        {
            if (simd == null) return "SimDescription is Null.";
            string logText = "";
            try
            {
                
                Sim sim = simd.CreatedSim;
                if (sim != null && sim.InteractionQueue != null)
                {
                    logText += NewLine + "Interactions:";

                    int index = 0;
                    foreach (InteractionInstance instance in sim.InteractionQueue.InteractionList)
                    {
                        if (instance == null)
                        {
                            logText += NewLine + "Empty Interaction";
                        }
                        else
                        {
                            InteractionInstanceParameters parameters = instance.GetInteractionParameters();

                            index++;

                            try
                            {
                                logText += NewLine + index + ": " + instance.GetInteractionName();
                            }
                            catch
                            {
                                logText += NewLine + index + ": error";
                            }

                            if (instance.InteractionDefinition != null)
                            {
                                logText += NewLine + instance.InteractionDefinition.GetType();
                                logText += NewLine + instance.InteractionDefinition;
                                logText += NewLine + instance.GetPriority().Level;
                            }
                            else
                            {
                                logText += NewLine + "Invalid Definition";
                            }
                        }
                    }

                    logText += NewLine;
                }
            }
            catch (Exception ex)
            {
                //return "Error: " + NewLine + ex.ToString();
                logText += NewLine + "Error: " + NewLine + ex.ToString();
            }
            
            return logText;
        }

        public static Sim GetActiveActor
        {
            get
            {
                try
                {
                    return Sim.ActiveActor;
                }
                catch
                {
                    return PlumbBob.Singleton.mSelectedActor;
                }
            }
        }


        public static string WhereActiveHousehold(SimDescription desc)
        {
            if (desc == null) return string.Empty;
            try
            {
                
                if (desc.CreatedSim != null && desc.CreatedSim == GetActiveActor && NFinalizeDeath.ActiveHouseholdWithoutDGSCore != null && NFinalizeDeath.ActiveHouseholdWithoutDGSCore.AllSimDescriptions.Contains(desc))
                {
                    //return NewLine + " Active Actor And Active Household";
                    return " : Active Actor And Active Household";
                }
                else if (desc.CreatedSim != null && desc.CreatedSim == PlumbBob.Singleton.mSelectedActor)
                {
                    return " : Active Actor";
                }
                else if (desc.Household == NFinalizeDeath.ActiveHouseholdWithoutDGSCore)
                {
                    return " : Active Household";
                }
                else
                {
                    //return " Is Non Active Household";
                    return string.Empty;
                }
            }
            catch (Exception exx)
            { return NewLine + " Error Active Household:" + NewLine + exx.ToString() + NewLine; }
        }

        public static string GetDescription(IMiniSimDescription obj)
        {
            if (obj == null)
            {
                return null;
            }
            else
            {
                string logText = null;

                logText += NewLine + "SimDescription:" + NewLine;
                logText += NewLine + " Name: " + obj.FullName;
                logText += NewLine + " Age: " + obj.Age;
                logText += NewLine + " Gender: " + obj.Gender;
                logText += NewLine + " SimDescriptionId: " + obj.SimDescriptionId;
                logText += NewLine + " LotHomeId: " + obj.LotHomeId;
                logText += NewLine + " HomeWorld: " + obj.HomeWorld;


                




                Genealogy genealogy = obj.CASGenealogy as Genealogy;
                if (genealogy != null)
                {
                    if ((genealogy.mSim != null) || (genealogy.mMiniSim != null))
                    {
                        logText += NewLine + " Proper Genealogy";
                    }
                    else
                    {
                        logText += NewLine + " Broken Genealogy";
                    }
                }
                else
                {
                    logText += NewLine + " No Genealogy";
                }

                if (obj.IsDead)
                {
                    logText += NewLine + " Dead";
                }

                if (obj.IsPregnant)
                {
                    logText += NewLine + " Pregnant";
                }

                SimDescription desc = obj as SimDescription;
                if (desc != null)
                {
                    logText += NewLine + " Valid: " + desc.IsValidDescription;

                    logText += NewLine + " Species: " + desc.Species;

                    if (desc.Household != null)
                    {
                        logText += NewLine + " Household: " + desc.Household.Name + WhereActiveHousehold(desc);
                        /*
                        try
                        {
                            //logText += NewLine;
                            if (desc.CreatedSim != null && desc.CreatedSim == GetActiveActor && NFinalizeDeath.ActiveHouseholdWithoutDGSCore != null && NFinalizeDeath.ActiveHouseholdWithoutDGSCore.AllSimDescriptions.Contains(desc))
                            {
                                logText += NewLine + " Active Actor And Active Household";
                            }
                            else if (desc.Household == NFinalizeDeath.ActiveHouseholdWithoutDGSCore)
                            {
                                logText += NewLine + " Active Household";
                            }
                            else
                            {
                                logText += NewLine + " Non Active Household";
                            }
                        }
                        catch (Exception exx)
                        { logText += NewLine + " Error Active Household:" + NewLine + exx.ToString() + NewLine; }
                         */
                        
                    }
                    else
                    {
                        logText += NewLine + " No Household";
                    }

                    if ((desc.CreatedByService != null) && (desc.CreatedByService.IsSimDescriptionInPool(desc)))
                    {
                        logText += NewLine + " Service: " + desc.CreatedByService.ServiceType;
                    }

                    if (desc.AssignedRole != null)
                    {
                        logText += NewLine + " Role: " + desc.AssignedRole.Type;
                    }

                    if (desc.OccultManager != null)
                    {
                        logText += NewLine + " Occult: " + desc.OccultManager.CurrentOccultTypes;
                    }
                    else
                    {
                        logText += NewLine + " No OccultManager";
                    }

                    logText += NewLine + " Alien: " + desc.AlienDNAPercentage;

                    try
                    {
                        if (desc.Occupation != null)
                        {
                            logText += NewLine + " Career: " + desc.Occupation.CareerName + " (" + desc.Occupation.CareerLevel + ")";
                        }
                        else
                        {
                            logText += NewLine + " Career: <Unemployed>";
                        }
                    }
                    catch
                    { }

                    try
                    {
                        if ((desc.CareerManager != null) && (desc.CareerManager.School != null))
                        {
                            logText += NewLine + " School: " + desc.CareerManager.School.CareerName + " (" + desc.CareerManager.School.CareerLevel + ")";
                        }
                        else
                        {
                            logText += NewLine + " School: <None>";
                        }
                    }
                    catch
                    { }

                    if (desc.LotHome != null)
                    {
                        logText += NewLine + " Lot: " + desc.LotHome.Name;
                        logText += NewLine + " Address: " + desc.LotHome.Address;
                    }

                    if (desc.SkillManager != null)
                    {
                        foreach (Skill skill in desc.SkillManager.List)
                        {
                            if (skill == null) continue;

                            string name = skill.Guid.ToString();

                            try
                            {
                                name = skill.Name;
                            }
                            catch
                            { }

                            logText += NewLine + " Skill " + name + ": " + skill.SkillLevel;
                        }
                    }

                    if (desc.TraitManager != null)
                    {
                        foreach (Trait trait in desc.TraitManager.List)
                        {
                            if (trait == null) continue;

                            string name = trait.Guid.ToString();
                            try
                            {
                                name = trait.TraitName(desc.IsFemale);
                            }
                            catch
                            { }

                            logText += NewLine + " Trait " + name;
                        }
                    }
                }

                return logText;
            }
        }
    }
}