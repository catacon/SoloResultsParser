using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using SoloResultsAnalyzer.Processors;
using SoloResultsAnalyzer.Models;

namespace SoloResultsAnalyzer
{
    class RunUpdater
    {
        IFileParser _fileParser;
        SqlConnection _dbConnection;

        private List<Run> _eventRuns = new List<Run>();
        private List<Result> _eventResults = new List<Result>();

        readonly int _defaultClassId = 1;

        public RunUpdater(IFileParser fileParser, string dbPath)
        {
            _fileParser = fileParser;
            _dbConnection = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", dbPath));
        }

        /// <summary>
        /// Import event data into database
        /// </summary>
        /// <param name="Season">Season year for data</param>
        /// <param name="Event">Event number for data</param>
        /// <param name="Database">Path to database in which to store data</param>
        /// <param name="EventFile">Path to event CSV file</param>
        /// <returns>True if file was parsed successfully, false otherwise</returns>
        public bool ParseEventData(int Season, int Event, string Database, string EventFile)
        {
            // Attempt to open class list file
            if (!File.Exists(EventFile))
            {
                Console.WriteLine(string.Format("Event File {0} does not exist!", EventFile));
                return false;
            }

            // Parse event file into runs and results lists
            if (!_fileParser.ParseEventFile(EventFile, ref _eventRuns, ref _eventResults))
            {
                Console.WriteLine("Failed to parse event file {0}", EventFile);
                return false;
            }

            // Verify there is data in the runs and results list
            if (_eventRuns.Count <= 0 || _eventResults.Count <= 0)
            {
                Console.WriteLine("No data in runs or results list: {0}, {1}", _eventRuns.Count, _eventResults.Count);
                return false;
            }

            // Update result and run class IDs
            if (!UpdateClassIDs())
            {
                Console.WriteLine("Failed to update class IDs.");
                return false;
            }

            // Update PAX results
            if (!UpdatePAXResults())
            {
                Console.WriteLine("Failed to update PAX results");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Insert event data into season database
        /// </summary>
        /// <param name="Season">Season for the data</param>
        /// <param name="Event">Event for the data</param>
        /// <param name="Database">Path to database</param>
        /// <returns>True if data was inserted successfully, false otherwise</returns>
        public bool InsertData(int Season, int Event, string Database)
        {
            // Create run insert query
            String RunInsertQuery = "INSERT INTO Runs (Season,Event,RunNumber,FirstName,LastName,Car,Class,Number,RawTime,Cones,Penalty,Ladies,Novice) VALUES (@Season,@Event,@RunNumber,@FirstName,@LastName,@Car,@Class,@Number,@RawTime,@Cones,@Penalty,@Ladies,@Novice)";

            // Create result insert query
            String ResultInsertQuery = "INSERT INTO Results (Season,Event,FirstName,LastName,Car,Class,Number,RawTime,PaxTime,Ladies,Novice) VALUES (@Season,@Event,@FirstName,@LastName,@Car,@Class,@Number,@RawTime,@PaxTime,@Ladies,@Novice)";

            // Open database
            _dbConnection.Open();

            foreach (Result currentResult in _eventResults)
            {               
                // Insert result into database                  
                using (SqlCommand command = new SqlCommand(ResultInsertQuery, _dbConnection))
                {
                    command.Parameters.AddWithValue("@Season", Season);
                    command.Parameters.AddWithValue("@Event", Event);
                    command.Parameters.AddWithValue("@FirstName", currentResult.FirstName);
                    command.Parameters.AddWithValue("@LastName", currentResult.LastName);
                    command.Parameters.AddWithValue("@Car", currentResult.Car);
                    command.Parameters.AddWithValue("@Class", currentResult.ClassId);
                    command.Parameters.AddWithValue("@Number", currentResult.ClassNumber);
                    command.Parameters.AddWithValue("@RawTime", currentResult.RawTime);
                    command.Parameters.AddWithValue("@PaxTime", currentResult.PaxTime);
                    command.Parameters.AddWithValue("@Ladies", currentResult.Ladies ? 1 : 0);
                    command.Parameters.AddWithValue("@Novice", currentResult.Novice ? 1 : 0);

                    // Execute insert command
                    int result = command.ExecuteNonQuery();

                    // Check Error
                    if (result < 0)
                    {
                        Console.WriteLine("Error inserting result data into Database!");
                        // TODO handle error
                    }
                }

                for (int iRun = 0; iRun < currentResult.Runs.Count; ++iRun)
                {
                    // Insert run into database
                    using (SqlCommand command = new SqlCommand(RunInsertQuery, _dbConnection))
                    {
                        command.Parameters.AddWithValue("@Season", Season);
                        command.Parameters.AddWithValue("@Event", Event);
                        command.Parameters.AddWithValue("@RunNumber", iRun + 1);
                        command.Parameters.AddWithValue("@FirstName", currentResult.FirstName);
                        command.Parameters.AddWithValue("@LastName", currentResult.LastName);
                        command.Parameters.AddWithValue("@Car", currentResult.Car);
                        command.Parameters.AddWithValue("@Class", currentResult.ClassId);
                        command.Parameters.AddWithValue("@Number", currentResult.ClassNumber);
                        command.Parameters.AddWithValue("@RawTime", currentResult.Runs[iRun].RawTime);
                        command.Parameters.AddWithValue("@Cones", currentResult.Runs[iRun].Cones);
                        command.Parameters.AddWithValue("@Penalty", (int)currentResult.Runs[iRun].Penalty);
                        command.Parameters.AddWithValue("@Ladies", currentResult.Ladies ? 1 : 0);
                        command.Parameters.AddWithValue("@Novice", currentResult.Novice ? 1 : 0);

                        // Execute insert command
                        int result = command.ExecuteNonQuery();

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

        /// <summary>
        /// Update result class IDs based on class string paresed from event data file
        /// </summary>
        /// <returns>True if all class IDs where updated successfully, false otherwise</returns>
        public bool UpdateClassIDs()
        {
            try
            {
                // Open the database
                _dbConnection.Open();

                foreach (Result currentResult in _eventResults)
                {
                    // Get the class ID - strip off 'L' from ladies classes
                    String ClassIdQuery = string.Format("SELECT Id FROM Classes WHERE Abbreviation = '{0}'", currentResult.ClassString.Substring(currentResult.ClassString.Length - 1) == "L" ? currentResult.ClassString.Substring(0, currentResult.ClassString.Length - 1) : currentResult.ClassString);

                    // Store class ID in results, if no result is found, store the default class
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

                // Close the connection
                _dbConnection.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine("UpdateClassIDs() Exception: {0}", ex.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update the PAX times within each Result
        /// </summary>
        /// <returns>True if results were updated successfully, false otherwise</returns>
        public bool UpdatePAXResults()
        {
            try
            {
                // Open the database
                _dbConnection.Open();

                foreach (Result currentResult in _eventResults)
                {
                    // Get the PAX multiplier and apply it
                    String PaxQuery = string.Format("SELECT Multiplier FROM Classes WHERE Id = '{0}'", currentResult.ClassId);

                    using (SqlCommand command = new SqlCommand(PaxQuery, _dbConnection))
                    {
                        try
                        {
                            Decimal Multiplier = (Decimal)command.ExecuteScalar();
                            currentResult.PaxTime = (currentResult.RawTime * (double)Multiplier);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught while updating PAX: {0}", ex.ToString());
                            Console.WriteLine(ex.Message);
                            return false;
                        }
                    }

                }

                // Close the connection
                _dbConnection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdatePAXResults() Exception: {0}", ex.ToString());
                return false;
            }

            return true;
        }
    }
}
