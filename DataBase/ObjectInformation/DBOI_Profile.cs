namespace GameTimeX.DataBase.ObjectInformation
{
    /// <summary>
    /// Index-/Info-Klasse für DBO_Profile: Tabellen- und Spaltennamen zentral.
    /// Verwendung: DBOI_Profile.ProfileID, DBOI_Profile.Q(DBOI_Profile.GameName)
    /// </summary>
    public static class DBOI_Profile
    {
        // Tabellenname
        public const string TABLE = "tblGameProfiles";

        // Spalten
        public const string ProfileID = "ProfileID";
        public const string GameName = "GameName";
        public const string GameTime = "GameTime";
        public const string FirstPlay = "FirstPlay";
        public const string LastPlay = "LastPlay";
        public const string ProfilePicFileName = "ProfilePicFileName";
        public const string ExtGameFolder = "ExtGameFolder";
        public const string CreatedAt = "CreatedAt";
        public const string ChangedAt = "ChangedAt";
        public const string SteamAppID = "SteamAppID";
        public const string ProfileSettings = "ProfileSettings";
        public const string TodayStats = "TodayStats";
        public const string Executables = "Executables";
        public const string PlaythroughStartPointDate = "PlaythroughStartPointDate";

        /// <summary>
        /// Qualifiziert einen Spaltennamen mit dem Tabellenalias oder Tabellennamen.
        /// </summary>
        public static string Q(string column, string? aliasOrTable = null)
            => $"{(string.IsNullOrWhiteSpace(aliasOrTable) ? TABLE : aliasOrTable)}.{column}";
    }
}
