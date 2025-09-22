using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GameTimeX.Function
{
    internal static class SteamLibrariesHandler
    {
        /// <summary>
        /// Liefert alle steamapps-Ordner (Standard + zusätzliche Libraries).
        /// </summary>
        public static List<string> GetLibraryPaths(string steamRoot)
        {

            steamRoot = steamRoot.Replace("/", "\\");

            var paths = new List<string> { Path.Combine(steamRoot, "steamapps") };

            var vdf = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf)) return paths;

            var text = File.ReadAllText(vdf);

            // matched "path" "D:\\SteamLibrary"
            var rx = new Regex(@"""path""\s*""([^""]+)""", RegexOptions.IgnoreCase);
            foreach (Match m in rx.Matches(text))
            {
                var p = m.Groups[1].Value.Replace(@"\\", @"\");
                var steamapps = Path.Combine(p, "steamapps");
                if (Directory.Exists(steamapps) && !paths.Contains(steamapps))
                    paths.Add(steamapps);
            }

            return paths;
        }
    }
}
