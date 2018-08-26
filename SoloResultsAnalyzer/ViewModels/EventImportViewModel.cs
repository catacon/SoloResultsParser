using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SoloResultsAnalyzer.Models;
using SoloResultsAnalyzer.Processors;

namespace SoloResultsAnalyzer.ViewModels
{
    public class EventImportViewModel : ViewModelBase
    {
        // Processor for parsing and inserting event data
        private Processors.EventDataImporter _dataImporter;

        // Flag for indicating data import is active
        private bool _importActive = false;

        // Publicly available event results
        public List<Result> EventResults
        {
            get
            {
                return _dataImporter.EventResults;
            }
        }

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

        public EventImportViewModel(string pageTitle, IFileParser fileParser, DbConnection dbConnection) : base(pageTitle)
        {
            _dataImporter = new EventDataImporter(fileParser, dbConnection);
        }

        /// <summary>
        /// Begin data import process
        /// </summary>
        private void ImportData()
        {
            // Create file browser
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            ofd.DefaultExt = _dataImporter.FileExtension;
            ofd.Filter = _dataImporter.FileFilter;

            bool? result = ofd.ShowDialog();

            if (result.HasValue && result.Value == true)
            {
                _importActive = true;
                if (_dataImporter.ParseEventData(ofd.FileName))
                {
                    MessageBox.Show("Data import successful! Please make necessary edits to data and then save data to database");
                }
                else
                {
                    MessageBox.Show("Data import failed! Please verify data file format.");
                    _importActive = false;
                }
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
            if (_dataImporter.SaveData())
            {
                MessageBox.Show("Event data saved to database successfully!");
            }
            else
            {
                MessageBox.Show("Failed to save event data to database!");
            }

            _importActive = false;
        }
    }
}
