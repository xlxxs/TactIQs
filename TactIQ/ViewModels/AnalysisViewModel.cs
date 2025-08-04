using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Collections.ObjectModel;
using static TactIQ.Miscellaneous.Interfaces;
using TactIQ.Model;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TactIQ.ViewModels;
public class AnalysisViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Match> LastMatches { get; set; }
    public PlotModel PieChartModel { get; set; }
    public PlotModel LineChartModel { get; set; }

    public AnalysisViewModel(INavigationService nav, IMatchRepository matchRepo)
    {
        var matches = matchRepo.GetAllMatches().OrderBy(m => m.Date ?? DateTime.MinValue).ToList();

        LastMatches = new ObservableCollection<Match>(matches.OrderByDescending(m => m.Date).Take(5));

        // PieChart
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

        // LineChart
        var lineModel = new PlotModel { Title = "Siegquote im Zeitverlauf" };
        lineModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Siegquote" });
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
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
