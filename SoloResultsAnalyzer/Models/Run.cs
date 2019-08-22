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
        public int EventId;
        public double RawTime;
        public double CorrectedTime;
        public int Cones;
        public RunPenalty Penalty;
    }
}
