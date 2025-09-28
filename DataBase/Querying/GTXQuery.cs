using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Text;

namespace GameTimeX.DataBase.Querying
{
    /// <summary>
    /// Klasse zur objektorientierten Erstellung und Ausführung von SQL-Select-Queries für SQLite,
    /// inkl. Aggregatsfunktionen (COUNT/SUM/AVG/MIN/MAX), GROUP BY und HAVING.
    /// Beinhaltet statische Helper, um Werte per Spaltenname aus einem Reader zu holen.
    /// </summary>
    public sealed class GTXQuery : IDisposable
    {
        private readonly string table;
        private readonly SQLiteConnection connection;

        private readonly List<string> fields = new();            // SELECT-Liste (inkl. Aggregate-Ausdrücke)
        private readonly List<string> whereClauses = new();      // WHERE-Klauseln mit '?'
        private readonly List<object?> parameters = new();       // Parameter für WHERE + HAVING (Reihenfolge!)
        private readonly List<string> orderByFields = new();     // ORDER BY
        private readonly List<string> groupByFields = new();     // GROUP BY
        private readonly List<string> havingClauses = new();     // HAVING-Klauseln mit '?'

        private SQLiteCommand? stmt;
        private SQLiteDataReader? reader;
        private int? topX = null;

        private static readonly object execLock = new();

        public GTXQuery(string table, SQLiteConnection connection)
        {
            this.table = table ?? throw new ArgumentNullException(nameof(table));
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /* ---------------------------
         * SELECT-Felder
         * --------------------------- */

        public void AddField(string field)
        {
            if (string.IsNullOrWhiteSpace(field)) return;
            fields.Add(field);
        }

        public void AddField(string field, string? alias)
        {
            if (string.IsNullOrWhiteSpace(field)) return;
            fields.Add(string.IsNullOrWhiteSpace(alias) ? field : $"{field} AS {alias}");
        }

        public enum AggregateFunc { COUNT, SUM, AVG, MIN, MAX }

        public void AddAggregate(AggregateFunc func, string field, bool distinct = false, string? alias = null)
        {
            if (func == AggregateFunc.COUNT && field.Trim() == "*")
            {
                fields.Add(string.IsNullOrWhiteSpace(alias) ? "COUNT(*)" : $"COUNT(*) AS {alias}");
                return;
            }

            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field must not be empty.", nameof(field));

            string arg = distinct ? $"DISTINCT {field}" : field;
            string expr = func switch
            {
                AggregateFunc.COUNT => $"COUNT({arg})",
                AggregateFunc.SUM => $"SUM({arg})",
                AggregateFunc.AVG => $"AVG({arg})",
                AggregateFunc.MIN => $"MIN({arg})",
                AggregateFunc.MAX => $"MAX({arg})",
                _ => throw new ArgumentOutOfRangeException(nameof(func))
            };

            fields.Add(string.IsNullOrWhiteSpace(alias) ? expr : $"{expr} AS {alias}");
        }

        // Shortcuts
        public void AddCountAll(string? alias = null) => AddAggregate(AggregateFunc.COUNT, "*", false, alias);
        public void AddCount(string field, bool distinct = false, string? alias = null) => AddAggregate(AggregateFunc.COUNT, field, distinct, alias);

        // WICHTIG: COALESCE, damit SUM nie NULL liefert
        public void AddSum(string field, string? alias = null, bool coalesceZero = true)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field must not be empty.", nameof(field));

            string expr = coalesceZero ? $"COALESCE(SUM({field}), 0)" : $"SUM({field})";
            fields.Add(string.IsNullOrWhiteSpace(alias) ? expr : $"{expr} AS {alias}");
        }

        public void AddAvg(string field, string? alias = null) => AddAggregate(AggregateFunc.AVG, field, false, alias);
        public void AddMin(string field, string? alias = null) => AddAggregate(AggregateFunc.MIN, field, false, alias);
        public void AddMax(string field, string? alias = null) => AddAggregate(AggregateFunc.MAX, field, false, alias);

