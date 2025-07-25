using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TactIQ.Views;

namespace TactIQ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadPage("Gegner");
        }

        private void Sidebar_MouseEnter(object sender, MouseEventArgs e)
        {
            Sidebar.Width = 160;
            Label_Gegner.Visibility = Visibility.Visible;
            Label_Analyse.Visibility = Visibility.Visible;
            Label_Export.Visibility = Visibility.Visible;
        }

        private void Sidebar_MouseLeave(object sender, MouseEventArgs e)
        {
            Sidebar.Width = 60;
            Label_Gegner.Visibility = Visibility.Collapsed;
            Label_Analyse.Visibility = Visibility.Collapsed;
            Label_Export.Visibility = Visibility.Collapsed;
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            string pageTag = (sender as Button)?.Tag.ToString();
            LoadPage(pageTag);
        }

        private void LoadPage(string page)
        {
            switch (page)
            {
                case "Gegner":
                    MainContent.Content = new OpponentProfilesUC(); // Beispielkontrolle
                    break;
                case "Analyse":
                    MainContent.Content = new OpponentProfilesUC(); // Deine Gegnerprofile
                    break;
                case "Export":
                    MainContent.Content = new OpponentProfilesUC(); // Deine Gegnerprofile
                    break;
            }
        }
    }
}