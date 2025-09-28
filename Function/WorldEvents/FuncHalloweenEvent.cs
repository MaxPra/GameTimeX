using System;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Objects.Components;

namespace GameTimeX.Function.WorldEvents
{
    public class FuncHalloweenEvent
    {
        public static bool IsShowHalloweenEvent()
        {
            DateTime today = DateTime.Now;

            DateTime start = new DateTime(DateTime.Now.Year, 10, 31);
            DateTime end = new DateTime(DateTime.Now.Year, 10, 31);

            CHalloweenEvent cHalloweenEvent = new CHalloweenEvent(SysProps.startUpParms.HalloweenEvent).Dezerialize();

            if (cHalloweenEvent.Year == DateTime.Now.Year.ToString() && today >= start && today <= end)
            {
                cHalloweenEvent.Year = DateTime.Now.AddYears(1).Year.ToString();
                cHalloweenEvent.Shown = true;

                SysProps.startUpParms.HalloweenEvent = cHalloweenEvent.Serialize();

                FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);

                return true;
            }

            return false;
        }

        public static void InitializeHalloweenEvent()
        {
            CHalloweenEvent cHalloweenEvent = new CHalloweenEvent(SysProps.startUpParms.HalloweenEvent).Dezerialize();
            if (cHalloweenEvent.Year == string.Empty)
            {
                cHalloweenEvent.Year = DateTime.Now.Year.ToString();
                SysProps.startUpParms.HalloweenEvent = cHalloweenEvent.Serialize();

                FileHandler.SaveStartParms(SysProps.startUpParmsPath, SysProps.startUpParms);
            }
        }
    }
}
