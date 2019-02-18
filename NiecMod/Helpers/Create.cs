using System;
using System.Collections.Generic;
using System.Text;
using Sims3.SimIFace;
using Sims3.Gameplay.Core;
using Sims3.Gameplay.Services;
using Sims3.Gameplay.Abstracts;
using Sims3.Gameplay.Actors;
using Sims3.SimIFace.CAS;
using NiecMod.Helpers.MakeSimPro;
using NiecMod.KillNiec;
using Sims3.Gameplay.ActorSystems;
using Sims3.Gameplay;
using Sims3.Gameplay.CAS;
using Sims3.Gameplay.Objects.Island;
using Sims3.Gameplay.Socializing;
using Sims3.UI.Controller;
using NiecMod.Nra;
using Sims3.Gameplay.Objects;

namespace NiecMod.Helpers
{
    public static class Create
    {

        public static SimDescription FindNiecSimDescription(ulong newValueid)
        {
            if (Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions != null)
            {
                foreach (SimDescription item in Sims3.NiecModList.Persistable.ListCollon.NiecSimDescriptions)
                {
                    if (item.SimDescriptionId == newValueid)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public static Urnstone GhostsGrave(SimDescription sim)
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





        public static bool RandomActiveHouseholdAndActiveActor()
        {
            if (GameStates.IsGameShuttingDown /* || Sims3.SimIFace.Environment.HasEditInGameModeSwitch */) return false;
            List<Sim> list = new List<Sim>();
            foreach (Sim actor in LotManager.Actors)
            {
                if (actor.SimDescription.CreatedSim != null && !actor.IsInActiveHousehold && actor.IsHuman && !actor.SimDescription.IsGhost && actor.SimDescription.Household != Household.NpcHousehold && actor.SimDescription.Household != Household.PetHousehold && actor.SimDescription.Household != Household.MermaidHousehold && actor.SimDescription.Household != Household.TouristHousehold && actor.SimDescription.Household != Household.PreviousTravelerHousehold && actor.SimDescription.Household != Household.AlienHousehold && actor.SimDescription.Household != Household.ServobotHousehold)
                {
                    list.Add(actor);
                }
            }
            if (list.Count == 0)
                return false;

            else if (list.Count > 0)
            {
                Sim randomObjectFromList = RandomUtil.GetRandomObjectFromList(list);
                if (!randomObjectFromList.InWorld)
                {
                    randomObjectFromList.AddToWorld();
                }
                PlumbBob.ForceSelectActor(randomObjectFromList);
                return true;
            }
            return false;
        }

        public static bool CreateActiveHouseholdAndActiveActor()
        { 
            if (GameStates.IsGameShuttingDown  || PlumbBob.Singleton == null) return false;
            if ( /*PlumbBob.SelectedActor != null &&*/ PlumbBob.Singleton.mSelectedActor != null) return true;

            List<Lot> lieast = new List<Lot>();
            Lot lot = null;

            //foreach (object obj in LotManager.AllLotsWithoutCommonExceptions)

            foreach (object obj in LotManager.AllLots)
            {
                //Lot lot2 = (Lot)obj;
                //if (lot2.IsResidentialLot && lot2.Household == null)
                Lot lot2 = (Lot)obj;
                if (!lot2.IsWorldLot && !lot2.IsCommunityLotOfType(CommercialLotSubType.kEP10_Diving) && !UnchartedIslandMarker.IsUnchartedIsland(lot2) && lot2.IsResidentialLot && lot2.Household == null && !World.LotIsEmpty(lot2.LotId) && !lot2.IsLotEmptyFromObjects())
                {
                    lieast.Add(lot2);
                }
                if (lieast.Count == 0)
                {
                    if (!lot2.IsWorldLot && !lot2.IsCommunityLotOfType(CommercialLotSubType.kEP10_Diving) && !UnchartedIslandMarker.IsUnchartedIsland(lot2) && lot2.IsResidentialLot && lot2.Household == null)
                    {
                        lieast.Add(lot2);
                    }
                }
            }
            
            if (lieast.Count > 0)
            {

                string simlastnamehousehold = "No Name";

                lot = RandomUtil.GetRandomObjectFromList<Lot>(lieast);

                List<Sim> siml = new List<Sim>();

                Sim sim = null;
                Sim sim2 = null;
                Sim sim3 = null;
                Sim sim4 = null;



                if (AssemblyCheckByNiec.IsInstalled("DGSCore"))
                {
                    try
                    {
                        CommandSystem.ExecuteCommandString("dgspx false");
                    }
                    catch
                    { }
                    sim4 = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                    sim = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Male, WorldName.RiverView);
                    sim2 = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Female, WorldName.IslaParadiso);
                    sim3 = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.Child, CASAgeGenderFlags.Male, GameUtils.GetCurrentWorld());

                }
                else
                {
                    sim = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Male, GameUtils.GetCurrentWorld());
                    sim2 = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                    sim3 = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.Child, CASAgeGenderFlags.Male, GameUtils.GetCurrentWorld());
                    sim4 = DGSMakeSim.DGSMakeRandomSimNoCheck(lot.Position, CASAgeGenderFlags.Child, CASAgeGenderFlags.Female, GameUtils.GetCurrentWorld());
                }



