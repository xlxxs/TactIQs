using System;
using System.IO;
using System.Linq;
using Xunit;
using ClosedXML.Excel;
using TactIQ.ViewModels;
using TactIQ.Model;
using TactIQ.Miscellaneous;
using TactIQ.Miscellaneous.SQLite;

namespace TactIQ.Tests.IntegrationTests
{
    [Collection("Sequential")]
    public class ExportViewModelIntegrationTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly SqliteOpponentRepository _opponentRepo;
        private readonly SqliteMatchRepository _matchRepo;
        private readonly SqliteNotesRepository _noteRepo;

        public ExportViewModelIntegrationTests()
        {
            // jede Testklasse bekommt eine eigene DB
            _dbPath = Path.Combine(Path.GetTempPath(), $"TestDb_{Guid.NewGuid()}.sqlite");
            DatabaseBuilder.Initialize(_dbPath);

            _opponentRepo = new SqliteOpponentRepository();
            _matchRepo = new SqliteMatchRepository();
            _noteRepo = new SqliteNotesRepository();

            // Testdaten vorbereiten
            var oppId = _opponentRepo.Add("Test Gegner", "FC Test");

            _matchRepo.Add(new Match
            {
                OpponentId = oppId,
                Competition = "Liga",
                Result = "3:1",
                Date = DateTime.Today,
                Marked = true
            });

            _noteRepo.Add(new Note
            {
                OpponentId = oppId,
                Category = "Taktik",
                Type = "Analyse",
                Content = "Testinhalt",
                CreatedAt = DateTime.Today,
                Marked = false
            });
        }

        [Fact]
        public void Export_CreatesExcelFile_WithMatchesAndNotes_INT09()
        {
            var vm = new ExportViewModel(_opponentRepo, _matchRepo, _noteRepo);
            var exportPath = Path.Combine(Path.GetTempPath(), $"Export_{Guid.NewGuid()}.xlsx");

            vm.ExportToExcel(exportPath);

            Assert.True(File.Exists(exportPath));

            using var workbook = new XLWorkbook(exportPath);
            var wsMatches = workbook.Worksheet("Matches");
            var wsNotes = workbook.Worksheet("Notizen");

            // Matches-Sheet prüfen
            Assert.Equal("Liga", wsMatches.Cell(2, 4).GetString());
            Assert.Equal("3:1", wsMatches.Cell(2, 5).GetString());

            // Notes-Sheet prüfen
            Assert.Equal("Analyse", wsNotes.Cell(2, 5).GetString());
            Assert.Equal("Testinhalt", wsNotes.Cell(2, 6).GetString());
        }

        [Fact]
        public void Export_OnlyMarked_ExcludesUnmarkedNotes_INT10()
        {
            var vm = new ExportViewModel(_opponentRepo, _matchRepo, _noteRepo)
            {
                ExportNotes = true,
                ExportMatches = false,
                OnlyMarked = true
            };
            var exportPath = Path.Combine(Path.GetTempPath(), $"Export_{Guid.NewGuid()}.xlsx");

            vm.ExportToExcel(exportPath);

            using var workbook = new XLWorkbook(exportPath);
            var wsNotes = workbook.Worksheet("Notizen");

            // Header + keine Zeilen, da Note nicht markiert war
            Assert.Equal("", wsNotes.Cell(2, 1).GetString());
        }

        [Fact]
        public void Export_FilterByOpponent_OnlyExportsSelectedOpponent_INT11()
        {
            // zweiten Gegner + Match anlegen
            var otherId = _opponentRepo.Add("Anderer Gegner", "SV Fremd");
            _matchRepo.Add(new Match { OpponentId = otherId, Competition = "Pokal", Result = "1:3", Date = DateTime.Today });

            var vm = new ExportViewModel(_opponentRepo, _matchRepo, _noteRepo)
            {
                SelectedOpponent = "Test Gegner (FC Test)"
            };
            var exportPath = Path.Combine(Path.GetTempPath(), $"Export_{Guid.NewGuid()}.xlsx");

            vm.ExportToExcel(exportPath);

            using var workbook = new XLWorkbook(exportPath);
            var wsMatches = workbook.Worksheet("Matches");

            // sollte nur den ersten Gegner enthalten
            Assert.Equal("Liga", wsMatches.Cell(2, 4).GetString());
            Assert.Equal("3:1", wsMatches.Cell(2, 5).GetString());
            Assert.Equal("", wsMatches.Cell(3, 1).GetString()); // keine zweite Zeile
        }

        public void Dispose()
        {
            try
            {
                File.Delete(_dbPath);
            }
            catch { }
        }
    }
}