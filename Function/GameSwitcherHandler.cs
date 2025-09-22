using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GameTimeX.Objects;

namespace GameTimeX.Function
{
    class GameSwitcherHandler
    {
        private Thread? th = null;
        private bool stopThread = false;
        private int lastSwitchedPID = 0;
        MainWindow wnd;
        private bool running = false;

        public Dictionary<int, List<string>> ExecutablesToSearch { get; set; } = new Dictionary<int, List<string>>();

        private CurrentProfileRunning currentProfileRunning = new CurrentProfileRunning();

        public GameSwitcherHandler(MainWindow wnd)
        {
            this.wnd = wnd;
        }

        /// <summary>
        /// Fügt für alle übergebenen Profile die gefunden EXEs hinzu
        /// </summary>
        /// <param name="profiles"></param>
        public void InitializeFirst(List<DBObject> profiles)
        {
            // Neuinitialisierung
            ExecutablesToSearch = new Dictionary<int, List<string>>();

            foreach (DBObject profile in profiles)
            {
                if (profile.ExtGameFolder == string.Empty)
                    continue;

                // Für dieses Profil alle Exes holen
                List<string> exes = GetAllActiveExecutablesFromDBObj(profile);

                // Hinzufügen
                AddExecutables(profile.ProfileID, exes);
            }
        }

        public void Start()
        {

            th = new Thread(HandleGameSwitching);

            if (th == null)
                return;

            th.IsBackground = true;
            stopThread = false;
            th.Start();
            running = true;
        }

        public void Stop()
        {
            stopThread = true;
            th = null;
            running = false;
        }

        public bool IsRunning()
        {
            return running;
        }

        private void HandleGameSwitching()
        {

            while (true)
            {
                if (stopThread)
                    break;

                // Alle angegebenen Executables durchloopen
                List<DBObject> profiles = DataBaseHandler.ReadAll();

                foreach (DBObject profile in profiles)
                {
                    if (profile.ExtGameFolder == string.Empty) continue;
                    List<string> executables = ExecutablesToSearch[profile.ProfileID];
                    string exePath = profile.ExtGameFolder;

                    foreach (string executable in executables)
                    {
                        if (IsProcessRunningWithPathPart(executable, exePath))
                        {
                            wnd.Dispatcher.Invoke((Action)(() =>
                            {
                                // Prüfen, ob in Suchleiste nichts eingegeben und ob die derzeitige Executable noch nicht aufgenommen wurde
                                // Weiters muss geprüft werden, ob im Namen der EXE "Unity" vorkommt (z.B. UnityCrashHandler => kann bei mehreren Spielen laufen und ist kein eindeutiger Indikator!)
                                if (wnd.txtSearchBar.Text == String.Empty && !currentProfileRunning.executables.Contains(executable) && (!executable.Contains("Unity")) && (!executable.Contains("EpicWebHelper")))
                                {
                                    // Nur switchen, wenn noch nicht als aktuelles Profil selektiert!
                                    // Ansonsten gibt es einen komischen Bug mit der Selektionsanzeige (Border animiert öfter als 1x)
                                    if (SysProps.currentSelectedPID != currentProfileRunning.pid)
                                    {
                                        DisplayHandler.SwitchToSpecificGame(wnd, SysProps.startUpParms.ViewMode, profile.ProfileID);

                                        VisualHandler.ShowToastNotification("GameTimeX | Switched to...", profile.GameName, 3000);

                                        // Wenn gerade die Spielzeit aufgenommen wird, muss hier die Aufnahme gestoppt werden!!
                                        // Ansonsten würde die Aufnahme für ein anderes Spiel weiterlaufen, obwohl das Profil gewechselt wurde
                                        // Nicht gut!!
                                        if (MonitorHandler.CurrentlyMonitoringGameTime())
                                        {
                                            MonitorHandler.EndMonitoringGameTime(wnd);
                                        }
                                    }

                                    currentProfileRunning.pid = profile.ProfileID;
                                    currentProfileRunning.executables.Add(executable);
                                }

                            }));
                        }
                        // Wenn Prozess nicht läuft
                        else
                        {
                            // Wenn derzeit noch kein Spiel aus den Profilen läuft
                            // interessiert uns das hier noch nicht (ist nur unnötig hier immer ein neues CurrentProfileRunning zu instanziieren)
                            if (currentProfileRunning.pid != 0)
                            {
                                // Prüfen, ob alle Exe vom derzeit aktiven Profil nicht mehr laufen
                                foreach (string exe in currentProfileRunning.executables)
                                {
                                    if (exe == executable)
                                    {
                                        currentProfileRunning.countNotRunning++;
                                    }
                                }

                                if (currentProfileRunning.countNotRunning == currentProfileRunning.executables.Count)
                                {

                                    // Profilsettings deaktivieren (z.B. HDR)
                                    GameStarterHandler.DeactivateProfileSettings(currentProfileRunning.pid);

                                    // Keine der vorher aufgenommenen EXE laufen mehr
                                    // D.h. Spiel muss geschlossen worden sein
                                    // Also => Clearen
                                    currentProfileRunning = new CurrentProfileRunning();

                                }
                            }
                        }
                    }
                }

                // So genau muss das nicht sein (da wir nur auf "gerade laufend" überprüfen, ist eine leichte Verzögerung nicht schlimm)
                // schont aber etwas die CPU.
                Thread.Sleep(1000);
            }
        }