        /* ---------------------------
         * WHERE
         * --------------------------- */

        public void AddWhere(string field, QueryCompareType compareType, object? value)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field must not be empty.", nameof(field));

            if (compareType == QueryCompareType.LIKE && value is string s)
            {
                value = $"%{EscapeLikeValue(s)}%";
                whereClauses.Add($"{field} {compareType.GetSymbol()} ? ESCAPE '\\'");
            }
            else
            {
                whereClauses.Add($"{field} {compareType.GetSymbol()} ?");
            }

            parameters.Add(value);
        }

        /// <summary>Für Spezialfälle: rohe WHERE-Klausel mit Platzhaltern "?" und dazu passende Parameter.</summary>
        public void AddWhereRaw(string clauseWithPlaceholders, params object?[] args)
        {
            if (string.IsNullOrWhiteSpace(clauseWithPlaceholders)) return;
            whereClauses.Add(clauseWithPlaceholders);
            if (args != null && args.Length > 0)
                parameters.AddRange(args);
        }

        /// <summary>
        /// WHERE-Klausel: field BETWEEN from AND to (Standard: [from, to) ).
        /// </summary>
        public void AddWhereDateBetween(string field, DateTime from, DateTime to, bool includeEnd = false)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field must not be empty.", nameof(field));

            if (to < from)
                (from, to) = (to, from);

            if (includeEnd)
                whereClauses.Add($"{field} >= ? AND {field} <= ?");
            else
                whereClauses.Add($"{field} >= ? AND {field} < ?");

