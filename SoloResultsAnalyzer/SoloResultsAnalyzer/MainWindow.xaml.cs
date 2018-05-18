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
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace SoloResultsAnalyzer
{
    public enum RunPenalty
    {
        None,
        DNF,
        RRN
    }

    public class Run
    {
        public double RawTime;
        public double CorrectedTime;
        public int Cones;
        public RunPenalty Penalty;
    }

    public class CarClass
    {
        public string ClassName;
        public double PaxModifier;
    }

    public class Result
    {
        public string Driver;
        public string Car;
        public CarClass Class;
        public List<Run> Runs;
        public double RawTime;
        public double PaxTime;

        public Result()
        {
            Runs = new List<Run>();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        // Autonomous field cal procedure
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
        }   // End of  ICommand DataWindow_Closing
    }
}
