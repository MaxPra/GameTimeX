using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.IO;
using System.Windows;
using GameTimeX.Objects;

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
        public static Brush defButtonColor = VisualHandler.convertHexToBrush("#0099ff");
        public static Brush defButtonHoverColor = VisualHandler.convertHexToBrush("#008ae6");
        public static Brush emptyFieldsColor = Brushes.Red;
        public static Brush monitoringRunningColor = VisualHandler.convertHexToBrush(hexValMonitoringActive);


        // ---- Database ----
        public static string dbFilePath = programPathFolder + Path.DirectorySeparatorChar + "GameTimeXDB.db";
        public static string apos = "'";

        // ---- Monitoring ----
        public static int currentSelectedPID = 0;
        public static string stopMonitoringText = "Stop Monitoring";
        public static string startMonitoringText = "Start Monitoring";

        // ---- Start-Up-Params ----
        public static StartUpParms startUpParms = null;
        

        public static void initializeSystem(MainWindow wnd)
        {
            // Alle nötigen Ordner erstellen
            FileHandler.createProgramFoldersAndFiles();

            // Start-Parameter lesen
            SysProps.startUpParms = FileHandler.readStartParms(startUpParmsPath);
            
            // Auf Backup Prüfen
            if(startUpParms.BackupType != StartUpParms.BackupTypes.NO_BACKUP)
            {
                // Muss ein Backup erstellt werden?
                if(startUpParms.BackupType == StartUpParms.BackupTypes.CREATE_BACKUP)
                {
                    // Backup erstellen
                    FileHandler.createBackup();

                    InfoBox info = new InfoBox("Backup was created successfully!");
                    info.Owner = Application.Current.MainWindow;
                    info.ShowDialog();
                }

                // Muss ein Backup importiert werden?
                if(startUpParms.BackupType == StartUpParms.BackupTypes.IMPORT_BACKUP)
                {
                    // Backup importieren
                    FileHandler.importBackup();

                    InfoBox info = new InfoBox("Backup was imported successfully!");
                    info.Owner = Application.Current.MainWindow;
                    info.ShowDialog();
                }
            }                

            bool db_created = DataBaseHandler.connectToSQLite();

            // Tabelle muss nur erzeugt werden, wenn es die Datenbank noch nicht gegeben hat
            if (db_created)
            {
                DataBaseHandler.createTable();
            }

            wnd.currProfileImage.BitmapEffect = VisualHandler.getDropShadowEffect();
            //wnd.btnStartStopMonitoring.BitmapEffect = VisualHandler.getDropShadowEffect();
            wnd.currProfileImage.Source = DisplayHandler.getDefaultProfileImage();
        }

        public static void restartApplication()
        {
            System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            App.Current.Shutdown();
        }

        

        
    }
}
