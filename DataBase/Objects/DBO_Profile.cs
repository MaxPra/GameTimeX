using System;
using GameTimeX.DataBase.ObjectInformation;

namespace GameTimeX.DataBase.Objects
{
    public class DBO_Profile
    {
        public static string GetTableName() => DBOI_Profile.TABLE;

        private string gameName;
        private long gameTime;
        private DateTime firstPlay;
        private DateTime lastPlay;
        private string profilePicFileName;
        private DateTime createdAt;
        private DateTime changedAt;
        private int profileID;
        private string extGameFolder;
        private string executables;
        private DateTime playthroughStartPointDate;

        public string GameName { get => gameName; set => gameName = value; }
        public long GameTime { get => gameTime; set => gameTime = value; }
        public DateTime FirstPlay { get => firstPlay; set => firstPlay = value; }
        public DateTime LastPlay { get => lastPlay; set => lastPlay = value; }
        public string ProfilePicFileName { get => profilePicFileName; set => profilePicFileName = value; }
        public DateTime CreatedAt { get => createdAt; set => createdAt = value; }
        public DateTime ChangedAt { get => changedAt; set => changedAt = value; }
        public int ProfileID { get => profileID; set => profileID = value; }
        public string ExtGameFolder { get => extGameFolder; set => extGameFolder = value; }
        public string Executables { get => executables; set => executables = value; }

        public DateTime PlaythroughStartPointDate { get => playthroughStartPointDate; set => playthroughStartPointDate = value; }

        public int SteamAppID { get; set; }
        public string ProfileSettings { get; set; }
        public string TodayStats { get; set; }
    }
}
