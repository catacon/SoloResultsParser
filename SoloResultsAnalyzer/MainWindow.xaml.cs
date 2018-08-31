using SoloResultsAnalyzer.ViewModels;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace SoloResultsAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Active view model
        public ViewModelBase CurrentViewModel { get; set; }

        // Make view models members so state can persist between view changes
        private ViewModelBase _homeViewModel;
        private ViewModelBase _eventImportViewModel;
        private ViewModelBase _eventReportViewModel;
        private ViewModelBase _championshipReportViewModel;
        private ViewModelBase _driversViewModel;
        private ViewModelBase _newSeasonViewModel;

        // Settings object for managing program state
        public Settings _appSettings = new Settings();

        // Logging object
        public NLog.Logger _appLog;

        // Event data file parser
        private Processors.IFileParser _fileParser = new Processors.ProntoFileParser();

        private Processors.EventCreator _eventCreator;

        //private readonly string _dbConnectionString = string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", @"C:\Users\Aaron\Projects\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
        private readonly string _dbConnectionString = string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", @"F:\Users\ahall\Projects\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");

        // Database connection for event data
        private DbConnection _dbConnection;

        private int _seasonYear = 2018;

        private int _eventNumber = 7;

        public MainWindow()
        {
            InitializeComponent();

            _dbConnection = new SqlConnection(_dbConnectionString);

            _eventCreator = new Processors.EventCreator(_dbConnection);

            // Initialize view models
            _homeViewModel = new HomeViewModel("Home", _seasonYear, _eventCreator);
            _eventImportViewModel = new EventImportViewModel("Import Event Data", _fileParser, _dbConnection, _seasonYear, _eventNumber);
            _eventReportViewModel = new EventReportViewModel("Create Event Reports");
            _championshipReportViewModel = new ChampionshipReportViewModel("Create Championship Reports");
            _driversViewModel = new DriversViewModel("View Drivers", _dbConnection);
            _newSeasonViewModel = new NewSeasonViewModel("Start New Season", _eventCreator);

            // Set initial view model
            CurrentViewModel = _homeViewModel;
            CurrentViewModel.Update();

            // Subscribe to PropertyChanged event for all view models
            _homeViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _eventImportViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _eventReportViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _championshipReportViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _driversViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _newSeasonViewModel.PropertyChanged += _viewModel_PropertyChanged;

            // Set data context to this class
            DataContext = this;

            // Setup log file
            _appLog = NLog.LogManager.GetLogger(GetType().Name);

            // Load setup file
            if (!_appSettings.LoadFromFile(Settings.SettingsPath + Settings.SettingsFile))
            {
                MessageBox.Show("Unable to load settings file.");

                // Create the settings file for future use
                _appSettings.CreateDefaultFile();
            }
        }

        /// <summary>
        /// Handler for current view model's PropertyChanged event. This is mainly used for navigating to a new view
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Event arguments</param>
        private void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the view model has requested a view change, handle it
            if (e.PropertyName == "nextViewModel")
            {
                switch (CurrentViewModel._nextViewModel)
                {
                    case "Home":
                        CurrentViewModel = _homeViewModel;
                        break;
                    case "EventImportViewModel":
                        CurrentViewModel = _eventImportViewModel;
                        break;
                    case "EventReportViewModel":
                        CurrentViewModel = _eventReportViewModel;
                        break;
                    case "ChampionshipReportViewModel":
                        CurrentViewModel = _championshipReportViewModel;
                        break;
                    case "DriversViewModel":
                        CurrentViewModel = _driversViewModel;
                        break;
                    case "NewSeasonViewModel":
                        CurrentViewModel = _newSeasonViewModel;
                        break;
                    default:
                        // Do nothing
                        break;
                }

                CurrentViewModel.Update();

                // Tell the view to update
                OnPropertyChanged("CurrentViewModel");
            }
        }

        // PropertyChanged event for INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged for implementation of INotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}