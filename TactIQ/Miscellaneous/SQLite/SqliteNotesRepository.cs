using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Miscellaneous.SQLite
{
    /// <summary>
    /// Klasse für den Zugriff auf die Notiz-Daten.
    /// </summary>
    public class SqliteNotesRepository: INoteRepository
    {
        /// <summary>
        /// Ruft alle Notizen für einen bestimmten Gegner ab.
        /// </summary>
        /// <param name="opponentId">Gegner-Id</param>
        /// <returns>Liste von Matches für bestimmtten Gegner</returns>
        public IEnumerable<Note> GetAllForOpponent(int opponentId)
        {
            var list = new List<Note>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, Content, Type, Marked FROM Note WHERE OpponentId = @id", conn);
            cmd.Parameters.AddWithValue("@id", opponentId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Note
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Content = r.IsDBNull(2) ? "" : r.GetString(2),
                    Type = r.IsDBNull(3) ? "" : r.GetString(3),
                    Marked = r.HasRows && r.GetBoolean(4)
                });           
            }

            return list;
        }

        /// <summary>
        /// Ruft alle Notizen ab.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Note> GetAllNotes()
        {
            var list = new List<Note>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, Content, Type, Marked FROM Note", conn);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Note
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Content = r.IsDBNull(2) ? "" : r.GetString(2),
                    Type = r.IsDBNull(3) ? "" : r.GetString(3),
                    Marked = r.HasRows && r.GetBoolean(4)
                });
            }

            return list;
        }

        /// <summary>
        /// Ruft eine Notiz anhand ihrer Id ab.
        /// </summary>
        /// <param name="id">Notiz-Id</param>
        /// <returns></returns>
        public Note? GetById(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, Content, Type, CreatedAt, Marked, Category FROM Note WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new Note
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Content = r.IsDBNull(2) ? "" : r.GetString(2),
                    Type = r.IsDBNull(3) ? "" : r.GetString(3),
                    CreatedAt = r.GetDateTime(4),
                    Marked = r.HasRows && r.GetBoolean(5),
                    Category = r.IsDBNull(6) ? "" : r.GetString(6)
                };
            }
            return null;
        }

        /// <summary>
        /// Fügt eine neue Notiz hinzu.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        public int Add(Note note)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Note (OpponentId, Content, Type, Marked, Category) VALUES (@opponentId, @content, @type, @marked, @category); SELECT last_insert_rowid();", conn);

            cmd.Parameters.AddWithValue("@opponentId", note.OpponentId);
            cmd.Parameters.AddWithValue("@content", note.Content);
            cmd.Parameters.AddWithValue("@type", note.Type);
            cmd.Parameters.AddWithValue("@marked", note.Marked ? 1 : 0);
            cmd.Parameters.AddWithValue("@category", note.Category ?? string.Empty);

            return (int)(long)cmd.ExecuteScalar()!;
        }

        /// <summary>
        /// Aktualisiert eine bestehende Notiz.
        /// </summary>
        /// <param name="note"></param>
        public void Update(Note note)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Note SET OpponentId=@opponentId, Content=@content, Type=@type, Marked = @marked, Category = @category WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@id", note.Id);
            cmd.Parameters.AddWithValue("@opponentId", note.OpponentId);
            cmd.Parameters.AddWithValue("@content", note.Content);
            cmd.Parameters.AddWithValue("@type", note.Type);
            cmd.Parameters.AddWithValue("@marked", note.Marked ? 1 : 0);
            cmd.Parameters.AddWithValue("@category", note.Category ?? string.Empty);

            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Löscht eine Notiz anhand ihrer Id.
        /// </summary>
        /// <param name="id">Notiz-Id</param>
        public void Delete(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Note WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
