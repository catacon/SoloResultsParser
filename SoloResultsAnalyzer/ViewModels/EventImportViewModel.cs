using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SoloResultsAnalyzer.ViewModels
{
    public class EventImportViewModel : ViewModelBase
    {
        public ICommand Home
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    _nextViewModel = "Home";
                    OnPropertyChanged("nextViewModel");
                });
            }
        }
    }
}
