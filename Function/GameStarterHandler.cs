using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameTimeX.Objects;

namespace GameTimeX.Function
{
    internal class GameStarterHandler
    {
        public static void ActivateProfileSettings(int pid)
        {
            DBObject dbObj = DataBaseHandler.ReadPID(pid);

            if (dbObj != null)
            {
                // Hier Profileinstellungen laden
                CProfileSettings cProfileSettings = new CProfileSettings(dbObj.ProfileSettings).Dezerialize();
                
                // Wenn HDR im Profil aktiviert --> HDR aktivieren
                if (cProfileSettings.HDREnabled)
                {
                    HdrToggler.SetHdrForAllActiveDisplays(true);
                }
            }
        }

        public static void DeactivateProfileSettings(int pid)
        {
            DBObject dbObj = DataBaseHandler.ReadPID(pid);

            if (dbObj != null)
            {
                // Hier Profileinstellungen laden
                CProfileSettings cProfileSettings = new CProfileSettings(dbObj.ProfileSettings).Dezerialize();

                // Wenn HDR im Profil aktiviert --> HDR jetzt deaktivieren
                if (cProfileSettings.HDREnabled)
                {
                    HdrToggler.SetHdrForAllActiveDisplays(false);
                }
            }
        }
    }
}
