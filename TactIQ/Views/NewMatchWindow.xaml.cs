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
    /// Interaktionslogik für NewMatchWindow.xaml
    /// </summary>
    public partial class NewMatchWindow : Window, IDialogCloser 
    {
        public Match NewMatch { get; private set; }

        public NewMatchWindow(INavigationService nav, IMatchRepository repo, Match? existing = null)
        {
            InitializeComponent();
            var vm = new MatchEditViewModel(nav, repo, existing);
            vm.DialogCloser = this; 
            this.DataContext = vm;
        }
        public void Close(bool? dialogResult = true)
        {
            DialogResult = dialogResult;
            Close();
        }
    }
}
