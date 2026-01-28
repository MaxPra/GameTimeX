using System.Collections.Generic;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Function.baseClass;
using GameTimeX.Function.Monitoring;
using GameTimeX.Function.UserInterface;
using GameTimeX.Function.Utils;
using GameTimeX.Function.Windows;

namespace GameTimeX.Function.Game
{
    public class GameRunningHandler : GTXBackgroundProcess
    {

        // -- Attributes --
        public Dictionary<int, List<string>> ExecutablesToSearch { get; set; } = new Dictionary<int, List<string>>();
        private CurrentProfileRunning currentProfileRunning = new CurrentProfileRunning();


        public GameRunningHandler() : base() { }

        public override void Logic()
        {
            // Alle Profile durchloopen
            List<DBO_Profile> profiles = DM_Profile.ReadAll();

            foreach (DBO_Profile profile in profiles)
            {
                if (profile.ExtGameFolder == string.Empty)
                    continue;

                List<string> executables = ExecutablesToSearch[profile.ProfileID];

                string exePath = profile.ExtGameFolder;

                foreach (string executable in executables)
                {
                    if (FuncWindowsProcess.IsProcessRunningWithPathPart(executable, exePath))
                    {
                        SysProps.mainWindow.Dispatcher.Invoke(() =>
                        {
                            // Prüfen, ob in Suchleiste nichts eingegeben und ob die derzeitige Executable noch nicht aufgenommen wurde
                            if (SysProps.mainWindow.txtSearchBar.Text == string.Empty && !currentProfileRunning.executables.Contains(executable))
                            {

                                if (SysProps.infoBoxGameStarting != null)
                                {
                                    SysProps.infoBoxGameStarting.Close();
                                    SysProps.infoBoxGameStarting = null;
                                }

                                SwitchToGame(profile);

                                currentProfileRunning.pid = profile.ProfileID;
                                currentProfileRunning.executables.Add(executable);
                                currentProfileRunning.path = exePath;
                            }

                        });
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
                                if (exe == executable && exePath == currentProfileRunning.path)
                                {
                                    currentProfileRunning.countNotRunning++;
                                }
                            }

                            // Spiel wurde geschlossen
                            if (currentProfileRunning.countNotRunning == currentProfileRunning.executables.Count)
                            {

                                // Wenn Monitoring rennt --> beenden (weil Spiel geschlossen wurde)
                                // e.g. Absturz, etc.
                                if (MonitorHandler.CurrentlyMonitoringGameTime())
                                {
                                    DBO_Profile dbo_Profile = DM_Profile.ReadPID(SysProps.currentSelectedPID);
                                    string profileName = dbo_Profile.GameName;

                                    string title = "GameTimeX | " + profileName;

                                    MonitorHandler.EndMonitoringGameTime(SysProps.mainWindow);

                                    // Wenn Seitenmonitore blacked --> deaktivieren, wenn Spiel geschlossen
                                    if (BlackoutHandler.IsActive())
                                    {
                                        BlackoutHandler.ToggleSecondaryBlackout(SysProps.mainWindow);
                                    }

                                    VisualHandler.ShowToastNotification(title, "Monitoring stopped!", 3000);

                                    SysProps.mainWindow.Dispatcher.Invoke(() =>
                                    {
                                        DisplayHandler.BuildInfoDisplay(dbo_Profile.ProfileID, SysProps.mainWindow);
                                    });
                                }

                                // Start Game Button aktivieren
                                VisualHandler.EnableStartGameButtons();

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
        }

        private void SwitchToGame(DBO_Profile profile)
        {
            // Nur switchen, wenn in den Einstellungen aktiviert
            if (!SysProps.startUpParms.AutoProfileSwitching)
                return;

            // Nur switchen, wenn noch nicht als aktuelles Profil selektiert
            if (SysProps.currentSelectedPID != currentProfileRunning.pid)
            {
                DisplayHandler.SwitchToSpecificGame(SysProps.mainWindow, SysProps.startUpParms.ViewMode, profile.ProfileID);

                VisualHandler.ShowToastNotification("GameTimeX | Switched to...", profile.GameName, 3000);

                // Wenn gerade die Spielzeit aufgenommen wird, muss hier die Aufnahme gestoppt werden
                if (MonitorHandler.CurrentlyMonitoringGameTime())
                {
                    MonitorHandler.EndMonitoringGameTime(SysProps.mainWindow);
                }
            }
        }

        /// <summary>
        /// Fügt für alle übergebenen Profile die gefunden EXEs hinzu
        /// </summary>
        /// <param name="profiles"></param>
        public void Initialize(List<DBO_Profile> profiles)
        {
            // Neuinitialisierung
            ExecutablesToSearch = new Dictionary<int, List<string>>();

            foreach (DBO_Profile profile in profiles)
            {
                if (profile.ExtGameFolder == string.Empty)
                    continue;

                // Für dieses Profil alle Exes holen
                List<string> exes = FuncExecutables.GetAllActiveExecutablesFromDBObj(profile);

                // Hinzufügen
                AddExecutables(profile.ProfileID, exes);
            }
        }

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

        private class CurrentProfileRunning
        {
            public int pid = 0;
            public List<string> executables = new List<string>();
            public int countNotRunning = 0;
            public string path = string.Empty;
        }
    }
}
