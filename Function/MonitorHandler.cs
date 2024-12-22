﻿using GameTimeX.Function;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Controls;

namespace GameTimeX
{
    internal class MonitorHandler
    {
        private static long startTimeMonitoring = 0;
        private static long endTimeMonitoring = 0;
        private static int monitoringPid = 0;

        private static GameSessionThread? gameTimeSessionThread = null;

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
        public static void startMonitoringGameTime(MainWindow wnd, int pid)
        {
            // Style ändern
            VisualHandler.activateMonitoringVisualButton(wnd.btnStartStopMonitoring);

            // Startzeit setzen
            startTimeMonitoring = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // GameSession Text Thread
            // Nur starten, wenn Option in Settings gesetzt
            if (SysProps.startUpParms.SessionGameTime)
            {
                VisualHandler.activateGameTimeSeesion(wnd.txtGameSession);
                gameTimeSessionThread = new GameSessionThread(startTimeMonitoring);
                gameTimeSessionThread.start(wnd);
            }

            monitoringPid = pid;

            // Erste Spielzeit speichern
            DataBaseHandler.saveFirstTimePlayed(pid);
        }

        private static long GetCurrentGameTimeInMinutes()
        {
            long currentGameTime = 0;

            // Derzeitige Zeit berechnen
            currentGameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTimeMonitoring;
            currentGameTime = calcMinutesFromMillis(currentGameTime);

            return currentGameTime;
        }

        public static long GetCurrentGameTimeInMinutes(long startTime)
        {
            long currentGameTime = 0;

            // Derzeitige Zeit berechnen
            currentGameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - startTime;
            currentGameTime = calcMinutesFromMillis(currentGameTime);

            return currentGameTime;
        }

        /// <summary>
        /// Beendet die Spielzeitaufzeichnung und speichert die Werte in der Datenbank
        /// </summary>
        /// <param name="btn"></param>
        public static void endMonitoringGameTime(MainWindow wnd)
        {
            // Thread beenden
            if(gameTimeSessionThread != null)
                gameTimeSessionThread.stop();

            // Style ändern
            VisualHandler.deactivateMonitoringVisualButton(wnd.btnStartStopMonitoring);
            VisualHandler.deactivateGameTimeSeesion(wnd.txtGameSession);

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
