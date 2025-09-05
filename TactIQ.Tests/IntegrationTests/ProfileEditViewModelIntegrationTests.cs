using System;
using System.IO;
using Xunit;
using TactIQ.Miscellaneous;
using TactIQ.Miscellaneous.SQLite;
using TactIQ.Model;
using TactIQ.ViewModels;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Tests.IntegrationTests
{
    [Collection("Sequential")]
    public class ProfileEditViewModelIntegrationTests : IDisposable
    {
        private readonly string _testDbPath;

        public ProfileEditViewModelIntegrationTests()
        {
            // Testdatenbank im Temp-Ordner erstellen
            _testDbPath = Path.Combine(Path.GetTempPath(), $"TestDb_{Guid.NewGuid()}.sqlite");

            if (File.Exists(_testDbPath))
                File.Delete(_testDbPath);

            // Datenbank initialisieren
            DatabaseBuilder.Initialize(_testDbPath);
        }

        [Fact]
        public void Save_NewOpponent_IsPersistedInDatabase_INT01()
        {
            // Arrange
            var navMock = new DummyNavigationService(); // kann stub sein
            var opponentRepo = new SqliteOpponentRepository();
            var matchRepo = new SqliteMatchRepository();
            var noteRepo = new SqliteNotesRepository();

            var opponent = new Opponent { Id = 0, Name = "Integration Gegner", Club = "ClubX", Marked = false };
            var vm = new ProfileEditViewModel(navMock, opponentRepo, opponent, matchRepo, noteRepo);

            // Act
            vm.Name = "Neuer Gegner";
            vm.Club = "IntegrationClub";
            vm.SaveCommand.Execute(null);

            // Assert
            var saved = opponentRepo.GetAll();
            Assert.Contains(saved, o => o.Name == "Neuer Gegner" && o.Club == "IntegrationClub");
        }

        [Fact]
        public void LoadMatchesAndNotes_ReturnsPersistedData_INT02()
        {
            // Arrange
            var navMock = new DummyNavigationService();
            var opponentRepo = new SqliteOpponentRepository();
            var matchRepo = new SqliteMatchRepository();
            var noteRepo = new SqliteNotesRepository();

            var oppId = opponentRepo.Add("Opponent1", "TestClub");

            // Ein Match und eine Note direkt über Repository speichern
            matchRepo.Add(new Match { OpponentId = oppId, Result = "3:1", Competition = "Liga" });
            noteRepo.Add(new Note { OpponentId = oppId, Content = "Starker Angriff", Type = "Stärke" });

            var opponent = opponentRepo.GetById(oppId);

            // Act
            var vm = new ProfileEditViewModel(navMock, opponentRepo, opponent, matchRepo, noteRepo);

            // Assert
            Assert.NotEmpty(vm.AllMatches);
            Assert.NotEmpty(vm.AllNotes);
            Assert.Contains(vm.AllNotes, n => n.Content.Contains("Starker Angriff"));
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            try
            {
                File.Delete(_testDbPath);
            }
            catch (IOException)
            {
                Thread.Sleep(200);
                File.Delete(_testDbPath);
            }
        }
    }

    // Dummy-Navigation (damit Tests nicht fehlschlagen)
    internal class DummyNavigationService : INavigationService
    {
        public void NavigateTo(object viewModel) { /* keine echte Navigation */ }
    }
}
