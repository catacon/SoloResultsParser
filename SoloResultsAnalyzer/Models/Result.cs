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
        public bool _isLadies;
        public bool _isNovice;
        private bool _driverExists;

        // OnPropertyChanged is used so UI can be updated when these properties are changed
        public bool DriverExists
        {
            get
            {
                return _driverExists;
            }

            set
            {
                _driverExists = value;
                OnPropertyChanged("DriverExists");
            }
        }

        public bool IsLadies
        {
            get
            {
                return _isLadies;
            }
            set
            {
                _isLadies = value;
                OnPropertyChanged("IsLadies");
            }
        }

        public bool IsNovice
        {
            get
            {
                return _isNovice;
            }
            set
            {
                _isNovice = value;
                OnPropertyChanged("IsNovice");
            }
        }

        // PropertyChanged event for INotifyPeopertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// OnPropertyChanged for implementation of INotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Result()
        {
            Runs = new List<Run>();
        }
    }
}
