using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static GameTimeX.Function.KeyInputHandler;

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
            foreach (DBObject profile in profiles)
            {
                if(profile.ExtGameFolder == string.Empty)
                    continue;

                // Für dieses Profil alle Exes holen
                List<string> exes = GetAllExecutablesFromDirectory(profile.ExtGameFolder);

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
                    foreach (string executable in executables)
                    {
                        if (IsProcessRunning(executable))
                        {
                            wnd.Dispatcher.Invoke((Action)(() =>
                            {
                                if (wnd.txtSearchBar.Text == String.Empty && lastSwitchedPID != profile.ProfileID)
                                {
                                    DisplayHandler.SwitchToSpecificGame(wnd, SysProps.startUpParms.ViewMode, profile.ProfileID);
                                    lastSwitchedPID = profile.ProfileID;
                                }

                            }));
                        }
                        // Wenn Prozess nicht läuft
                        else
                        {
                            // Prüfen ob es sich um den letzten Prozess handlet, wo geswitched wurde
                            // dann muss das Switchen wieder "freigegeben werden"
                            if (profile.ProfileID == lastSwitchedPID)
                            {
                                lastSwitchedPID = 0;
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

        /// <summary>
        /// Liefert alle Exes (auch in Unterordnern) zum übergebenen Ordnerpfad
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<string> GetAllExecutablesFromDirectory(string directoryPath)
        {
            List<string> allExes = new List<string>();

            if (!Directory.Exists(directoryPath))
                return allExes;

            // Alle .exe-Dateien im angegebenen Verzeichnis und in Unterverzeichnissen abrufen
            string[] exeFiles = Directory.GetFiles(directoryPath, "*.exe", SearchOption.AllDirectories);

            // Alle gefundenen .exe-Dateien ausgeben
            foreach (string exeFile in exeFiles)
            {
                
                allExes.Add(Path.GetFileName(exeFile));   
            }

            return allExes;
        }
    }
}
