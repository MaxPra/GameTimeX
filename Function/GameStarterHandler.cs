using GameTimeX.Objects;

namespace GameTimeX.Function
{
    internal class GameStarterHandler
    {
        public static void ActivateProfileSettings(CProfileSettings cProfileSettings)
        {
            // Wenn HDR im Profil aktiviert --> HDR aktivieren
            if (cProfileSettings.HDREnabled)
            {
                HdrToggler.SetHdrForAllActiveDisplays(true);
            }
        }

        public static void DeactivateProfileSettings(CProfileSettings cProfileSettings)
        {
            // Wenn HDR im Profil aktiviert --> HDR aktivieren
            if (cProfileSettings.HDREnabled)
            {
                HdrToggler.SetHdrForAllActiveDisplays(false);
            }
        }

        public static void DeactivateProfileSettings(int pid)
        {
            if (pid == 0)
                return;

            DBObject dBObject = DataBaseHandler.ReadPID(pid);

            if (dBObject == null)
                return;

            CProfileSettings cProfileSettings = new CProfileSettings(dBObject.ProfileSettings).Dezerialize();

            DeactivateProfileSettings(cProfileSettings);
        }
    }
}
