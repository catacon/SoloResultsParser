using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SoloResultsAnalyzer.DataClasses;

namespace SoloResultsAnalyzer.Processors
{
    class ProntoFileParser : IFileParser
    {
        // Number of seconds added to raw time for each cone hit
        readonly int _TimePenaltyForCone = 2;

        // Minimum number of fields per line from Pronto CSV file to be considered a valid line
        readonly int _ProntoCsvMinFields = 11;

        // Number of header lines in Pronto CSV file
        readonly int _ProntoCsvHeaderLines = 1;

        /// <summary>
        /// Parse Pronto CSV event file and populate Run and Results lists
        /// </summary>
        /// <param name="EventFile">Pronto CSV file to parse</param>
        /// <param name="Runs">List of runs that will be populated from event file</param>
        /// <param name="Results">List of results that will be populated from event file</param>
        /// <returns>True if file was parsed successfully. False otherwise.</returns>
        public bool ParseEventFile(string EventFile, ref List<Run> Runs, ref List<Result> Results)
        {
            // Initialize output lists
            Runs.Clear();
            Results.Clear();

            // Verify event file exists
            if (!File.Exists(EventFile))
            {
                Console.WriteLine("Failed to open file {0}", EventFile);
                return false;
            }

            // Open event file
            TextFieldParser Parser = new TextFieldParser(EventFile);
            Parser.SetDelimiters(",");

            // Skip the first line
            Parser.ReadLine();

            // Read each line of results and populate output lists
            while (!Parser.EndOfData)
            {
                // TODO define fields
                // 0         1      2        3     4     5            6          7         8           9       10       11     12    13, ...
                // PAX pos, class, number, fname, lname, car year, car make, car model, car color, best run, pax time, time, cones, penalty, ...
                string[] Fields = Parser.ReadFields();

                // TODO define min lines
                if (Fields.Length < _ProntoCsvMinFields)
                {
                    Console.WriteLine("Failed to read line {0}", Parser.LineNumber);
                    continue;
                }

                // Skip entries with no valid runs
                if (Fields[9] == "DNF")
                {
                    Console.WriteLine("No valid runs...skipping.");
                    continue;
                }

                // Create new result
                Result NewResult = new Result();

                NewResult.ClassString = Fields[1];
                NewResult.ClassNumber = int.Parse(Fields[2]);
                NewResult.FirstName = Fields[3];
                NewResult.LastName = Fields[4];
                NewResult.Car = string.Format("{0} {1} {2} {3}", Fields[5], Fields[6], Fields[7].Substring(Fields[7].Length - 1) == "/" ? Fields[7].Substring(0, Fields[7].Length - 1) : Fields[7] + " |", Fields[8]);  // Strip slash if car color was not specified

                // Extract all run data
                // Each run has a time, penalty, and cones - process each triplet then move to the next triplet
                for (int field = _ProntoCsvMinFields; field < Fields.Length - 2; field += 3)
                {
                    Run run = new Run();

                    run.RawTime = double.Parse(Fields[field]);
                    run.Cones = int.Parse(Fields[field + 1]);

                    string penalty = Fields[field + 2];

                    // Set corrected time based on penalties and cones
                    if (penalty == "DNF")
                    {
                        run.Penalty = RunPenalty.DNF;
                        run.CorrectedTime = 999.999;
                    }
                    else if (penalty == "RL")
                    {
                        // Pronto abbreviate for Rerun is RRN, but it stores them as RL (redlight?)
                        run.Penalty = RunPenalty.RRN;
                        run.CorrectedTime = 999.999;
                    }
                    else
                    {
                        run.Penalty = RunPenalty.None;
                        run.CorrectedTime = run.RawTime + (_TimePenaltyForCone * run.Cones);
                    }

                    NewResult.Runs.Add(run);
                }

                // Sort runs to find best corrected time
                NewResult.Runs = NewResult.Runs.OrderBy(x => x.CorrectedTime).ToList();
                NewResult.RawTime = NewResult.Runs.First().CorrectedTime;
            }

            return true;
        }
    }
}