                if (sim != null)
                {
                    simlastnamehousehold = sim.SimDescription.LastName;
                    siml.Add(sim);
                }

                if (sim2 != null)
                {
                    simlastnamehousehold = sim2.SimDescription.LastName;
                    siml.Add(sim2);
                }
                if (sim3 != null) siml.Add(sim3);
                if (sim4 != null) siml.Add(sim4);






                try
                {
                    Relationship relation = Relationship.Get(sim.SimDescription, sim2.SimDescription, true);
                    if (relation != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation, LongTermRelationshipTypes.Spouse);
                    }
                    /*
                    try
                    {
                        sim.Genealogy.RemoveDirectRelation(sim2.Genealogy);
                        sim2.Genealogy.RemoveDirectRelation(sim.Genealogy);
                    }
                    catch
                    { }


                    sim2.Genealogy.AddSibling(sim.Genealogy);
                     */
                }
                catch
                { }







                try
                {
                    Relationship childfrined = Relationship.Get(sim3.SimDescription, sim4.SimDescription, true);
                    if (childfrined != null)
                    {
                        NFinalizeDeath.ForceChangeState(childfrined, LongTermRelationshipTypes.GoodFriend);
                    }










                    Relationship relation2 = Relationship.Get(sim.SimDescription, sim3.SimDescription, true);
                    if (relation2 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation2, LongTermRelationshipTypes.GoodFriend);
                    }

