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
    /// Interaktionslogik für OpponentProfilesUC.xaml
    /// </summary>
    public partial class OpponentProfilesUC : UserControl
    {
        public OpponentProfilesUC()
        {
            InitializeComponent();
        }

        private void ProfileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProfileList.SelectedItem is ListBoxItem item)
            {
                string selectedName = item.Content.ToString();

                var editUC = new ProfileEditUC();

                editUC.tb_OpponentName.Text = "Gegnerprofil " + selectedName; 

                if (Window.GetWindow(this) is MainWindow main)
                {
                    main.MainContent.Content = editUC;
                }
            }
        }
    }
}
