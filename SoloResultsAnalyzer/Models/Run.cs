using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Models
{
    public enum RunPenalty
    {
        None,
        DNF,
        RRN
    }

    public class Run
    {
        public int RunNumber;
        public string FirstName;
        public string LastName;
        public string Car;
        public string ClassString;
        public int ClassId;
        public int ClassNumber;
        public double RawTime;
        public double CorrectedTime;
        public int Cones;
        public RunPenalty Penalty;
        public bool Ladies;
        public bool Novice;
    }
}
