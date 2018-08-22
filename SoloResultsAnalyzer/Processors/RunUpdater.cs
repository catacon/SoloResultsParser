using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoloResultsAnalyzer
{
    public enum RunPenalty
    {
        None,
        DNF,
        RRN
    }

    public class Run
    {
        public int RunNumber;
        public string FirstName;
        public string LastName;
        public string Car;
        public string ClassString;
        public int ClassId;
        public int ClassNumber;
        public double RawTime;
        public double CorrectedTime;
        public int Cones;
        public RunPenalty Penalty;
    }

    public class Result
    {
        public string FirstName;
        public string LastName;
        public string Car;
        public string ClassString;
        public int ClassId;
        public int ClassNumber;
        public List<Run> Runs;
        public double RawTime;
        public double PaxTime;

        public Result()
        {
            Runs = new List<Run>();
        }
    }

    class RunUpdater
    {
        /// <summary>
        /// Parse data from Pronto CSV file
        /// </summary>
        /// <param name="EventFile">Path to event CSV file</param>
        /// <param name="EventRuns">Output list of runs from the event</param>
        /// <param name="EventResults">Output list of results from the event</param>
        /// <returns>True if file was parsed successfully, false otherwise</returns>
        public bool ParseData(string EventFile, out List<Run> EventRuns, out List<Result> EventResults)
        {
            EventRuns = new List<Run>();
            EventResults = new List<Result>();
            return true;
        }

        /// <summary>
        /// Insert parsed data into season database
        /// </summary>
        /// <param name="Season">Season for the data</param>
        /// <param name="Event">Event for the data</param>
        /// <param name="Database">Path to database</param>
        /// <param name="EventRuns">List of event runs</param>
        /// <param name="EventResults">List of event results</param>
        /// <returns>True if data was inserted successfully, false otherwise</returns>
        public bool CommitData(int Season, int Event, string Database, List<Run> EventRuns, List<Result> EventResults)
        {
            return true;
        }

        public static bool Update(int Season, int Event, string RunFile, string Database)
        {
            // Validate season input
            if (Season <= 0)
            {
                Console.WriteLine(string.Format("Invalid season: {0}", Season));
                return false;
            }

            // Validate event input
            if (Event <= 0)
            {
                Console.WriteLine(string.Format("Invalid event: {0}", Event));
                return false;
            }

            // Attempt to open class list file
            if (!File.Exists(RunFile))
            {
                Console.WriteLine(string.Format("Class List File {0} does not exist!", RunFile));
                return false;
            }

            // Update class database
            try
            {
                using (SqlConnection db = new SqlConnection(string.Format(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={0};Integrated Security=True;Connect Timeout=30", Database)))
                {
                    // Open class list file
                    TextFieldParser Parser = new TextFieldParser(RunFile);
                    Parser.SetDelimiters(",");

                    // Skip the first line
                    Parser.ReadLine();

                    // Open database
                    db.Open();

                    // Read every line of results file and insert data into database
                    while (!Parser.EndOfData)
                    {
                        // 0         1      2        3     4     5            6          7         8           9       10       11     12    13, ...
                        // PAX pos, class, number, fname, lname, car year, car make, car model, car color, best run, pax time, time, cones, penalty, ...
                        string[] Fields = Parser.ReadFields();

                        // Skip entries with no valid runs
                        if (Fields[9] == "DNF")
                        {
                            Console.WriteLine("No valid runs...skipping.");
                            continue;
                        }

                        Result NewResult = new Result();

                        NewResult.ClassString = Fields[1];
                        NewResult.ClassNumber = int.Parse(Fields[2]);
                        NewResult.FirstName = Fields[3];
                        NewResult.LastName = Fields[4];
                        NewResult.Car = string.Format("{0} {1} {2} {3}", Fields[5], Fields[6], Fields[7].Substring(Fields[7].Length - 1) == "/" ? Fields[7].Substring(0, Fields[7].Length - 1) : Fields[7] + " |", Fields[8]);  // Strip slash if car color was not specified

                        // Get the class ID - strip off 'L' from ladies classes
                        String ClassIdQuery = string.Format("SELECT Id FROM Classes WHERE Abbreviation = '{0}'", NewResult.ClassString.Substring(NewResult.ClassString.Length - 1) == "L" ? NewResult.ClassString.Substring(0, NewResult.ClassString.Length - 1) : NewResult.ClassString);

                        using (SqlCommand command = new SqlCommand(ClassIdQuery, db))
                        {
                            try
                            {
                                NewResult.ClassId = (int)command.ExecuteScalar();
                            }
                            catch (Exception ex)
                            {
                                // TODO bail out
                                Console.WriteLine("Unable to get class ID");
                                Console.WriteLine(ex.Message);
                                NewResult.ClassId = 1;
                            }
                        }

                        // Build single run insert query here and update it for each run below
                        String RunInsertQuery = "INSERT INTO Runs (Season,Event,RunNumber,FirstName,LastName,Car,Class,Number,RawTime,Cones,Penalty,Ladies,Novice) VALUES (@Season,@Event,@RunNumber,@FirstName,@LastName,@Car,@Class,@Number,@RawTime,@Cones,@Penalty,@Ladies,@Novice)";

                        // Extract all run data
                        for (int field = 11; field < Fields.Length - 2; field += 3)
                        {
                            Run run = new Run();

                            run.RawTime = double.Parse(Fields[field]);
                            run.Cones = int.Parse(Fields[field + 1]);

                            string penalty = Fields[field + 2];

                            if (penalty == "DNF")
                            {
                                run.Penalty = RunPenalty.DNF;
                                run.CorrectedTime = 999.999;
                            }
                            else if (penalty == "RL")
                            {
                                run.Penalty = RunPenalty.RRN;
                                run.CorrectedTime = 999.999;
                            }
                            else
                            {
                                run.Penalty = RunPenalty.None;
                                run.CorrectedTime = run.RawTime + (2 * run.Cones);
                            }

                            NewResult.Runs.Add(run);

                            // Insert run into database
                            using (SqlCommand command = new SqlCommand(RunInsertQuery, db))
                            {
                                command.Parameters.AddWithValue("@Season", Season);
                                command.Parameters.AddWithValue("@Event", Event);
                                command.Parameters.AddWithValue("@RunNumber", NewResult.Runs.Count);
                                command.Parameters.AddWithValue("@FirstName", NewResult.FirstName);
                                command.Parameters.AddWithValue("@LastName", NewResult.LastName);
                                command.Parameters.AddWithValue("@Car", NewResult.Car);
                                command.Parameters.AddWithValue("@Class", NewResult.ClassId);
                                command.Parameters.AddWithValue("@Number", NewResult.ClassNumber);
                                command.Parameters.AddWithValue("@RawTime", run.RawTime);
                                command.Parameters.AddWithValue("@Cones", run.Cones);
                                command.Parameters.AddWithValue("@Penalty", (int)run.Penalty);
                                command.Parameters.AddWithValue("@Ladies", 0);
                                command.Parameters.AddWithValue("@Novice", 0);

                                // Execute insert command
                                int result = command.ExecuteNonQuery();

                                // Check Error
                                if (result < 0)
                                {
                                    Console.WriteLine("Error inserting data into Database!");
                                    // TODO handle error
                                }
                            }
                        }

                        // Sort runs to find best time
                        NewResult.Runs = NewResult.Runs.OrderBy(x => x.CorrectedTime).ToList();
                        NewResult.RawTime = NewResult.Runs.First().CorrectedTime;

                        // Get the PAX multiplier and apply it
                        String PaxQuery = string.Format("SELECT Multiplier FROM Classes WHERE Id = '{0}'", NewResult.ClassId);

                        using (SqlCommand command = new SqlCommand(PaxQuery, db))
                        {
                            try
                            {
                                Decimal Multiplier = (Decimal)command.ExecuteScalar();
                                NewResult.PaxTime = (NewResult.RawTime * (double)Multiplier);
                            }
                            catch (Exception ex)
                            {
                                // TODO bail out
                                Console.WriteLine("Unable to get class ID");
                                Console.WriteLine(ex.Message);
                            }
                        }

                        // Insert result into database
                        String ResultInsertQuery = "INSERT INTO Results (Season,Event,FirstName,LastName,Car,Class,Number,RawTime,PaxTime,Ladies,Novice) VALUES (@Season,@Event,@FirstName,@LastName,@Car,@Class,@Number,@RawTime,@PaxTime,@Ladies,@Novice)";

                        using (SqlCommand command = new SqlCommand(ResultInsertQuery, db))
                        {
                            command.Parameters.AddWithValue("@Season", Season);
                            command.Parameters.AddWithValue("@Event", Event);
                            command.Parameters.AddWithValue("@FirstName", NewResult.FirstName);
                            command.Parameters.AddWithValue("@LastName", NewResult.LastName);
                            command.Parameters.AddWithValue("@Car", NewResult.Car);
                            command.Parameters.AddWithValue("@Class", NewResult.ClassId);
                            command.Parameters.AddWithValue("@Number", NewResult.ClassNumber);
                            command.Parameters.AddWithValue("@RawTime", NewResult.RawTime);
                            command.Parameters.AddWithValue("@PaxTime", NewResult.PaxTime);
                            command.Parameters.AddWithValue("@Ladies", 0);
                            command.Parameters.AddWithValue("@Novice", 0);

                            // Execute insert command
                            int result = command.ExecuteNonQuery();

                            // Check Error
                            if (result < 0)
                            {
                                Console.WriteLine("Error inserting data into Database!");
                                // TODO handle error
                            }
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return true;
        }
    }
}
