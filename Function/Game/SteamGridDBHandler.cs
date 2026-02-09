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
            {
                OpenUrl("https://www.steamgriddb.com/");
                return;
            }

            gameName = Uri.UnescapeDataString(gameName);
            gameName = gameName.Replace('+', ' ');
            gameName = System.Text.RegularExpressions.Regex.Replace(gameName, @"\s+", " ").Trim();

            string query = Uri.EscapeDataString(gameName);
            string url = $"https://www.steamgriddb.com/search/grids?term={query}";

            OpenUrl(url);
        }
    }
}
