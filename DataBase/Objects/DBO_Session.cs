using System;
using GameTimeX.DataBase.ObjectInformation;

namespace GameTimeX.DataBase.Objects
{
    public class DBO_Session
    {
        public static string GetTableName() => DBOI_Session.TABLE;

        public int SID { get; set; }              // PK (AUTOINCREMENT)
        public int FK_PID { get; set; }           // FK -> tblGameProfiles.ProfileID
        public DateTime Played_From { get; set; } // Start der Session
        public DateTime Played_To { get; set; }   // Ende der Session
        public double Playtime { get; set; }      // Minuten (Double, erlaubt Kommawerte)

        public DBO_Session()
        {
            SID = 0;
            FK_PID = 0;
            Played_From = DateTime.MinValue;
            Played_To = DateTime.MinValue;
            Playtime = 0.0;
        }
    }
}
