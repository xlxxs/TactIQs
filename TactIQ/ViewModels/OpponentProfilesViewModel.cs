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
    /// Klasse f�r das ViewModel zur Verwaltung von Gegnerprofilen in der Anwendung.
    /// </summary>
    public class OpponentProfilesViewModel : INotifyPropertyChanged
    {
        // NavigationService und Repository f�r Gegnerprofile
        private readonly INavigationService _nav;

        // Repository f�r die Gegnerprofile
        private readonly IOpponentRepository _opponentRepo;

        // Listen f�r alle Gegner und gefilterte Gegner
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

        // Ausgew�hlter Gegner
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
        /// Konstruktor f�r das OpponentProfilesViewModel, der die Repositories initialisiert und die Befehle anlegt.
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
            // Alle Gegnerprofile aus dem Repository laden und in die ObservableCollection einf�gen
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

            // Gefilterte Gegner in die ObservableCollection einf�gen
            foreach (var o in filtered)
                FilteredOpponents.Add(o);
        }

        /// <summary>
        /// Methode zum Hinzuf�gen eines neuen Gegners anhand des Namens.
        /// </summary>
        /// <param name="name"></param>
        public void AddOpponent(string? name)
        {
            // �berpr�fen, ob der Name leer oder null ist
            if (string.IsNullOrWhiteSpace(name)) 
                return;

            // Neuen Gegner erstellen und zur Profilbearbeitung navigieren
            var vm = new ProfileEditViewModel(_nav, _opponentRepo, new Opponent { Name = name });
            _nav.NavigateTo(vm);

            // Nach dem Hinzuf�gen den Suchbegriff zur�cksetzen und die Gegner neu laden
            LoadOpponents();
        }

        /// <summary>
        /// Methode zum L�schen eines Gegners anhand des �bergebenen Opponent-Objekts.
        /// </summary>
        /// <param name="opponent"></param>
        public void DeleteOpponent(Opponent? opponent)
        {
            if (opponent == null) 
                return;

            // Gegner aus dem Repository l�schen
            _opponentRepo.Delete(opponent.Id);

            // Nach dem L�schen den Suchbegriff zur�cksetzen und die Gegner neu laden
            LoadOpponents();
        }

        /// <summary>
        /// Methode zum �ffnen des ausgew�hlten Gegners in der Profilbearbeitung.
        /// </summary>
        private void OpenSelected()
        {
            if (SelectedOpponent == null) 
                return;

            // Navigiere zur Profilbearbeitung des ausgew�hlten Gegners
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
