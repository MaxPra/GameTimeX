using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Threading;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function.AppEnvironment;

namespace GameTimeX.Function.DataBase
{
    internal class DataBaseConnector
    {
        public static SQLiteConnection connection = null;

        // ---------------------------
        // Public API
        // ---------------------------

        public static bool CreateDB()
        {
            if (File.Exists(SysProps.dbFilePath))
                return false;

            // Datei erzeugen und Handle sofort schließen (sonst Windows-Hold).
            using (File.Create(SysProps.dbFilePath)) { }
            return true;
        }

        public static void MigrateDB()
        {
            // Spalten-Additions (unkritisch, bleiben auf globaler connection)
            EnsureColumnExists("tblGameProfiles", "ExtGameFolder", AlterTableExtGameFolder);
            EnsureColumnExists("tblGameProfiles", "Executables", AlterTableExecutables);
            EnsureColumnExists("tblGameProfiles", "SteamAppID", AlterTableSteamAppID);
            EnsureColumnExists("tblGameProfiles", "ProfileSettings", AlterTableProfileSettings);
            EnsureColumnExists("tblGameProfiles", "TodayStats", AlterTableTodayStats);

            // Neue Spalte PlaythroughStartPointDate
            EnsureColumnExists("tblGameProfiles", "PlaythroughStartPointDate", AlterTableAddPlaythroughStartPointDate);

            // Alte Spalte StartpointPlaythroughTime entfernen -> Rebuild (auf eigener exklusiver Verbindung)
            if (ColumnExists("tblGameProfiles", "StartpointPlaythroughTime"))
            {
                RebuildProfilesTableWithout_StartpointPlaythroughTime();
            }

            // Sessions-Tabelle + Legacy-Übertrag
            if (!TableExists("tblGameSessions"))
            {
                CreateTable_Sessions();
                MigrateOldPlaytimeToSessions();
            }
        }

        public static void CreateTable()
        {
            if (connection == null) return;

            using (var cmd = new SQLiteCommand(
                "CREATE TABLE tblGameProfiles (" +
                "ProfileID INTEGER PRIMARY KEY, " +
                "GameName VARCHAR(200), " +
                "GameTime BIGINT, " +
                "FirstPlay DATETIME, " +
                "LastPlay DATETIME, " +
                "ProfilePicFileName varchar(10000), " +
                "ExtGameFolder varchar(1000), " +
                "CreatedAt DATETIME, " +
                "ChangedAt DATETIME, " +
                "SteamAppID INTEGER, " +
                "ProfileSettings TEXT, " +
                "TodayStats TEXT, " +
                "Executables TEXT, " +
                "PlaythroughStartPointDate DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00')",
                connection))
            {
                cmd.ExecuteNonQuery();
            }

            CreateTable_Sessions();
        }

        public static bool ConnectToSQLite()
        {
            string connectionString;
            bool newDB;

            if (!File.Exists(SysProps.dbFilePath))
            {
                connectionString = $"Data Source={SysProps.dbFilePath};Version=3;New=True;Compress=True;BusyTimeout=15000;Pooling=False;";
                newDB = true;
            }
            else
            {
                connectionString = $"Data Source={SysProps.dbFilePath};Version=3;Compress=True;BusyTimeout=15000;Pooling=False;";
                newDB = false;
            }

            connection = new SQLiteConnection(connectionString);
            try { connection.Open(); } catch { }

            try
            {
                using var fkOn = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection);
                fkOn.ExecuteNonQuery();
            }
            catch { }

            return newDB;
        }

        public static string ToSQLDateFormat(DateTime date)
            => date.ToString("yyyy-MM-dd HH:mm:ss");

        // ---------------------------
        // Helpers: schema detection
        // ---------------------------

        private static void EnsureColumnExists(string table, string col, Action addColumnAction)
        {
            using var cmd = new SQLiteCommand($"SELECT 1 FROM PRAGMA_table_info('{table}') WHERE name = @n;", connection);
            cmd.Parameters.AddWithValue("@n", col);
            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                addColumnAction();
        }

        private static bool ColumnExists(string table, string col)
        {
            using var cmd = new SQLiteCommand($"SELECT 1 FROM PRAGMA_table_info('{table}') WHERE name = @n;", connection);
            cmd.Parameters.AddWithValue("@n", col);
            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }

