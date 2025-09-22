using System;

namespace GameTimeX.Objects
{
    class CTodayStats : GTXComponent<CTodayStats>
    {

        // Heutiges Datum
        public string Date { get; set; } = DateTime.MinValue.ToShortDateString();

        // Startpunkt der Spielzeit (ähnlich zu Playthrough Start Point)
        public long playTime { get; set; }


        public CTodayStats(string rawValue) : base(rawValue) { }

        public CTodayStats() : base() { }


        public void ResetProfileStats(DBObject dbObject)
        {
            DateTime dateToday = DateTime.Now;

            // Date casten
            DateTime date = DateTime.Parse(Date);

            // Prüfen, ob heutiger Tage ungleich der Tag aus der DB ist
            if (date.Date != dateToday.Date)
            {
                // Dann Stats zurücksetzen
                this.Date = DateTime.Now.ToShortDateString();

                this.playTime = dbObject.GameTime;
            }

            dbObject.TodayStats = this.Serialize();
        }

    }
}
