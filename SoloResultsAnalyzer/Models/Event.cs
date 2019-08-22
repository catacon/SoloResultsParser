using System;

namespace SoloResultsAnalyzer.Models
{
    public class Event
    {
        public int Id { get; set; }
        public int SeasonId { get; set; }
        public int EventNumber { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public bool Points { get; set; }
    }
}
