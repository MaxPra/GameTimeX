using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Objects.Components;

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

            DBO_Profile dbo_Profile = DM_Profile.ReadPID(pid);

            if (dbo_Profile == null)
                return;

            CProfileSettings cProfileSettings = new CProfileSettings(dbo_Profile.ProfileSettings).Dezerialize();

            DeactivateProfileSettings(cProfileSettings);
        }
    }
}
