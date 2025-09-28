using System.IO;
using System.Windows;
using System.Windows.Media;
using GameTimeX.DataBase.DataManager;
using GameTimeX.Function.DataBase;
using GameTimeX.Function.Game;
using GameTimeX.Function.UserInterface;
using GameTimeX.Function.WorldEvents;
using GameTimeX.Objects;
using GameTimeX.XApplication.SubDisplays;

namespace GameTimeX.Function.AppEnvironment
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
        public static KeyInputHandler? keyInputHandlerBlackout;
        public static GameRunningHandler? gameRunningHandler = null;

        public static MainWindow? mainWindow = null;

        public static bool contextShown = false;

        public static int waitTimeGameRunningHandler = 2000;

        public static void InitializeSystem(MainWindow wnd)
        {

            // Loadingscreen zeigen
            LoadingApplication loadingApp = new LoadingApplication();
            loadingApp.Owner = wnd;
            loadingApp.Show();

            // Alle nötigen Ordner erstellen
            FileHandler.CreateProgramFoldersAndFiles();

            // Start-Parameter lesen
            startUpParms = FileHandler.ReadStartParms(startUpParmsPath);

            // List-View wurde mit 2.0.1 / Retouched entfernt
            // Hier für alle, die diese aktiviert hatten auf Tile-View ändern
            startUpParms.ViewMode = StartUpParms.ViewModes.TILES;

            // Toggle Button für "Nur spielbare Spiele" aktivieren/deaktivieren lt. StartupParms
            wnd.btnPlayableFilter.IsChecked = startUpParms.ShowOnlyPlayableGames;

            // Auf Backup Prüfen
            if (startUpParms.BackupType != StartUpParms.BackupTypes.NO_BACKUP || startUpParms.AutoBackup)
            {
                // Muss ein Backup erstellt werden?
                if (startUpParms.BackupType == StartUpParms.BackupTypes.CREATE_BACKUP || startUpParms.AutoBackup && startUpParms.BackupType != StartUpParms.BackupTypes.IMPORT_BACKUP)
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
                if (startUpParms.BackupType == StartUpParms.BackupTypes.IMPORT_BACKUP)
                {
                    // Backup importieren
                    FileHandler.ImportBackup();

                    InfoBox info = new InfoBox("Backup was imported successfully!");
                    info.Owner = Application.Current.MainWindow;
                    info.ShowDialog();
                }
            }

            bool db_created = DataBaseConnector.ConnectToSQLite();

            // Tabelle muss nur erzeugt werden, wenn es die Datenbank noch nicht gegeben hat
            if (db_created)
                DataBaseConnector.CreateTable();

            // Aufgrund der neu hinzugefügten Spalten ist eine Migration nötig

            // Info anzeigen am Loadingscreen
            loadingApp.ShowInfo("Migrating to new data model…");

            DataBaseConnector.MigrateDB();

            loadingApp.HideInfo();

            // Events initialisieren
            FuncYearStats.InitializeYearStats();
            FuncNewYear.InitializeHappyNewYearEvent();
            FuncHalloweenEvent.InitializeHalloweenEvent();



            //wnd.btnStartStopMonitoring.BitmapEffect = VisualHandler.GetDropShadowEffect();
            wnd.currProfileImage.Source = DisplayHandler.GetDefaultProfileImage();

            // GameRunningHandler starten
            gameRunningHandler = new GameRunningHandler();
            gameRunningHandler.Initialize(DM_Profile.ReadAll());
            gameRunningHandler.Start(waitTimeGameRunningHandler);

            if (startUpParms.MonitorShortcutActive)
            {
                // Starte KeyInputHandler
                keyInputHandler = new KeyInputHandler(startUpParms.MonitorShortcut, wnd);
                keyInputHandler.StartListening();
            }

            if (startUpParms.BlackOutShortcutActive)
            {
                // KeyInputHandler für BlackOut starten
                keyInputHandlerBlackout = new KeyInputHandler(wnd, KeyInputHandler.StartType.BLACKOUT_SCREEN);
                keyInputHandlerBlackout.StartListening();
            }

            loadingApp.Close();

            wnd.dgProfiles.Visibility = Visibility.Collapsed;
            wnd.grdGameProfiles.Visibility = Visibility.Visible;
            wnd.emptyStateOverlay.Visibility = Visibility.Collapsed;
        }

        public static void StopKeyInputHandler()
        {
            if (keyInputHandler != null)
                keyInputHandler.StopListening();
        }

        public static void StopGameRunningHandler()
        {
            if (gameRunningHandler != null)
                gameRunningHandler.Stop();
        }

        public static void RestartApplication()
        {
            if (keyInputHandler != null)
                keyInputHandler.StopListening();

            if (keyInputHandlerBlackout != null)
                keyInputHandlerBlackout.StopListening();

            if (gameRunningHandler != null)
                gameRunningHandler.Stop();

            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            Application.Current.Shutdown();
        }

        public static void ShutdownApplication()
        {
            if (keyInputHandler != null)
                keyInputHandler.StopListening();

            if (keyInputHandlerBlackout != null)
                keyInputHandlerBlackout.StopListening();

            if (gameRunningHandler != null)
                gameRunningHandler.Stop();

            Application.Current.Shutdown();
        }
    }
}
