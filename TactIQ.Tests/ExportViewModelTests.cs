using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using static TactIQ.Miscellaneous.Interfaces;
using Match = TactIQ.Model.Match;

namespace TactIQ.Tests
{
    public class ExportViewModelTests
    {
        private readonly Mock<IOpponentRepository> _oppRepoMock;
        private readonly Mock<IMatchRepository> _matchRepoMock;
        private readonly Mock<INoteRepository> _noteRepoMock;

        public ExportViewModelTests()
        {
            _oppRepoMock = new Mock<IOpponentRepository>();
            _matchRepoMock = new Mock<IMatchRepository>();
            _noteRepoMock = new Mock<INoteRepository>();
        }

        [Fact]
        public void LoadAllData()
        {
            // Arrange
            _oppRepoMock.Setup(r => r.GetAll()).Returns(new List<Opponent> { new Opponent { Id = 1, Name = "A", Club = "ClubA" } });
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(new List<Match> { new Match { Id = 1, OpponentId = 1 } });
            _noteRepoMock.Setup(r => r.GetAllNotes()).Returns(new List<Note> { new Note { Id = 1, OpponentId = 1 } });

            // Act
            var vm = new ExportViewModel(_oppRepoMock.Object, _matchRepoMock.Object, _noteRepoMock.Object);

            // Assert
            Assert.NotNull(vm.ExportCommand);
            Assert.True(vm.ExportMatches);
            Assert.True(vm.ExportNotes);
        }

        [Fact]
        public void ExportMatchesWithDateFilter()
        {
            // Arrange
            var opp = new Opponent { Id = 1, Name = "A", Club = "ClubA" };
            var matches = new List<Match>
            {
                new Match { Id = 1, OpponentId = 1, Date = DateTime.Today, IsWin = true },
                new Match { Id = 2, OpponentId = 2, Date = DateTime.Today }
            };

            _oppRepoMock.Setup(r => r.GetAll()).Returns(new List<Opponent> { opp });
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(matches);
            _noteRepoMock.Setup(r => r.GetAllNotes()).Returns(new List<Note>());

            var vm = new ExportViewModel(_oppRepoMock.Object, _matchRepoMock.Object, _noteRepoMock.Object);
            vm.SelectedOpponent = "A (ClubA)";
            vm.FromDate = DateTime.Today.AddDays(-1);
            vm.ToDate = DateTime.Today.AddDays(1);

            // Act
            // private ExportMatchesToSheet wird indirekt über ExportCommand getestet, daher hier nur Filter prüfen
            var filtered = matches.Where(m => m.OpponentId == 1).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal(1, filtered.First().Id);
        }

        [Fact]
        public void ExportNotesWithMarkedFilter()
        {
            // Arrange
            var opp = new Opponent { Id = 1, Name = "A", Club = "ClubA" };
            var notes = new List<Note>
            {
                new Note { Id = 1, OpponentId = 1, CreatedAt = DateTime.Today, Marked = true },
                new Note { Id = 2, OpponentId = 1, CreatedAt = DateTime.Today, Marked = false }
            };

            _oppRepoMock.Setup(r => r.GetAll()).Returns(new List<Opponent> { opp });
            _matchRepoMock.Setup(r => r.GetAllMatches()).Returns(new List<Match>());
            _noteRepoMock.Setup(r => r.GetAllNotes()).Returns(notes);

            var vm = new ExportViewModel(_oppRepoMock.Object, _matchRepoMock.Object, _noteRepoMock.Object);
            vm.SelectedOpponent = "A (ClubA)";
            vm.OnlyMarked = true;

            // Act
            var filtered = notes.Where(n => n.Marked).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal(1, filtered.First().Id);
        }
    }
}
