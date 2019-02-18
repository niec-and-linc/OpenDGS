/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 24/09/2018
 * Time: 3:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Sims3.Gameplay.Actors;
using Sims3.Gameplay.CAS;
using NRaas.CommonSpace.Helpers;
using Sims3.Gameplay.Services;

namespace NiecMod.Nra
{
    internal class TimeDeather
    {
        public static bool TKill(Sim target, SimDescription.DeathType deathType, uint time)
        {
            if (target.IsInActiveHousehold || target.Service is GrimReaper)
            {
                return false;
            }
            SpeedTrap.Sleep(time);
            Urnstones.CreateGrave(target.SimDescription, deathType, true, true);
            return true;
        }

    }
}