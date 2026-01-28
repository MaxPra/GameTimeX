using System;
using System.Diagnostics;

namespace GameTimeX.Function.Game
{
    internal class SteamGridDBHandler
    {
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        public static void OpenSteamGridDbSearch(string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
                return;

            gameName = gameName.Replace(" ", "+");

            string query = Uri.EscapeDataString(gameName.Trim());
            string url = $"https://www.steamgriddb.com/search/grids?term={query}";
            OpenUrl(url);
        }
    }
}
