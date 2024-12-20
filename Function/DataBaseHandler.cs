using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Threading;

namespace GameTimeX
{
    internal class DataBaseHandler
    {


        private static SQLiteConnection connection = null;


        public static bool createDB()
        {
            if (System.IO.File.Exists(SysProps.dbFilePath))
            {
                return false;
            }

            System.IO.File.Create(SysProps.dbFilePath);

            Thread.Sleep(000);

            return true;
        }

        /// <summary>
        /// Erstellt, wenn nötig die SQLite-Datenbank
        /// </summary>
        public static void createTable()
        {
            if(connection == null)
            {
                return;
            }

            string sql = "CREATE TABLE tblGameProfiles (ProfileID INTEGER PRIMARY KEY, GameName VARCHAR(200), GameTime BIGINT, FirstPlay DATETIME, LastPlay DATETIME, ProfilePicFileName varchar(10000), CreatedAt DATETIME, ChangedAt DATETIME)";
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        public static bool connectToSQLite()
        {

            string connectionString = "";
            bool newDB = false;

            if (!System.IO.File.Exists(SysProps.dbFilePath))
            {
                connectionString = "Data Source=" + SysProps.dbFilePath + ";Version=3;New=True;Compress=True;";
                newDB = true;
            }
            else
            {
                connectionString = "Data Source=" + SysProps.dbFilePath + ";Version=3;Compress=True;";
                newDB = false;
            }
           
            connection = new SQLiteConnection(connectionString);

            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                // Nichts machen
            }

            return newDB;
        }

        private static String buildSQLUpdate(DBObject dbObj)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE tblGameProfiles set ");
            sb.Append("GameName = '" + dbObj.GameName + "', ");
            sb.Append("GameTime = " + dbObj.GameTime + ", ");
            sb.Append("FirstPlay = '" + dbObj.FirstPlay + "', ");
            sb.Append("LastPlay = '" + dbObj.LastPlay + "', ");
            sb.Append("ProfilePicFileName = '" + dbObj.ProfilePicFileName + "', ");
            sb.Append("ChangedAt = '" + DateTime.Now + "' ");
            sb.Append("where ProfileID = " + dbObj.ProfileID);

            return sb.ToString();
        }

        private static String buildSQLCreateNew(DBObject dbObj)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO tblGameProfiles (ProfileID, GameName, GameTime, FirstPlay, LastPlay, ProfilePicFileName, CreatedAt, ChangedAt) values (");
            sb.Append("NULL");
            sb.Append(", ");
            sb.Append(SysProps.apos + dbObj.GameName + SysProps.apos);
            sb.Append(", ");
            sb.Append(SysProps.apos + dbObj.GameTime + SysProps.apos);
            sb.Append(", ");
            sb.Append("'" + dbObj.FirstPlay + "'");
            sb.Append(", ");
            sb.Append("'" + dbObj.LastPlay + "'");
            sb.Append(", ");
            sb.Append(SysProps.apos + dbObj.ProfilePicFileName + SysProps.apos);
            sb.Append(", ");
            sb.Append("'" + DateTime.Now + "'");
            sb.Append(", ");
            sb.Append("'" + DateTime.Now + "')");


            return sb.ToString();
        }

        private static String buildSQL(DBObject dbObj)
        {
            if (existsInDatabse(dbObj.ProfileID))
            {
                return buildSQLUpdate(dbObj);
            }
            else
            {
                return buildSQLCreateNew(dbObj);
            }
        }

