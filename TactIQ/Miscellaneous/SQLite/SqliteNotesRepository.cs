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
    public class SqliteNotesRepository: INoteRepository
    {
        public IEnumerable<Note> GetAllForOpponent(int opponentId)
        {
            var list = new List<Note>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, Content, Type FROM Note WHERE OpponentId = @id", conn);
            cmd.Parameters.AddWithValue("@id", opponentId);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Note
                {
                    Id = r.GetInt32(0),
                    OpponentId = r.GetInt32(1),
                    Content = r.IsDBNull(2) ? "" : r.GetString(2),
                    Type = r.IsDBNull(3) ? "" : r.GetString(3)
                });           
            }

            return list;
        }

        public Note? GetById(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, OpponentId, Content, Type, CreatedAt FROM Note WHERE Id = @id", conn);
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
                    CreatedAt = r.GetDateTime(4)
                };
            }
            return null;
        }

        public int Add(Note note)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Note (OpponentId, Content, Type) VALUES (@opponentId, @content, @type); SELECT last_insert_rowid();", conn);

            cmd.Parameters.AddWithValue("@opponentId", note.OpponentId);
            cmd.Parameters.AddWithValue("@content", note.Content);
            cmd.Parameters.AddWithValue("@type", note.Type);

            return (int)(long)cmd.ExecuteScalar()!;
        }

        public void Update(Note note)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Note SET OpponnentId=@opponentId, Content=@content, Type=@type WHERE Id=@id", conn);

            cmd.Parameters.AddWithValue("@id", note.Id);
            cmd.Parameters.AddWithValue("@opponentId", note.OpponentId);
            cmd.Parameters.AddWithValue("@content", note.Content);
            cmd.Parameters.AddWithValue("@type", note.Type);

            cmd.ExecuteNonQuery();
        }

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