                    Relationship relation3 = Relationship.Get(sim.SimDescription, sim4.SimDescription, true);
                    if (relation3 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation3, LongTermRelationshipTypes.GoodFriend);
                    }








                    Relationship relation4 = Relationship.Get(sim2.SimDescription, sim3.SimDescription, true);
                    if (relation4 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation4, LongTermRelationshipTypes.GoodFriend);
                    }

                    Relationship relation5 = Relationship.Get(sim2.SimDescription, sim4.SimDescription, true);
                    if (relation5 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation5, LongTermRelationshipTypes.GoodFriend);
                    }









                    Relationship relation6 = Relationship.Get(sim3.SimDescription, sim.SimDescription, true);
                    if (relation6 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation6, LongTermRelationshipTypes.GoodFriend);
                    }

                    Relationship relation7 = Relationship.Get(sim3.SimDescription, sim2.SimDescription, true);
                    if (relation7 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation7, LongTermRelationshipTypes.GoodFriend);
                    }






                    Relationship relation8 = Relationship.Get(sim4.SimDescription, sim.SimDescription, true);
                    if (relation8 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation8, LongTermRelationshipTypes.GoodFriend);
                    }

                    Relationship relation9 = Relationship.Get(sim4.SimDescription, sim2.SimDescription, true);
                    if (relation9 != null)
                    {
                        NFinalizeDeath.ForceChangeState(relation9, LongTermRelationshipTypes.GoodFriend);
                    }




                }
                catch
                { }







                try
                {
                    if (sim.SimDescription != null && sim.SimDescription.Genealogy != null)
                    {
                        sim.SimDescription.Genealogy.AddChild(sim3.SimDescription.Genealogy);
                    }
                    if (sim2.SimDescription != null && sim2.SimDescription.Genealogy != null)
                    {
                        sim2.SimDescription.Genealogy.AddChild(sim3.SimDescription.Genealogy);
                    }
                }
                catch
                { }


                try
                {
                    if (sim.SimDescription != null && sim.SimDescription.Genealogy != null)
                    {
                        sim.SimDescription.Genealogy.AddChild(sim4.SimDescription.Genealogy);
                    }
                    if (sim2.SimDescription != null && sim2.SimDescription.Genealogy != null)
                    {
                        sim2.SimDescription.Genealogy.AddChild(sim4.SimDescription.Genealogy);
                    }
                }
                catch
                { }



                if (siml.Count > 0)
                {
                    bool checknullsim = false;
                    try
                    {
                        if (sim4 != null)
                        {
                            PlumbBob.ForceSelectActor(sim4);
                        }
                        else
                        {
                            checknullsim = true;
                        }
                        sim2.SimDescription.Household.SetName(sim2.SimDescription.LastName);
                    }
                    catch
                    { }
                    try
                    {
                        foreach (Sim sitem in siml)
                        {
                            if (sitem == null) continue;

                            try
                            {
                                if (checknullsim)
                                {
                                    PlumbBob.ForceSelectActor(sitem);
                                    checknullsim = false;
                                }
                                sitem.SimDescription.LastName = simlastnamehousehold;

                                sitem.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                try
                                {
                                    if (sitem.SimDescription.Child || sitem.SimDescription.Teen)
                                    {
                                        sitem.SimDescription.AssignSchool();
                                    }
                                    GlobalFunctions.PlaceAtGoodLocation(sitem, new World.FindGoodLocationParams(lot.Position), false);
                                }
                                catch
                                { }
                                try
                                {
                                    if (sitem.IsSelectable)
                                    {
                                        sitem.OnBecameSelectable();
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
                                if (sitem.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                {
                                    sitem.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                    sitem.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                }
                                sitem.AddInitialObjects(sitem.IsSelectable);
                            }
                            catch
                            { }
                        }
                    }
                    catch
                    { }
                }


            }
            else if (lieast.Count == 0)
            {
                return false;
            }
            return true;
        }



        public static Vector3 GetPositionInRandomLot(Lot lot)
        {
            List<Lot> lots = new List<Lot>(LotManager.sLots.Values);
            lots.Remove(lot);

            if (lots.Count == 0)
            {
                lots.Add(lot);
            }

            return Service.GetPositionInRandomLot(RandomUtil.GetRandomObjectFromList(lots));
        }
        public static Vector3 AttemptToFindSafeLocation(Lot lot, bool isHorse)
        {
            if (lot == null) return Vector3.Invalid;

            if (isHorse)
            {
                Mailbox mailbox = lot.FindMailbox();
                if (mailbox != null)
                {
                    return mailbox.Position;
                }
                else
                {
                    Door frontDoor = lot.FindFrontDoor();
                    if (frontDoor != null)
                    {
                        int roomId = frontDoor.GetRoomIdOfDoorSide(CommonDoor.tSide.Front);
                        if (roomId != 0)
                        {
                            roomId = frontDoor.GetRoomIdOfDoorSide(CommonDoor.tSide.Back);
                        }

                        if (roomId == 0)
                        {
                            List<GameObject> objects = lot.GetObjectsInRoom<GameObject>(roomId);
                            if (objects.Count > 0)
                            {
                                return RandomUtil.GetRandomObjectFromList(objects).Position;
                            }
                        }
                    }
                }
            }

            return lot.EntryPoint();
        }
    }


}
