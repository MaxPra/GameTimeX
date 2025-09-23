using System.Threading.Tasks;
using GameTimeX.Objects;

namespace GameTimeX.Function
{
    internal class SteamGameStarterHandler
    {
        /// <summary>
        /// Startet ein Steam-Spiel anhand der ID
        /// </summary>
        /// <param name="steamAppID"></param>
        public static async void StartSteamGame(string steamAppID, int pid)
        {
            if (SteamLocatorHandler.IsGameInstalledByAppId(steamAppID))
            {
                DBObject dbObj = DataBaseHandler.ReadPID(pid);

                if (dbObj == null)
                    return;

                // Profileinstellungen laden
                CProfileSettings cProfileSettings = new CProfileSettings(dbObj.ProfileSettings).Dezerialize();

                // Zuvor Profileinstellungen aktivieren
                GameStarterHandler.ActivateProfileSettings(cProfileSettings);

                await Task.Delay(5000);

                SteamGameStarter steamGameStarter = new SteamGameStarter(FuncConvert.ToList(cProfileSettings.SteamGameArgs), steamAppID);
                steamGameStarter.StartGame();

            }
            else
            {
                InfoBox infoBox = new InfoBox("Game is not installed!");
                infoBox.Owner = SysProps.mainWindow;
                infoBox.ShowDialog();
            }
        }

    }
}
