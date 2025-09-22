using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                // Zuvor Profileinstellungen aktivieren
                GameStarterHandler.ActivateProfileSettings(pid);

                // Asynchron 2 Sekunden warten (UI bleibt responsiv)
                await Task.Delay(5000);

                var steamRoot = SteamLocatorHandler.GetSteamRoot();
                var steamExe = System.IO.Path.Combine(steamRoot, "steam.exe");
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = steamExe,
                    Arguments = $"-applaunch {steamAppID}",
                    UseShellExecute = true
                });
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
