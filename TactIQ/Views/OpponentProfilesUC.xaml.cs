using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TactIQ.ViewModels;

namespace TactIQ.Views
{
    public partial class OpponentProfilesUC : UserControl
    {
        public OpponentProfilesUC()
        {
            InitializeComponent();
        }

        private void ProfileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is OpponentProfilesViewModel vm && vm.OpenSelectedCommand.CanExecute(null))
                vm.OpenSelectedCommand.Execute(null);
        }
    }
}
