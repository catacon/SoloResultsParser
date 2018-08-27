using System.Collections.Generic;
using SoloResultsAnalyzer.Models;


namespace SoloResultsAnalyzer.Processors
{
    public interface IFileParser
    {
        string FileExtension { get; }
        string FileFilter { get; }
        
        /// <summary>
        /// Parse event file and populate Results lists
        /// </summary>
        /// <param name="EventFile">Event data file to parse</param>
        /// <param name="Results">List of results that will be populated from event file</param>
        /// <returns>True if file was parsed successfully. False otherwise.</returns>
        bool ParseEventFile(string EventFile, ref List<Result> EventResults);
    }
}
