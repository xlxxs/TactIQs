using Moq;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using static TactIQ.Miscellaneous.Interfaces;
using Match = TactIQ.Model.Match;

namespace TactIQ.Tests.UnitTests
{
    public class ProfileEditViewModelTests
    {
        private readonly Mock<INavigationService> _navMock;
        private readonly Mock<IOpponentRepository> _opponentRepoMock;
        private readonly Mock<IMatchRepository> _matchRepoMock;
        private readonly Mock<INoteRepository> _noteRepoMock;
        private readonly Opponent _opponent;

        public ProfileEditViewModelTests()
        {
            DatabaseBuilder.Initialize();
            _navMock = new Mock<INavigationService>();
            _opponentRepoMock = new Mock<IOpponentRepository>();
            _matchRepoMock = new Mock<IMatchRepository>();
            _noteRepoMock = new Mock<INoteRepository>();
            _opponent = new Opponent { Id = 1, Name = "Max", Club = "Verein", Marked = false };
        }

        private ProfileEditViewModel CreateViewModel(
            IEnumerable<Match>? matches = null,
            IEnumerable<Note>? notes = null)
        {
            _matchRepoMock.Setup(r => r.GetAllForOpponent(It.IsAny<int>()))
                .Returns(matches ?? Enumerable.Empty<Match>());

            _noteRepoMock.Setup(r => r.GetAllForOpponent(It.IsAny<int>()))
                .Returns(notes ?? Enumerable.Empty<Note>());

            var vm = new ProfileEditViewModel(
                _navMock.Object,
                _opponentRepoMock.Object,
                _opponent,
                _matchRepoMock.Object,
                _noteRepoMock.Object);


            typeof(ProfileEditViewModel).GetField("_matchRepo",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(vm, _matchRepoMock.Object);

            typeof(ProfileEditViewModel).GetField("_notesRepo",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(vm, _noteRepoMock.Object);

            return vm;
        }

        [Fact]
        public void LoadMatches_MatchesLoaded_K01()
        {
            // Arrange
            var matches = new List<Match>
            {
                new Match { Id = 1, OpponentId = 1, Date = DateTime.Now.AddDays(-1), Marked = false },
                new Match { Id = 2, OpponentId = 1, Date = DateTime.Now, Marked = true },
                new Match { Id = 3, OpponentId = 1, Date = DateTime.Now.AddDays(-2), Marked = false }
            };

            // Act
            var vm = CreateViewModel(matches);

            vm.LoadMatches();

            // Assert
            Assert.Equal(3, vm.AllMatches.Count);
            Assert.Equal(3, vm.RecentMatches.Count);
            Assert.Equal(2, vm.RecentMatches[0].Id); // Marked zuerst
        }

        [Fact]
        public void LoadNotes_NotesLoaded_K02()
        {
            // Arrange
            var notes = new List<Note>
            {
                new Note { Id = 1, OpponentId = 1, Type = "Stärke", CreatedAt = DateTime.Now, Marked = true },
                new Note { Id = 2, OpponentId = 1, Type = "Stärke", CreatedAt = DateTime.Now.AddMinutes(-1), Marked = false },
                new Note { Id = 3, OpponentId = 1, Type = "Schwäche", CreatedAt = DateTime.Now, Marked = false },
                new Note { Id = 4, OpponentId = 1, Type = "Sonstiges", CreatedAt = DateTime.Now, Marked = false }
            };

            // Act
            var vm = CreateViewModel(null, notes);

            vm.LoadNotes();

            // Assert
            Assert.Equal(4, vm.AllNotes.Count);
            Assert.Equal(2, vm.RecentStrengths.Count);
            Assert.Single(vm.RecentWeaknesses);
            Assert.Single(vm.RecentMisc);
        }

        [Fact]
        public void DeleteMatchCommand_MatchDeleted_K03()
        {
            // Arrange
            var match = new Match { Id = 42, OpponentId = 1 };
            _matchRepoMock.Setup(r => r.GetAllForOpponent(It.IsAny<int>())).Returns(new[] { match });

            // Act
            var vm = CreateViewModel(matches: new[] { match });

            vm.DeleteMatchCommand.Execute(match);

            // Assert
            _matchRepoMock.Verify(r => r.Delete(42), Times.Once);
        }

        [Fact]
        public void DeleteNoteCommand_NoteDeleted_K04()
        {
            // Arrange
            var note = new Note { Id = 99, OpponentId = 1 };
            _noteRepoMock.Setup(r => r.GetAllForOpponent(It.IsAny<int>())).Returns(new[] { note });

            // Act
            var vm = CreateViewModel(notes: new[] { note });

            vm.DeleteNoteCommand.Execute(note);

            // Assert
            _noteRepoMock.Verify(r => r.Delete(99), Times.Once);
        }

        [Fact]
        public void SaveCommandIfNew_NewOpponentSaved_K05()
        {
            //Arrange
            _opponentRepoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Opponent)null);
            var vm = CreateViewModel();

            vm.Name = "Test";
            vm.Club = "TestClub";

            //Act
            vm.SaveCommand.Execute(null);

            //Assert
            _opponentRepoMock.Verify(r => r.Add("Test", "TestClub"), Times.Once);
            _navMock.Verify(n => n.NavigateTo(It.IsAny<object>()), Times.AtLeastOnce);
        }

        [Fact]
        public void SaveCommandIfExisting_UpdatedOpponentSaved_K06()
        {
            //Arrange
            _opponentRepoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(_opponent);
            var vm = CreateViewModel();
            vm.Name = "Neu";
            vm.Club = "NeuClub";
            vm.Marked = true;

            //Act
            vm.SaveCommand.Execute(null);

            //Assert
            _opponentRepoMock.Verify(r => r.Update(It.Is<Opponent>(o =>
                o.Name == "Neu" && o.Club == "NeuClub" && o.Marked)), Times.Once);
            _navMock.Verify(n => n.NavigateTo(It.IsAny<object>()), Times.AtLeastOnce);
        }

        [Fact]
        public void PropertyChanged_K07()
        {
            //Arrange
            var vm = CreateViewModel();
            var props = new List<string>();

            //Act
            vm.PropertyChanged += (s, e) => props.Add(e.PropertyName!);

            vm.Name = "A";
            vm.Club = "B";
            vm.Marked = true;

            //Assert
            Assert.Contains("Name", props);
            Assert.Contains("Club", props);
            Assert.Contains("Marked", props);
        }

        [Fact]
        public void SelectedMatch_SelectedMatchChanged_K08()
        {
            //Arrange
            var vm = CreateViewModel();
            var called = false;

            //Act
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == "SelectedMatch") called = true; };

            vm.SelectedMatch = new Match();

            //Assert
            Assert.True(called);
        }

        [Fact]
        public void SelectedNote_SelectedNoteChanged_K09()
        {
            //Arrange
            var vm = CreateViewModel();
            var called = false;

            //Act
            vm.PropertyChanged += (s, e) => { if (e.PropertyName == "SelectedNote") called = true; };

            vm.SelectedNote = new Note();

            //Assert
            Assert.True(called);
        }
    }
}
