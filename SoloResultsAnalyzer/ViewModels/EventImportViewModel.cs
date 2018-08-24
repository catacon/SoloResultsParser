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
        public EventImportViewModel(string pageTitle) : base(pageTitle)
        {
           
        }

        // Begin data import
        public ICommand ImportEventData
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    // Create file browser
                    Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

                    // Set filter for file extension and default file extension 
                    ofd.DefaultExt = ".csv";
                    ofd.Filter = "CSV Files (*.csv)|*.csv";

                    bool? result = ofd.ShowDialog();

                    if (result.HasValue && result.Value == true)
                    {
                        // TODO parse data
                    }
                });
            }
        }

        // Abandon current data import
        public ICommand CancelEventDataImport
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    // TODO cancel data import
                });
            }
        }

        // Commit current data import to database
        public ICommand CommitEventData
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    // TODO save event data
                });
            }
        }
    }
}
