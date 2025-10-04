using System;
using GameTimeX.DataBase.Objects;
using GameTimeX.Objects.baseClass;

namespace GameTimeX.Objects.Components
{
    class CTodayStats : GTXComponent<CTodayStats>
    {

        // Heutiges Datum
        public string Date { get; set; } = DateTime.MinValue.ToShortDateString();

        // Startpunkt der Spielzeit (ähnlich zu Playthrough Start Point)
        public double playTime { get; set; }


        public CTodayStats(string rawValue) : base(rawValue) { }

        public CTodayStats() : base() { }


        public void ResetProfileStats(DBO_Profile dbo_Profile)
        {
            DateTime dateToday = DateTime.Now;

            // Date casten
            DateTime date = DateTime.Parse(Date);

            // Prüfen, ob heutiger Tage ungleich der Tag aus der DB ist
            if (date.Date != dateToday.Date)
            {
                // Dann Stats zurücksetzen
                Date = DateTime.Now.ToShortDateString();

                playTime = dbo_Profile.GameTime;
            }

            dbo_Profile.TodayStats = Serialize();
        }

    }
}