        private static bool TableExists(string table)
        {
            using var cmd = new SQLiteCommand("SELECT 1 FROM sqlite_master WHERE type='table' AND name=@t;", connection);
            cmd.Parameters.AddWithValue("@t", table);
            using var reader = cmd.ExecuteReader();
            return reader.Read();
        }

        // ---------------------------
        // Column migrations (global conn)
        // ---------------------------

        private static void AlterTableExtGameFolder()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN ExtGameFolder varchar(1000);", connection))
                cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET ExtGameFolder = '' WHERE ExtGameFolder IS NULL;", connection))
                cmd.ExecuteNonQuery();
        }

        private static void AlterTableExecutables()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN Executables TEXT;", connection))
                cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET Executables = '' WHERE Executables IS NULL;", connection))
                cmd.ExecuteNonQuery();
        }

        private static void AlterTableSteamAppID()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN SteamAppID INTEGER;", connection))
                cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET SteamAppID = 0 WHERE SteamAppID IS NULL;", connection))
                cmd.ExecuteNonQuery();
        }

        private static void AlterTableProfileSettings()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN ProfileSettings TEXT;", connection))
                cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET ProfileSettings = '' WHERE ProfileSettings IS NULL;", connection))
                cmd.ExecuteNonQuery();
        }

        private static void AlterTableTodayStats()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN TodayStats TEXT;", connection))
                cmd.ExecuteNonQuery();
            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET TodayStats = '' WHERE TodayStats IS NULL;", connection))
                cmd.ExecuteNonQuery();
        }

        private static void AlterTableAddPlaythroughStartPointDate()
        {
            using (var cmd = new SQLiteCommand(
                "ALTER TABLE tblGameProfiles " +
                "ADD COLUMN PlaythroughStartPointDate DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00';",
                connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // ---------------------------
        // Robust rebuild (separate conn + retry)
        // ---------------------------

        private static void RebuildProfilesTableWithout_StartpointPlaythroughTime()
        {
            // Merke ConnectionString und schließe globale Connection, um ALLE Handles freizugeben
            var cs = connection.ConnectionString;
            bool reopen = connection.State == ConnectionState.Open;
            try { connection.Close(); } catch { }
            SQLiteConnection.ClearAllPools();

            // Mit eigener exklusiver Verbindung + Retries arbeiten
            using (var conn = new SQLiteConnection(cs))
            {
                conn.Open();

                ExecNonQueryRetry(conn, "PRAGMA busy_timeout = 20000;");
                ExecNonQueryRetry(conn, "PRAGMA foreign_keys = OFF;");

                // Transaktion mit IMMEDIATE (reservierter Lock, DDL hebt auf exklusiv)
                ExecNonQueryRetry(conn, "BEGIN IMMEDIATE;");

                bool ok = false;
                try
                {
                    ExecNonQueryRetry(conn,
                        "CREATE TABLE tblGameProfiles_new (" +
                        "ProfileID INTEGER PRIMARY KEY, " +
                        "GameName VARCHAR(200), " +
                        "GameTime BIGINT, " +
                        "FirstPlay DATETIME, " +
                        "LastPlay DATETIME, " +
                        "ProfilePicFileName varchar(10000), " +
                        "ExtGameFolder varchar(1000), " +
                        "CreatedAt DATETIME, " +
                        "ChangedAt DATETIME, " +
                        "SteamAppID INTEGER, " +
                        "ProfileSettings TEXT, " +
                        "TodayStats TEXT, " +
                        "Executables TEXT, " +
                        "PlaythroughStartPointDate DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00')");

                    ExecNonQueryRetry(conn,
                        "INSERT INTO tblGameProfiles_new (" +
                        "ProfileID, GameName, GameTime, FirstPlay, LastPlay, ProfilePicFileName, ExtGameFolder, " +
                        "CreatedAt, ChangedAt, SteamAppID, ProfileSettings, TodayStats, Executables, PlaythroughStartPointDate) " +
                        "SELECT " +
                        "ProfileID, GameName, GameTime, FirstPlay, LastPlay, ProfilePicFileName, ExtGameFolder, " +
                        "CreatedAt, ChangedAt, SteamAppID, ProfileSettings, TodayStats, Executables, " +
                        "COALESCE(PlaythroughStartPointDate, '0001-01-01 00:00:00') " +
                        "FROM tblGameProfiles;");

                    ExecNonQueryRetry(conn, "DROP TABLE tblGameProfiles;");
                    ExecNonQueryRetry(conn, "ALTER TABLE tblGameProfiles_new RENAME TO tblGameProfiles;");

                    ExecNonQueryRetry(conn, "COMMIT;");
                    ok = true;
                }
                finally
                {
                    if (!ok)
                    {
                        try { ExecNonQueryQuiet(conn, "ROLLBACK;"); } catch { }
                    }
                    ExecNonQueryQuiet(conn, "PRAGMA foreign_keys = ON;");
                }
            }

            // Globale Connection wieder öffnen
            if (reopen)
            {
                try
                {
                    connection.Open();
                    using var fkOn = new SQLiteCommand("PRAGMA foreign_keys = ON;", connection);
                    fkOn.ExecuteNonQuery();
                }
                catch { }
            }
        }

        // Retry-Helfer: führt NonQuery mit Backoff aus, falls DB "locked"/"busy"
        private static void ExecNonQueryRetry(SQLiteConnection conn, string sql, int maxAttempts = 8, int initialDelayMs = 200)
        {
            int attempt = 0;
            Exception last = null;
            int delay = initialDelayMs;

            while (attempt < maxAttempts)
            {
                try
                {
                    using var cmd = new SQLiteCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    return;
                }
                catch (SQLiteException ex) when (IsLockOrBusy(ex))
                {
                    last = ex;
                    Thread.Sleep(delay);
                    delay = Math.Min(delay * 2, 4000); // Exponentielles Backoff, begrenzt
                    attempt++;
                    continue;
                }
                catch (Exception ex)
                {
                    last = ex;
                    break;
                }
            }

            throw new InvalidOperationException($"SQL failed after retries: {sql}", last);
        }

        private static void ExecNonQueryQuiet(SQLiteConnection conn, string sql)
        {
            try { using var cmd = new SQLiteCommand(sql, conn); cmd.ExecuteNonQuery(); } catch { }
        }

        private static bool IsLockOrBusy(SQLiteException ex)
        {
            // System.Data.SQLite mappt "database is locked/busy" oft als ErrorCode=Busy/Locked
            return ex.ResultCode == SQLiteErrorCode.Busy ||
                   ex.ResultCode == SQLiteErrorCode.Locked ||
                   ex.Message.IndexOf("database is locked", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   ex.Message.IndexOf("database is busy", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // ---------------------------
        // Sessions
        // ---------------------------

        private static void CreateTable_Sessions()
        {
            using (var cmd = new SQLiteCommand(
                "CREATE TABLE tblGameSessions (" +
                "SID INTEGER PRIMARY KEY, " +
                "FK_PID INTEGER NOT NULL, " +
                "Played_From DATETIME NOT NULL, " +
                "Played_To DATETIME NOT NULL, " +
                "Playtime BIGINT NOT NULL DEFAULT 0, " +
                "FOREIGN KEY (FK_PID) REFERENCES tblGameProfiles(ProfileID) ON DELETE CASCADE)",
                connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        private static void MigrateOldPlaytimeToSessions()
        {
            // nutzt global connection; kommt NACH Rebuild
            List<DBO_Profile> dbo_profiles = DM_Profile.ReadAll();

            foreach (DBO_Profile dbo_prof in dbo_profiles)
            {
                if (dbo_prof.GameTime <= 0) continue;

                var dbo_Session = DM_Session.CreateNew(dbo_prof.ProfileID);
                dbo_Session.Played_From = dbo_prof.FirstPlay == DateTime.MinValue ? dbo_prof.LastPlay : dbo_prof.FirstPlay;
                dbo_Session.Played_To = dbo_prof.LastPlay == DateTime.MinValue ? dbo_prof.FirstPlay : dbo_prof.LastPlay;
                if (dbo_Session.Played_To < dbo_Session.Played_From)
                    (dbo_Session.Played_From, dbo_Session.Played_To) = (dbo_Session.Played_To, dbo_Session.Played_From);
                dbo_Session.Playtime = dbo_prof.GameTime;

                DM_Session.Save(dbo_Session);
            }
        }
    }
}
