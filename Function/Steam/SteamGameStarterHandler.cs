using System.Threading.Tasks;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Function.Game;
using GameTimeX.Function.UserInterface;
using GameTimeX.Function.Utils;
using GameTimeX.Objects.Components;

namespace GameTimeX.Function.Steam
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
                // InfoBox anzeigen, dass Spiel startet
                SysProps.infoBoxGameStarting = new InfoBox("Starting game...", false);
                SysProps.infoBoxGameStarting.Owner = SysProps.mainWindow;
                SysProps.infoBoxGameStarting.Show();

                // Launch Steam Game Button ausgrauen
                VisualHandler.DisableStartGameButtons();

                DBO_Profile dbo_Profile = DM_Profile.ReadPID(pid);

                if (dbo_Profile == null)
                    return;

                // Profileinstellungen laden
                CProfileSettings cProfileSettings = new CProfileSettings(dbo_Profile.ProfileSettings).Dezerialize();

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
