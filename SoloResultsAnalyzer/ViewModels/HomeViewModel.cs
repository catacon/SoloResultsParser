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
        public HomeViewModel()
        {
            _pageTitle = "Home";
        }

        public ICommand Import
        {
            get
            {
                return SetNextViewModel("EventImportViewModel");
            }
        }
    }
}
