using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using TactIQ.Model;
using TactIQ.Miscellaneous;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.ViewModels
{
    public class ExportViewModel : INotifyPropertyChanged
    {
        public string? SelectedOpponent { get; set; }

        public bool ExportNotes { get; set; } = true;
        public bool ExportMatches { get; set; } = true;
        public bool OnlyMarked { get; set; } = false;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        private readonly IEnumerable<Note> _notes;
        private readonly IEnumerable<Match> _matches;
        private readonly Dictionary<int, Opponent> _opponentLookup;

        private IOpponentRepository _opponentRepo;
        private IMatchRepository _matchRepo;
        private INoteRepository _noteRepo;
        public ICommand ExportCommand { get; }

        public ExportViewModel(IOpponentRepository opponentRepo, IMatchRepository matchRepo, INoteRepository noteRepo)
        {
            _opponentRepo = opponentRepo;
            _matchRepo = matchRepo;
            _noteRepo = noteRepo;

            _notes = _noteRepo.GetAllNotes();
            _matches = _matchRepo.GetAllMatches();

            _opponentLookup = _opponentRepo.GetAll()
                .ToDictionary(o => o.Id);

            ExportCommand = new RelayCommand(_ => ExportToExcel());
        }

        private void ExportToExcel()
        {
            SaveFileDialog saveDialog = new SaveFileDialog
            {
                Filter = "Excel Workbook|*.xlsx",
                FileName = "TactIQ_Export"
            };

            if (saveDialog.ShowDialog() != true)
                return;

            using var workbook = new XLWorkbook();

            if (ExportMatches)
                ExportMatchesToSheet(workbook);

            if (ExportNotes)
                ExportNotesToSheet(workbook);

            workbook.SaveAs(saveDialog.FileName);
            MessageBox.Show("Export erfolgreich!", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ExportMatchesToSheet(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Matches");
            ws.Cell(1, 1).Value = "Datum";
            ws.Cell(1, 2).Value = "Gegner";
            ws.Cell(1, 3).Value = "Verein";
            ws.Cell(1, 4).Value = "Wettkampf";
            ws.Cell(1, 5).Value = "Ergebnis";
            ws.Cell(1, 6).Value = "Sieg";

            var filtered = _matches.Where(m =>
            {
                if (!_opponentLookup.TryGetValue(m.OpponentId, out var opp))
                    return false;

                var fullName = $"{opp.Name} ({opp.Club})";

                return (string.IsNullOrEmpty(SelectedOpponent) || fullName == SelectedOpponent)
                    && (!OnlyMarked || m.Marked)
                    && (!FromDate.HasValue || m.Date >= FromDate)
                    && (!ToDate.HasValue || m.Date <= ToDate);
            }).ToList();

            for (int i = 0; i < filtered.Count; i++)
            {
                var m = filtered[i];
                var opp = _opponentLookup[m.OpponentId];
                int row = i + 2;

                ws.Cell(row, 1).Value = m.Date?.ToString("dd.MM.yyyy");
                ws.Cell(row, 2).Value = opp.Name;
                ws.Cell(row, 3).Value = opp.Club;
                ws.Cell(row, 4).Value = m.Competition;
                ws.Cell(row, 5).Value = m.Result;
                ws.Cell(row, 6).Value = m.IsWin ? "Ja" : "Nein";

                if (m.Marked)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.LightPink;
            }

            // Formatierung
            int lastRow = filtered.Count + 1;
            var headerRange = ws.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var dataRange = ws.Range(1, 1, lastRow, 6);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dataRange.Style.Alignment.WrapText = true;

            ws.Columns().AdjustToContents();
        }

        private void ExportNotesToSheet(XLWorkbook workbook)
        {
            var ws = workbook.Worksheets.Add("Notizen");
            ws.Cell(1, 1).Value = "Datum";
            ws.Cell(1, 2).Value = "Gegner";
            ws.Cell(1, 3).Value = "Verein";
            ws.Cell(1, 4).Value = "Kategorie";
            ws.Cell(1, 5).Value = "Typ";
            ws.Cell(1, 6).Value = "Inhalt";

            var filtered = _notes.Where(n =>
            {
                if (!_opponentLookup.TryGetValue(n.OpponentId, out var opp))
                    return false;

                var fullName = $"{opp.Name} ({opp.Club})";

                return (string.IsNullOrEmpty(SelectedOpponent) || fullName == SelectedOpponent)
                    && (!OnlyMarked || n.Marked)
                    && (!FromDate.HasValue || n.CreatedAt >= FromDate)
                    && (!ToDate.HasValue || n.CreatedAt <= ToDate);
            }).ToList();

            for (int i = 0; i < filtered.Count; i++)
            {
                var n = filtered[i];
                var opp = _opponentLookup[n.OpponentId];
                int row = i + 2;

                ws.Cell(row, 1).Value = n.CreatedAt.ToString("dd.MM.yyyy");
                ws.Cell(row, 2).Value = opp.Name;
                ws.Cell(row, 3).Value = opp.Club;
                ws.Cell(row, 4).Value = n.Category;
                ws.Cell(row, 5).Value = n.Type;
                ws.Cell(row, 6).Value = n.Content;

                if (n.Marked)
                    ws.Row(row).Style.Fill.BackgroundColor = XLColor.LightPink;
            }

            // Formatierung
            int lastRow = filtered.Count + 1;
            var headerRange = ws.Range(1, 1, 1, 6);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            var dataRange = ws.Range(1, 1, lastRow, 6);
            dataRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            dataRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            dataRange.Style.Alignment.WrapText = true;

            ws.Columns().AdjustToContents();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
