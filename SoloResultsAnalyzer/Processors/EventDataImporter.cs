using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer.Processors
{
    public class EventDataImporter
    {
        private IFileParser _fileParser;

        private DbConnection _dbConnection;

        private List<Models.Result> _eventResults = new List<Models.Result>();

        // Default ID if class is not found
        readonly int _defaultClassId = 1;

        public List<Models.Result> EventResults
        {
            get
            {
                return _eventResults;
            }

            set
            {
                _eventResults = value;
            }
        }

        public string FileExtension
        {
            get
            {
                return _fileParser.FileExtension;
            }
        }

        public string FileFilter
        {
            get
            {
                return _fileParser.FileFilter;
            }
        }

        public EventDataImporter(IFileParser fileParser, DbConnection dbConnection)
        {
            _fileParser = fileParser ?? throw new ArgumentNullException("fileParser");
            _dbConnection = dbConnection ?? throw new ArgumentNullException("dbConnection");

        }

        public bool ImportEventData(string eventFile)
        {
            if (!_fileParser.ParseEventFile(eventFile, ref _eventResults))
            {
                // TODO log
                return false;
            }

            if (!UpdateClassIds())
            {
                // TODO log
                return false;
            }

            return true;
        }

        private bool ParseEventData(string eventFile)
        {
            return _fileParser.ParseEventFile(eventFile, ref _eventResults);
        }

        private bool UpdateClassIds()
        {
            try
            {
                _dbConnection.Open();

                foreach (Models.Result currentResult in _eventResults)
                {
                    // Get the class ID - strip off 'L' from ladies classes
                    string ClassIdQuery = string.Format("SELECT Id FROM Classes WHERE Abbreviation = '{0}'", currentResult.ClassString.Substring(currentResult.ClassString.Length - 1) == "L" ? currentResult.ClassString.Substring(0, currentResult.ClassString.Length - 1) : currentResult.ClassString);

                    using (SqlCommand command = new SqlCommand(ClassIdQuery, _dbConnection))
                    {
                        try
                        {
                            currentResult.ClassId = (int)command.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Unable to get class ID");
                            Console.WriteLine("Exception caught while getting class ID: {0}", ex.Message);
                            currentResult.ClassId = _defaultClassId;
                            return false;
                        }
                    }

                }

                _dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateClassIDs() Exception: {0}", ex.ToString());
                return false;
            }

            return true;
        }

        public bool SaveData()
        {
            return true;
        }
    }
}
