using System;
using System.IO;
using System.Linq;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using TactIQ.Miscellaneous;
using TactIQ.Miscellaneous.SQLite;

namespace TactIQ.Tests.IntegrationTests
{
    [Collection("Sequential")]
    public class AnalysisViewModelIntegrationTests : IDisposable
    {
        private readonly string _dbPath;
        private readonly SqliteMatchRepository _matchRepo;
        private readonly SqliteOpponentRepository _opponentRepo;

        public AnalysisViewModelIntegrationTests()
        {
            _dbPath = Path.Combine(Path.GetTempPath(), $"TestDb_{Guid.NewGuid()}.sqlite");
            DatabaseBuilder.Initialize(_dbPath);

            _matchRepo = new SqliteMatchRepository();
            _opponentRepo = new SqliteOpponentRepository();

            var opp = new Opponent { Name = "Max", Club = "FC Test" };
            var id = _opponentRepo.Add(opp.Name, opp.Club);

            _matchRepo.Add(new Match { OpponentId = id, Date = DateTime.Today.AddDays(-1), Result = "3:1", IsWin = true });
            _matchRepo.Add(new Match { OpponentId = id, Date = DateTime.Today, Result = "1:3", IsWin = false });
        }

        [Fact]
        public void Filter_ByOpponent_ReducesMatchSet_INT07()
        {
            var vm = new AnalysisViewModel(_matchRepo, _opponentRepo);
            var opp = _opponentRepo.GetAll().First();

            vm.SelectedOpponent = opp;

            Assert.All(vm.LastMatches, m => Assert.Equal(opp.Id, m.OpponentId));
        }

        [Fact]
        public void Charts_AreGenerated_INT08()
        {
            var vm = new AnalysisViewModel(_matchRepo, _opponentRepo);

            Assert.NotNull(vm.PieChartModel);
            Assert.NotEmpty(vm.PieChartModel.Series);
            Assert.NotNull(vm.LineChartModel);
            Assert.NotEmpty(vm.LineChartModel.Series);
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
