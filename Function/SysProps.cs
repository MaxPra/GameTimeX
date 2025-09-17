using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Windows;
using GameTimeX.Objects;
using GameTimeX.Function;
using System.Threading;
using GameTimeX.XApplication.SubDisplays;

namespace GameTimeX
{
    internal class SysProps
    {
        public static char separator = Path.DirectorySeparatorChar;

        // ---- Paths ----
        public static string programPathFolder = "C:\\GameTimeX";
        public static string picDestPath = programPathFolder + Path.DirectorySeparatorChar + "images";
        public static string startUpParmsPath = programPathFolder + Path.DirectorySeparatorChar + "startUpParms.json";

        // Colors
        public static string hexValDef = "#0099ff";
        public static string hexValDefHover = "#008ae6";
        public static string hexValCloseWindow = "#b50b0b";
        public static string hexValMonitoringActive = "#06d14d";

        // ---- Brushs ----
        public static Brush defButtonColor = VisualHandler.ConvertHexToBrush("#0099ff");
        public static Brush defButtonHoverColor = VisualHandler.ConvertHexToBrush("#008ae6");
        public static Brush emptyFieldsColor = Brushes.Red;
        public static Brush monitoringRunningColor = VisualHandler.ConvertHexToBrush(hexValMonitoringActive);

        // ---- Database ----
        public static string dbFilePath = programPathFolder + Path.DirectorySeparatorChar + "GameTimeXDB.db";
        public static string apos = "'";

        // ---- Monitoring ----
        public static int currentSelectedPID = 0;
        public static string stopMonitoringText = "Stop Monitoring";
        public static string startMonitoringText = "Start Monitoring";

        // ---- Start-Up-Params ----
        public static StartUpParms startUpParms = null;

        public static KeyInputHandler? keyInputHandler;
        public static GameSwitcherHandler? gameSwitcherHandler = null;

        public static MainWindow? mainWindow = null;

        public static bool contextShown = false;
        

        public static void InitializeSystem(MainWindow wnd)
        {

            // Loadingscreen zeigen
            LoadingApplication loadingApp = new LoadingApplication();
            loadingApp.Owner = wnd;
            loadingApp.Show();

            // Alle nötigen Ordner erstellen
            FileHandler.CreateProgramFoldersAndFiles();

            // Start-Parameter lesen
            SysProps.startUpParms = FileHandler.ReadStartParms(startUpParmsPath);

            // List-View wurde mit 2.0.1 / Retouched entfernt
            // Hier für alle, die diese aktiviert hatten auf Tile-View ändern
            SysProps.startUpParms.ViewMode = StartUpParms.ViewModes.TILES;
            
            // Auf Backup Prüfen
            if(startUpParms.BackupType != StartUpParms.BackupTypes.NO_BACKUP || startUpParms.AutoBackup)
            {
                // Muss ein Backup erstellt werden?
                if(startUpParms.BackupType == StartUpParms.BackupTypes.CREATE_BACKUP || (startUpParms.AutoBackup && startUpParms.BackupType != StartUpParms.BackupTypes.IMPORT_BACKUP))
                {
                    // Backup erstellen
                    FileHandler.CreateBackup();

                    if (!startUpParms.AutoBackup)
                    {
                        InfoBox info = new InfoBox("Backup was created successfully!");
                        info.Owner = Application.Current.MainWindow;
                        info.ShowDialog();
                    }
                }

                // Muss ein Backup importiert werden?
                if(startUpParms.BackupType == StartUpParms.BackupTypes.IMPORT_BACKUP)
                {
                    // Backup importieren
                    FileHandler.ImportBackup();

                    InfoBox info = new InfoBox("Backup was imported successfully!");
                    info.Owner = Application.Current.MainWindow;
                    info.ShowDialog();
                }
            }                

            bool db_created = DataBaseHandler.ConnectToSQLite();

            // Tabelle muss nur erzeugt werden, wenn es die Datenbank noch nicht gegeben hat
            if (db_created)
            {
                DataBaseHandler.CreateTable();
            }

            // Aufgrund der neu hinzugefügten Spalte ExtGamePath ist eine Migration notwendig!
            //DataBaseHandler.MigrateDB();

            //wnd.btnStartStopMonitoring.BitmapEffect = VisualHandler.GetDropShadowEffect();
            wnd.currProfileImage.Source = DisplayHandler.GetDefaultProfileImage();


            if (startUpParms.MonitorShortcutActive)
            {
                // Starte KeyInputHandler
                keyInputHandler = new KeyInputHandler(startUpParms.MonitorShortcut, wnd);
                keyInputHandler.StartListening();
            }

            // Wenn das autom. wechseln der Spielprofile aktiviert ist => GameSwitcherHandler starten (Überwachung der Prozesse)
            if (startUpParms.AutoProfileSwitching)
            {
                // GameSwitcher starten
                gameSwitcherHandler = new GameSwitcherHandler(wnd);
                gameSwitcherHandler.InitializeFirst(DataBaseHandler.ReadAll());
                gameSwitcherHandler.Start();
            }

            loadingApp.Close();
        }

        public static void StopKeyInputHandler()
        {
            if(keyInputHandler != null)
                keyInputHandler.StopListening();
        }

        public static void StopGameSwicherHandler()
        {
            if (gameSwitcherHandler != null)
                gameSwitcherHandler.Stop();
        }
        
        public static void RestartApplication()
        {
            if (keyInputHandler != null)
                keyInputHandler.StopListening();

            if (gameSwitcherHandler != null)
                gameSwitcherHandler.Stop();

            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            App.Current.Shutdown();
        }

        public static void ShutdownApplication()
        {
            if (keyInputHandler != null)
                keyInputHandler.StopListening();

            if (gameSwitcherHandler != null)
                gameSwitcherHandler.Stop();

            Application.Current.Shutdown();
        }
    }
}
