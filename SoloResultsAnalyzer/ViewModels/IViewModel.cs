using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string _nextViewModel;

        protected string _pageTitle;

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
        }

        public void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the _nextViewModel property.  This will be used by the main view to switch to a new view model
        /// </summary>
        /// <param name="nextViewModel">Name of next view model to be used</param>
        /// <returns>DelegateCommand that will set next view model and inform main view of change</returns>
        public DelegateCommand SetNextViewModel(string nextViewModel)
        {
            return new DelegateCommand(o =>
            {
                _nextViewModel = nextViewModel;
                OnPropertyChanged("nextViewModel");
            });
        }
    }
}
