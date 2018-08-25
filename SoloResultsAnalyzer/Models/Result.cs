using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Models
{
    public class Result
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Car { get; set; }
        public string ClassString { get; set; }
        public int ClassId { get; set; }
        public int ClassNumber { get; set; }
        public List<Run> Runs { get; set; }
        public double RawTime { get; set; }
        public double PaxTime { get; set; }
        public bool Ladies { get; set; }
        public bool Novice { get; set; }

        public Result()
        {
            Runs = new List<Run>();
        }
    }
}
