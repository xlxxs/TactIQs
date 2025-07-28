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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TactIQ.Views
{
    /// <summary>
    /// Interaktionslogik für ProfileEditUC.xaml
    /// </summary>
    public partial class ProfileEditUC : UserControl
    {
        public ProfileEditUC()
        {
            InitializeComponent();
        }

        private void NewMatch_Click(object sender, RoutedEventArgs e)
        {
            var popup = new NewMatchWindow();
            popup.Owner = Window.GetWindow(this);
            if (popup.ShowDialog() == true)
            {
                var match = popup.NewMatch;
                MatchDataGrid.Items.Add(match);
            }
        }

        private void NewNote_Click(object sender, RoutedEventArgs e)
        {
            var popup = new NewNoteWindow();
            popup.Owner = Window.GetWindow(this);
            if (popup.ShowDialog() == true)
            {
                var note = popup.NewNote;
                NotesDataGrid.Items.Add(note);
            }
        }
    }
}
