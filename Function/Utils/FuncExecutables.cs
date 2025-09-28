using System.Collections.Generic;
using System.IO;
using GameTimeX.DataBase.Objects;
using GameTimeX.Objects.Components;

namespace GameTimeX.Function.Utils
{
    internal class FuncExecutables
    {
        public static List<string> GetAllActiveExecutablesFromDBObj(DBO_Profile dbo_Profile)
        {
            List<string> activeExes = new List<string>();

            if (!Directory.Exists(dbo_Profile.ExtGameFolder))
                return activeExes;

            CExecutables cExecutables = new CExecutables(dbo_Profile.Executables).Dezerialize();

            foreach (var kvp in cExecutables.KeyValuePairs)
            {
                // Nur aktive Exes in die Liste aufnehmen
                if (kvp.Value)
                    activeExes.Add(kvp.Key);
            }

            return activeExes;
        }


        /// <summary>
        /// Liefert alle Exes (auch in Unterordnern) zum übergebenen Ordnerpfad
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<string> GetAllExecutablesFromDirectory(string directoryPath)
        {
            var allExes = new List<string>();

            if (!Directory.Exists(directoryPath))
                return allExes;

            // Alle .exe-Dateien im angegebenen Verzeichnis und in Unterverzeichnissen abrufen
            string[] exeFiles = Directory.GetFiles(directoryPath, "*.exe", SearchOption.AllDirectories);

            // Alle gefundenen .exe-Dateien sammeln
            foreach (string exeFile in exeFiles)
            {
                allExes.Add(Path.GetFileName(exeFile));
            }

            return allExes;
        }
    }
}
