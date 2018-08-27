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

                    using (DbCommand command = _dbConnection.CreateCommand())
                    {
                        command.CommandText = ClassIdQuery;

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

        public bool SaveData(int seasonYear, int eventNumber)
        {
            // Create result insert query - TODO add runs
            String ResultInsertQuery = "INSERT INTO Results (Season,Event,FirstName,LastName,Car,Class,Number,RawTime,PaxTime,Ladies,Novice) VALUES (@Season,@Event,@FirstName,@LastName,@Car,@Class,@Number,@RawTime,@PaxTime,@Ladies,@Novice)";

            // Open database
            _dbConnection.Open();

            foreach (Models.Result currentResult in _eventResults)
            {
                // Insert result into database                  
                DbCommand command = _dbConnection.CreateCommand();
                
                command.CommandText = ResultInsertQuery;

                Utilities.Extensions.AddParamWithValue(ref command, "@Season", seasonYear);
                Utilities.Extensions.AddParamWithValue(ref command, "@Event", eventNumber);
                Utilities.Extensions.AddParamWithValue(ref command, "@FirstName", currentResult.FirstName);
                Utilities.Extensions.AddParamWithValue(ref command, "@LastName", currentResult.LastName);
                Utilities.Extensions.AddParamWithValue(ref command, "@Car", currentResult.Car);
                Utilities.Extensions.AddParamWithValue(ref command, "@Class", currentResult.ClassId);
                Utilities.Extensions.AddParamWithValue(ref command, "@Number", currentResult.ClassNumber);
                Utilities.Extensions.AddParamWithValue(ref command, "@RawTime", currentResult.RawTime);
                Utilities.Extensions.AddParamWithValue(ref command, "@PaxTime", currentResult.PaxTime);
                Utilities.Extensions.AddParamWithValue(ref command, "@Ladies", currentResult.IsLadies ? 1 : 0);
                Utilities.Extensions.AddParamWithValue(ref command, "@Novice", currentResult.IsNovice ? 1 : 0);
                // TODO insert runs

                // Execute insert command
                int result = command.ExecuteNonQuery();

                // Check Error
                if (result < 0)
                {
                    Console.WriteLine("Error inserting result data into Database!");
                    // TODO handle error
                }
            }

            _dbConnection.Close();

            return true;
        }
    }
}
