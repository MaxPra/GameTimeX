using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace GameTimeX
{
    internal class DataBaseHandler
    {
        private static SQLiteConnection connection = null;

        public static bool CreateDB()
        {
            if (System.IO.File.Exists(SysProps.dbFilePath))
                return false;

            System.IO.File.Create(SysProps.dbFilePath);
            return true;
        }

        public static void MigrateDB()
        {
            // ExtGameFolder
            using (var cmd = new SQLiteCommand("SELECT 1 FROM PRAGMA_table_info('tblGameProfiles') WHERE name = 'ExtGameFolder';", connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    AlterTableExtGameFolder();
            }

            // StartpointPlaythroughTime
            using (var cmd = new SQLiteCommand("SELECT 1 FROM PRAGMA_table_info('tblGameProfiles') WHERE name = 'StartpointPlaythroughTime';", connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                    AlterTableStartpointPlaythroughTime();
            }
        }

        private static void AlterTableExtGameFolder()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN ExtGameFolder varchar(1000);", connection))
                cmd.ExecuteNonQuery();

            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET ExtGameFolder = '';", connection))
                cmd.ExecuteNonQuery();
        }

        private static void AlterTableStartpointPlaythroughTime()
        {
            using (var cmd = new SQLiteCommand("ALTER TABLE tblGameProfiles ADD COLUMN StartpointPlaythroughTime INTEGER;", connection))
                cmd.ExecuteNonQuery();

            using (var cmd = new SQLiteCommand("UPDATE tblGameProfiles SET StartpointPlaythroughTime = 0;", connection))
                cmd.ExecuteNonQuery();
        }

        /// <summary>Erstellt – wenn nötig – die SQLite-Datenbanktabelle.</summary>
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
                "StartpointPlaythroughTime INTEGER, " +
                "CreatedAt DATETIME, " +
                "ChangedAt DATETIME)", connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public static bool ConnectToSQLite()
        {
            string connectionString;
            bool newDB;

            if (!System.IO.File.Exists(SysProps.dbFilePath))
            {
                connectionString = $"Data Source={SysProps.dbFilePath};Version=3;New=True;Compress=True;";
                newDB = true;
            }
            else
            {
                connectionString = $"Data Source={SysProps.dbFilePath};Version=3;Compress=True;";
                newDB = false;
            }

            connection = new SQLiteConnection(connectionString);
            try { connection.Open(); } catch { /* ignore */ }
            return newDB;
        }

        // ---------- Parameterisierte Commands ----------

        private static SQLiteCommand BuildUpdateCommand(DBObject dbObj)
        {
            var sql =
                "UPDATE tblGameProfiles SET " +
                "GameName = @GameName, " +
                "GameTime = @GameTime, " +
                "FirstPlay = @FirstPlay, " +
                "LastPlay = @LastPlay, " +
                "ProfilePicFileName = @ProfilePicFileName, " +
                "ExtGameFolder = @ExtGameFolder, " +
                "StartpointPlaythroughTime = @Startpoint, " +
                "ChangedAt = @ChangedAt " +
                "WHERE ProfileID = @ProfileID;";

            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@GameName", dbObj.GameName ?? string.Empty);
            cmd.Parameters.AddWithValue("@GameTime", dbObj.GameTime);
            cmd.Parameters.AddWithValue("@FirstPlay", ToSQLDateFormat(dbObj.FirstPlay));
            cmd.Parameters.AddWithValue("@LastPlay", ToSQLDateFormat(dbObj.LastPlay));
            cmd.Parameters.AddWithValue("@ProfilePicFileName", dbObj.ProfilePicFileName ?? string.Empty);
            cmd.Parameters.AddWithValue("@ExtGameFolder", dbObj.ExtGameFolder ?? string.Empty);
            cmd.Parameters.AddWithValue("@Startpoint", dbObj.PlayThroughStartingPoint);
            cmd.Parameters.AddWithValue("@ChangedAt", ToSQLDateFormat(DateTime.Now));
            cmd.Parameters.AddWithValue("@ProfileID", dbObj.ProfileID);
            return cmd;
        }

        private static SQLiteCommand BuildInsertCommand(DBObject dbObj)
        {
            var sql =
                "INSERT INTO tblGameProfiles " +
                "(GameName, GameTime, FirstPlay, LastPlay, ProfilePicFileName, ExtGameFolder, StartpointPlaythroughTime, CreatedAt, ChangedAt) " +
                "VALUES (@GameName, @GameTime, @FirstPlay, @LastPlay, @ProfilePicFileName, @ExtGameFolder, @Startpoint, @CreatedAt, @ChangedAt);";

            var cmd = new SQLiteCommand(sql, connection);
            cmd.Parameters.AddWithValue("@GameName", dbObj.GameName ?? string.Empty);
            cmd.Parameters.AddWithValue("@GameTime", dbObj.GameTime);
            cmd.Parameters.AddWithValue("@FirstPlay", ToSQLDateFormat(dbObj.FirstPlay));
            cmd.Parameters.AddWithValue("@LastPlay", ToSQLDateFormat(dbObj.LastPlay));
            cmd.Parameters.AddWithValue("@ProfilePicFileName", dbObj.ProfilePicFileName ?? string.Empty);
            cmd.Parameters.AddWithValue("@ExtGameFolder", dbObj.ExtGameFolder ?? string.Empty);
            cmd.Parameters.AddWithValue("@Startpoint", dbObj.PlayThroughStartingPoint);
            cmd.Parameters.AddWithValue("@CreatedAt", ToSQLDateFormat(DateTime.Now));
            cmd.Parameters.AddWithValue("@ChangedAt", ToSQLDateFormat(DateTime.Now));
            return cmd;
        }

        private static SQLiteCommand BuildCommand(DBObject dbObj)
            => ExistsInDatabse(dbObj.ProfileID) ? BuildUpdateCommand(dbObj) : BuildInsertCommand(dbObj);

        private static bool ExistsInDatabse(int profileID) => profileID != 0;

        public static void Save(DBObject obj)
        {
            using (var cmd = BuildCommand(obj))
                cmd.ExecuteNonQuery();

            // Nur nach Insert überschreiben
            if (obj.ProfileID == 0)
                obj.ProfileID = getLastInsertedPID();
        }

        private static int getLastInsertedPID()
        {
            using (var cmd = new SQLiteCommand("SELECT last_insert_rowid();", connection))
            {
                var lastInsertedId = cmd.ExecuteScalar();
                return Convert.ToInt32(lastInsertedId);
            }
        }

        public static void Delete(int pid)
        {
            using (var cmd = new SQLiteCommand("DELETE FROM tblGameProfiles WHERE ProfileID = @pid;", connection))
            {
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>Alle Profile, deren Name den Suchtext enthält (SQL-Injection-sicher, LIKE mit Escape).</summary>
        public static List<DBObject> ReadGameName(string gameName)
        {
            var list = new List<DBObject>();

            using (var cmd = connection.CreateCommand())
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

        public static List<DBObject> ReadAll()
        {
            var list = new List<DBObject>();
            using (var cmd = connection.CreateCommand())
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

        /// <summary>Liest das Objekt mit der übergebenen PID ein.</summary>
        public static DBObject ReadPID(int pid)
        {
            using (var cmd = connection.CreateCommand())
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

        private static DBObject Map(SQLiteDataReader reader)
        {
            var dbObj = new DBObject
            {
                ProfileID = reader.GetInt32(0),
                GameName = reader.GetString(1),
                GameTime = reader.GetInt64(2),
                FirstPlay = DateTime.Parse(reader.GetString(3)),
                LastPlay = DateTime.Parse(reader.GetString(4)),
                ProfilePicFileName = reader.GetString(5),
                ExtGameFolder = reader.GetString(6),
                PlayThroughStartingPoint = reader.GetInt32(7),
                CreatedAt = DateTime.Parse(reader.GetString(8)),
                ChangedAt = DateTime.Parse(reader.GetString(9))
            };
            return dbObj;
        }

        public static DBObject CreateNew()
        {
            return new DBObject
            {
                GameName = "",
                GameTime = 0,
                FirstPlay = DateTime.MinValue,
                LastPlay = DateTime.MinValue,
                ProfilePicFileName = "",
                ExtGameFolder = "",
                PlayThroughStartingPoint = 0,
                CreatedAt = DateTime.Now,
                ChangedAt = DateTime.MinValue
            };
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

        public static bool IsPlayTimeGreaterZero(int pid)
        {
            var obj = ReadPID(pid);
            return obj == null ? true : obj.GameTime > 0;
        }

        private static string ToSQLDateFormat(DateTime date)
            => date.ToString("yyyy-MM-dd HH:mm:ss");

        // Escaping für LIKE (%, _ und \)
        private static string EscapeLikeValue(string value)
        {
            return value
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_");
        }
    }
}
