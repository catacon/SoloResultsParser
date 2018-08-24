using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        // PropertyChanged event for INotifyPeopertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        public string _nextViewModel;

        private string _pageTitle;

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
        }

        public ViewModelBase(string pageTitle)
        {
            _pageTitle = pageTitle;
        }

        // Command for navigating to the home view model
        public ICommand Home
        {
            get
            {
                return SetNextViewModel("Home");
            }
        }

        /// <summary>
        /// OnPropertyChanged for implementation of INotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
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
