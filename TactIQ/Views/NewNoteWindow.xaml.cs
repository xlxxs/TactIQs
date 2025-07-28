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
    /// Interaktionslogik für NewNoteWindow.xaml
    /// </summary>
    public partial class NewNoteWindow : Window
    {
        public Note NewNote { get; private set; }

        public NewNoteWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            NewNote = new Note
            {
                Text = NoteTextBox.Text,
                Type = TypeBox.Text,
                Category = CategoryBox.Text
            };
            DialogResult = true;
            Close();
        }
    }
}
