using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    public class MatchEditViewModel: INotifyPropertyChanged
    {
        private readonly IMatchRepository _repo;
        private readonly IOpponentRepository _opponentRepo;

        private readonly INavigationService _nav;
        public IDialogCloser? DialogCloser { get; set; }

        public int Id { get; }

        private DateTime _date;
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }

        private bool _isWin;
        public bool IsWin { get => _isWin; set { _isWin = value; OnPropertyChanged(); } }
        private bool _marked;
        public bool Marked { get => _marked; set { _marked = value; OnPropertyChanged(); } }

        private int _opponentId;
        public int OpponentId { get => _opponentId; set { _opponentId = value; OnPropertyChanged(); } }

        private string _competition;
        public string Competition { get => _competition; set { _competition = value; OnPropertyChanged(); } }

        private string _result;
        public string Result { get => _result; set { _result = value; _isWin = ParseResultToIsWin(_result); OnPropertyChanged(); OnPropertyChanged(nameof(IsWin)); } }

        private string _notes;
        public string Notes { get => _notes; set { _notes = value; OnPropertyChanged(); } }
        public ICommand SaveCommand { get; }
        public Action? OnSaved { get; set; }

        public MatchEditViewModel(INavigationService nav, IMatchRepository repo, Match match)
        {
            _nav = nav;
            _repo = repo;
            
            _date = match.Date.HasValue ? match.Date.Value : DateTime.Now;
            _opponentId = match.OpponentId;

            if(match.Id != 0)
            {
                Id = match.Id;
                _competition = match.Competition;
                _isWin = match.IsWin;
                _result = match.Result;
                _notes = match.Notes;
                _marked = match.Marked;
            }

            SaveCommand = new RelayCommand(_ => Save());

        }
        private bool ParseResultToIsWin(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return false;

            var parts = result.Split(':');
            if (parts.Length != 2) return false;

            if (int.TryParse(parts[0].Trim(), out int ownScore) && int.TryParse(parts[1].Trim(), out int opponentScore))
            {
                return ownScore > opponentScore;
            }

            return false;
        }

        private bool IsValidResultFormat(string result)
        {
            if (string.IsNullOrWhiteSpace(result)) return false;

            var parts = result.Split(':');
            if (parts.Length != 2) return false;

            return int.TryParse(parts[0].Trim(), out _) && int.TryParse(parts[1].Trim(), out _);
        }


        private void Save()
        {
            if (!IsValidResultFormat(_result))
            {
                MessageBox.Show("Bitte gib das Ergebnis im Format X:Y ('3:2', '1:3', ...) ein.",
                                "Ungültiges Format",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            if (Id == 0 || _repo.GetById(Id) == null)
            {
                _repo.Add(new Match { Competition = _competition, Date = _date, IsWin = _isWin, Notes = _notes, OpponentId = _opponentId, Result = _result, Marked = _marked });
            }
            else
            {
                _repo.Update(new Match { Id = Id, Competition = _competition, Date = _date, IsWin = _isWin, Notes = _notes, OpponentId = _opponentId, Result = _result });
            }

            OnSaved?.Invoke();

            DialogCloser?.Close(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
