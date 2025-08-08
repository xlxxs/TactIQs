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
        public readonly IOpponentRepository _opponentsRepo;
        public readonly IMatchRepository _matchRepo;
        public readonly INoteRepository _notesRepo;

        public readonly INavigationService _nav;

        public int Id { get; }
        private string _name;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private bool _marked;
        public bool Marked { get => _marked; set { _marked = value; OnPropertyChanged(); } }

        private string _club;
        public string Club { get => _club; set { _club = value; OnPropertyChanged(); } }

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
        public ICommand LoadMatchesCommand { get; }
        public ICommand LoadNotesCommand { get; }
        public ICommand SaveCommand { get; }
        public ObservableCollection<Match> AllMatches { get; } = new();
        public ObservableCollection<Match> RecentMatches { get; } = new();

        public ObservableCollection<Note> AllNotes { get; } = new();
        public ObservableCollection<Note> RecentStrengths { get; } = new();
        public ObservableCollection<Note> RecentWeaknesses { get; } = new();
        public ObservableCollection<Note> RecentMisc { get; } = new();

        public ProfileEditViewModel(INavigationService nav,IOpponentRepository repo, Opponent opponent)
        {
            _opponentsRepo = repo;
            _nav = nav;

            _matchRepo = new SqliteMatchRepository();
            _notesRepo = new SqliteNotesRepository();

            Id = opponent.Id;
            _name = opponent.Name;
            _marked = opponent.Marked;
            _club = opponent.Club;

            LoadMatchesCommand = new RelayCommand(_ => LoadMatches());
            LoadNotesCommand = new RelayCommand(_ => LoadNotes());

            SaveCommand = new RelayCommand(_ => Save());

            LoadMatches();
            LoadNotes();
        }

        public void LoadMatches()
        {
            AllMatches.Clear();
            var matches = _matchRepo.GetAllForOpponent(Id);

            foreach (var m in matches)
                AllMatches.Add(m);

            var recent = matches
                .OrderByDescending(m => m.Marked)
                .ThenByDescending(m => m.Date)
                .Take(3)
                .ToList();

            RecentMatches.Clear();
            foreach (var m in recent)
                RecentMatches.Add(m);
        }
        public void LoadNotes()
        {
            AllNotes.Clear();
            var notes = _notesRepo.GetAllForOpponent(Id);

            foreach (var n in notes)
                AllNotes.Add(n);

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

            RecentStrengths.Clear(); foreach (var n in strengths) RecentStrengths.Add(n);
            RecentWeaknesses.Clear(); foreach (var n in weaknesses) RecentWeaknesses.Add(n);
            RecentMisc.Clear(); foreach (var n in misc) RecentMisc.Add(n);
        }
        private void Save()
        {
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
