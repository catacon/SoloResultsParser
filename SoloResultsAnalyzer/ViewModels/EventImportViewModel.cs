using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SoloResultsAnalyzer.Models;
using SoloResultsAnalyzer.Processors;

namespace SoloResultsAnalyzer.ViewModels
{
    public class EventImportViewModel : ViewModelBase
    {
        private EventDataImporter _dataImporter;

        private bool _importActive = false;

        private int _seasonYear;

        private int _eventNumber;

        private Result _selectedResult;

        public List<Result> EventResults
        {
            get
            {
                return _dataImporter.EventResults;
            }

            set
            {
                _dataImporter.EventResults = value;
            }
        }

        public Result SelectedResult
        {
            get
            {
                return _selectedResult;
            }
            
            set
            {
                _selectedResult = value;
            }
        }

        public ICommand Import
        {
            get
            {
                return new DelegateCommand(o => PromptUserForDataFile(), o => { return !_importActive; });
            }
        }

        public ICommand Cancel
        {
            get
            {
                return new DelegateCommand(o => CancelImport(), o => { return _importActive; });
            }
        }

        public ICommand Save
        {
            get
            {
                return new DelegateCommand(o => SaveData(), o => { return _importActive; });
            }
        }

        public ICommand CellChangedCommand
        {
            get
            {
                return new DelegateCommand(o =>
                {
                    _dataImporter.CheckForSingleExistingDriver(_selectedResult);
                });
            }
        }

        public EventImportViewModel(string pageTitle, IFileParser fileParser, DbConnection dbConnection, int seasonYear, int eventNumber) : base(pageTitle)
        {
            _dataImporter = new EventDataImporter(fileParser, dbConnection);
            _seasonYear = seasonYear;
            _eventNumber = eventNumber;
        }

        // TODO better name?
        private void PromptUserForDataFile()
        {
            // Create file browser with filters
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog() { DefaultExt = _dataImporter.FileExtension, Filter = _dataImporter.FileFilter };

            bool? result = ofd.ShowDialog();

            // If user selected a file, attempt to import the data
            if (result.HasValue && result.Value == true)
            {
                if (ImportData(ofd.FileName))
                {
                    MessageBox.Show("Data import successful! Please make necessary edits to data and then save data to database");
                }
                else
                {
                    MessageBox.Show("Data import failed! Please verify data file format.");
                }
            }
        }

        // TODO async
        private bool ImportData(string eventFile)
        {
            _importActive = true;

            if (_dataImporter.ImportEventData(eventFile))
            {
                OnPropertyChanged("EventResults");
                return true;
            }
            else
            {
                _importActive = false;
                return false;
            }

        }

        private void CancelImport()
        {
            // TODO verify cancel
            _importActive = false;
            EventResults.Clear();
            OnPropertyChanged("EventResults");
        }

        private void SaveData()
        {
            if (_dataImporter.SaveData(_seasonYear, _eventNumber))
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
