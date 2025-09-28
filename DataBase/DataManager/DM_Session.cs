using System;
using System.Collections.Generic;
using System.Data.SQLite;
using GameTimeX.DataBase.Objects;
using GameTimeX.Function.DataBase;

namespace GameTimeX.DataBase.DataManager
{
    internal class DM_Session
    {
        public static DBO_Session CreateNew(int fkPid)
        {
            return new DBO_Session
            {
                FK_PID = fkPid,
                Played_From = DateTime.MinValue,
                Played_To = DateTime.MinValue,
                Playtime = 0L
            };
        }

        public static void Delete(int sid)
        {
            using (var cmd = new SQLiteCommand("DELETE FROM tblGameSessions WHERE SID = @sid;", DataBaseConnector.connection))
            {
                cmd.Parameters.AddWithValue("@sid", sid);
                cmd.ExecuteNonQuery();
            }
        }

        public static DBO_Session ReadSID(int sid)
        {
            using (var cmd = DataBaseConnector.connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM tblGameSessions WHERE SID = @sid;";
                cmd.Parameters.AddWithValue("@sid", sid);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return Map(reader);
                }
            }
            return null;
        }

        public static List<DBO_Session> ReadAllByPid(int fkPid)
        {
            var list = new List<DBO_Session>();
            using (var cmd = DataBaseConnector.connection.CreateCommand())
            {
                cmd.CommandText = @"
                    SELECT * FROM tblGameSessions
                    WHERE FK_PID = @pid
                    ORDER BY Played_From DESC;";
                cmd.Parameters.AddWithValue("@pid", fkPid);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        list.Add(Map(reader));
                }
            }
            return list;
        }

        public static void Save(DBO_Session obj)
        {
            using (var cmd = BuildCommand(obj))
                cmd.ExecuteNonQuery();

            if (obj.SID == 0)
                obj.SID = GetLastInsertedSID();
        }

        private static SQLiteCommand BuildCommand(DBO_Session dbo)
            => ExistsInDatabase(dbo.SID) ? BuildUpdateCommand(dbo) : BuildInsertCommand(dbo);

        private static SQLiteCommand BuildInsertCommand(DBO_Session dbo)
        {
            var sql = @"
                INSERT INTO tblGameSessions
                (FK_PID, Played_From, Played_To, Playtime)
                VALUES (@FK_PID, @Played_From, @Played_To, @Playtime);";

            var cmd = new SQLiteCommand(sql, DataBaseConnector.connection);
            cmd.Parameters.AddWithValue("@FK_PID", dbo.FK_PID);
            cmd.Parameters.AddWithValue("@Played_From", DataBaseConnector.ToSQLDateFormat(dbo.Played_From));
            cmd.Parameters.AddWithValue("@Played_To", DataBaseConnector.ToSQLDateFormat(dbo.Played_To));
            cmd.Parameters.AddWithValue("@Playtime", dbo.Playtime);
            return cmd;
        }

        private static SQLiteCommand BuildUpdateCommand(DBO_Session dbo)
        {
            var sql = @"
                UPDATE tblGameSessions SET
                    FK_PID = @FK_PID,
                    Played_From = @Played_From,
                    Played_To = @Played_To,
                    Playtime = @Playtime
                WHERE SID = @SID;";

            var cmd = new SQLiteCommand(sql, DataBaseConnector.connection);
            cmd.Parameters.AddWithValue("@FK_PID", dbo.FK_PID);
            cmd.Parameters.AddWithValue("@Played_From", DataBaseConnector.ToSQLDateFormat(dbo.Played_From));
            cmd.Parameters.AddWithValue("@Played_To", DataBaseConnector.ToSQLDateFormat(dbo.Played_To));
            cmd.Parameters.AddWithValue("@Playtime", dbo.Playtime);
            cmd.Parameters.AddWithValue("@SID", dbo.SID);
            return cmd;
        }

        private static bool ExistsInDatabase(int sid) => sid != 0;

        private static int GetLastInsertedSID()
        {
            using (var cmd = new SQLiteCommand("SELECT last_insert_rowid();", DataBaseConnector.connection))
            {
                var lastInsertedId = cmd.ExecuteScalar();
                return Convert.ToInt32(lastInsertedId);
            }
        }

        private static DBO_Session Map(SQLiteDataReader reader)
        {
            var dbo = new DBO_Session
            {
                SID = reader.GetInt32(reader.GetOrdinal("SID")),
                FK_PID = reader.GetInt32(reader.GetOrdinal("FK_PID")),
                Played_From = DateTime.Parse(reader.GetString(reader.GetOrdinal("Played_From"))),
                Played_To = DateTime.Parse(reader.GetString(reader.GetOrdinal("Played_To"))),
                Playtime = Convert.ToInt64(reader.GetValue(reader.GetOrdinal("Playtime")))
            };
            return dbo;
        }
    }
}
