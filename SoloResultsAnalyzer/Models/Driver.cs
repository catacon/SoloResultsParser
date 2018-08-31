using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Models
{
    public class Driver : INotifyPropertyChanged
    {
        public string _firstName;
        public string _lastName;
        public bool _isLadies;
        public bool _isNovice;
        public bool _driverExists;
        public int Id { get; set; }

        public string FirstName
        {
            get
            {
                return _firstName;
            }
            
            set
            {
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public string LastName
        {
            get
            {
                return _lastName;
            }

            set
            {
                _lastName = value;
                OnPropertyChanged("LastName");
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
    }
}
