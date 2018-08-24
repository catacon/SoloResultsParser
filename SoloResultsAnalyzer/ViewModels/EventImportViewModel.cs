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
        public EventImportViewModel()
        {
            _pageTitle = "Import Event Results";
        }

        public ICommand Home
        {
            get
            {
                return SetNextViewModel("Home");
            }
        }
    }
}
