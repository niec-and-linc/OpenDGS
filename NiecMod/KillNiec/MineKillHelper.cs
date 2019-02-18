namespace Sims3.NiecHelp.Tasks
{

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Sims3.SimIFace;
    using NiecMod.Nra;
    using Sims3.Gameplay.Actors;
    using Sims3.Gameplay.CAS;
    using Sims3.Gameplay.Abstracts;
    using Sims3.SimIFace.CAS;
    using Sims3.Gameplay.ActorSystems;
    using Sims3.Gameplay.Core;


    public class KillTask : Task
    {
        public bool IfMineKill = true;

        public Sim mtarget;
        public SimDescription.DeathType mdeathType;
        public GameObject mobj;
        public bool mplayDeathAnim;
        public bool msleepyes;
        public bool mwasisCrib;

        internal static SimDescription.DeathType GetDeathType(Sim target)
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
            if (target.SimDescription.Elder)
            {
                listr.Add(SimDescription.DeathType.OldAge);
            }
            listr.Add(SimDescription.DeathType.HauntingCurse);
            SimDescription.DeathType randomObjectFromList = Sims3.Gameplay.Core.RandomUtil.GetRandomObjectFromList(listr);
            return randomObjectFromList;
        }

        internal static SimDescription.DeathType GetDGSDeathType(Sim target)
        {
            try
            {
                List<SimDescription.DeathType> listr = new List<SimDescription.DeathType>();

                if (target.BuffManager.HasElement(Gameplay.ActorSystems.BuffNames.Drowning))
                {
                    listr.Add(SimDescription.DeathType.Drown);

                    if (GameUtils.IsInstalled(ProductVersion.EP10))
                    {
                        listr.Add(SimDescription.DeathType.ScubaDrown);
                        listr.Add(SimDescription.DeathType.Shark);
                    }
                }
                else if (target.BuffManager.HasElement(Gameplay.ActorSystems.BuffNames.OnFire))
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

        public KillTask(Sim target)
        {
            if (target == null)
            {
                return;
                //throw new ArgumentNullException("target As Sim");
            }
            mtarget = target;
            if (mdeathType == SimDescription.DeathType.None)
                mdeathType = RandomUtil.CoinFlip() ? GetDGSDeathType(target) : GetDeathType(target);
            mobj = null;
            mplayDeathAnim = true;
            msleepyes = true;
        }

        public KillTask(Sim target, SimDescription.DeathType deathType)
        {
            if (target == null)
            {
                return;
                //throw new ArgumentNullException("target As Sim");
            }
            mtarget = target;
            if (mdeathType == SimDescription.DeathType.None)
                mdeathType = RandomUtil.CoinFlip() ? GetDGSDeathType(target) : GetDeathType(target);
            mobj = null;
            mplayDeathAnim = true;
            msleepyes = true;
        }

        public KillTask(Sim target, SimDescription.DeathType deathType, GameObject obj)
        {
            if (target == null)
            {
                return;
                //throw new ArgumentNullException("target As Sim");
            }
            mtarget = target;
            mdeathType = RandomUtil.CoinFlip() ? GetDGSDeathType(target) : GetDeathType(target);
            mobj = obj;
            mplayDeathAnim = true;
            msleepyes = true;
        }

        public KillTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim)
        {
            if (target == null)
            {
                return;
                //throw new ArgumentNullException("target As Sim");
            }
            mtarget = target;
            if (mdeathType == SimDescription.DeathType.None)
                mdeathType = RandomUtil.CoinFlip() ? GetDGSDeathType(target) : GetDeathType(target);
            mobj = obj;
            mplayDeathAnim = playDeathAnim;
            msleepyes = true;
        }

        public KillTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes)
        {
            if (target == null)
            {
                return;
                //throw new ArgumentNullException("target As Sim");
            }
            mtarget = target;
            if (mdeathType == SimDescription.DeathType.None)
                mdeathType = RandomUtil.CoinFlip() ? GetDGSDeathType(target) : GetDeathType(target);
            mobj = obj;
            mplayDeathAnim = playDeathAnim;
            msleepyes = sleepyes;
        }

        public KillTask(Sim target, SimDescription.DeathType deathType, GameObject obj, bool playDeathAnim, bool sleepyes, bool wasisCrib)
        {
            if (target == null)
            {
                return;
                //throw new ArgumentNullException("target As Sim");
            }
            mtarget = target;
            if (mdeathType == SimDescription.DeathType.None)
                mdeathType = RandomUtil.CoinFlip() ? GetDGSDeathType(target) : GetDeathType(target);
            mobj = obj;
            mplayDeathAnim = playDeathAnim;
            msleepyes = sleepyes;
            mwasisCrib = wasisCrib;
        }



        public virtual ObjectGuid AddToSimulator()
        {
            return Simulator.AddObject(this);
        }

        protected virtual void OnStart()
        { }

        protected virtual void OnCleanUp()
        { }

        public override void Simulate()
        {
            try
            {
                OnStart();
                if (IfMineKill)
                {
                    NiecMod.KillNiec.KillSimNiecX.MineKill(mtarget, mdeathType, mobj, mplayDeathAnim, msleepyes);
                }
            }
            catch (ResetException)
            {
                throw;
            }
            catch (Exception exception)
            {
                NiecException.WriteLog(mtarget.Name + " " + NiecException.LogException(exception), true, false, false);
            }
            finally
            {
                OnCleanUp();
                Simulator.DestroyObject(ObjectId);
            }
        }
    }
}
