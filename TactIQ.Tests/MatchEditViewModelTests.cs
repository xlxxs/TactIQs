using Moq;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using static TactIQ.Miscellaneous.Interfaces;
using Match = TactIQ.Model.Match;

namespace TactIQ.Tests
{
    public class MatchEditViewModelTests
    {
        private readonly Mock<IMatchRepository> _matchRepoMock;

        public MatchEditViewModelTests()
        {
            _matchRepoMock = new Mock<IMatchRepository>();
        }

        [Fact]
        public void SaveCommand_AddsMatch()
        {
            var newMatch = new Match { Id = 1, OpponentId = 1, Result = "3:1" };
            var vm = new MatchEditViewModel(_matchRepoMock.Object, newMatch);

            vm.SaveCommand.Execute(null);

            _matchRepoMock.Verify(r => r.Add(It.Is<Match>(m => m.Result == "3:1")), Times.Once);
        }

        [Fact]
        public void SaveCommand_UpdatedMatch()
        {
            var existingMatch = new Match { Id = 42, OpponentId = 1, Result = "2:3" };
            _matchRepoMock.Setup(r => r.GetById(42)).Returns(existingMatch);

            var vm = new MatchEditViewModel(_matchRepoMock.Object, existingMatch);

            vm.SaveCommand.Execute(null);

            _matchRepoMock.Verify(r => r.Update(It.Is<Match>(m => m.Id == 42 && m.Result == "2:3")), Times.Once);
        }
    }
}
