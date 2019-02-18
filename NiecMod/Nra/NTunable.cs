/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 20/10/2018
 * Time: 0:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Sims3.SimIFace;

namespace Sims3.Gameplay.Controllers.Niec
{
    /// <summary>
    /// Tunable By Niec
    /// </summary>
    public static class NTunable
    {
        /// <summary>
        /// kDedugNotificationCheckNiecKill
        /// </summary>
        [Tunable, TunableComment("Dedug? CheckNiecKill Styled Notification")]
        public static bool kDedugNotificationCheckNiecKill = false;
        /// <summary>
        /// kEACanBeKilledExByNiecNotification
        /// </summary>
        [Tunable, TunableComment("Dedug? EA CanBeKilled Styled Notification")]
        public static bool kEACanBeKilledExByNiecNotification = false;
        /// <summary>
        /// kDedugNotificationMineKill
        /// </summary>
        [Tunable, TunableComment("Dedug? MineKill Styled Notification")]
        public static bool kDedugNotificationMineKill = false;
        /// <summary>
        /// kDedugNiecModExceptionMineKill
        /// </summary>
        [Tunable, TunableComment("Dedug? MineKill NiecModException Not Error")]
        public static bool kDedugNiecModExceptionMineKill = false;
        /// <summary>
        /// kDedugNiecModExceptionExtKillSimNiec
        /// </summary>
        [Tunable, TunableComment("Dedug? ExtKillSimNiec NiecModException Not Error")]
        public static bool kDedugNiecModExceptionExtKillSimNiec = false;
        /// <summary>
        /// kCarmeraExtKillSimNiec
        /// </summary>
        [Tunable, TunableComment("kCarmeraExtKillSimNiec")]
        public static bool kCarmeraExtKillSimNiec = true;
        /// <summary>
        /// kHelperNraNoCheckKillSim
        /// </summary>
        [Tunable, TunableComment("kHelperNraNoCheckKillSim")]
        public static bool kHelperNraNoCheckKillSim = true;
        /// <summary>
        /// kNoAutonomousWithActiveDOF
        /// </summary>
        [Tunable, TunableComment("Autonomous With ActiveHousehold in DoorOfLifeAndDeath")]
        public static bool kNoAutonomousWithActiveDOF = true;
        /// <summary>
        /// kForceMaxDeath
        /// </summary>
        [Tunable, TunableComment("Focre Level MaxDeath")]
        public static bool kForceMaxDeath = false;

        [Tunable, TunableComment("kCheckFailed")]
        public static bool kCheckFailed = true;

        [Tunable, TunableComment("kCreatesSim")]
        public static bool kCreatesSim = false;
    }
}


