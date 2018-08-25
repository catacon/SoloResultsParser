using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using SoloResultsAnalyzer.Models;

namespace SoloResultsAnalyzer.ViewModels
{
    public class EventImportViewModel : ViewModelBase
    {
        // List for storing imported results
        public List<Result> EventResults { get; set; }

        // List for storing imported runs
        private List<Run> _eventRuns;

        // Flag for indicating data import is active
        private bool _importActive = false;

        // Command for beginning data import
        public ICommand Import
        {
            get
            {
                return new DelegateCommand(o => ImportData(), o => { return !_importActive; });
            }
        }

        // Command to abandon current data import
        public ICommand Cancel
        {
            get
            {
                return new DelegateCommand(o => CancelImport(), o => { return _importActive; });
            }
        }

        // Command to save current data import to database
        public ICommand Save
        {
            get
            {
                return new DelegateCommand(o => SaveData(), o => { return _importActive; });
            }
        }

        public EventImportViewModel(string pageTitle) : base(pageTitle)
        {
            EventResults = new List<Result>();
            EventResults.Add(new Result() { FirstName = "Aaron" });
            EventResults.Add(new Result() { FirstName = "Matt" });
            EventResults.Add(new Result() { FirstName = "Jake" });
            OnPropertyChanged("EventResults");


        }

        /// <summary>
        /// Begin data import process
        /// </summary>
        private void ImportData()
        {
            // Create file browser
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            ofd.DefaultExt = ".csv";
            ofd.Filter = "CSV Files (*.csv)|*.csv";

            bool? result = ofd.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                _importActive = true;
                // TODO parse data
            }
        }

        /// <summary>
        /// Cancel current data import
        /// </summary>
        private void CancelImport()
        {
            _importActive = false;
            // TODO cancel data import
        }

        /// <summary>
        /// Commit current event data to database
        /// </summary>
        private void SaveData()
        {
            // TODO Save data to database
        }
    }
}
