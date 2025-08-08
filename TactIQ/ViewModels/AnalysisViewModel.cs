using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TactIQ.Model;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels;

public class AnalysisViewModel : INotifyPropertyChanged
{
    private readonly IMatchRepository _matchRepo;
    private readonly IOpponentRepository _opponentRepository;

    public ObservableCollection<Match> LastMatches { get; set; } = new();
    public ObservableCollection<Opponent> Opponents { get; set; }
    private List<Match> _allMatches;

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

    public PlotModel PieChartModel { get; set; } = new();
    public PlotModel LineChartModel { get; set; } = new();


    public AnalysisViewModel(IMatchRepository matchRepo, IOpponentRepository opponentRepo)
    {
        _matchRepo = matchRepo;
        _opponentRepository = opponentRepo;

        _allMatches = _matchRepo.GetAllMatches().OrderBy(m => m.Date ?? DateTime.MinValue).ToList();

        Opponents = new ObservableCollection<Opponent>(_opponentRepository.GetAll().OrderBy(o => o.Name));

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = _allMatches.Where(m =>
            (SelectedOpponent == null || m.OpponentId == SelectedOpponent.Id) &&
            (!_fromDate.HasValue || (m.Date ?? DateTime.MinValue) >= _fromDate.Value) &&
            (!_toDate.HasValue || (m.Date ?? DateTime.MinValue) <= _toDate.Value)
        ).ToList();

        LastMatches.Clear();
        foreach (var m in filtered.OrderByDescending(m => m.Date).Take(5))
            LastMatches.Add(m);

        UpdateCharts(filtered);
    }

    private void UpdateCharts(List<Match> matches)
    {
        // Pie Chart
        var wins = matches.Count(m => m.IsWin);
        var losses = matches.Count - wins;

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
        OnPropertyChanged(nameof(PieChartModel));

        // Line Chart
        var lineModel = new PlotModel { Title = "Siegquote im Zeitverlauf" };
        lineModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Siegquote", Minimum = 0, Maximum = 1 });
        lineModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Spielnummer" });

        var series = new LineSeries { MarkerType = MarkerType.Circle };
        int winCount = 0;
        for (int i = 0; i < matches.Count; i++)
        {
            if (matches[i].IsWin) winCount++;
            double ratio = (double)winCount / (i + 1);
            series.Points.Add(new DataPoint(i + 1, ratio));
        }
        lineModel.Series.Add(series);
        LineChartModel = lineModel;
        OnPropertyChanged(nameof(LineChartModel));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
