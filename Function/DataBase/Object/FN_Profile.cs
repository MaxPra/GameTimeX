using System;
using System.Collections.Generic;
using GameTimeX.DataBase.DataManager;
using GameTimeX.DataBase.ObjectInformation;
using GameTimeX.DataBase.Objects;
using GameTimeX.DataBase.Querying;
using GameTimeX.Function.AppEnvironment;
using GameTimeX.Function.Monitoring;
using GameTimeX.XApplication.SubDisplays;

namespace GameTimeX.Function.DataBase.Object
{
    /// <summary>
    /// Funktionsklasse für DBO_Profile
    /// </summary>
    public class FN_Profile
    {


        public static List<YearGameRow> GetYearStats()
        {

            List<YearGameRow> yearGameRows = new List<YearGameRow>();

            GTXQuery query = new GTXQuery(DBOI_Profile.TABLE, DataBaseConnector.connection);

            query.AddField(DBOI_Profile.ProfileID);
            query.AddField(DBOI_Profile.GameName);
            query.AddField(DBOI_Profile.ProfilePicFileName);

            query.AddWhereYearEquals(DBOI_Profile.LastPlay, DateTime.Now.Year);

            string sql = query.PreviewQuery();

            using (var reader = query.Execute())
            {
                while (reader.Read())
                {

                    int pid = GTXQuery.GetInt32(reader, DBOI_Profile.ProfileID);
                    string picName = GTXQuery.GetString(reader, DBOI_Profile.ProfilePicFileName);
                    string profileName = GTXQuery.GetString(reader, DBOI_Profile.GameName);

                    YearGameRow yearGameRow = GetYearStatsForSpecificProfile(pid);

                    yearGameRow.CoverUri = SysProps.picDestPath + SysProps.separator + picName;
                    yearGameRow.Title = profileName;

                    yearGameRows.Add(yearGameRow);
                }
            }

            return yearGameRows;
        }


        public static YearGameRow GetYearStatsForSpecificProfile(int pid)
        {
            YearGameRow yearGameRow = new YearGameRow();

            // --- Stunden letztes Jahr ---
            GTXQuery query = new GTXQuery(DBOI_Session.TABLE, DataBaseConnector.connection);

            query.AddSum(DBOI_Session.Playtime, "PlayTimeSumYear");

            query.AddWhereYearEquals(DBOI_Session.Played_From, DateTime.Now.Year);
            query.AddWhereYearEquals(DBOI_Session.Played_To, DateTime.Now.Year);
            query.AddWhere(DBOI_Session.FK_PID, QueryCompareType.EQUALS, pid);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                {
                    double playTimeLastYearMinutes = GTXQuery.GetDouble(reader, "PlayTimeSumYear");
                    yearGameRow.HoursLastYear = MonitorHandler.CalcGameTime(playTimeLastYearMinutes);
                }
            }

            // --- Stunden gesamt ---
            query = new GTXQuery(DBOI_Session.TABLE, DataBaseConnector.connection);

            query.AddSum(DBOI_Session.Playtime, "PlayTimeTotal");
            query.AddWhere(DBOI_Session.FK_PID, QueryCompareType.EQUALS, pid);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                {
                    double playTimeTotal = GTXQuery.GetDouble(reader, "PlayTimeTotal");
                    yearGameRow.HoursTotalOverall = MonitorHandler.CalcGameTime(playTimeTotal);
                }
            }

            return yearGameRow;
        }

        /// <summary>
        /// Liefert die gesamte Spielzeit für ein Profil
        /// </summary>
        /// <param name="pid">Profil-ID</param>
        /// <returns>Gesamte Spielzeit</returns>
        public static double GetTotalPlayTime(int pid)
        {

            double totalPlayTime = 0;

            // Query zusammenbauen
            GTXQuery query = new GTXQuery(DBO_Session.GetTableName(), DataBaseConnector.connection);

            query.AddSum(DBOI_Session.Playtime, "TotalPlaytime");

            query.AddWhere(DBOI_Session.FK_PID, QueryCompareType.EQUALS, pid);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                {
                    totalPlayTime = GTXQuery.GetDouble(reader, "TotalPlaytime");
                }
            }

            return totalPlayTime;
        }

        public static double GetTodaysPlayTime(int pid)
        {
            double todaysPlayTime = 0;

            // Query zusammenbauen
            GTXQuery query = new GTXQuery(DBOI_Session.TABLE, DataBaseConnector.connection);

            query.AddSum(DBOI_Session.Playtime, "TotalPlayTimeToday");

            query.AddWhere(DBOI_Session.FK_PID, QueryCompareType.EQUALS, pid);

            query.AddWhereDateOnDay(DBOI_Session.Played_To, DateTime.Now);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                {
                    todaysPlayTime = GTXQuery.GetDouble(reader, "TotalPlayTimeToday");
                }
            }

            return todaysPlayTime;
        }

        /// <summary>
        /// Liefert die Playthrough Zeit für den aktuellen Playthrough laut Profiles StartPointDate
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static double GetCurrentPlaythroughTime(int pid, DateTime playthroughStartPointDate)
        {
            double currPlayThorughTime = 0;

            // Query zusammenbauen
            GTXQuery query = new GTXQuery(DBOI_Session.TABLE, DataBaseConnector.connection);

            query.AddSum(DBOI_Session.Playtime, "CurrPlayThroughTime");

            query.AddWhere(DBOI_Session.FK_PID, QueryCompareType.EQUALS, pid);

            query.AddWhereDateBetween(DBOI_Session.Played_From, playthroughStartPointDate, DateTime.Now, true);

            using (var reader = query.Execute())
            {
                if (reader.Read())
                {
                    currPlayThorughTime = GTXQuery.GetDouble(reader, "CurrPlayThroughTime");
                }
            }

            return currPlayThorughTime;
        }

        public static void SaveMonitoredTime(double minutes, int pid, DateTime startTimeMonitoring, DateTime endTimeMonitoring)
        {
            var dboProfile = DM_Profile.ReadPID(pid);
            if (dboProfile != null)
            {
                DBO_Session dboSession = DM_Session.CreateNew(dboProfile.ProfileID);

                dboSession.Playtime = minutes;
                dboSession.Played_From = startTimeMonitoring;
                dboSession.Played_To = endTimeMonitoring;

                DM_Session.Save(dboSession);
            }
        }

        public static void SaveFirstTimePlayed(int pid)
        {
            var obj = DM_Profile.ReadPID(pid);
            if (obj == null || obj.FirstPlay != DateTime.MinValue) return;

            obj.FirstPlay = DateTime.Now;
            DM_Profile.Save(obj);
        }

        public static void SaveLastTimePlayed(int pid)
        {
            var obj = DM_Profile.ReadPID(pid);
            if (obj != null)
            {
                obj.LastPlay = DateTime.Now;
                DM_Profile.Save(obj);
            }
        }
    }
}
