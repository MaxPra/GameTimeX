using System;
using System.Collections.Generic;
using System.Data.SQLite;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function.DataBase;

namespace GameTimeX.DataBase.DataManager
{
    internal class DM_Profile
    {
        public static DBO_Profile CreateNew()
        {
            return new DBO_Profile
            {
                GameName = "",
                GameTime = 0,
                FirstPlay = DateTime.MinValue,
                LastPlay = DateTime.MinValue,
                ProfilePicFileName = "",
                ExtGameFolder = "",
                CreatedAt = DateTime.Now,
                ChangedAt = DateTime.MinValue,
                SteamAppID = 0,
                ProfileSettings = "",
                TodayStats = "",
                Executables = "",
                PlaythroughStartPointDate = DateTime.MinValue
            };
        }

        /// <summary>
        /// Löscht ein Profil UND alle zugehörigen Sessions in einer Transaktion.
        /// </summary>
        public static void Delete(int pid)
        {
            using (var tx = DataBaseConnector.connection.BeginTransaction())
            {
                using (var delSessions = new SQLiteCommand("DELETE FROM tblGameSessions WHERE FK_PID = @pid;", DataBaseConnector.connection, tx))
                {
                    delSessions.Parameters.AddWithValue("@pid", pid);
                    delSessions.ExecuteNonQuery();
                }

                using (var delProfile = new SQLiteCommand("DELETE FROM tblGameProfiles WHERE ProfileID = @pid;", DataBaseConnector.connection, tx))
                {
                    delProfile.Parameters.AddWithValue("@pid", pid);
                    delProfile.ExecuteNonQuery();
                }

                tx.Commit();
            }
        }

        public static bool IsPlayTimeGreaterZero(int pid)
        {
            var obj = ReadPID(pid);
            return obj == null ? true : obj.GameTime > 0;
        }

        public static List<DBO_Profile> ReadAll()
        {
            var list = new List<DBO_Profile>();
            using (var cmd = DataBaseConnector.connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM tblGameProfiles ORDER BY LastPlay DESC;";
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(Map(reader));
                }
            }
            return list;
        }

