namespace NiecMod.Helpers.MakeSimPro
{
    using System;
    using System.Collections.Generic;
    using Sims3.Gameplay.Actors;
    using Sims3.Gameplay.Autonomy;
    using Sims3.Gameplay.Core;
    using Sims3.Gameplay.Interactions;
    using Sims3.Gameplay.Interfaces;
    using Sims3.SimIFace;
    using Sims3.SimIFace.CAS;
    using Sims3.Gameplay.CAS;
    using Sims3.Gameplay.ActorSystems;
    using Sims3.Gameplay.CAS.Locale;
    using Sims3.Gameplay.Skills;
    using Sims3.Gameplay.Controllers;
    using Sims3.UI;
    using Sims3.Gameplay;
    using Sims3.Gameplay.Pools;
    using NiecMod.KillNiec;

    public sealed class DGSMakeSim : ImmediateInteractionGameObjectHit<IActor, IGameObject>
    {

        public static Sim DGSMakeRandomSim(Vector3 point, CASAgeGenderFlags age, CASAgeGenderFlags gender, WorldName worldname)
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
            SimDescription simDescription2 = DGSMakeSSimDescription(age, gender, worldname, uint.MaxValue);
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
                household2.SetName(/* "E3Lesa is Good" */ "Good Household");
                household2.Add(simDescription2);
            }
            WorldName currentWorld = worldname;
            simDescription2.FirstName = SimUtils.GetRandomGivenName(simDescription2.IsMale, currentWorld);
            simDescription2.LastName = SimUtils.GetRandomFamilyName(currentWorld);

