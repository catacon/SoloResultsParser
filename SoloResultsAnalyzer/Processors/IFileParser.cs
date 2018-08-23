using System.Collections.Generic;
using SoloResultsAnalyzer.DataClasses;


namespace SoloResultsAnalyzer.Processors
{
    interface IFileParser
    {
        /// <summary>
        /// Parse event file and populate Run and Results lists
        /// </summary>
        /// <param name="EventFile">Event data file to parse</param>
        /// <param name="Runs">List of runs that will be populated from event file</param>
        /// <param name="Results">List of results that will be populated from event file</param>
        /// <returns>True if file was parsed successfully. False otherwise.</returns>
        bool ParseEventFile(string EventFile, ref List<Run> EventRuns, ref List<Result> EventResults);
    }
}
