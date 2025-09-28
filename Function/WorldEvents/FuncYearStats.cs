using System;
using GameTimeX.DataBase.DataManager;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Objects.Components;

namespace GameTimeX.Function.WorldEvents
{
    public class FuncYearStats
    {
        /// <summary>
        /// Gibt zurück, ob die Jahrestatistik gezeigt werden soll oder nicht
        /// </summary>
        /// <param name="cYearStats"></param>
        /// <returns></returns>
        public static bool IsShowYearStats()
        {

            DateTime today = DateTime.Now;

            // Start: 01.12. des aktuellen Jahres
            DateTime start = new DateTime(DateTime.Now.Year, 12, 1);
            // Ende: 30.12. des aktuellen Jahres
            DateTime end = new DateTime(DateTime.Now.Year, 12, 30);

            CYearStats cYearStats = new CYearStats(SysProps.startUpParms.YearStats).Dezerialize();

            if (cYearStats.Year == DateTime.Now.Year.ToString() && today >= start && today <= end && DM_Profile.ReadAll().Count > 0)
            {
                cYearStats.Year = DateTime.Now.AddYears(1).Year.ToString();
                cYearStats.Shown = true;

                SysProps.startUpParms.YearStats = cYearStats.Serialize();

                FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

                return true;
            }

            return false;
        }

        public static void InitializeYearStats()
        {
            CYearStats cYearStats = new CYearStats(SysProps.startUpParms.YearStats).Dezerialize();
            if (cYearStats.Year == string.Empty)
            {
                cYearStats.Year = DateTime.Now.Year.ToString();
                SysProps.startUpParms.YearStats = cYearStats.Serialize();

                FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);
            }
        }
    }
}
