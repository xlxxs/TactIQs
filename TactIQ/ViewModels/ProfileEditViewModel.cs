using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Miscellaneous.SQLite;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    public class ProfileEditViewModel : INotifyPropertyChanged
    {
        // Repositories für Matches und Notizen
        public readonly IOpponentRepository _opponentsRepo;
        public readonly IMatchRepository _matchRepo;
        public readonly INoteRepository _notesRepo;

        // NavigationService für die Navigation zwischen Views
        public readonly INavigationService _nav;

        // Eigenschaften für das Gegnerprofil
        public int Id { get; }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        private bool _marked;
        public bool Marked { get => _marked; set { _marked = value; OnPropertyChanged(); } }
        private string _club;
        public string Club { get => _club; set { _club = value; OnPropertyChanged(); } }

        // Ausgewählte Matches und Notizen
        private Match? _selectedMatch;
        public Match? SelectedMatch
        {
            get => _selectedMatch;
            set
            {
                if (_selectedMatch != value)
                {
                    _selectedMatch = value;
                    OnPropertyChanged();
                }
            }
        }
        private Note? _selectedNote;
        public Note? SelectedNote
        {
            get => _selectedNote;
            set
            {
                if (_selectedNote != value)
                {
                    _selectedNote = value;
                    OnPropertyChanged();
                }
            }
        }

        // Befehle für das Laden von Matches und Notizen sowie das Speichern des Profils
        public ICommand LoadMatchesCommand { get; }
        public ICommand LoadNotesCommand { get; }
        public ICommand SaveCommand { get; }

        // Listen für Matches und Notizen
        public ObservableCollection<Match> AllMatches { get; } = new();
        public ObservableCollection<Match> RecentMatches { get; } = new();
        public ObservableCollection<Note> AllNotes { get; } = new();
        public ObservableCollection<Note> RecentStrengths { get; } = new();
        public ObservableCollection<Note> RecentWeaknesses { get; } = new();
        public ObservableCollection<Note> RecentMisc { get; } = new();

        /// <summary>
        /// Konstruktor für das ProfileEditViewModel, der die Repositories initialisiert und die Daten eines Gegners lädt.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="repo"></param>
        /// <param name="opponent"></param>
        public ProfileEditViewModel(INavigationService nav,IOpponentRepository repo, Opponent opponent)
        {
            // Initialisierung der Repositories und NavigationService
            _opponentsRepo = repo;
            _nav = nav;

            // Initialisierung der Repositories für Matches und Notizen
            _matchRepo = new SqliteMatchRepository();
            _notesRepo = new SqliteNotesRepository();

            // Setzen der Eigenschaften des Gegners
            Id = opponent.Id;
            _name = opponent.Name;
            _marked = opponent.Marked;
            _club = opponent.Club;

            // Befehle initialisieren
            LoadMatchesCommand = new RelayCommand(_ => LoadMatches());
            LoadNotesCommand = new RelayCommand(_ => LoadNotes());
            SaveCommand = new RelayCommand(_ => Save());

            // Laden der Matches und Notizen für den Gegner
            LoadMatches();
            LoadNotes();
        }

        /// <summary>
        /// Methode zum Laden der Matches für den Gegner.
        /// </summary>
        public void LoadMatches()
        {
            AllMatches.Clear();

            // Alle Matches für den Gegner laden und in die ObservableCollection einfügen
            var matches = _matchRepo.GetAllForOpponent(Id);

            // Sortieren der Matches nach Markierung und Datum und nehmen der letzten 3 Matches
            foreach (var m in matches)
                AllMatches.Add(m);

            var recent = matches
                .OrderByDescending(m => m.Marked)
                .ThenByDescending(m => m.Date)
                .Take(3)
                .ToList();

            RecentMatches.Clear();

            // Einfügen der letzten 3 Matches in die ObservableCollection für die Anzeige
            foreach (var m in recent)
                RecentMatches.Add(m);
        }

        /// <summary>
        /// Methode zum Laden der Notizen für den Gegner.
        /// </summary>
        public void LoadNotes()
        {
            AllNotes.Clear();
            var notes = _notesRepo.GetAllForOpponent(Id);

            // Alle Notizen für den Gegner laden und in die ObservableCollection einfügen
            foreach (var n in notes)
                AllNotes.Add(n);

            // Sortieren der Notizen nach Markierung und Erstellungsdatum und nehmen der letzten 2 Notizen für jede Kategorie (Stärken, Schwächen, Sonstiges)
            var strengths = notes
                .Where(n => n.Type == "Stärke")
                .OrderByDescending(n => n.Marked)
                .ThenByDescending(n => n.CreatedAt)
                .Take(2);

            var weaknesses = notes
                .Where(n => n.Type == "Schwäche")
                .OrderByDescending(n => n.Marked)
                .ThenByDescending(n => n.CreatedAt)
                .Take(2);

            var misc = notes
                .Where(n => n.Type == "Sonstiges")
                .OrderByDescending(n => n.Marked)
                .ThenByDescending(n => n.CreatedAt)
                .Take(2);

            // Einfügen der letzten Notizen in die entsprechenden ObservableCollections
            RecentStrengths.Clear(); 
            foreach (var n in strengths) 
                RecentStrengths.Add(n);
            
            RecentWeaknesses.Clear();
            foreach (var n in weaknesses) 
                RecentWeaknesses.Add(n);

            RecentMisc.Clear();
            foreach (var n in misc) 
                RecentMisc.Add(n);
        }

        /// <summary>
        /// Methode zum Speichern der Änderungen am Gegnerprofil.
        /// </summary>
        private void Save()
        {
            // Unterscheidung zwischen Hinzufügen und Aktualisieren eines Gegners
            if (Id == 0 || _opponentsRepo.GetById(Id) == null)
            {
                _opponentsRepo.Add(Name, Club);
            }
            else
            {
                _opponentsRepo.Update(new Opponent { Id = Id, Club = Club, Marked = Marked, Name = Name});
            }

            _nav.NavigateTo(new OpponentProfilesViewModel(_nav, _opponentsRepo));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
