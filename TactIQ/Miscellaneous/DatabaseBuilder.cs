using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TactIQ.Miscellaneous
{
    public static class DatabaseBuilder
    {
        private static string _databasePath;

        public static void Initialize()
        {
            string? assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _databasePath = Path.Combine(assemblyDir, "AppData.sqlite");

            if (!File.Exists(_databasePath))
            {
                CreateDatabase();
            }
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(_databasePath);

            using (var connection = new SQLiteConnection($"Data Source={_databasePath};Version=3;"))
            {
                connection.Open();

                CreateOpponentTable(connection);
                CreateMatchTable(connection);
                CreateNoteTable(connection);
            }
        }

        private static void CreateOpponentTable(SQLiteConnection conn)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Opponent (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Marked BOOLEAN DEFAULT 0
            );";
            new SQLiteCommand(sql, conn).ExecuteNonQuery();
        }

        private static void CreateMatchTable(SQLiteConnection conn)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Match (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OpponentId INTEGER NOT NULL,
                MatchDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                Result TEXT,
                FOREIGN KEY (OpponentId) REFERENCES Opponent(Id)
            );";
            new SQLiteCommand(sql, conn).ExecuteNonQuery();
        }

        private static void CreateNoteTable(SQLiteConnection conn)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Note (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OpponentId INTEGER NOT NULL,
                Content TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (OpponentId) REFERENCES Opponent(Id)
            );";
            new SQLiteCommand(sql, conn).ExecuteNonQuery();
        }

        public static string GetDatabasePath() => _databasePath;
    }
}
