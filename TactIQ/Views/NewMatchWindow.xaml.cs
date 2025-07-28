using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TactIQ.Model;

namespace TactIQ.Views
{
    /// <summary>
    /// Interaktionslogik für NewMatchWindow.xaml
    /// </summary>
    public partial class NewMatchWindow : Window
    {
        public Match NewMatch { get; private set; }

        public NewMatchWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            NewMatch = new Match
            {
                Date = MatchDatePicker.SelectedDate?.ToString("dd.MM.yyyy"),
                Result = ResultBox.Text,
                Competition = CompetitionBox.Text,
                Notes = NotesBox.Text
            };
            DialogResult = true;
            Close();
        }
    }
}
