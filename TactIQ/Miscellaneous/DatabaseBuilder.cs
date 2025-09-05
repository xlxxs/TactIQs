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
    /// <summary>
    /// Klasse zum Erstellen und Initialisieren der SQLite-Datenbank.
    /// </summary>
    public static class DatabaseBuilder
    {
        private static string _databasePath;

        /// <summary>
        /// Metthode zum Initialisieren der Datenbank.
        /// </summary>
        public static void Initialize(string? customPath = null)
        {
            if (customPath != null)
            {
                _databasePath = customPath;
            }
            else
            {
                string? assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                _databasePath = Path.Combine(assemblyDir, "AppData.sqlite");
            }

            if (!File.Exists(_databasePath))
            {
                CreateDatabase();
            }
        }

        /// <summary>
        /// Methode zum Erstellen der SQLite-Datenbank und der erforderlichen Tabellen.
        /// </summary>
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

        /// <summary>
        /// Methode zum Erstellen der Tabelle für Gegner.
        /// </summary>
        /// <param name="conn">SQLite Verbindung</param>
        private static void CreateOpponentTable(SQLiteConnection conn)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Opponent (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Club TEXT NOT NULL,
                Marked BOOLEAN DEFAULT 0
            );";
            new SQLiteCommand(sql, conn).ExecuteNonQuery();
        }

        /// <summary>
        /// Methode zum Erstellen der Tabelle für Matches.
        /// </summary>
        /// <param name="conn">SQLite Verbindung</param>
        private static void CreateMatchTable(SQLiteConnection conn)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Match (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OpponentId INTEGER NOT NULL,
                MatchDate DATETIME DEFAULT CURRENT_TIMESTAMP,
                Result TEXT,
                Competition TEXT,   
                IsWin BOOLEAN DEFAULT 0,
                Marked BOOLEAN DEFAULT 0,
                Notes TEXT,
                FOREIGN KEY (OpponentId) REFERENCES Opponent(Id)
            );";
            new SQLiteCommand(sql, conn).ExecuteNonQuery();
        }

        /// <summary>
        /// Methode zum Erstellen der Tabelle für Notizen.
        /// </summary>
        /// <param name="conn">SQLite Verbindung</param>
        private static void CreateNoteTable(SQLiteConnection conn)
        {
            string sql = @"
            CREATE TABLE IF NOT EXISTS Note (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                OpponentId INTEGER NOT NULL,
                Content TEXT,
                Type TEXT,
                Category TEXT,
                Marked BOOLEAN DEFAULT 0,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (OpponentId) REFERENCES Opponent(Id)
            );";
            new SQLiteCommand(sql, conn).ExecuteNonQuery();
        }

        public static string GetDatabasePath() => _databasePath;
    }
}
