using System;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Objects.Components;

namespace GameTimeX.Function.WorldEvents
{
    public class FuncNewYear
    {
        public static bool IsShowNewYearEvent()
        {
            DateTime today = DateTime.Now;

            DateTime start = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime end = new DateTime(DateTime.Now.Year, 1, 15);

            CHappyNewYearEvent cHappyNewYearEvent = new CHappyNewYearEvent(SysProps.startUpParms.HappyNewYearEvent).Dezerialize();

            if (cHappyNewYearEvent.Year == DateTime.Now.Year.ToString() && today >= start && today <= end)
            {
                cHappyNewYearEvent.Year = DateTime.Now.AddYears(1).Year.ToString();
                cHappyNewYearEvent.Shown = true;

                SysProps.startUpParms.HappyNewYearEvent = cHappyNewYearEvent.Serialize();

                FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

                return true;
            }

            return false;
        }

        public static void InitializeHappyNewYearEvent()
        {
            CHappyNewYearEvent cHappyNewYearEvent = new CHappyNewYearEvent(SysProps.startUpParms.HappyNewYearEvent).Dezerialize();
            if (cHappyNewYearEvent.Year == string.Empty)
            {
                cHappyNewYearEvent.Year = DateTime.Now.AddYears(1).Year.ToString();
                SysProps.startUpParms.HappyNewYearEvent = cHappyNewYearEvent.Serialize();

                FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

            }
        }
    }
}
