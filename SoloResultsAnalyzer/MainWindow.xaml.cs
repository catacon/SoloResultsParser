using SoloResultsAnalyzer.ViewModels;
using System.ComponentModel;
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
        private ViewModelBase _currentViewModel;

        // Make view models members so state can persist between view changes
        private ViewModelBase _homeViewModel;
        private ViewModelBase _eventImportViewModel;
        private ViewModelBase _eventReportViewModel;
        private ViewModelBase _championshipReportViewModel;
        private ViewModelBase _newSeasonViewModel;

        // Settings object for managing program state
        public Settings _appSettings = new Settings();

        // Logging object
        public NLog.Logger _appLog;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize view models
            _homeViewModel = new HomeViewModel("Home");
            _eventImportViewModel = new EventImportViewModel("Import Event Data");
            _eventReportViewModel = new EventReportViewModel("Create Event Reports");
            _championshipReportViewModel = new ChampionshipReportViewModel("Create Championship Reports");
            _newSeasonViewModel = new NewSeasonViewModel("Start New Season");

            // Set initial view model
            _currentViewModel = _homeViewModel;

            // Subscribe to PropertyChanged event for all view models
            _homeViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _eventImportViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _eventReportViewModel.PropertyChanged += _viewModel_PropertyChanged;
            _championshipReportViewModel.PropertyChanged += _viewModel_PropertyChanged;
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
                switch (_currentViewModel._nextViewModel)
                {
                    case "Home":
                        _currentViewModel = _homeViewModel;
                        break;
                    case "EventImportViewModel":
                        _currentViewModel = _eventImportViewModel;
                        break;
                    case "EventReportViewModel":
                        _currentViewModel = _eventReportViewModel;
                        break;
                    case "ChampionshipReportViewModel":
                        _currentViewModel = _championshipReportViewModel;
                        break;
                    case "NewSeasonViewModel":
                        _currentViewModel = _newSeasonViewModel;
                        break;
                    default:
                        // Do nothing
                        break;
                }

                // Tell the view to update
                OnPropertyChanged("CurrentViewModel");
            }
        }

        // Current view model - made accessible to UI
        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
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

        /*
        public ICommand OpenFile
        {
            get
            {
                return new DelegateCommand((object context) =>
                {
                    TextFieldParser parser = new TextFieldParser(@"E:\Downloads\Temp\2018AutoXTandS\2018event01results.csv");
                    parser.SetDelimiters(",");

                    // Skip the first line
                    parser.ReadLine();

                    List<Result> Results = new List<Result>();

                    int MaxRuns = 0;

                    while (!parser.EndOfData)
                    {
                        //Process row
                        string[] fields = parser.ReadFields();

                        Result r = new Result();

                        // 0         1      2        3     4     5            6          7         8           9       10       11     12    13, ...
                        // PAX pos, class, number, fname, lname, car year, car make, car model, car color, best run, pax time, time, cones, penalty, ...

                        r.Driver = string.Format("{0} {1}", fields[3], fields[4]);
                        r.Car = string.Format("{0} {1} {2}", fields[5], fields[6], fields[7]);
                        if (fields[8] != "")
                        {
                            r.Car += string.Format(" | {0}", fields[8]);
                        }

                        r.Class = new CarClass() { ClassName = fields[1] };

                        List<double> Times = new List<double>();

                        for (int field = 11; field < fields.Length - 2; field += 3)
                        {
                            Run run = new Run();

                            run.RawTime = double.Parse(fields[field]);
                            run.Cones = int.Parse(fields[field + 1]);

                            string penalty = fields[field + 2];

                            if (penalty == "DNF")
                            {
                                run.Penalty = RunPenalty.DNF;
                                run.CorrectedTime = 999.99;
                            }
                            else if (penalty == "RL")
                            {
                                run.Penalty = RunPenalty.RRN;
                                run.CorrectedTime = 999.99;
                            }
                            else
                            {
                                run.Penalty = RunPenalty.None;
                                run.CorrectedTime = run.RawTime + run.Cones;
                            }

                            r.Runs.Add(run);
                        }

                        if (r.Runs.Count > MaxRuns)
                        {
                            MaxRuns = r.Runs.Count;
                        }

                        if (r.Runs.Count > 0)
                        {
                            r.RawTime = r.Runs.Min(x => x.CorrectedTime);
                            r.PaxTime = r.RawTime * 1.0; // PAX
                            Results.Add(r);
                        }
                    }

                    StreamWriter sw = new StreamWriter(@"E:\Downloads\Temp\2018AutoXTandS\simple_results.csv");

                    // Write header
                    string header = "Driver,Car,Class,Best Time,PAX Time,";

                    for (int i = 0; i < MaxRuns; ++i)
                    {
                        header += string.Format("Run {0},", i + 1);
                    }

                    sw.WriteLine(header);

                    foreach (Result result in Results)
                    {
                        string s = string.Format("{0},{1},{2},{3},{4},", result.Driver, result.Car, result.Class.ClassName,
                            result.RawTime.ToString("##0.000"), result.PaxTime.ToString("##0.000"));

                        foreach (Run run in result.Runs)
                        {
                            string r = "";

                            if (run.Penalty == RunPenalty.None)
                            {
                                r = string.Format("{0}+{1}", run.RawTime.ToString("##0.000"), run.Cones);
                            }
                            else
                            {
                                r = run.Penalty.ToString();
                            }

                            s += string.Format("{0},", r);
                        }

                        sw.WriteLine(s);
                    }

                    sw.Close();
                });
            }
        } 
        */
        public ICommand UpdateClasses
        {
            get
            {
                return new DelegateCommand((object context) =>
                {
                    ClassUpdater.Update(2018, "test.csv", @"A:\Projects\Autocross\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                });
            }
        }

        public ICommand UpdateRuns
        {
            get
            {
                return new DelegateCommand((object context) =>
                {
                    //RunUpdater.Update(2018, 7, @"A:\Projects\Autocross\2018 Results\Event 7\2018event7-Standard.csv", @"A:\Projects\Autocross\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");

                    // Create file browser
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

                    // Set filter for file extension and default file extension 
                    ofd.DefaultExt = ".csv";
                    ofd.Filter = "CSV Files (*.csv)|*.csv";

                    bool? result = ofd.ShowDialog();

                    if (result.HasValue && result.Value == true)
                    {
                        // Parse file

                        //ei = new EventImport();
                        //ei.Show();
                    }

                });
            }
        }

        public ICommand GenerateEventReports
        {
            get
            {
                return new DelegateCommand((object context) =>
                {
                    //ReportBuilder.GenerateEventPaxReport(2018, 7, @"A:\Projects\Autocross\2018 Results\PAX_Results_2018_test.xlsx", @"A:\Projects\Autocross\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                    //ReportBuilder.GenerateEventRawReport(2018, 7, @"A:\Projects\Autocross\2018 Results\Raw_Results_2018_test.xlsx", @"A:\Projects\Autocross\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                    ReportBuilder.GenerateEventClassReport(2018, 7, @"A:\Projects\Autocross\2018 Results\Class_Results_2018_test.xlsx", @"A:\Projects\Autocross\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                    //ReportBuilder.GenerateEventLadiesReport(2018, 7, @"A:\Projects\Autocross\2018 Results\Ladies_Results_2018_test.xlsx", @"F:\Users\ahall\Projects\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                    //ReportBuilder.GenerateEventNoviceReport(2018, 7, @"A:\Projects\Autocross\2018 Results\Novice_Results_2018_test.xlsx", @"F:\Users\ahall\Projects\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                });
            }
        }
    }
}