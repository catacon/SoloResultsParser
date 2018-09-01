using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int Season { get; set; }
        public int EventNumber { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
    }
}
