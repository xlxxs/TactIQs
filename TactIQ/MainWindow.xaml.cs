using System.ComponentModel;
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
using TactIQ.Miscellaneous;
using TactIQ.Miscellaneous.SQLite;
using TactIQ.Services;
using TactIQ.ViewModels;
using TactIQ.Views;
using NavigationService = TactIQ.Services.NavigationService;

namespace TactIQ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private bool _isSidebarExpanded;
        public bool isSidebarExpanded
        {
            get => _isSidebarExpanded;
            set
            {
                if (_isSidebarExpanded != value)
                {
                    _isSidebarExpanded = value;
                    OnPropertyChanged(nameof(isSidebarExpanded));
                    OnPropertyChanged(nameof(isSidebarCollapsed));
                }
            }
        }

        public bool isSidebarCollapsed => !isSidebarExpanded;
        private readonly MainViewModel _mainVM;

        SqliteOpponentRepository opponentRepo;
        SqliteMatchRepository matchRepo;
        NavigationService nav;

        public MainWindow()
        {
            InitializeComponent();

            // DB sicherstellen
            DatabaseBuilder.Initialize();

            // VM + Services zusammensetzen
            _mainVM = new MainViewModel();
            DataContext = _mainVM;

            opponentRepo = new SqliteOpponentRepository();

            nav = new NavigationService(vm => _mainVM.CurrentViewModel = vm);

            // Startseite: Gegnerliste
            _mainVM.CurrentViewModel = new OpponentProfilesViewModel(nav, opponentRepo);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                    this.Title = "Gegnerprofile";
                    var vm = new OpponentProfilesViewModel(nav, opponentRepo);
                    _mainVM.CurrentViewModel = vm;
                    break;
                case "Analyse":
                    this.Title = "Analyse";
                    MainContent.Content = new OpponentProfilesUC(); 
                    break;
                case "Export":
                    this.Title = "Export";
                    MainContent.Content = new OpponentProfilesUC();
                    break;
                case "Expand":
                    Sidebar.Width = 160;
                    Label_Gegner.Visibility = Visibility.Visible;
                    Label_Analyse.Visibility = Visibility.Visible;
                    Label_Export.Visibility = Visibility.Visible;
                    isSidebarExpanded = true;
                    break;
                case "Reduce":
                    Sidebar.Width = 60;
                    Label_Gegner.Visibility = Visibility.Collapsed;
                    Label_Analyse.Visibility = Visibility.Collapsed;
                    Label_Export.Visibility = Visibility.Collapsed;
                    isSidebarExpanded = false;
                    break;
            }
        }
    }
}