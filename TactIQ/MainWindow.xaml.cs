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
using static TactIQ.Miscellaneous.Interfaces;
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

        private SqliteOpponentRepository _opponentRepo;
        private SqliteMatchRepository _matchRepo;
        private SqliteNotesRepository _noteRepo;
        private readonly IMatchEditViewModelFactory _matchEditVmFactory;
        private readonly INoteEditViewModelFactory _noteEditVmFactory;


        private NavigationService nav;

        public MainWindow()
        {
            InitializeComponent();

            // DB sicherstellen
            DatabaseBuilder.Initialize();

            // VM + Services zusammensetzen
            _mainVM = new MainViewModel();
            DataContext = _mainVM;

            _opponentRepo = new SqliteOpponentRepository();

            _matchRepo = new SqliteMatchRepository();
            _matchEditVmFactory = new MatchEditViewModelFactory(_matchRepo);
            
            _noteRepo = new SqliteNotesRepository();
            _noteEditVmFactory = new NoteEditViewModelFactory(_noteRepo);

            nav = new NavigationService(vm => _mainVM.CurrentViewModel = vm);

            // Startseite: Gegnerliste
            _mainVM.CurrentViewModel = new OpponentProfilesViewModel(nav,_matchEditVmFactory, _noteEditVmFactory, _opponentRepo, _matchRepo, _noteRepo);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
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
                    var opponentVM = new OpponentProfilesViewModel(nav, _matchEditVmFactory, _noteEditVmFactory, _opponentRepo, _matchRepo, _noteRepo);
                    _mainVM.CurrentViewModel = opponentVM;
                    break;
                case "Analyse":
                    this.Title = "Analyse";
                    var analysisVM = new AnalysisViewModel(_matchRepo, _opponentRepo);
                    _mainVM.CurrentViewModel = analysisVM;
                    break;
                case "Export":
                    this.Title = "Export";
                    var vm = new ExportViewModel(_opponentRepo, _matchRepo, _noteRepo);
                    _mainVM.CurrentViewModel = vm;
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