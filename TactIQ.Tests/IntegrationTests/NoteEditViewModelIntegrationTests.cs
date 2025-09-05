using System;
using System.IO;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using TactIQ.Miscellaneous;
using TactIQ.Miscellaneous.SQLite;

namespace TactIQ.Tests.IntegrationTests
{
    [Collection("Sequential")]
    public class NoteEditViewModelIntegrationTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly SqliteNotesRepository _noteRepo;

        public NoteEditViewModelIntegrationTests()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"TestDb_{Guid.NewGuid()}.sqlite");
            DatabaseBuilder.Initialize(_dbPath);

            _noteRepo = new SqliteNotesRepository();
        }

        [Fact]
        public void Save_NewNote_IsPersisted_INT03()
        {
            var note = new Note { OpponentId = 1, Content = "Testnote", Category = "Kategorie", Type = "Stärke", Marked = true };
            var vm = new NoteEditViewModel(_noteRepo, note);

            vm.SaveCommand.Execute(null);

            var saved = _noteRepo.GetAllForOpponent(1);
            Assert.Contains(saved, n => n.Content == "Testnote" && n.Marked);
        }

        [Fact]
        public void Save_UpdateNote_IsUpdated_INT04()
        {
            var note = new Note { OpponentId = 1, Content = "Alt", Category = "Kategorie", Type = "Schwäche", Marked = false };
            var id = _noteRepo.Add(note);

            var loaded = _noteRepo.GetById(id)!;
            var vm = new NoteEditViewModel(_noteRepo, loaded)
            {
                Content = "Neu",
                IsMarked = true
            };

            vm.SaveCommand.Execute(null);

            var updated = _noteRepo.GetById(id)!;
            Assert.Equal("Neu", updated.Content);
            Assert.True(updated.Marked);
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (File.Exists(_dbPath))
                File.Delete(_dbPath);
        }
    }
}
