using System.Collections.Generic;
using System.Data.SQLite;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Abstractions;

namespace TactIQ.Services
{
    public class SqliteOpponentRepository : IOpponentRepository
    {
        public IEnumerable<Opponent> GetAll()
        {
            var list = new List<Opponent>();
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, Name, Club, Marked FROM Opponent", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Opponent
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Club = r.IsDBNull(2) ? "" : r.GetString(2),
                    Marked = Convert.ToBoolean(r.GetString(3))
                });
            }
            return list;
        }

        public Opponent? GetById(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, Name, '' as Club, Marked FROM Opponent WHERE Id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = cmd.ExecuteReader();
            if (r.Read())
            {
                return new Opponent
                {
                    Id = r.GetInt32(0),
                    Name = r.GetString(1),
                    Club = r.IsDBNull(2) ? "" : r.GetString(2),
                    Marked = r.GetBoolean(3)
                };
            }
            return null;
        }

        public int Add(string name)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Opponent (Name) VALUES (@name); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@name", name);
            return (int)(long)cmd.ExecuteScalar()!;
        }

        public void Update(Opponent opponent)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Opponent SET Name=@name, Marked=@marked WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@name", opponent.Name);
            cmd.Parameters.AddWithValue("@marked", opponent.Marked ? 1 : 0);
            cmd.Parameters.AddWithValue("@id", opponent.Id);
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("DELETE FROM Opponent WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
    }
}
