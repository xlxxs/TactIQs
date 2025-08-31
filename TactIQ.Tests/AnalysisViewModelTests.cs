using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Moq;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using static TactIQ.Miscellaneous.Interfaces;
using Match = TactIQ.Model.Match;

namespace TactIQ.Tests
{
    public class AnalysisViewModelTests
    {
        private readonly Mock<IMatchRepository> _matchRepoMock;
        private readonly Mock<IOpponentRepository> _opponentRepoMock;

        public AnalysisViewModelTests()
        {
            _matchRepoMock = new Mock<IMatchRepository>();
            _opponentRepoMock = new Mock<IOpponentRepository>();
        }

        [Fact]
        public void LoadAllMatchesAndOpponents()
        {
            // Arrange
            var matches = new List<Match>
            {
                new Match { Id = 1, Date = DateTime.Today, IsWin = true },
                new Match { Id = 2, Date = DateTime.Today.AddDays(-1), IsWin = false }
            };
            var opponents = new List<Opponent>
            {
                new Opponent { Id = 1, Name = "A", Club = "ClubA" }
            };
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(matches);
            _opponentRepoMock.Setup(r => r.GetAll()).Returns(opponents);

            // Act
            var vm = new AnalysisViewModel(_matchRepoMock.Object, _opponentRepoMock.Object);

            // Assert
            Assert.Equal(2, vm.LastMatches.Count);
            Assert.Single(vm.Opponents);
        }

        [Fact]
        public void ApplyFilterOpponent()
        {
            // Arrange
            var matches = new List<Match>
            {
                new Match { Id = 1, OpponentId = 1, Date = DateTime.Today },
                new Match { Id = 2, OpponentId = 2, Date = DateTime.Today }
            };
            var opponents = new List<Opponent>
            {
                new Opponent { Id = 1, Name = "A", Club = "ClubA" },
                new Opponent { Id = 2, Name = "B", Club = "ClubB" }
            };
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(matches);
            _opponentRepoMock.Setup(r => r.GetAll()).Returns(opponents);

            var vm = new AnalysisViewModel(_matchRepoMock.Object, _opponentRepoMock.Object);

            // Act
            vm.SelectedOpponent = opponents.First(o => o.Id == 1);

            // Assert
            Assert.Single(vm.LastMatches);
            Assert.Equal(1, vm.LastMatches.First().Id);
        }

        [Fact]
        public void ApplyFilterDate()
        {
            // Arrange
            var matches = new List<Match>
            {
                new Match { Id = 1, Date = DateTime.Today.AddDays(-10) },
                new Match { Id = 2, Date = DateTime.Today }
            };
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(matches);
            _opponentRepoMock.Setup(r => r.GetAll()).Returns(new List<Opponent>());

            var vm = new AnalysisViewModel(_matchRepoMock.Object, _opponentRepoMock.Object);

            // Act
            vm.FromDate = DateTime.Today.AddDays(-5);

            // Assert
            Assert.Single(vm.LastMatches);
            Assert.Equal(2, vm.LastMatches.First().Id);
        }

        [Fact]
        public void UpdateCharts()
        {
            // Arrange
            var matches = new List<Match>
            {
                new Match { Id = 1, Date = DateTime.Today, IsWin = true },
                new Match { Id = 2, Date = DateTime.Today, IsWin = false }
            };
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(matches);
            _opponentRepoMock.Setup(r => r.GetAll()).Returns(new List<Opponent>());

            var vm = new AnalysisViewModel(_matchRepoMock.Object, _opponentRepoMock.Object);

            // Act
            var pie = vm.PieChartModel;
            var line = vm.LineChartModel;

            // Assert
            Assert.NotNull(pie);
            Assert.NotNull(line);
            Assert.True(pie.Series.Count > 0);
            Assert.True(line.Series.Count > 0);
        }
    }
}