            parameters.Add(ToSqlDate(from));
            parameters.Add(ToSqlDate(to));
        }

        /// <summary>
        /// WHERE-Klausel: alle Werte, die exakt am Kalendertag liegen [day 00:00, next 00:00).
        /// </summary>
        public void AddWhereDateOnDay(string field, DateTime day)
        {
            var start = day.Date;
            var end = start.AddDays(1);
            AddWhereDateBetween(field, start, end, includeEnd: false);
        }

        /// <summary>
        /// WHERE-Klausel: alle Werte, deren Jahr = year ist.
        /// Nutzt strftime('%Y', field).
        /// </summary>
        public void AddWhereYearEquals(string field, int year)
        {
            if (string.IsNullOrWhiteSpace(field))
                throw new ArgumentException("Field must not be empty.", nameof(field));

            // SQLite gibt strftime('%Y') als 4-stelligen String zurück, deshalb casten wir die Zahl in String
            whereClauses.Add($"strftime('%Y', {field}) = ?");
            parameters.Add(year.ToString("0000", CultureInfo.InvariantCulture));
        }


        private static string ToSqlDate(DateTime dt)
            => dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

        /* ---------------------------
         * GROUP BY / HAVING
         * --------------------------- */

        public void AddGroupBy(params string[] fieldsToGroup)
        {
            if (fieldsToGroup == null) return;
            foreach (var f in fieldsToGroup)
                if (!string.IsNullOrWhiteSpace(f))
                    groupByFields.Add(f);
        }

        public void AddHaving(string fieldOrAggregate, QueryCompareType compareType, object? value)
        {
            if (string.IsNullOrWhiteSpace(fieldOrAggregate))
                throw new ArgumentException("Field must not be empty.", nameof(fieldOrAggregate));

            if (compareType == QueryCompareType.LIKE && value is string s)
            {
                value = $"%{EscapeLikeValue(s)}%";
                havingClauses.Add($"{fieldOrAggregate} {compareType.GetSymbol()} ? ESCAPE '\\'");
            }
            else
            {
                havingClauses.Add($"{fieldOrAggregate} {compareType.GetSymbol()} ?");
            }
            parameters.Add(value);
        }

        public void AddHavingRaw(string clauseWithPlaceholders, params object?[] args)
        {
            if (string.IsNullOrWhiteSpace(clauseWithPlaceholders)) return;
            havingClauses.Add(clauseWithPlaceholders);
            if (args != null && args.Length > 0)
                parameters.AddRange(args);
        }

        /* ---------------------------
         * ORDER BY / LIMIT
         * --------------------------- */

        public void AddOrderBy(string field) => AddOrderBy(field, null);

        public void AddOrderBy(string field, OrderDirection? direction)
        {
            if (string.IsNullOrWhiteSpace(field)) return;
            orderByFields.Add(direction is null ? field : $"{field} {direction.Value}");
        }

        public void SetTopX(int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            topX = limit;
        }

        public void ClearTopX() => topX = null;

        /* ---------------------------
         * SQL bauen / ausführen / preview
         * --------------------------- */

        public string GetSQL()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");

            if (fields.Count == 0)
                sb.Append('*');
            else
                sb.Append(string.Join(", ", fields));

            sb.Append(" FROM ").Append(table);

            if (whereClauses.Count > 0)
            {
                sb.Append(" WHERE ");
                sb.Append(string.Join(" AND ", whereClauses));
            }

            if (groupByFields.Count > 0)
            {
                sb.Append(" GROUP BY ");
                sb.Append(string.Join(", ", groupByFields));
            }

            if (havingClauses.Count > 0)
            {
                sb.Append(" HAVING ");
                sb.Append(string.Join(" AND ", havingClauses));
            }

            if (orderByFields.Count > 0)
            {
                sb.Append(" ORDER BY ");
                sb.Append(string.Join(", ", orderByFields));
            }

            if (topX.HasValue)
            {
                sb.Append(" LIMIT ").Append(topX.Value);
            }

            return sb.ToString();
        }

        public SQLiteDataReader Execute()
        {
            lock (execLock)
            {
                stmt = new SQLiteCommand(GetSQL(), connection);

                // Parameter in Reihenfolge hinzufügen
                for (int i = 0; i < parameters.Count; i++)
                {
                    object? param = parameters[i];
                    var p = new SQLiteParameter { Value = param ?? DBNull.Value };
                    stmt.Parameters.Add(p);
                }
            }

            reader = stmt.ExecuteReader();
            return reader;
        }

        public string PreviewQuery()
        {
            var sb = new StringBuilder();
            sb.Append("SELECT ");

            if (fields.Count == 0)
                sb.Append('*');
            else
                sb.Append(string.Join(", ", fields));

            sb.Append(" FROM ").Append(table);

            int paramIndex = 0;

            if (whereClauses.Count > 0)
            {
                sb.Append(" WHERE ");
                for (int i = 0; i < whereClauses.Count; i++)
                {
                    string clause = whereClauses[i];
                    string replaced = ReplaceFirstQuestionMark(clause, FormatParameter(parameters[paramIndex++]));
                    sb.Append(replaced);
                    if (i < whereClauses.Count - 1)
                        sb.Append(" AND ");
                }
            }

            if (groupByFields.Count > 0)
            {
                sb.Append(" GROUP BY ");
                sb.Append(string.Join(", ", groupByFields));
            }

            if (havingClauses.Count > 0)
            {
                sb.Append(" HAVING ");
                for (int i = 0; i < havingClauses.Count; i++)
                {
                    string clause = havingClauses[i];
                    string replaced = ReplaceFirstQuestionMark(clause, FormatParameter(parameters[paramIndex++]));
                    sb.Append(replaced);
                    if (i < havingClauses.Count - 1)
                        sb.Append(" AND ");
                }
            }

            if (orderByFields.Count > 0)
            {
                sb.Append(" ORDER BY ");
                sb.Append(string.Join(", ", orderByFields));
            }

            if (topX.HasValue)
            {
                sb.Append(" LIMIT ").Append(topX.Value);
            }

            return sb.ToString();
        }

        /* ---------------------------
         * Utils
         * --------------------------- */

        private static string ReplaceFirstQuestionMark(string input, string replacement)
        {
            int idx = input.IndexOf('?');
            if (idx < 0) return input;
            return input.Substring(0, idx) + replacement + input.Substring(idx + 1);
        }

        private static string EscapeLikeValue(string value)
        {
            return value
                .Replace(@"\", @"\\")
                .Replace("%", @"\%")
                .Replace("_", @"\_");
        }

        private static string FormatParameter(object? param)
        {
            if (param is null) return "NULL";

            return param switch
            {
                string s => $"'{s.Replace("'", "''")}'",
                bool b => b ? "1" : "0",
                DateTime dt => $"'{dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)}'",
                byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal
                    => Convert.ToString(param, CultureInfo.InvariantCulture) ?? "NULL",
                _ => $"'{param.ToString()!.Replace("'", "''")}'"
            };
        }

        public void Dispose()
        {
            try { if (reader != null && !reader.IsClosed) reader.Close(); } catch { }
            try { stmt?.Dispose(); } catch { }
        }

        /* ---------------------------
         * Static Reader Helpers
         * --------------------------- */

        public enum ColumnType
        {
            Int32,
            Int64,
            String,
            DateTime,
            Boolean,
            Double,
            Decimal
        }

        /// <summary>
        /// Enum-basierter Zugriff: GTXQuery.GetValue(reader, DBOI_Profile.GameName, ColumnType.String, defaultValue)
        /// </summary>
        public static object? GetValue(SQLiteDataReader reader, string column, ColumnType type, object? defaultValue = null)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (string.IsNullOrWhiteSpace(column)) throw new ArgumentException("column must not be empty", nameof(column));

            int idx = reader.GetOrdinal(column);
            if (idx < 0 || reader.IsDBNull(idx)) return defaultValue;

            try
            {
                return type switch
                {
                    ColumnType.Int32 => GetInt32(reader, column, (int?)defaultValue ?? default),
                    ColumnType.Int64 => GetInt64(reader, column, (long?)defaultValue ?? default),
                    ColumnType.String => reader.GetString(idx),
                    ColumnType.Boolean => reader.GetBoolean(idx),
                    ColumnType.Double => GetDouble(reader, column, (double?)defaultValue ?? default),
                    ColumnType.Decimal => GetDecimal(reader, column, (decimal?)defaultValue ?? default),
                    ColumnType.DateTime => ReadDateTime(reader, idx),
                    _ => reader.GetValue(idx)
                };
            }
            catch
            {
                var raw = reader.GetValue(idx);
                return raw ?? defaultValue;
            }
        }

        /// <summary>
        /// Generischer Zugriff: GTXQuery.Get&lt;long&gt;(reader, "TotalPlaytime")
        /// </summary>
        public static T? Get<T>(SQLiteDataReader reader, string column, T? defaultValue = default)
        {
            if (reader == null) throw new ArgumentNullException(nameof(reader));
            if (string.IsNullOrWhiteSpace(column)) throw new ArgumentException("column must not be empty", nameof(column));

            int idx = reader.GetOrdinal(column);
            if (idx < 0 || reader.IsDBNull(idx)) return defaultValue;

            object val = reader.GetValue(idx);

            try
            {
                if (typeof(T) == typeof(DateTime))
                {
                    object dt = ReadDateTimeObject(val);
                    return (T)dt;
                }

                return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        // ---- Robuste Komforthelfer (tolerant gegenüber DOUBLE/DECIMAL/STRING) ----

        public static int GetInt32(SQLiteDataReader r, string col, int def = default)
        {
            int idx = r.GetOrdinal(col);
            if (idx < 0 || r.IsDBNull(idx)) return def;
            object v = r.GetValue(idx);
            try
            {
                return v switch
                {
                    int i => i,
                    long l => unchecked((int)l),
                    double d => unchecked((int)d),
                    decimal m => unchecked((int)m),
                    string s => int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var i2) ? i2 : def,
                    _ => Convert.ToInt32(v, CultureInfo.InvariantCulture)
                };
            }
            catch { return def; }
        }

        public static long GetInt64(SQLiteDataReader r, string col, long def = default)
        {
            int idx = r.GetOrdinal(col);
            if (idx < 0 || r.IsDBNull(idx)) return def;
            object v = r.GetValue(idx);
            try
            {
                return v switch
                {
                    long l => l,
                    int i => i,
                    double d => checked((long)d),
                    decimal m => checked((long)m),
                    string s => long.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var l2) ? l2 : def,
                    _ => Convert.ToInt64(v, CultureInfo.InvariantCulture)
                };
            }
            catch { return def; }
        }

        public static double GetDouble(SQLiteDataReader r, string col, double def = default)
        {
            int idx = r.GetOrdinal(col);
            if (idx < 0 || r.IsDBNull(idx)) return def;
            object v = r.GetValue(idx);
            try
            {
                return v switch
                {
                    double d => d,
                    float f => f,
                    decimal m => (double)m,
                    long l => l,
                    int i => i,
                    string s => double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var d2) ? d2 : def,
                    _ => Convert.ToDouble(v, CultureInfo.InvariantCulture)
                };
            }
            catch { return def; }
        }

        public static decimal GetDecimal(SQLiteDataReader r, string col, decimal def = default)
        {
            int idx = r.GetOrdinal(col);
            if (idx < 0 || r.IsDBNull(idx)) return def;
            object v = r.GetValue(idx);
            try
            {
                return v switch
                {
                    decimal m => m,
                    double d => (decimal)d,
                    float f => (decimal)f,
                    long l => l,
                    int i => i,
                    string s => decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var m2) ? m2 : def,
                    _ => Convert.ToDecimal(v, CultureInfo.InvariantCulture)
                };
            }
            catch { return def; }
        }

        public static string GetString(SQLiteDataReader r, string col, string def = "")
            => Get<string>(r, col, def) ?? def;

        public static bool GetBool(SQLiteDataReader r, string col, bool def = default)
            => Get<bool?>(r, col, null) ?? def;

        public static DateTime GetDateTime(SQLiteDataReader r, string col, DateTime def = default)
        {
            int idx = r.GetOrdinal(col);
            if (idx < 0 || r.IsDBNull(idx)) return def;
            var parsed = ReadDateTime(r, idx);
            return parsed ?? def;
        }

        private static DateTime? ReadDateTime(SQLiteDataReader reader, int idx)
        {
            try { return reader.GetDateTime(idx); } catch { }

            try
            {
                var s = reader.GetString(idx);
                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var dt))
                    return dt;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
                    return dt;
            }
            catch { }

            return null;
        }

        private static object ReadDateTimeObject(object val)
        {
            if (val is DateTime dt) return dt;
            if (val is string s)
            {
                if (DateTime.TryParseExact(s, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var dt1)) return dt1;
                if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var dt2)) return dt2;
            }
            return default(DateTime);
        }
    }

    public enum OrderDirection { ASC, DESC }

    public enum QueryCompareType
    {
        EQUALS, NOT_EQUALS, GREATER_THAN, LESS_THAN, GREATER_OR_EQUAL, LESS_OR_EQUAL, LIKE
    }

    internal static class QueryCompareTypeExtensions
    {
        public static string GetSymbol(this QueryCompareType type) => type switch
        {
            QueryCompareType.EQUALS => "=",
            QueryCompareType.NOT_EQUALS => "<>",
            QueryCompareType.GREATER_THAN => ">",
            QueryCompareType.LESS_THAN => "<",
            QueryCompareType.GREATER_OR_EQUAL => ">=",
            QueryCompareType.LESS_OR_EQUAL => "<=",
            QueryCompareType.LIKE => "LIKE",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
