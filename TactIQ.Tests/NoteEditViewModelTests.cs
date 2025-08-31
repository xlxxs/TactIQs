using Moq;
using TactIQ.Model;
using TactIQ.ViewModels;
using Xunit;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Tests
{
    public class NoteEditViewModelTests
    {
        private readonly Mock<INoteRepository> _noteRepoMock;

        public NoteEditViewModelTests()
        {
            _noteRepoMock = new Mock<INoteRepository>();
        }

        [Fact]
        public void SaveCommand_AddsNote()
        {
            var newNote = new Note { Id = 42, OpponentId = 1, Content = "Neue Notiz" };
            var vm = new NoteEditViewModel(_noteRepoMock.Object, newNote);

            vm.SaveCommand.Execute(null);

            _noteRepoMock.Verify(r => r.Add(It.Is<Note>(n => n.Content == "Neue Notiz")), Times.Once);
        }

        [Fact]
        public void SaveCommand_UpdatedNote()
        {
            var existingNote = new Note { Id = 42, OpponentId = 1, Content = "Update Notiz" };
            _noteRepoMock.Setup(r => r.GetById(42)).Returns(existingNote);

            var vm = new NoteEditViewModel(_noteRepoMock.Object, existingNote);

            vm.SaveCommand.Execute(null);

            _noteRepoMock.Verify(r => r.Update(It.Is<Note>(n => n.Id == 42 && n.Content == "Update Notiz")), Times.Once);
        }
    }
}