            return simDescription2.Instantiate(point);
        }





        public static Sim DGSMakeRandomSimNoCheck(Vector3 point, CASAgeGenderFlags age, CASAgeGenderFlags gender, WorldName worldname)
        {
            LotLocation lotLocation = default(LotLocation);
            ulong lotLocation2 = World.GetLotLocation(point, ref lotLocation);
            Lot lot = LotManager.GetLot(lotLocation2);
            SimDescription simDescription2 = DGSMakeSSimDescription(age, gender, worldname, uint.MaxValue);
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
                household2.SetName(/* "E3Lesa is Good" */ "Good Household");
                household2.Add(simDescription2);
            }
            WorldName currentWorld = worldname;
            simDescription2.FirstName = SimUtils.GetRandomGivenName(simDescription2.IsMale, currentWorld);
            simDescription2.LastName = SimUtils.GetRandomFamilyName(currentWorld);

            return simDescription2.Instantiate(point);
        }




        private bool FixFoxNonStaticDGSMakeRandomSim = false;

        public Sim NonStaticDGSMakeRandomSim(Vector3 point, CASAgeGenderFlags age, CASAgeGenderFlags gender, WorldName worldname)
        {
            LotLocation lotLocation = default(LotLocation);
            ulong lotLocation2 = World.GetLotLocation(point, ref lotLocation);
            Lot lot = LotManager.GetLot(lotLocation2);
            if (!FixFoxNonStaticDGSMakeRandomSim && (age & (CASAgeGenderFlags.Baby | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child)) != CASAgeGenderFlags.None)
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
            SimDescription simDescription2 = DGSMakeSSimDescription(age, gender, worldname, uint.MaxValue);
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
                household2.SetName(/* "E3Lesa is Good" */ "Good Household");
                household2.Add(simDescription2);
            }
            WorldName currentWorld = worldname;
            simDescription2.FirstName = SimUtils.GetRandomGivenName(simDescription2.IsMale, currentWorld);
            simDescription2.LastName = SimUtils.GetRandomFamilyName(currentWorld);

            return simDescription2.Instantiate(point);
        }

        public static SimDescription DGSMakeSSimDescription(CASAgeGenderFlags age, CASAgeGenderFlags gender, WorldName homeWorld, uint outfitCategoriesToBuild)
        {
            Color[] hairColors;
            if (age == CASAgeGenderFlags.Elder)
            {
                hairColors = Genetics.GetRandomElderHairColor();
            }
            else
            {
                hairColors = Genetics.GetGeneticHairColor(homeWorld);
            }
            float skinToneIndex = 0f;
            ResourceKey skinTone = Genetics.RandomSkin(false, homeWorld, ref skinToneIndex);
            return DGSMakeSSimDescription(null, age, gender, skinTone, skinToneIndex, hairColors, homeWorld, outfitCategoriesToBuild, false);
        }


        public static SimDescription DGSMakeSSimDescription(SimBuilder builder, CASAgeGenderFlags age, CASAgeGenderFlags gender, ResourceKey skinTone, float skinToneIndex, Color[] hairColors, WorldName homeWorld, uint outfitCategoriesToBuild, bool isAlien)
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
            simDescription.HomeWorld = GameUtils.GetCurrentWorld();
            return simDescription;
        }


        public override string GetInteractionName()
        {
            return "DGS Make Sim Test";
        }

        public Sim mSima = null;

        public static SimDescription mSimaStat = null;

        public bool Sitoat = false;

        public override bool Run()
        {
            string msg = "Run";
            checked
            {
                try
                {

                    if (AcceptCancelDialog.Show("Force Select Actor?"))
                    {


                        msg = "Accept";
                        var definition = base.InteractionDefinition as Definition;
                        LotLocation lotLocation = default(LotLocation);
                        ulong lotLocation2 = World.GetLotLocation(this.Hit.mPoint, ref lotLocation);
                        Lot lot = LotManager.GetLot(lotLocation2);
                        if ((definition.Age & (CASAgeGenderFlags.Baby | CASAgeGenderFlags.Toddler | CASAgeGenderFlags.Child)) != CASAgeGenderFlags.None)
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

                                if (AssemblyCheckByNiec.IsInstalled("DGSCore") && TwoButtonDialog.Show("DGSCore" + NiecMod.Nra.NiecException.NewLine + "Found Childern Sims in Household Without Adult" + NiecMod.Nra.NiecException.NewLine + "Note: Social Worker Catch Remove Sims" + NiecMod.Nra.NiecException.NewLine + "Are you sure?!", "Yea It Create " + definition.Age, "No Create YoungAdult"))
                                {
                                    msg = "Accept if not flag";
                                    Sim simnocheck = DGSMakeRandomSimNoCheck(Hit.mPoint, definition.Age, definition.Gender, definition.WorldName);
                                    if (simnocheck != null)
                                    {
                                        if (simnocheck.IsSelectable)
                                        {
                                            PlumbBob.SelectActor(simnocheck);
                                        }
                                        else
                                        {
                                            simnocheck.SimDescription.IsNeverSelectable = false;
                                            PlumbBob.ForceSelectActor(simnocheck);
                                        }
                                        try
                                        {
                                            GlobalFunctions.PlaceAtGoodLocation(simnocheck, new World.FindGoodLocationParams(Hit.mPoint), false);
                                            if (simnocheck.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                            {
                                                simnocheck.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                simnocheck.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                            }
                                        }
                                        catch
                                        { }

                                        try
                                        {
                                            if (simnocheck.SimDescription.Household.mName == null)
                                            {
                                                simnocheck.SimDescription.Household.SetName(simnocheck.SimDescription.LastName);
                                            }

                                        }
                                        catch (Exception ex2)
                                        {
                                            NiecMod.Nra.NiecException.WriteLog(msg + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);

                                        }
                                    }
                                    else
                                    {
                                        //return false;
                                        msg = "MakeSim Failed! if flag";
                                        goto backif;
                                    }
                                    return true;
                                }
                            backif:
                                Sim sim = DGSMakeRandomSim(this.Hit.mPoint, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Male, definition.WorldName);
                                msg = "Terraininstance";
                                if (sim != null)
                                {
                                    Sitoat = true;
                                    mSima = sim;
                                    mSimaStat = sim.SimDescription;
                                    sim.FadeOut();
                                    var terraininstance = new Terrain.TeleportMeHere.Definition(false).CreateInstance(Terrain.Singleton, sim, new InteractionPriority((InteractionPriorityLevel)8195), base.Autonomous, base.CancellableByPlayer) as TerrainInteraction;

                                    try
                                    {
                                        if (sim.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                        {
                                            sim.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                            sim.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                        }
                                    }
                                    catch
                                    { }

                                    if (terraininstance != null)
                                    {
                                        terraininstance.Hidden = true;
                                        terraininstance.MustRun = true;
                                        //Vector3 vector2;
                                        Lot loty = sim.SimDescription.LotHome;
                                        if (loty == null)
                                        {
                                            loty = sim.SimDescription.VirtualLotHome;
                                        }
                                        //World.FindGoodLocationParams fglParams = new World.FindGoodLocationParams(Helper.Create.GetPositionInRandomLot(loty));
                                        /*
                                        fglParams.BooleanConstraints |= FindGoodLocationBooleans.StayInRoom;
                                        fglParams.InitialSearchDirection = RandomUtil.GetInt(0x0, 0x7);
                                         */
                                        //lot.Household.Add(sim.SimDescription);
                                        terraininstance.Destination = Helpers.Create.GetPositionInRandomLot(loty);
                                        sim.InteractionQueue.Add(terraininstance);
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                                try
                                {
                                    //sim.SimDescription.Household.SetName(/* "E3Lesa is Good" */ "Good Household");
                                    sim.SimDescription.Household.SetName(sim.SimDescription.LastName);
                                }
                                catch (Exception ex2)
                                {
                                    NiecMod.Nra.NiecException.WriteLog(msg + " SetName: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex2), true, true);
                                }

                            }

                        }

                        //lot.MoveIn(lot.Household);
                        //FixFoxNonStaticDGSMakeRandomSim = true;
                        Sim sim2 = DGSMakeRandomSim(this.Hit.mPoint, definition.Age, definition.Gender, definition.WorldName);
                        if (sim2 != null)
                        {
                            PlumbBob.ForceSelectActor(sim2);
                            if (mSima != null && Sitoat)
                            {
                                var followchildsim = Sims3.Gameplay.Actors.Sim.FollowParent.Singleton.CreateInstance(sim2, mSima, new InteractionPriority((InteractionPriorityLevel)8195), base.Autonomous, base.CancellableByPlayer) as Sims3.Gameplay.Actors.Sim.FollowParent;
                                followchildsim.Hidden = true;
                                followchildsim.MustRun = true;
                                if (mSima.InteractionQueue.AddNextIfPossibleAfterCheckingForDuplicates(followchildsim))
                                {
                                    Sim.ForceSocial(mSima, sim2, "Chat", (InteractionPriorityLevel)8195, true);
                                }

                                //sim2.SimDescription.TraitManager.RemoveAllElements();
                                try
                                {
                                    if (sim2.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                    {
                                        sim2.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                        sim2.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                    }
                                }
                                catch
                                { }

                            }
                        }


                    }












                    else
                    {
                        msg = "Cancel";
                        var definition = base.InteractionDefinition as Definition;
                        Sim sim3 = DGSMakeRandomSim(this.Hit.mPoint, definition.Age, definition.Gender, definition.WorldName);
                        if (sim3 == null)
                        {
                            msg = "Sim3 is Null";
                            Sim sim4 = DGSMakeRandomSim(this.Hit.mPoint, CASAgeGenderFlags.YoungAdult, CASAgeGenderFlags.Male, definition.WorldName);
                            if (sim4 != null)
                            {
                                msg = "Sim4 is Keep";


                                if (!sim4.IsInActiveHousehold)
                                {
                                    try
                                    {
                                        if (sim4.SimDescription.Household.NameUnlocalized == "Good Household")
                                        {
                                            sim4.SimDescription.Household.SetName("Evil Household");
                                        }
                                        else
                                        {
                                            sim4.SimDescription.Household.SetName(sim4.LastName);
                                        }
                                        sim4.SimDescription.TraitManager.RemoveAllElements();
                                        sim4.SimDescription.TraitManager.AddElement(TraitNames.Daredevil);
                                        sim4.SimDescription.TraitManager.AddElement(TraitNames.Evil);
                                        sim4.SimDescription.TraitManager.AddElement(TraitNames.MeanSpirited);
                                        sim4.SimDescription.TraitManager.AddElement(TraitNames.Loser);
                                        sim4.SimDescription.TraitManager.AddElement(TraitNames.Adventurous);
                                    }
                                    catch
                                    { }

                                }
                                else
                                {
                                    try
                                    {
                                        if (sim4.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                        {
                                            sim4.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                            sim4.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                        }
                                    }
                                    catch
                                    { }

                                    if (sim4.SimDescription.Household.NameUnlocalized == "Evil Household")
                                    {
                                        sim4.SimDescription.Household.SetName("Good Household");
                                    }
                                    else
                                    {
                                        sim4.SimDescription.Household.SetName(sim4.LastName);
                                    }
                                }







                                Sim sim5 = DGSMakeRandomSim(this.Hit.mPoint, definition.Age, definition.Gender, definition.WorldName);
                                if (sim5 != null)
                                {
                                    msg = "Sim5 is Keep";
                                    GlobalFunctions.PlaceAtGoodLocation(sim5, new World.FindGoodLocationParams(Hit.mPoint), false);

                                    if (!sim5.IsInActiveHousehold)
                                    {
                                        try
                                        {
                                            if (sim5.SimDescription.Household.NameUnlocalized == "Good Household")
                                            {
                                                sim5.SimDescription.Household.SetName("Evil Household");
                                            }
                                            else
                                            {
                                                sim5.SimDescription.Household.SetName(sim5.LastName);
                                            }
                                            if (definition.Gender == CASAgeGenderFlags.Child)
                                            {
                                                sim5.SimDescription.TraitManager.RemoveAllElements();
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Evil);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.MeanSpirited);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Loser);
                                            }
                                            else
                                            {
                                                sim5.SimDescription.TraitManager.RemoveAllElements();
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Daredevil);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Evil);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.MeanSpirited);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Loser);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Adventurous);
                                            }
                                        }
                                        catch
                                        { }

                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (sim5.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                            {
                                                sim5.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                                sim5.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                            }
                                        }
                                        catch
                                        { }


                                        if (sim5.SimDescription.Household.NameUnlocalized == "Evil Household")
                                        {
                                            sim5.SimDescription.Household.SetName("Good Household");
                                        }
                                        else
                                        {
                                            sim5.SimDescription.Household.SetName(sim5.LastName);
                                        }
                                    }






                                }
                                else
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {

                            if (!sim3.IsInActiveHousehold)
                            {
                                try
                                {
                                    if (sim3.SimDescription.Household.NameUnlocalized == "Good Household")
                                    {
                                        sim3.SimDescription.Household.SetName("Evil Household");
                                    }

                                    else
                                    {
                                        sim3.SimDescription.Household.SetName(sim3.LastName);
                                    }

                                    if (definition.Gender == CASAgeGenderFlags.Child)
                                    {
                                        sim3.SimDescription.TraitManager.RemoveAllElements();
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Evil);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.MeanSpirited);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Loser);
                                    }
                                    else
                                    {
                                        sim3.SimDescription.TraitManager.RemoveAllElements();
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Daredevil);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Evil);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.MeanSpirited);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Loser);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Adventurous);
                                    }
                                }
                                catch
                                { }

                            }
                            else
                            {
                                try
                                {
                                    if (sim3.SimDescription.TraitManager.HasElement(TraitNames.Evil))
                                    {
                                        sim3.SimDescription.TraitManager.RemoveElement(TraitNames.Evil);
                                        sim3.SimDescription.TraitManager.AddElement(TraitNames.Good);
                                    }
                                }
                                catch
                                { }


                                if (sim3.SimDescription.Household.NameUnlocalized == "Evil Household")
                                {
                                    sim3.SimDescription.Household.SetName("Good Household");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    NiecMod.Nra.NiecException.WriteLog(msg + " DGSMakeSim: " + NiecMod.Nra.NiecException.NewLine + NiecMod.Nra.NiecException.LogException(ex), true, true);
                    return false;
                }
            }

            return true;
        }


        public static readonly InteractionDefinition Singleton = new Definition();


        [DoesntRequireTuning]

        private sealed class Definition : ActorlessInteractionDefinition<IActor, IGameObject, DGSMakeSim>, IDontNeedToBeCheckedInResort, IAllowedOnClosedVenues, IOverridesAgeTests, IUseTargetForAutonomousIsAllowedInRoomCheck, IScubaDivingInteractionDefinition, IUsableWhileOnFire, IUsableDuringFire, IUsableDuringBirthSequence, IUsableDuringShow
        {

            public Definition()
            {
            }

            /*
            public override string GetInteractionName(IActor actor, IGameObject target, InteractionObjectPair interaction)
            {
                return "Make Sim";
            }
            */
            public Definition(CASAgeGenderFlags age, CASAgeGenderFlags gender, WorldName worldname)
            {
                this.Age = age;
                this.Gender = gender;
                this.WorldName = worldname;
            }


            public override string GetInteractionName(IActor actor, IGameObject target, InteractionObjectPair interaction)
            {
                return this.WorldName.ToString();
            }


            public override bool Test(IActor actor, IGameObject target, bool isAutonomous, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                if (isAutonomous) return false;
                //var terraintest = target as Terrain;
                //if (terraintest == null) return false;

                return true;
            }

            public bool TestNoLpp(ref InteractionInstanceParameters parameters)
            {
                var terraintest = parameters.Target as Terrain;
                if (terraintest == null) return false;
                //Sim sim = parameters.Actor as Sim;
                GameObjectHit hit = parameters.Hit;
                //SwimmingInPool swimmingInPool = sim.Posture as SwimmingInPool;
                switch (hit.mType)
                {
                    case GameObjectHitType.WaterSea:
                        /*
                        if (swimmingInPool != null)
                        {
                            return !swimmingInPool.ContainerIsOcean;
                        }
                         * */
                        return false;
                    case GameObjectHitType.WaterPond:
                        /*
                        if (PondManager.ArePondsLiquid())
                        {
                            if (swimmingInPool != null)
                            {
                                return !swimmingInPool.ContainerIsOcean;
                            }
                            return false;
                        }
                        break;
                         * */
                        return false;
                    case GameObjectHitType.WaterPool:
                        return false;
                }

                return true;
            }

            public override InteractionTestResult Test(ref InteractionInstanceParameters parameters, ref GreyedOutTooltipCallback greyedOutTooltipCallback)
            {
                try
                {
                    if (parameters.Actor == null) return InteractionTestResult.Root_Null_Actor;
                    if (parameters.Target == null) return InteractionTestResult.Root_Null_Target;

                    if (TestNoLpp(ref parameters))
                    {
                        //return InteractionTestResult.Gen_BadTerrainType;
                        return base.Test(ref parameters, ref greyedOutTooltipCallback);
                    }
                    return InteractionTestResult.Gen_BadTerrainType;

                    /*
                    if (!Test(parameters.Actor as Sim, parameters.Target as Sim, parameters.Autonomous, ref greyedOutTooltipCallback))
                    {
                        return InteractionTestResult.Def_TestFailed;
                    }
                     * 
                     * better
                     * 
                    return base.Test(ref parameters, ref greyedOutTooltipCallback);
                     * */
                    //return base.Test(ref parameters, ref greyedOutTooltipCallback);
                }
                catch
                { }

                return InteractionTestResult.GenericFail;
            }

            public override void AddInteractions(InteractionObjectPair iop, IActor actor, IGameObject target, List<InteractionObjectPair> results)
            {
                for (int i = 0x0; i < kAges.Length; i++)
                {
                    for (int j = 0x0; j < kGenders.Length; j++)
                    {
                        for (int x = 0x0; x < kWorldName.Length; x++)
                        {

                            Definition interaction = new Definition(Definition.kAges[i], Definition.kGenders[j], Definition.kWorldName[x]);
                            results.Add(new InteractionObjectPair(interaction, target));
                        }
                    }
                }
            }

            public SpecialCaseAgeTests GetSpecialCaseAgeTests()
            {
                return SpecialCaseAgeTests.None;
            }

            public override string[] GetPath(bool isFemale)
            {
                return new string[]
			    {
                    "DGSMods",
				    "DGS Make Sim",
				    this.Age + "",
                    this.Gender + ""
			    };
            }


            public CASAgeGenderFlags Age;


            public CASAgeGenderFlags Gender;

            public WorldName WorldName;


            private static CASAgeGenderFlags[] kAges = new CASAgeGenderFlags[]
		    {
			    CASAgeGenderFlags.Baby,
			    CASAgeGenderFlags.Toddler,
			    CASAgeGenderFlags.Child,
			    CASAgeGenderFlags.Teen,
			    CASAgeGenderFlags.YoungAdult,
			    CASAgeGenderFlags.Adult,
			    CASAgeGenderFlags.Elder
		    };


            private static CASAgeGenderFlags[] kGenders = new CASAgeGenderFlags[]
		    {
			    CASAgeGenderFlags.Male,
			    CASAgeGenderFlags.Female
		    };

            private static WorldName[] kWorldName = new WorldName[]
		    {
			    WorldName.France,
			    WorldName.FutureWorld,
                WorldName.Egypt,
			    WorldName.China,
                WorldName.University
		    };
        }
    }
}
