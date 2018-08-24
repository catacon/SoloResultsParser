using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Models
{
    public class Result
    {
        public string FirstName;
        public string LastName;
        public string Car;
        public string ClassString;
        public int ClassId;
        public int ClassNumber;
        public List<Run> Runs;
        public double RawTime;
        public double PaxTime;
        public bool Ladies;
        public bool Novice;

        public Result()
        {
            Runs = new List<Run>();
        }
    }
}
