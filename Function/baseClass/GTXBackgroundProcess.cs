using System.Threading;

namespace GameTimeX.Function.baseClass
{
    public abstract class GTXBackgroundProcess
    {
        private Thread? th = null;
        private bool stopThread = false;
        private bool running = false;
        private int waitTime = 0;

        public GTXBackgroundProcess() { }

        /// <summary>
        /// Startet den Background Process
        /// </summary>
        /// <param name="waitTime"></param>
        public void Start(int waitTime)
        {

            th = new Thread(Run);

            if (th == null)
                return;

            th.IsBackground = true;
            stopThread = false;
            this.waitTime = waitTime;
            th.Start();
            running = true;
        }

        /// <summary>
        /// Stoppt den Background Process
        /// </summary>
        public void Stop()
        {
            stopThread = true;
            th = null;
            running = false;
        }

        /// <summary>
        /// Gibt zurück, ob der Background Process derzeit läuft
        /// </summary>
        /// <returns></returns>
        public bool IsRunning()
        {
            return running;
        }

        private void Run()
        {
            while (IsRunning())
            {
                if (stopThread)
                    break;

                // Logik der Subklasse
                Logic();

                Thread.Sleep(waitTime);
            }
        }

        /// <summary>
        /// Wird vom Background Process im defnierten Zeitraum aufgerufen
        /// </summary>
        public abstract void Logic();
    }
}

