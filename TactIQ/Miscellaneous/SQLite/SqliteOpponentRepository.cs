using System.Collections.Generic;
using System.Data.SQLite;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Miscellaneous.SQLite
{
    /// <summary>
    /// Klasse für den Zugriff auf die Gegner-Daten.
    /// </summary>
    public class SqliteOpponentRepository : IOpponentRepository
    {
        /// <summary>
        /// Ruft alle Gegner ab.
        /// </summary>
        /// <returns></returns>
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
                    Marked = r.GetBoolean(3)
                });
            }
            return list;
        }

        /// <summary>
        /// Ruft einen Gegner anhand der Id ab.
        /// </summary>
        /// <param name="id">Gegner-Id</param>
        /// <returns></returns>
        public Opponent? GetById(int id)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("SELECT Id, Name, Club, Marked FROM Opponent WHERE Id = @id", conn);
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

        /// <summary>
        /// Fügt einen neuen Gegner hinzu.
        /// </summary>
        /// <param name="name">Gegnername</param>
        /// <param name="club">Gegnerverein</param>
        /// <returns></returns>
        public int Add(string name, string club)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("INSERT INTO Opponent (Name, Club, Marked) VALUES (@name, @club, false); SELECT last_insert_rowid();", conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@club", club);
            return (int)(long)cmd.ExecuteScalar()!;
        }

        /// <summary>
        /// Aktualisiert einen bestehenden Gegner.
        /// </summary>
        /// <param name="opponent"></param>
        public void Update(Opponent opponent)
        {
            using var conn = new SQLiteConnection($"Data Source={DatabaseBuilder.GetDatabasePath()};Version=3;");
            conn.Open();
            using var cmd = new SQLiteCommand("UPDATE Opponent SET Name=@name, Marked=@marked, Club=@club WHERE Id=@id", conn);
            cmd.Parameters.AddWithValue("@name", opponent.Name);
            cmd.Parameters.AddWithValue("@marked", opponent.Marked ? 1 : 0);
            cmd.Parameters.AddWithValue("@club", opponent.Club);
            cmd.Parameters.AddWithValue("@id", opponent.Id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Löscht einen Gegner anhand der Id.
        /// </summary>
        /// <param name="id"></param>
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
