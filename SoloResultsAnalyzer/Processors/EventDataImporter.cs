using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Processors
{
    public class EventDataImporter
    {
        // Parser object for extracting data from event file
        private IFileParser _fileParser;

        // Connection to database in which data will be stored
        private DbConnection _dbConnection;

        // Container for event results
        private List<Models.Result> _eventResults = new List<Models.Result>();

        // Container for event runs
        private List<Models.Run> _eventRuns = new List<Models.Run>();

        // Publicly accessible results container
        public List<Models.Result> EventResults
        {
            get
            {
                return _eventResults;
            }
        }

        // Publicly accessible runs container
        public List<Models.Run> EventRuns
        {
            get
            {
                return _eventRuns;
            }
        }

        // File extension for data files
        public string FileExtension
        {
            get
            {
                return _fileParser.FileExtension;
            }
        }

        // File filter for data files
        public string FileFilter
        {
            get
            {
                return _fileParser.FileExtension;
            }
        }

        public EventDataImporter(IFileParser fileParser, DbConnection dbConnection)
        {
            _fileParser = fileParser ?? throw new ArgumentNullException("fileParser");
            _dbConnection = dbConnection ?? throw new ArgumentNullException("dbConnection");

            _eventResults.Add(new Models.Result() { FirstName = "Aaron", LastName = "Hall" });
        }

        public bool ParseEventData(string eventFile)
        {
            return _fileParser.ParseEventFile(eventFile, ref _eventRuns, ref _eventResults);
        }

        public bool SaveData()
        {
            return true;
        }
    }
}
