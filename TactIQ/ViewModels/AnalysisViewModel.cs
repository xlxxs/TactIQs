using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels;

/// <summary>
/// ViewModel für die Analyse-Ansicht.
/// </summary>
public class AnalysisViewModel : INotifyPropertyChanged
{
    // Repositories
    private readonly IMatchRepository _matchRepo;
    private readonly IOpponentRepository _opponentRepository;

    // Liste der letzten Matches und Gegner
    public ObservableCollection<Match> LastMatches { get; set; } = new();
    public ObservableCollection<Opponent> Opponents { get; set; }
    private List<Match> _allMatches;

    // Ausgewählte Eigenschaften für Filterung
    private Opponent? _selectedOpponent;
    public Opponent? SelectedOpponent
    {
        get => _selectedOpponent;
        set
        {
            _selectedOpponent = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    private DateTime? _fromDate;
    public DateTime? FromDate
    {
        get => _fromDate;
        set
        {
            _fromDate = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    private DateTime? _toDate;
    public DateTime? ToDate
    {
        get => _toDate;
        set
        {
            _toDate = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    // Diagramme für die Analyse
    public PlotModel PieChartModel { get; set; } = new();
    public PlotModel LineChartModel { get; set; } = new();

    /// <summary>
    /// Konstruktor für das AnalysisViewModel, der die Repositories initialisiert und die Daten lädt.
    /// </summary>
    /// <param name="matchRepo"></param>
    /// <param name="opponentRepo"></param>
    public AnalysisViewModel(IMatchRepository matchRepo, IOpponentRepository opponentRepo)
    {
        // Repositories initialisieren
        _matchRepo = matchRepo;
        _opponentRepository = opponentRepo;

        // Alle Matches und Gegner laden
        _allMatches = _matchRepo.GetAllMatches().OrderBy(m => m.Date ?? DateTime.MinValue).ToList();

        // Letzte Matches initialisieren
        Opponents = new ObservableCollection<Opponent>(_opponentRepository.GetAll().OrderBy(o => o.Name));

        // Filter anwenden, um die letzten 5 Matches anzuzeigen
        ApplyFilter();
    }

    /// <summary>
    /// Methode zum Anwenden des Filters auf die Matches basierend auf dem ausgewählten Gegner und den Datumsbereichen.
    /// </summary>
    private void ApplyFilter()
    {
        // Filter Matches basierend auf dem ausgewählten Gegner und den Datumsbereichen
        var filtered = _allMatches.Where(m =>
            (SelectedOpponent == null || m.OpponentId == SelectedOpponent.Id) &&
            (!_fromDate.HasValue || (m.Date ?? DateTime.MinValue) >= _fromDate.Value) &&
            (!_toDate.HasValue || (m.Date ?? DateTime.MinValue) <= _toDate.Value)
        ).ToList();

        // Aktualisiere die ObservableCollection mit den gefilterten Matches
        LastMatches.Clear();
        foreach (var m in filtered.OrderByDescending(m => m.Date).Take(5))
            LastMatches.Add(m);

        // Aktualisiere die Diagramme mit den gefilterten Matches
        UpdateCharts(filtered);
    }

    /// <summary>
    /// Methode zum Aktualisieren der Diagramme basierend auf den gefilterten Matches.
    /// </summary>
    /// <param name="matches"></param>
    private void UpdateCharts(List<Match> matches)
    {
        // Pie Chart
        // Zähle Siege und Niederlagen
        var wins = matches.Count(m => m.IsWin);
        var losses = matches.Count - wins;

        // Erstelle das Pie Chart
        var pieModel = new PlotModel { Title = "Siege vs. Niederlagen" };
        var pieSeries = new PieSeries
        {
            StrokeThickness = 1.0,
            InsideLabelPosition = 0.8,
            AngleSpan = 360,
            StartAngle = 0
        };
        pieSeries.Slices.Add(new PieSlice("Siege", wins) { Fill = OxyColors.Green });
        pieSeries.Slices.Add(new PieSlice("Niederlagen", losses) { Fill = OxyColors.Red });
        pieModel.Series.Add(pieSeries);
        PieChartModel = pieModel;

        // Aktualisiere die PropertyChanged-Ereignisse für die Diagramme
        OnPropertyChanged(nameof(PieChartModel));

        // Line Chart
        // Erstelle das Line Chart für die Siegquote im Zeitverlauf
        var lineModel = new PlotModel { Title = "Siegquote im Zeitverlauf" };
        lineModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Siegquote", Minimum = 0, Maximum = 1 });
        lineModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Spielnummer" });

        var series = new LineSeries { MarkerType = MarkerType.Circle };

        // Berechne die Siegquote im Zeitverlauf
        int winCount = 0;
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].IsWin) winCount++;
            double ratio = (double)winCount / (i + 1);
            series.Points.Add(new DataPoint(i + 1, ratio));
        }
        lineModel.Series.Add(series);
        LineChartModel = lineModel;

        // Aktualisiere die PropertyChanged-Ereignisse für das Line Chart
        OnPropertyChanged(nameof(LineChartModel));
    }

    /// <summary>
    /// Ereignis, das ausgelöst wird, wenn sich eine Eigenschaft ändert.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Methode zum Auslösen des PropertyChanged-Ereignisses.
    /// </summary>
    /// <param name="name"></param>
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
