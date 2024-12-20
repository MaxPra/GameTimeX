using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GameTimeX
{
    internal class MonitorHandler
    {
        private static long startTimeMonitoring = 0;
        private static long endTimeMonitoring = 0;
        private static int monitoringPid = 0;

        /// <summary>
        /// Prüft, ob gerade ein Spiel aufgenommen wird
        /// </summary>
        /// <returns></returns>
        public static bool currentlyMonitoringGameTime()
        {
            return !(monitoringPid == 0);
        }

        /// <summary>
        /// Startet die Spielzeitaufzeichnung
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="pid"></param>
        public static void startMonitoringGameTime(Button btn, int pid)
        {
            // Style ändern
            VisualHandler.activateMonitoringVisualButton(btn);

            // Startzeit setzen
            startTimeMonitoring = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            monitoringPid = pid;

            // Erste Spielzeit speichern
            DataBaseHandler.saveFirstTimePlayed(pid);
        }

        /// <summary>
        /// Beendet die Spielzeitaufzeichnung und speichert die Werte in der Datenbank
        /// </summary>
        /// <param name="btn"></param>
        public static void endMonitoringGameTime(Button btn)
        {
            // Style ändern
            VisualHandler.deactivateMonitoringVisualButton(btn);

            // Endzeit setzen
            endTimeMonitoring = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Spielzeit berechnen
            long playtime = endTimeMonitoring - startTimeMonitoring;

            // In Datenbank abspeichern
            DataBaseHandler.saveMonitoredTime(calcMinutesFromMillis(playtime), monitoringPid);

            DataBaseHandler.saveLastTimePlayed(monitoringPid);

            // Werte zurücksetzen
            resetMonitoringValues();
        }

        /// <summary>
        /// Berechnet aus den Millis die Minuten
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        private static long calcMinutesFromMillis(long millis)
        {
            return millis / 60000;
        }

        /// <summary>
        /// Setzt die Variablen der Klasse zurück auf 0
        /// </summary>
        private static void resetMonitoringValues()
        {
            startTimeMonitoring = 0;
            endTimeMonitoring = 0;
            monitoringPid = 0;
        }

        /// <summary>
        /// Berechnet anhand der Millisekunden die Stunden
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static double calcGameTime(long minutes)
        {
            if(minutes == 0)
            {
                return 0;
            }

            double returnVal = (double) minutes / 60;

            return returnVal;
        }
    }
}
