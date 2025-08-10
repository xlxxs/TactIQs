using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    /// <summary>
    /// Klasse für das ViewModel zur Verwaltung von Gegnerprofilen in der Anwendung.
    /// </summary>
    public class OpponentProfilesViewModel : INotifyPropertyChanged
    {
        // NavigationService und Repository für Gegnerprofile
        private readonly INavigationService _nav;

        // Repository für die Gegnerprofile
        private readonly IOpponentRepository _opponentRepo;

        // Listen für alle Gegner und gefilterte Gegner
        public ObservableCollection<Opponent> AllOpponents { get; } = new();
        public ObservableCollection<Opponent> FilteredOpponents { get; } = new();

        // Suchtext und Markierungsfilter
        private string _searchTerm = "";
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm != value)
                {
                    _searchTerm = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        private bool _onlyMarked;
        public bool OnlyMarked
        {
            get => _onlyMarked;
            set
            {
                if (_onlyMarked != value)
                {
                    _onlyMarked = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        // Ausgewählter Gegner
        private Opponent? _selectedOpponent;
        public Opponent? SelectedOpponent
        {
            get => _selectedOpponent;
            set
            {
                if (_selectedOpponent != value)
                {
                    _selectedOpponent = value;
                    OnPropertyChanged();
                }
            }
        }

        // Befehle
        public ICommand LoadOpponentsCommand { get; }
        public ICommand AddOpponentCommand { get; }
        public ICommand DeleteOpponentCommand { get; }
        public ICommand OpenSelectedCommand { get; }

        /// <summary>
        /// Konstruktor für das OpponentProfilesViewModel, der die Repositories initialisiert und die Befehle anlegt.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="opponentRepo"></param>
        public OpponentProfilesViewModel(INavigationService nav, IOpponentRepository opponentRepo)
        {
            // Initialisierung der Navigation und des Repositories
            _nav = nav;
            _opponentRepo = opponentRepo;

            // Befehle initialisieren
            LoadOpponentsCommand = new RelayCommand(_ => LoadOpponents());
            AddOpponentCommand = new RelayCommand(name => AddOpponent(name as string));
            DeleteOpponentCommand = new RelayCommand(op => { if (op is Opponent o) DeleteOpponent(o); });
            OpenSelectedCommand = new RelayCommand(_ => OpenSelected(), _ => SelectedOpponent != null);

            LoadOpponents();
        }

        /// <summary>
        /// Methode zum Laden aller Gegnerprofile aus dem Repository und Anwenden des Filters.
        /// </summary>
        public void LoadOpponents()
        {
            // Alle Gegnerprofile aus dem Repository laden und in die ObservableCollection einfügen
            AllOpponents.Clear();
            foreach (var o in _opponentRepo.GetAll())
                AllOpponents.Add(o);

            ApplyFilter();
        }

        /// <summary>
        /// Methode zum Anwenden des Filters auf die Gegnerprofile basierend auf dem Suchbegriff und der Markierung.
        /// </summary>
        public void ApplyFilter()
        {
            FilteredOpponents.Clear();

            var filtered = AllOpponents.AsEnumerable();

            // Filter anwenden: nach Name und Markierung
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                filtered = filtered.Where(o => o.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            if (OnlyMarked)
                filtered = filtered.Where(o => o.Marked);

            // Gefilterte Gegner in die ObservableCollection einfügen
            foreach (var o in filtered)
                FilteredOpponents.Add(o);
        }

        /// <summary>
        /// Methode zum Hinzufügen eines neuen Gegners anhand des Namens.
        /// </summary>
        /// <param name="name"></param>
        public void AddOpponent(string? name)
        {
            // Überprüfen, ob der Name leer oder null ist
            if (string.IsNullOrWhiteSpace(name)) 
                return;

            // Neuen Gegner erstellen und zur Profilbearbeitung navigieren
            var vm = new ProfileEditViewModel(_nav, _opponentRepo, new Opponent { Name = name });
            _nav.NavigateTo(vm);

            // Nach dem Hinzufügen den Suchbegriff zurücksetzen und die Gegner neu laden
            LoadOpponents();
        }

        /// <summary>
        /// Methode zum Löschen eines Gegners anhand des übergebenen Opponent-Objekts.
        /// </summary>
        /// <param name="opponent"></param>
        public void DeleteOpponent(Opponent? opponent)
        {
            if (opponent == null) 
                return;

            // Gegner aus dem Repository löschen
            _opponentRepo.Delete(opponent.Id);

            // Nach dem Löschen den Suchbegriff zurücksetzen und die Gegner neu laden
            LoadOpponents();
        }

        /// <summary>
        /// Methode zum Öffnen des ausgewählten Gegners in der Profilbearbeitung.
        /// </summary>
        private void OpenSelected()
        {
            if (SelectedOpponent == null) 
                return;

            // Navigiere zur Profilbearbeitung des ausgewählten Gegners
            var vm = new ProfileEditViewModel(_nav, _opponentRepo, SelectedOpponent);
            _nav.NavigateTo(vm);

            SelectedOpponent = null;
            CommandManager.InvalidateRequerySuggested();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
