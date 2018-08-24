using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public ICommand Import
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    _nextViewModel = "EventImportViewModel";
                    OnPropertyChanged("nextViewModel");
                });
            }
        }
    }
}