        private static bool existsInDatabse(int profileID)
        {
            if(profileID == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void save(DBObject obj)
        {
            string sql = buildSQL(obj);
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        public static void delete(int pid)
        {
            DBObject obj = DataBaseHandler.readPID(pid);

            string sql = "DELETE from tblGameProfiles where ProfileID = " + pid;
            SQLiteCommand cmd = new SQLiteCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Liest das oder mehrere GameObjekte ein, wo der übergebene String im GameName vorkommt
        /// </summary>
        /// <param name="gameName"></param>
        /// <returns></returns>
        public static List<DBObject> readGameName(string gameName)
        {
            SQLiteDataReader reader = null;
            List<DBObject> list = new List<DBObject>();

            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * from tblGameProfiles where GameName like '%" + gameName + "%' order by LastPlay desc;";

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DBObject dbObj = new DBObject();
                dbObj.ProfileID = reader.GetInt32(0);
                dbObj.GameName = reader.GetString(1);
                dbObj.GameTime = reader.GetInt64(2);
                dbObj.FirstPlay = DateTime.Parse(reader.GetString(3));
                dbObj.LastPlay = DateTime.Parse(reader.GetString(4));
                dbObj.ProfilePicFileName = reader.GetString(5);
                dbObj.CreatedAt = DateTime.Parse(reader.GetString(6));
                dbObj.ChangedAt = DateTime.Parse(reader.GetString(7));

                list.Add(dbObj);
            }

            return list;
        }

        public static List<DBObject> readAll()
        {
            SQLiteDataReader reader = null;
            List<DBObject> list = new List<DBObject>();

            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * from tblGameProfiles order by LastPlay desc;";

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                DBObject dbObj = new DBObject();
                dbObj.ProfileID = reader.GetInt32(0);
                dbObj.GameName = reader.GetString(1);
                dbObj.GameTime = reader.GetInt64(2);
                dbObj.FirstPlay = DateTime.Parse(reader.GetString(3));
                dbObj.LastPlay = DateTime.Parse(reader.GetString(4));
                dbObj.ProfilePicFileName = reader.GetString(5);
                dbObj.CreatedAt = DateTime.Parse(reader.GetString(6));
                dbObj.ChangedAt = DateTime.Parse(reader.GetString(7));

                list.Add(dbObj);
            }

            return list;
        }

        /// <summary>
        /// Liest zur übergebenen PID das Datenbankobjekt ein
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static DBObject readPID(int pid)
        {
            SQLiteDataReader reader = null;
            DBObject dbObj = null;

            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * from tblGameProfiles where ProfileID = " + pid + ";";

            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                dbObj = new DBObject();
                dbObj.ProfileID = reader.GetInt32(0);
                dbObj.GameName = reader.GetString(1);
                dbObj.GameTime = reader.GetInt64(2);
                dbObj.FirstPlay = DateTime.Parse(reader.GetString(3));
                dbObj.LastPlay = DateTime.Parse(reader.GetString(4));
                dbObj.ProfilePicFileName = reader.GetString(5);
                dbObj.CreatedAt = DateTime.Parse(reader.GetString(6));
                dbObj.ChangedAt = DateTime.Parse(reader.GetString(7));
            }

            return dbObj;
        }

        public static DBObject createNew()
        {
            DBObject dbObj = new DBObject();
            dbObj.GameName = "";
            dbObj.GameTime = 0;
            dbObj.FirstPlay = DateTime.MinValue;
            dbObj.LastPlay = DateTime.MinValue;
            dbObj.ProfilePicFileName = "";
            dbObj.CreatedAt = DateTime.Now;
            dbObj.ChangedAt = DateTime.MinValue;

            return dbObj;
        }

        public static void saveMonitoredTime(long minutes, int pid)
        {
            DBObject obj = DataBaseHandler.readPID(pid);

            if(obj != null)
            {
                obj.GameTime += minutes;
                DataBaseHandler.save(obj);
            }
        }

        public static void saveFirstTimePlayed(int pid)
        {
            DBObject obj = DataBaseHandler.readPID(pid);

            if(obj == null || obj.FirstPlay != DateTime.MinValue)
            {
                return;
            }

            obj.FirstPlay = DateTime.Now;
            DataBaseHandler.save(obj);
        }

        public static void saveLastTimePlayed(int pid)
        {
            DBObject obj = DataBaseHandler.readPID(pid);

            if(obj != null)
            {
                obj.LastPlay = DateTime.Now;
                DataBaseHandler.save(obj);
            }
        }




    }
}
