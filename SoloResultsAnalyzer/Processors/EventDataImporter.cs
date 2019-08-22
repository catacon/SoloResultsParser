using System;
using System.Collections.Generic;
using System.Data.Common;

namespace SoloResultsAnalyzer.Processors
{
    public class EventDataImporter
    {
        private IFileParser _fileParser;

        private DbConnection _dbConnection;

        private List<Models.Result> _eventResults = new List<Models.Result>();

        // Default ID if class is not found
        private readonly int _defaultClassId = 1;

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

            CheckForExistingDrivers();

            UpdateClassIds();

            return true;
        }

        private bool ParseEventData(string eventFile)
        {
            return _fileParser.ParseEventFile(eventFile, ref _eventResults);
        }

        private void CheckForExistingDrivers()
        {
            foreach (Models.Result result in _eventResults)
            {
                CheckForSingleExistingDriver(result);
            }
        }

        public void CheckForSingleExistingDriver(Models.Result result)
        {
            _dbConnection.Open();

            using (DbCommand driverQueryCommand = CreateDriverCommand(result.DriverInfo.FirstName, result.DriverInfo.LastName))
            {
                var reader = driverQueryCommand.ExecuteReader();

                if (reader.HasRows && reader.Read())
                {
                    result.DriverInfo.DriverExists = true;
                    result.DriverInfo.IsLadies = (bool)reader["IsLadies"];
                    result.DriverInfo.IsNovice = (bool)reader["IsNovice"];
                }
                else
                {
                    result.DriverInfo.DriverExists = false;
                }
            }

            _dbConnection.Close();
        }

        private void UpdateClassIds()
        {
            _dbConnection.Open();

            foreach (Models.Result currentResult in _eventResults)
            {
                // Strip off 'L' from ladies classes
                string classString = currentResult.ClassString.Substring(currentResult.ClassString.Length - 1) == "L" ? currentResult.ClassString.Substring(0, currentResult.ClassString.Length - 1) : currentResult.ClassString;

                using (DbCommand command = CreateClassCommand(classString))
                {
                    try
                    {
                        currentResult.ClassId = (int)command.ExecuteScalar();
                    }
                    catch(Exception)
                    {
                        // TODO log
                        currentResult.ClassId = _defaultClassId;
                    }
                }
            }

            _dbConnection.Close();
        }

        public bool SaveData(int eventId)
        {
            // Open database
            _dbConnection.Open();

            foreach (Models.Result currentResult in _eventResults)
            {
                int resultId = InsertResult(currentResult, eventId);

                InsertRuns(currentResult.Runs, resultId);

                InsertDriver(currentResult.DriverInfo);
            }

            _dbConnection.Close();

            return true;
        }

        private int InsertResult(Models.Result result, int eventId)
        {
            int resultId = 0;

            // Insert result into database                  
            using (DbCommand resultInsertCommand = CreateResultInsertCommand(result, eventId))
            {
                // Execute insert command
                resultId = (int)resultInsertCommand.ExecuteScalar();

                if (resultId < 0)
                {
                    Console.WriteLine("Error inserting result data into Database!");
                    // TODO handle error
                }
            }

            return resultId;
        }

        private void InsertDriver(Models.Driver driver)
        {
            // Insert driver if they do not exist
            if (!driver.DriverExists)
            {
                using (DbCommand driverInsertCommand = CreateDriverInsertCommand(driver))
                {
                    int result = driverInsertCommand.ExecuteNonQuery();

                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting run data into Database!");
                        // TODO handle error
                    }
                }
            }
        }

        private void InsertRuns(List<Models.Run> runs, int resultId)
        {
            // Store each run and associate it with the result
            foreach (Models.Run run in runs)
            {
                using (DbCommand runInsertCommand = CreateRunInsertCommand(run, resultId))
                {
                    int result = runInsertCommand.ExecuteNonQuery();

                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting run data into Database!");
                        // TODO handle error
                    }
                }
            }
        }

        private DbCommand CreateResultInsertCommand(Models.Result result, int eventId)
        {
            DbCommand resultInsertCommand = _dbConnection.CreateCommand();

            resultInsertCommand.CommandText = "INSERT INTO Results (EventId,DriverId,Car,Class,Number,RawTime,PaxTime) " +
                                    "OUTPUT INSERTED.ID " +
                                    "VALUES (@EventId,@DriverId,@Car,@Class,@Number,@RawTime,@PaxTime)";

            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@EventId", eventId);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@DriverId", result.DriverInfo.Id);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Car", result.Car);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Class", result.ClassId);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@Number", result.ClassNumber);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@RawTime", result.RawTime);
            Utilities.Extensions.AddParamWithValue(ref resultInsertCommand, "@PaxTime", result.PaxTime);

            return resultInsertCommand;
        }

        private DbCommand CreateRunInsertCommand(Models.Run run, int resultId)
        {
            DbCommand runInsertCommand = _dbConnection.CreateCommand();

            runInsertCommand.CommandText = "INSERT INTO Runs (RunNumber,RawTime,Cones,Penalty,ResultId) VALUES (@RunNumber,@RawTime,@Cones,@Penalty,@ResultId)";

            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@RunNumber", run.RunNumber);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@RawTime", run.RawTime);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@Cones", run.Cones);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@Penalty", run.Penalty);
            Utilities.Extensions.AddParamWithValue(ref runInsertCommand, "@ResultId", resultId);

            return runInsertCommand;
        }

        // TODO rename
        private DbCommand CreateDriverCommand(string firstName, string lastName)
        {
            DbCommand driverQueryCommand = _dbConnection.CreateCommand();

            driverQueryCommand.CommandText = "SELECT * FROM Drivers WHERE FirstName = @firstName AND LastName = @lastName";

            Utilities.Extensions.AddParamWithValue(ref driverQueryCommand, "firstName", firstName);
            Utilities.Extensions.AddParamWithValue(ref driverQueryCommand, "lastName", lastName);

            return driverQueryCommand;
        }

        private DbCommand CreateClassCommand(string classString)
        {
            DbCommand classQueryCommand = _dbConnection.CreateCommand();

            Utilities.Extensions.AddParamWithValue(ref classQueryCommand, "classString", classString);

            classQueryCommand.CommandText = "SELECT Id FROM Classes WHERE Abbreviation = @classString";

            return classQueryCommand;
        }

        private DbCommand CreateDriverInsertCommand(Models.Driver driverInfo)
        { 
            DbCommand driverInsertQuery = _dbConnection.CreateCommand();

            driverInsertQuery.CommandText = "INSERT INTO Drivers (SeasonId, FirstName, LastName, IsLadies, IsNovice) VALUES (@SeasonId, @FirstName, @LastName, @IsLadies, @IsNovice)"; ;

            Utilities.Extensions.AddParamWithValue(ref driverInsertQuery, "SeasonId", driverInfo.SeasonId);
            Utilities.Extensions.AddParamWithValue(ref driverInsertQuery, "FirstName", driverInfo.FirstName);
            Utilities.Extensions.AddParamWithValue(ref driverInsertQuery, "LastName", driverInfo.LastName);
            Utilities.Extensions.AddParamWithValue(ref driverInsertQuery, "IsLadies", driverInfo.IsLadies);
            Utilities.Extensions.AddParamWithValue(ref driverInsertQuery, "IsNovice", driverInfo.IsNovice);

            return driverInsertQuery;
        }
    }
}
