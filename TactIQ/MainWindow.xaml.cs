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
using TactIQ.Miscellaneous.Factories;
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
        private readonly IProfileEditViewModelFactory _profileEditVmFactory;
        private readonly IAnalysisViewModelFactory _analysisViewModelFactory;
        private readonly IExportViewModelFactory _exportViewModelFactory;
        private readonly IOpponentProfilesViewModelFactory _opponentProfilesViewModelFactory;

        private NavigationService nav;

        public MainWindow()
        {
            InitializeComponent();

            // DB sicherstellen
            DatabaseBuilder.Initialize();

            // VMs & Services instanzieren
            _mainVM = new MainViewModel();
            DataContext = _mainVM;

            _matchRepo = new SqliteMatchRepository();
            _matchEditVmFactory = new MatchEditViewModelFactory(_matchRepo);
            
            _noteRepo = new SqliteNotesRepository();
            _noteEditVmFactory = new NoteEditViewModelFactory(_noteRepo);

            _opponentRepo = new SqliteOpponentRepository();
            _profileEditVmFactory = new ProfileEditViewModelFactory(nav, _matchEditVmFactory, _noteEditVmFactory, _opponentProfilesViewModelFactory, _opponentRepo, _matchRepo, _noteRepo);

            _analysisViewModelFactory = new AnalysisViewModelFactory(_matchRepo, _opponentRepo);
            _exportViewModelFactory = new ExportViewModelFactory(_opponentRepo, _matchRepo, _noteRepo);

            nav = new NavigationService(vm => _mainVM.CurrentViewModel = vm);

            _opponentProfilesViewModelFactory = new OpponentProfilesViewModelFactory(nav, _matchEditVmFactory, _noteEditVmFactory, _profileEditVmFactory, _opponentRepo, _matchRepo, _noteRepo, _opponentProfilesViewModelFactory);

            // Startseite: Gegnerliste
            var opponentVM = _opponentProfilesViewModelFactory.Create();
            _mainVM.CurrentViewModel = opponentVM;

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            string? pageTag = (sender as Button)?.Tag.ToString();
            LoadPage(pageTag ?? "Gegner");
        }

        private void LoadPage(string page)
        {
            switch (page)
            {
                case "Gegner":
                    this.Title = "Gegnerprofile";
                    var opponentVM = _opponentProfilesViewModelFactory.Create();
                    _mainVM.CurrentViewModel = opponentVM;

                    break;
                case "Analyse":
                    this.Title = "Analyse";
                    var analysisVM = _analysisViewModelFactory.Create();
                    _mainVM.CurrentViewModel = analysisVM;
                    break;
                case "Export":
                    this.Title = "Export";
                    var exportVM = _exportViewModelFactory.Create();
                    _mainVM.CurrentViewModel = exportVM;
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