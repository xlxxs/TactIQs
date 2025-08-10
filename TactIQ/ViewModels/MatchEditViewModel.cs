using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using TactIQ.Miscellaneous;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    /// <summary>
    /// ViewModel für die Bearbeitung von Matches in der Anwendung.
    /// </summary>
    public class MatchEditViewModel: INotifyPropertyChanged
    {
        // Repository für die Matches
        private readonly IMatchRepository _matchRepo;
        
        // DialogCloser für das Schließen des Dialogs nach dem Speichern
        public IDialogCloser? DialogCloser { get; set; }

        // Eigenschaften für die Match-Daten
        private int _id;
        private DateTime _date;
        public DateTime Date { get => _date; set { _date = value; OnPropertyChanged(); } }

        private bool _isWin;
        public bool IsWin { get => _isWin; set { _isWin = value; OnPropertyChanged(); } }

        private bool _isMarked;
        public bool Marked { get => _isMarked; set { _isMarked = value; OnPropertyChanged(); } }

        private int _opponentId;
        public int OpponentId { get => _opponentId; set { _opponentId = value; OnPropertyChanged(); } }

        private string _competition;
        public string Competition { get => _competition; set { _competition = value; OnPropertyChanged(); } }

        private string _result;
        public string Result { get => _result; set { _result = value; _isWin = ParseResultToIsWin(_result); OnPropertyChanged(); OnPropertyChanged(nameof(IsWin)); } }
        private string _notes;
        public string Notes { get => _notes; set { _notes = value; OnPropertyChanged(); } }

        // ICommand für das Speichern des Matches   
        public ICommand SaveCommand { get; }

        // Aktion, die aufgerufen wird, wenn das Match erfolgreich gespeichert wurde
        public Action? OnSaved { get; set; }

        /// <summary>
        /// Konstruktor für das MatchEditViewModel, der die Repositories initialisiert und die Daten eines Matches lädt.
        /// </summary>
        /// <param name="repo"></param>
        /// <param name="match"></param>
        public MatchEditViewModel(IMatchRepository repo, Match match)
        {
            // Initialisiere das Match-Repository
            _matchRepo = repo;

            // Setze die Standardwerte für die Eigenschaften
            _date = match.Date.HasValue ? match.Date.Value : DateTime.Now;
            _opponentId = match.OpponentId;

            // Setze die Eigenschaften basierend auf dem übergebenen Match
            if (match.Id != 0)
            {
                _id = match.Id;
                _competition = match.Competition;
                _isWin = match.IsWin;
                _result = match.Result;
                _notes = match.Notes;
                _isMarked = match.Marked;
            }

            // Setze die ICommand für das Speichern des Matches
            SaveCommand = new RelayCommand(_ => Save());
        }

        /// <summary>
        /// Methode zum Parsen des Ergebnisses in ein boolesches Ergebnis, ob das Match gewonnen oder verloren wurde.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ParseResultToIsWin(string result)
        {
            // Überprüfe, ob das Ergebnis leer oder null ist
            if (string.IsNullOrWhiteSpace(result)) 
                return false;

            // Teile das Ergebnis in zwei Teile (eigene Punkte und Punkte des Gegners)
            var parts = result.Split(':');
            if (parts.Length != 2) return false;

            if (int.TryParse(parts[0].Trim(), out int ownScore) && int.TryParse(parts[1].Trim(), out int opponentScore))
            {
                // Vergleiche die Punkte, um zu bestimmen, ob das Match gewonnen wurde
                return ownScore > opponentScore;
            }

            return false;
        }

        /// <summary>
        /// Methode zur Validierung des Formats des Ergebnisses. (Format 3:1, 1:3 usw.)
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool IsValidResultFormat(string result)
        {
            // Überprüfe, ob das Ergebnis leer oder null ist
            if (string.IsNullOrWhiteSpace(result)) 
                return false;

            var parts = result.Split(':');

            // Überprüfe, ob das Ergebnis genau zwei Teile hat (eigene Punkte und Punkte des Gegners)
            if (parts.Length != 2) 
                return false;

            // Überprüfe, ob beide Teile gültige Ganzzahlen sind
            return int.TryParse(parts[0].Trim(), out _) && int.TryParse(parts[1].Trim(), out _);
        }

        /// <summary>
        /// Methode zum Speichern des Matches. Validiert das Ergebnisformat und speichert das Match in der Datenbank.
        /// </summary>
        private void Save()
        {
            // Überprüfe, ob das Ergebnis im gültigen Format vorliegt
            if (!IsValidResultFormat(_result))
            {
                MessageBox.Show("Bitte gib das Ergebnis im Format X:Y ('3:2', '1:3', ...) ein.",
                                "Ungültiges Format",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            //Unterscheidung, ob es sich um ein neues Match oder ein Update handelt
            if (_id == 0 || _matchRepo.GetById(_id) == null)
            {
                _matchRepo.Add(new Match { Competition = _competition, Date = _date, IsWin = _isWin, Notes = _notes, OpponentId = _opponentId, Result = _result, Marked = _isMarked });
            }
            else
            {
                _matchRepo.Update(new Match { Id = _id, Competition = _competition, Date = _date, IsWin = _isWin, Notes = _notes, OpponentId = _opponentId, Result = _result });
            }

            // Rufe die OnSaved-Aktion auf, dafür dass das Match erfolgreich gespeichert wurde
            OnSaved?.Invoke();

            // Schließe den Dialog
            DialogCloser?.Close(null);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
