using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoloResultsAnalyzer.Models 
{
    public class Result
    {
        public int EventId { get; set; }
        public string Car { get; set; }
        public string ClassString { get; set; }
        public int ClassId { get; set; }
        public int ClassNumber { get; set; }
        public List<Run> Runs { get; set; }
        public double RawTime { get; set; }
        public double PaxTime { get; set; }
        public Driver DriverInfo { get; set; }

        public Result()
        {
            Runs = new List<Run>();
            DriverInfo = new Driver();
        }
    }
}