        private bool IsProcessRunning(string exeName)
        {
            // Hole alle laufenden Prozesse
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    // Überprüfe, ob der Prozess den Namen der EXE-Datei enthält
                    if (process.ProcessName.Equals(System.IO.Path.GetFileNameWithoutExtension(exeName), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return false;
        }

        public bool IsProcessRunningWithPathPart(string exeName, string partialPath)
        {
            var nameNoExt = Path.GetFileNameWithoutExtension(exeName);

            foreach (var p in Process.GetProcesses())
            {
                try
                {
                    if (!p.ProcessName.Equals(nameNoExt, StringComparison.OrdinalIgnoreCase))
                        continue;

                    string procPath = p.MainModule?.FileName;
                    if (string.IsNullOrEmpty(procPath))
                        continue;

                    if (procPath.IndexOf(partialPath, StringComparison.OrdinalIgnoreCase) >= 0)
                        return true;
                }
                catch
                {
                    return IsProcessRunning(exeName);
                }
                finally
                {
                    try { p.Dispose(); } catch { }
                }
            }
            return false;
        }


        /// <summary>
        /// Fügt zu einem Gameprofile eine Exe hinzu
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="exeName"></param>
        public void AddExecutable(int pid, string exeName)
        {
            List<string> executableList = ExecutablesToSearch[pid];

            executableList.Add(exeName);
        }

        /// <summary>
        /// Fügt zu einem Gameprofile eine Liste an Executables hinzu
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="executablesList"></param>
        public void AddExecutables(int pid, List<string> executablesList)
        {
            if (!ExecutablesToSearch.ContainsKey(pid))
            {
                ExecutablesToSearch.Add(pid, executablesList);
            }
            else
            {
                ExecutablesToSearch[pid] = executablesList;
            }


        }

        /// <summary>
        /// Entfernt für ein Gameprofile alle Executables
        /// </summary>
        /// <param name="pid"></param>
        public void RemoveExecutables(int pid)
        {
            ExecutablesToSearch[pid] = new List<string>();
        }

        /// <summary>
        /// Entfernt für die übergebene PID die Executables und den kompletten Eintrag des Profils
        /// </summary>
        /// <param name="pid"></param>
        public void RemoveProfileAndExecutables(int pid)
        {
            ExecutablesToSearch.Remove(pid);
        }

        public static List<string> GetAllActiveExecutablesFromDBObj(DBObject dbObject)
        {
            List<string> activeExes = new List<string>();

            if (!Directory.Exists(dbObject.ExtGameFolder))
                return activeExes;

            CExecutables cExecutables = new CExecutables(dbObject.Executables).Dezerialize();

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

        private class CurrentProfileRunning
        {
            public int pid = 0;
            public List<string> executables = new List<string>();
            public int countNotRunning = 0;
        }
    }
}
