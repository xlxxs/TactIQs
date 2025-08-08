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
        private readonly INoteRepository _noteRepo;

        public IDialogCloser? DialogCloser { get; set; }

        private int _id { get; }

        private int _opponentId;
        public int OpponentId { get => _opponentId; set { _opponentId = value; OnPropertyChanged(); } }

        private string _content;
        public string Content { get => _content; set { _content = value; OnPropertyChanged(); } }

        private string _type;
        public string Type { get => _type; set { _type = value; OnPropertyChanged(); } }

        private string _category;
        public string Category { get => _category; set { _category = value; OnPropertyChanged(); } }

        private bool _isMarked;
        public bool IsMarked { get => _isMarked; set { _isMarked = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public Action? OnSaved { get; set; } 

        public NoteEditViewModel(INoteRepository repo, Note note)
        {
            _noteRepo = repo;

            _opponentId = note.OpponentId;

            if (note.Id != 0)
            {
                _id = note.Id;
                _content = note.Content ?? String.Empty;
                _category = note.Category ?? String.Empty;
                _type = note.Type ?? String.Empty;
            }

            SaveCommand = new RelayCommand(_ => Save());

        }

        private void Save()
        {
            if (_id == 0 || _noteRepo.GetById(_id) == null)
            {
                _noteRepo.Add(new Note { Content = _content, Category = _category.Replace("System.Windows.Controls.ComboBoxItem: ", ""), OpponentId = _opponentId, Type = _type.Replace("System.Windows.Controls.ComboBoxItem: ", ""), Marked = _isMarked });
            }
            else
            {
                _noteRepo.Update(new Note { Id = _id, Content = _content, Category = _category.Replace("System.Windows.Controls.ComboBoxItem: ", ""), OpponentId = _opponentId, Type = _type.Replace("System.Windows.Controls.ComboBoxItem: ", ""), Marked = _isMarked });
            }

            OnSaved?.Invoke();

            DialogCloser?.Close(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
