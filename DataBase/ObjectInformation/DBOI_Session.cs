namespace GameTimeX.DataBase.ObjectInformation
{
    /// <summary>
    /// Index-/Info-Klasse für DBO_Session: Tabellen- und Spaltennamen zentral.
    /// Verwendung: DBOI_Session.SID, DBOI_Session.Q(DBOI_Session.Played_From)
    /// </summary>
    public static class DBOI_Session
    {
        // Tabellenname
        public const string TABLE = "tblGameSessions";

        // Spalten
        public const string SID = "SID";
        public const string FK_PID = "FK_PID";
        public const string Played_From = "Played_From";
        public const string Played_To = "Played_To";
        public const string Playtime = "Playtime";

        /// <summary>
        /// Qualifiziert einen Spaltennamen mit dem Tabellenalias oder Tabellennamen.
        /// </summary>
        public static string Q(string column, string? aliasOrTable = null)
            => $"{(string.IsNullOrWhiteSpace(aliasOrTable) ? TABLE : aliasOrTable)}.{column}";
    }
}
