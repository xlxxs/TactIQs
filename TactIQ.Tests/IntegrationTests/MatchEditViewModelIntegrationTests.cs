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
    public class MatchEditViewModelIntegrationTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly SqliteMatchRepository _matchRepo;

        public MatchEditViewModelIntegrationTests()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"TestDb_{Guid.NewGuid()}.sqlite");
            DatabaseBuilder.Initialize(_dbPath);

            _matchRepo = new SqliteMatchRepository();
        }

        [Fact]
        public void Save_NewMatch_IsPersisted_INT05()
        {
            var match = new Match { OpponentId = 1, Competition = "Liga", Result = "3:1", Date = DateTime.Today };
            var vm = new MatchEditViewModel(_matchRepo, match);

            vm.SaveCommand.Execute(null);

            var saved = _matchRepo.GetAllForOpponent(1);
            Assert.Contains(saved, m => m.Result == "3:1" && m.IsWin);
        }

        [Fact]
        public void Save_UpdateMatch_IsUpdated_INT06()
        {
            var match = new Match { OpponentId = 1, Competition = "Alt", Result = "0:3", Date = DateTime.Today };
            var id = _matchRepo.Add(match);

            var loaded = _matchRepo.GetById(id)!;
            var vm = new MatchEditViewModel(_matchRepo, loaded)
            {
                Competition = "Neu",
                Result = "3:0"
            };

            vm.SaveCommand.Execute(null);

            var updated = _matchRepo.GetById(id)!;
            Assert.Equal("Neu", updated.Competition);
            Assert.True(updated.IsWin);
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            try
            {
                File.Delete(_dbPath);
            }
            catch (IOException)
            {
                Thread.Sleep(200);
                File.Delete(_dbPath);
            }
        }
    }
}