        public static List<DBO_Profile> ReadGameName(string gameName)
        {
            var list = new List<DBO_Profile>();
            using (var cmd = DataBaseConnector.connection.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM tblGameProfiles " +
                    "WHERE GameName LIKE @q ESCAPE '\\' " +
                    "ORDER BY LastPlay DESC;";
                cmd.Parameters.AddWithValue("@q", "%" + EscapeLikeValue(gameName ?? string.Empty) + "%");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(Map(reader));
                }
            }
            return list;
        }

        public static DBO_Profile ReadPID(int pid)
        {
            using (var cmd = DataBaseConnector.connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM tblGameProfiles WHERE ProfileID = @pid;";
                cmd.Parameters.AddWithValue("@pid", pid);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return Map(reader);
                }
            }
            return null;
        }

        public static void Save(DBO_Profile obj)
        {
            using (var cmd = BuildCommand(obj))
                cmd.ExecuteNonQuery();

            if (obj.ProfileID == 0)
                obj.ProfileID = getLastInsertedPID();
        }

        // --- unverändert gelassen: deine drei Helfer ---

        public static void SaveFirstTimePlayed(int pid)
        {
            var obj = ReadPID(pid);
            if (obj == null || obj.FirstPlay != DateTime.MinValue) return;

            obj.FirstPlay = DateTime.Now;
            Save(obj);
        }

        public static void SaveLastTimePlayed(int pid)
        {
            var obj = ReadPID(pid);
            if (obj != null)
            {
                obj.LastPlay = DateTime.Now;
                Save(obj);
            }
        }

        public static void SaveMonitoredTime(long minutes, int pid)
        {
            var obj = ReadPID(pid);
            if (obj != null)
            {
                obj.GameTime += minutes;
                Save(obj);
            }
        }

        // --- intern ---

        private static SQLiteCommand BuildCommand(DBO_Profile dbo_Profile)
            => ExistsInDatabse(dbo_Profile.ProfileID) ? BuildUpdateCommand(dbo_Profile) : BuildInsertCommand(dbo_Profile);

        private static SQLiteCommand BuildInsertCommand(DBO_Profile dbo_Profile)
        {
            var sql =
                "INSERT INTO tblGameProfiles " +
                "(GameName, GameTime, FirstPlay, LastPlay, ProfilePicFileName, ExtGameFolder, CreatedAt, ChangedAt, SteamAppID, ProfileSettings, TodayStats, Executables, PlaythroughStartPointDate) " +
                "VALUES (@GameName, @GameTime, @FirstPlay, @LastPlay, @ProfilePicFileName, @ExtGameFolder, @CreatedAt, @ChangedAt, @SteamAppID, @ProfileSettings, @TodayStats, @Executables, @PlaythroughStartPointDate);";

            var cmd = new SQLiteCommand(sql, DataBaseConnector.connection);
            cmd.Parameters.AddWithValue("@GameName", dbo_Profile.GameName ?? string.Empty);
            cmd.Parameters.AddWithValue("@GameTime", dbo_Profile.GameTime);
            cmd.Parameters.AddWithValue("@FirstPlay", DataBaseConnector.ToSQLDateFormat(dbo_Profile.FirstPlay));
            cmd.Parameters.AddWithValue("@LastPlay", DataBaseConnector.ToSQLDateFormat(dbo_Profile.LastPlay));
            cmd.Parameters.AddWithValue("@ProfilePicFileName", dbo_Profile.ProfilePicFileName ?? string.Empty);
            cmd.Parameters.AddWithValue("@ExtGameFolder", dbo_Profile.ExtGameFolder ?? string.Empty);
            cmd.Parameters.AddWithValue("@CreatedAt", DataBaseConnector.ToSQLDateFormat(DateTime.Now));
            cmd.Parameters.AddWithValue("@ChangedAt", DataBaseConnector.ToSQLDateFormat(DateTime.Now));
            cmd.Parameters.AddWithValue("@SteamAppID", dbo_Profile.SteamAppID);
            cmd.Parameters.AddWithValue("@ProfileSettings", dbo_Profile.ProfileSettings ?? string.Empty);
            cmd.Parameters.AddWithValue("@TodayStats", dbo_Profile.TodayStats ?? string.Empty);
            cmd.Parameters.AddWithValue("@Executables", dbo_Profile.Executables ?? string.Empty);
            cmd.Parameters.AddWithValue("@PlaythroughStartPointDate", DataBaseConnector.ToSQLDateFormat(dbo_Profile.PlaythroughStartPointDate));
            return cmd;
        }

        private static SQLiteCommand BuildUpdateCommand(DBO_Profile dbo_Profile)
        {
            var sql =
                "UPDATE tblGameProfiles SET " +
                "GameName = @GameName, " +
                "GameTime = @GameTime, " +
                "FirstPlay = @FirstPlay, " +
                "LastPlay = @LastPlay, " +
                "ProfilePicFileName = @ProfilePicFileName, " +
                "ExtGameFolder = @ExtGameFolder, " +
                "ChangedAt = @ChangedAt, " +
                "SteamAppID = @SteamAppID, " +
                "ProfileSettings = @ProfileSettings, " +
                "TodayStats = @TodayStats, " +
                "Executables = @Executables, " +
                "PlaythroughStartPointDate = @PlaythroughStartPointDate " +
                "WHERE ProfileID = @ProfileID;";

            var cmd = new SQLiteCommand(sql, DataBaseConnector.connection);
            cmd.Parameters.AddWithValue("@GameName", dbo_Profile.GameName ?? string.Empty);
            cmd.Parameters.AddWithValue("@GameTime", dbo_Profile.GameTime);
            cmd.Parameters.AddWithValue("@FirstPlay", DataBaseConnector.ToSQLDateFormat(dbo_Profile.FirstPlay));
            cmd.Parameters.AddWithValue("@LastPlay", DataBaseConnector.ToSQLDateFormat(dbo_Profile.LastPlay));
            cmd.Parameters.AddWithValue("@ProfilePicFileName", dbo_Profile.ProfilePicFileName ?? string.Empty);
            cmd.Parameters.AddWithValue("@ExtGameFolder", dbo_Profile.ExtGameFolder ?? string.Empty);
            cmd.Parameters.AddWithValue("@ChangedAt", DataBaseConnector.ToSQLDateFormat(DateTime.Now));
            cmd.Parameters.AddWithValue("@SteamAppID", dbo_Profile.SteamAppID);
            cmd.Parameters.AddWithValue("@ProfileSettings", dbo_Profile.ProfileSettings ?? string.Empty);
            cmd.Parameters.AddWithValue("@TodayStats", dbo_Profile.TodayStats ?? string.Empty);
            cmd.Parameters.AddWithValue("@Executables", dbo_Profile.Executables ?? string.Empty);
            cmd.Parameters.AddWithValue("@PlaythroughStartPointDate", DataBaseConnector.ToSQLDateFormat(dbo_Profile.PlaythroughStartPointDate));
            cmd.Parameters.AddWithValue("@ProfileID", dbo_Profile.ProfileID);
            return cmd;
        }

        private static string EscapeLikeValue(string value)
        {
            return value
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_");
        }

        private static bool ExistsInDatabse(int profileID) => profileID != 0;

        private static int getLastInsertedPID()
        {
            using (var cmd = new SQLiteCommand("SELECT last_insert_rowid();", DataBaseConnector.connection))
            {
                var lastInsertedId = cmd.ExecuteScalar();
                return Convert.ToInt32(lastInsertedId);
            }
        }

        private static DBO_Profile Map(SQLiteDataReader reader)
        {
            var p = new DBO_Profile();

            int o;

            o = reader.GetOrdinal("ProfileID"); if (o >= 0 && !reader.IsDBNull(o)) p.ProfileID = reader.GetInt32(o);
            o = reader.GetOrdinal("GameName"); if (o >= 0 && !reader.IsDBNull(o)) p.GameName = reader.GetString(o);
            o = reader.GetOrdinal("GameTime"); if (o >= 0 && !reader.IsDBNull(o)) p.GameTime = reader.GetInt64(o);

            o = reader.GetOrdinal("FirstPlay");
            if (o >= 0 && !reader.IsDBNull(o))
                p.FirstPlay = DateTime.TryParse(reader.GetString(o), out var dtFP) ? dtFP : DateTime.MinValue;

            o = reader.GetOrdinal("LastPlay");
            if (o >= 0 && !reader.IsDBNull(o))
                p.LastPlay = DateTime.TryParse(reader.GetString(o), out var dtLP) ? dtLP : DateTime.MinValue;

            o = reader.GetOrdinal("ProfilePicFileName"); if (o >= 0 && !reader.IsDBNull(o)) p.ProfilePicFileName = reader.GetString(o);
            o = reader.GetOrdinal("ExtGameFolder"); if (o >= 0 && !reader.IsDBNull(o)) p.ExtGameFolder = reader.GetString(o);

            o = reader.GetOrdinal("CreatedAt");
            if (o >= 0 && !reader.IsDBNull(o))
                p.CreatedAt = DateTime.TryParse(reader.GetString(o), out var dtC) ? dtC : DateTime.MinValue;

            o = reader.GetOrdinal("ChangedAt");
            if (o >= 0 && !reader.IsDBNull(o))
                p.ChangedAt = DateTime.TryParse(reader.GetString(o), out var dtCh) ? dtCh : DateTime.MinValue;

            o = reader.GetOrdinal("SteamAppID"); if (o >= 0 && !reader.IsDBNull(o)) p.SteamAppID = Convert.ToInt32(reader.GetValue(o));
            o = reader.GetOrdinal("ProfileSettings"); if (o >= 0 && !reader.IsDBNull(o)) p.ProfileSettings = reader.GetString(o);
            o = reader.GetOrdinal("TodayStats"); if (o >= 0 && !reader.IsDBNull(o)) p.TodayStats = reader.GetString(o);
            o = reader.GetOrdinal("Executables"); if (o >= 0 && !reader.IsDBNull(o)) p.Executables = reader.GetString(o);

            o = reader.GetOrdinal("PlaythroughStartPointDate");
            if (o >= 0 && !reader.IsDBNull(o))
                p.PlaythroughStartPointDate = DateTime.TryParse(reader.GetString(o), out var dtPSPD) ? dtPSPD : DateTime.MinValue;

            return p;
        }
    }
}
