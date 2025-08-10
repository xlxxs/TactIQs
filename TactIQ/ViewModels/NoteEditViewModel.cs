using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    /// <summary>
    /// Klasse für das ViewModel zur Bearbeitung von Notizen in der Anwendung.
    /// </summary>
    public class NoteEditViewModel : INotifyPropertyChanged
    {
        // Repository für die Notizen
        private readonly INoteRepository _noteRepo;

        // DialogCloser für das Schließen des Dialogs nach dem Speichern
        public IDialogCloser? DialogCloser { get; set; }

        // Eigenschaften für die Notiz-Daten
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

        //ICommand für das Speichern der Notiz
        public ICommand SaveCommand { get; }

        // Aktion, die aufgerufen wird, wenn die Notiz erfolgreich gespeichert wurde
        public Action? OnSaved { get; set; }

        /// <summary>
        /// Konstruktor für das NoteEditViewModel, der die Repositories initialisiert und die Daten einer Notiz lädt.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="note"></param>
        public NoteEditViewModel(INoteRepository repo, Note note)
        {
            // Initialisiere das Notiz-Repository
            _noteRepo = repo;

            // Setze die übergebenen Werte für die Eigenschaften
            _opponentId = note.OpponentId;

            if (note.Id != 0)
            {
                _id = note.Id;
                _content = note.Content ?? String.Empty;
                _category = note.Category ?? String.Empty;
                _type = note.Type ?? String.Empty;
            }

            // SaveCommand initialisieren
            SaveCommand = new RelayCommand(_ => Save());
        }

        /// <summary>
        /// Methode zum Speichern der Notiz.
        /// </summary>
        private void Save()
        {

            // Unterscheidung, ob es sich um eine neue Notiz oder ein Update handelt
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
