using System;
using System.Threading;

namespace GameTimeX.Function
{
    internal class GameSessionThread
    {

        private Thread? th = null;
        private bool stopThread = false;
        private long startTime = 0;

        /// <summary>
        /// Initialisiert den Thread
        /// </summary>
        /// <param name="startTime">Startzeit der Zeitaufnahme</param>
        public GameSessionThread(long startTime)
        {
            this.startTime = startTime;
            th = new Thread(new ParameterizedThreadStart(HandleSessionGameText));
        }

        /// <summary>
        /// Kümmert sich um das Updaten des GameSession-Textes
        /// </summary>
        /// <param name="obj">Main Window</param>
        private void HandleSessionGameText(object? obj)
        {
            if (obj == null)
                return;

            MainWindow wnd = (MainWindow)obj;

            while (true)
            {
                if (stopThread)
                    break;

                // Logik
                // Session Text setzen
                wnd.Dispatcher.Invoke((Action)(() =>
                {
                    wnd.txtGameSession.Text = "Session: " + MonitorHandler.GetCurrentGameTimeInMinutes(startTime) + " minute(s)";
                }));

                // Zwei Sekunden schlafen legen
                Thread.Sleep(2000);
            }
        }

        /// <summary>
        /// Startet den GameSession-Thread
        /// </summary>
        /// <param name="objParm">Main Window</param>
        public void start(Object objParm)
        {

            if (th == null)
                return;

            try
            {
                // Thread starten
                th.Start(objParm);
            }
            catch (Exception ex)
            {
                // Infomeldung ausgeben
                InfoBox infoBox = new InfoBox("There was an error while starting a thread!");
                infoBox.ShowDialog();
            }
        }

        /// <summary>
        /// Stoppt den GameSessionThread
        /// </summary>
        public void stop()
        {
            stopThread = true;
            th = null;
        }
    }
}
