﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using SoloResultsAnalyzer.Models;

namespace SoloResultsAnalyzer.Processors
{
    public class ProntoFileParser : IFileParser
    {
        // Default file extension for Pronto data files
        public string FileExtension { get; }

        // Default file filter for Pronto data files
        public string FileFilter { get; }

        // Number of seconds added to raw time for each cone hit
        readonly int _TimePenaltyForCone = 2;

        // Minimum number of fields per line from Pronto CSV file to be considered a valid line
        readonly int _ProntoCsvMinFields = 11;

        // Number of header lines in Pronto CSV file
        readonly int _ProntoCsvHeaderLines = 1;

        public ProntoFileParser()
        {
            FileExtension = ".csv";
            FileFilter = "CSV Files (*.csv)|*.csv";
        }

        /// <summary>
        /// Parse Pronto CSV event file and populate Results lists
        /// </summary>
        /// <param name="EventFile">Pronto CSV file to parse</param>
        /// <param name="Results">List of results that will be populated from event file</param>
        /// <returns>True if file was parsed successfully. False otherwise.</returns>
        public bool ParseEventFile(string EventFile, ref List<Result> Results)
        {
            // Initialize output lists
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
                NewResult.DriverInfo.FirstName = Fields[3];
                NewResult.DriverInfo.LastName = Fields[4];
                NewResult.Car = string.Format("{0} {1} {2} {3}", Fields[5], Fields[6], Fields[7].Substring(Fields[7].Length - 1) == "/" ? Fields[7].Substring(0, Fields[7].Length - 1) : Fields[7] + " |", Fields[8]);  // Strip slash if car color was not specified

                int RunNumber = 1;

                // Extract all run data
                // Each run has a time, penalty, and cones - process each triplet then move to the next triplet
                for (int field = _ProntoCsvMinFields; field < Fields.Length - 2; field += 3)
                {
                    Run run = new Run();

                    run.RunNumber = RunNumber++;
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
                        // Pronto abbreviation for Rerun is RRN, but it stores them as RL (redlight?)
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

                NewResult.RawTime = double.Parse(Fields[9]);
                NewResult.PaxTime = double.Parse(Fields[10]);
                Results.Add(NewResult);
            }

            return true;
        }
    }
}
