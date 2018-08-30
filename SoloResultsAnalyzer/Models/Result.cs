using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SoloResultsAnalyzer.Models 
{
    public class Result : INotifyPropertyChanged
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
        public bool IsLadies { get; set; }
        public bool IsNovice { get; set; }
        private bool _driverExists;

        // NotifyPropertyChanged is used here since DriverExists is used as a flag to update the UI
        public bool DriverExists
        {
            get
            {
                return _driverExists;
            }

            set
            {
                _driverExists = value;
                NotifyPropertyChanged();
            }
        }

        // PropertyChanged event for INotifyPeopertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Result()
        {
            Runs = new List<Run>();
        }
    }
}
