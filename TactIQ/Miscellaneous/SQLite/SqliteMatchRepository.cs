using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Miscellaneous.SQLite
{
    /// <summary>
    /// Klasse für den Zugriff auf die Match-Daten.
    /// </summary>
    public class SqliteMatchRepository: IMatchRepository
    {
        /// <summary>
        /// Ruft alle Matches für einen bestimmten Gegner ab.
        /// </summary>
        /// <param name="opponentId">Id des Gegners</param>
        /// <returns>Liste von Matches für Gegner-Id</returns>
        public IEnumerable<Match> GetAllForOpponent(int opponentId)
        {
            var list = new List<Match>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, MatchDate, Result, Competition, IsWin, Notes, Marked FROM Match Where OpponentId = @id", conn);
            cmd.Parameters.AddWithValue("@id", opponentId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Match
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Date = ParseDateSafe(r, 2),
                    Result = r.IsDBNull(3) ? "" : r.GetString(3),
                    Competition = r.IsDBNull(4) ? "" : r.GetString(4),
                    IsWin = r.GetBoolean(5),
                    Notes = r.IsDBNull(6) ? "" : r.GetString(6),
                    Marked = r.HasRows && r.GetBoolean(7)
                });
            }
            return list;
        }

        /// <summary>
        /// Ruft alle Matches ab.
        /// </summary>
        /// <returns>Liste aller Matches</returns>
        public IEnumerable<Match> GetAllMatches()
        {
            var list = new List<Match>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, MatchDate, Result, Competition, IsWin, Notes, Marked FROM Match", conn);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Match
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Date = ParseDateSafe(r, 2),
                    Result = r.IsDBNull(3) ? "" : r.GetString(3),
                    Competition = r.IsDBNull(4) ? "" : r.GetString(4),
                    IsWin = r.GetBoolean(5),
                    Notes = r.IsDBNull(6) ? "" : r.GetString(6),
                    Marked = r.HasRows && r.GetBoolean(7)
                });
            }
            return list;
        }

        /// <summary>
        /// Ruft ein Match anhand der Id ab.
        /// </summary>
        /// <param name="id">Match-Id</param>
        /// <returns>Match mit bestimmter Id</returns>
        public Match? GetById(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, MatchDate, Result, Competition, IsWin, Notes FROM Match WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new Match
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Date = r.GetDateTime(2),
                    Result = r.IsDBNull(3) ? "" : r.GetString(3),
                    Competition = r.IsDBNull(4) ? "" : r.GetString(4),
                    IsWin = r.GetBoolean(5),
                    Notes = r.IsDBNull(6) ? "" : r.GetString(6),
                    Marked = r.HasRows && r.GetBoolean(7)
                };
            }
            return null;
        }

        /// <summary>
        /// Fügt ein neues Match hinzu.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public int Add(Match match)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Match (OpponentId, MatchDate, Result, Competition, IsWin, Notes, Marked) VALUES (@opponentId, @date, @result, @competition, @iswin, @notes, @marked); SELECT last_insert_rowid();", conn);
            
            cmd.Parameters.AddWithValue("@opponentId", match.OpponentId);
            cmd.Parameters.AddWithValue("@date", match.Date);
            cmd.Parameters.AddWithValue("@result", match.Result);
            cmd.Parameters.AddWithValue("@competition", match.Competition);
            cmd.Parameters.AddWithValue("@iswin", match.IsWin ? 1 : 0);
            cmd.Parameters.AddWithValue("@notes", match.Notes);
            cmd.Parameters.AddWithValue("@marked", match.Marked ? 1 : 0);

            return (int)(long)cmd.ExecuteScalar()!;
        }

        /// <summary>
        /// Aktualisiert ein bestehendes Match.
        /// </summary>
        /// <param name="match"></param>
        public void Update(Match match)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Match SET OpponnentId=@opponentId, MatchDate=@date, Result=@result, Competition=@competition, IsWin = @iswin, Notes = @notes, Marked = @marked WHERE Id=@id", conn);
    
            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.Parameters.AddWithValue("@opponentId", match.OpponentId);
            cmd.Parameters.AddWithValue("@date", match.Date);
            cmd.Parameters.AddWithValue("@result", match.Result);
            cmd.Parameters.AddWithValue("@competition", match.Competition);
            cmd.Parameters.AddWithValue("@iswin", match.IsWin ? 1 : 0);
            cmd.Parameters.AddWithValue("@notes", match.Notes);
            cmd.Parameters.AddWithValue("@marked", match.Marked ? 1 : 0);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Löscht ein Match anhand der Id.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Match WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        #region Helper
        /// <summary>
        /// Wandelt ein Datum aus der Datenbank in ein DateTime-Objekt um.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private DateTime ParseDateSafe(SQLiteDataReader r, int index)
        {
            if (r.IsDBNull(index))
                return DateTime.MinValue;

            try
            {
                return r.GetDateTime(index);
            }
            catch
            {
                var raw = r.GetString(index);

                if (DateTime.TryParseExact(
                        raw,
                        new[] { "dd.MM.yyyy HH:mm:ss", "dd.MM.yyyy", "dd.MM.yyyy H:mm:ss" },
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsed))
                {
                    return parsed;
                }

                return DateTime.MinValue;
            }
        }

        #endregion
    }
}
