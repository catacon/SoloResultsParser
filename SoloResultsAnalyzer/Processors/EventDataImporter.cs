using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
            if (!ParseEventData(eventFile))
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

        private bool CheckForExistingDrivers()
        {
            return true;
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
            // Open database
            _dbConnection.Open();

            foreach (Models.Result currentResult in _eventResults)
            {
                int resultId = 0;

                // Insert result into database                  
                using (DbCommand resultInsertCommand = CreateResultInsertCommand(currentResult, seasonYear, eventNumber))
                {
                    // Execute insert command
                    resultId = (int)resultInsertCommand.ExecuteScalar();

                    // Check Error
                    if (resultId < 0)
                    {
                        Console.WriteLine("Error inserting result data into Database!");
                        // TODO handle error
                    }
                }

                // Store each run and associate it with the result
                foreach (Models.Run currentRun in currentResult.Runs)
                {
                    using (DbCommand runInsertCommand = CreateRunInsertCommand(currentRun, resultId))
                    {
                        int result = runInsertCommand.ExecuteNonQuery();

                        // Check Error
                        if (result < 0)
                        {
                            Console.WriteLine("Error inserting run data into Database!");
                            // TODO handle error
                        }
                    }
                }
            }

            _dbConnection.Close();

            return true;
        }

        private DbCommand CreateResultInsertCommand(Models.Result result, int seasonYear, int eventNumber)
        {
            string ResultInsertQuery = "INSERT INTO Results (Season,Event,FirstName,LastName,Car,Class,Number,RawTime,PaxTime,IsLadies,IsNovice,Runs) " +
                                    "OUTPUT INSERTED.ID " +
                                    "VALUES (@Season,@Event,@FirstName,@LastName,@Car,@Class,@Number,@RawTime,@PaxTime,@IsLadies,@IsNovice,@Runs)";

            DbCommand resultInsertCommand = _dbConnection.CreateCommand();

            resultInsertCommand.CommandText = ResultInsertQuery;

            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Season", seasonYear);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Event", eventNumber);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@FirstName", result.FirstName);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@LastName", result.LastName);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Car", result.Car);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Class", result.ClassId);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Number", result.ClassNumber);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@RawTime", result.RawTime);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@PaxTime", result.PaxTime);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@IsLadies", result.IsLadies ? 1 : 0);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@IsNovice", result.IsNovice ? 1 : 0);

            return resultInsertCommand;
        }

        private DbCommand CreateRunInsertCommand(Models.Run run, int resultId)
        {
            string RunInsertQuery = "INSERT INTO Runs (RunNumber,RawTime,Cones,Penalty,ResultId) VALUES (@RunNumber,@RawTime,@Cones,@Penalty,@ResultId)";

            DbCommand runInsertCommand = _dbConnection.CreateCommand();

            runInsertCommand.CommandText = RunInsertQuery;

            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@RunNumber", run.RunNumber);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@RawTime", run.RawTime);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@Cones", run.Cones);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@Penalty", run.Penalty);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@ResultId", resultId);

            return runInsertCommand;
        }
    }
}
