using System;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Function.DataBase.Object;
using GameTimeX.Function.Game;
using GameTimeX.Function.UserInterface;

namespace GameTimeX.Function.Monitoring
{
    internal class MonitorHandler
    {
        private static long startTimeMonitoring = 0;
        private static long endTimeMonitoring = 0;
        private static int monitoringPid = 0;

        private static GameSessionThread? gameTimeSessionThread = null;

        private static DateTime startTimeMonitoringTime;
        private static DateTime endTimeMonitoringTime;

        /// <summary>
        /// Prüft, ob gerade ein Spiel aufgenommen wird
        /// </summary>
        /// <returns></returns>
        public static bool CurrentlyMonitoringGameTime()
        {
            return !(monitoringPid == 0);
        }

        /// <summary>
        /// Startet die Spielzeitaufzeichnung
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="pid"></param>
        public static void StartMonitoringGameTime(MainWindow wnd, int pid)
        {
            // Style ändern
            VisualHandler.ActivateMonitoringVisualButton(wnd.btnStartStopMonitoring);

            // Startzeit setzen
            startTimeMonitoring = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // GameSession Text Thread
            // Nur starten, wenn Option in Settings gesetzt
            if (SysProps.startUpParms.SessionGameTime)
            {
                VisualHandler.ActivateGameTimeSeesion(wnd.txtGameSession);
                gameTimeSessionThread = new GameSessionThread(startTimeMonitoring);
                gameTimeSessionThread.Start(wnd);
            }

            monitoringPid = pid;

            startTimeMonitoringTime = DateTimeOffset.Now.DateTime;

            // Erste Spielzeit speichern
            FN_Profile.SaveFirstTimePlayed(pid);
        }

        private static long GetCurrentGameTimeInMinutes()
        {
            long currentGameTime = 0;

            // Derzeitige Zeit berechnen
            currentGameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTimeMonitoring;
            currentGameTime = CalcMinutesFromMillis(currentGameTime);

            return currentGameTime;
        }

        public static long GetCurrentGameTimeInMinutes(long startTime)
        {
            long currentGameTime = 0;

            // Derzeitige Zeit berechnen
            currentGameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            currentGameTime = CalcMinutesFromMillis(currentGameTime);

            return currentGameTime;
        }

        /// <summary>
        /// Beendet die Spielzeitaufzeichnung und speichert die Werte in der Datenbank
        /// </summary>
        /// <param name="btn"></param>
        public static void EndMonitoringGameTime(MainWindow wnd)
        {
            // Thread beenden
            if (gameTimeSessionThread != null)
                gameTimeSessionThread.Stop();

            // Style ändern
            VisualHandler.DeactivateMonitoringVisualButton(wnd.btnStartStopMonitoring);
            VisualHandler.DeactivateGameTimeSeesion(wnd.txtGameSession);

            // Endzeit setzen
            endTimeMonitoring = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Spielzeit berechnen
            long playtime = endTimeMonitoring - startTimeMonitoring;

            endTimeMonitoringTime = DateTimeOffset.Now.DateTime;

            // In Datenbank abspeichern
            FN_Profile.SaveMonitoredTime(CalcMinutesFromMillis(playtime), monitoringPid, startTimeMonitoringTime, endTimeMonitoringTime);

            FN_Profile.SaveLastTimePlayed(monitoringPid);

            // Werte zurücksetzen
            ResetMonitoringValues();
        }

        /// <summary>
        /// Berechnet aus den Millis die Minuten
        /// </summary>
        /// <param name="millis"></param>
        /// <returns></returns>
        private static long CalcMinutesFromMillis(long millis)
        {
            return millis / 60000;
        }

        /// <summary>
        /// Setzt die Variablen der Klasse zurück auf 0
        /// </summary>
        private static void ResetMonitoringValues()
        {
            startTimeMonitoring = 0;
            endTimeMonitoring = 0;
            monitoringPid = 0;
        }

        /// <summary>
        /// Berechnet anhand der Minuten die Stunden
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static double CalcGameTime(long minutes)
        {
            if (minutes == 0)
            {
                return 0;
            }

            double returnVal = (double)minutes / 60;

            return returnVal;
        }
    }
}
