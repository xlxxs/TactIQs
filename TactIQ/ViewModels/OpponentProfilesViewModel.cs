using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    public class OpponentProfilesViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _nav;
        private readonly IOpponentRepository _opponentRepo;
        private readonly IMatchRepository _matchRepo;

        public ObservableCollection<Opponent> AllOpponents { get; } = new();
        public ObservableCollection<Opponent> FilteredOpponents { get; } = new();

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

        public ICommand LoadOpponentsCommand { get; }
        public ICommand AddOpponentCommand { get; }
        public ICommand DeleteOpponentCommand { get; }
        public ICommand OpenSelectedCommand { get; }

        public OpponentProfilesViewModel(INavigationService nav, IOpponentRepository opponentRepo)
        {
            _nav = nav;
            _opponentRepo = opponentRepo;

            LoadOpponentsCommand = new RelayCommand(_ => LoadOpponents());
            AddOpponentCommand = new RelayCommand(name => AddOpponent(name as string));
            DeleteOpponentCommand = new RelayCommand(op => { if (op is Opponent o) DeleteOpponent(o); });
            OpenSelectedCommand = new RelayCommand(_ => OpenSelected(), _ => SelectedOpponent != null);

            LoadOpponents();
        }

        public void LoadOpponents()
        {
            AllOpponents.Clear();
            foreach (var o in _opponentRepo.GetAll())
                AllOpponents.Add(o);

            ApplyFilter();
        }

        public void ApplyFilter()
        {
            FilteredOpponents.Clear();

            var filtered = AllOpponents.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
                filtered = filtered.Where(o => o.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

            if (OnlyMarked)
                filtered = filtered.Where(o => o.Marked);

            foreach (var o in filtered)
                FilteredOpponents.Add(o);
        }

        public void AddOpponent(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;

            var vm = new ProfileEditViewModel(_nav, _opponentRepo, new Opponent { Name = name });
            _nav.NavigateTo(vm);

            LoadOpponents();
        }

        public void DeleteOpponent(Opponent? opponent)
        {
            if (opponent == null) return;
            _opponentRepo.Delete(opponent.Id);
            LoadOpponents();
        }

        private void OpenSelected()
        {
            if (SelectedOpponent == null) return;
            var vm = new ProfileEditViewModel(_nav, _opponentRepo, SelectedOpponent);
            _nav.NavigateTo(vm);

            SelectedOpponent = null;
            CommandManager.InvalidateRequerySuggested();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }


    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Predicate<object?>? _canExecute;
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        { _execute = execute; _canExecute = canExecute; }
        public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object? parameter) => _execute(parameter);
        public event EventHandler? CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
}
