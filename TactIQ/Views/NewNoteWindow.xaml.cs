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
using TactIQ.ViewModels;
using static TactIQ.Miscellaneous.Interfaces;

namespace TactIQ.Views
{
    /// <summary>
    /// Interaktionslogik für NewNoteWindow.xaml
    /// </summary>
    public partial class NewNoteWindow : Window, IDialogCloser 
    {
        public Note NewNote { get; private set; }

        public NewNoteWindow(INavigationService nav, INoteRepository repo, Note? existing = null)
        {
            var vm = new NoteEditViewModel(nav, repo, existing);
            vm.DialogCloser = this;
            this.DataContext = vm;
            InitializeComponent();
        }

        public void Close(bool? dialogResult = true)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}
