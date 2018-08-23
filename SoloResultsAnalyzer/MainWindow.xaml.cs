using System.Windows;
using System.Windows.Input;

namespace SoloResultsAnalyzer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Setup log file
            AppLog = NLog.LogManager.GetLogger(GetType().Name);

            // Load setup file
            if (!AppSettings.LoadFromFile(Settings.SettingsPath + Settings.SettingsFile))
            {
                MessageBox.Show("Unable to load settings file.");

                // Create the settings file for future use
                AppSettings.CreateDefaultFile();
            }
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

                        EventImport ei = new EventImport();
                        ei.Show();
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

        public ICommand UpdateChampionshipReports
        {
            get
            {
                return new DelegateCommand((object context) =>
                {
                    ReportBuilder.GenerateEventRawReport(2018, 6, @"A:\Projects\Autocross\2018 Results\Raw_Results_2018_test.xlsx", @"A:\Projects\Autocross\SoloResultsParser\SoloResultsAnalyzer\SoloResults.mdf");
                });
            }
        }

        public Settings AppSettings = new Settings();
        public NLog.Logger AppLog;
        public int CurrentSeason
        {
            get
            {
                return AppSettings.CurrentSeason;
            }
        }
    }
}