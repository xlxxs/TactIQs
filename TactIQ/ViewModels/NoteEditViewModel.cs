using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    public class NoteEditViewModel : INotifyPropertyChanged
    {
        private readonly INoteRepository _repo;
        private readonly IOpponentRepository _opponentRepo;

        private readonly INavigationService _nav;
        public IDialogCloser? DialogCloser { get; set; }

        public int Id { get; }

        public int _opponentId;
        public int OpponentId { get => _opponentId; set { _opponentId = value; OnPropertyChanged(); } }

        public string _content;
        public string Content { get => _content; set { _content = value; OnPropertyChanged(); } }

        public string _type;
        public string Type { get => _type; set { _type = value; OnPropertyChanged(); } }

        public string _category;
        public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }

        public bool _isMarked;
        public bool IsMarked { get => _isMarked; set { _isMarked = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public Action? OnSaved { get; set; } 

        public NoteEditViewModel(INavigationService nav, INoteRepository repo, Note note)
        {
            _nav = nav;
            _repo = repo;

            _opponentId = note.OpponentId;

            if (note.Id != 0)
            {
                Id = note.Id;
                _content = note.Content;
                _category = note.Category;
                _type = note.Type;
            }

            SaveCommand = new RelayCommand(_ => Save());

        }

        private void Save()
        {
            if (Id == 0 || _repo.GetById(Id) == null)
            {
                _repo.Add(new Note { Content = _content, Category = _category.Replace("System.Windows.Controls.ComboBoxItem: ", ""), OpponentId = _opponentId, Type = _type.Replace("System.Windows.Controls.ComboBoxItem: ", ""), Marked = _isMarked });
            }
            else
            {
                _repo.Update(new Note { Id = Id, Content = _content, Category = _category.Replace("System.Windows.Controls.ComboBoxItem: ", ""), OpponentId = _opponentId, Type = _type.Replace("System.Windows.Controls.ComboBoxItem: ", ""), Marked = _isMarked });
            }

            OnSaved?.Invoke();

            DialogCloser?.Close(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
