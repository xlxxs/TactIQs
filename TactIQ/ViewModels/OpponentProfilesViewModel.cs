using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Abstractions;

namespace TactIQ.ViewModels
{
    public class OpponentProfilesViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _nav;
        private readonly IOpponentRepository _repo;

        public ObservableCollection<Opponent> Opponents { get; } = new();

        private Opponent? _selectedOpponent;
        public Opponent? SelectedOpponent
        {
            get => _selectedOpponent;
            set { if (_selectedOpponent != value) { _selectedOpponent = value; OnPropertyChanged(); } }
        }

        public ICommand LoadOpponentsCommand { get; }
        public ICommand AddOpponentCommand { get; }
        public ICommand DeleteOpponentCommand { get; }
        public ICommand OpenSelectedCommand { get; }

        public OpponentProfilesViewModel(INavigationService nav, IOpponentRepository repo)
        {
            _nav = nav;
            _repo = repo;

            LoadOpponentsCommand = new RelayCommand(_ => LoadOpponents());
            AddOpponentCommand = new RelayCommand(name => AddOpponent(name as string));
            DeleteOpponentCommand = new RelayCommand(op => { if (op is Opponent o) DeleteOpponent(o); });
            OpenSelectedCommand = new RelayCommand(_ => OpenSelected(), _ => SelectedOpponent != null);

            LoadOpponents();
        }

        public void LoadOpponents()
        {
            Opponents.Clear();
            foreach (var o in _repo.GetAll())
                Opponents.Add(o);
        }

        public void AddOpponent(string? name)
        {
            if (string.IsNullOrWhiteSpace(name)) return;
            _repo.Add(name);
            LoadOpponents();
        }

        public void DeleteOpponent(Opponent? opponent)
        {
            if (opponent == null) return;
            _repo.Delete(opponent.Id);
            LoadOpponents();
        }

        private void OpenSelected()
        {
            if (SelectedOpponent == null) return;
            var vm = new ProfileEditViewModel(_repo, SelectedOpponent);
            _nav.NavigateTo(vm);
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
