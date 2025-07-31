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
    public class SqliteMatchRepository: IMatchRepository
    {
        public IEnumerable<Match> GetAllForOpponent(int opponentId)
        {
            var list = new List<Match>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, MatchDate, Result, Competition, IsWin, Notes FROM Match Where OpponentId = @id", conn);
            cmd.Parameters.AddWithValue("@id", opponentId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Match
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Date = ParseDateSafe(r, 2), // ← Aufruf der Hilfsmethode
                    Result = r.IsDBNull(3) ? "" : r.GetString(3),
                    Competition = r.IsDBNull(4) ? "" : r.GetString(4),
                    IsWin = r.GetBoolean(5),
                    Notes = r.IsDBNull(6) ? "" : r.GetString(6)
                });
            }
            return list;
        }

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
                    Notes = r.IsDBNull(6) ? "" : r.GetString(6)
                };
            }
            return null;
        }

        public int Add(Match match)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Match (OpponentId, MatchDate, Result, Competition, IsWin, Notes) VALUES (@opponentId, @date, @result, @competition, @iswin, @notes); SELECT last_insert_rowid();", conn);
            
            cmd.Parameters.AddWithValue("@opponentId", match.OpponentId);
            cmd.Parameters.AddWithValue("@date", match.Date);
            cmd.Parameters.AddWithValue("@result", match.Result);
            cmd.Parameters.AddWithValue("@competition", match.Competition);
            cmd.Parameters.AddWithValue("@iswin", match.IsWin ? 1 : 0);
            cmd.Parameters.AddWithValue("@notes", match.Notes);

            return (int)(long)cmd.ExecuteScalar()!;
        }

        public void Update(Match match)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Match SET OpponnentId=@opponentId, MatchDate=@date, Result=@result, Competition=@competition, IsWin = @iswin, Notes = @notes WHERE Id=@id", conn);
    
            cmd.Parameters.AddWithValue("@id", match.Id);
            cmd.Parameters.AddWithValue("@opponentId", match.OpponentId);
            cmd.Parameters.AddWithValue("@date", match.Date);
            cmd.Parameters.AddWithValue("@result", match.Result);
            cmd.Parameters.AddWithValue("@competition", match.Competition);
            cmd.Parameters.AddWithValue("@iswin", match.IsWin ? 1 : 0);
            cmd.Parameters.AddWithValue("@notes", match.Notes);

            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Match WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        #region Helper
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
